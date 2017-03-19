namespace PK
{
    partial class QuotDocsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbCause = new System.Windows.Forms.ComboBox();
            this.pOrphanhood = new System.Windows.Forms.Panel();
            this.tbOrphanhoodDocOrg = new System.Windows.Forms.TextBox();
            this.tbOrphanhoodDocName = new System.Windows.Forms.TextBox();
            this.dtpOrphanhoodDocDate = new System.Windows.Forms.DateTimePicker();
            this.cbOrphanhoodDocType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pMed = new System.Windows.Forms.Panel();
            this.dtpConclusionDate = new System.Windows.Forms.DateTimePicker();
            this.cbDisabilityGroup = new System.Windows.Forms.ComboBox();
            this.tbConclusionNumber = new System.Windows.Forms.TextBox();
            this.tbMedDocNumber = new System.Windows.Forms.TextBox();
            this.tbMedDocSeries = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbMedCause = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btSave = new System.Windows.Forms.Button();
            this.pOrphanhood.SuspendLayout();
            this.pMed.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Основание для льготы:";
            // 
            // cbCause
            // 
            this.cbCause.FormattingEnabled = true;
            this.cbCause.Items.AddRange(new object[] {
            "Сиротство",
            "Медицинские показатели"});
            this.cbCause.Location = new System.Drawing.Point(191, 23);
            this.cbCause.Name = "cbCause";
            this.cbCause.Size = new System.Drawing.Size(174, 21);
            this.cbCause.TabIndex = 1;
            this.cbCause.SelectedIndexChanged += new System.EventHandler(this.cbCause_SelectedIndexChanged);
            // 
            // pOrphanhood
            // 
            this.pOrphanhood.Controls.Add(this.tbOrphanhoodDocOrg);
            this.pOrphanhood.Controls.Add(this.tbOrphanhoodDocName);
            this.pOrphanhood.Controls.Add(this.dtpOrphanhoodDocDate);
            this.pOrphanhood.Controls.Add(this.cbOrphanhoodDocType);
            this.pOrphanhood.Controls.Add(this.label5);
            this.pOrphanhood.Controls.Add(this.label4);
            this.pOrphanhood.Controls.Add(this.label3);
            this.pOrphanhood.Controls.Add(this.label2);
            this.pOrphanhood.Enabled = false;
            this.pOrphanhood.Location = new System.Drawing.Point(13, 57);
            this.pOrphanhood.Name = "pOrphanhood";
            this.pOrphanhood.Size = new System.Drawing.Size(380, 261);
            this.pOrphanhood.TabIndex = 2;
            this.pOrphanhood.Visible = false;
            // 
            // tbOrphanhoodDocOrg
            // 
            this.tbOrphanhoodDocOrg.Location = new System.Drawing.Point(18, 198);
            this.tbOrphanhoodDocOrg.Name = "tbOrphanhoodDocOrg";
            this.tbOrphanhoodDocOrg.Size = new System.Drawing.Size(347, 20);
            this.tbOrphanhoodDocOrg.TabIndex = 7;
            // 
            // tbOrphanhoodDocName
            // 
            this.tbOrphanhoodDocName.Location = new System.Drawing.Point(18, 104);
            this.tbOrphanhoodDocName.Name = "tbOrphanhoodDocName";
            this.tbOrphanhoodDocName.Size = new System.Drawing.Size(347, 20);
            this.tbOrphanhoodDocName.TabIndex = 6;
            // 
            // dtpOrphanhoodDocDate
            // 
            this.dtpOrphanhoodDocDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOrphanhoodDocDate.Location = new System.Drawing.Point(157, 139);
            this.dtpOrphanhoodDocDate.Name = "dtpOrphanhoodDocDate";
            this.dtpOrphanhoodDocDate.Size = new System.Drawing.Size(208, 20);
            this.dtpOrphanhoodDocDate.TabIndex = 5;
            // 
            // cbOrphanhoodDocType
            // 
            this.cbOrphanhoodDocType.FormattingEnabled = true;
            this.cbOrphanhoodDocType.Location = new System.Drawing.Point(15, 42);
            this.cbOrphanhoodDocType.Name = "cbOrphanhoodDocType";
            this.cbOrphanhoodDocType.Size = new System.Drawing.Size(350, 21);
            this.cbOrphanhoodDocType.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(186, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Организация, выдавшая документ:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Дата выдачи документа:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Наименование документа:\r\n";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Тип документа:";
            // 
            // pMed
            // 
            this.pMed.Controls.Add(this.dtpConclusionDate);
            this.pMed.Controls.Add(this.cbDisabilityGroup);
            this.pMed.Controls.Add(this.tbConclusionNumber);
            this.pMed.Controls.Add(this.tbMedDocNumber);
            this.pMed.Controls.Add(this.tbMedDocSeries);
            this.pMed.Controls.Add(this.label12);
            this.pMed.Controls.Add(this.label11);
            this.pMed.Controls.Add(this.label10);
            this.pMed.Controls.Add(this.label9);
            this.pMed.Controls.Add(this.label8);
            this.pMed.Controls.Add(this.label7);
            this.pMed.Controls.Add(this.cbMedCause);
            this.pMed.Controls.Add(this.label6);
            this.pMed.Enabled = false;
            this.pMed.Location = new System.Drawing.Point(13, 57);
            this.pMed.Name = "pMed";
            this.pMed.Size = new System.Drawing.Size(380, 261);
            this.pMed.TabIndex = 3;
            this.pMed.Visible = false;
            // 
            // dtpConclusionDate
            // 
            this.dtpConclusionDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpConclusionDate.Location = new System.Drawing.Point(105, 235);
            this.dtpConclusionDate.Name = "dtpConclusionDate";
            this.dtpConclusionDate.Size = new System.Drawing.Size(171, 20);
            this.dtpConclusionDate.TabIndex = 12;
            // 
            // cbDisabilityGroup
            // 
            this.cbDisabilityGroup.Enabled = false;
            this.cbDisabilityGroup.FormattingEnabled = true;
            this.cbDisabilityGroup.Location = new System.Drawing.Point(140, 131);
            this.cbDisabilityGroup.Name = "cbDisabilityGroup";
            this.cbDisabilityGroup.Size = new System.Drawing.Size(222, 21);
            this.cbDisabilityGroup.TabIndex = 11;
            // 
            // tbConclusionNumber
            // 
            this.tbConclusionNumber.Location = new System.Drawing.Point(129, 204);
            this.tbConclusionNumber.Name = "tbConclusionNumber";
            this.tbConclusionNumber.Size = new System.Drawing.Size(100, 20);
            this.tbConclusionNumber.TabIndex = 10;
            // 
            // tbMedDocNumber
            // 
            this.tbMedDocNumber.Location = new System.Drawing.Point(123, 100);
            this.tbMedDocNumber.Name = "tbMedDocNumber";
            this.tbMedDocNumber.Size = new System.Drawing.Size(100, 20);
            this.tbMedDocNumber.TabIndex = 9;
            // 
            // tbMedDocSeries
            // 
            this.tbMedDocSeries.Enabled = false;
            this.tbMedDocSeries.Location = new System.Drawing.Point(119, 63);
            this.tbMedDocSeries.Name = "tbMedDocSeries";
            this.tbMedDocSeries.Size = new System.Drawing.Size(100, 20);
            this.tbMedDocSeries.TabIndex = 8;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(41, 172);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(311, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Заключение об отсутствии противопоказаний для обучения";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Enabled = false;
            this.label11.Location = new System.Drawing.Point(15, 134);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(119, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "Группа инвалидности:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 204);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Номер документа:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 235);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Дата выдачи:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Номер документа:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(15, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Серия документа:";
            // 
            // cbMedCause
            // 
            this.cbMedCause.FormattingEnabled = true;
            this.cbMedCause.Items.AddRange(new object[] {
            "Справква об установлении инвалидности",
            "Заключение психолого-медико-педагогической комиссии"});
            this.cbMedCause.Location = new System.Drawing.Point(18, 36);
            this.cbMedCause.Name = "cbMedCause";
            this.cbMedCause.Size = new System.Drawing.Size(344, 21);
            this.cbMedCause.TabIndex = 1;
            this.cbMedCause.SelectedIndexChanged += new System.EventHandler(this.cbMedCause_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Основание для льготы:";
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(170, 329);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 4;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // QuotDocsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 364);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.cbCause);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pOrphanhood);
            this.Controls.Add(this.pMed);
            this.Name = "QuotDocsForm";
            this.Text = "Особая квота";
            this.pOrphanhood.ResumeLayout(false);
            this.pOrphanhood.PerformLayout();
            this.pMed.ResumeLayout(false);
            this.pMed.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCause;
        private System.Windows.Forms.Panel pOrphanhood;
        private System.Windows.Forms.TextBox tbOrphanhoodDocOrg;
        private System.Windows.Forms.TextBox tbOrphanhoodDocName;
        private System.Windows.Forms.DateTimePicker dtpOrphanhoodDocDate;
        private System.Windows.Forms.ComboBox cbOrphanhoodDocType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pMed;
        private System.Windows.Forms.DateTimePicker dtpConclusionDate;
        private System.Windows.Forms.ComboBox cbDisabilityGroup;
        private System.Windows.Forms.TextBox tbConclusionNumber;
        private System.Windows.Forms.TextBox tbMedDocNumber;
        private System.Windows.Forms.TextBox tbMedDocSeries;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbMedCause;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btSave;
    }
}