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
			this.tcMethod = new System.Windows.Forms.TabControl();
			this.tpPostMethod = new System.Windows.Forms.TabPage();
			this.tpFTPMethod = new System.Windows.Forms.TabPage();
			this.lServer = new System.Windows.Forms.Label();
			this.tbAdress = new System.Windows.Forms.TextBox();
			this.lUser = new System.Windows.Forms.Label();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.lPassword = new System.Windows.Forms.Label();
			this.tbUser = new System.Windows.Forms.TextBox();
			this.lAdressForScript = new System.Windows.Forms.Label();
			this.tbServer = new System.Windows.Forms.TextBox();
			this.tpDirectToDBMethod = new System.Windows.Forms.TabPage();
			this.tbDirectToDBBaseName = new System.Windows.Forms.TextBox();
			this.lDirectToDBBaseName = new System.Windows.Forms.Label();
			this.lDirectToDBServer = new System.Windows.Forms.Label();
			this.tbDirectToDBAdressForScript = new System.Windows.Forms.TextBox();
			this.lDirectToDBUser = new System.Windows.Forms.Label();
			this.tbDirectToDBPassword = new System.Windows.Forms.TextBox();
			this.lDirectToDBPassword = new System.Windows.Forms.Label();
			this.tbDirectToDBUser = new System.Windows.Forms.TextBox();
			this.lDirectToDBAdressForScript = new System.Windows.Forms.Label();
			this.tbDirectToDBServer = new System.Windows.Forms.TextBox();
			this.rbDirectToDB = new System.Windows.Forms.RadioButton();
			this.rbFTPMethod = new System.Windows.Forms.RadioButton();
			this.rbPostMethod = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lAverageTime = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lTime = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lCount = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.timerSum = new System.Windows.Forms.Timer(this.components);
			this.methodGroupBox.SuspendLayout();
			this.tcMethod.SuspendLayout();
			this.tpPostMethod.SuspendLayout();
			this.tpFTPMethod.SuspendLayout();
			this.tpDirectToDBMethod.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbCampaigns
			// 
			this.cbCampaigns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbCampaigns.FormattingEnabled = true;
			this.cbCampaigns.Location = new System.Drawing.Point(142, 21);
			this.cbCampaigns.Name = "cbCampaigns";
			this.cbCampaigns.Size = new System.Drawing.Size(480, 21);
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
			this.lAdress.Location = new System.Drawing.Point(6, 8);
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
			this.cbAdress.Location = new System.Drawing.Point(6, 27);
			this.cbAdress.Name = "cbAdress";
			this.cbAdress.Size = new System.Drawing.Size(311, 21);
			this.cbAdress.TabIndex = 10;
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
			this.cbInterval.TabIndex = 2;
			// 
			// btStart
			// 
			this.btStart.Location = new System.Drawing.Point(268, 435);
			this.btStart.Name = "btStart";
			this.btStart.Size = new System.Drawing.Size(75, 23);
			this.btStart.TabIndex = 22;
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
			this.cbUnits.TabIndex = 3;
			// 
			// tbResponse
			// 
			this.tbResponse.Location = new System.Drawing.Point(12, 277);
			this.tbResponse.Multiline = true;
			this.tbResponse.Name = "tbResponse";
			this.tbResponse.ReadOnly = true;
			this.tbResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbResponse.Size = new System.Drawing.Size(615, 148);
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
			this.cbPost.TabIndex = 5;
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
			this.cbSave.TabIndex = 4;
			this.cbSave.Text = "Сохранить в файл \"doc.xml\"";
			this.cbSave.UseVisualStyleBackColor = true;
			// 
			// methodGroupBox
			// 
			this.methodGroupBox.Controls.Add(this.tcMethod);
			this.methodGroupBox.Controls.Add(this.rbDirectToDB);
			this.methodGroupBox.Controls.Add(this.rbFTPMethod);
			this.methodGroupBox.Controls.Add(this.rbPostMethod);
			this.methodGroupBox.Enabled = false;
			this.methodGroupBox.Location = new System.Drawing.Point(284, 57);
			this.methodGroupBox.Name = "methodGroupBox";
			this.methodGroupBox.Size = new System.Drawing.Size(343, 214);
			this.methodGroupBox.TabIndex = 12;
			this.methodGroupBox.TabStop = false;
			this.methodGroupBox.Text = "Способ отправки данных";
			// 
			// tcMethod
			// 
			this.tcMethod.Controls.Add(this.tpPostMethod);
			this.tcMethod.Controls.Add(this.tpFTPMethod);
			this.tcMethod.Controls.Add(this.tpDirectToDBMethod);
			this.tcMethod.Location = new System.Drawing.Point(7, 42);
			this.tcMethod.Name = "tcMethod";
			this.tcMethod.SelectedIndex = 0;
			this.tcMethod.Size = new System.Drawing.Size(331, 167);
			this.tcMethod.TabIndex = 16;
			// 
			// tpPostMethod
			// 
			this.tpPostMethod.Controls.Add(this.cbAdress);
			this.tpPostMethod.Controls.Add(this.lAdress);
			this.tpPostMethod.Location = new System.Drawing.Point(4, 22);
			this.tpPostMethod.Name = "tpPostMethod";
			this.tpPostMethod.Padding = new System.Windows.Forms.Padding(3);
			this.tpPostMethod.Size = new System.Drawing.Size(323, 141);
			this.tpPostMethod.TabIndex = 0;
			this.tpPostMethod.Text = "POST запрос";
			this.tpPostMethod.UseVisualStyleBackColor = true;
			// 
			// tpFTPMethod
			// 
			this.tpFTPMethod.Controls.Add(this.lServer);
			this.tpFTPMethod.Controls.Add(this.tbAdress);
			this.tpFTPMethod.Controls.Add(this.lUser);
			this.tpFTPMethod.Controls.Add(this.tbPassword);
			this.tpFTPMethod.Controls.Add(this.lPassword);
			this.tpFTPMethod.Controls.Add(this.tbUser);
			this.tpFTPMethod.Controls.Add(this.lAdressForScript);
			this.tpFTPMethod.Controls.Add(this.tbServer);
			this.tpFTPMethod.Location = new System.Drawing.Point(4, 22);
			this.tpFTPMethod.Name = "tpFTPMethod";
			this.tpFTPMethod.Padding = new System.Windows.Forms.Padding(3);
			this.tpFTPMethod.Size = new System.Drawing.Size(323, 141);
			this.tpFTPMethod.TabIndex = 1;
			this.tpFTPMethod.Text = "FTP сервер";
			this.tpFTPMethod.UseVisualStyleBackColor = true;
			// 
			// lServer
			// 
			this.lServer.AutoSize = true;
			this.lServer.Enabled = false;
			this.lServer.Location = new System.Drawing.Point(6, 14);
			this.lServer.Name = "lServer";
			this.lServer.Size = new System.Drawing.Size(109, 13);
			this.lServer.TabIndex = 3;
			this.lServer.Text = "Адрес FTP сервера:";
			// 
			// tbAdress
			// 
			this.tbAdress.Enabled = false;
			this.tbAdress.Location = new System.Drawing.Point(121, 89);
			this.tbAdress.Name = "tbAdress";
			this.tbAdress.Size = new System.Drawing.Size(194, 20);
			this.tbAdress.TabIndex = 15;
			this.tbAdress.Text = "http://madi.ru/abit/getXMLdata.php";
			// 
			// lUser
			// 
			this.lUser.AutoSize = true;
			this.lUser.Enabled = false;
			this.lUser.Location = new System.Drawing.Point(74, 40);
			this.lUser.Name = "lUser";
			this.lUser.Size = new System.Drawing.Size(41, 13);
			this.lUser.TabIndex = 3;
			this.lUser.Text = "Логин:";
			// 
			// tbPassword
			// 
			this.tbPassword.Enabled = false;
			this.tbPassword.Location = new System.Drawing.Point(121, 63);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.PasswordChar = '*';
			this.tbPassword.Size = new System.Drawing.Size(194, 20);
			this.tbPassword.TabIndex = 14;
			this.tbPassword.Text = "hocEEJVr";
			// 
			// lPassword
			// 
			this.lPassword.AutoSize = true;
			this.lPassword.Enabled = false;
			this.lPassword.Location = new System.Drawing.Point(67, 66);
			this.lPassword.Name = "lPassword";
			this.lPassword.Size = new System.Drawing.Size(48, 13);
			this.lPassword.TabIndex = 3;
			this.lPassword.Text = "Пароль:";
			// 
			// tbUser
			// 
			this.tbUser.Enabled = false;
			this.tbUser.Location = new System.Drawing.Point(121, 37);
			this.tbUser.Name = "tbUser";
			this.tbUser.Size = new System.Drawing.Size(194, 20);
			this.tbUser.TabIndex = 13;
			this.tbUser.Text = "madi_abit";
			// 
			// lAdressForScript
			// 
			this.lAdressForScript.AutoSize = true;
			this.lAdressForScript.Enabled = false;
			this.lAdressForScript.Location = new System.Drawing.Point(4, 92);
			this.lAdressForScript.Name = "lAdressForScript";
			this.lAdressForScript.Size = new System.Drawing.Size(111, 13);
			this.lAdressForScript.TabIndex = 3;
			this.lAdressForScript.Text = "Скрипт для запуска:";
			// 
			// tbServer
			// 
			this.tbServer.Enabled = false;
			this.tbServer.Location = new System.Drawing.Point(121, 11);
			this.tbServer.Name = "tbServer";
			this.tbServer.Size = new System.Drawing.Size(194, 20);
			this.tbServer.TabIndex = 12;
			this.tbServer.Text = "ftp://www.madi.ru/";
			// 
			// tpDirectToDBMethod
			// 
			this.tpDirectToDBMethod.Controls.Add(this.tbDirectToDBBaseName);
			this.tpDirectToDBMethod.Controls.Add(this.lDirectToDBBaseName);
			this.tpDirectToDBMethod.Controls.Add(this.lDirectToDBServer);
			this.tpDirectToDBMethod.Controls.Add(this.tbDirectToDBAdressForScript);
			this.tpDirectToDBMethod.Controls.Add(this.lDirectToDBUser);
			this.tpDirectToDBMethod.Controls.Add(this.tbDirectToDBPassword);
			this.tpDirectToDBMethod.Controls.Add(this.lDirectToDBPassword);
			this.tpDirectToDBMethod.Controls.Add(this.tbDirectToDBUser);
			this.tpDirectToDBMethod.Controls.Add(this.lDirectToDBAdressForScript);
			this.tpDirectToDBMethod.Controls.Add(this.tbDirectToDBServer);
			this.tpDirectToDBMethod.Location = new System.Drawing.Point(4, 22);
			this.tpDirectToDBMethod.Name = "tpDirectToDBMethod";
			this.tpDirectToDBMethod.Padding = new System.Windows.Forms.Padding(3);
			this.tpDirectToDBMethod.Size = new System.Drawing.Size(323, 141);
			this.tpDirectToDBMethod.TabIndex = 2;
			this.tpDirectToDBMethod.Text = "Напрямую в БД";
			this.tpDirectToDBMethod.UseVisualStyleBackColor = true;
			// 
			// tbDirectToDBBaseName
			// 
			this.tbDirectToDBBaseName.Enabled = false;
			this.tbDirectToDBBaseName.Location = new System.Drawing.Point(138, 87);
			this.tbDirectToDBBaseName.Name = "tbDirectToDBBaseName";
			this.tbDirectToDBBaseName.Size = new System.Drawing.Size(179, 20);
			this.tbDirectToDBBaseName.TabIndex = 20;
			this.tbDirectToDBBaseName.Text = "madi_abit";
			// 
			// lDirectToDBBaseName
			// 
			this.lDirectToDBBaseName.AutoSize = true;
			this.lDirectToDBBaseName.Enabled = false;
			this.lDirectToDBBaseName.Location = new System.Drawing.Point(81, 90);
			this.lDirectToDBBaseName.Name = "lDirectToDBBaseName";
			this.lDirectToDBBaseName.Size = new System.Drawing.Size(51, 13);
			this.lDirectToDBBaseName.TabIndex = 14;
			this.lDirectToDBBaseName.Text = "Имя БД:";
			// 
			// lDirectToDBServer
			// 
			this.lDirectToDBServer.AutoSize = true;
			this.lDirectToDBServer.Enabled = false;
			this.lDirectToDBServer.Location = new System.Drawing.Point(8, 13);
			this.lDirectToDBServer.Name = "lDirectToDBServer";
			this.lDirectToDBServer.Size = new System.Drawing.Size(124, 13);
			this.lDirectToDBServer.TabIndex = 6;
			this.lDirectToDBServer.Text = "Адрес MySQL сервера:";
			// 
			// tbDirectToDBAdressForScript
			// 
			this.tbDirectToDBAdressForScript.Enabled = false;
			this.tbDirectToDBAdressForScript.Location = new System.Drawing.Point(138, 112);
			this.tbDirectToDBAdressForScript.Name = "tbDirectToDBAdressForScript";
			this.tbDirectToDBAdressForScript.Size = new System.Drawing.Size(179, 20);
			this.tbDirectToDBAdressForScript.TabIndex = 21;
			this.tbDirectToDBAdressForScript.Text = "http://madi.ru/abit/copyDB.php";
			// 
			// lDirectToDBUser
			// 
			this.lDirectToDBUser.AutoSize = true;
			this.lDirectToDBUser.Enabled = false;
			this.lDirectToDBUser.Location = new System.Drawing.Point(91, 39);
			this.lDirectToDBUser.Name = "lDirectToDBUser";
			this.lDirectToDBUser.Size = new System.Drawing.Size(41, 13);
			this.lDirectToDBUser.TabIndex = 7;
			this.lDirectToDBUser.Text = "Логин:";
			// 
			// tbDirectToDBPassword
			// 
			this.tbDirectToDBPassword.Enabled = false;
			this.tbDirectToDBPassword.Location = new System.Drawing.Point(138, 62);
			this.tbDirectToDBPassword.Name = "tbDirectToDBPassword";
			this.tbDirectToDBPassword.PasswordChar = '*';
			this.tbDirectToDBPassword.Size = new System.Drawing.Size(179, 20);
			this.tbDirectToDBPassword.TabIndex = 19;
			this.tbDirectToDBPassword.Text = "fVxcXtEH";
			// 
			// lDirectToDBPassword
			// 
			this.lDirectToDBPassword.AutoSize = true;
			this.lDirectToDBPassword.Enabled = false;
			this.lDirectToDBPassword.Location = new System.Drawing.Point(84, 65);
			this.lDirectToDBPassword.Name = "lDirectToDBPassword";
			this.lDirectToDBPassword.Size = new System.Drawing.Size(48, 13);
			this.lDirectToDBPassword.TabIndex = 8;
			this.lDirectToDBPassword.Text = "Пароль:";
			// 
			// tbDirectToDBUser
			// 
			this.tbDirectToDBUser.Enabled = false;
			this.tbDirectToDBUser.Location = new System.Drawing.Point(138, 36);
			this.tbDirectToDBUser.Name = "tbDirectToDBUser";
			this.tbDirectToDBUser.Size = new System.Drawing.Size(179, 20);
			this.tbDirectToDBUser.TabIndex = 18;
			this.tbDirectToDBUser.Text = "madi_abit";
			// 
			// lDirectToDBAdressForScript
			// 
			this.lDirectToDBAdressForScript.AutoSize = true;
			this.lDirectToDBAdressForScript.Enabled = false;
			this.lDirectToDBAdressForScript.Location = new System.Drawing.Point(21, 115);
			this.lDirectToDBAdressForScript.Name = "lDirectToDBAdressForScript";
			this.lDirectToDBAdressForScript.Size = new System.Drawing.Size(111, 13);
			this.lDirectToDBAdressForScript.TabIndex = 9;
			this.lDirectToDBAdressForScript.Text = "Скрипт для запуска:";
			// 
			// tbDirectToDBServer
			// 
			this.tbDirectToDBServer.Enabled = false;
			this.tbDirectToDBServer.Location = new System.Drawing.Point(138, 10);
			this.tbDirectToDBServer.Name = "tbDirectToDBServer";
			this.tbDirectToDBServer.Size = new System.Drawing.Size(179, 20);
			this.tbDirectToDBServer.TabIndex = 17;
			this.tbDirectToDBServer.Text = "madihost.ru";
			// 
			// rbDirectToDB
			// 
			this.rbDirectToDB.AutoSize = true;
			this.rbDirectToDB.Location = new System.Drawing.Point(195, 19);
			this.rbDirectToDB.Name = "rbDirectToDB";
			this.rbDirectToDB.Size = new System.Drawing.Size(106, 17);
			this.rbDirectToDB.TabIndex = 8;
			this.rbDirectToDB.Text = "Напрямую в БД";
			this.rbDirectToDB.UseVisualStyleBackColor = true;
			this.rbDirectToDB.CheckedChanged += new System.EventHandler(this.rbDirectToDB_CheckedChanged);
			this.rbDirectToDB.Click += new System.EventHandler(this.rbDirectToDB_Click);
			// 
			// rbFTPMethod
			// 
			this.rbFTPMethod.AutoSize = true;
			this.rbFTPMethod.Location = new System.Drawing.Point(105, 19);
			this.rbFTPMethod.Name = "rbFTPMethod";
			this.rbFTPMethod.Size = new System.Drawing.Size(84, 17);
			this.rbFTPMethod.TabIndex = 7;
			this.rbFTPMethod.Text = "FTP сервер";
			this.rbFTPMethod.UseVisualStyleBackColor = true;
			this.rbFTPMethod.CheckedChanged += new System.EventHandler(this.rbFTPMethod_CheckedChanged);
			this.rbFTPMethod.Click += new System.EventHandler(this.rbFTPMethod_Click);
			// 
			// rbPostMethod
			// 
			this.rbPostMethod.AutoSize = true;
			this.rbPostMethod.Checked = true;
			this.rbPostMethod.Location = new System.Drawing.Point(6, 19);
			this.rbPostMethod.Name = "rbPostMethod";
			this.rbPostMethod.Size = new System.Drawing.Size(93, 17);
			this.rbPostMethod.TabIndex = 6;
			this.rbPostMethod.TabStop = true;
			this.rbPostMethod.Text = "POST запрос";
			this.rbPostMethod.UseVisualStyleBackColor = true;
			this.rbPostMethod.CheckedChanged += new System.EventHandler(this.rbPostMethod_CheckedChanged);
			this.rbPostMethod.Click += new System.EventHandler(this.rbPostMethod_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lAverageTime);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.lTime);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.lCount);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 188);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(266, 83);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Статистика текущей отправки";
			// 
			// lAverageTime
			// 
			this.lAverageTime.AutoSize = true;
			this.lAverageTime.Location = new System.Drawing.Point(109, 61);
			this.lAverageTime.Name = "lAverageTime";
			this.lAverageTime.Size = new System.Drawing.Size(118, 13);
			this.lAverageTime.TabIndex = 3;
			this.lAverageTime.Text = "0 сек. на абитуриента";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(20, 61);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(88, 13);
			this.label6.TabIndex = 4;
			this.label6.Text = "Среднее время:";
			// 
			// lTime
			// 
			this.lTime.AutoSize = true;
			this.lTime.Location = new System.Drawing.Point(109, 41);
			this.lTime.Name = "lTime";
			this.lTime.Size = new System.Drawing.Size(84, 13);
			this.lTime.TabIndex = 1;
			this.lTime.Text = "00 мин. 00 сек.";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(65, 41);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(43, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "Время:";
			// 
			// lCount
			// 
			this.lCount.AutoSize = true;
			this.lCount.Location = new System.Drawing.Point(109, 21);
			this.lCount.Name = "lCount";
			this.lCount.Size = new System.Drawing.Size(85, 13);
			this.lCount.TabIndex = 0;
			this.lCount.Text = "0 абитуриентов";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(37, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Обработано:";
			// 
			// timerSum
			// 
			this.timerSum.Interval = 1000;
			this.timerSum.Tick += new System.EventHandler(this.timerSum_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(640, 474);
			this.Controls.Add(this.groupBox1);
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
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.methodGroupBox.ResumeLayout(false);
			this.methodGroupBox.PerformLayout();
			this.tcMethod.ResumeLayout(false);
			this.tpPostMethod.ResumeLayout(false);
			this.tpPostMethod.PerformLayout();
			this.tpFTPMethod.ResumeLayout(false);
			this.tpFTPMethod.PerformLayout();
			this.tpDirectToDBMethod.ResumeLayout(false);
			this.tpDirectToDBMethod.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.RadioButton rbDirectToDB;
        private System.Windows.Forms.TabControl tcMethod;
        private System.Windows.Forms.TabPage tpPostMethod;
        private System.Windows.Forms.TabPage tpFTPMethod;
        private System.Windows.Forms.TabPage tpDirectToDBMethod;
        private System.Windows.Forms.TextBox tbDirectToDBBaseName;
        private System.Windows.Forms.Label lDirectToDBBaseName;
        private System.Windows.Forms.Label lDirectToDBServer;
        private System.Windows.Forms.TextBox tbDirectToDBAdressForScript;
        private System.Windows.Forms.Label lDirectToDBUser;
        private System.Windows.Forms.TextBox tbDirectToDBPassword;
        private System.Windows.Forms.Label lDirectToDBPassword;
        private System.Windows.Forms.TextBox tbDirectToDBUser;
        private System.Windows.Forms.Label lDirectToDBAdressForScript;
        private System.Windows.Forms.TextBox tbDirectToDBServer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lCount;
        private System.Windows.Forms.Label lTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lAverageTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timerSum;
    }
}

