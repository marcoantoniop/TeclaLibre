using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TeclaLibre.Native;

internal static class NativeLayoutValidator
{
    internal static void Validate()
    {
        bool is64Bit = IntPtr.Size == 8;

        CheckSize<KBDLLHOOKSTRUCT>(is64Bit ? 24 : 20);
        CheckSize<KEYBDINPUT>(is64Bit ? 24 : 16);
        CheckSize<MOUSEINPUT>(is64Bit ? 32 : 24);
        CheckSize<HARDWAREINPUT>(8);
        CheckSize<INPUT>(is64Bit ? 40 : 28);

        CheckOffset<INPUT>(
            nameof(INPUT.Data),
            is64Bit ? 8 : 4);

        CheckOffset<KEYBDINPUT>(
            nameof(KEYBDINPUT.ExtraInfo),
            is64Bit ? 16 : 12);

        Debug.WriteLine(
            $"TeclaLibre: estructuras nativas verificadas. " +
            $"Proceso de {IntPtr.Size * 8} bits. " +
            $"INPUT = {Marshal.SizeOf<INPUT>()} bytes.");
    }

    private static void CheckSize<T>(int expected)
        where T : struct
    {
        int actual = Marshal.SizeOf<T>();

        if (actual != expected)
        {
            throw new InvalidOperationException(
                $"Tamaño incorrecto de {typeof(T).Name}: " +
                $"se esperaban {expected} bytes y se obtuvieron {actual}.");
        }
    }

    private static void CheckOffset<T>(
        string fieldName,
        int expected)
        where T : struct
    {
        int actual = Marshal.OffsetOf<T>(fieldName).ToInt32();

        if (actual != expected)
        {
            throw new InvalidOperationException(
                $"Alineación incorrecta de {typeof(T).Name}.{fieldName}: " +
                $"se esperaba la posición {expected} y se obtuvo {actual}.");
        }
    }
}