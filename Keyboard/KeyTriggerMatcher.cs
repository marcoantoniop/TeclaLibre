using TeclaLibre.Models;

namespace TeclaLibre.Keyboard;

internal static class KeyTriggerMatcher
{
    internal static bool Matches(
        KeyTrigger trigger,
        KeyboardEvent keyboardEvent,
        KeyModifiers currentModifiers)
    {
        ArgumentNullException.ThrowIfNull(trigger);
        ArgumentNullException.ThrowIfNull(keyboardEvent);

        if (keyboardEvent.EventType != KeyboardEventType.KeyDown)
        {
            return false;
        }

        if (trigger.VirtualKey == 0 ||
            trigger.VirtualKey != keyboardEvent.VirtualKey)
        {
            return false;
        }

        if (trigger.ExactMatch)
        {
            return currentModifiers == trigger.Modifiers;
        }

        return (currentModifiers & trigger.Modifiers)
            == trigger.Modifiers;
    }
}