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
    private readonly IConfiguration _configuration;

    public AuthService(RsaKeyGenerator rsaKeyGenerator, IConfiguration configuration)
    {
        _rsaKeyGenerator = rsaKeyGenerator ?? throw new Exception($"{nameof(RsaKeyGenerator)} is required");
        _configuration = configuration ?? throw new Exception($"{nameof(IConfiguration)} is required");
    }
    
    public string Login(LoginDto loginDto)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, loginDto.Email),
            new Claim("kid", _configuration["Jwt:KeyId"]),
        };
        var rsaKey = _rsaKeyGenerator.GetRsaKey();
        var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);
        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(12),
            notBefore: DateTime.Now,
            signingCredentials: signingCredentials,
            audience: _configuration["Audience"],
            issuer: _configuration["Issuer"]
        );
        var jws = new JwtSecurityTokenHandler().WriteToken(jwt);

        return jws;
    }
}
