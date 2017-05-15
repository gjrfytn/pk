namespace PK.Forms
{
    partial class OlympicsDictionary
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
            this.dgvOlympics = new System.Windows.Forms.DataGridView();
            this.dgvOlympics_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOlympics_Year = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOlympics_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOlympics_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbProfiles = new System.Windows.Forms.GroupBox();
            this.dgvProfiles = new System.Windows.Forms.DataGridView();
            this.dgvProfiles_Dict_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvProfiles_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvProfiles_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvProfiles_Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbSubjects = new System.Windows.Forms.GroupBox();
            this.lbSubjects = new System.Windows.Forms.ListBox();
            this.panel = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip_Update = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOlympics)).BeginInit();
            this.gbProfiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).BeginInit();
            this.gbSubjects.SuspendLayout();
            this.panel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvOlympics
            // 
            this.dgvOlympics.AllowUserToAddRows = false;
            this.dgvOlympics.AllowUserToDeleteRows = false;
            this.dgvOlympics.AllowUserToResizeColumns = false;
            this.dgvOlympics.AllowUserToResizeRows = false;
            this.dgvOlympics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOlympics.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvOlympics_ID,
            this.dgvOlympics_Year,
            this.dgvOlympics_Number,
            this.dgvOlympics_Name});
            this.dgvOlympics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOlympics.Location = new System.Drawing.Point(0, 25);
            this.dgvOlympics.MultiSelect = false;
            this.dgvOlympics.Name = "dgvOlympics";
            this.dgvOlympics.ReadOnly = true;
            this.dgvOlympics.RowHeadersVisible = false;
            this.dgvOlympics.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOlympics.Size = new System.Drawing.Size(674, 454);
            this.dgvOlympics.TabIndex = 0;
            this.dgvOlympics.SelectionChanged += new System.EventHandler(this.dgvOlympics_SelectionChanged);
            // 
            // dgvOlympics_ID
            // 
            this.dgvOlympics_ID.HeaderText = "ID";
            this.dgvOlympics_ID.Name = "dgvOlympics_ID";
            this.dgvOlympics_ID.ReadOnly = true;
            this.dgvOlympics_ID.Width = 40;
            // 
            // dgvOlympics_Year
            // 
            this.dgvOlympics_Year.HeaderText = "Год";
            this.dgvOlympics_Year.Name = "dgvOlympics_Year";
            this.dgvOlympics_Year.ReadOnly = true;
            this.dgvOlympics_Year.Width = 40;
            // 
            // dgvOlympics_Number
            // 
            this.dgvOlympics_Number.FillWeight = 10F;
            this.dgvOlympics_Number.HeaderText = "Номер";
            this.dgvOlympics_Number.Name = "dgvOlympics_Number";
            this.dgvOlympics_Number.ReadOnly = true;
            this.dgvOlympics_Number.Width = 50;
            // 
            // dgvOlympics_Name
            // 
            this.dgvOlympics_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOlympics_Name.HeaderText = "Имя";
            this.dgvOlympics_Name.MinimumWidth = 100;
            this.dgvOlympics_Name.Name = "dgvOlympics_Name";
            this.dgvOlympics_Name.ReadOnly = true;
            // 
            // gbProfiles
            // 
            this.gbProfiles.Controls.Add(this.dgvProfiles);
            this.gbProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbProfiles.Location = new System.Drawing.Point(0, 0);
            this.gbProfiles.Name = "gbProfiles";
            this.gbProfiles.Size = new System.Drawing.Size(210, 366);
            this.gbProfiles.TabIndex = 1;
            this.gbProfiles.TabStop = false;
            this.gbProfiles.Text = "Профили олимпиады";
            // 
            // dgvProfiles
            // 
            this.dgvProfiles.AllowUserToAddRows = false;
            this.dgvProfiles.AllowUserToDeleteRows = false;
            this.dgvProfiles.AllowUserToResizeColumns = false;
            this.dgvProfiles.AllowUserToResizeRows = false;
            this.dgvProfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProfiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvProfiles_Dict_ID,
            this.dgvProfiles_ID,
            this.dgvProfiles_Name,
            this.dgvProfiles_Level});
            this.dgvProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProfiles.Location = new System.Drawing.Point(3, 16);
            this.dgvProfiles.MultiSelect = false;
            this.dgvProfiles.Name = "dgvProfiles";
            this.dgvProfiles.ReadOnly = true;
            this.dgvProfiles.RowHeadersVisible = false;
            this.dgvProfiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProfiles.Size = new System.Drawing.Size(204, 347);
            this.dgvProfiles.TabIndex = 0;
            this.dgvProfiles.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProfiles_RowEnter);
            // 
            // dgvProfiles_Dict_ID
            // 
            this.dgvProfiles_Dict_ID.HeaderText = "Dict_ID";
            this.dgvProfiles_Dict_ID.Name = "dgvProfiles_Dict_ID";
            this.dgvProfiles_Dict_ID.ReadOnly = true;
            this.dgvProfiles_Dict_ID.Visible = false;
            // 
            // dgvProfiles_ID
            // 
            this.dgvProfiles_ID.HeaderText = "ID";
            this.dgvProfiles_ID.Name = "dgvProfiles_ID";
            this.dgvProfiles_ID.ReadOnly = true;
            this.dgvProfiles_ID.Visible = false;
            // 
            // dgvProfiles_Name
            // 
            this.dgvProfiles_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvProfiles_Name.HeaderText = "Наименование";
            this.dgvProfiles_Name.MinimumWidth = 100;
            this.dgvProfiles_Name.Name = "dgvProfiles_Name";
            this.dgvProfiles_Name.ReadOnly = true;
            // 
            // dgvProfiles_Level
            // 
            this.dgvProfiles_Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvProfiles_Level.FillWeight = 50F;
            this.dgvProfiles_Level.HeaderText = "Уровень";
            this.dgvProfiles_Level.MinimumWidth = 50;
            this.dgvProfiles_Level.Name = "dgvProfiles_Level";
            this.dgvProfiles_Level.ReadOnly = true;
            // 
            // gbSubjects
            // 
            this.gbSubjects.Controls.Add(this.lbSubjects);
            this.gbSubjects.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbSubjects.Location = new System.Drawing.Point(0, 366);
            this.gbSubjects.Name = "gbSubjects";
            this.gbSubjects.Size = new System.Drawing.Size(210, 88);
            this.gbSubjects.TabIndex = 2;
            this.gbSubjects.TabStop = false;
            this.gbSubjects.Text = "Дисциплины профиля";
            // 
            // lbSubjects
            // 
            this.lbSubjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSubjects.FormattingEnabled = true;
            this.lbSubjects.Location = new System.Drawing.Point(3, 16);
            this.lbSubjects.Name = "lbSubjects";
            this.lbSubjects.Size = new System.Drawing.Size(204, 69);
            this.lbSubjects.TabIndex = 0;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.gbProfiles);
            this.panel.Controls.Add(this.gbSubjects);
            this.panel.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel.Location = new System.Drawing.Point(674, 25);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(210, 454);
            this.panel.TabIndex = 3;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_Update});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(884, 25);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStrip_Update
            // 
            this.toolStrip_Update.Image = global::PK.Properties.Resources.refresh;
            this.toolStrip_Update.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Update.Name = "toolStrip_Update";
            this.toolStrip_Update.Size = new System.Drawing.Size(81, 22);
            this.toolStrip_Update.Text = "Обновить";
            this.toolStrip_Update.Click += new System.EventHandler(this.toolStrip_Update_Click);
            // 
            // OlympicsDictionary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 479);
            this.Controls.Add(this.dgvOlympics);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.toolStrip);
            this.Name = "OlympicsDictionary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Олимпиады (справочник №19 ФИС)";
            ((System.ComponentModel.ISupportInitialize)(this.dgvOlympics)).EndInit();
            this.gbProfiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).EndInit();
            this.gbSubjects.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOlympics;
        private System.Windows.Forms.GroupBox gbProfiles;
        private System.Windows.Forms.DataGridView dgvProfiles;
        private System.Windows.Forms.GroupBox gbSubjects;
        private System.Windows.Forms.ListBox lbSubjects;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStrip_Update;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvProfiles_Dict_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvProfiles_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvProfiles_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvProfiles_Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOlympics_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOlympics_Year;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOlympics_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOlympics_Name;
    }
}