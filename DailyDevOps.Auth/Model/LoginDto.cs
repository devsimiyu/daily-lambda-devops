using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DailyDevOps.Auth.Model;

public record LoginDto
{
    [JsonPropertyName("email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
