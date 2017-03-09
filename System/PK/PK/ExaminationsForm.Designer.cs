namespace PK
{
    partial class ExaminationsForm
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
            this.dgvExams = new System.Windows.Forms.DataGridView();
            this.dgvExamsAudiences = new System.Windows.Forms.DataGridView();
            this.dgvExamsAudiences_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvExamsAudiences_Capacity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.dgvExams_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvExams_Subject = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvExams_Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvExams_RegStartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvExams_RegEndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExamsAudiences)).BeginInit();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvExams
            // 
            this.dgvExams.AllowUserToResizeColumns = false;
            this.dgvExams.AllowUserToResizeRows = false;
            this.dgvExams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvExams_ID,
            this.dgvExams_Subject,
            this.dgvExams_Date,
            this.dgvExams_RegStartDate,
            this.dgvExams_RegEndDate});
            this.dgvExams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExams.Location = new System.Drawing.Point(0, 0);
            this.dgvExams.MultiSelect = false;
            this.dgvExams.Name = "dgvExams";
            this.dgvExams.RowHeadersWidth = 30;
            this.dgvExams.Size = new System.Drawing.Size(608, 472);
            this.dgvExams.TabIndex = 0;
            this.dgvExams.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            this.dgvExams.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvExams_RowEnter);
            // 
            // dgvExamsAudiences
            // 
            this.dgvExamsAudiences.AllowUserToResizeColumns = false;
            this.dgvExamsAudiences.AllowUserToResizeRows = false;
            this.dgvExamsAudiences.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExamsAudiences.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvExamsAudiences_Number,
            this.dgvExamsAudiences_Capacity});
            this.dgvExamsAudiences.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExamsAudiences.Location = new System.Drawing.Point(3, 16);
            this.dgvExamsAudiences.MultiSelect = false;
            this.dgvExamsAudiences.Name = "dgvExamsAudiences";
            this.dgvExamsAudiences.RowHeadersWidth = 30;
            this.dgvExamsAudiences.Size = new System.Drawing.Size(132, 453);
            this.dgvExamsAudiences.TabIndex = 1;
            this.dgvExamsAudiences.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            // 
            // dgvExamsAudiences_Number
            // 
            this.dgvExamsAudiences_Number.HeaderText = "Номер";
            this.dgvExamsAudiences_Number.Name = "dgvExamsAudiences_Number";
            this.dgvExamsAudiences_Number.Width = 50;
            // 
            // dgvExamsAudiences_Capacity
            // 
            this.dgvExamsAudiences_Capacity.HeaderText = "Места";
            this.dgvExamsAudiences_Capacity.Name = "dgvExamsAudiences_Capacity";
            this.dgvExamsAudiences_Capacity.Width = 50;
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.dgvExamsAudiences);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox.Location = new System.Drawing.Point(608, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(138, 472);
            this.groupBox.TabIndex = 2;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Аудитории";
            // 
            // dgvExams_ID
            // 
            this.dgvExams_ID.HeaderText = "ID";
            this.dgvExams_ID.Name = "dgvExams_ID";
            this.dgvExams_ID.Visible = false;
            // 
            // dgvExams_Subject
            // 
            this.dgvExams_Subject.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvExams_Subject.HeaderText = "Дисциплина";
            this.dgvExams_Subject.Name = "dgvExams_Subject";
            // 
            // dgvExams_Date
            // 
            this.dgvExams_Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.NullValue = null;
            this.dgvExams_Date.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvExams_Date.HeaderText = "Дата проведения";
            this.dgvExams_Date.Name = "dgvExams_Date";
            // 
            // dgvExams_RegStartDate
            // 
            this.dgvExams_RegStartDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvExams_RegStartDate.HeaderText = "Дата начала потока";
            this.dgvExams_RegStartDate.Name = "dgvExams_RegStartDate";
            // 
            // dgvExams_RegEndDate
            // 
            this.dgvExams_RegEndDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvExams_RegEndDate.HeaderText = "Дата окончания потока";
            this.dgvExams_RegEndDate.Name = "dgvExams_RegEndDate";
            // 
            // ExaminationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 472);
            this.Controls.Add(this.dgvExams);
            this.Controls.Add(this.groupBox);
            this.Name = "ExaminationsForm";
            this.Text = "ExaminationsForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvExams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExamsAudiences)).EndInit();
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvExams;
        private System.Windows.Forms.DataGridView dgvExamsAudiences;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvExamsAudiences_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvExamsAudiences_Capacity;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvExams_ID;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvExams_Subject;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvExams_Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvExams_RegStartDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvExams_RegEndDate;
    }
}