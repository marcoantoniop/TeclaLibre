using System.Text.Json.Serialization;

namespace TeclaLibre.Models;

public sealed class AppSettings
{
    [JsonRequired]
    public bool StartWithWindows { get; set; } = false;

    [JsonRequired]
    public bool StartMinimized { get; set; } = false;

    [JsonRequired]
    public bool ActivateOnStartup { get; set; } = false;

    [JsonRequired]
    public bool CloseToTray { get; set; } = true;

    [JsonRequired]
    public KeyTrigger? PauseShortcut { get; set; }
}