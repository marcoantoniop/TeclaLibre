using System.Text.Json.Serialization;

namespace TeclaLibre.Models;

public sealed class KeyTrigger
{
    [JsonRequired]
    public int VirtualKey { get; set; }

    [JsonRequired]
    public KeyModifiers Modifiers { get; set; }
        = KeyModifiers.None;

    [JsonRequired]
    public bool ExactMatch { get; set; } = true;
}