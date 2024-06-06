using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HttpServer.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HttpServer.Services;

public class TokenService : ITokenService
{
    public string GenerateToken(string id)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AppConfiguration.GetJwtOptions().EncryptionKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var userClaims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, id)
        };

        var securityToken = new JwtSecurityToken(
            claims: userClaims,
            notBefore: DateTime.Now,
            expires: null,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public string? GetId(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is JwtSecurityToken jwtSecurityToken)
        {
            return jwtSecurityToken.Claims
                .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?
                .Value;
        }

        return null;
    }
}