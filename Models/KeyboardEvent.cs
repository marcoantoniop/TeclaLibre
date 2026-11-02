namespace TeclaLibre.Models;

public sealed record KeyboardEvent(
    int VirtualKey,
    uint ScanCode,
    KeyboardEventType EventType,
    bool IsExtended,
    bool IsInjected,
    bool IsOwnInjected,
    uint Timestamp
);