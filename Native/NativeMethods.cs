using System.Runtime.InteropServices;

namespace TeclaLibre.Native;

internal static class NativeMethods
{
    [DllImport(
    "user32.dll",
    ExactSpelling = true)]
    internal static extern nint GetForegroundWindow();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate nint LowLevelKeyboardProc(
        int nCode,
        nuint wParam,
        nint lParam);

    [DllImport(
        "user32.dll",
        EntryPoint = "SetWindowsHookExW",
        ExactSpelling = true,
        SetLastError = true)]
    internal static extern nint SetWindowsHookEx(
        int idHook,
        LowLevelKeyboardProc callback,
        nint moduleHandle,
        uint threadId);

    [DllImport(
        "user32.dll",
        ExactSpelling = true,
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(
        nint hookHandle);

    [DllImport(
        "user32.dll",
        ExactSpelling = true)]
    internal static extern nint CallNextHookEx(
        nint hookHandle,
        int nCode,
        nuint wParam,
        nint lParam);

    [DllImport(
        "user32.dll",
        ExactSpelling = true,
        SetLastError = true)]
    internal static extern uint SendInput(
        uint inputCount,
        [In] INPUT[] inputs,
        int inputSize);

    [DllImport(
        "user32.dll",
        ExactSpelling = true)]
    internal static extern short GetAsyncKeyState(
        int virtualKey);

    [DllImport(
        "kernel32.dll",
        EntryPoint = "GetModuleHandleW",
        CharSet = CharSet.Unicode,
        ExactSpelling = true,
        SetLastError = true)]
    internal static extern nint GetModuleHandle(
        string? moduleName);

    [DllImport(
    "user32.dll",
    ExactSpelling = true,
    SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(
    nint windowHandle,
    int id,
    uint modifiers,
    uint virtualKey);

    [DllImport(
        "user32.dll",
        ExactSpelling = true,
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(
        nint windowHandle,
        int id);
}