namespace TeclaLibre.Models;

[Flags]
public enum KeyModifiers
{
    None = 0,
    Control = 1,
    Shift = 2,
    Alt = 4,
    Windows = 8,
    AltGr = 16
}

public enum OutputKind
{
    Text = 0,
    Key = 1,
    Shortcut = 2
}

public enum RepeatMode
{
    OncePerPress = 0,
    RepeatWhileHeld = 1
}

public enum ApplicationState
{
    Stopped = 0,
    Active = 1,
    Paused = 2,
    Testing = 3,
    Error = 4
}

public enum KeyboardEventType
{
    KeyDown = 0,
    KeyUp = 1
}