namespace SitePost
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cbCampaigns = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbAdress = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbInterval = new System.Windows.Forms.ComboBox();
            this.btStart = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.cbUnits = new System.Windows.Forms.ComboBox();
            this.tbResponse = new System.Windows.Forms.TextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cbPost = new System.Windows.Forms.CheckBox();
            this.cbSave = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbCampaigns
            // 
            this.cbCampaigns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCampaigns.FormattingEnabled = true;
            this.cbCampaigns.Location = new System.Drawing.Point(142, 21);
            this.cbCampaigns.Name = "cbCampaigns";
            this.cbCampaigns.Size = new System.Drawing.Size(122, 21);
            this.cbCampaigns.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Кампания бакалавров:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(279, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Адрес отправки:";
            // 
            // cbAdress
            // 
            this.cbAdress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdress.FormattingEnabled = true;
            this.cbAdress.Items.AddRange(new object[] {
            "http://madi.ru/abit/getXMLdata.php",
            "http://sociomadi.ru/pk/getXMLdata.php"});
            this.cbAdress.Location = new System.Drawing.Point(376, 57);
            this.cbAdress.Name = "cbAdress";
            this.cbAdress.Size = new System.Drawing.Size(231, 21);
            this.cbAdress.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Частота отправки: 1 раз в";
            // 
            // cbInterval
            // 
            this.cbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInterval.FormattingEnabled = true;
            this.cbInterval.Location = new System.Drawing.Point(157, 57);
            this.cbInterval.Name = "cbInterval";
            this.cbInterval.Size = new System.Drawing.Size(55, 21);
            this.cbInterval.TabIndex = 6;
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(271, 231);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(75, 23);
            this.btStart.TabIndex = 7;
            this.btStart.Text = "Старт";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // timer
            // 
            this.timer.Interval = 10000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // cbUnits
            // 
            this.cbUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnits.FormattingEnabled = true;
            this.cbUnits.Items.AddRange(new object[] {
            "ч",
            "мин.",
            "сек."});
            this.cbUnits.Location = new System.Drawing.Point(218, 57);
            this.cbUnits.Name = "cbUnits";
            this.cbUnits.Size = new System.Drawing.Size(46, 21);
            this.cbUnits.TabIndex = 8;
            // 
            // tbResponse
            // 
            this.tbResponse.Location = new System.Drawing.Point(15, 95);
            this.tbResponse.Multiline = true;
            this.tbResponse.Name = "tbResponse";
            this.tbResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResponse.Size = new System.Drawing.Size(592, 125);
            this.tbResponse.TabIndex = 9;
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Отправка данных на сайт ПК МАДИ";
            this.notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
            // 
            // cbPost
            // 
            this.cbPost.AutoSize = true;
            this.cbPost.Location = new System.Drawing.Point(476, 23);
            this.cbPost.Name = "cbPost";
            this.cbPost.Size = new System.Drawing.Size(131, 17);
            this.cbPost.TabIndex = 10;
            this.cbPost.Text = "Выполнять отправку";
            this.cbPost.UseVisualStyleBackColor = true;
            this.cbPost.CheckedChanged += new System.EventHandler(this.cbPost_CheckedChanged);
            // 
            // cbSave
            // 
            this.cbSave.AutoSize = true;
            this.cbSave.Checked = true;
            this.cbSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSave.Location = new System.Drawing.Point(282, 23);
            this.cbSave.Name = "cbSave";
            this.cbSave.Size = new System.Drawing.Size(166, 17);
            this.cbSave.TabIndex = 11;
            this.cbSave.Text = "Сохранить в файл \"doc.xml\"";
            this.cbSave.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 266);
            this.Controls.Add(this.cbSave);
            this.Controls.Add(this.cbPost);
            this.Controls.Add(this.tbResponse);
            this.Controls.Add(this.cbUnits);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.cbInterval);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbAdress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbCampaigns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Отправка данных на сайт ПК МАДИ";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbCampaigns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbAdress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbInterval;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ComboBox cbUnits;
        private System.Windows.Forms.TextBox tbResponse;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.CheckBox cbPost;
        private System.Windows.Forms.CheckBox cbSave;
    }
}

