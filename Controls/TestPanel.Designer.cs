namespace TeclaLibre.Controls
{
    partial class TestPanel
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            layoutTest = new TableLayoutPanel();
            flowTestButtons = new FlowLayoutPanel();
            btnStartTest = new Button();
            btnStopTest = new Button();
            btnClearTest = new Button();
            btnSendText = new Button();
            btnSendEnter = new Button();
            btnSendShortcut = new Button();
            lblPressed = new Label();
            lvEvents = new ListView();
            txtPreview = new TextBox();
            chkDemoRule = new CheckBox();
            layoutTest.SuspendLayout();
            flowTestButtons.SuspendLayout();
            SuspendLayout();
            // 
            // layoutTest
            // 
            layoutTest.ColumnCount = 1;
            layoutTest.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutTest.Controls.Add(flowTestButtons, 0, 0);
            layoutTest.Controls.Add(lblPressed, 0, 1);
            layoutTest.Controls.Add(lvEvents, 0, 2);
            layoutTest.Controls.Add(txtPreview, 0, 3);
            layoutTest.Dock = DockStyle.Fill;
            layoutTest.Location = new Point(0, 0);
            layoutTest.Name = "layoutTest";
            layoutTest.Padding = new Padding(8);
            layoutTest.RowCount = 4;
            layoutTest.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layoutTest.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            layoutTest.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutTest.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            layoutTest.Size = new Size(900, 500);
            layoutTest.TabIndex = 0;
            // 
            // flowTestButtons
            // 
            flowTestButtons.AutoScroll = true;
            flowTestButtons.Controls.Add(btnStartTest);
            flowTestButtons.Controls.Add(btnStopTest);
            flowTestButtons.Controls.Add(btnClearTest);
            flowTestButtons.Controls.Add(btnSendText);
            flowTestButtons.Controls.Add(btnSendEnter);
            flowTestButtons.Controls.Add(btnSendShortcut);
            flowTestButtons.Controls.Add(chkDemoRule);
            flowTestButtons.Dock = DockStyle.Fill;
            flowTestButtons.Location = new Point(11, 11);
            flowTestButtons.Name = "flowTestButtons";
            flowTestButtons.Size = new Size(878, 42);
            flowTestButtons.TabIndex = 0;
            flowTestButtons.WrapContents = false;
            // 
            // btnStartTest
            // 
            btnStartTest.Location = new Point(3, 3);
            btnStartTest.Name = "btnStartTest";
            btnStartTest.Size = new Size(130, 32);
            btnStartTest.TabIndex = 0;
            btnStartTest.Text = "Iniciar Prueba";
            btnStartTest.UseVisualStyleBackColor = true;
            // 
            // btnStopTest
            // 
            btnStopTest.Location = new Point(139, 3);
            btnStopTest.Name = "btnStopTest";
            btnStopTest.Size = new Size(130, 32);
            btnStopTest.TabIndex = 1;
            btnStopTest.Text = "Detener Prueba";
            btnStopTest.UseVisualStyleBackColor = true;
            // 
            // btnClearTest
            // 
            btnClearTest.Location = new Point(275, 3);
            btnClearTest.Name = "btnClearTest";
            btnClearTest.Size = new Size(100, 32);
            btnClearTest.TabIndex = 2;
            btnClearTest.Text = "Limpiar";
            btnClearTest.UseVisualStyleBackColor = true;
            // 
            // btnSendText
            // 
            btnSendText.Location = new Point(381, 3);
            btnSendText.Name = "btnSendText";
            btnSendText.Size = new Size(75, 23);
            btnSendText.TabIndex = 3;
            btnSendText.Text = "Enviar texto";
            btnSendText.UseVisualStyleBackColor = true;
            // 
            // btnSendEnter
            // 
            btnSendEnter.Location = new Point(462, 3);
            btnSendEnter.Name = "btnSendEnter";
            btnSendEnter.Size = new Size(75, 23);
            btnSendEnter.TabIndex = 4;
            btnSendEnter.Text = "Un enter";
            btnSendEnter.UseVisualStyleBackColor = true;
            // 
            // btnSendShortcut
            // 
            btnSendShortcut.Location = new Point(543, 3);
            btnSendShortcut.Name = "btnSendShortcut";
            btnSendShortcut.Size = new Size(120, 23);
            btnSendShortcut.TabIndex = 5;
            btnSendShortcut.Text = "Enviar Control+A";
            btnSendShortcut.UseVisualStyleBackColor = true;
            // 
            // lblPressed
            // 
            lblPressed.Dock = DockStyle.Fill;
            lblPressed.Location = new Point(11, 56);
            lblPressed.Name = "lblPressed";
            lblPressed.Size = new Size(878, 36);
            lblPressed.TabIndex = 1;
            lblPressed.Text = "Teclas presionadas: ninguna";
            lblPressed.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lvEvents
            // 
            lvEvents.Dock = DockStyle.Fill;
            lvEvents.FullRowSelect = true;
            lvEvents.GridLines = true;
            lvEvents.Location = new Point(11, 95);
            lvEvents.MultiSelect = false;
            lvEvents.Name = "lvEvents";
            lvEvents.Size = new Size(878, 294);
            lvEvents.TabIndex = 2;
            lvEvents.UseCompatibleStateImageBehavior = false;
            lvEvents.View = View.Details;
            // 
            // txtPreview
            // 
            txtPreview.AcceptsReturn = true;
            txtPreview.Dock = DockStyle.Fill;
            txtPreview.Location = new Point(11, 395);
            txtPreview.Multiline = true;
            txtPreview.Name = "txtPreview";
            txtPreview.PlaceholderText = "Escribe aquí para probar el teclado...";
            txtPreview.ScrollBars = ScrollBars.Vertical;
            txtPreview.Size = new Size(878, 94);
            txtPreview.TabIndex = 3;
            // 
            // chkDemoRule
            // 
            chkDemoRule.AutoSize = true;
            chkDemoRule.Checked = true;
            chkDemoRule.CheckState = CheckState.Checked;
            chkDemoRule.Location = new Point(669, 3);
            chkDemoRule.Name = "chkDemoRule";
            chkDemoRule.Size = new Size(124, 19);
            chkDemoRule.TabIndex = 6;
            chkDemoRule.Text = "Regla AltGr+Z → >";
            chkDemoRule.UseVisualStyleBackColor = true;
            // 
            // TestPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(layoutTest);
            Name = "TestPanel";
            Size = new Size(900, 500);
            layoutTest.ResumeLayout(false);
            layoutTest.PerformLayout();
            flowTestButtons.ResumeLayout(false);
            flowTestButtons.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel layoutTest;
        private FlowLayoutPanel flowTestButtons;
        private Button btnStartTest;
        private Button btnStopTest;
        private Button btnClearTest;
        private Label lblPressed;
        private ListView lvEvents;
        private TextBox txtPreview;
        private Button btnSendText;
        private Button btnSendEnter;
        private Button btnSendShortcut;
        private CheckBox chkDemoRule;
    }
}
