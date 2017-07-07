namespace PK.Forms
{
    partial class ExaminationDocsPrint
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
            this.bAlphaCodes = new System.Windows.Forms.Button();
            this.bAlphaAuditories = new System.Windows.Forms.Button();
            this.bAbitAudDistib = new System.Windows.Forms.Button();
            this.bExamCardsSheet = new System.Windows.Forms.Button();
            this.bExamFill = new System.Windows.Forms.Button();
            this.bAudLists = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bAlphaCodes
            // 
            this.bAlphaCodes.Location = new System.Drawing.Point(12, 12);
            this.bAlphaCodes.Name = "bAlphaCodes";
            this.bAlphaCodes.Size = new System.Drawing.Size(118, 34);
            this.bAlphaCodes.TabIndex = 0;
            this.bAlphaCodes.Text = "Алфавитный список с кодами работ";
            this.bAlphaCodes.UseVisualStyleBackColor = true;
            this.bAlphaCodes.Click += new System.EventHandler(this.bAlphaCodes_Click);
            // 
            // bAlphaAuditories
            // 
            this.bAlphaAuditories.Location = new System.Drawing.Point(136, 12);
            this.bAlphaAuditories.Name = "bAlphaAuditories";
            this.bAlphaAuditories.Size = new System.Drawing.Size(127, 34);
            this.bAlphaAuditories.TabIndex = 1;
            this.bAlphaAuditories.Text = "Алфавитный список с номерами аудиторий";
            this.bAlphaAuditories.UseVisualStyleBackColor = true;
            this.bAlphaAuditories.Click += new System.EventHandler(this.bAlphaAuditories_Click);
            // 
            // bAbitAudDistib
            // 
            this.bAbitAudDistib.Location = new System.Drawing.Point(38, 52);
            this.bAbitAudDistib.Name = "bAbitAudDistib";
            this.bAbitAudDistib.Size = new System.Drawing.Size(190, 34);
            this.bAbitAudDistib.TabIndex = 2;
            this.bAbitAudDistib.Text = "Распределение экзаменационных групп по аудиториям";
            this.bAbitAudDistib.UseVisualStyleBackColor = true;
            this.bAbitAudDistib.Click += new System.EventHandler(this.bAbitAudDistib_Click);
            // 
            // bExamCardsSheet
            // 
            this.bExamCardsSheet.Location = new System.Drawing.Point(37, 92);
            this.bExamCardsSheet.Name = "bExamCardsSheet";
            this.bExamCardsSheet.Size = new System.Drawing.Size(192, 34);
            this.bExamCardsSheet.TabIndex = 3;
            this.bExamCardsSheet.Text = "Ведомость проставления номеров экзаменационных билетов";
            this.bExamCardsSheet.UseVisualStyleBackColor = true;
            this.bExamCardsSheet.Click += new System.EventHandler(this.bExamCardsSheet_Click);
            // 
            // bExamFill
            // 
            this.bExamFill.Location = new System.Drawing.Point(119, 132);
            this.bExamFill.Name = "bExamFill";
            this.bExamFill.Size = new System.Drawing.Size(118, 34);
            this.bExamFill.TabIndex = 4;
            this.bExamFill.Text = "Посещаемость по буквам";
            this.bExamFill.UseVisualStyleBackColor = true;
            this.bExamFill.Click += new System.EventHandler(this.bExamFill_Click);
            // 
            // bAudLists
            // 
            this.bAudLists.Location = new System.Drawing.Point(38, 132);
            this.bAudLists.Name = "bAudLists";
            this.bAudLists.Size = new System.Drawing.Size(75, 34);
            this.bAudLists.TabIndex = 5;
            this.bAudLists.Text = "Списки по аудиториям";
            this.bAudLists.UseVisualStyleBackColor = true;
            this.bAudLists.Click += new System.EventHandler(this.bAudLists_Click);
            // 
            // ExaminationDocsPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 174);
            this.Controls.Add(this.bAudLists);
            this.Controls.Add(this.bExamFill);
            this.Controls.Add(this.bExamCardsSheet);
            this.Controls.Add(this.bAbitAudDistib);
            this.Controls.Add(this.bAlphaAuditories);
            this.Controls.Add(this.bAlphaCodes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExaminationDocsPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Печать";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bAlphaCodes;
        private System.Windows.Forms.Button bAlphaAuditories;
        private System.Windows.Forms.Button bAbitAudDistib;
        private System.Windows.Forms.Button bExamCardsSheet;
        private System.Windows.Forms.Button bExamFill;
        private System.Windows.Forms.Button bAudLists;
    }
}