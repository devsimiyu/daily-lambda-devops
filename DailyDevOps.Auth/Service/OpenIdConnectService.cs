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

    public OpenIdConnectService(RsaKeyGenerator rsaKeyGenerator, IServer server)
    {
        _rsaKeyGenerator = rsaKeyGenerator ?? throw new Exception($"{nameof(RsaKeyGenerator)} is required");
        _server = server ?? throw new Exception($"{nameof(IServer)} is required");
    }

    public OpenIdConnectConfiguration GetConfiguration()
    {
        var host = "https://57ic2cwid5.execute-api.us-east-1.amazonaws.com/auth";
        var config = new OpenIdConnectConfiguration
        {
            JwksUri = $"{host}/.well-known/jwks.json",
            Issuer = host
        };

        return config;
    }

    public object GetJwks()
    {
        var rsaKey = _rsaKeyGenerator.GetRsaKey();
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaKey);
        var json = JsonSerializer.Serialize(jwk);
        var keys = new { keys = new object[] { JsonSerializer.Deserialize<object>(json) }};

        return keys;
    }
}
