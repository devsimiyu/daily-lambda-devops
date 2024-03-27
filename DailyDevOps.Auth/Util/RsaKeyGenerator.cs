using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace DailyDevOps.Auth.Util;

public class RsaKeyGenerator
{
    private readonly RSA _rsa = RSA.Create();
    private readonly IConfiguration _configuration;

    public RsaKeyGenerator(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new Exception($"{nameof(IConfiguration)} is required");

        _rsa.ImportFromPem(_configuration["Jwt:PrivateKey"]);
    }

    public RsaSecurityKey GetRsaKey() => new RsaSecurityKey(_rsa)
    {
        KeyId = _configuration["Jwt:KeyId"]
    };
}
