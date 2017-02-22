namespace PK
{
    partial class NewCampaignForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.gbEduForm = new System.Windows.Forms.GroupBox();
            this.rbZaochForm = new System.Windows.Forms.RadioButton();
            this.rbOchZForm = new System.Windows.Forms.RadioButton();
            this.rbOchForm = new System.Windows.Forms.RadioButton();
            this.gbEduLevel = new System.Windows.Forms.GroupBox();
            this.rbQual = new System.Windows.Forms.RadioButton();
            this.rabSPO = new System.Windows.Forms.RadioButton();
            this.rabSpec = new System.Windows.Forms.RadioButton();
            this.rbMag = new System.Windows.Forms.RadioButton();
            this.rbBac = new System.Windows.Forms.RadioButton();
            this.gbEduForm.SuspendLayout();
            this.gbEduLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Название:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(76, 32);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(211, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(293, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Год начала:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(433, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Год окончания:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(365, 32);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(62, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(520, 32);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(62, 21);
            this.comboBox2.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Тип приемной кампании:";
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(153, 69);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(238, 21);
            this.comboBox3.TabIndex = 7;
            // 
            // gbEduForm
            // 
            this.gbEduForm.Controls.Add(this.rbZaochForm);
            this.gbEduForm.Controls.Add(this.rbOchZForm);
            this.gbEduForm.Controls.Add(this.rbOchForm);
            this.gbEduForm.Location = new System.Drawing.Point(185, 104);
            this.gbEduForm.Name = "gbEduForm";
            this.gbEduForm.Size = new System.Drawing.Size(165, 100);
            this.gbEduForm.TabIndex = 8;
            this.gbEduForm.TabStop = false;
            this.gbEduForm.Text = "Форма обучения";
            // 
            // rbZaochForm
            // 
            this.rbZaochForm.AutoSize = true;
            this.rbZaochForm.Location = new System.Drawing.Point(7, 67);
            this.rbZaochForm.Name = "rbZaochForm";
            this.rbZaochForm.Size = new System.Drawing.Size(104, 17);
            this.rbZaochForm.TabIndex = 2;
            this.rbZaochForm.TabStop = true;
            this.rbZaochForm.Text = "Заочная форма";
            this.rbZaochForm.UseVisualStyleBackColor = true;
            // 
            // rbOchZForm
            // 
            this.rbOchZForm.AutoSize = true;
            this.rbOchZForm.Location = new System.Drawing.Point(7, 43);
            this.rbOchZForm.Name = "rbOchZForm";
            this.rbOchZForm.Size = new System.Drawing.Size(131, 17);
            this.rbOchZForm.TabIndex = 1;
            this.rbOchZForm.TabStop = true;
            this.rbOchZForm.Text = "Очно-заочная форма";
            this.rbOchZForm.UseVisualStyleBackColor = true;
            // 
            // rbOchForm
            // 
            this.rbOchForm.AutoSize = true;
            this.rbOchForm.Location = new System.Drawing.Point(6, 19);
            this.rbOchForm.Name = "rbOchForm";
            this.rbOchForm.Size = new System.Drawing.Size(93, 17);
            this.rbOchForm.TabIndex = 0;
            this.rbOchForm.TabStop = true;
            this.rbOchForm.Text = "Очная форма";
            this.rbOchForm.UseVisualStyleBackColor = true;
            // 
            // gbEduLevel
            // 
            this.gbEduLevel.Controls.Add(this.rbQual);
            this.gbEduLevel.Controls.Add(this.rabSPO);
            this.gbEduLevel.Controls.Add(this.rabSpec);
            this.gbEduLevel.Controls.Add(this.rbMag);
            this.gbEduLevel.Controls.Add(this.rbBac);
            this.gbEduLevel.Location = new System.Drawing.Point(397, 72);
            this.gbEduLevel.Name = "gbEduLevel";
            this.gbEduLevel.Size = new System.Drawing.Size(185, 144);
            this.gbEduLevel.TabIndex = 9;
            this.gbEduLevel.TabStop = false;
            this.gbEduLevel.Text = "Уровень образования";
            // 
            // rbQual
            // 
            this.rbQual.AutoSize = true;
            this.rbQual.Location = new System.Drawing.Point(7, 115);
            this.rbQual.Name = "rbQual";
            this.rbQual.Size = new System.Drawing.Size(178, 17);
            this.rbQual.TabIndex = 4;
            this.rbQual.TabStop = true;
            this.rbQual.Text = "Кадры высшей квалификации";
            this.rbQual.UseVisualStyleBackColor = true;
            // 
            // rabSPO
            // 
            this.rabSPO.AutoSize = true;
            this.rabSPO.Location = new System.Drawing.Point(7, 92);
            this.rabSPO.Name = "rabSPO";
            this.rabSPO.Size = new System.Drawing.Size(48, 17);
            this.rabSPO.TabIndex = 3;
            this.rabSPO.TabStop = true;
            this.rabSPO.Text = "СПО";
            this.rabSPO.UseVisualStyleBackColor = true;
            // 
            // rabSpec
            // 
            this.rabSpec.AutoSize = true;
            this.rabSpec.Location = new System.Drawing.Point(7, 68);
            this.rabSpec.Name = "rabSpec";
            this.rabSpec.Size = new System.Drawing.Size(90, 17);
            this.rabSpec.TabIndex = 2;
            this.rabSpec.TabStop = true;
            this.rabSpec.Text = "Специалитет";
            this.rabSpec.UseVisualStyleBackColor = true;
            // 
            // rbMag
            // 
            this.rbMag.AutoSize = true;
            this.rbMag.Location = new System.Drawing.Point(7, 44);
            this.rbMag.Name = "rbMag";
            this.rbMag.Size = new System.Drawing.Size(96, 17);
            this.rbMag.TabIndex = 1;
            this.rbMag.TabStop = true;
            this.rbMag.Text = "Магистратура";
            this.rbMag.UseVisualStyleBackColor = true;
            // 
            // rbBac
            // 
            this.rbBac.AutoSize = true;
            this.rbBac.Location = new System.Drawing.Point(7, 20);
            this.rbBac.Name = "rbBac";
            this.rbBac.Size = new System.Drawing.Size(91, 17);
            this.rbBac.TabIndex = 0;
            this.rbBac.TabStop = true;
            this.rbBac.Text = "Бакалавриат";
            this.rbBac.UseVisualStyleBackColor = true;
            // 
            // NewCampaignForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 453);
            this.Controls.Add(this.gbEduLevel);
            this.Controls.Add(this.gbEduForm);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "NewCampaignForm";
            this.Text = "Создание приемной кампании";
            this.gbEduForm.ResumeLayout(false);
            this.gbEduForm.PerformLayout();
            this.gbEduLevel.ResumeLayout(false);
            this.gbEduLevel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.GroupBox gbEduForm;
        private System.Windows.Forms.RadioButton rbZaochForm;
        private System.Windows.Forms.RadioButton rbOchZForm;
        private System.Windows.Forms.RadioButton rbOchForm;
        private System.Windows.Forms.GroupBox gbEduLevel;
        private System.Windows.Forms.RadioButton rbQual;
        private System.Windows.Forms.RadioButton rabSPO;
        private System.Windows.Forms.RadioButton rabSpec;
        private System.Windows.Forms.RadioButton rbMag;
        private System.Windows.Forms.RadioButton rbBac;
    }
}