using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Differ.Models;

/// <summary>
/// Basic input type for controllers.
/// </summary>
public record InputType
{
    [Required]
    [MaxLength(2000)]
    [JsonPropertyName("input")] 
    public string Input { get; set; } = string.Empty;
}