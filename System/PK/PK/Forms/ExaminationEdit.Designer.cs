namespace PK.Forms
{
    partial class ExaminationEdit
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridView_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Capacity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbSubject = new System.Windows.Forms.ComboBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lSubject = new System.Windows.Forms.Label();
            this.lDate = new System.Windows.Forms.Label();
            this.lRegStartDate = new System.Windows.Forms.Label();
            this.lRegEndDate = new System.Windows.Forms.Label();
            this.dtpRegStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpRegEndDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.bSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_Number,
            this.dataGridView_Capacity});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 16);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 30;
            this.dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView.Size = new System.Drawing.Size(132, 143);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // dataGridView_Number
            // 
            this.dataGridView_Number.DataPropertyName = "number";
            this.dataGridView_Number.HeaderText = "Номер";
            this.dataGridView_Number.Name = "dataGridView_Number";
            this.dataGridView_Number.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridView_Capacity
            // 
            this.dataGridView_Capacity.DataPropertyName = "capacity";
            this.dataGridView_Capacity.HeaderText = "Места";
            this.dataGridView_Capacity.Name = "dataGridView_Capacity";
            this.dataGridView_Capacity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cbSubject
            // 
            this.cbSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubject.FormattingEnabled = true;
            this.cbSubject.Location = new System.Drawing.Point(148, 6);
            this.cbSubject.Name = "cbSubject";
            this.cbSubject.Size = new System.Drawing.Size(167, 21);
            this.cbSubject.TabIndex = 1;
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(148, 33);
            this.dtpDate.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.dtpDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(90, 20);
            this.dtpDate.TabIndex = 3;
            // 
            // lSubject
            // 
            this.lSubject.AutoSize = true;
            this.lSubject.Location = new System.Drawing.Point(69, 9);
            this.lSubject.Name = "lSubject";
            this.lSubject.Size = new System.Drawing.Size(73, 13);
            this.lSubject.TabIndex = 0;
            this.lSubject.Text = "Дисциплина:";
            // 
            // lDate
            // 
            this.lDate.AutoSize = true;
            this.lDate.Location = new System.Drawing.Point(106, 39);
            this.lDate.Name = "lDate";
            this.lDate.Size = new System.Drawing.Size(36, 13);
            this.lDate.TabIndex = 2;
            this.lDate.Text = "Дата:";
            // 
            // lRegStartDate
            // 
            this.lRegStartDate.AutoSize = true;
            this.lRegStartDate.Location = new System.Drawing.Point(30, 65);
            this.lRegStartDate.Name = "lRegStartDate";
            this.lRegStartDate.Size = new System.Drawing.Size(112, 13);
            this.lRegStartDate.TabIndex = 4;
            this.lRegStartDate.Text = "Дата начала потока:";
            // 
            // lRegEndDate
            // 
            this.lRegEndDate.AutoSize = true;
            this.lRegEndDate.Location = new System.Drawing.Point(12, 91);
            this.lRegEndDate.Name = "lRegEndDate";
            this.lRegEndDate.Size = new System.Drawing.Size(130, 13);
            this.lRegEndDate.TabIndex = 6;
            this.lRegEndDate.Text = "Дата окончания потока:";
            // 
            // dtpRegStartDate
            // 
            this.dtpRegStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRegStartDate.Location = new System.Drawing.Point(148, 59);
            this.dtpRegStartDate.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.dtpRegStartDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpRegStartDate.Name = "dtpRegStartDate";
            this.dtpRegStartDate.Size = new System.Drawing.Size(90, 20);
            this.dtpRegStartDate.TabIndex = 5;
            // 
            // dtpRegEndDate
            // 
            this.dtpRegEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRegEndDate.Location = new System.Drawing.Point(148, 85);
            this.dtpRegEndDate.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.dtpRegEndDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpRegEndDate.Name = "dtpRegEndDate";
            this.dtpRegEndDate.Size = new System.Drawing.Size(90, 20);
            this.dtpRegEndDate.TabIndex = 7;
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.dataGridView);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox.Location = new System.Drawing.Point(321, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(138, 162);
            this.groupBox.TabIndex = 8;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Аудитории";
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(12, 127);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 9;
            this.bSave.Text = "Сохранить";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // ExaminationEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 162);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.dtpRegEndDate);
            this.Controls.Add(this.dtpRegStartDate);
            this.Controls.Add(this.lRegEndDate);
            this.Controls.Add(this.lRegStartDate);
            this.Controls.Add(this.lDate);
            this.Controls.Add(this.lSubject);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.cbSubject);
            this.Controls.Add(this.groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ExaminationEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Экзамен";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ComboBox cbSubject;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lSubject;
        private System.Windows.Forms.Label lDate;
        private System.Windows.Forms.Label lRegStartDate;
        private System.Windows.Forms.Label lRegEndDate;
        private System.Windows.Forms.DateTimePicker dtpRegStartDate;
        private System.Windows.Forms.DateTimePicker dtpRegEndDate;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Capacity;
    }
}