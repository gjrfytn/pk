namespace PK.Forms
{
    partial class MasterExaminations
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
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bSave = new System.Windows.Forms.Button();
            this.bSetDate = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridView_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_ApplD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Faculty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Profile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Mark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Bonus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Chair = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Program = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(12, 12);
            this.dtpDate.MaxDate = new System.DateTime(2030, 12, 31, 0, 0, 0, 0);
            this.dtpDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(90, 20);
            this.dtpDate.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bSave);
            this.panel1.Controls.Add(this.bSetDate);
            this.panel1.Controls.Add(this.dtpDate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(884, 47);
            this.panel1.TabIndex = 0;
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(305, 11);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 2;
            this.bSave.Text = "Сохранить";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bSetDate
            // 
            this.bSetDate.Location = new System.Drawing.Point(108, 11);
            this.bSetDate.Name = "bSetDate";
            this.bSetDate.Size = new System.Drawing.Size(191, 23);
            this.bSetDate.TabIndex = 1;
            this.bSetDate.Text = "Проставить всем выбранную дату";
            this.bSetDate.UseVisualStyleBackColor = true;
            this.bSetDate.Click += new System.EventHandler(this.bSetDate_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_ID,
            this.dataGridView_ApplD,
            this.dataGridView_Faculty,
            this.dataGridView_Direction,
            this.dataGridView_Profile,
            this.dataGridView_Name,
            this.dataGridView_Mark,
            this.dataGridView_Bonus,
            this.dataGridView_Date,
            this.dataGridView_Chair,
            this.dataGridView_Program});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 47);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(884, 514);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // dataGridView_ID
            // 
            this.dataGridView_ID.HeaderText = "ID";
            this.dataGridView_ID.Name = "dataGridView_ID";
            this.dataGridView_ID.Visible = false;
            // 
            // dataGridView_ApplD
            // 
            this.dataGridView_ApplD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_ApplD.HeaderText = "УИН";
            this.dataGridView_ApplD.Name = "dataGridView_ApplD";
            this.dataGridView_ApplD.ReadOnly = true;
            this.dataGridView_ApplD.Width = 50;
            // 
            // dataGridView_Faculty
            // 
            this.dataGridView_Faculty.HeaderText = "Факультет";
            this.dataGridView_Faculty.Name = "dataGridView_Faculty";
            this.dataGridView_Faculty.Visible = false;
            // 
            // dataGridView_Direction
            // 
            this.dataGridView_Direction.HeaderText = "Направление";
            this.dataGridView_Direction.Name = "dataGridView_Direction";
            this.dataGridView_Direction.Visible = false;
            // 
            // dataGridView_Profile
            // 
            this.dataGridView_Profile.HeaderText = "Профиль";
            this.dataGridView_Profile.Name = "dataGridView_Profile";
            this.dataGridView_Profile.Visible = false;
            // 
            // dataGridView_Name
            // 
            this.dataGridView_Name.HeaderText = "ФИО";
            this.dataGridView_Name.MinimumWidth = 100;
            this.dataGridView_Name.Name = "dataGridView_Name";
            this.dataGridView_Name.ReadOnly = true;
            // 
            // dataGridView_Mark
            // 
            this.dataGridView_Mark.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_Mark.HeaderText = "Оценка";
            this.dataGridView_Mark.Name = "dataGridView_Mark";
            this.dataGridView_Mark.Width = 50;
            // 
            // dataGridView_Bonus
            // 
            this.dataGridView_Bonus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_Bonus.HeaderText = "Доп. баллы";
            this.dataGridView_Bonus.Name = "dataGridView_Bonus";
            this.dataGridView_Bonus.Width = 50;
            // 
            // dataGridView_Date
            // 
            this.dataGridView_Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_Date.HeaderText = "Дата экзамена";
            this.dataGridView_Date.Name = "dataGridView_Date";
            this.dataGridView_Date.Width = 65;
            // 
            // dataGridView_Chair
            // 
            this.dataGridView_Chair.FillWeight = 80F;
            this.dataGridView_Chair.HeaderText = "Кафедра";
            this.dataGridView_Chair.MinimumWidth = 100;
            this.dataGridView_Chair.Name = "dataGridView_Chair";
            this.dataGridView_Chair.ReadOnly = true;
            // 
            // dataGridView_Program
            // 
            this.dataGridView_Program.HeaderText = "Программа";
            this.dataGridView_Program.MinimumWidth = 100;
            this.dataGridView_Program.Name = "dataGridView_Program";
            this.dataGridView_Program.ReadOnly = true;
            // 
            // MasterExaminations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel1);
            this.Icon = global::PK.Properties.Resources.logo;
            this.Name = "MasterExaminations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Оценки магистров";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MasterExaminations_FormClosing);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button bSetDate;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ApplD;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Faculty;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Direction;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Profile;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Mark;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Bonus;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Chair;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Program;
    }
}