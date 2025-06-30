using Entities;
using System.Threading.Tasks;
using StackExchange.Redis;
using Models.RequestModels;
using System;

public class LoginService : ILoginService
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IConnectionMultiplexer _redis;

    public LoginService(IUserService userService, ITokenService tokenService, IConnectionMultiplexer redis)
    {
        _userService = userService;
        _tokenService = tokenService;
        _redis = redis;
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var user = _userService.ValidateUser(email, password);
        if (user == null)
            throw new Exception("Kullanıcı bulunamadı");

        // Access ve refresh token üret
        var (accessToken, refreshToken, jti, created, expiry) = _tokenService.GenerateJwtWithRefreshToken(user);

        // Token'ı Redis'e kaydet
        var redisDb = _redis.GetDatabase();
        var redisKey = $"active-token:{jti}";
        await redisDb.StringSetAsync(redisKey, user.Id.ToString(), expiry - created);

        // token, device ve user bilgilerini db'ye kaydet

        return new LoginResponse
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Created = created,
            Expiry = expiry
        };
    }
}