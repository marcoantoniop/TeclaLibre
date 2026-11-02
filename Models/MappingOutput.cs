using System.Text.Json.Serialization;

namespace TeclaLibre.Models;

public sealed class MappingOutput
{
    [JsonRequired]
    public OutputKind Kind { get; set; } = OutputKind.Text;

    [JsonRequired]
    public string Text { get; set; } = string.Empty;

    [JsonRequired]
    public int VirtualKey { get; set; }

    [JsonRequired]
    public KeyModifiers Modifiers { get; set; }
        = KeyModifiers.None;
}