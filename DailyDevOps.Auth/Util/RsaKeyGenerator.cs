using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace DailyDevOps.Auth.Util;

public class RsaKeyGenerator
{
    private readonly RSA _rsa = RSA.Create();

    public RsaKeyGenerator(IConfiguration configuration)
    {
        var xml = configuration["Jwt:XmlKey"] ?? throw new Exception("Jwt:XmlKey configuration is required");
        _rsa.KeySize = 2048;

        _rsa.FromXmlString(xml);
    }

    public RsaSecurityKey GetRsaKey() => new RsaSecurityKey(_rsa.ExportParameters(true))
    {
        KeyId = "123456"
    };
}
