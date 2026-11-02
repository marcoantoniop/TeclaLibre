using TeclaLibre.Models;
using TeclaLibre.Native;

namespace TeclaLibre.Keyboard;

internal static class AltGrNormalizer
{
    internal static KeyModifiers GetModifiers(
        IReadOnlySet<int> pressedKeys)
    {
        KeyModifiers modifiers = KeyModifiers.None;

        bool leftControl =
            pressedKeys.Contains(NativeConstants.VK_LCONTROL);

        bool rightControl =
            pressedKeys.Contains(NativeConstants.VK_RCONTROL);

        bool leftAlt =
            pressedKeys.Contains(NativeConstants.VK_LMENU);

        bool rightAlt =
            pressedKeys.Contains(NativeConstants.VK_RMENU);

        bool shift =
            pressedKeys.Contains(NativeConstants.VK_LSHIFT) ||
            pressedKeys.Contains(NativeConstants.VK_RSHIFT);

        bool windows =
            pressedKeys.Contains(NativeConstants.VK_LWIN) ||
            pressedKeys.Contains(NativeConstants.VK_RWIN);

        if (shift)
        {
            modifiers |= KeyModifiers.Shift;
        }

        if (windows)
        {
            modifiers |= KeyModifiers.Windows;
        }

        if (leftAlt)
        {
            modifiers |= KeyModifiers.Alt;
        }

        if (rightAlt)
        {
            modifiers |= KeyModifiers.AltGr;
        }

        // Al usar AltGr, Windows puede acompañar Alt derecho
        // con un evento de Ctrl izquierdo.
        if (rightControl || (leftControl && !rightAlt))
        {
            modifiers |= KeyModifiers.Control;
        }

        return modifiers;
    }
}