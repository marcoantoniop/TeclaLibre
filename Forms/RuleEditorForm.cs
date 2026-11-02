using TeclaLibre.Keyboard;
using TeclaLibre.Models;
using System.ComponentModel;

namespace TeclaLibre.Forms;

public partial class RuleEditorForm : Form
{
    private KeyboardHookService? _keyboardHook;

    private KeyTrigger? _inputTrigger;
    private KeyTrigger? _outputTrigger;

    private Guid _ruleId = Guid.NewGuid();

    //public MappingRule? ResultRule { get; private set; }
    [Browsable(false)]
    [DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)]
    public MappingRule? ResultRule { get; private set; }

    // Constructor utilizado por el diseñador.
    public RuleEditorForm()
    {
        InitializeComponent();

        cmbOutputKind.Items.AddRange(
        [
            "Texto",
            "Tecla",
            "Combinación"
        ]);

        cmbOutputKind.SelectedIndexChanged +=
            CmbOutputKind_SelectedIndexChanged;

        btnCaptureTrigger.Click += BtnCaptureTrigger_Click;
        btnCaptureOutput.Click += BtnCaptureOutput_Click;
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += BtnCancel_Click;

        chkEnabled.Checked = true;
        chkExact.Checked = true;
        chkRepeat.Checked = false;

        cmbOutputKind.SelectedIndex = 0;

        AcceptButton = btnSave;
        CancelButton = btnCancel;
    }

    internal RuleEditorForm(
        KeyboardHookService keyboardHook,
        MappingRule? existingRule = null)
        : this()
    {
        _keyboardHook = keyboardHook;

        if (existingRule is not null)
        {
            LoadRule(existingRule);
        }
    }

    private void CmbOutputKind_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        _outputTrigger = null;
        txtOutput.Clear();

        UpdateOutputControls();
    }

    private void UpdateOutputControls()
    {
        bool isText = cmbOutputKind.SelectedIndex == 0;

        txtOutput.ReadOnly = !isText;
        btnCaptureOutput.Enabled = !isText;
    }

    private void BtnCaptureTrigger_Click(
        object? sender,
        EventArgs e)
    {
        KeyTrigger? result = CaptureKey();

        if (result is null)
        {
            return;
        }

        _inputTrigger = result;
        txtTrigger.Text = FormatTrigger(result);
    }

    private void BtnCaptureOutput_Click(
        object? sender,
        EventArgs e)
    {
        KeyTrigger? result = CaptureKey();

        if (result is null)
        {
            return;
        }

        if (cmbOutputKind.SelectedIndex == 1 &&
            result.Modifiers != KeyModifiers.None)
        {
            MessageBox.Show(
                this,
                "Para una salida de tipo Tecla, captura una tecla " +
                "sin modificadores.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (cmbOutputKind.SelectedIndex == 2 &&
            result.Modifiers == KeyModifiers.None)
        {
            MessageBox.Show(
                this,
                "Para una salida de tipo Combinación, incluye " +
                "al menos un modificador.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        _outputTrigger = result;
        txtOutput.Text = FormatTrigger(result);
    }

    private KeyTrigger? CaptureKey()
    {
        if (_keyboardHook is null || !_keyboardHook.IsRunning)
        {
            MessageBox.Show(
                this,
                "Activa la captura en la ventana principal " +
                "antes de seleccionar una combinación.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        using KeyCaptureForm captureForm = new();

        Func<KeyboardEvent, bool>? previousHandler =
            _keyboardHook.ShouldSuppress;

        try
        {
            // Mientras el diálogo está abierto, recibe la decisión
            // de bloqueo para capturar la tecla principal.
            _keyboardHook.ShouldSuppress =
                captureForm.ProcessKeyboardEvent;

            DialogResult result = captureForm.ShowDialog(this);

            return result == DialogResult.OK
                ? captureForm.CapturedTrigger
                : null;
        }
        finally
        {
            _keyboardHook.ShouldSuppress = previousHandler;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (_inputTrigger is null)
        {
            ShowValidationMessage(
                "Primero captura la tecla o combinación de entrada.");

            return;
        }

        OutputKind outputKind =
            (OutputKind)cmbOutputKind.SelectedIndex;

        MappingOutput output;

        if (outputKind == OutputKind.Text)
        {
            if (string.IsNullOrEmpty(txtOutput.Text))
            {
                ShowValidationMessage(
                    "Escribe el texto que deseas enviar.");

                return;
            }

            output = new MappingOutput
            {
                Kind = OutputKind.Text,
                Text = txtOutput.Text
            };
        }
        else
        {
            if (_outputTrigger is null)
            {
                ShowValidationMessage(
                    "Captura la tecla o combinación de salida.");

                return;
            }

            bool hasAltGr =
                (_outputTrigger.Modifiers & KeyModifiers.AltGr) != 0;

            bool hasControl =
                (_outputTrigger.Modifiers & KeyModifiers.Control) != 0;

            if (hasAltGr && hasControl)
            {
                ShowValidationMessage(
                    "La salida con AltGr y Control adicional " +
                    "todavía no está admitida.");

                return;
            }

            output = new MappingOutput
            {
                Kind = outputKind,
                VirtualKey = _outputTrigger.VirtualKey,
                Modifiers = outputKind == OutputKind.Shortcut
                    ? _outputTrigger.Modifiers
                    : KeyModifiers.None
            };
        }

        KeyTrigger input = new()
        {
            VirtualKey = _inputTrigger.VirtualKey,
            Modifiers = _inputTrigger.Modifiers,
            ExactMatch = chkExact.Checked
        };

        ResultRule = new MappingRule
        {
            Id = _ruleId,

            Name = string.IsNullOrWhiteSpace(txtName.Text)
                ? "Regla sin nombre"
                : txtName.Text.Trim(),

            Enabled = chkEnabled.Checked,

            Trigger = input,
            Output = output,

            RepeatMode = chkRepeat.Checked
                ? RepeatMode.RepeatWhileHeld
                : RepeatMode.OncePerPress
        };

        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void ShowValidationMessage(string message)
    {
        MessageBox.Show(
            this,
            message,
            "TeclaLibre - Regla incompleta",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void LoadRule(MappingRule rule)
    {
        _ruleId = rule.Id;

        txtName.Text = rule.Name;
        chkEnabled.Checked = rule.Enabled;
        chkExact.Checked = rule.Trigger.ExactMatch;

        chkRepeat.Checked =
            rule.RepeatMode == RepeatMode.RepeatWhileHeld;

        _inputTrigger = new KeyTrigger
        {
            VirtualKey = rule.Trigger.VirtualKey,
            Modifiers = rule.Trigger.Modifiers,
            ExactMatch = rule.Trigger.ExactMatch
        };

        txtTrigger.Text = FormatTrigger(_inputTrigger);

        cmbOutputKind.SelectedIndex = (int)rule.Output.Kind;

        if (rule.Output.Kind == OutputKind.Text)
        {
            txtOutput.Text = rule.Output.Text;
        }
        else
        {
            _outputTrigger = new KeyTrigger
            {
                VirtualKey = rule.Output.VirtualKey,
                Modifiers = rule.Output.Modifiers,
                ExactMatch = true
            };

            txtOutput.Text = FormatTrigger(_outputTrigger);
        }
    }

    private static string FormatTrigger(KeyTrigger trigger)
    {
        List<string> parts = new();

        if ((trigger.Modifiers & KeyModifiers.Control) != 0)
        {
            parts.Add("Ctrl");
        }

        if ((trigger.Modifiers & KeyModifiers.Shift) != 0)
        {
            parts.Add("Shift");
        }

        if ((trigger.Modifiers & KeyModifiers.Alt) != 0)
        {
            parts.Add("Alt");
        }

        if ((trigger.Modifiers & KeyModifiers.AltGr) != 0)
        {
            parts.Add("AltGr");
        }

        if ((trigger.Modifiers & KeyModifiers.Windows) != 0)
        {
            parts.Add("Windows");
        }

        parts.Add(((Keys)trigger.VirtualKey).ToString());

        return string.Join(" + ", parts);
    }
}