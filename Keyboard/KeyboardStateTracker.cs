using TeclaLibre.Models;
using TeclaLibre.Native;

namespace TeclaLibre.Keyboard;

internal sealed class KeyboardStateTracker
{
    private readonly HashSet<int> _pressedKeys = new();

    public IReadOnlySet<int> PressedKeys => _pressedKeys;

    public KeyModifiers CurrentModifiers =>
        AltGrNormalizer.GetModifiers(_pressedKeys);

    /// <summary>
    /// Actualiza el estado y devuelve true si el evento
    /// es una repetición de una tecla ya presionada.
    /// </summary>
    public bool Process(KeyboardEvent keyboardEvent)
    {
        int key = NormalizeVirtualKey(keyboardEvent);

        if (keyboardEvent.EventType == KeyboardEventType.KeyDown)
        {
            // Add devuelve false si la tecla ya estaba en el conjunto.
            bool wasAdded = _pressedKeys.Add(key);

            return !wasAdded;
        }

        _pressedKeys.Remove(key);

        return false;
    }

    public void Reset()
    {
        _pressedKeys.Clear();
    }

    private static int NormalizeVirtualKey(
        KeyboardEvent keyboardEvent)
    {
        int key = keyboardEvent.VirtualKey;

        if (key == NativeConstants.VK_CONTROL)
        {
            return keyboardEvent.IsExtended
                ? NativeConstants.VK_RCONTROL
                : NativeConstants.VK_LCONTROL;
        }

        if (key == NativeConstants.VK_MENU)
        {
            return keyboardEvent.IsExtended
                ? NativeConstants.VK_RMENU
                : NativeConstants.VK_LMENU;
        }

        if (key == NativeConstants.VK_SHIFT)
        {
            // En los scan codes habituales de Windows:
            // 0x36 corresponde a Shift derecho.
            return keyboardEvent.ScanCode == 0x36
                ? NativeConstants.VK_RSHIFT
                : NativeConstants.VK_LSHIFT;
        }

        return key;
    }
}