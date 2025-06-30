using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

// WorkContext
builder.Services.AddScoped<IWorkContext, WorkContext>();

// UserService & ActionService
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IActionService, ActionService>();

// LoginFilter
builder.Services.AddScoped<LoginFilter>();

// TokenService
builder.Services.AddSingleton<ITokenService, TokenService>();

// LoginService
builder.Services.AddScoped<ILoginService, LoginService>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();