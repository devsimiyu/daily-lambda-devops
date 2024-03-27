using System.Text.Json;
using DailyDevOps.Auth.Model;
using DailyDevOps.Auth.Util;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.IdentityModel.Tokens;

namespace DailyDevOps.Auth.Service;

public interface IOpenIdConnectService
{
    OpenIdConnectConfiguration GetConfiguration();
    object GetJwks();
}

public class OpenIdConnectService : IOpenIdConnectService
{
    private readonly RsaKeyGenerator _rsaKeyGenerator;
    private readonly IServer _server;
    private readonly IConfiguration _configuration;

    public OpenIdConnectService(RsaKeyGenerator rsaKeyGenerator, IServer server, IConfiguration configuration)
    {
        _rsaKeyGenerator = rsaKeyGenerator ?? throw new Exception($"{nameof(RsaKeyGenerator)} is required");
        _server = server ?? throw new Exception($"{nameof(IServer)} is required");
        _configuration = configuration ?? throw new Exception($"{nameof(IConfiguration)} is required");
    }

    public OpenIdConnectConfiguration GetConfiguration()
    {
        var issuer = _configuration["Issuer"];
        var config = new OpenIdConnectConfiguration
        {
            JwksUri = $"{issuer}/.well-known/jwks.json",
            Issuer = issuer
        };

        return config;
    }

    public object GetJwks()
    {
        var jwks = new 
        {
            keys = new object[]
            {
                new {
                    alg = "RSA256",
                    kty = "RSA",
                    use = "sig",
                    kid = _configuration["Jwt:KeyId"],
                    x5c = new string[] { _configuration["Jwt:PublicKey"] }
                }
            }
        };

        return jwks;
    }
}
