namespace PK.Forms
{
    partial class CampaignsForm
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
            this.dgvPriemComp = new System.Windows.Forms.DataGridView();
            this.btCreatePriemComp = new System.Windows.Forms.Button();
            this.cID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTiming = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cEduLevels = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriemComp)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPriemComp
            // 
            this.dgvPriemComp.AllowUserToAddRows = false;
            this.dgvPriemComp.AllowUserToDeleteRows = false;
            this.dgvPriemComp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPriemComp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cID,
            this.cName,
            this.cTiming,
            this.cEduLevels,
            this.cStatus});
            this.dgvPriemComp.Location = new System.Drawing.Point(12, 55);
            this.dgvPriemComp.MultiSelect = false;
            this.dgvPriemComp.Name = "dgvPriemComp";
            this.dgvPriemComp.ReadOnly = true;
            this.dgvPriemComp.RowHeadersVisible = false;
            this.dgvPriemComp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPriemComp.Size = new System.Drawing.Size(715, 382);
            this.dgvPriemComp.TabIndex = 0;
            // 
            // btCreatePriemComp
            // 
            this.btCreatePriemComp.Location = new System.Drawing.Point(149, 12);
            this.btCreatePriemComp.Name = "btCreatePriemComp";
            this.btCreatePriemComp.Size = new System.Drawing.Size(171, 23);
            this.btCreatePriemComp.TabIndex = 1;
            this.btCreatePriemComp.Text = "Создать приемную кампанию";
            this.btCreatePriemComp.UseVisualStyleBackColor = true;
            this.btCreatePriemComp.Click += new System.EventHandler(this.btCreatePriemComp_Click);
            // 
            // cID
            // 
            this.cID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cID.HeaderText = "ID";
            this.cID.Name = "cID";
            this.cID.ReadOnly = true;
            this.cID.Visible = false;
            this.cID.Width = 24;
            // 
            // cName
            // 
            this.cName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cName.HeaderText = "Название";
            this.cName.Name = "cName";
            this.cName.ReadOnly = true;
            // 
            // cTiming
            // 
            this.cTiming.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.cTiming.HeaderText = "Сроки проведения";
            this.cTiming.Name = "cTiming";
            this.cTiming.ReadOnly = true;
            this.cTiming.Width = 115;
            // 
            // cEduLevels
            // 
            this.cEduLevels.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cEduLevels.HeaderText = "Уровни образования";
            this.cEduLevels.Name = "cEduLevels";
            this.cEduLevels.ReadOnly = true;
            this.cEduLevels.Width = 127;
            // 
            // cStatus
            // 
            this.cStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cStatus.HeaderText = "Статус";
            this.cStatus.Name = "cStatus";
            this.cStatus.ReadOnly = true;
            this.cStatus.Width = 66;
            // 
            // btUpdate
            // 
            this.btUpdate.Location = new System.Drawing.Point(450, 12);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(142, 23);
            this.btUpdate.TabIndex = 2;
            this.btUpdate.Text = "Изменить выделенную";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
            // 
            // CampaignsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 449);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.btCreatePriemComp);
            this.Controls.Add(this.dgvPriemComp);
            this.Name = "CampaignsForm";
            this.Text = "Приемные кампании";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriemComp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPriemComp;
        private System.Windows.Forms.Button btCreatePriemComp;
        private System.Windows.Forms.DataGridViewTextBoxColumn cID;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTiming;
        private System.Windows.Forms.DataGridViewTextBoxColumn cEduLevels;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStatus;
        private System.Windows.Forms.Button btUpdate;
    }
}