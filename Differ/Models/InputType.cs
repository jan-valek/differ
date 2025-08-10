using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Differ.Models;

public record InputType
{
    [Required]
    [JsonPropertyName("input")] 
    public string Input { get; set; } = string.Empty;
}