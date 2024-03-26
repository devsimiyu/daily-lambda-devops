using System.Text.Json.Serialization;

namespace DailyDevOps.Auth.Model;

public record OpenIdConnectConfiguration
{
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = string.Empty;

    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = string.Empty;
}
