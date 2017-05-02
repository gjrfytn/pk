namespace PK.Forms
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripMain_CreateApplication = new System.Windows.Forms.ToolStripButton();
            this.toolStripMain_CreateMagApplication = new System.Windows.Forms.ToolStripButton();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuStrip_Campaign = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Campaign_Campaigns = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Campaign_Exams = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Orders = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Entrants = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_CreateBacApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_CreateMagApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Univesity = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Constants = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Faculties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Directions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_TargetOrganizations = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_InstitutionAchievements = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_OutDocuments = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_RegJournal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Administration = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Users = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_FIS_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_KLADR_Update = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Dictionaries = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_DirDictionary = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_OlympDictionary = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvApplications = new System.Windows.Forms.DataGridView();
            this.dgvApplications_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_MiddleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_Original = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvApplications_Entrances = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_RegDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_EditDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_PickUpDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_DeductionDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_EnrollmentDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_RegistratorLogin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.tbLastName = new System.Windows.Forms.TextBox();
            this.tbFirstName = new System.Windows.Forms.TextBox();
            this.tbMiddleName = new System.Windows.Forms.TextBox();
            this.dtpRegDate = new System.Windows.Forms.DateTimePicker();
            this.tbRegNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbDateSearch = new System.Windows.Forms.CheckBox();
            this.cbEnroll = new System.Windows.Forms.CheckBox();
            this.cbPickUp = new System.Windows.Forms.CheckBox();
            this.cbNew = new System.Windows.Forms.CheckBox();
            this.toolStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplications)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMain_CreateApplication,
            this.toolStripMain_CreateMagApplication});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1008, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripMain_CreateApplication
            // 
            this.toolStripMain_CreateApplication.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMain_CreateApplication.Image")));
            this.toolStripMain_CreateApplication.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMain_CreateApplication.Name = "toolStripMain_CreateApplication";
            this.toolStripMain_CreateApplication.Size = new System.Drawing.Size(189, 22);
            this.toolStripMain_CreateApplication.Text = "Создать заявление бакалавра";
            this.toolStripMain_CreateApplication.Click += new System.EventHandler(this.toolStrip_CreateApplication_Click);
            // 
            // toolStripMain_CreateMagApplication
            // 
            this.toolStripMain_CreateMagApplication.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMain_CreateMagApplication.Image")));
            this.toolStripMain_CreateMagApplication.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMain_CreateMagApplication.Name = "toolStripMain_CreateMagApplication";
            this.toolStripMain_CreateMagApplication.Size = new System.Drawing.Size(183, 22);
            this.toolStripMain_CreateMagApplication.Text = "Создать заявление магистра";
            this.toolStripMain_CreateMagApplication.Click += new System.EventHandler(this.toolStripMain_CreateMagApplication_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Campaign,
            this.menuStrip_Entrants,
            this.menuStrip_Univesity,
            this.menuStrip_OutDocuments,
            this.menuStrip_Administration,
            this.menuStrip_Help});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuStrip_Campaign
            // 
            this.menuStrip_Campaign.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Campaign_Campaigns,
            this.menuStrip_Campaign_Exams,
            this.menuStrip_Orders});
            this.menuStrip_Campaign.Name = "menuStrip_Campaign";
            this.menuStrip_Campaign.Size = new System.Drawing.Size(133, 20);
            this.menuStrip_Campaign.Text = "Приемная кампания";
            // 
            // menuStrip_Campaign_Campaigns
            // 
            this.menuStrip_Campaign_Campaigns.Name = "menuStrip_Campaign_Campaigns";
            this.menuStrip_Campaign_Campaigns.Size = new System.Drawing.Size(192, 22);
            this.menuStrip_Campaign_Campaigns.Text = "Приемные кампании";
            this.menuStrip_Campaign_Campaigns.Click += new System.EventHandler(this.menuStrip_Campaign_Campaigns_Click);
            // 
            // menuStrip_Campaign_Exams
            // 
            this.menuStrip_Campaign_Exams.Name = "menuStrip_Campaign_Exams";
            this.menuStrip_Campaign_Exams.Size = new System.Drawing.Size(192, 22);
            this.menuStrip_Campaign_Exams.Text = "Экзамены";
            this.menuStrip_Campaign_Exams.Click += new System.EventHandler(this.menuStrip_Campaign_Exams_Click);
            // 
            // menuStrip_Orders
            // 
            this.menuStrip_Orders.Name = "menuStrip_Orders";
            this.menuStrip_Orders.Size = new System.Drawing.Size(192, 22);
            this.menuStrip_Orders.Text = "Приказы";
            this.menuStrip_Orders.Click += new System.EventHandler(this.menuStrip_Orders_Click);
            // 
            // menuStrip_Entrants
            // 
            this.menuStrip_Entrants.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_CreateBacApplication,
            this.menuStrip_CreateMagApplication});
            this.menuStrip_Entrants.Name = "menuStrip_Entrants";
            this.menuStrip_Entrants.Size = new System.Drawing.Size(93, 20);
            this.menuStrip_Entrants.Text = "Абитуриенты";
            // 
            // menuStrip_CreateBacApplication
            // 
            this.menuStrip_CreateBacApplication.Name = "menuStrip_CreateBacApplication";
            this.menuStrip_CreateBacApplication.Size = new System.Drawing.Size(236, 22);
            this.menuStrip_CreateBacApplication.Text = "Создать заявление бакалавра";
            this.menuStrip_CreateBacApplication.Click += new System.EventHandler(this.menuStrip_CreateApplication_Click);
            // 
            // menuStrip_CreateMagApplication
            // 
            this.menuStrip_CreateMagApplication.Name = "menuStrip_CreateMagApplication";
            this.menuStrip_CreateMagApplication.Size = new System.Drawing.Size(236, 22);
            this.menuStrip_CreateMagApplication.Text = "Создать заявление магистра";
            this.menuStrip_CreateMagApplication.Click += new System.EventHandler(this.menuStrip_CreateMagApplication_Click);
            // 
            // menuStrip_Univesity
            // 
            this.menuStrip_Univesity.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Constants,
            this.menuStrip_Faculties,
            this.menuStrip_Directions,
            this.menuStrip_TargetOrganizations,
            this.menuStrip_InstitutionAchievements});
            this.menuStrip_Univesity.Name = "menuStrip_Univesity";
            this.menuStrip_Univesity.Size = new System.Drawing.Size(88, 20);
            this.menuStrip_Univesity.Text = "Университет";
            // 
            // menuStrip_Constants
            // 
            this.menuStrip_Constants.Name = "menuStrip_Constants";
            this.menuStrip_Constants.Size = new System.Drawing.Size(351, 22);
            this.menuStrip_Constants.Text = "Основная информация";
            this.menuStrip_Constants.Click += new System.EventHandler(this.menuStrip_Constants_Click);
            // 
            // menuStrip_Faculties
            // 
            this.menuStrip_Faculties.Name = "menuStrip_Faculties";
            this.menuStrip_Faculties.Size = new System.Drawing.Size(351, 22);
            this.menuStrip_Faculties.Text = "Факультеты";
            this.menuStrip_Faculties.Click += new System.EventHandler(this.menuStrip_Faculties_Click);
            // 
            // menuStrip_Directions
            // 
            this.menuStrip_Directions.Name = "menuStrip_Directions";
            this.menuStrip_Directions.Size = new System.Drawing.Size(351, 22);
            this.menuStrip_Directions.Text = "Профили подготовки и магистерские программы";
            this.menuStrip_Directions.Click += new System.EventHandler(this.menuStrip_Directions_Click);
            // 
            // menuStrip_TargetOrganizations
            // 
            this.menuStrip_TargetOrganizations.Name = "menuStrip_TargetOrganizations";
            this.menuStrip_TargetOrganizations.Size = new System.Drawing.Size(351, 22);
            this.menuStrip_TargetOrganizations.Text = "Целевые организации";
            this.menuStrip_TargetOrganizations.Click += new System.EventHandler(this.menuStrip_TargetOrganizations_Click);
            // 
            // menuStrip_InstitutionAchievements
            // 
            this.menuStrip_InstitutionAchievements.Name = "menuStrip_InstitutionAchievements";
            this.menuStrip_InstitutionAchievements.Size = new System.Drawing.Size(351, 22);
            this.menuStrip_InstitutionAchievements.Text = "Индивидуальные достижения";
            this.menuStrip_InstitutionAchievements.Click += new System.EventHandler(this.menuStrip_InstitutionAchievements_Click);
            // 
            // menuStrip_OutDocuments
            // 
            this.menuStrip_OutDocuments.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_RegJournal});
            this.menuStrip_OutDocuments.Name = "menuStrip_OutDocuments";
            this.menuStrip_OutDocuments.Size = new System.Drawing.Size(118, 20);
            this.menuStrip_OutDocuments.Text = "Печатные формы";
            // 
            // menuStrip_RegJournal
            // 
            this.menuStrip_RegJournal.Name = "menuStrip_RegJournal";
            this.menuStrip_RegJournal.Size = new System.Drawing.Size(191, 22);
            this.menuStrip_RegJournal.Text = "Журнал регистрации";
            this.menuStrip_RegJournal.Click += new System.EventHandler(this.toolStrip_RegJournal_Click);
            // 
            // menuStrip_Administration
            // 
            this.menuStrip_Administration.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Users,
            this.menuStrip_FIS_Export,
            this.menuStrip_KLADR_Update});
            this.menuStrip_Administration.Name = "menuStrip_Administration";
            this.menuStrip_Administration.Size = new System.Drawing.Size(134, 20);
            this.menuStrip_Administration.Text = "Администрирование";
            // 
            // menuStrip_Users
            // 
            this.menuStrip_Users.Name = "menuStrip_Users";
            this.menuStrip_Users.Size = new System.Drawing.Size(184, 22);
            this.menuStrip_Users.Text = "Пользователи";
            this.menuStrip_Users.Click += new System.EventHandler(this.toolStrip_Users_Click);
            // 
            // menuStrip_FIS_Export
            // 
            this.menuStrip_FIS_Export.Name = "menuStrip_FIS_Export";
            this.menuStrip_FIS_Export.Size = new System.Drawing.Size(184, 22);
            this.menuStrip_FIS_Export.Text = "Экспорт в ФИС";
            this.menuStrip_FIS_Export.Click += new System.EventHandler(this.toolStrip_FIS_Export_Click);
            // 
            // menuStrip_KLADR_Update
            // 
            this.menuStrip_KLADR_Update.Name = "menuStrip_KLADR_Update";
            this.menuStrip_KLADR_Update.Size = new System.Drawing.Size(184, 22);
            this.menuStrip_KLADR_Update.Text = "Обновление КЛАДР";
            this.menuStrip_KLADR_Update.Click += new System.EventHandler(this.menuStrip_KLADR_Update_Click);
            // 
            // menuStrip_Help
            // 
            this.menuStrip_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Dictionaries,
            this.menuStrip_DirDictionary,
            this.menuStrip_OlympDictionary});
            this.menuStrip_Help.Name = "menuStrip_Help";
            this.menuStrip_Help.Size = new System.Drawing.Size(65, 20);
            this.menuStrip_Help.Text = "Справка";
            // 
            // menuStrip_Dictionaries
            // 
            this.menuStrip_Dictionaries.Name = "menuStrip_Dictionaries";
            this.menuStrip_Dictionaries.Size = new System.Drawing.Size(247, 22);
            this.menuStrip_Dictionaries.Text = "Справочники ФИС";
            this.menuStrip_Dictionaries.Click += new System.EventHandler(this.menuStrip_Dictionaries_Click);
            // 
            // menuStrip_DirDictionary
            // 
            this.menuStrip_DirDictionary.Name = "menuStrip_DirDictionary";
            this.menuStrip_DirDictionary.Size = new System.Drawing.Size(247, 22);
            this.menuStrip_DirDictionary.Text = "Справочник направлений ФИС";
            this.menuStrip_DirDictionary.Click += new System.EventHandler(this.menuStrip_DirDictionary_Click);
            // 
            // menuStrip_OlympDictionary
            // 
            this.menuStrip_OlympDictionary.Name = "menuStrip_OlympDictionary";
            this.menuStrip_OlympDictionary.Size = new System.Drawing.Size(247, 22);
            this.menuStrip_OlympDictionary.Text = "Справочник олимпиад ФИС";
            this.menuStrip_OlympDictionary.Click += new System.EventHandler(this.menuStrip_OlympDictionary_Click);
            // 
            // dgvApplications
            // 
            this.dgvApplications.AllowUserToAddRows = false;
            this.dgvApplications.AllowUserToDeleteRows = false;
            this.dgvApplications.AllowUserToResizeRows = false;
            this.dgvApplications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvApplications.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvApplications.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvApplications_ID,
            this.dgvApplications_LastName,
            this.dgvApplications_FirstName,
            this.dgvApplications_MiddleName,
            this.dgvApplications_Original,
            this.dgvApplications_Entrances,
            this.dgvApplications_RegDate,
            this.dgvApplications_EditDate,
            this.dgvApplications_PickUpDate,
            this.dgvApplications_DeductionDate,
            this.dgvApplications_EnrollmentDate,
            this.dgvApplications_RegistratorLogin,
            this.dgvApplications_Status});
            this.dgvApplications.Location = new System.Drawing.Point(0, 76);
            this.dgvApplications.MultiSelect = false;
            this.dgvApplications.Name = "dgvApplications";
            this.dgvApplications.ReadOnly = true;
            this.dgvApplications.RowHeadersVisible = false;
            this.dgvApplications.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvApplications.Size = new System.Drawing.Size(1008, 385);
            this.dgvApplications.TabIndex = 2;
            this.dgvApplications.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvApplications_CellClick);
            this.dgvApplications.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvApplications_CellDoubleClick);
            // 
            // dgvApplications_ID
            // 
            this.dgvApplications_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvApplications_ID.HeaderText = "№ заявления";
            this.dgvApplications_ID.Name = "dgvApplications_ID";
            this.dgvApplications_ID.ReadOnly = true;
            this.dgvApplications_ID.Width = 92;
            // 
            // dgvApplications_LastName
            // 
            this.dgvApplications_LastName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvApplications_LastName.HeaderText = "Фамилия";
            this.dgvApplications_LastName.Name = "dgvApplications_LastName";
            this.dgvApplications_LastName.ReadOnly = true;
            // 
            // dgvApplications_FirstName
            // 
            this.dgvApplications_FirstName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvApplications_FirstName.HeaderText = "Имя";
            this.dgvApplications_FirstName.Name = "dgvApplications_FirstName";
            this.dgvApplications_FirstName.ReadOnly = true;
            // 
            // dgvApplications_MiddleName
            // 
            this.dgvApplications_MiddleName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvApplications_MiddleName.HeaderText = "Отчество";
            this.dgvApplications_MiddleName.Name = "dgvApplications_MiddleName";
            this.dgvApplications_MiddleName.ReadOnly = true;
            // 
            // dgvApplications_Original
            // 
            this.dgvApplications_Original.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvApplications_Original.HeaderText = "Оригиналы";
            this.dgvApplications_Original.Name = "dgvApplications_Original";
            this.dgvApplications_Original.ReadOnly = true;
            this.dgvApplications_Original.Width = 70;
            // 
            // dgvApplications_Entrances
            // 
            this.dgvApplications_Entrances.HeaderText = "Направления";
            this.dgvApplications_Entrances.Name = "dgvApplications_Entrances";
            this.dgvApplications_Entrances.ReadOnly = true;
            // 
            // dgvApplications_RegDate
            // 
            this.dgvApplications_RegDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvApplications_RegDate.HeaderText = "Дата подачи";
            this.dgvApplications_RegDate.Name = "dgvApplications_RegDate";
            this.dgvApplications_RegDate.ReadOnly = true;
            this.dgvApplications_RegDate.Width = 88;
            // 
            // dgvApplications_EditDate
            // 
            this.dgvApplications_EditDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvApplications_EditDate.HeaderText = "Дата изменения";
            this.dgvApplications_EditDate.Name = "dgvApplications_EditDate";
            this.dgvApplications_EditDate.ReadOnly = true;
            this.dgvApplications_EditDate.Width = 107;
            // 
            // dgvApplications_PickUpDate
            // 
            this.dgvApplications_PickUpDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvApplications_PickUpDate.HeaderText = "Забрал документы";
            this.dgvApplications_PickUpDate.Name = "dgvApplications_PickUpDate";
            this.dgvApplications_PickUpDate.ReadOnly = true;
            this.dgvApplications_PickUpDate.Width = 70;
            // 
            // dgvApplications_DeductionDate
            // 
            this.dgvApplications_DeductionDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvApplications_DeductionDate.HeaderText = "Приказ об отчислении";
            this.dgvApplications_DeductionDate.Name = "dgvApplications_DeductionDate";
            this.dgvApplications_DeductionDate.ReadOnly = true;
            this.dgvApplications_DeductionDate.Width = 80;
            // 
            // dgvApplications_EnrollmentDate
            // 
            this.dgvApplications_EnrollmentDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvApplications_EnrollmentDate.HeaderText = "Приказ о зачислении";
            this.dgvApplications_EnrollmentDate.Name = "dgvApplications_EnrollmentDate";
            this.dgvApplications_EnrollmentDate.ReadOnly = true;
            this.dgvApplications_EnrollmentDate.Width = 80;
            // 
            // dgvApplications_RegistratorLogin
            // 
            this.dgvApplications_RegistratorLogin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvApplications_RegistratorLogin.HeaderText = "Регистратор";
            this.dgvApplications_RegistratorLogin.Name = "dgvApplications_RegistratorLogin";
            this.dgvApplications_RegistratorLogin.ReadOnly = true;
            this.dgvApplications_RegistratorLogin.Width = 96;
            // 
            // dgvApplications_Status
            // 
            this.dgvApplications_Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvApplications_Status.HeaderText = "Статус";
            this.dgvApplications_Status.Name = "dgvApplications_Status";
            this.dgvApplications_Status.ReadOnly = true;
            this.dgvApplications_Status.Width = 70;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Поиск:";
            // 
            // tbLastName
            // 
            this.tbLastName.ForeColor = System.Drawing.Color.Gray;
            this.tbLastName.Location = new System.Drawing.Point(185, 52);
            this.tbLastName.Name = "tbLastName";
            this.tbLastName.Size = new System.Drawing.Size(170, 20);
            this.tbLastName.TabIndex = 7;
            this.tbLastName.Tag = "Фамилия";
            this.tbLastName.Text = "Фамилия";
            this.tbLastName.TextChanged += new System.EventHandler(this.tbField_TextChanged);
            this.tbLastName.Enter += new System.EventHandler(this.tbField_Enter);
            this.tbLastName.Leave += new System.EventHandler(this.tbField_Leave);
            // 
            // tbFirstName
            // 
            this.tbFirstName.ForeColor = System.Drawing.Color.Gray;
            this.tbFirstName.Location = new System.Drawing.Point(370, 52);
            this.tbFirstName.Name = "tbFirstName";
            this.tbFirstName.Size = new System.Drawing.Size(170, 20);
            this.tbFirstName.TabIndex = 8;
            this.tbFirstName.Tag = "Имя";
            this.tbFirstName.Text = "Имя";
            this.tbFirstName.TextChanged += new System.EventHandler(this.tbField_TextChanged);
            this.tbFirstName.Enter += new System.EventHandler(this.tbField_Enter);
            this.tbFirstName.Leave += new System.EventHandler(this.tbField_Leave);
            // 
            // tbMiddleName
            // 
            this.tbMiddleName.ForeColor = System.Drawing.Color.Gray;
            this.tbMiddleName.Location = new System.Drawing.Point(556, 52);
            this.tbMiddleName.Name = "tbMiddleName";
            this.tbMiddleName.Size = new System.Drawing.Size(170, 20);
            this.tbMiddleName.TabIndex = 9;
            this.tbMiddleName.Tag = "Отчество";
            this.tbMiddleName.Text = "Отчество";
            this.tbMiddleName.TextChanged += new System.EventHandler(this.tbField_TextChanged);
            this.tbMiddleName.Enter += new System.EventHandler(this.tbField_Enter);
            this.tbMiddleName.Leave += new System.EventHandler(this.tbField_Leave);
            // 
            // dtpRegDate
            // 
            this.dtpRegDate.Enabled = false;
            this.dtpRegDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRegDate.Location = new System.Drawing.Point(875, 53);
            this.dtpRegDate.Name = "dtpRegDate";
            this.dtpRegDate.Size = new System.Drawing.Size(94, 20);
            this.dtpRegDate.TabIndex = 10;
            this.dtpRegDate.ValueChanged += new System.EventHandler(this.tbField_TextChanged);
            // 
            // tbRegNumber
            // 
            this.tbRegNumber.ForeColor = System.Drawing.Color.Gray;
            this.tbRegNumber.Location = new System.Drawing.Point(67, 52);
            this.tbRegNumber.Name = "tbRegNumber";
            this.tbRegNumber.Size = new System.Drawing.Size(100, 20);
            this.tbRegNumber.TabIndex = 11;
            this.tbRegNumber.Tag = "№ заявления";
            this.tbRegNumber.Text = "№ заявления";
            this.tbRegNumber.TextChanged += new System.EventHandler(this.tbField_TextChanged);
            this.tbRegNumber.Enter += new System.EventHandler(this.tbField_Enter);
            this.tbRegNumber.Leave += new System.EventHandler(this.tbField_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(766, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Дата регистрации:";
            // 
            // cbDateSearch
            // 
            this.cbDateSearch.AutoSize = true;
            this.cbDateSearch.Location = new System.Drawing.Point(750, 55);
            this.cbDateSearch.Name = "cbDateSearch";
            this.cbDateSearch.Size = new System.Drawing.Size(15, 14);
            this.cbDateSearch.TabIndex = 13;
            this.cbDateSearch.UseVisualStyleBackColor = true;
            this.cbDateSearch.CheckedChanged += new System.EventHandler(this.cbDateSearch_CheckedChanged);
            // 
            // cbEnroll
            // 
            this.cbEnroll.AutoSize = true;
            this.cbEnroll.Location = new System.Drawing.Point(601, 29);
            this.cbEnroll.Name = "cbEnroll";
            this.cbEnroll.Size = new System.Drawing.Size(94, 17);
            this.cbEnroll.TabIndex = 5;
            this.cbEnroll.Text = "Зачисленные";
            this.cbEnroll.UseVisualStyleBackColor = true;
            this.cbEnroll.CheckedChanged += new System.EventHandler(this.cbFilter_CheckedChanged);
            // 
            // cbPickUp
            // 
            this.cbPickUp.AutoSize = true;
            this.cbPickUp.Location = new System.Drawing.Point(453, 29);
            this.cbPickUp.Name = "cbPickUp";
            this.cbPickUp.Size = new System.Drawing.Size(142, 17);
            this.cbPickUp.TabIndex = 4;
            this.cbPickUp.Text = "Забравшие документы";
            this.cbPickUp.UseVisualStyleBackColor = true;
            this.cbPickUp.CheckedChanged += new System.EventHandler(this.cbFilter_CheckedChanged);
            // 
            // cbNew
            // 
            this.cbNew.AutoSize = true;
            this.cbNew.Location = new System.Drawing.Point(387, 29);
            this.cbNew.Name = "cbNew";
            this.cbNew.Size = new System.Drawing.Size(60, 17);
            this.cbNew.TabIndex = 3;
            this.cbNew.Text = "Новые";
            this.cbNew.UseVisualStyleBackColor = true;
            this.cbNew.CheckedChanged += new System.EventHandler(this.cbFilter_CheckedChanged);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 461);
            this.Controls.Add(this.cbDateSearch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbRegNumber);
            this.Controls.Add(this.dtpRegDate);
            this.Controls.Add(this.tbMiddleName);
            this.Controls.Add(this.tbFirstName);
            this.Controls.Add(this.tbLastName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbEnroll);
            this.Controls.Add(this.cbPickUp);
            this.Controls.Add(this.cbNew);
            this.Controls.Add(this.dgvApplications);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Main";
            this.Text = "ИС \"Приемная комиссия\"";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplications)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripMain_CreateApplication;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Campaign;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Campaign_Campaigns;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Entrants;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_CreateBacApplication;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Univesity;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Faculties;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Directions;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_TargetOrganizations;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Help;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Dictionaries;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_DirDictionary;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Campaign_Exams;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_OlympDictionary;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Administration;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Users;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_FIS_Export;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_InstitutionAchievements;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Orders;
        private System.Windows.Forms.DataGridView dgvApplications;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Constants;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_OutDocuments;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_RegJournal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbLastName;
        private System.Windows.Forms.TextBox tbFirstName;
        private System.Windows.Forms.TextBox tbMiddleName;
        private System.Windows.Forms.DateTimePicker dtpRegDate;
        private System.Windows.Forms.TextBox tbRegNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbDateSearch;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_MiddleName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvApplications_Original;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_Entrances;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_RegDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_EditDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_PickUpDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_DeductionDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_EnrollmentDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_RegistratorLogin;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_Status;
        private System.Windows.Forms.CheckBox cbEnroll;
        private System.Windows.Forms.CheckBox cbPickUp;
        private System.Windows.Forms.CheckBox cbNew;
        private System.Windows.Forms.ToolStripButton toolStripMain_CreateMagApplication;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_CreateMagApplication;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_KLADR_Update;
    }
}

