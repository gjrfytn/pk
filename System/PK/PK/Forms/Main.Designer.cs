﻿namespace PK.Forms
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
            this.toolStrip_CreateApplication = new System.Windows.Forms.ToolStripButton();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuStrip_Campaign = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Campaign_Campaigns = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Campaign_Exams = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Entrants = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_CreateApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Univesity = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Faculties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Directions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_TargetOrganizations = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_Administration = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_Users = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Dictionaries = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_DirDictionary = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_OlympDictionary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_FisImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_CreateApplication});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(682, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStrip_CreateApplication
            // 
            this.toolStrip_CreateApplication.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_CreateApplication.Image")));
            this.toolStrip_CreateApplication.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_CreateApplication.Name = "toolStrip_CreateApplication";
            this.toolStrip_CreateApplication.Size = new System.Drawing.Size(129, 22);
            this.toolStrip_CreateApplication.Text = "Создать заявление";
            this.toolStrip_CreateApplication.Click += new System.EventHandler(this.toolStrip_CreateApplication_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Campaign,
            this.menuStrip_Entrants,
            this.menuStrip_Univesity,
            this.toolStrip_Administration,
            this.menuStrip_Help});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(682, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuStrip_Campaign
            // 
            this.menuStrip_Campaign.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Campaign_Campaigns,
            this.menuStrip_Campaign_Exams});
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
            this.menuStrip_Faculties,
            this.menuStrip_Directions,
            this.menuStrip_TargetOrganizations});
            this.menuStrip_Univesity.Name = "menuStrip_Univesity";
            this.menuStrip_Univesity.Size = new System.Drawing.Size(88, 20);
            this.menuStrip_Univesity.Text = "Университет";
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
            // toolStrip_FisImport
            // 
            this.toolStrip_FisImport.Name = "toolStrip_FisImport";
            this.toolStrip_FisImport.Size = new System.Drawing.Size(156, 22);
            this.toolStrip_FisImport.Text = "Импорт в ФИС";
            this.toolStrip_FisImport.Click += new System.EventHandler(this.toolStrip_FisImport_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 461);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "ИС \"Приемная комиссия\"";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStrip_CreateApplication;
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
    }
}
