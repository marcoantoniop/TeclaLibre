using TeclaLibre.Models;
using TeclaLibre.Native;

namespace TeclaLibre.Keyboard;

internal sealed class DemoRemappingEngine
{
    private const int VirtualKeyZ = 0x5A;
    private const int MaxPendingOutputs = 32;

    private readonly KeyboardStateTracker _keyState = new();
    private readonly Queue<nint> _pendingTargets = new();

    private readonly KeyTrigger _trigger = new()
    {
        VirtualKey = VirtualKeyZ,
        Modifiers = KeyModifiers.AltGr,
        ExactMatch = true
    };

    private bool _enabled;
    private bool _zBlocked;

    public event Action<nint, int>? OutputReady;

    public bool Enabled => _enabled;

    // Permite invalidar envíos pendientes si se cambia el estado.
    public int Version { get; private set; }

    public void SetEnabled(bool enabled)
    {
        if (_enabled == enabled)
        {
            return;
        }

        _enabled = enabled;
        Version++;

        if (!enabled)
        {
            _pendingTargets.Clear();
            _zBlocked = false;
        }
    }

    public void Reset()
    {
        _keyState.Reset();
        _pendingTargets.Clear();

        _zBlocked = false;
        Version++;
    }

    /// <summary>
    /// Devuelve true cuando el evento original debe bloquearse.
    /// </summary>
    public bool Process(KeyboardEvent keyboardEvent)
    {
        // Las entradas inyectadas siguen su curso y no modifican
        // el estado físico utilizado por esta regla de prueba.
        if (keyboardEvent.IsInjected)
        {
            return false;
        }

        _keyState.Process(keyboardEvent);

        bool suppress = false;

        if (keyboardEvent.VirtualKey == VirtualKeyZ)
        {
            if (_zBlocked)
            {
                // También bloqueamos las repeticiones y la
                // liberación de una Z cuyo KeyDown se bloqueó.
                suppress = true;

                if (keyboardEvent.EventType ==
                    KeyboardEventType.KeyUp)
                {
                    _zBlocked = false;
                }
            }
            else if (
                _enabled &&
                keyboardEvent.EventType ==
                    KeyboardEventType.KeyDown &&
                _pendingTargets.Count < MaxPendingOutputs &&
                KeyTriggerMatcher.Matches(
                    _trigger,
                    keyboardEvent,
                    _keyState.CurrentModifiers))
            {
                nint targetWindow =
                    NativeMethods.GetForegroundWindow();

                if (targetWindow != nint.Zero)
                {
                    _pendingTargets.Enqueue(targetWindow);

                    _zBlocked = true;
                    suppress = true;
                }
            }
        }

        // Esperamos a que se libere Z y todos los modificadores.
        if (!_zBlocked &&
            _keyState.CurrentModifiers == KeyModifiers.None)
        {
            ReleasePendingOutputs();
        }

        return suppress;
    }

    private void ReleasePendingOutputs()
    {
        while (_pendingTargets.Count > 0)
        {
            nint targetWindow = _pendingTargets.Dequeue();

            OutputReady?.Invoke(targetWindow, Version);
        }
    }
}