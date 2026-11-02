using System.ComponentModel;
using System.Diagnostics;
using TeclaLibre.Keyboard;
using TeclaLibre.Models;
using TeclaLibre.Controls;
using TeclaLibre.Native;
using System.Text.Json;
using TeclaLibre.Services;
using System.Runtime.InteropServices;
using System.Security;

namespace TeclaLibre.Forms;

public partial class MainForm : Form
{
    private readonly KeyboardHookService _keyboardHook = new();
    private readonly TestPanel _testPanel = new();

    private readonly DemoRemappingEngine _demoEngine = new();
    private readonly InputSender _inputSender = new();

    private readonly RulesPanel _rulesPanel = new();
    private readonly RemappingEngine _rulesEngine = new();

    private bool _editingRule;
    private readonly ConfigurationService _configurationService = new();

    private AppSettings _appSettings = new();

    private bool _restoringConfiguration;
    private bool _automaticSaveBlocked;

    private const int PauseHotkeyId = 1401;

    private readonly StartupService _startupService = new();

    private readonly NotifyIcon _trayIcon = new();
    private readonly ContextMenuStrip _trayMenu = new();

    private ToolStripMenuItem? _trayToggleItem;

    private bool _pauseHotkeyRegistered;
    private bool _settingsReady;
    private bool _explicitExit;
    private SingleInstanceListener? _instanceListener;


    public MainForm()
    {
        InitializeComponent();
        _testPanel.Dock = DockStyle.Fill;
        tabTests.Controls.Add(_testPanel);

        _rulesPanel.Dock = DockStyle.Fill;
        tabMappings.Controls.Add(_rulesPanel);

        _rulesPanel.EditRuleRequested = OpenRuleEditor;
        _rulesPanel.RulesChanged += RulesPanel_RulesChanged;

        btnSaveConfiguration.Click += BtnSaveConfiguration_Click;
        btnImportConfiguration.Click += BtnImportConfiguration_Click;
        btnExportConfiguration.Click += BtnExportConfiguration_Click;

        lblConfigPath.Text =
            $"Archivo automático: {_configurationService.DefaultFilePath}";

        _rulesEngine.OutputReady += RulesEngine_OutputReady;

        _keyboardHook.ShouldSuppress = ProcessKeyboardEvent;

        _demoEngine.OutputReady += DemoEngine_OutputReady;


        _testPanel.DemoRuleChanged += (_, _) =>
        {
            RefreshActiveEngines();
        };


        _keyboardHook.KeyboardEventReceived += OnKeyboardEventReceived;


        btnToggleEnabled.Enabled = true;
        btnToggleEnabled.Click += BtnToggleEnabled_Click;

        //RestoreSavedConfiguration();

        //UpdateHookStatus();

        InitializeTray();

        btnApplySettings.Click += BtnApplySettings_Click;

        _settingsReady = true;

        RestoreSavedConfiguration();

        RefreshSettingsControls();
        UpdateHookStatus();
    }

    private void BtnToggleEnabled_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_keyboardHook.IsRunning)
            {
                _keyboardHook.Stop();
            }
            else
            {
                _keyboardHook.Start();
            }

            UpdateHookStatus();
        }
        catch (Win32Exception ex)
        {
            MessageBox.Show(
                this,
                $"{ex.Message}{Environment.NewLine}" +
                $"Código de Windows: {ex.NativeErrorCode}",
                "TeclaLibre - Error de teclado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "TeclaLibre - Error de teclado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void UpdateHookStatus()
    {
        //_testPanel.SetCaptureRunning(_keyboardHook.IsRunning);
        if (_keyboardHook.IsRunning)
        {
            lblState.Text = "Prueba activa";
            lblState.ForeColor = Color.DarkGreen;

            btnToggleEnabled.Text = "Detener";

            lblStatus.Text = "Captura de prueba activa";
        }
        else
        {
            //_demoEngine.Reset();
            _rulesEngine.Reset();
            _demoEngine.Reset();
            lblState.Text = "Inactivo";
            lblState.ForeColor = Color.DimGray;

            btnToggleEnabled.Text = "Activar";

            lblStatus.Text = "Motor detenido";


        }
        _testPanel.SetCaptureRunning(_keyboardHook.IsRunning);
        RefreshActiveEngines();
        UpdateTrayStatus();
    }

    private void DemoEngine_OutputReady(
    nint targetWindow,
    int version)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        // El evento se programa para después de que termine
        // el callback del hook.
        BeginInvoke((Action)(() =>
        {
            if (IsDisposed ||
                !_keyboardHook.IsRunning ||
                !_demoEngine.Enabled ||
                version != _demoEngine.Version)
            {
                return;
            }

            if (NativeMethods.GetForegroundWindow() != targetWindow)
            {
                lblStatus.Text =
                    "Se descartó la salida: cambió la ventana activa.";

                return;
            }

            try
            {
                _inputSender.SendText(">");

                lblStatus.Text =
                    "Regla de prueba: se envió >";
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is InvalidOperationException)
            {
                lblStatus.Text =
                    $"No se pudo enviar la salida: {ex.Message}";
            }
        }));
    }

    private void OnKeyboardEventReceived(KeyboardEvent keyboardEvent)
    {
        _testPanel.EnqueueEvent(keyboardEvent);
    }


    /*
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (e.Cancel)
        {
            return;
        }

        if (e.CloseReason == CloseReason.UserClosing && !_explicitExit && _appSettings.CloseToTray)
        {
            e.Cancel = true;
            HideToTray();
            return;
        }

        try
        {
            _keyboardHook.Dispose();
        }
        catch (Win32Exception ex)
        {
            e.Cancel = true;
            _explicitExit = false;

            MessageBox.Show(
                this,
                $"No se pudo detener la captura.{Environment.NewLine}" +
                $"{ex.Message}{Environment.NewLine}" +
                $"Código de Windows: {ex.NativeErrorCode}",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (InvalidOperationException ex)
        {
            e.Cancel = true;
            _explicitExit = false;

            MessageBox.Show(
                this,
                ex.Message,
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
    */

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (e.Cancel)
        {
            return;
        }

        if (e.CloseReason == CloseReason.UserClosing &&
            !_explicitExit &&
            _appSettings.CloseToTray)
        {
            e.Cancel = true;
            HideToTray();
            return;
        }

        // Intentar liberar el hook. Si falla, mostrar advertencia
        // pero NO cancelar el cierre: OnFormClosed debe ejecutarse
        // para liberar tray, listener de instancia y hotkey.
        // Windows liberará el hook restante al terminar el proceso.
        try
        {
            _keyboardHook.Dispose();
        }
        catch (Win32Exception ex)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MessageBox.Show(
                    this,
                    $"No se pudo detener la captura del teclado.{Environment.NewLine}" +
                    $"{ex.Message}{Environment.NewLine}" +
                    $"Código de Windows: {ex.NativeErrorCode}{Environment.NewLine}{Environment.NewLine}" +
                    "El cierre continuará. Windows liberará el hook " +
                    "al terminar el proceso.",
                    "TeclaLibre",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (InvalidOperationException ex)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MessageBox.Show(
                    this,
                    ex.Message + Environment.NewLine + Environment.NewLine +
                    "El cierre continuará. Windows liberará el hook " +
                    "al terminar el proceso.",
                    "TeclaLibre",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }


    private bool ProcessKeyboardEvent(KeyboardEvent keyboardEvent)
    {
        // Ambos motores observan los eventos para mantener su estado.
        // Solo uno estará habilitado para iniciar reglas nuevas.
        bool suppressUserRule = _rulesEngine.Process(keyboardEvent);
        bool suppressDemoRule = _demoEngine.Process(keyboardEvent);

        return suppressUserRule || suppressDemoRule;
    }

    private void RulesPanel_RulesChanged(
    object? sender,
    EventArgs e)
    {
        _rulesEngine.SetRules(_rulesPanel.Rules);

        RefreshActiveEngines();

        if (!_restoringConfiguration)
        {
            SaveAutomaticConfiguration(showSuccess: false);
        }
    }

    private void RefreshActiveEngines()
    {
        bool canRun =
            _keyboardHook.IsRunning &&
            !_editingRule;

        bool runUserRules =
            canRun &&
            _rulesPanel.HasEnabledRules;

        bool runDemoRule =
            canRun &&
            !runUserRules &&
            _testPanel.DemoRuleEnabled;

        _rulesEngine.SetEnabled(runUserRules);
        _demoEngine.SetEnabled(runDemoRule);
    }

    private MappingRule? OpenRuleEditor(MappingRule? existingRule)
    {
        if (!_keyboardHook.IsRunning)
        {
            MessageBox.Show(
                this,
                "Primero pulsa Activar en la ventana principal.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        _editingRule = true;
        RefreshActiveEngines();

        try
        {
            using RuleEditorForm editor =
                new(_keyboardHook, existingRule);

            return editor.ShowDialog(this) == DialogResult.OK
                ? editor.ResultRule
                : null;
        }
        finally
        {
            // Durante la captura, el diálogo recibe directamente
            // la decisión de bloqueo del hook.
            _demoEngine.Reset();
            _rulesEngine.Reset();

            _editingRule = false;
            RefreshActiveEngines();
        }
    }

    private void RulesEngine_OutputReady(
        nint targetWindow,
        MappingOutput output,
        int version)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        BeginInvoke((Action)(() =>
        {
            if (IsDisposed ||
                !_keyboardHook.IsRunning ||
                !_rulesEngine.Enabled ||
                version != _rulesEngine.Version)
            {
                return;
            }

            if (NativeMethods.GetForegroundWindow() != targetWindow)
            {
                lblStatus.Text =
                    "Se descartó una salida: cambió la ventana activa.";

                return;
            }

            try
            {
                switch (output.Kind)
                {
                    case OutputKind.Text:
                        _inputSender.SendText(output.Text);
                        break;

                    case OutputKind.Key:
                        _inputSender.SendKey(output.VirtualKey);
                        break;

                    case OutputKind.Shortcut:
                        _inputSender.SendShortcut(
                            output.VirtualKey,
                            output.Modifiers);
                        break;

                    default:
                        throw new InvalidOperationException(
                            "Tipo de salida desconocido.");
                }

                lblStatus.Text = "Regla aplicada";
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is InvalidOperationException)
            {
                lblStatus.Text =
                    $"No se pudo enviar la salida: {ex.Message}";
            }
        }));
    }

    private AppConfiguration BuildCurrentConfiguration()
    {
        return new AppConfiguration
        {
            SchemaVersion = 1,
            Settings = _appSettings,
            Rules = _rulesPanel.Rules.ToList()
        };
    }

    private void RestoreSavedConfiguration()
    {
        if (!_configurationService.DefaultFileExists())
        {
            return;
        }

        try
        {
            AppConfiguration configuration =
                _configurationService.LoadDefault();

            ApplyConfiguration(configuration);
        }
        catch (Exception ex) when (IsConfigurationException(ex))
        {
            // Protegemos el archivo existente para que una lista
            // vacía no lo sobrescriba automáticamente.
            _automaticSaveBlocked = true;

            MessageBox.Show(
                this,
                "No se pudo cargar la configuración automática.\r\n\r\n" +
                $"{ex.Message}\r\n\r\n" +
                "El guardado automático quedó bloqueado. " +
                "Puedes importar un archivo válido o revisar la copia " +
                "config.backup.json.",
                "TeclaLibre - Error de configuración",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ApplyConfiguration(AppConfiguration configuration)
    {
        _restoringConfiguration = true;

        try
        {
            // ReplaceRules valida conflictos antes de sustituir la lista.
            _rulesPanel.ReplaceRules(configuration.Rules);

            _appSettings = configuration.Settings;
            if (_settingsReady)
            {
                RefreshSettingsControls();

                if (Visible)
                {
                    try
                    {
                        SetPauseHotkeyEnabled(
                            IsConfiguredPauseHotkey(
                                _appSettings.PauseShortcut));
                    }
                    catch (InvalidOperationException ex)
                    {
                        lblSettingsStatus.Text = ex.Message;
                    }
                }
            }
        }
        finally
        {
            _restoringConfiguration = false;
        }
    }

    private bool SaveAutomaticConfiguration(bool showSuccess)
    {
        if (_automaticSaveBlocked)
        {
            if (showSuccess)
            {
                MessageBox.Show(
                    this,
                    "El guardado automático está bloqueado porque " +
                    "no se pudo cargar el archivo existente.\r\n\r\n" +
                    "Importa una configuración válida para continuar.",
                    "TeclaLibre",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return false;
        }

        try
        {
            _configurationService.SaveDefault(
                BuildCurrentConfiguration());

            if (showSuccess)
            {
                MessageBox.Show(
                    this,
                    "La configuración se guardó correctamente.",
                    "TeclaLibre",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            return true;
        }
        catch (Exception ex) when (IsConfigurationException(ex))
        {
            _automaticSaveBlocked = true;

            MessageBox.Show(
                this,
                "No se pudo guardar la configuración.\r\n\r\n" +
                $"{ex.Message}\r\n\r\n" +
                "El guardado automático quedó bloqueado.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }
    }

    private void BtnSaveConfiguration_Click(
        object? sender,
        EventArgs e)
    {
        SaveAutomaticConfiguration(showSuccess: true);
    }

    private void BtnExportConfiguration_Click(
        object? sender,
        EventArgs e)
    {
        using SaveFileDialog dialog = new()
        {
            Title = "Exportar configuración",
            Filter = "Archivos JSON (*.json)|*.json",
            DefaultExt = "json",
            AddExtension = true,
            FileName = "TeclaLibre-configuracion.json",
            OverwritePrompt = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            string selectedPath = Path.GetFullPath(dialog.FileName);

            string automaticPath = Path.GetFullPath(
                _configurationService.DefaultFilePath);

            if (string.Equals(
                    selectedPath,
                    automaticPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    this,
                    "Selecciona otro archivo para la exportación. " +
                    "Para actualizar el archivo automático utiliza " +
                    "«Guardar ahora».",
                    "TeclaLibre",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            _configurationService.ExportToFile(
                selectedPath,
                BuildCurrentConfiguration());

            MessageBox.Show(
                this,
                "La configuración se exportó correctamente.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex) when (IsConfigurationException(ex))
        {
            MessageBox.Show(
                this,
                $"No se pudo exportar el archivo.\r\n\r\n{ex.Message}",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void BtnImportConfiguration_Click(
        object? sender,
        EventArgs e)
    {
        using OpenFileDialog dialog = new()
        {
            Title = "Importar configuración",
            Filter = "Archivos JSON (*.json)|*.json",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            AppConfiguration imported =
                _configurationService.LoadFromFile(dialog.FileName);

            string preview = BuildImportPreview(imported);

            DialogResult answer = MessageBox.Show(
                this,
                preview +
                "\r\n\r\n¿Reemplazar la configuración actual?",
                "TeclaLibre - Revisar importación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            ApplyConfiguration(imported);

            // La importación válida permite volver a guardar.
            _automaticSaveBlocked = false;

            if (SaveAutomaticConfiguration(showSuccess: false))
            {
                MessageBox.Show(
                    this,
                    "La configuración se importó y guardó correctamente.",
                    "TeclaLibre",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        catch (Exception ex) when (IsConfigurationException(ex))
        {
            MessageBox.Show(
                this,
                "No se pudo importar la configuración.\r\n\r\n" +
                ex.Message,
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static string BuildImportPreview(
        AppConfiguration configuration)
    {
        const int maxShown = 12;

        List<string> lines = new()
    {
        $"Reglas encontradas: {configuration.Rules.Count}"
    };

        foreach (MappingRule rule in configuration.Rules.Take(maxShown))
        {
            string state = rule.Enabled ? "Habilitada" : "Pausada";

            lines.Add(
                $"• {rule.Name} — {rule.Trigger.Modifiers} + " +
                $"{(Keys)rule.Trigger.VirtualKey} — {state}");
        }

        if (configuration.Rules.Count > maxShown)
        {
            lines.Add(
                $"... y {configuration.Rules.Count - maxShown} más.");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static bool IsConfigurationException(Exception ex)
    {
        return ex is
            IOException or
            UnauthorizedAccessException or
            JsonException or
            InvalidOperationException or
            ArgumentException or
            NotSupportedException;
    }

    private void InitializeTray()
    {
        ToolStripMenuItem openItem = new("Abrir TeclaLibre");

        _trayToggleItem = new ToolStripMenuItem("Activar");

        ToolStripMenuItem exitItem = new("Salir");

        openItem.Click += (_, _) => ShowMainWindow();

        _trayToggleItem.Click += (_, _) =>
        {
            BtnToggleEnabled_Click(this, EventArgs.Empty);
        };

        exitItem.Click += (_, _) =>
        {
            _explicitExit = true;
            Close();
        };

        _trayMenu.Items.Add(openItem);
        _trayMenu.Items.Add(_trayToggleItem);
        _trayMenu.Items.Add(new ToolStripSeparator());
        _trayMenu.Items.Add(exitItem);

        _trayIcon.ContextMenuStrip = _trayMenu;
        _trayIcon.Icon = this.Icon;
        _trayIcon.Text = "TeclaLibre - Inactivo";
        _trayIcon.Visible = true;

        _trayIcon.DoubleClick += (_, _) => ShowMainWindow();
    }

    private void ShowMainWindow()
    {
        Show();
        ShowInTaskbar = true;

        if (WindowState == FormWindowState.Minimized)
        {
            WindowState = FormWindowState.Normal;
        }

        Activate();
    }

    private void HideToTray()
    {
        Hide();
        ShowInTaskbar = false;
    }

    private void UpdateTrayStatus()
    {
        if (!_trayIcon.Visible)
        {
            return;
        }

        // Utiliza el mismo icono configurado en MainForm.
        _trayIcon.Icon = this.Icon;

        if (_keyboardHook.IsRunning)
        {
            _trayIcon.Text = "TeclaLibre - Activo";

            if (_trayToggleItem is not null)
            {
                _trayToggleItem.Text = "Detener";
            }
        }
        else
        {
            _trayIcon.Text = "TeclaLibre - Inactivo";

            if (_trayToggleItem is not null)
            {
                _trayToggleItem.Text = "Activar";
            }
        }
    }

    private void RefreshSettingsControls()
    {
        if (!_settingsReady)
        {
            return;
        }

        try
        {
            // La casilla muestra lo que está registrado realmente
            // para este ejecutable en Windows.
            chkStartWithWindows.Checked =
                _startupService.IsCurrentExecutableRegistered();

            lblSettingsStatus.Text = "Preferencias cargadas";
        }
        catch (Exception ex) when (
            ex is UnauthorizedAccessException or
            SecurityException or
            IOException)
        {
            chkStartWithWindows.Checked = false;

            lblSettingsStatus.Text =
                "No se pudo consultar el inicio de Windows.";
        }

        chkStartMinimized.Checked =
            _appSettings.StartMinimized;

        chkActivateOnStartup.Checked =
            _appSettings.ActivateOnStartup;

        chkCloseToTray.Checked =
            _appSettings.CloseToTray;

        chkPauseHotkey.Checked =
            IsConfiguredPauseHotkey(_appSettings.PauseShortcut);
    }

    private static bool IsConfiguredPauseHotkey(
        KeyTrigger? trigger)
    {
        return trigger is not null &&
               trigger.VirtualKey == (int)NativeConstants.VK_F9 &&
               trigger.Modifiers ==
                   (KeyModifiers.Control | KeyModifiers.Shift) &&
               trigger.ExactMatch;
    }

    private static KeyTrigger CreatePauseHotkey()
    {
        return new KeyTrigger
        {
            VirtualKey = (int)NativeConstants.VK_F9,
            Modifiers =
                KeyModifiers.Control | KeyModifiers.Shift,
            ExactMatch = true
        };
    }

    private void SetPauseHotkeyEnabled(bool enabled)
    {
        if (enabled == _pauseHotkeyRegistered)
        {
            return;
        }

        if (enabled)
        {
            uint modifiers =
                NativeConstants.MOD_CONTROL |
                NativeConstants.MOD_SHIFT |
                NativeConstants.MOD_NOREPEAT;

            bool registered = NativeMethods.RegisterHotKey(
                Handle,
                PauseHotkeyId,
                modifiers,
                NativeConstants.VK_F9);

            if (!registered)
            {
                int error = Marshal.GetLastWin32Error();

                throw new InvalidOperationException(
                    "No se pudo registrar Ctrl + Shift + F9. " +
                    "Puede estar siendo utilizado por otra aplicación. " +
                    $"Código de Windows: {error}.");
            }

            _pauseHotkeyRegistered = true;
        }
        else
        {
            if (IsHandleCreated)
            {
                bool removed = NativeMethods.UnregisterHotKey(
                    Handle,
                    PauseHotkeyId);

                if (!removed)
                {
                    int error = Marshal.GetLastWin32Error();

                    throw new InvalidOperationException(
                        "No se pudo retirar el atajo global. " +
                        $"Código de Windows: {error}.");
                }
            }

            _pauseHotkeyRegistered = false;
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeConstants.WM_HOTKEY &&
            m.WParam == (nint)PauseHotkeyId)
        {
            if (!_editingRule)
            {
                BtnToggleEnabled_Click(this, EventArgs.Empty);
            }

            return;
        }

        base.WndProc(ref m);
    }

    private void BtnApplySettings_Click(
    object? sender,
    EventArgs e)
    {
        if (_automaticSaveBlocked)
        {
            MessageBox.Show(
                this,
                "El guardado automático está bloqueado. " +
                "Importa primero una configuración válida.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        AppSettings previousSettings = _appSettings;
        bool previousHotkeyState = _pauseHotkeyRegistered;

        string? previousStartupEntry = null;
        bool startupSnapshotAvailable = false;

        try
        {
            previousStartupEntry = _startupService.ReadEntry();
            startupSnapshotAvailable = true;

            bool startWithWindows =
                chkStartWithWindows.Checked;

            bool pauseHotkey =
                chkPauseHotkey.Checked;

            _startupService.SetEnabled(startWithWindows);
            SetPauseHotkeyEnabled(pauseHotkey);

            _appSettings = new AppSettings
            {
                StartWithWindows = startWithWindows,

                StartMinimized =
                    chkStartMinimized.Checked,

                ActivateOnStartup =
                    chkActivateOnStartup.Checked,

                CloseToTray =
                    chkCloseToTray.Checked,

                PauseShortcut = pauseHotkey
                    ? CreatePauseHotkey()
                    : null
            };

            if (!SaveAutomaticConfiguration(showSuccess: false))
            {
                throw new InvalidOperationException(
                    "No se pudo guardar la configuración.");
            }

            lblSettingsStatus.Text =
                "Preferencias aplicadas y guardadas";

            MessageBox.Show(
                this,
                "Las preferencias se aplicaron correctamente.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex) when (
            ex is IOException or
            UnauthorizedAccessException or
            SecurityException or
            InvalidOperationException or
            ArgumentException)
        {
            _appSettings = previousSettings;

            string rollbackMessage = string.Empty;

            try
            {
                if (startupSnapshotAvailable)
                {
                    _startupService.WriteEntry(previousStartupEntry);
                }

                SetPauseHotkeyEnabled(previousHotkeyState);
            }
            catch (Exception rollbackEx) when (
                rollbackEx is IOException or
                UnauthorizedAccessException or
                SecurityException or
                InvalidOperationException)
            {
                rollbackMessage =
                    "\r\n\r\nNo se pudieron restaurar todos los cambios: " +
                    rollbackEx.Message;
            }

            RefreshSettingsControls();

            MessageBox.Show(
                this,
                $"No se pudieron aplicar las preferencias.\r\n\r\n" +
                ex.Message +
                rollbackMessage,
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        StartInstanceListener();

        try
        {
            SetPauseHotkeyEnabled(
                IsConfiguredPauseHotkey(
                    _appSettings.PauseShortcut));
        }
        catch (InvalidOperationException ex)
        {
            lblSettingsStatus.Text = ex.Message;
        }

        if (_appSettings.ActivateOnStartup &&
            !_keyboardHook.IsRunning)
        {
            BtnToggleEnabled_Click(this, EventArgs.Empty);
        }

        if (_appSettings.StartMinimized)
        {
            BeginInvoke((Action)HideToTray);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _instanceListener?.Dispose();
        _instanceListener = null;
        if (_pauseHotkeyRegistered && IsHandleCreated)
        {
            NativeMethods.UnregisterHotKey(
                Handle,
                PauseHotkeyId);

            _pauseHotkeyRegistered = false;
        }

        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        _trayMenu.Dispose();

        base.OnFormClosed(e);
    }

    private void StartInstanceListener()
    {
        if (_instanceListener is not null)
        {
            return;
        }

        try
        {
            (_, string pipeName) =
                SingleInstanceService.GetNames();

            _instanceListener = new SingleInstanceListener(
                pipeName,
                OnShowRequestedBySecondInstance,
                OnInstanceListenerError);

            _instanceListener.Start();
        }
        catch (Exception ex) when (
            ex is InvalidOperationException or
            UnauthorizedAccessException or
            IOException)
        {
            lblStatus.Text =
                $"No se pudo iniciar el receptor de instancia única: " +
                ex.Message;
        }
    }

    private void OnShowRequestedBySecondInstance()
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        try
        {
            BeginInvoke((Action)(() =>
            {
                if (!IsDisposed)
                {
                    ShowMainWindow();
                }
            }));
        }
        catch (InvalidOperationException)
        {
            // La ventana está cerrándose.
        }
    }

    private void OnInstanceListenerError(Exception ex)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        try
        {
            BeginInvoke((Action)(() =>
            {
                if (IsDisposed)
                {
                    return;
                }

                lblStatus.Text =
                    $"El receptor de instancia única se detuvo: " +
                    ex.Message;

                if (_trayIcon.Visible)
                {
                    _trayIcon.ShowBalloonTip(
                        5000,
                        "TeclaLibre",
                        "No se podrán recibir solicitudes para abrir " +
                        "la ventana desde una segunda ejecución.",
                        ToolTipIcon.Warning);
                }
            }));
        }
        catch (InvalidOperationException)
        {
            // La ventana está cerrándose.
        }
    }
}