namespace PK
{
    partial class TargetOrganizationsForm
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
            this.cID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOrgName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btNewTargetOrganization = new System.Windows.Forms.Button();
            this.btRename = new System.Windows.Forms.Button();
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
            this.cID,
            this.cOrgName});
            this.dgvTargetOrganizations.Location = new System.Drawing.Point(12, 61);
            this.dgvTargetOrganizations.MultiSelect = false;
            this.dgvTargetOrganizations.Name = "dgvTargetOrganizations";
            this.dgvTargetOrganizations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTargetOrganizations.Size = new System.Drawing.Size(469, 385);
            this.dgvTargetOrganizations.TabIndex = 0;
            // 
            // cID
            // 
            this.cID.HeaderText = "ID";
            this.cID.Name = "cID";
            this.cID.ReadOnly = true;
            this.cID.Visible = false;
            // 
            // cOrgName
            // 
            this.cOrgName.HeaderText = "Название организации";
            this.cOrgName.Name = "cOrgName";
            this.cOrgName.ReadOnly = true;
            // 
            // btNewTargetOrganization
            // 
            this.btNewTargetOrganization.Location = new System.Drawing.Point(63, 22);
            this.btNewTargetOrganization.Name = "btNewTargetOrganization";
            this.btNewTargetOrganization.Size = new System.Drawing.Size(183, 23);
            this.btNewTargetOrganization.TabIndex = 1;
            this.btNewTargetOrganization.Text = "Добавить целевую организацию";
            this.btNewTargetOrganization.UseVisualStyleBackColor = true;
            this.btNewTargetOrganization.Click += new System.EventHandler(this.btNewTargetOrganization_Click);
            // 
            // btRename
            // 
            this.btRename.Location = new System.Drawing.Point(271, 22);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(166, 23);
            this.btRename.TabIndex = 2;
            this.btRename.Text = "Переименовать выделенную";
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // TargetOrganizationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 458);
            this.Controls.Add(this.btRename);
            this.Controls.Add(this.btNewTargetOrganization);
            this.Controls.Add(this.dgvTargetOrganizations);
            this.Name = "TargetOrganizationsForm";
            this.Text = "Целевые организации";
            this.Load += new System.EventHandler(this.TargetOrganizationsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargetOrganizations)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTargetOrganizations;
        private System.Windows.Forms.Button btNewTargetOrganization;
        private System.Windows.Forms.DataGridViewTextBoxColumn cID;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOrgName;
        private System.Windows.Forms.Button btRename;
    }
}