using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DailyDevOps.Auth.Model;

public record class LoginDto
{
    [JsonPropertyName("email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
