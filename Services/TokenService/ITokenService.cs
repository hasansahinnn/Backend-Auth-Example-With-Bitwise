using Entities;
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;

public interface ITokenService
{
    (string accessToken, string refreshToken, string jti, DateTime created, DateTime expiry) GenerateJwtWithRefreshToken(User user);
    JwtSecurityToken ValidateAndReadJwt(string token);
    Task<bool> IsTokenActive(string jti);
}