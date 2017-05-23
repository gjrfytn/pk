namespace PK.Forms
{
    partial class FIS_Export
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
            this.cbAdress = new System.Windows.Forms.ComboBox();
            this.bExport = new System.Windows.Forms.Button();
            this.lAdress = new System.Windows.Forms.Label();
            this.bOpenAdressPage = new System.Windows.Forms.Button();
            this.lExportData = new System.Windows.Forms.Label();
            this.cbCampaignData = new System.Windows.Forms.CheckBox();
            this.cbApplications = new System.Windows.Forms.CheckBox();
            this.cbOrders = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbAdress
            // 
            this.cbAdress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdress.FormattingEnabled = true;
            this.cbAdress.Location = new System.Drawing.Point(12, 25);
            this.cbAdress.Name = "cbAdress";
            this.cbAdress.Size = new System.Drawing.Size(150, 21);
            this.cbAdress.TabIndex = 0;
            // 
            // bExport
            // 
            this.bExport.Location = new System.Drawing.Point(199, 273);
            this.bExport.Name = "bExport";
            this.bExport.Size = new System.Drawing.Size(75, 23);
            this.bExport.TabIndex = 1;
            this.bExport.Text = "Экспорт";
            this.bExport.UseVisualStyleBackColor = true;
            this.bExport.Click += new System.EventHandler(this.bExport_Click);
            // 
            // lAdress
            // 
            this.lAdress.AutoSize = true;
            this.lAdress.Location = new System.Drawing.Point(12, 9);
            this.lAdress.Name = "lAdress";
            this.lAdress.Size = new System.Drawing.Size(70, 13);
            this.lAdress.TabIndex = 2;
            this.lAdress.Text = "Адрес ФИС:";
            // 
            // bOpenAdressPage
            // 
            this.bOpenAdressPage.Location = new System.Drawing.Point(168, 23);
            this.bOpenAdressPage.Name = "bOpenAdressPage";
            this.bOpenAdressPage.Size = new System.Drawing.Size(140, 23);
            this.bOpenAdressPage.TabIndex = 3;
            this.bOpenAdressPage.Text = "Открыть web-интерфейс";
            this.bOpenAdressPage.UseVisualStyleBackColor = true;
            this.bOpenAdressPage.Click += new System.EventHandler(this.bOpenAdressPage_Click);
            // 
            // lExportData
            // 
            this.lExportData.AutoSize = true;
            this.lExportData.Location = new System.Drawing.Point(12, 49);
            this.lExportData.Name = "lExportData";
            this.lExportData.Size = new System.Drawing.Size(66, 13);
            this.lExportData.TabIndex = 4;
            this.lExportData.Text = "Выгружать:";
            // 
            // cbCampaignData
            // 
            this.cbCampaignData.AutoSize = true;
            this.cbCampaignData.Location = new System.Drawing.Point(12, 65);
            this.cbCampaignData.Name = "cbCampaignData";
            this.cbCampaignData.Size = new System.Drawing.Size(173, 17);
            this.cbCampaignData.TabIndex = 5;
            this.cbCampaignData.Text = "Данные приёмной кампании";
            this.cbCampaignData.UseVisualStyleBackColor = true;
            // 
            // cbApplications
            // 
            this.cbApplications.AutoSize = true;
            this.cbApplications.Location = new System.Drawing.Point(12, 88);
            this.cbApplications.Name = "cbApplications";
            this.cbApplications.Size = new System.Drawing.Size(81, 17);
            this.cbApplications.TabIndex = 6;
            this.cbApplications.Text = "Заявления";
            this.cbApplications.UseVisualStyleBackColor = true;
            // 
            // cbOrders
            // 
            this.cbOrders.AutoSize = true;
            this.cbOrders.Location = new System.Drawing.Point(12, 111);
            this.cbOrders.Name = "cbOrders";
            this.cbOrders.Size = new System.Drawing.Size(72, 17);
            this.cbOrders.TabIndex = 7;
            this.cbOrders.Text = "Приказы";
            this.cbOrders.UseVisualStyleBackColor = true;
            // 
            // FIS_Export
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 348);
            this.Controls.Add(this.cbOrders);
            this.Controls.Add(this.cbApplications);
            this.Controls.Add(this.cbCampaignData);
            this.Controls.Add(this.lExportData);
            this.Controls.Add(this.bOpenAdressPage);
            this.Controls.Add(this.lAdress);
            this.Controls.Add(this.bExport);
            this.Controls.Add(this.cbAdress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FIS_Export";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Экспорт в ФИС";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbAdress;
        private System.Windows.Forms.Button bExport;
        private System.Windows.Forms.Label lAdress;
        private System.Windows.Forms.Button bOpenAdressPage;
        private System.Windows.Forms.Label lExportData;
        private System.Windows.Forms.CheckBox cbCampaignData;
        private System.Windows.Forms.CheckBox cbApplications;
        private System.Windows.Forms.CheckBox cbOrders;
    }
}