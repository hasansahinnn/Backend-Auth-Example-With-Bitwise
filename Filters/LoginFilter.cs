using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using Exceptions;

public class LoginFilter : IAsyncActionFilter
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IWorkContext _workContext;
    private readonly IUserService _userService;
    private readonly IActionService _actionService;
    private readonly ITokenService _tokenService;

    public LoginFilter(IConnectionMultiplexer redis, IWorkContext workContext, IUserService userService, IActionService actionService, ITokenService tokenService)
    {
        _redis = redis;
        _workContext = workContext;
        _userService = userService;
        _actionService = actionService;
        _tokenService = tokenService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Eğer [Ignore] attribute'u varsa, filtreyi atla
        if (HasIgnoreAttribute(context))
        {
            await next();
            return;
        }

        // Authorization header'ından token'ı al
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token == null)
        {
            context.Result = new ObjectResult(new { message = "Token bulunamadı." }) { StatusCode = 401 };
            return;
        }

        try
        {
            // Token'ı doğrula ve çözümle
            var jwtToken = _tokenService.ValidateAndReadJwt(token);

            // JTI ve UserId claim'lerini al
            var jti = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var userId = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

            // Redis'te token aktif mi kontrol et
            var db = _redis.GetDatabase();
            if (!await db.KeyExistsAsync($"active-token:{jti}"))
            {
                context.Result = new ObjectResult(new { message = "Token geçersiz veya oturum sonlandırılmış." }) { StatusCode = 401 };
                return;
            }

            // Kullanıcı bilgisini WorkContext'e ata
            _workContext.CurrentUserId = int.Parse(userId);
            // kullanının device(versiyon & os type) ve dil bilgileri de header üzerinden alınıp servis tarafına taşınabilir
            try
            {
                // Yetki kontrolü (aksiyon bazlı)
                CheckCustomAuthorization(context);
            }
            catch (ForbiddenException ex)
            {
                context.Result = new ObjectResult(new { message = ex.Message }) { StatusCode = 403 };
                return;
            }

            // Filtreyi geçenler için devam et
            await next();
        }
        catch
        {
            // Token hatalıysa
            context.Result = new ObjectResult(new { message = "Geçersiz token." }) { StatusCode = 401 };
        }
    }

    // [Ignore] attribute'u var mı kontrolü
    private bool HasIgnoreAttribute(ActionExecutingContext context) =>
        ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttributes(typeof(IgnoreAttribute), true).Any();

    // Aksiyon bazlı yetki kontrolü
    private void CheckCustomAuthorization(ActionExecutingContext context)
    {
        var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
        var securityAttribute = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(SecurityActionAttribute), true).FirstOrDefault() as SecurityActionAttribute;
        if (securityAttribute == null) return; // Eğer SecurityAction yoksa, kontrol etme

        int requiredControllerId = securityAttribute.ControllerId;
        long requiredActionId = securityAttribute.ActionId;

        // Kullanıcının ilgili controller için sahip olduğu aksiyonlar
        var userPermissions = _actionService.GetUserPermissions(_workContext.CurrentUserId);
        if (!userPermissions.TryGetValue(requiredControllerId, out long userTotalActionForController))
        {
            // Controller'a erişim yok
            throw new ForbiddenException("Bu alana erişim yetkiniz bulunmamaktadır.");
        }
        if ((userTotalActionForController & requiredActionId) != requiredActionId)
        {
            // İstenen aksiyona yetki yok
            throw new ForbiddenException("Bu işlemi yapmak için yetkiniz bulunmamaktadır.");
        }
    }
}