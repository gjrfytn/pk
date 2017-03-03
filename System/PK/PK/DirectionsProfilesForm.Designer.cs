namespace PK
{
    partial class DirectionsProfilesForm
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
            this.dgvDirections = new System.Windows.Forms.DataGridView();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFacultity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btAddPrifile = new System.Windows.Forms.Button();
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
            this.cName,
            this.cCode,
            this.cType,
            this.cFacultity});
            this.dgvDirections.Location = new System.Drawing.Point(13, 13);
            this.dgvDirections.Name = "dgvDirections";
            this.dgvDirections.ReadOnly = true;
            this.dgvDirections.RowHeadersVisible = false;
            this.dgvDirections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDirections.Size = new System.Drawing.Size(679, 272);
            this.dgvDirections.TabIndex = 0;
            // 
            // cName
            // 
            this.cName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cName.HeaderText = "Направление подготовки/специальность";
            this.cName.Name = "cName";
            this.cName.ReadOnly = true;
            // 
            // cCode
            // 
            this.cCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.cCode.HeaderText = "Код направления/ специальности";
            this.cCode.Name = "cCode";
            this.cCode.ReadOnly = true;
            this.cCode.Width = 186;
            // 
            // cType
            // 
            this.cType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cType.HeaderText = "Программа обучения";
            this.cType.Name = "cType";
            this.cType.ReadOnly = true;
            this.cType.Width = 128;
            // 
            // cFacultity
            // 
            this.cFacultity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.cFacultity.HeaderText = "Факультет";
            this.cFacultity.Name = "cFacultity";
            this.cFacultity.ReadOnly = true;
            this.cFacultity.Width = 88;
            // 
            // btAddPrifile
            // 
            this.btAddPrifile.Location = new System.Drawing.Point(297, 302);
            this.btAddPrifile.Name = "btAddPrifile";
            this.btAddPrifile.Size = new System.Drawing.Size(118, 23);
            this.btAddPrifile.TabIndex = 1;
            this.btAddPrifile.Text = "Добавить профиль";
            this.btAddPrifile.UseVisualStyleBackColor = true;
            this.btAddPrifile.Click += new System.EventHandler(this.btAddPrifile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(486, 335);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Направление:";
            // 
            // cbDirections
            // 
            this.cbDirections.Enabled = false;
            this.cbDirections.FormattingEnabled = true;
            this.cbDirections.Location = new System.Drawing.Point(340, 357);
            this.cbDirections.Name = "cbDirections";
            this.cbDirections.Size = new System.Drawing.Size(352, 21);
            this.cbDirections.Sorted = true;
            this.cbDirections.TabIndex = 3;
            // 
            // gbType
            // 
            this.gbType.Controls.Add(this.rbMag);
            this.gbType.Controls.Add(this.rbSpec);
            this.gbType.Controls.Add(this.rbBacc);
            this.gbType.Enabled = false;
            this.gbType.Location = new System.Drawing.Point(13, 335);
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
            this.rbMag.CheckedChanged += new System.EventHandler(this.rbMag_CheckedChanged);
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
            this.rbSpec.CheckedChanged += new System.EventHandler(this.rbSpec_CheckedChanged);
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
            this.rbBacc.CheckedChanged += new System.EventHandler(this.rbBacc_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(201, 394);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Название:";
            // 
            // tbName
            // 
            this.tbName.Enabled = false;
            this.tbName.Location = new System.Drawing.Point(267, 391);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(425, 20);
            this.tbName.TabIndex = 7;
            // 
            // btSave
            // 
            this.btSave.Enabled = false;
            this.btSave.Location = new System.Drawing.Point(321, 417);
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
            this.label3.Location = new System.Drawing.Point(17, 394);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Факультет:";
            // 
            // cbFaculties
            // 
            this.cbFaculties.Enabled = false;
            this.cbFaculties.FormattingEnabled = true;
            this.cbFaculties.Location = new System.Drawing.Point(89, 390);
            this.cbFaculties.Name = "cbFaculties";
            this.cbFaculties.Size = new System.Drawing.Size(84, 21);
            this.cbFaculties.TabIndex = 10;
            // 
            // DirectionsProfilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 450);
            this.Controls.Add(this.cbFaculties);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gbType);
            this.Controls.Add(this.cbDirections);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btAddPrifile);
            this.Controls.Add(this.dgvDirections);
            this.Name = "DirectionsProfilesForm";
            this.Text = "DictionaryProfilesForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirections)).EndInit();
            this.gbType.ResumeLayout(false);
            this.gbType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDirections;
        private System.Windows.Forms.Button btAddPrifile;
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
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn cType;
        private System.Windows.Forms.DataGridViewTextBoxColumn cFacultity;
    }
}