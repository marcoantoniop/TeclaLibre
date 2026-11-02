namespace TeclaLibre.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            layoutMain = new TableLayoutPanel();
            layoutHeader = new TableLayoutPanel();
            lblState = new Label();
            btnToggleEnabled = new Button();
            tabMain = new TabControl();
            tabTests = new TabPage();
            tabMappings = new TabPage();
            tabSettings = new TabPage();
            lblSettingsStatus = new Label();
            btnApplySettings = new Button();
            chkPauseHotkey = new CheckBox();
            chkCloseToTray = new CheckBox();
            chkActivateOnStartup = new CheckBox();
            chkStartMinimized = new CheckBox();
            chkStartWithWindows = new CheckBox();
            btnExportConfiguration = new Button();
            btnImportConfiguration = new Button();
            btnSaveConfiguration = new Button();
            lblConfigPath = new Label();
            lblConfigDescription = new Label();
            statusMain = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            pictureBox1 = new PictureBox();
            layoutMain.SuspendLayout();
            layoutHeader.SuspendLayout();
            tabMain.SuspendLayout();
            tabSettings.SuspendLayout();
            statusMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // layoutMain
            // 
            layoutMain.ColumnCount = 1;
            layoutMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutMain.Controls.Add(layoutHeader, 0, 0);
            layoutMain.Controls.Add(tabMain, 0, 1);
            layoutMain.Controls.Add(statusMain, 0, 2);
            layoutMain.Dock = DockStyle.Fill;
            layoutMain.Location = new Point(0, 0);
            layoutMain.Name = "layoutMain";
            layoutMain.Padding = new Padding(12);
            layoutMain.RowCount = 3;
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layoutMain.Size = new Size(871, 566);
            layoutMain.TabIndex = 0;
            // 
            // layoutHeader
            // 
            layoutHeader.ColumnCount = 3;
            layoutHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            layoutHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            layoutHeader.Controls.Add(lblState, 1, 0);
            layoutHeader.Controls.Add(btnToggleEnabled, 2, 0);
            layoutHeader.Controls.Add(pictureBox1, 0, 0);
            layoutHeader.Dock = DockStyle.Fill;
            layoutHeader.Location = new Point(12, 12);
            layoutHeader.Margin = new Padding(0);
            layoutHeader.Name = "layoutHeader";
            layoutHeader.RowCount = 1;
            layoutHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutHeader.Size = new Size(847, 64);
            layoutHeader.TabIndex = 0;
            // 
            // lblState
            // 
            lblState.Dock = DockStyle.Fill;
            lblState.Location = new Point(590, 0);
            lblState.Name = "lblState";
            lblState.Size = new Size(134, 64);
            lblState.TabIndex = 1;
            lblState.Text = "Inactivo";
            lblState.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnToggleEnabled
            // 
            btnToggleEnabled.Enabled = false;
            btnToggleEnabled.Location = new Point(730, 3);
            btnToggleEnabled.Name = "btnToggleEnabled";
            btnToggleEnabled.Size = new Size(75, 23);
            btnToggleEnabled.TabIndex = 2;
            btnToggleEnabled.Text = "Activar";
            btnToggleEnabled.UseVisualStyleBackColor = true;
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabTests);
            tabMain.Controls.Add(tabMappings);
            tabMain.Controls.Add(tabSettings);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(12, 84);
            tabMain.Margin = new Padding(0, 8, 0, 8);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(847, 434);
            tabMain.TabIndex = 1;
            // 
            // tabTests
            // 
            tabTests.Location = new Point(4, 24);
            tabTests.Name = "tabTests";
            tabTests.Padding = new Padding(3);
            tabTests.Size = new Size(839, 406);
            tabTests.TabIndex = 3;
            tabTests.Text = "Pruebas";
            tabTests.UseVisualStyleBackColor = true;
            // 
            // tabMappings
            // 
            tabMappings.Location = new Point(4, 24);
            tabMappings.Name = "tabMappings";
            tabMappings.Padding = new Padding(8);
            tabMappings.Size = new Size(839, 406);
            tabMappings.TabIndex = 1;
            tabMappings.Text = "Reasignaciones";
            tabMappings.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(lblSettingsStatus);
            tabSettings.Controls.Add(btnApplySettings);
            tabSettings.Controls.Add(chkPauseHotkey);
            tabSettings.Controls.Add(chkCloseToTray);
            tabSettings.Controls.Add(chkActivateOnStartup);
            tabSettings.Controls.Add(chkStartMinimized);
            tabSettings.Controls.Add(chkStartWithWindows);
            tabSettings.Controls.Add(btnExportConfiguration);
            tabSettings.Controls.Add(btnImportConfiguration);
            tabSettings.Controls.Add(btnSaveConfiguration);
            tabSettings.Controls.Add(lblConfigPath);
            tabSettings.Controls.Add(lblConfigDescription);
            tabSettings.Location = new Point(4, 24);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new Padding(8);
            tabSettings.Size = new Size(839, 406);
            tabSettings.TabIndex = 2;
            tabSettings.Text = "Configuración";
            tabSettings.UseVisualStyleBackColor = true;
            // 
            // lblSettingsStatus
            // 
            lblSettingsStatus.AutoSize = true;
            lblSettingsStatus.Location = new Point(180, 235);
            lblSettingsStatus.Name = "lblSettingsStatus";
            lblSettingsStatus.Size = new Size(100, 15);
            lblSettingsStatus.TabIndex = 11;
            lblSettingsStatus.Text = "Preferencias listas";
            // 
            // btnApplySettings
            // 
            btnApplySettings.Location = new Point(33, 231);
            btnApplySettings.Name = "btnApplySettings";
            btnApplySettings.Size = new Size(141, 23);
            btnApplySettings.TabIndex = 10;
            btnApplySettings.Text = "Aplicar preferencias";
            btnApplySettings.UseVisualStyleBackColor = true;
            // 
            // chkPauseHotkey
            // 
            chkPauseHotkey.AutoSize = true;
            chkPauseHotkey.Location = new Point(17, 204);
            chkPauseHotkey.Name = "chkPauseHotkey";
            chkPauseHotkey.Size = new Size(252, 19);
            chkPauseHotkey.TabIndex = 9;
            chkPauseHotkey.Text = "Usar Ctrl + Shift + F9 para activar o detener";
            chkPauseHotkey.UseVisualStyleBackColor = true;
            // 
            // chkCloseToTray
            // 
            chkCloseToTray.AutoSize = true;
            chkCloseToTray.Location = new Point(17, 179);
            chkCloseToTray.Name = "chkCloseToTray";
            chkCloseToTray.Size = new Size(312, 19);
            chkCloseToTray.TabIndex = 8;
            chkCloseToTray.Text = "Al cerrar la ventana, mantener TeclaLibre en la bandeja";
            chkCloseToTray.UseVisualStyleBackColor = true;
            // 
            // chkActivateOnStartup
            // 
            chkActivateOnStartup.AutoSize = true;
            chkActivateOnStartup.Location = new Point(17, 154);
            chkActivateOnStartup.Name = "chkActivateOnStartup";
            chkActivateOnStartup.Size = new Size(220, 19);
            chkActivateOnStartup.TabIndex = 7;
            chkActivateOnStartup.Text = "Activar la captura al iniciar TeclaLibre";
            chkActivateOnStartup.UseVisualStyleBackColor = true;
            // 
            // chkStartMinimized
            // 
            chkStartMinimized.AutoSize = true;
            chkStartMinimized.Location = new Point(17, 129);
            chkStartMinimized.Name = "chkStartMinimized";
            chkStartMinimized.Size = new Size(197, 19);
            chkStartMinimized.TabIndex = 6;
            chkStartMinimized.Text = "Iniciar minimizado en la bandeja";
            chkStartMinimized.UseVisualStyleBackColor = true;
            // 
            // chkStartWithWindows
            // 
            chkStartWithWindows.AutoSize = true;
            chkStartWithWindows.Location = new Point(17, 104);
            chkStartWithWindows.Name = "chkStartWithWindows";
            chkStartWithWindows.Size = new Size(188, 19);
            chkStartWithWindows.TabIndex = 5;
            chkStartWithWindows.Text = "Iniciar TeclaLibre con Windows";
            chkStartWithWindows.UseVisualStyleBackColor = true;
            // 
            // btnExportConfiguration
            // 
            btnExportConfiguration.Location = new Point(269, 64);
            btnExportConfiguration.Name = "btnExportConfiguration";
            btnExportConfiguration.Size = new Size(120, 23);
            btnExportConfiguration.TabIndex = 4;
            btnExportConfiguration.Text = "Exportar...";
            btnExportConfiguration.UseVisualStyleBackColor = true;
            // 
            // btnImportConfiguration
            // 
            btnImportConfiguration.Location = new Point(143, 64);
            btnImportConfiguration.Name = "btnImportConfiguration";
            btnImportConfiguration.Size = new Size(120, 23);
            btnImportConfiguration.TabIndex = 3;
            btnImportConfiguration.Text = "Importar...";
            btnImportConfiguration.UseVisualStyleBackColor = true;
            // 
            // btnSaveConfiguration
            // 
            btnSaveConfiguration.Location = new Point(17, 64);
            btnSaveConfiguration.Name = "btnSaveConfiguration";
            btnSaveConfiguration.Size = new Size(120, 23);
            btnSaveConfiguration.TabIndex = 2;
            btnSaveConfiguration.Text = "Guardar ahora";
            btnSaveConfiguration.UseVisualStyleBackColor = true;
            // 
            // lblConfigPath
            // 
            lblConfigPath.AutoSize = true;
            lblConfigPath.Location = new Point(13, 37);
            lblConfigPath.Name = "lblConfigPath";
            lblConfigPath.Size = new Size(124, 15);
            lblConfigPath.TabIndex = 1;
            lblConfigPath.Text = "Ruta de configuración";
            // 
            // lblConfigDescription
            // 
            lblConfigDescription.AutoSize = true;
            lblConfigDescription.Location = new Point(11, 8);
            lblConfigDescription.Name = "lblConfigDescription";
            lblConfigDescription.Size = new Size(200, 15);
            lblConfigDescription.TabIndex = 0;
            lblConfigDescription.Text = "Configuración y copias de seguridad";
            // 
            // statusMain
            // 
            statusMain.AutoSize = false;
            statusMain.Dock = DockStyle.Fill;
            statusMain.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusMain.Location = new Point(12, 526);
            statusMain.Name = "statusMain";
            statusMain.Size = new Size(847, 28);
            statusMain.SizingGrip = false;
            statusMain.TabIndex = 2;
            statusMain.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(832, 23);
            lblStatus.Spring = true;
            lblStatus.Text = "Motor detenido";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Properties.Resources.logo_titulo;
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(581, 58);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(871, 566);
            Controls.Add(layoutMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(800, 500);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TeclaLibre";
            layoutMain.ResumeLayout(false);
            layoutHeader.ResumeLayout(false);
            tabMain.ResumeLayout(false);
            tabSettings.ResumeLayout(false);
            tabSettings.PerformLayout();
            statusMain.ResumeLayout(false);
            statusMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel layoutMain;
        private TableLayoutPanel layoutHeader;
        private Label lblState;
        private Button btnToggleEnabled;
        private TabControl tabMain;
        
        private TabPage tabMappings;
        private TabPage tabSettings;
        private TabPage tabTests;
        private StatusStrip statusMain;
        private ToolStripStatusLabel lblStatus;
        private Button btnExportConfiguration;
        private Button btnImportConfiguration;
        private Button btnSaveConfiguration;
        private Label lblConfigPath;
        private Label lblConfigDescription;
        private CheckBox chkStartWithWindows;
        private CheckBox chkCloseToTray;
        private CheckBox chkActivateOnStartup;
        private CheckBox chkStartMinimized;
        private Label lblSettingsStatus;
        private Button btnApplySettings;
        private CheckBox chkPauseHotkey;
        private PictureBox pictureBox1;
    }
}
