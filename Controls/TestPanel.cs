using TeclaLibre.Keyboard;
using TeclaLibre.Models;
using System.ComponentModel;


namespace TeclaLibre.Controls;

public partial class TestPanel : UserControl
{
    private const int MaxPendingEvents = 500;
    private const int MaxVisibleEvents = 300;
    private const int MaxEventsPerTick = 80;

    private readonly Queue<KeyboardEvent> _pendingEvents = new();
    
    private readonly KeyboardStateTracker _keyState = new();

    private readonly System.Windows.Forms.Timer _displayTimer = new();
    private readonly InputSender _inputSender = new();

    private bool _captureRunning;
    private bool _isTesting;
    private bool _queueOverflowed;
    //public bool DemoRuleEnabled => chkDemoRule.Checked;
    [Browsable(false)]
    [DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)]
    public bool DemoRuleEnabled => chkDemoRule.Checked;
    public event EventHandler? DemoRuleChanged;

    public TestPanel()
    {
        InitializeComponent();

        ConfigureEventList();

        btnStartTest.Click += BtnStartTest_Click;
        btnStopTest.Click += BtnStopTest_Click;
        btnClearTest.Click += BtnClearTest_Click;

        btnSendText.Click += BtnSendText_Click;
        btnSendEnter.Click += BtnSendEnter_Click;
        btnSendShortcut.Click += BtnSendShortcut_Click;

        _displayTimer.Interval = 100;
        _displayTimer.Tick += DisplayTimer_Tick;

        chkDemoRule.CheckedChanged += (_, _) =>
        {
            DemoRuleChanged?.Invoke(this, EventArgs.Empty);
        };

        Disposed += (_, _) => _displayTimer.Dispose();

        UpdateButtons();
    }

    public void SetCaptureRunning(bool running)
    {
        _captureRunning = running;

        if (!running && _isTesting)
        {
            StopTest();
        }

        UpdateButtons();
    }

    public void EnqueueEvent(KeyboardEvent keyboardEvent)
    {
        if (!_isTesting)
        {
            return;
        }

        if (_pendingEvents.Count >= MaxPendingEvents)
        {
            _queueOverflowed = true;
            return;
        }

        _pendingEvents.Enqueue(keyboardEvent);
    }

    private void ConfigureEventList()
    {
        lvEvents.Columns.Clear();

        lvEvents.Columns.Add("Evento", 100);
        lvEvents.Columns.Add("Tecla", 130);
        lvEvents.Columns.Add("Código VK", 100);
        lvEvents.Columns.Add("Scan code", 100);
        lvEvents.Columns.Add("Extendida", 100);
        lvEvents.Columns.Add("Origen", 130);
    }

    private void BtnStartTest_Click(object? sender, EventArgs e)
    {
        if (!_captureRunning)
        {
            return;
        }

        _pendingEvents.Clear();
        //_pressedKeys.Clear();
        _keyState.Reset();
        _queueOverflowed = false;
        _isTesting = true;

        lblPressed.Text = "Teclas presionadas: ninguna";

        _displayTimer.Start();

        UpdateButtons();
        txtPreview.Focus();
    }

    private void BtnStopTest_Click(object? sender, EventArgs e)
    {
        StopTest();
    }

    private void BtnClearTest_Click(object? sender, EventArgs e)
    {
        lvEvents.Items.Clear();
        txtPreview.Clear();
    }

    private void StopTest()
    {
        _isTesting = false;
        _displayTimer.Stop();

        _pendingEvents.Clear();
        //_pressedKeys.Clear();
        _keyState.Reset();
        _queueOverflowed = false;

        lblPressed.Text = "Teclas presionadas: ninguna";

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        btnStartTest.Enabled = _captureRunning && !_isTesting;
        btnStopTest.Enabled = _isTesting;
        btnClearTest.Enabled = true;
    }

    private void DisplayTimer_Tick(object? sender, EventArgs e)
    {
        if (_queueOverflowed)
        {
            _pendingEvents.Clear();
            _keyState.Reset();
            _queueOverflowed = false;

            lblPressed.Text =
                "Se perdieron eventos; suelta las teclas y vuelve a probar.";

            return;
        }

        int processed = 0;

        lvEvents.BeginUpdate();

        try
        {
            while (_pendingEvents.Count > 0 &&
                   processed < MaxEventsPerTick)
            {
                KeyboardEvent keyboardEvent = _pendingEvents.Dequeue();

                bool isRepeat = _keyState.Process(keyboardEvent);
                AddEventRow(keyboardEvent, isRepeat);

                processed++;
            }

            while (lvEvents.Items.Count > MaxVisibleEvents)
            {
                lvEvents.Items.RemoveAt(0);
            }

            if (processed > 0 && lvEvents.Items.Count > 0)
            {
                lvEvents.Items[lvEvents.Items.Count - 1].EnsureVisible();
            }
        }
        finally
        {
            lvEvents.EndUpdate();
        }

        if (processed > 0)
        {
            UpdatePressedLabel();
        }
    }


    private void UpdatePressedLabel()
    {
        string keys = _keyState.PressedKeys.Count == 0
            ? "ninguna"
            : string.Join(
                " + ",
                _keyState.PressedKeys
                    .OrderBy(key => key)
                    .Select(GetKeyName));

        string modifiers = _keyState.CurrentModifiers == KeyModifiers.None
            ? "ninguno"
            : _keyState.CurrentModifiers.ToString();

        lblPressed.Text =
            $"Teclas: {keys}  |  Modificadores: {modifiers}";
    }

    private void AddEventRow(
    KeyboardEvent keyboardEvent,
    bool isRepeat)
    //private void AddEventRow(KeyboardEvent keyboardEvent)
    {
        
        string eventName = keyboardEvent.EventType switch
        {
            KeyboardEventType.KeyDown when isRepeat => "Repetida",
            KeyboardEventType.KeyDown => "Presionada",
            _ => "Liberada"
        };

        string origin = keyboardEvent.IsOwnInjected
            ? "TeclaLibre"
            : keyboardEvent.IsInjected
                ? "Software"
                : "Teclado";

        ListViewItem item = new(eventName);

        item.SubItems.Add(GetKeyName(keyboardEvent.VirtualKey));
        item.SubItems.Add($"0x{keyboardEvent.VirtualKey:X2}");
        item.SubItems.Add($"0x{keyboardEvent.ScanCode:X2}");
        item.SubItems.Add(keyboardEvent.IsExtended ? "Sí" : "No");
        item.SubItems.Add(origin);

        lvEvents.Items.Add(item);
    }

    private static string GetKeyName(int virtualKey)
    {
        return ((Keys)virtualKey).ToString();
    }

    private void BtnSendText_Click(object? sender, EventArgs e)
    {
        RunOutputTest(
            () => _inputSender.SendText("Hola, TeclaLibre: > < ñ"));
    }

    private void BtnSendEnter_Click(object? sender, EventArgs e)
    {
        RunOutputTest(
            () => _inputSender.SendKey((int)Keys.Enter));
    }

    private void BtnSendShortcut_Click(object? sender, EventArgs e)
    {
        RunOutputTest(
            () => _inputSender.SendShortcut(
                (int)Keys.A,
                KeyModifiers.Control));
    }

    private void RunOutputTest(Action sendAction)
    {
        try
        {
            // El envío va a la ventana que tiene el foco.
            // En esta prueba elegimos el cuadro del propio panel.
            if (!txtPreview.Focus())
            {
                throw new InvalidOperationException(
                    "No se pudo enfocar el cuadro de prueba.");
            }

            sendAction();
        }
        catch (Exception ex) when (
            ex is ArgumentException ||
            ex is InvalidOperationException)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "TeclaLibre - Prueba de salida",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}