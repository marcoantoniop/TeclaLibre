using TeclaLibre.Keyboard;
using TeclaLibre.Models;
using TeclaLibre.Native;
using System.ComponentModel;

namespace TeclaLibre.Forms;

internal sealed class KeyCaptureForm : Form
{
    private readonly KeyboardStateTracker _keyState = new();

    private KeyTrigger? _capturedTrigger;
    private bool _completionScheduled;

    //public KeyTrigger? CapturedTrigger => _capturedTrigger;
    [Browsable(false)]
    [DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)]
    public KeyTrigger? CapturedTrigger => _capturedTrigger;

    public KeyCaptureForm()
    {
        Text = "Capturar combinación";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        MinimizeBox = false;
        MaximizeBox = false;

        ClientSize = new Size(440, 160);

        Label lblInstructions = new()
        {
            AutoSize = false,
            Location = new Point(20, 20),
            Size = new Size(400, 70),
            Text =
                "Presiona la tecla o combinación que deseas capturar.\r\n\r\n" +
                "Suelta la tecla principal para confirmar.\r\n" +
                "Pulsa Esc para cancelar."
        };

        Button btnCancel = new()
        {
            Text = "Cancelar",
            Size = new Size(100, 32),
            Location = new Point(320, 108),
            DialogResult = DialogResult.Cancel
        };

        Controls.Add(lblInstructions);
        Controls.Add(btnCancel);

        CancelButton = btnCancel;
    }

    /// <summary>
    /// Se llama desde el hook mientras esta ventana está activa.
    /// Devuelve true cuando la tecla debe bloquearse.
    /// </summary>
    internal bool ProcessKeyboardEvent(KeyboardEvent keyboardEvent)
    {
        if (Form.ActiveForm != this || keyboardEvent.IsInjected)
        {
            return false;
        }

        _keyState.Process(keyboardEvent);

        // Esc puro cancela el diálogo
        if (_capturedTrigger is null &&
            keyboardEvent.VirtualKey == NativeConstants.VK_ESCAPE &&
            keyboardEvent.EventType == KeyboardEventType.KeyDown &&
            _keyState.CurrentModifiers == KeyModifiers.None)
        {
            CancelAndClose();
            return true; // suprimir Esc para que no abra Start menu
        }

        if (_capturedTrigger is not null)
        {
            if (keyboardEvent.VirtualKey == _capturedTrigger.VirtualKey)
            {
                if (keyboardEvent.EventType == KeyboardEventType.KeyUp && !_completionScheduled)
                {
                    _completionScheduled = true;

                    // Cerramos después de terminar el callback.
                    BeginInvoke((Action)(() =>
                    {
                        if (!IsDisposed)
                        {
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                    }));
                }

                // Bloquear repeticiones y liberación de la tecla
                // principal que estamos capturando.
                return true;
            }

            return false;
        }

        if (keyboardEvent.EventType != KeyboardEventType.KeyDown)
        {
            return false;
        }

        if (IsModifierKey(keyboardEvent.VirtualKey))
        {
            return false;
        }

        if (keyboardEvent.VirtualKey < 1 ||
            keyboardEvent.VirtualKey > 254)
        {
            return false;
        }

        _capturedTrigger = new KeyTrigger
        {
            VirtualKey = keyboardEvent.VirtualKey,
            Modifiers = _keyState.CurrentModifiers,
            ExactMatch = true
        };

        // Bloquear la pulsación principal para que no se ejecute
        // en el diálogo mientras se está capturando.
        return true;
    }

    // 🆕 Cierre diferido, sin reentrancia
    private void CancelAndClose()
    {
        if (_completionScheduled) return;
        _completionScheduled = true;

        BeginInvoke((Action)(() =>
        {
            if (!IsDisposed)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }));
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

    // Reset del estado cuando el formulario pierde el foco.
    // Evita que un Alt+Tab deje modificadores "pegados" en el tracker.
    protected override void OnDeactivate(EventArgs e)
    {
        base.OnDeactivate(e);
        _keyState.Reset();
    }


}