using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Entities;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string accessToken, string refreshToken, string jti, DateTime created, DateTime expiry) GenerateJwtWithRefreshToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jti = Guid.NewGuid().ToString();
        var created = DateTime.UtcNow;
        var expiry = created.AddMinutes(30); // appsettings.json'da ayarlanabilir
        var refreshExpiry = created.AddYears(1);

        // Access Token Claims
        var accessClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim("typ", "access")
        };

        var accessToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            claims: accessClaims,
            notBefore: created,
            expires: expiry,
            signingCredentials: creds
        );

        // Refresh Token Claims
        var refreshClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("typ", "refresh")
        };

        var refreshToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            claims: refreshClaims,
            notBefore: created,
            expires: refreshExpiry,
            signingCredentials: creds
        );

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
        var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);

        return (accessTokenString, refreshTokenString, jti, created, expiry);
    }


    public async Task<bool> IsTokenActive(string jti)
    {
        return false;
    }

    public JwtSecurityToken ValidateAndReadJwt(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        tokenHandler.ValidateToken(token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);
        return (JwtSecurityToken)validatedToken;
    }
}