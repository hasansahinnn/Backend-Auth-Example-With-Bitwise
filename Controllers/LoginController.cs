using Microsoft.AspNetCore.Mvc;
using RequestModels;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var response = await _loginService.LoginAsync(model.Email, model.Password);
        if (response == null)
            return Unauthorized("Kullanıcı adı veya şifre hatalı.");
        return Ok(response);
    }
}