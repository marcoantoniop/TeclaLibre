namespace TeclaLibre.Native;

internal static class NativeConstants
{
    // Tipo de hook y código de evento procesable.
    internal const int WH_KEYBOARD_LL = 13;
    internal const int HC_ACTION = 0;

    // Mensajes de teclado.
    internal const int WM_KEYDOWN = 0x0100;
    internal const int WM_KEYUP = 0x0101;
    internal const int WM_SYSKEYDOWN = 0x0104;
    internal const int WM_SYSKEYUP = 0x0105;

    // Marcas de un evento recibido por el hook.
    internal const uint LLKHF_EXTENDED = 0x00000001;
    internal const uint LLKHF_LOWER_IL_INJECTED = 0x00000002;
    internal const uint LLKHF_INJECTED = 0x00000010;
    internal const uint LLKHF_ALTDOWN = 0x00000020;
    internal const uint LLKHF_UP = 0x00000080;

    // Tipo de entrada para SendInput.
    internal const uint INPUT_KEYBOARD = 1;

    // Opciones para generar eventos de teclado.
    internal const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    internal const uint KEYEVENTF_KEYUP = 0x0002;
    internal const uint KEYEVENTF_UNICODE = 0x0004;
    internal const uint KEYEVENTF_SCANCODE = 0x0008;

    // Modificadores genéricos.
    internal const int VK_SHIFT = 0x10;
    internal const int VK_CONTROL = 0x11;
    internal const int VK_MENU = 0x12;

    // Modificadores izquierdo y derecho.
    internal const int VK_LWIN = 0x5B;
    internal const int VK_RWIN = 0x5C;
    internal const int VK_LSHIFT = 0xA0;
    internal const int VK_RSHIFT = 0xA1;
    internal const int VK_LCONTROL = 0xA2;
    internal const int VK_RCONTROL = 0xA3;
    internal const int VK_LMENU = 0xA4;
    internal const int VK_RMENU = 0xA5;

    // Entrada de texto Unicode.
    internal const int VK_PACKET = 0xE7;

    // Marca que utilizaremos para reconocer nuestras propias entradas.
    internal static readonly nuint TeclaLibreExtraInfo = 0x4B4D4147u;

    // Atajo global de Windows.
    internal const int WM_HOTKEY = 0x0312;

    internal const uint MOD_CONTROL = 0x0002;
    internal const uint MOD_SHIFT = 0x0004;
    internal const uint MOD_NOREPEAT = 0x4000;

    internal const uint VK_F9 = 0x78;

    // Contemplar la tecla Escape como un modificador para permitir su captura en el diálogo de captura de teclas.
    internal const int VK_ESCAPE = 0x1B;
}