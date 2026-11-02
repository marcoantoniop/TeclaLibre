namespace TeclaLibre.Forms
{
    partial class RuleEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblName = new Label();
            txtName = new TextBox();
            chkEnabled = new CheckBox();
            lblTrigger = new Label();
            txtTrigger = new TextBox();
            btnCaptureTrigger = new Button();
            chkExact = new CheckBox();
            lblOutputKind = new Label();
            cmbOutputKind = new ComboBox();
            lblOutput = new Label();
            txtOutput = new TextBox();
            btnCaptureOutput = new Button();
            chkRepeat = new CheckBox();
            btnSave = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(20, 25);
            lblName.Name = "lblName";
            lblName.Size = new Size(54, 15);
            lblName.TabIndex = 0;
            lblName.Text = "Nombre:";
            // 
            // txtName
            // 
            txtName.Location = new Point(80, 22);
            txtName.Name = "txtName";
            txtName.Size = new Size(197, 23);
            txtName.TabIndex = 1;
            // 
            // chkEnabled
            // 
            chkEnabled.AutoSize = true;
            chkEnabled.Location = new Point(283, 24);
            chkEnabled.Name = "chkEnabled";
            chkEnabled.Size = new Size(81, 19);
            chkEnabled.TabIndex = 2;
            chkEnabled.Text = "Habilitado";
            chkEnabled.UseVisualStyleBackColor = true;
            // 
            // lblTrigger
            // 
            lblTrigger.AutoSize = true;
            lblTrigger.Location = new Point(20, 56);
            lblTrigger.Name = "lblTrigger";
            lblTrigger.Size = new Size(50, 15);
            lblTrigger.TabIndex = 3;
            lblTrigger.Text = "Entrada:";
            // 
            // txtTrigger
            // 
            txtTrigger.Location = new Point(80, 51);
            txtTrigger.Name = "txtTrigger";
            txtTrigger.ReadOnly = true;
            txtTrigger.Size = new Size(197, 23);
            txtTrigger.TabIndex = 4;
            // 
            // btnCaptureTrigger
            // 
            btnCaptureTrigger.Location = new Point(289, 51);
            btnCaptureTrigger.Name = "btnCaptureTrigger";
            btnCaptureTrigger.Size = new Size(75, 23);
            btnCaptureTrigger.TabIndex = 5;
            btnCaptureTrigger.Text = "Capturar...";
            btnCaptureTrigger.UseVisualStyleBackColor = true;
            // 
            // chkExact
            // 
            chkExact.AutoSize = true;
            chkExact.Location = new Point(381, 54);
            chkExact.Name = "chkExact";
            chkExact.Size = new Size(227, 19);
            chkExact.TabIndex = 6;
            chkExact.Text = "Coincidencia exacta de modificadores";
            chkExact.UseVisualStyleBackColor = true;
            // 
            // lblOutputKind
            // 
            lblOutputKind.AutoSize = true;
            lblOutputKind.Location = new Point(20, 89);
            lblOutputKind.Name = "lblOutputKind";
            lblOutputKind.Size = new Size(83, 15);
            lblOutputKind.TabIndex = 7;
            lblOutputKind.Text = "Tipo de Salida:";
            // 
            // cmbOutputKind
            // 
            cmbOutputKind.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOutputKind.FormattingEnabled = true;
            cmbOutputKind.Location = new Point(109, 86);
            cmbOutputKind.Name = "cmbOutputKind";
            cmbOutputKind.Size = new Size(168, 23);
            cmbOutputKind.TabIndex = 8;
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Location = new Point(20, 122);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(41, 15);
            lblOutput.TabIndex = 9;
            lblOutput.Text = "Salida:";
            // 
            // txtOutput
            // 
            txtOutput.Location = new Point(80, 119);
            txtOutput.Name = "txtOutput";
            txtOutput.Size = new Size(197, 23);
            txtOutput.TabIndex = 10;
            // 
            // btnCaptureOutput
            // 
            btnCaptureOutput.Location = new Point(289, 119);
            btnCaptureOutput.Name = "btnCaptureOutput";
            btnCaptureOutput.Size = new Size(75, 23);
            btnCaptureOutput.TabIndex = 11;
            btnCaptureOutput.Text = "Capturar...";
            btnCaptureOutput.UseVisualStyleBackColor = true;
            // 
            // chkRepeat
            // 
            chkRepeat.AutoSize = true;
            chkRepeat.Location = new Point(381, 121);
            chkRepeat.Name = "chkRepeat";
            chkRepeat.Size = new Size(223, 19);
            chkRepeat.TabIndex = 12;
            chkRepeat.Text = "Repetir mientras se mantenga la tecla";
            chkRepeat.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(143, 178);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 13;
            btnSave.Text = "Guardar";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(261, 178);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 14;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // RuleEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(704, 361);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(chkRepeat);
            Controls.Add(btnCaptureOutput);
            Controls.Add(txtOutput);
            Controls.Add(lblOutput);
            Controls.Add(cmbOutputKind);
            Controls.Add(lblOutputKind);
            Controls.Add(chkExact);
            Controls.Add(btnCaptureTrigger);
            Controls.Add(txtTrigger);
            Controls.Add(lblTrigger);
            Controls.Add(chkEnabled);
            Controls.Add(txtName);
            Controls.Add(lblName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RuleEditorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Editar regla";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblName;
        private TextBox txtName;
        private CheckBox chkEnabled;
        private Label lblTrigger;
        private TextBox txtTrigger;
        private Button btnCaptureTrigger;
        private CheckBox chkExact;
        private Label lblOutputKind;
        private ComboBox cmbOutputKind;
        private Label lblOutput;
        private TextBox txtOutput;
        private Button btnCaptureOutput;
        private CheckBox chkRepeat;
        private Button btnSave;
        private Button btnCancel;
    }
}