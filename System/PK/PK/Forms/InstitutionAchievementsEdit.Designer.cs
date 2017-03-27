namespace PK.Forms
{
    partial class InstitutionAchievementsEdit
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
            this.dgvAchievements = new System.Windows.Forms.DataGridView();
            this.btSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btLoad = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbCampaign = new System.Windows.Forms.ComboBox();
            this.dgvAchievements_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvAchievements_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvAchievements_MaxValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvAchievements_Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvAchievements_CategoryID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAchievements)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAchievements
            // 
            this.dgvAchievements.AllowUserToAddRows = false;
            this.dgvAchievements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAchievements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvAchievements_ID,
            this.dgvAchievements_Name,
            this.dgvAchievements_MaxValue,
            this.dgvAchievements_Category,
            this.dgvAchievements_CategoryID});
            this.dgvAchievements.Location = new System.Drawing.Point(12, 59);
            this.dgvAchievements.Name = "dgvAchievements";
            this.dgvAchievements.RowHeadersVisible = false;
            this.dgvAchievements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAchievements.Size = new System.Drawing.Size(935, 363);
            this.dgvAchievements.TabIndex = 0;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(775, 440);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 1;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Учитываемые достжения отмечаются галочкой";
            // 
            // btLoad
            // 
            this.btLoad.Location = new System.Drawing.Point(84, 440);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(202, 23);
            this.btLoad.TabIndex = 3;
            this.btLoad.Text = "Загрузить отсутствующие из ФИС";
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(405, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Выберите кампанию:";
            // 
            // cbCampaign
            // 
            this.cbCampaign.FormattingEnabled = true;
            this.cbCampaign.Location = new System.Drawing.Point(305, 25);
            this.cbCampaign.Name = "cbCampaign";
            this.cbCampaign.Size = new System.Drawing.Size(338, 21);
            this.cbCampaign.TabIndex = 6;
            this.cbCampaign.SelectedIndexChanged += new System.EventHandler(this.cbCampaign_SelectedIndexChanged);
            // 
            // dgvAchievements_ID
            // 
            this.dgvAchievements_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvAchievements_ID.HeaderText = "ID";
            this.dgvAchievements_ID.Name = "dgvAchievements_ID";
            this.dgvAchievements_ID.ReadOnly = true;
            this.dgvAchievements_ID.Visible = false;
            this.dgvAchievements_ID.Width = 24;
            // 
            // dgvAchievements_Name
            // 
            this.dgvAchievements_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvAchievements_Name.HeaderText = "Наименование";
            this.dgvAchievements_Name.Name = "dgvAchievements_Name";
            // 
            // dgvAchievements_MaxValue
            // 
            this.dgvAchievements_MaxValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvAchievements_MaxValue.HeaderText = "Баллы за достижение";
            this.dgvAchievements_MaxValue.Name = "dgvAchievements_MaxValue";
            this.dgvAchievements_MaxValue.Width = 80;
            // 
            // dgvAchievements_Category
            // 
            this.dgvAchievements_Category.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvAchievements_Category.HeaderText = "Категория";
            this.dgvAchievements_Category.Name = "dgvAchievements_Category";
            this.dgvAchievements_Category.ReadOnly = true;
            // 
            // dgvAchievements_CategoryID
            // 
            this.dgvAchievements_CategoryID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvAchievements_CategoryID.HeaderText = "ID категории";
            this.dgvAchievements_CategoryID.Name = "dgvAchievements_CategoryID";
            this.dgvAchievements_CategoryID.ReadOnly = true;
            this.dgvAchievements_CategoryID.Visible = false;
            this.dgvAchievements_CategoryID.Width = 90;
            // 
            // InstitutionAchievementsEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 475);
            this.Controls.Add(this.cbCampaign);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btLoad);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.dgvAchievements);
            this.Name = "InstitutionAchievementsEdit";
            this.Text = "Индивидуальные достижения";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAchievements)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAchievements;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbCampaign;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvAchievements_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvAchievements_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvAchievements_MaxValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvAchievements_Category;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvAchievements_CategoryID;
    }
}