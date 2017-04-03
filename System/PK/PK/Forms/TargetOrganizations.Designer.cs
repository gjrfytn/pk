namespace PK.Forms
{
    partial class TargetOrganizations
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
            this.dgvTargetOrganizations = new System.Windows.Forms.DataGridView();
            this.dgvTargetOrganizations_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvTargetOrganizations_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btNewTargetOrganization = new System.Windows.Forms.Button();
            this.btRename = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargetOrganizations)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTargetOrganizations
            // 
            this.dgvTargetOrganizations.AllowUserToAddRows = false;
            this.dgvTargetOrganizations.AllowUserToDeleteRows = false;
            this.dgvTargetOrganizations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTargetOrganizations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTargetOrganizations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvTargetOrganizations_ID,
            this.dgvTargetOrganizations_Name});
            this.dgvTargetOrganizations.Location = new System.Drawing.Point(12, 61);
            this.dgvTargetOrganizations.MultiSelect = false;
            this.dgvTargetOrganizations.Name = "dgvTargetOrganizations";
            this.dgvTargetOrganizations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTargetOrganizations.Size = new System.Drawing.Size(469, 385);
            this.dgvTargetOrganizations.TabIndex = 0;
            // 
            // dgvTargetOrganizations_ID
            // 
            this.dgvTargetOrganizations_ID.HeaderText = "ID";
            this.dgvTargetOrganizations_ID.Name = "dgvTargetOrganizations_ID";
            this.dgvTargetOrganizations_ID.ReadOnly = true;
            this.dgvTargetOrganizations_ID.Visible = false;
            // 
            // dgvTargetOrganizations_Name
            // 
            this.dgvTargetOrganizations_Name.HeaderText = "Название организации";
            this.dgvTargetOrganizations_Name.Name = "dgvTargetOrganizations_Name";
            this.dgvTargetOrganizations_Name.ReadOnly = true;
            // 
            // btNewTargetOrganization
            // 
            this.btNewTargetOrganization.Location = new System.Drawing.Point(12, 22);
            this.btNewTargetOrganization.Name = "btNewTargetOrganization";
            this.btNewTargetOrganization.Size = new System.Drawing.Size(183, 23);
            this.btNewTargetOrganization.TabIndex = 1;
            this.btNewTargetOrganization.Text = "Добавить целевую организацию";
            this.btNewTargetOrganization.UseVisualStyleBackColor = true;
            this.btNewTargetOrganization.Click += new System.EventHandler(this.btNewTargetOrganization_Click);
            // 
            // btRename
            // 
            this.btRename.Location = new System.Drawing.Point(219, 22);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(166, 23);
            this.btRename.TabIndex = 2;
            this.btRename.Text = "Переименовать выделенную";
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // btDelete
            // 
            this.btDelete.Location = new System.Drawing.Point(406, 22);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(75, 23);
            this.btDelete.TabIndex = 3;
            this.btDelete.Text = "Удалить";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // TargetOrganizations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 458);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.btRename);
            this.Controls.Add(this.btNewTargetOrganization);
            this.Controls.Add(this.dgvTargetOrganizations);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TargetOrganizations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Целевые организации";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargetOrganizations)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTargetOrganizations;
        private System.Windows.Forms.Button btNewTargetOrganization;
        private System.Windows.Forms.Button btRename;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvTargetOrganizations_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvTargetOrganizations_Name;
    }
}