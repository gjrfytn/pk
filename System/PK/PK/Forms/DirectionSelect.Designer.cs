namespace PK.Forms
{
    partial class DirectionSelect
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
            this.dgvDirectionSelection = new System.Windows.Forms.DataGridView();
            this.dgvDirectionSelection_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirectionSelection_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirectionSelection_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirectionSelection_Faculty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.btSelect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirectionSelection)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDirectionSelection
            // 
            this.dgvDirectionSelection.AllowUserToAddRows = false;
            this.dgvDirectionSelection.AllowUserToDeleteRows = false;
            this.dgvDirectionSelection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDirectionSelection.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvDirectionSelection_ID,
            this.dgvDirectionSelection_Code,
            this.dgvDirectionSelection_Name,
            this.dgvDirectionSelection_Faculty});
            this.dgvDirectionSelection.Location = new System.Drawing.Point(13, 31);
            this.dgvDirectionSelection.MultiSelect = false;
            this.dgvDirectionSelection.Name = "dgvDirectionSelection";
            this.dgvDirectionSelection.ReadOnly = true;
            this.dgvDirectionSelection.RowHeadersVisible = false;
            this.dgvDirectionSelection.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDirectionSelection.Size = new System.Drawing.Size(612, 353);
            this.dgvDirectionSelection.TabIndex = 0;
            // 
            // dgvDirectionSelection_ID
            // 
            this.dgvDirectionSelection_ID.HeaderText = "ID";
            this.dgvDirectionSelection_ID.Name = "dgvDirectionSelection_ID";
            this.dgvDirectionSelection_ID.ReadOnly = true;
            this.dgvDirectionSelection_ID.Visible = false;
            // 
            // dgvDirectionSelection_Code
            // 
            this.dgvDirectionSelection_Code.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvDirectionSelection_Code.HeaderText = "Код";
            this.dgvDirectionSelection_Code.Name = "dgvDirectionSelection_Code";
            this.dgvDirectionSelection_Code.ReadOnly = true;
            this.dgvDirectionSelection_Code.Width = 51;
            // 
            // dgvDirectionSelection_Name
            // 
            this.dgvDirectionSelection_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvDirectionSelection_Name.HeaderText = "Наименование";
            this.dgvDirectionSelection_Name.Name = "dgvDirectionSelection_Name";
            this.dgvDirectionSelection_Name.ReadOnly = true;
            // 
            // dgvDirectionSelection_Faculty
            // 
            this.dgvDirectionSelection_Faculty.HeaderText = "Факультет";
            this.dgvDirectionSelection_Faculty.Name = "dgvDirectionSelection_Faculty";
            this.dgvDirectionSelection_Faculty.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(261, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выберите направление:";
            // 
            // btSelect
            // 
            this.btSelect.Location = new System.Drawing.Point(286, 390);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(75, 23);
            this.btSelect.TabIndex = 2;
            this.btSelect.Text = "Выбрать";
            this.btSelect.UseVisualStyleBackColor = true;
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // DirectionSelect
            // 
            this.AcceptButton = this.btSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 423);
            this.Controls.Add(this.btSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvDirectionSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DirectionSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор направления";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirectionSelection)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDirectionSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirectionSelection_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirectionSelection_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirectionSelection_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirectionSelection_Faculty;
    }
}