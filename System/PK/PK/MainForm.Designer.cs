﻿namespace PK
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip_CreateApplication = new System.Windows.Forms.ToolStripButton();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuStrip_Campaign = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Campaign_Campaigns = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Entrants = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_CreateApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.университетToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.факультетыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.направленияПодготовкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.целевыеОрганизацииToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Dictionaries = new System.Windows.Forms.ToolStripMenuItem();
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
            this.университетToolStripMenuItem,
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
            this.menuStrip_Campaign_Campaigns});
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
            // университетToolStripMenuItem
            // 
            this.университетToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.факультетыToolStripMenuItem,
            this.направленияПодготовкиToolStripMenuItem,
            this.целевыеОрганизацииToolStripMenuItem});
            this.университетToolStripMenuItem.Name = "университетToolStripMenuItem";
            this.университетToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.университетToolStripMenuItem.Text = "Университет";
            // 
            // факультетыToolStripMenuItem
            // 
            this.факультетыToolStripMenuItem.Name = "факультетыToolStripMenuItem";
            this.факультетыToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.факультетыToolStripMenuItem.Text = "Факультеты";
            // 
            // направленияПодготовкиToolStripMenuItem
            // 
            this.направленияПодготовкиToolStripMenuItem.Name = "направленияПодготовкиToolStripMenuItem";
            this.направленияПодготовкиToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.направленияПодготовкиToolStripMenuItem.Text = "Направления подготовки";
            // 
            // целевыеОрганизацииToolStripMenuItem
            // 
            this.целевыеОрганизацииToolStripMenuItem.Name = "целевыеОрганизацииToolStripMenuItem";
            this.целевыеОрганизацииToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.целевыеОрганизацииToolStripMenuItem.Text = "Целевые организации";
            this.целевыеОрганизацииToolStripMenuItem.Click += new System.EventHandler(this.целевыеОрганизацииToolStripMenuItem_Click);
            // menuStrip_Help
            // 
            this.menuStrip_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStrip_Dictionaries});
            this.menuStrip_Help.Name = "menuStrip_Help";
            this.menuStrip_Help.Size = new System.Drawing.Size(65, 20);
            this.menuStrip_Help.Text = "Справка";
            // 
            // menuStrip_Dictionaries
            // 
            this.menuStrip_Dictionaries.Name = "menuStrip_Dictionaries";
            this.menuStrip_Dictionaries.Size = new System.Drawing.Size(178, 22);
            this.menuStrip_Dictionaries.Text = "Справочники ФИС";
            this.menuStrip_Dictionaries.Click += new System.EventHandler(this.menuStrip_Dictionaries_Click);
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
        private System.Windows.Forms.ToolStripMenuItem университетToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem факультетыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem направленияПодготовкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem целевыеОрганизацииToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Help;
        private System.Windows.Forms.ToolStripMenuItem menuStrip_Dictionaries;
    }
}
