﻿namespace PK.Forms
{
    partial class ApplicationDocsPrint
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
            this.cbMoveJournal = new System.Windows.Forms.CheckBox();
            this.cbInventory = new System.Windows.Forms.CheckBox();
            this.cbPercRecordFace = new System.Windows.Forms.CheckBox();
            this.cbPercRecordBack = new System.Windows.Forms.CheckBox();
            this.cbReceipt = new System.Windows.Forms.CheckBox();
            this.bOpen = new System.Windows.Forms.Button();
            this.bPrint = new System.Windows.Forms.Button();
            this.cbAdmAgreement = new System.Windows.Forms.CheckBox();
            this.cbApplication = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbMoveJournal
            // 
            this.cbMoveJournal.AutoSize = true;
            this.cbMoveJournal.Checked = true;
            this.cbMoveJournal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMoveJournal.Location = new System.Drawing.Point(16, 146);
            this.cbMoveJournal.Margin = new System.Windows.Forms.Padding(4);
            this.cbMoveJournal.Name = "cbMoveJournal";
            this.cbMoveJournal.Size = new System.Drawing.Size(230, 21);
            this.cbMoveJournal.TabIndex = 0;
            this.cbMoveJournal.Text = "Журнал движения личных дел";
            this.cbMoveJournal.UseVisualStyleBackColor = true;
            // 
            // cbInventory
            // 
            this.cbInventory.AutoSize = true;
            this.cbInventory.Checked = true;
            this.cbInventory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbInventory.Location = new System.Drawing.Point(16, 62);
            this.cbInventory.Margin = new System.Windows.Forms.Padding(4);
            this.cbInventory.Name = "cbInventory";
            this.cbInventory.Size = new System.Drawing.Size(71, 21);
            this.cbInventory.TabIndex = 1;
            this.cbInventory.Text = "Опись";
            this.cbInventory.UseVisualStyleBackColor = true;
            // 
            // cbPercRecordFace
            // 
            this.cbPercRecordFace.AutoSize = true;
            this.cbPercRecordFace.Checked = true;
            this.cbPercRecordFace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPercRecordFace.Location = new System.Drawing.Point(16, 90);
            this.cbPercRecordFace.Margin = new System.Windows.Forms.Padding(4);
            this.cbPercRecordFace.Name = "cbPercRecordFace";
            this.cbPercRecordFace.Size = new System.Drawing.Size(181, 21);
            this.cbPercRecordFace.TabIndex = 2;
            this.cbPercRecordFace.Text = "Лицевая сторона дела";
            this.cbPercRecordFace.UseVisualStyleBackColor = true;
            // 
            // cbPercRecordBack
            // 
            this.cbPercRecordBack.AutoSize = true;
            this.cbPercRecordBack.Checked = true;
            this.cbPercRecordBack.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPercRecordBack.Location = new System.Drawing.Point(16, 118);
            this.cbPercRecordBack.Margin = new System.Windows.Forms.Padding(4);
            this.cbPercRecordBack.Name = "cbPercRecordBack";
            this.cbPercRecordBack.Size = new System.Drawing.Size(198, 21);
            this.cbPercRecordBack.TabIndex = 3;
            this.cbPercRecordBack.Text = "Оборотная сторона дела";
            this.cbPercRecordBack.UseVisualStyleBackColor = true;
            // 
            // cbReceipt
            // 
            this.cbReceipt.AutoSize = true;
            this.cbReceipt.Checked = true;
            this.cbReceipt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbReceipt.Location = new System.Drawing.Point(16, 34);
            this.cbReceipt.Margin = new System.Windows.Forms.Padding(4);
            this.cbReceipt.Name = "cbReceipt";
            this.cbReceipt.Size = new System.Drawing.Size(92, 21);
            this.cbReceipt.TabIndex = 4;
            this.cbReceipt.Text = "Расписка";
            this.cbReceipt.UseVisualStyleBackColor = true;
            // 
            // bOpen
            // 
            this.bOpen.Location = new System.Drawing.Point(204, 203);
            this.bOpen.Margin = new System.Windows.Forms.Padding(4);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(81, 28);
            this.bOpen.TabIndex = 7;
            this.bOpen.Text = "Открыть";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // bPrint
            // 
            this.bPrint.Location = new System.Drawing.Point(16, 203);
            this.bPrint.Margin = new System.Windows.Forms.Padding(4);
            this.bPrint.Name = "bPrint";
            this.bPrint.Size = new System.Drawing.Size(105, 28);
            this.bPrint.TabIndex = 6;
            this.bPrint.Text = "Распечатать";
            this.bPrint.UseVisualStyleBackColor = true;
            this.bPrint.Click += new System.EventHandler(this.bPrint_Click);
            // 
            // cbAdmAgreement
            // 
            this.cbAdmAgreement.AutoSize = true;
            this.cbAdmAgreement.Enabled = false;
            this.cbAdmAgreement.Location = new System.Drawing.Point(16, 174);
            this.cbAdmAgreement.Margin = new System.Windows.Forms.Padding(4);
            this.cbAdmAgreement.Name = "cbAdmAgreement";
            this.cbAdmAgreement.Size = new System.Drawing.Size(279, 21);
            this.cbAdmAgreement.TabIndex = 5;
            this.cbAdmAgreement.Text = "Заявление о согласии на зачисление";
            this.cbAdmAgreement.UseVisualStyleBackColor = true;
            // 
            // cbApplication
            // 
            this.cbApplication.AutoSize = true;
            this.cbApplication.Checked = true;
            this.cbApplication.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbApplication.Location = new System.Drawing.Point(16, 6);
            this.cbApplication.Margin = new System.Windows.Forms.Padding(4);
            this.cbApplication.Name = "cbApplication";
            this.cbApplication.Size = new System.Drawing.Size(106, 21);
            this.cbApplication.TabIndex = 0;
            this.cbApplication.Text = "Заявление ";
            this.cbApplication.UseVisualStyleBackColor = true;
            // 
            // ApplicationDocsPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 242);
            this.Controls.Add(this.cbAdmAgreement);
            this.Controls.Add(this.bPrint);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.cbReceipt);
            this.Controls.Add(this.cbPercRecordBack);
            this.Controls.Add(this.cbPercRecordFace);
            this.Controls.Add(this.cbInventory);
            this.Controls.Add(this.cbApplication);
            this.Controls.Add(this.cbMoveJournal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplicationDocsPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Печать";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.Button bPrint;
        private System.Windows.Forms.CheckBox cbMoveJournal;
        private System.Windows.Forms.CheckBox cbInventory;
        private System.Windows.Forms.CheckBox cbPercRecordFace;
        private System.Windows.Forms.CheckBox cbPercRecordBack;
        private System.Windows.Forms.CheckBox cbReceipt;
        private System.Windows.Forms.CheckBox cbAdmAgreement;
        private System.Windows.Forms.CheckBox cbApplication;
    }
}