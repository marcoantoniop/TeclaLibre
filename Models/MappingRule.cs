using System.Text.Json.Serialization;

namespace TeclaLibre.Models;

public sealed class MappingRule
{
    [JsonRequired]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonRequired]
    public string Name { get; set; } = string.Empty;

    [JsonRequired]
    public bool Enabled { get; set; } = true;

    [JsonRequired]
    public KeyTrigger Trigger { get; set; } = new();

    [JsonRequired]
    public MappingOutput Output { get; set; } = new();

    [JsonRequired]
    public RepeatMode RepeatMode { get; set; }
        = RepeatMode.OncePerPress;
}