using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DailyDevOps.Auth.Model;
using DailyDevOps.Auth.Util;
using Microsoft.IdentityModel.Tokens;

namespace DailyDevOps.Auth.Service;


public interface IAuthService
{
    string Login(LoginDto loginDto);
}

public class AuthService : IAuthService
{
    private readonly RsaKeyGenerator _rsaKeyGenerator;

    public AuthService(RsaKeyGenerator rsaKeyGenerator)
        => _rsaKeyGenerator = rsaKeyGenerator ?? throw new Exception($"{nameof(RsaKeyGenerator)} is required");
    
    public string Login(LoginDto loginDto)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, loginDto.Email),
            new Claim("kid", "123456"),
        };
        var rsaKey = _rsaKeyGenerator.GetRsaKey();
        var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);
        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(12),
            notBefore: DateTime.Now,
            signingCredentials: signingCredentials,
            audience: "daily-devops",
            issuer: "https://57ic2cwid5.execute-api.us-east-1.amazonaws.com/auth"
        );
        var jws = new JwtSecurityTokenHandler().WriteToken(jwt);

        return jws;
    }
}
