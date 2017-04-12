namespace PK.Forms
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripMain_CreateApplication = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripMain_cbCurrCampaign = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuStrip_Campaign = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Campaign_Campaigns = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Campaign_Exams = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Orders = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Entrants = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_CreateApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Univesity = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Constants = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Faculties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Directions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_TargetOrganizations = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_InstitutionAchievements = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_Administration = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_Users = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_FisImport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Dictionaries = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_DirDictionary = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_OlympDictionary = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvApplications = new System.Windows.Forms.DataGridView();
            this.dgvApplications_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvApplications_MiddleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip_OutDocuments = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_RegJournal = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplications)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMain_CreateApplication,
            this.toolStripLabel1,
            this.toolStripMain_cbCurrCampaign});
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
            this.toolStripMain_CreateApplication.Size = new System.Drawing.Size(129, 22);
            this.toolStripMain_CreateApplication.Text = "Создать заявление";
            this.toolStripMain_CreateApplication.Click += new System.EventHandler(this.toolStrip_CreateApplication_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(173, 22);
            this.toolStripLabel1.Text = "Текущая приемная кампания:";
            // 
            // toolStripMain_cbCurrCampaign
            // 
            this.toolStripMain_cbCurrCampaign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripMain_cbCurrCampaign.Name = "toolStripMain_cbCurrCampaign";
            this.toolStripMain_cbCurrCampaign.Size = new System.Drawing.Size(121, 25);
            this.toolStripMain_cbCurrCampaign.SelectedIndexChanged += new System.EventHandler(this.toolStripMain_cbCurrCampaign_SelectedIndexChanged);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Campaign,
            this.menuStrip_Entrants,
            this.menuStrip_Univesity,
            this.toolStrip_OutDocuments,
            this.toolStrip_Administration,
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
            this.menuStrip_CreateApplication});
            this.menuStrip_Entrants.Name = "menuStrip_Entrants";
            this.menuStrip_Entrants.Size = new System.Drawing.Size(93, 20);
            this.menuStrip_Entrants.Text = "Абитуриенты";
            // 
            // menuStrip_CreateApplication
            // 
            this.menuStrip_CreateApplication.Name = "menuStrip_CreateApplication";
            this.menuStrip_CreateApplication.Size = new System.Drawing.Size(176, 22);
            this.menuStrip_CreateApplication.Text = "Создать заявление";
            this.menuStrip_CreateApplication.Click += new System.EventHandler(this.menuStrip_CreateApplication_Click);
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
            this.menuStrip_Constants.Size = new System.Drawing.Size(278, 22);
            this.menuStrip_Constants.Text = "Основная информация";
            this.menuStrip_Constants.Click += new System.EventHandler(this.menuStrip_Constants_Click);
            // 
            // menuStrip_Faculties
            // 
            this.menuStrip_Faculties.Name = "menuStrip_Faculties";
            this.menuStrip_Faculties.Size = new System.Drawing.Size(278, 22);
            this.menuStrip_Faculties.Text = "Факультеты";
            this.menuStrip_Faculties.Click += new System.EventHandler(this.menuStrip_Faculties_Click);
            // 
            // menuStrip_Directions
            // 
            this.menuStrip_Directions.Name = "menuStrip_Directions";
            this.menuStrip_Directions.Size = new System.Drawing.Size(278, 22);
            this.menuStrip_Directions.Text = "Направления и профили подготовки";
            this.menuStrip_Directions.Click += new System.EventHandler(this.menuStrip_Directions_Click);
            // 
            // menuStrip_TargetOrganizations
            // 
            this.menuStrip_TargetOrganizations.Name = "menuStrip_TargetOrganizations";
            this.menuStrip_TargetOrganizations.Size = new System.Drawing.Size(278, 22);
            this.menuStrip_TargetOrganizations.Text = "Целевые организации";
            this.menuStrip_TargetOrganizations.Click += new System.EventHandler(this.menuStrip_TargetOrganizations_Click);
            // 
            // menuStrip_InstitutionAchievements
            // 
            this.menuStrip_InstitutionAchievements.Name = "menuStrip_InstitutionAchievements";
            this.menuStrip_InstitutionAchievements.Size = new System.Drawing.Size(278, 22);
            this.menuStrip_InstitutionAchievements.Text = "Индивидуальные достижения";
            this.menuStrip_InstitutionAchievements.Click += new System.EventHandler(this.menuStrip_InstitutionAchievements_Click);
            // 
            // toolStrip_Administration
            // 
            this.toolStrip_Administration.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_Users,
            this.toolStrip_FisImport});
            this.toolStrip_Administration.Name = "toolStrip_Administration";
            this.toolStrip_Administration.Size = new System.Drawing.Size(134, 20);
            this.toolStrip_Administration.Text = "Администрирование";
            // 
            // toolStrip_Users
            // 
            this.toolStrip_Users.Name = "toolStrip_Users";
            this.toolStrip_Users.Size = new System.Drawing.Size(156, 22);
            this.toolStrip_Users.Text = "Пользователи";
            this.toolStrip_Users.Click += new System.EventHandler(this.toolStrip_Users_Click);
            // 
            // toolStrip_FisImport
            // 
            this.toolStrip_FisImport.Name = "toolStrip_FisImport";
            this.toolStrip_FisImport.Size = new System.Drawing.Size(156, 22);
            this.toolStrip_FisImport.Text = "Импорт в ФИС";
            this.toolStrip_FisImport.Click += new System.EventHandler(this.toolStrip_FisImport_Click);
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
            this.dgvApplications.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvApplications.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvApplications_ID,
            this.dgvApplications_LastName,
            this.dgvApplications_FirstName,
            this.dgvApplications_MiddleName});
            this.dgvApplications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvApplications.Location = new System.Drawing.Point(0, 49);
            this.dgvApplications.MultiSelect = false;
            this.dgvApplications.Name = "dgvApplications";
            this.dgvApplications.ReadOnly = true;
            this.dgvApplications.RowHeadersVisible = false;
            this.dgvApplications.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvApplications.Size = new System.Drawing.Size(1008, 412);
            this.dgvApplications.TabIndex = 2;
            this.dgvApplications.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvApplications_CellDoubleClick);
            // 
            // dgvApplications_ID
            // 
            this.dgvApplications_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvApplications_ID.HeaderText = "ID";
            this.dgvApplications_ID.Name = "dgvApplications_ID";
            this.dgvApplications_ID.ReadOnly = true;
            this.dgvApplications_ID.Width = 43;
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
            // toolStrip_OutDocuments
            // 
            this.toolStrip_OutDocuments.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_RegJournal});
            this.toolStrip_OutDocuments.Name = "toolStrip_OutDocuments";
            this.toolStrip_OutDocuments.Size = new System.Drawing.Size(118, 20);
            this.toolStrip_OutDocuments.Text = "Печатные формы";
            // 
            // toolStrip_RegJournal
            // 
            this.toolStrip_RegJournal.Name = "toolStrip_RegJournal";
            this.toolStrip_RegJournal.Size = new System.Drawing.Size(191, 22);
            this.toolStrip_RegJournal.Text = "Журнал регистрации";
            this.toolStrip_RegJournal.Click += new System.EventHandler(this.toolStrip_RegJournal_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 461);
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
        private System.Windows.Forms.ToolStripMenuItem menuStrip_CreateApplication;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Univesity;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Faculties;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Directions;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_TargetOrganizations;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Help;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Dictionaries;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_DirDictionary;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Campaign_Exams;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_OlympDictionary;
        private System.Windows.Forms.ToolStripMenuItem toolStrip_Administration;
        private System.Windows.Forms.ToolStripMenuItem toolStrip_Users;
        private System.Windows.Forms.ToolStripMenuItem toolStrip_FisImport;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_InstitutionAchievements;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Orders;
        private System.Windows.Forms.DataGridView dgvApplications;
        private System.Windows.Forms.ToolStripComboBox toolStripMain_cbCurrCampaign;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvApplications_MiddleName;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Constants;
        private System.Windows.Forms.ToolStripMenuItem toolStrip_OutDocuments;
        private System.Windows.Forms.ToolStripMenuItem toolStrip_RegJournal;
    }
}

