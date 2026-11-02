using TeclaLibre.Models;
using TeclaLibre.Native;

namespace TeclaLibre.Keyboard;

internal sealed class RemappingEngine
{
    private const int MaxPendingOutputs = 128;

    private readonly KeyboardStateTracker _keyState = new();

    private readonly Dictionary<int, ActivePress> _blockedKeys = new();

    private readonly Queue<PendingOutput> _pendingOutputs = new();

    private List<MappingRule> _rules = new();

    private bool _enabled;

    public bool Enabled => _enabled;

    public int Version { get; private set; }

    public event Action<nint, MappingOutput, int>? OutputReady;

    public void SetRules(IEnumerable<MappingRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        // Las reglas se reemplazan en el hilo de la interfaz.
        // Copiamos sus datos para que una edición posterior no
        // modifique una pulsación que ya está en curso.
        _rules = rules
            .Where(rule => rule.Enabled)
            .Select(CopyRule)
            .ToList();

        _pendingOutputs.Clear();
        Version++;
    }

    public void SetEnabled(bool enabled)
    {
        if (_enabled == enabled)
        {
            return;
        }

        _enabled = enabled;
        _pendingOutputs.Clear();

        // Conservamos las teclas bloqueadas hasta recibir su
        // KeyUp, incluso si se pausa mientras están presionadas.
        Version++;
    }

    public void Reset()
    {
        _keyState.Reset();
        _blockedKeys.Clear();
        _pendingOutputs.Clear();

        Version++;
    }

    public bool Process(KeyboardEvent keyboardEvent)
    {
        if (keyboardEvent.IsInjected)
        {
            return false;
        }

        bool isRepeat = _keyState.Process(keyboardEvent);

        int key = keyboardEvent.VirtualKey;
        bool suppress = false;

        if (_blockedKeys.TryGetValue(
                key,
                out ActivePress? activePress))
        {
            suppress = true;

            if (keyboardEvent.EventType ==
                KeyboardEventType.KeyUp)
            {
                _blockedKeys.Remove(key);
            }
            else if (
                _enabled &&
                isRepeat &&
                activePress.RepeatMode ==
                    RepeatMode.RepeatWhileHeld)
            {
                QueueOutput(
                    activePress.TargetWindow,
                    activePress.Output);
            }
        }
        else if (
            _enabled &&
            !isRepeat &&
            keyboardEvent.EventType ==
                KeyboardEventType.KeyDown)
        {
            MappingRule? rule = _rules.FirstOrDefault(candidate =>
                KeyTriggerMatcher.Matches(
                    candidate.Trigger,
                    keyboardEvent,
                    _keyState.CurrentModifiers));

            if (rule is not null)
            {
                nint targetWindow =
                    NativeMethods.GetForegroundWindow();

                if (targetWindow != nint.Zero &&
                    _pendingOutputs.Count < MaxPendingOutputs)
                {
                    MappingOutput output = CopyOutput(rule.Output);

                    _blockedKeys.Add(
                        key,
                        new ActivePress(
                            targetWindow,
                            output,
                            rule.RepeatMode));

                    QueueOutput(targetWindow, output);

                    suppress = true;
                }
            }
        }

        if (_blockedKeys.Count == 0 &&
            _keyState.CurrentModifiers == KeyModifiers.None)
        {
            ReleasePendingOutputs();
        }

        return suppress;
    }

    private void QueueOutput(
        nint targetWindow,
        MappingOutput output)
    {
        if (_pendingOutputs.Count >= MaxPendingOutputs)
        {
            return;
        }

        _pendingOutputs.Enqueue(
            new PendingOutput(
                targetWindow,
                CopyOutput(output)));
    }

    private void ReleasePendingOutputs()
    {
        while (_pendingOutputs.Count > 0)
        {
            PendingOutput pending =
                _pendingOutputs.Dequeue();

            OutputReady?.Invoke(
                pending.TargetWindow,
                pending.Output,
                Version);
        }
    }

    private static MappingRule CopyRule(MappingRule source)
    {
        return new MappingRule
        {
            Id = source.Id,
            Name = source.Name,
            Enabled = source.Enabled,
            RepeatMode = source.RepeatMode,

            Trigger = new KeyTrigger
            {
                VirtualKey = source.Trigger.VirtualKey,
                Modifiers = source.Trigger.Modifiers,
                ExactMatch = source.Trigger.ExactMatch
            },

            Output = CopyOutput(source.Output)
        };
    }

    private static MappingOutput CopyOutput(
        MappingOutput source)
    {
        return new MappingOutput
        {
            Kind = source.Kind,
            Text = source.Text,
            VirtualKey = source.VirtualKey,
            Modifiers = source.Modifiers
        };
    }

    private sealed record ActivePress(
        nint TargetWindow,
        MappingOutput Output,
        RepeatMode RepeatMode);

    private sealed record PendingOutput(
        nint TargetWindow,
        MappingOutput Output);
}