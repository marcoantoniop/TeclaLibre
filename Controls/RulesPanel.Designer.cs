namespace TeclaLibre.Controls
{
    partial class RulesPanel
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
            layoutRules = new TableLayoutPanel();
            flowRuleButtons = new FlowLayoutPanel();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDuplicate = new Button();
            btnToggle = new Button();
            btnDelete = new Button();
            gridRules = new DataGridView();
            layoutRules.SuspendLayout();
            flowRuleButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridRules).BeginInit();
            SuspendLayout();
            // 
            // layoutRules
            // 
            layoutRules.ColumnCount = 1;
            layoutRules.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRules.Controls.Add(flowRuleButtons, 0, 0);
            layoutRules.Controls.Add(gridRules, 0, 1);
            layoutRules.Dock = DockStyle.Fill;
            layoutRules.Location = new Point(0, 0);
            layoutRules.Name = "layoutRules";
            layoutRules.Padding = new Padding(8);
            layoutRules.RowCount = 2;
            layoutRules.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            layoutRules.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRules.Size = new Size(900, 500);
            layoutRules.TabIndex = 0;
            // 
            // flowRuleButtons
            // 
            flowRuleButtons.Controls.Add(btnAdd);
            flowRuleButtons.Controls.Add(btnEdit);
            flowRuleButtons.Controls.Add(btnDuplicate);
            flowRuleButtons.Controls.Add(btnToggle);
            flowRuleButtons.Controls.Add(btnDelete);
            flowRuleButtons.Dock = DockStyle.Fill;
            flowRuleButtons.Location = new Point(11, 11);
            flowRuleButtons.Name = "flowRuleButtons";
            flowRuleButtons.Size = new Size(878, 46);
            flowRuleButtons.TabIndex = 0;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(3, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Nueva";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(84, 3);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(75, 23);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "Editar";
            btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnDuplicate
            // 
            btnDuplicate.Location = new Point(165, 3);
            btnDuplicate.Name = "btnDuplicate";
            btnDuplicate.Size = new Size(75, 23);
            btnDuplicate.TabIndex = 2;
            btnDuplicate.Text = "Duplicar";
            btnDuplicate.UseVisualStyleBackColor = true;
            // 
            // btnToggle
            // 
            btnToggle.Location = new Point(246, 3);
            btnToggle.Name = "btnToggle";
            btnToggle.Size = new Size(112, 23);
            btnToggle.TabIndex = 3;
            btnToggle.Text = "Activar / Pausar";
            btnToggle.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(364, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Eliminar";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // gridRules
            // 
            gridRules.AllowUserToAddRows = false;
            gridRules.AllowUserToDeleteRows = false;
            gridRules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridRules.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRules.Dock = DockStyle.Fill;
            gridRules.Location = new Point(11, 63);
            gridRules.MultiSelect = false;
            gridRules.Name = "gridRules";
            gridRules.ReadOnly = true;
            gridRules.RowHeadersVisible = false;
            gridRules.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridRules.Size = new Size(878, 426);
            gridRules.TabIndex = 1;
            // 
            // RulesPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(layoutRules);
            Name = "RulesPanel";
            Size = new Size(900, 500);
            layoutRules.ResumeLayout(false);
            flowRuleButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridRules).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel layoutRules;
        private FlowLayoutPanel flowRuleButtons;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDuplicate;
        private Button btnToggle;
        private Button btnDelete;
        private DataGridView gridRules;
    }
}
