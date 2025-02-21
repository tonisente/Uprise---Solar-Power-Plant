using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Uprise___Solar_Power_Plant.Models;

namespace Uprise___Solar_Power_Plant.Security;

public class JWTTokenGenerator
{
    public string GenerateJWTToken(IConfiguration config, User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config.GetValue<string>("AppSettings:JWT:Secret")));

        var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: config.GetValue<string>("AppSettings:JWT:Issuer"),
            audience: config.GetValue<string>("AppSettings:JWT:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(config.GetValue<int>("AppSettings:JWT:ExpiresInMin")),
            signingCredentials: signingCredential
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
