using System.ComponentModel;
using System.Runtime.InteropServices;
using TeclaLibre.Models;
using TeclaLibre.Native;

namespace TeclaLibre.Keyboard;

internal sealed class KeyboardHookService : IDisposable
{
    // Conservamos el delegado mientras el hook esté instalado.
    private readonly NativeMethods.LowLevelKeyboardProc _callback;

    private nint _hookHandle;
    private int _ownerThreadId;
    private bool _disposed;

    public KeyboardHookService()
    {
        _callback = HookCallback;
    }

    public event Action<KeyboardEvent>? KeyboardEventReceived;

    public bool IsRunning => _hookHandle != nint.Zero;

    public Func<KeyboardEvent, bool>? ShouldSuppress { get; set; }


    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (IsRunning)
        {
            return;
        }

        nint moduleHandle = NativeMethods.GetModuleHandle(null);

        if (moduleHandle == nint.Zero)
        {
            throw new Win32Exception(
                Marshal.GetLastWin32Error(),
                "No se pudo obtener el módulo del proceso.");
        }

        nint hookHandle = NativeMethods.SetWindowsHookEx(
            NativeConstants.WH_KEYBOARD_LL,
            _callback,
            moduleHandle,
            0);

        if (hookHandle == nint.Zero)
        {
            throw new Win32Exception(
                Marshal.GetLastWin32Error(),
                "No se pudo instalar el hook del teclado.");
        }

        _ownerThreadId = Environment.CurrentManagedThreadId;
        _hookHandle = hookHandle;
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        if (Environment.CurrentManagedThreadId != _ownerThreadId)
        {
            throw new InvalidOperationException(
                "El hook debe detenerse desde el hilo que lo instaló.");
        }

        if (!NativeMethods.UnhookWindowsHookEx(_hookHandle))
        {
            throw new Win32Exception(
                Marshal.GetLastWin32Error(),
                "No se pudo retirar el hook del teclado.");
        }

        _hookHandle = nint.Zero;
        _ownerThreadId = 0;
    }

    private nint HookCallback(
    int nCode,
    nuint wParam,
    nint lParam)
    {
        if (nCode == NativeConstants.HC_ACTION && lParam != nint.Zero)
        {
            try
            {
                KeyboardEventType? eventType = GetEventType(wParam);

                if (eventType.HasValue)
                {
                    KBDLLHOOKSTRUCT data =
                        Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                    bool isInjected =
                        (data.Flags & NativeConstants.LLKHF_INJECTED) != 0;

                    bool isOwnInjected =
                        isInjected &&
                        data.ExtraInfo == NativeConstants.TeclaLibreExtraInfo;

                    KeyboardEvent keyboardEvent = new(
                        VirtualKey: checked((int)data.VirtualKey),
                        ScanCode: data.ScanCode,
                        EventType: eventType.Value,
                        IsExtended:
                            (data.Flags & NativeConstants.LLKHF_EXTENDED) != 0,
                        IsInjected: isInjected,
                        IsOwnInjected: isOwnInjected,
                        Timestamp: data.Time);

                    // El panel recibe los eventos para su diagnóstico.
                    KeyboardEventReceived?.Invoke(keyboardEvent);

                    // El motor decide si se bloquea la entrada original.
                    bool suppress =
                        ShouldSuppress?.Invoke(keyboardEvent) == true;

                    if (suppress)
                    {
                        return (nint)1;
                    }
                }
            }
            catch (Exception)
            {
                // Si ocurre un error, dejamos pasar el evento original.
            }
        }

        return NativeMethods.CallNextHookEx(
            _hookHandle,
            nCode,
            wParam,
            lParam);
    }

    private static KeyboardEventType? GetEventType(nuint message)
    {
        if (message == (nuint)NativeConstants.WM_KEYDOWN ||
            message == (nuint)NativeConstants.WM_SYSKEYDOWN)
        {
            return KeyboardEventType.KeyDown;
        }

        if (message == (nuint)NativeConstants.WM_KEYUP ||
            message == (nuint)NativeConstants.WM_SYSKEYUP)
        {
            return KeyboardEventType.KeyUp;
        }

        return null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Stop();
        _disposed = true;
    }
}