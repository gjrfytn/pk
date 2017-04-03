namespace PK.Forms
{
    partial class MADIOlimps
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
            this.cbOlympType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbOlympName = new System.Windows.Forms.TextBox();
            this.tbDocNumber = new System.Windows.Forms.TextBox();
            this.cbDiplomaType = new System.Windows.Forms.ComboBox();
            this.cbOlympID = new System.Windows.Forms.ComboBox();
            this.cbOlympProfile = new System.Windows.Forms.ComboBox();
            this.cbClass = new System.Windows.Forms.ComboBox();
            this.cbDiscipline = new System.Windows.Forms.ComboBox();
            this.cbContry = new System.Windows.Forms.ComboBox();
            this.btSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbOlympType
            // 
            this.cbOlympType.FormattingEnabled = true;
            this.cbOlympType.Items.AddRange(new object[] {
            "Диплом победителя/призера олимпиады школьников",
            "Диплом победителя/призера всероссийской олимпиады школьников",
            "Диплом 4 этапа всеукраинской олимпиады",
            "Диплом международной олимпиады"});
            this.cbOlympType.Location = new System.Drawing.Point(124, 30);
            this.cbOlympType.Name = "cbOlympType";
            this.cbOlympType.Size = new System.Drawing.Size(284, 21);
            this.cbOlympType.TabIndex = 0;
            this.cbOlympType.SelectedIndexChanged += new System.EventHandler(this.cbOlympType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Вид олимпиады:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(28, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Наименование олимпиады:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(28, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Номер документа:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(244, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Тип диплома:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(28, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(151, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Идентификатор олимпиады:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 176);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Профиль олимпиады:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(28, 211);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Класс обучения:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(195, 211);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Дисциплина:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(28, 246);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Страна:";
            // 
            // tbOlympName
            // 
            this.tbOlympName.Enabled = false;
            this.tbOlympName.Location = new System.Drawing.Point(181, 65);
            this.tbOlympName.Name = "tbOlympName";
            this.tbOlympName.Size = new System.Drawing.Size(227, 20);
            this.tbOlympName.TabIndex = 10;
            // 
            // tbDocNumber
            // 
            this.tbDocNumber.Enabled = false;
            this.tbDocNumber.Location = new System.Drawing.Point(135, 101);
            this.tbDocNumber.Name = "tbDocNumber";
            this.tbDocNumber.Size = new System.Drawing.Size(104, 20);
            this.tbDocNumber.TabIndex = 11;
            // 
            // cbDiplomaType
            // 
            this.cbDiplomaType.Enabled = false;
            this.cbDiplomaType.FormattingEnabled = true;
            this.cbDiplomaType.Location = new System.Drawing.Point(326, 101);
            this.cbDiplomaType.Name = "cbDiplomaType";
            this.cbDiplomaType.Size = new System.Drawing.Size(82, 21);
            this.cbDiplomaType.TabIndex = 12;
            // 
            // cbOlympID
            // 
            this.cbOlympID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbOlympID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOlympID.Enabled = false;
            this.cbOlympID.FormattingEnabled = true;
            this.cbOlympID.Location = new System.Drawing.Point(185, 138);
            this.cbOlympID.Name = "cbOlympID";
            this.cbOlympID.Size = new System.Drawing.Size(223, 21);
            this.cbOlympID.TabIndex = 13;
            this.cbOlympID.SelectedIndexChanged += new System.EventHandler(this.cbOlympID_SelectedIndexChanged);
            // 
            // cbOlympProfile
            // 
            this.cbOlympProfile.FormattingEnabled = true;
            this.cbOlympProfile.Location = new System.Drawing.Point(151, 173);
            this.cbOlympProfile.Name = "cbOlympProfile";
            this.cbOlympProfile.Size = new System.Drawing.Size(257, 21);
            this.cbOlympProfile.TabIndex = 14;
            // 
            // cbClass
            // 
            this.cbClass.Enabled = false;
            this.cbClass.FormattingEnabled = true;
            this.cbClass.Items.AddRange(new object[] {
            "7",
            "8",
            "9",
            "10",
            "11"});
            this.cbClass.Location = new System.Drawing.Point(124, 208);
            this.cbClass.Name = "cbClass";
            this.cbClass.Size = new System.Drawing.Size(55, 21);
            this.cbClass.TabIndex = 15;
            // 
            // cbDiscipline
            // 
            this.cbDiscipline.Enabled = false;
            this.cbDiscipline.FormattingEnabled = true;
            this.cbDiscipline.Location = new System.Drawing.Point(274, 208);
            this.cbDiscipline.Name = "cbDiscipline";
            this.cbDiscipline.Size = new System.Drawing.Size(135, 21);
            this.cbDiscipline.TabIndex = 16;
            // 
            // cbContry
            // 
            this.cbContry.Enabled = false;
            this.cbContry.FormattingEnabled = true;
            this.cbContry.Location = new System.Drawing.Point(80, 243);
            this.cbContry.Name = "cbContry";
            this.cbContry.Size = new System.Drawing.Size(247, 21);
            this.cbContry.TabIndex = 17;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(185, 283);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 18;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // MADIOlimps
            // 
            this.AcceptButton = this.btSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 318);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.cbContry);
            this.Controls.Add(this.cbDiscipline);
            this.Controls.Add(this.cbClass);
            this.Controls.Add(this.cbOlympProfile);
            this.Controls.Add(this.cbOlympID);
            this.Controls.Add(this.cbDiplomaType);
            this.Controls.Add(this.tbDocNumber);
            this.Controls.Add(this.tbOlympName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbOlympType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MADIOlimps";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Олимпиады и конференции МАДИ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbOlympType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbOlympName;
        private System.Windows.Forms.TextBox tbDocNumber;
        private System.Windows.Forms.ComboBox cbDiplomaType;
        private System.Windows.Forms.ComboBox cbOlympID;
        private System.Windows.Forms.ComboBox cbOlympProfile;
        private System.Windows.Forms.ComboBox cbClass;
        private System.Windows.Forms.ComboBox cbDiscipline;
        private System.Windows.Forms.ComboBox cbContry;
        private System.Windows.Forms.Button btSave;
    }
}