namespace PK.Forms
{
    partial class HR_DepartmentPrint
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
            this.rbDate = new System.Windows.Forms.RadioButton();
            this.rbNumber = new System.Windows.Forms.RadioButton();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.tbNumber = new System.Windows.Forms.TextBox();
            this.cbActs = new System.Windows.Forms.CheckBox();
            this.cbReceipts = new System.Windows.Forms.CheckBox();
            this.cbExamSheets = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bPrint = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbDate
            // 
            this.rbDate.AutoSize = true;
            this.rbDate.Location = new System.Drawing.Point(12, 12);
            this.rbDate.Name = "rbDate";
            this.rbDate.Size = new System.Drawing.Size(79, 17);
            this.rbDate.TabIndex = 0;
            this.rbDate.TabStop = true;
            this.rbDate.Text = "Диапазон:";
            this.rbDate.UseVisualStyleBackColor = true;
            this.rbDate.CheckedChanged += new System.EventHandler(this.rbDate_CheckedChanged);
            // 
            // rbNumber
            // 
            this.rbNumber.AutoSize = true;
            this.rbNumber.Location = new System.Drawing.Point(12, 35);
            this.rbNumber.Name = "rbNumber";
            this.rbNumber.Size = new System.Drawing.Size(107, 17);
            this.rbNumber.TabIndex = 1;
            this.rbNumber.TabStop = true;
            this.rbNumber.Text = "Номер приказа:";
            this.rbNumber.UseVisualStyleBackColor = true;
            // 
            // dtpStart
            // 
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStart.Location = new System.Drawing.Point(125, 12);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(80, 20);
            this.dtpStart.TabIndex = 2;
            // 
            // dtpEnd
            // 
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEnd.Location = new System.Drawing.Point(227, 12);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(80, 20);
            this.dtpEnd.TabIndex = 3;
            // 
            // tbNumber
            // 
            this.tbNumber.Location = new System.Drawing.Point(125, 34);
            this.tbNumber.Name = "tbNumber";
            this.tbNumber.Size = new System.Drawing.Size(80, 20);
            this.tbNumber.TabIndex = 4;
            // 
            // cbActs
            // 
            this.cbActs.AutoSize = true;
            this.cbActs.Location = new System.Drawing.Point(12, 58);
            this.cbActs.Name = "cbActs";
            this.cbActs.Size = new System.Drawing.Size(123, 17);
            this.cbActs.TabIndex = 5;
            this.cbActs.Text = "Акты передачи дел";
            this.cbActs.UseVisualStyleBackColor = true;
            // 
            // cbReceipts
            // 
            this.cbReceipts.AutoSize = true;
            this.cbReceipts.Location = new System.Drawing.Point(12, 81);
            this.cbReceipts.Name = "cbReceipts";
            this.cbReceipts.Size = new System.Drawing.Size(142, 17);
            this.cbReceipts.TabIndex = 6;
            this.cbReceipts.Text = "Выписки о зачислении";
            this.cbReceipts.UseVisualStyleBackColor = true;
            // 
            // cbExamSheets
            // 
            this.cbExamSheets.AutoSize = true;
            this.cbExamSheets.Location = new System.Drawing.Point(12, 104);
            this.cbExamSheets.Name = "cbExamSheets";
            this.cbExamSheets.Size = new System.Drawing.Size(155, 17);
            this.cbExamSheets.TabIndex = 7;
            this.cbExamSheets.Text = "Экзаменационные листы";
            this.cbExamSheets.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(211, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "-";
            // 
            // bPrint
            // 
            this.bPrint.Location = new System.Drawing.Point(232, 100);
            this.bPrint.Name = "bPrint";
            this.bPrint.Size = new System.Drawing.Size(75, 23);
            this.bPrint.TabIndex = 9;
            this.bPrint.Text = "Печать";
            this.bPrint.UseVisualStyleBackColor = true;
            this.bPrint.Click += new System.EventHandler(this.bPrint_Click);
            // 
            // HR_DepartmentPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 131);
            this.Controls.Add(this.bPrint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbExamSheets);
            this.Controls.Add(this.cbReceipts);
            this.Controls.Add(this.cbActs);
            this.Controls.Add(this.tbNumber);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.dtpStart);
            this.Controls.Add(this.rbNumber);
            this.Controls.Add(this.rbDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.Name = "HR_DepartmentPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Печатные формы отдела кадров";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbDate;
        private System.Windows.Forms.RadioButton rbNumber;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.TextBox tbNumber;
        private System.Windows.Forms.CheckBox cbActs;
        private System.Windows.Forms.CheckBox cbReceipts;
        private System.Windows.Forms.CheckBox cbExamSheets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bPrint;
    }
}