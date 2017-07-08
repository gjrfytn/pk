namespace FIS_Exporter
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.bOpenAddressPage = new System.Windows.Forms.Button();
            this.lAddress = new System.Windows.Forms.Label();
            this.cbAddress = new System.Windows.Forms.ComboBox();
            this.bExport = new System.Windows.Forms.Button();
            this.bOpen = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textBox = new System.Windows.Forms.TextBox();
            this.nudDictionaryNumber = new System.Windows.Forms.NumericUpDown();
            this.bLoadDictionary = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.bDictionariesList = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudDictionaryNumber)).BeginInit();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOpenAddressPage
            // 
            this.bOpenAddressPage.Location = new System.Drawing.Point(168, 23);
            this.bOpenAddressPage.Name = "bOpenAddressPage";
            this.bOpenAddressPage.Size = new System.Drawing.Size(140, 23);
            this.bOpenAddressPage.TabIndex = 6;
            this.bOpenAddressPage.Text = "Открыть web-интерфейс";
            this.bOpenAddressPage.UseVisualStyleBackColor = true;
            this.bOpenAddressPage.Click += new System.EventHandler(this.bOpenAddressPage_Click);
            // 
            // lAddress
            // 
            this.lAddress.AutoSize = true;
            this.lAddress.Location = new System.Drawing.Point(12, 9);
            this.lAddress.Name = "lAddress";
            this.lAddress.Size = new System.Drawing.Size(70, 13);
            this.lAddress.TabIndex = 5;
            this.lAddress.Text = "Адрес ФИС:";
            // 
            // cbAddress
            // 
            this.cbAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAddress.FormattingEnabled = true;
            this.cbAddress.Location = new System.Drawing.Point(12, 25);
            this.cbAddress.Name = "cbAddress";
            this.cbAddress.Size = new System.Drawing.Size(150, 21);
            this.cbAddress.TabIndex = 4;
            // 
            // bExport
            // 
            this.bExport.Location = new System.Drawing.Point(328, 23);
            this.bExport.Name = "bExport";
            this.bExport.Size = new System.Drawing.Size(75, 23);
            this.bExport.TabIndex = 8;
            this.bExport.Text = "Экспорт";
            this.bExport.UseVisualStyleBackColor = true;
            this.bExport.Click += new System.EventHandler(this.bExport_Click);
            // 
            // bOpen
            // 
            this.bOpen.Location = new System.Drawing.Point(304, 227);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(99, 23);
            this.bOpen.TabIndex = 9;
            this.bOpen.Text = "Открыть файл...";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xml";
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.Filter = "XML-файлы|*.xml|Все файлы|*.*";
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(12, 52);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(391, 169);
            this.textBox.TabIndex = 7;
            // 
            // nudDictionaryNumber
            // 
            this.nudDictionaryNumber.Location = new System.Drawing.Point(345, 22);
            this.nudDictionaryNumber.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.nudDictionaryNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDictionaryNumber.Name = "nudDictionaryNumber";
            this.nudDictionaryNumber.Size = new System.Drawing.Size(40, 20);
            this.nudDictionaryNumber.TabIndex = 10;
            this.nudDictionaryNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // bLoadDictionary
            // 
            this.bLoadDictionary.Location = new System.Drawing.Point(201, 19);
            this.bLoadDictionary.Name = "bLoadDictionary";
            this.bLoadDictionary.Size = new System.Drawing.Size(138, 23);
            this.bLoadDictionary.TabIndex = 11;
            this.bLoadDictionary.Text = "Загрузить справочник...";
            this.bLoadDictionary.UseVisualStyleBackColor = true;
            this.bLoadDictionary.Click += new System.EventHandler(this.bLoadDictionary_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.Filter = "XML-файлы|*.xml|Все файлы|*.*";
            // 
            // bDictionariesList
            // 
            this.bDictionariesList.Location = new System.Drawing.Point(6, 19);
            this.bDictionariesList.Name = "bDictionariesList";
            this.bDictionariesList.Size = new System.Drawing.Size(189, 23);
            this.bDictionariesList.TabIndex = 12;
            this.bDictionariesList.Text = "Загрузить список справочников...";
            this.bDictionariesList.UseVisualStyleBackColor = true;
            this.bDictionariesList.Click += new System.EventHandler(this.bDictionariesList_Click);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.nudDictionaryNumber);
            this.groupBox.Controls.Add(this.bLoadDictionary);
            this.groupBox.Controls.Add(this.bDictionariesList);
            this.groupBox.Location = new System.Drawing.Point(12, 256);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(391, 52);
            this.groupBox.TabIndex = 13;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Справочники";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 313);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.bExport);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.bOpenAddressPage);
            this.Controls.Add(this.lAddress);
            this.Controls.Add(this.cbAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::FIS_Exporter.Properties.Resources.logo_export;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FIS Exporter";
            ((System.ComponentModel.ISupportInitialize)(this.nudDictionaryNumber)).EndInit();
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bOpenAddressPage;
        private System.Windows.Forms.Label lAddress;
        private System.Windows.Forms.ComboBox cbAddress;
        private System.Windows.Forms.Button bExport;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.NumericUpDown nudDictionaryNumber;
        private System.Windows.Forms.Button bLoadDictionary;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button bDictionariesList;
        private System.Windows.Forms.GroupBox groupBox;
    }
}

