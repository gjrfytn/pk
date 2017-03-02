namespace PK
{
    partial class FacultiesForm
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
            this.btNewFaculty = new System.Windows.Forms.Button();
            this.dgvFaculties = new System.Windows.Forms.DataGridView();
            this.cShortName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbFacultyShortName = new System.Windows.Forms.TextBox();
            this.tbFacultyName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFaculties)).BeginInit();
            this.SuspendLayout();
            // 
            // btNewFaculty
            // 
            this.btNewFaculty.Location = new System.Drawing.Point(235, 384);
            this.btNewFaculty.Name = "btNewFaculty";
            this.btNewFaculty.Size = new System.Drawing.Size(136, 23);
            this.btNewFaculty.TabIndex = 0;
            this.btNewFaculty.Text = "Добавить факультет";
            this.btNewFaculty.UseVisualStyleBackColor = true;
            this.btNewFaculty.Click += new System.EventHandler(this.btNewFaculty_Click);
            // 
            // dgvFaculties
            // 
            this.dgvFaculties.AllowUserToAddRows = false;
            this.dgvFaculties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFaculties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cShortName,
            this.cName});
            this.dgvFaculties.Location = new System.Drawing.Point(13, 12);
            this.dgvFaculties.Name = "dgvFaculties";
            this.dgvFaculties.ReadOnly = true;
            this.dgvFaculties.RowHeadersVisible = false;
            this.dgvFaculties.Size = new System.Drawing.Size(559, 316);
            this.dgvFaculties.TabIndex = 1;
            // 
            // cShortName
            // 
            this.cShortName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.cShortName.HeaderText = "Краткое название";
            this.cShortName.Name = "cShortName";
            this.cShortName.ReadOnly = true;
            this.cShortName.Width = 114;
            // 
            // cName
            // 
            this.cName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cName.HeaderText = "Название факультета";
            this.cName.Name = "cName";
            this.cName.ReadOnly = true;
            // 
            // tbFacultyShortName
            // 
            this.tbFacultyShortName.Enabled = false;
            this.tbFacultyShortName.Location = new System.Drawing.Point(446, 348);
            this.tbFacultyShortName.Name = "tbFacultyShortName";
            this.tbFacultyShortName.Size = new System.Drawing.Size(47, 20);
            this.tbFacultyShortName.TabIndex = 2;
            this.tbFacultyShortName.Visible = false;
            // 
            // tbFacultyName
            // 
            this.tbFacultyName.Enabled = false;
            this.tbFacultyName.Location = new System.Drawing.Point(79, 348);
            this.tbFacultyName.Name = "tbFacultyName";
            this.tbFacultyName.Size = new System.Drawing.Size(248, 20);
            this.tbFacultyName.TabIndex = 3;
            this.tbFacultyName.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(13, 351);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Название:";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(337, 351);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Краткое название:";
            this.label2.Visible = false;
            // 
            // btSave
            // 
            this.btSave.Enabled = false;
            this.btSave.Location = new System.Drawing.Point(503, 346);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(70, 23);
            this.btSave.TabIndex = 6;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Visible = false;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // FacultiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 419);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbFacultyName);
            this.Controls.Add(this.tbFacultyShortName);
            this.Controls.Add(this.dgvFaculties);
            this.Controls.Add(this.btNewFaculty);
            this.Name = "FacultiesForm";
            this.Text = "FacultiesForm";
            this.Load += new System.EventHandler(this.FacultiesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFaculties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btNewFaculty;
        private System.Windows.Forms.DataGridView dgvFaculties;
        private System.Windows.Forms.DataGridViewTextBoxColumn cShortName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.TextBox tbFacultyShortName;
        private System.Windows.Forms.TextBox tbFacultyName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btSave;
    }
}