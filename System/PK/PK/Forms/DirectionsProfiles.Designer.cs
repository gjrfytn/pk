namespace PK.Forms
{
    partial class DirectionsProfiles
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
            this.dgvDirections = new System.Windows.Forms.DataGridView();
            this.dgvDirections_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_ShortName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_EduLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_FacultyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDirections_Kafedra = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btAddProfile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDirections = new System.Windows.Forms.ComboBox();
            this.gbType = new System.Windows.Forms.GroupBox();
            this.rbMag = new System.Windows.Forms.RadioButton();
            this.rbSpec = new System.Windows.Forms.RadioButton();
            this.rbBacc = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbFaculties = new System.Windows.Forms.ComboBox();
            this.btDelete = new System.Windows.Forms.Button();
            this.tbShortName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbKafedra = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirections)).BeginInit();
            this.gbType.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDirections
            // 
            this.dgvDirections.AllowUserToAddRows = false;
            this.dgvDirections.AllowUserToDeleteRows = false;
            this.dgvDirections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDirections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvDirections_ID,
            this.dgvDirections_Type,
            this.dgvDirections_Name,
            this.dgvDirections_ShortName,
            this.dgvDirections_Code,
            this.dgvDirections_EduLevel,
            this.dgvDirections_FacultyName,
            this.dgvDirections_Kafedra});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDirections.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDirections.Location = new System.Drawing.Point(13, 13);
            this.dgvDirections.MultiSelect = false;
            this.dgvDirections.Name = "dgvDirections";
            this.dgvDirections.RowHeadersVisible = false;
            this.dgvDirections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDirections.Size = new System.Drawing.Size(997, 335);
            this.dgvDirections.TabIndex = 0;
            this.dgvDirections.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDirections_CellEndEdit);
            // 
            // dgvDirections_ID
            // 
            this.dgvDirections_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvDirections_ID.HeaderText = "ID";
            this.dgvDirections_ID.Name = "dgvDirections_ID";
            this.dgvDirections_ID.ReadOnly = true;
            this.dgvDirections_ID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvDirections_ID.Visible = false;
            // 
            // dgvDirections_Type
            // 
            this.dgvDirections_Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvDirections_Type.HeaderText = "";
            this.dgvDirections_Type.Name = "dgvDirections_Type";
            this.dgvDirections_Type.ReadOnly = true;
            this.dgvDirections_Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvDirections_Type.Visible = false;
            // 
            // dgvDirections_Name
            // 
            this.dgvDirections_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvDirections_Name.HeaderText = "Направление подготовки/специальность";
            this.dgvDirections_Name.Name = "dgvDirections_Name";
            this.dgvDirections_Name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvDirections_ShortName
            // 
            this.dgvDirections_ShortName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvDirections_ShortName.HeaderText = "Сокр.";
            this.dgvDirections_ShortName.Name = "dgvDirections_ShortName";
            this.dgvDirections_ShortName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvDirections_ShortName.Width = 60;
            // 
            // dgvDirections_Code
            // 
            this.dgvDirections_Code.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvDirections_Code.HeaderText = "Код направления/ специальности";
            this.dgvDirections_Code.Name = "dgvDirections_Code";
            this.dgvDirections_Code.ReadOnly = true;
            this.dgvDirections_Code.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvDirections_Code.Width = 90;
            // 
            // dgvDirections_EduLevel
            // 
            this.dgvDirections_EduLevel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvDirections_EduLevel.HeaderText = "Программа обучения";
            this.dgvDirections_EduLevel.Name = "dgvDirections_EduLevel";
            this.dgvDirections_EduLevel.ReadOnly = true;
            this.dgvDirections_EduLevel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvDirections_EduLevel.Width = 109;
            // 
            // dgvDirections_FacultyName
            // 
            this.dgvDirections_FacultyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvDirections_FacultyName.HeaderText = "Факультет";
            this.dgvDirections_FacultyName.Name = "dgvDirections_FacultyName";
            this.dgvDirections_FacultyName.ReadOnly = true;
            this.dgvDirections_FacultyName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvDirections_FacultyName.Width = 70;
            // 
            // dgvDirections_Kafedra
            // 
            this.dgvDirections_Kafedra.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvDirections_Kafedra.HeaderText = "Кафедра";
            this.dgvDirections_Kafedra.Name = "dgvDirections_Kafedra";
            this.dgvDirections_Kafedra.ReadOnly = true;
            this.dgvDirections_Kafedra.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvDirections_Kafedra.Width = 70;
            // 
            // btAddProfile
            // 
            this.btAddProfile.Location = new System.Drawing.Point(13, 365);
            this.btAddProfile.Name = "btAddProfile";
            this.btAddProfile.Size = new System.Drawing.Size(75, 23);
            this.btAddProfile.TabIndex = 1;
            this.btAddProfile.Text = "Добавить";
            this.btAddProfile.UseVisualStyleBackColor = true;
            this.btAddProfile.Click += new System.EventHandler(this.btAddProfile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(604, 367);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Направление:";
            // 
            // cbDirections
            // 
            this.cbDirections.Enabled = false;
            this.cbDirections.FormattingEnabled = true;
            this.cbDirections.Location = new System.Drawing.Point(484, 389);
            this.cbDirections.Name = "cbDirections";
            this.cbDirections.Size = new System.Drawing.Size(336, 21);
            this.cbDirections.TabIndex = 3;
            this.cbDirections.SelectedIndexChanged += new System.EventHandler(this.cbDirections_SelectedIndexChanged);
            // 
            // gbType
            // 
            this.gbType.Controls.Add(this.rbMag);
            this.gbType.Controls.Add(this.rbSpec);
            this.gbType.Controls.Add(this.rbBacc);
            this.gbType.Enabled = false;
            this.gbType.Location = new System.Drawing.Point(161, 367);
            this.gbType.Name = "gbType";
            this.gbType.Size = new System.Drawing.Size(299, 43);
            this.gbType.TabIndex = 5;
            this.gbType.TabStop = false;
            this.gbType.Text = "Программа обучения";
            // 
            // rbMag
            // 
            this.rbMag.AutoSize = true;
            this.rbMag.Location = new System.Drawing.Point(201, 20);
            this.rbMag.Name = "rbMag";
            this.rbMag.Size = new System.Drawing.Size(96, 17);
            this.rbMag.TabIndex = 2;
            this.rbMag.TabStop = true;
            this.rbMag.Text = "Магистратура";
            this.rbMag.UseVisualStyleBackColor = true;
            this.rbMag.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbSpec
            // 
            this.rbSpec.AutoSize = true;
            this.rbSpec.Location = new System.Drawing.Point(104, 21);
            this.rbSpec.Name = "rbSpec";
            this.rbSpec.Size = new System.Drawing.Size(90, 17);
            this.rbSpec.TabIndex = 1;
            this.rbSpec.TabStop = true;
            this.rbSpec.Text = "Специалитет";
            this.rbSpec.UseVisualStyleBackColor = true;
            this.rbSpec.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbBacc
            // 
            this.rbBacc.AutoSize = true;
            this.rbBacc.Location = new System.Drawing.Point(7, 20);
            this.rbBacc.Name = "rbBacc";
            this.rbBacc.Size = new System.Drawing.Size(91, 17);
            this.rbBacc.TabIndex = 0;
            this.rbBacc.TabStop = true;
            this.rbBacc.Text = "Бакалавриат";
            this.rbBacc.UseVisualStyleBackColor = true;
            this.rbBacc.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(355, 428);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Название:";
            // 
            // tbName
            // 
            this.tbName.Enabled = false;
            this.tbName.Location = new System.Drawing.Point(433, 425);
            this.tbName.MaxLength = 150;
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(351, 20);
            this.tbName.TabIndex = 7;
            // 
            // btSave
            // 
            this.btSave.Enabled = false;
            this.btSave.Location = new System.Drawing.Point(13, 423);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 8;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(850, 390);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Факультет:";
            // 
            // cbFaculties
            // 
            this.cbFaculties.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFaculties.Enabled = false;
            this.cbFaculties.FormattingEnabled = true;
            this.cbFaculties.Location = new System.Drawing.Point(922, 386);
            this.cbFaculties.Name = "cbFaculties";
            this.cbFaculties.Size = new System.Drawing.Size(88, 21);
            this.cbFaculties.TabIndex = 10;
            // 
            // btDelete
            // 
            this.btDelete.Location = new System.Drawing.Point(12, 394);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(131, 23);
            this.btDelete.TabIndex = 11;
            this.btDelete.Text = "Удалить выделенное";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // tbShortName
            // 
            this.tbShortName.Enabled = false;
            this.tbShortName.Location = new System.Drawing.Point(883, 425);
            this.tbShortName.MaxLength = 5;
            this.tbShortName.Name = "tbShortName";
            this.tbShortName.Size = new System.Drawing.Size(127, 20);
            this.tbShortName.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(803, 429);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Сокращение:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(165, 428);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Кафедра:";
            // 
            // tbKafedra
            // 
            this.tbKafedra.Enabled = false;
            this.tbKafedra.Location = new System.Drawing.Point(235, 425);
            this.tbKafedra.MaxLength = 149;
            this.tbKafedra.Name = "tbKafedra";
            this.tbKafedra.Size = new System.Drawing.Size(100, 20);
            this.tbKafedra.TabIndex = 15;
            // 
            // DirectionsProfiles
            // 
            this.AcceptButton = this.btSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 458);
            this.Controls.Add(this.tbKafedra);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbShortName);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.cbFaculties);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gbType);
            this.Controls.Add(this.cbDirections);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btAddProfile);
            this.Controls.Add(this.dgvDirections);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DirectionsProfiles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Направления, профили и магистерские программы";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirections)).EndInit();
            this.gbType.ResumeLayout(false);
            this.gbType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDirections;
        private System.Windows.Forms.Button btAddProfile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDirections;
        private System.Windows.Forms.GroupBox gbType;
        private System.Windows.Forms.RadioButton rbMag;
        private System.Windows.Forms.RadioButton rbSpec;
        private System.Windows.Forms.RadioButton rbBacc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbFaculties;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.TextBox tbShortName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbKafedra;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_ShortName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_EduLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_FacultyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDirections_Kafedra;
    }
}