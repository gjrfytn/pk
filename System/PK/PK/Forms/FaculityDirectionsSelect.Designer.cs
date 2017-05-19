namespace PK.Forms
{
    partial class FaculityDirectionsSelect
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvDirections_ = new System.Windows.Forms.DataGridView();
            this.dgvDirections_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvDirections_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_ShortName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirections_)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDirections_
            // 
            this.dgvDirections_.AllowUserToAddRows = false;
            this.dgvDirections_.AllowUserToDeleteRows = false;
            this.dgvDirections_.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDirections_.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvDirections_ID,
            this.dgvDirections_Select,
            this.dgvDirections_Name,
            this.dgvDirections_Code,
            this.dgvDirections_Type,
            this.dgvDirections_ShortName});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDirections_.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDirections_.Location = new System.Drawing.Point(13, 34);
            this.dgvDirections_.Name = "dgvDirections_";
            this.dgvDirections_.RowHeadersVisible = false;
            this.dgvDirections_.Size = new System.Drawing.Size(666, 436);
            this.dgvDirections_.TabIndex = 0;
            // 
            // dgvDirections_ID
            // 
            this.dgvDirections_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvDirections_ID.HeaderText = "ID";
            this.dgvDirections_ID.Name = "dgvDirections_ID";
            this.dgvDirections_ID.ReadOnly = true;
            this.dgvDirections_ID.Visible = false;
            // 
            // dgvDirections_Select
            // 
            this.dgvDirections_Select.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvDirections_Select.HeaderText = "Выбрать";
            this.dgvDirections_Select.Name = "dgvDirections_Select";
            this.dgvDirections_Select.Width = 57;
            // 
            // dgvDirections_Name
            // 
            this.dgvDirections_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvDirections_Name.HeaderText = "Наименование направления";
            this.dgvDirections_Name.Name = "dgvDirections_Name";
            this.dgvDirections_Name.ReadOnly = true;
            // 
            // dgvDirections_Code
            // 
            this.dgvDirections_Code.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvDirections_Code.HeaderText = "Код направления";
            this.dgvDirections_Code.Name = "dgvDirections_Code";
            this.dgvDirections_Code.ReadOnly = true;
            this.dgvDirections_Code.Width = 110;
            // 
            // dgvDirections_Type
            // 
            this.dgvDirections_Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvDirections_Type.HeaderText = "Программа обучения";
            this.dgvDirections_Type.Name = "dgvDirections_Type";
            this.dgvDirections_Type.ReadOnly = true;
            this.dgvDirections_Type.Width = 128;
            // 
            // dgvDirections_ShortName
            // 
            this.dgvDirections_ShortName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvDirections_ShortName.HeaderText = "Сокращение";
            this.dgvDirections_ShortName.Name = "dgvDirections_ShortName";
            this.dgvDirections_ShortName.Width = 80;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(291, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Направления факультета отмечаются галочками слева";
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(319, 484);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 2;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // FaculityDirectionsSelect
            // 
            this.AcceptButton = this.btSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 519);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvDirections_);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FaculityDirectionsSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Выбор направлений факультета";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirections_)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDirections_;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_ID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvDirections_Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_ShortName;
    }
}