using System.Runtime.InteropServices;
using TeclaLibre.Models;
using TeclaLibre.Native;

namespace TeclaLibre.Keyboard;

internal sealed class InputSender
{
    public void SendText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length == 0)
        {
            return;
        }

        EnsureModifiersReleased();

        List<INPUT> inputs = new(text.Length * 2);

        // Cada char representa una unidad UTF-16.
        // Los caracteres suplementarios se envían como
        // sus dos unidades sustitutas, en orden.
        foreach (char character in text)
        {
            ushort codeUnit = character;

            inputs.Add(CreateUnicodeInput(
                codeUnit,
                keyUp: false));

            inputs.Add(CreateUnicodeInput(
                codeUnit,
                keyUp: true));
        }

        Send(inputs);
    }

    public void SendKey(int virtualKey)
    {
        ValidateActionKey(virtualKey);
        EnsureModifiersReleased();
        EnsureKeyReleased(virtualKey);

        List<INPUT> inputs =
        [
            CreateKeyInput(virtualKey, keyUp: false),
            CreateKeyInput(virtualKey, keyUp: true)
        ];

        Send(inputs);
    }

    public void SendShortcut(
        int virtualKey,
        KeyModifiers modifiers)
    {
        ValidateActionKey(virtualKey);
        ValidateModifiers(modifiers);

        EnsureModifiersReleased();
        EnsureKeyReleased(virtualKey);

        List<int> modifierKeys = BuildModifierKeys(modifiers);
        List<INPUT> inputs = new();

        // Presionar modificadores.
        foreach (int modifierKey in modifierKeys)
        {
            inputs.Add(CreateKeyInput(
                modifierKey,
                keyUp: false));
        }

        // Presionar y soltar la tecla principal.
        inputs.Add(CreateKeyInput(
            virtualKey,
            keyUp: false));

        inputs.Add(CreateKeyInput(
            virtualKey,
            keyUp: true));

        // Soltar modificadores en orden inverso.
        for (int i = modifierKeys.Count - 1; i >= 0; i--)
        {
            inputs.Add(CreateKeyInput(
                modifierKeys[i],
                keyUp: true));
        }

        Send(inputs);
    }

    private static List<int> BuildModifierKeys(
        KeyModifiers modifiers)
    {
        List<int> keys = new();

        bool hasControl =
            (modifiers & KeyModifiers.Control) != 0;

        bool hasShift =
            (modifiers & KeyModifiers.Shift) != 0;

        bool hasAlt =
            (modifiers & KeyModifiers.Alt) != 0;

        bool hasWindows =
            (modifiers & KeyModifiers.Windows) != 0;

        bool hasAltGr =
            (modifiers & KeyModifiers.AltGr) != 0;

        // AltGr se genera como Ctrl izquierdo + Alt derecho.
        if (hasControl || hasAltGr)
        {
            keys.Add(NativeConstants.VK_LCONTROL);
        }

        if (hasShift)
        {
            keys.Add(NativeConstants.VK_LSHIFT);
        }

        if (hasAlt)
        {
            keys.Add(NativeConstants.VK_LMENU);
        }

        if (hasAltGr)
        {
            keys.Add(NativeConstants.VK_RMENU);
        }

        if (hasWindows)
        {
            keys.Add(NativeConstants.VK_LWIN);
        }

        return keys;
    }

    private static INPUT CreateUnicodeInput(
        ushort codeUnit,
        bool keyUp)
    {
        uint flags = NativeConstants.KEYEVENTF_UNICODE;

        if (keyUp)
        {
            flags |= NativeConstants.KEYEVENTF_KEYUP;
        }

        return new INPUT
        {
            Type = NativeConstants.INPUT_KEYBOARD,

            Data = new InputUnion
            {
                Keyboard = new KEYBDINPUT
                {
                    VirtualKey = 0,
                    ScanCode = codeUnit,
                    Flags = flags,
                    Time = 0,
                    ExtraInfo = NativeConstants.TeclaLibreExtraInfo
                }
            }
        };
    }

    private static INPUT CreateKeyInput(
        int virtualKey,
        bool keyUp)
    {
        uint flags = 0;

        if (IsExtendedKey(virtualKey))
        {
            flags |= NativeConstants.KEYEVENTF_EXTENDEDKEY;
        }

        if (keyUp)
        {
            flags |= NativeConstants.KEYEVENTF_KEYUP;
        }

        return new INPUT
        {
            Type = NativeConstants.INPUT_KEYBOARD,

            Data = new InputUnion
            {
                Keyboard = new KEYBDINPUT
                {
                    VirtualKey = checked((ushort)virtualKey),
                    ScanCode = 0,
                    Flags = flags,
                    Time = 0,
                    ExtraInfo = NativeConstants.TeclaLibreExtraInfo
                }
            }
        };
    }

    private static bool IsExtendedKey(int virtualKey)
    {
        return virtualKey switch
        {
            NativeConstants.VK_RCONTROL => true,
            NativeConstants.VK_RMENU => true,
            NativeConstants.VK_LWIN => true,
            NativeConstants.VK_RWIN => true,

            0x21 => true, // Page Up
            0x22 => true, // Page Down
            0x23 => true, // End
            0x24 => true, // Home
            0x25 => true, // Flecha izquierda
            0x26 => true, // Flecha arriba
            0x27 => true, // Flecha derecha
            0x28 => true, // Flecha abajo
            0x2D => true, // Insert
            0x2E => true, // Delete
            0x6F => true, // División del teclado numérico
            0x90 => true, // Num Lock

            _ => false
        };
    }

    private static void ValidateActionKey(int virtualKey)
    {
        if (virtualKey < 1 || virtualKey > 254)
        {
            throw new ArgumentOutOfRangeException(
                nameof(virtualKey),
                "El código de tecla debe estar entre 1 y 254.");
        }

        if (IsModifierKey(virtualKey))
        {
            throw new ArgumentException(
                "La tecla principal debe ser una tecla no modificadora.",
                nameof(virtualKey));
        }
    }

    private static bool IsModifierKey(int virtualKey)
    {
        return virtualKey is
            NativeConstants.VK_SHIFT or
            NativeConstants.VK_CONTROL or
            NativeConstants.VK_MENU or
            NativeConstants.VK_LSHIFT or
            NativeConstants.VK_RSHIFT or
            NativeConstants.VK_LCONTROL or
            NativeConstants.VK_RCONTROL or
            NativeConstants.VK_LMENU or
            NativeConstants.VK_RMENU or
            NativeConstants.VK_LWIN or
            NativeConstants.VK_RWIN;
    }

    private static void ValidateModifiers(
        KeyModifiers modifiers)
    {
        const KeyModifiers allowed =
            KeyModifiers.Control |
            KeyModifiers.Shift |
            KeyModifiers.Alt |
            KeyModifiers.Windows |
            KeyModifiers.AltGr;

        if ((modifiers & ~allowed) != 0)
        {
            throw new ArgumentException(
                "La combinación contiene modificadores desconocidos.",
                nameof(modifiers));
        }

        // AltGr ya utiliza Ctrl izquierdo. Evitamos representar
        // una combinación ambigua con Ctrl adicional.
        if ((modifiers & KeyModifiers.AltGr) != 0 &&
            (modifiers & KeyModifiers.Control) != 0)
        {
            throw new ArgumentException(
                "Por ahora no se admite Control adicional junto a AltGr.",
                nameof(modifiers));
        }
    }

    private static void EnsureModifiersReleased()
    {
        int[] modifierKeys =
        [
            NativeConstants.VK_LSHIFT,
            NativeConstants.VK_RSHIFT,
            NativeConstants.VK_LCONTROL,
            NativeConstants.VK_RCONTROL,
            NativeConstants.VK_LMENU,
            NativeConstants.VK_RMENU,
            NativeConstants.VK_LWIN,
            NativeConstants.VK_RWIN
        ];

        foreach (int key in modifierKeys)
        {
            if (IsKeyDown(key))
            {
                throw new InvalidOperationException(
                    "Suelta Ctrl, Shift, Alt, AltGr y Windows " +
                    "antes de ejecutar esta prueba.");
            }
        }
    }

    private static void EnsureKeyReleased(int virtualKey)
    {
        if (IsKeyDown(virtualKey))
        {
            throw new InvalidOperationException(
                "Suelta la tecla de salida antes de ejecutar la prueba.");
        }
    }

    private static bool IsKeyDown(int virtualKey)
    {
        return (NativeMethods.GetAsyncKeyState(virtualKey) & 0x8000) != 0;
    }

    private static void Send(List<INPUT> inputList)
    {
        if (inputList.Count == 0)
        {
            return;
        }

        INPUT[] inputs = inputList.ToArray();
        int inputSize = Marshal.SizeOf<INPUT>();

        uint sent = NativeMethods.SendInput(
            checked((uint)inputs.Length),
            inputs,
            inputSize);

        if (sent == (uint)inputs.Length)
        {
            return;
        }

        // Capturar el error antes de cualquier otra llamada nativa.
        int nativeError = Marshal.GetLastWin32Error();

        // Si Windows aceptó solo una parte de la secuencia,
        // intentar liberar las teclas que quedaron presionadas.
        bool cleanupSucceeded = TryReleasePartialSequence(
            inputs,
            sent,
            inputSize);

        string cleanupMessage = cleanupSucceeded
            ? "Se intentó liberar cualquier tecla pendiente."
            : "No se pudieron liberar todas las entradas pendientes.";

        throw new InvalidOperationException(
            $"Windows aceptó {sent} de {inputs.Length} eventos. " +
            $"Código de error informado: {nativeError}. " +
            cleanupMessage);
    }

    private static bool TryReleasePartialSequence(
        INPUT[] inputs,
        uint sent,
        int inputSize)
    {
        Stack<INPUT> pressedInputs = new();

        int acceptedCount = (int)Math.Min(
            sent,
            (uint)inputs.Length);

        for (int i = 0; i < acceptedCount; i++)
        {
            INPUT input = inputs[i];

            bool isKeyUp =
                (input.Data.Keyboard.Flags &
                 NativeConstants.KEYEVENTF_KEYUP) != 0;

            if (!isKeyUp)
            {
                pressedInputs.Push(input);
            }
            else if (pressedInputs.Count > 0)
            {
                pressedInputs.Pop();
            }
        }

        if (pressedInputs.Count == 0)
        {
            return true;
        }

        List<INPUT> releases = new();

        while (pressedInputs.Count > 0)
        {
            INPUT release = pressedInputs.Pop();

            release.Data.Keyboard.Flags |=
                NativeConstants.KEYEVENTF_KEYUP;

            releases.Add(release);
        }

        INPUT[] releaseArray = releases.ToArray();

        uint released = NativeMethods.SendInput(
            checked((uint)releaseArray.Length),
            releaseArray,
            inputSize);

        return released == (uint)releaseArray.Length;
    }
}