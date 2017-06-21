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
            this.bSave = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tbXSD_Path = new System.Windows.Forms.TextBox();
            this.bOpenXSD = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.dtpAppStartDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpOrdStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpAppEndDate = new System.Windows.Forms.DateTimePicker();
            this.dtpOrdEndDate = new System.Windows.Forms.DateTimePicker();
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
            this.bExport.Location = new System.Drawing.Point(233, 134);
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
            this.cbApplications.Size = new System.Drawing.Size(90, 17);
            this.cbApplications.TabIndex = 6;
            this.cbApplications.Text = "Заявления с";
            this.cbApplications.UseVisualStyleBackColor = true;
            // 
            // cbOrders
            // 
            this.cbOrders.AutoSize = true;
            this.cbOrders.Location = new System.Drawing.Point(12, 111);
            this.cbOrders.Name = "cbOrders";
            this.cbOrders.Size = new System.Drawing.Size(81, 17);
            this.cbOrders.TabIndex = 7;
            this.cbOrders.Text = "Приказы с";
            this.cbOrders.UseVisualStyleBackColor = true;
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(108, 134);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(116, 23);
            this.bSave.TabIndex = 8;
            this.bSave.Text = "Выгрузить в файл...";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.Filter = "XML-файлы|*.xml|Все файлы|*.*";
            // 
            // tbXSD_Path
            // 
            this.tbXSD_Path.Location = new System.Drawing.Point(12, 163);
            this.tbXSD_Path.Name = "tbXSD_Path";
            this.tbXSD_Path.ReadOnly = true;
            this.tbXSD_Path.Size = new System.Drawing.Size(197, 20);
            this.tbXSD_Path.TabIndex = 9;
            // 
            // bOpenXSD
            // 
            this.bOpenXSD.Location = new System.Drawing.Point(215, 161);
            this.bOpenXSD.Name = "bOpenXSD";
            this.bOpenXSD.Size = new System.Drawing.Size(93, 23);
            this.bOpenXSD.TabIndex = 10;
            this.bOpenXSD.Text = "Выбрать XSD...";
            this.bOpenXSD.UseVisualStyleBackColor = true;
            this.bOpenXSD.Click += new System.EventHandler(this.bOpenXSD_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xsd";
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.Filter = "XSD-файлы|*.xsd|Все файлы|*.*";
            // 
            // dtpAppStartDate
            // 
            this.dtpAppStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAppStartDate.Location = new System.Drawing.Point(108, 85);
            this.dtpAppStartDate.MaxDate = new System.DateTime(2030, 12, 31, 0, 0, 0, 0);
            this.dtpAppStartDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpAppStartDate.Name = "dtpAppStartDate";
            this.dtpAppStartDate.Size = new System.Drawing.Size(70, 20);
            this.dtpAppStartDate.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(184, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "по";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(184, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "по";
            // 
            // dtpOrdStartDate
            // 
            this.dtpOrdStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOrdStartDate.Location = new System.Drawing.Point(108, 108);
            this.dtpOrdStartDate.MaxDate = new System.DateTime(2030, 12, 31, 0, 0, 0, 0);
            this.dtpOrdStartDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpOrdStartDate.Name = "dtpOrdStartDate";
            this.dtpOrdStartDate.Size = new System.Drawing.Size(70, 20);
            this.dtpOrdStartDate.TabIndex = 17;
            // 
            // dtpAppEndDate
            // 
            this.dtpAppEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAppEndDate.Location = new System.Drawing.Point(209, 85);
            this.dtpAppEndDate.MaxDate = new System.DateTime(2030, 12, 31, 0, 0, 0, 0);
            this.dtpAppEndDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpAppEndDate.Name = "dtpAppEndDate";
            this.dtpAppEndDate.Size = new System.Drawing.Size(70, 20);
            this.dtpAppEndDate.TabIndex = 18;
            // 
            // dtpOrdEndDate
            // 
            this.dtpOrdEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOrdEndDate.Location = new System.Drawing.Point(209, 108);
            this.dtpOrdEndDate.MaxDate = new System.DateTime(2030, 12, 31, 0, 0, 0, 0);
            this.dtpOrdEndDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpOrdEndDate.Name = "dtpOrdEndDate";
            this.dtpOrdEndDate.Size = new System.Drawing.Size(70, 20);
            this.dtpOrdEndDate.TabIndex = 19;
            // 
            // FIS_Export
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 194);
            this.Controls.Add(this.dtpOrdEndDate);
            this.Controls.Add(this.dtpAppEndDate);
            this.Controls.Add(this.dtpOrdStartDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpAppStartDate);
            this.Controls.Add(this.bOpenXSD);
            this.Controls.Add(this.tbXSD_Path);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.cbOrders);
            this.Controls.Add(this.cbApplications);
            this.Controls.Add(this.cbCampaignData);
            this.Controls.Add(this.lExportData);
            this.Controls.Add(this.bOpenAddressPage);
            this.Controls.Add(this.lAddress);
            this.Controls.Add(this.bExport);
            this.Controls.Add(this.cbAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
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
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TextBox tbXSD_Path;
        private System.Windows.Forms.Button bOpenXSD;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.DateTimePicker dtpAppStartDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpOrdStartDate;
        private System.Windows.Forms.DateTimePicker dtpAppEndDate;
        private System.Windows.Forms.DateTimePicker dtpOrdEndDate;
    }
}