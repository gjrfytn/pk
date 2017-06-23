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
            this.lAdress = new System.Windows.Forms.Label();
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
            this.methodGroupBox = new System.Windows.Forms.GroupBox();
            this.rbPostMethod = new System.Windows.Forms.RadioButton();
            this.rbFTPMethod = new System.Windows.Forms.RadioButton();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lServer = new System.Windows.Forms.Label();
            this.lUser = new System.Windows.Forms.Label();
            this.lPassword = new System.Windows.Forms.Label();
            this.lAdressForScript = new System.Windows.Forms.Label();
            this.tbAdress = new System.Windows.Forms.TextBox();
            this.methodGroupBox.SuspendLayout();
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
            // lAdress
            // 
            this.lAdress.AutoSize = true;
            this.lAdress.Location = new System.Drawing.Point(6, 42);
            this.lAdress.Name = "lAdress";
            this.lAdress.Size = new System.Drawing.Size(91, 13);
            this.lAdress.TabIndex = 3;
            this.lAdress.Text = "Адрес отправки:";
            // 
            // cbAdress
            // 
            this.cbAdress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdress.FormattingEnabled = true;
            this.cbAdress.Items.AddRange(new object[] {
            "http://madi.ru/abit/getXMLdata.php",
            "http://sociomadi.ru/pk/getXMLdata.php"});
            this.cbAdress.Location = new System.Drawing.Point(6, 61);
            this.cbAdress.Name = "cbAdress";
            this.cbAdress.Size = new System.Drawing.Size(311, 21);
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
            this.btStart.Location = new System.Drawing.Point(268, 435);
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
            this.tbResponse.Location = new System.Drawing.Point(12, 236);
            this.tbResponse.Multiline = true;
            this.tbResponse.Name = "tbResponse";
            this.tbResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResponse.Size = new System.Drawing.Size(592, 184);
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
            this.cbPost.Location = new System.Drawing.Point(15, 129);
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
            this.cbSave.Location = new System.Drawing.Point(16, 95);
            this.cbSave.Name = "cbSave";
            this.cbSave.Size = new System.Drawing.Size(166, 17);
            this.cbSave.TabIndex = 11;
            this.cbSave.Text = "Сохранить в файл \"doc.xml\"";
            this.cbSave.UseVisualStyleBackColor = true;
            // 
            // methodGroupBox
            // 
            this.methodGroupBox.Controls.Add(this.tbAdress);
            this.methodGroupBox.Controls.Add(this.tbPassword);
            this.methodGroupBox.Controls.Add(this.tbUser);
            this.methodGroupBox.Controls.Add(this.tbServer);
            this.methodGroupBox.Controls.Add(this.rbFTPMethod);
            this.methodGroupBox.Controls.Add(this.rbPostMethod);
            this.methodGroupBox.Controls.Add(this.lAdressForScript);
            this.methodGroupBox.Controls.Add(this.cbAdress);
            this.methodGroupBox.Controls.Add(this.lPassword);
            this.methodGroupBox.Controls.Add(this.lUser);
            this.methodGroupBox.Controls.Add(this.lServer);
            this.methodGroupBox.Controls.Add(this.lAdress);
            this.methodGroupBox.Enabled = false;
            this.methodGroupBox.Location = new System.Drawing.Point(284, 21);
            this.methodGroupBox.Name = "methodGroupBox";
            this.methodGroupBox.Size = new System.Drawing.Size(323, 194);
            this.methodGroupBox.TabIndex = 12;
            this.methodGroupBox.TabStop = false;
            this.methodGroupBox.Text = "Способ отправки данных";
            // 
            // rbPostMethod
            // 
            this.rbPostMethod.AutoSize = true;
            this.rbPostMethod.Checked = true;
            this.rbPostMethod.Location = new System.Drawing.Point(6, 19);
            this.rbPostMethod.Name = "rbPostMethod";
            this.rbPostMethod.Size = new System.Drawing.Size(93, 17);
            this.rbPostMethod.TabIndex = 0;
            this.rbPostMethod.TabStop = true;
            this.rbPostMethod.Text = "POST запрос";
            this.rbPostMethod.UseVisualStyleBackColor = true;
            this.rbPostMethod.CheckedChanged += new System.EventHandler(this.rbPostMethod_CheckedChanged);
            // 
            // rbFTPMethod
            // 
            this.rbFTPMethod.AutoSize = true;
            this.rbFTPMethod.Location = new System.Drawing.Point(105, 19);
            this.rbFTPMethod.Name = "rbFTPMethod";
            this.rbFTPMethod.Size = new System.Drawing.Size(84, 17);
            this.rbFTPMethod.TabIndex = 0;
            this.rbFTPMethod.Text = "FTP сервер";
            this.rbFTPMethod.UseVisualStyleBackColor = true;
            this.rbFTPMethod.Click += new System.EventHandler(this.rbFTPMethod_Click);
            // 
            // tbServer
            // 
            this.tbServer.Enabled = false;
            this.tbServer.Location = new System.Drawing.Point(123, 88);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(194, 20);
            this.tbServer.TabIndex = 5;
            this.tbServer.Text = "ftp://www.madi.ru/";
            // 
            // tbUser
            // 
            this.tbUser.Enabled = false;
            this.tbUser.Location = new System.Drawing.Point(123, 114);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(194, 20);
            this.tbUser.TabIndex = 5;
            this.tbUser.Text = "madi_abit";
            // 
            // tbPassword
            // 
            this.tbPassword.Enabled = false;
            this.tbPassword.Location = new System.Drawing.Point(123, 140);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(194, 20);
            this.tbPassword.TabIndex = 5;
            this.tbPassword.Text = "hocEEJVr";
            // 
            // lServer
            // 
            this.lServer.AutoSize = true;
            this.lServer.Enabled = false;
            this.lServer.Location = new System.Drawing.Point(8, 91);
            this.lServer.Name = "lServer";
            this.lServer.Size = new System.Drawing.Size(109, 13);
            this.lServer.TabIndex = 3;
            this.lServer.Text = "Адрес FTP сервера:";
            // 
            // lUser
            // 
            this.lUser.AutoSize = true;
            this.lUser.Enabled = false;
            this.lUser.Location = new System.Drawing.Point(76, 117);
            this.lUser.Name = "lUser";
            this.lUser.Size = new System.Drawing.Size(41, 13);
            this.lUser.TabIndex = 3;
            this.lUser.Text = "Логин:";
            // 
            // lPassword
            // 
            this.lPassword.AutoSize = true;
            this.lPassword.Enabled = false;
            this.lPassword.Location = new System.Drawing.Point(69, 143);
            this.lPassword.Name = "lPassword";
            this.lPassword.Size = new System.Drawing.Size(48, 13);
            this.lPassword.TabIndex = 3;
            this.lPassword.Text = "Пароль:";
            // 
            // lAdressForScript
            // 
            this.lAdressForScript.AutoSize = true;
            this.lAdressForScript.Enabled = false;
            this.lAdressForScript.Location = new System.Drawing.Point(6, 169);
            this.lAdressForScript.Name = "lAdressForScript";
            this.lAdressForScript.Size = new System.Drawing.Size(111, 13);
            this.lAdressForScript.TabIndex = 3;
            this.lAdressForScript.Text = "Скрипт для запуска:";
            // 
            // tbAdress
            // 
            this.tbAdress.Enabled = false;
            this.tbAdress.Location = new System.Drawing.Point(123, 166);
            this.tbAdress.Name = "tbAdress";
            this.tbAdress.Size = new System.Drawing.Size(194, 20);
            this.tbAdress.TabIndex = 5;
            this.tbAdress.Text = "http://madi.ru/abit/getXMLdata.php";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 474);
            this.Controls.Add(this.methodGroupBox);
            this.Controls.Add(this.cbSave);
            this.Controls.Add(this.cbPost);
            this.Controls.Add(this.tbResponse);
            this.Controls.Add(this.cbUnits);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.cbInterval);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbCampaigns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Отправка данных на сайт ПК МАДИ";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.methodGroupBox.ResumeLayout(false);
            this.methodGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbCampaigns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lAdress;
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
        private System.Windows.Forms.GroupBox methodGroupBox;
        private System.Windows.Forms.RadioButton rbFTPMethod;
        private System.Windows.Forms.RadioButton rbPostMethod;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbUser;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.Label lServer;
        private System.Windows.Forms.Label lPassword;
        private System.Windows.Forms.Label lUser;
        private System.Windows.Forms.TextBox tbAdress;
        private System.Windows.Forms.Label lAdressForScript;
    }
}

