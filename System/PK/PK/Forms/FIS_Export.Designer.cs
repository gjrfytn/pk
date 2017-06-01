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
            this.cbAddress = new System.Windows.Forms.ComboBox();
            this.bExport = new System.Windows.Forms.Button();
            this.lAddress = new System.Windows.Forms.Label();
            this.bOpenAddressPage = new System.Windows.Forms.Button();
            this.lExportData = new System.Windows.Forms.Label();
            this.cbCampaignData = new System.Windows.Forms.CheckBox();
            this.cbApplications = new System.Windows.Forms.CheckBox();
            this.cbOrders = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbAddress
            // 
            this.cbAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAddress.FormattingEnabled = true;
            this.cbAddress.Location = new System.Drawing.Point(12, 25);
            this.cbAddress.Name = "cbAddress";
            this.cbAddress.Size = new System.Drawing.Size(150, 21);
            this.cbAddress.TabIndex = 0;
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
            // lAddress
            // 
            this.lAddress.AutoSize = true;
            this.lAddress.Location = new System.Drawing.Point(12, 9);
            this.lAddress.Name = "lAddress";
            this.lAddress.Size = new System.Drawing.Size(70, 13);
            this.lAddress.TabIndex = 2;
            this.lAddress.Text = "Адрес ФИС:";
            // 
            // bOpenAddressPage
            // 
            this.bOpenAddressPage.Location = new System.Drawing.Point(168, 23);
            this.bOpenAddressPage.Name = "bOpenAddressPage";
            this.bOpenAddressPage.Size = new System.Drawing.Size(140, 23);
            this.bOpenAddressPage.TabIndex = 3;
            this.bOpenAddressPage.Text = "Открыть web-интерфейс";
            this.bOpenAddressPage.UseVisualStyleBackColor = true;
            this.bOpenAddressPage.Click += new System.EventHandler(this.bOpenAddressPage_Click);
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
            this.Controls.Add(this.bOpenAddressPage);
            this.Controls.Add(this.lAddress);
            this.Controls.Add(this.bExport);
            this.Controls.Add(this.cbAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FIS_Export";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Экспорт в ФИС";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbAddress;
        private System.Windows.Forms.Button bExport;
        private System.Windows.Forms.Label lAddress;
        private System.Windows.Forms.Button bOpenAddressPage;
        private System.Windows.Forms.Label lExportData;
        private System.Windows.Forms.CheckBox cbCampaignData;
        private System.Windows.Forms.CheckBox cbApplications;
        private System.Windows.Forms.CheckBox cbOrders;
    }
}