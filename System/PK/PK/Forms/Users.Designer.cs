namespace PK.Forms
{
    partial class Users
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridView_Login = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Password = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Phone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Role = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridView_Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel = new System.Windows.Forms.Panel();
            this.bDelete = new System.Windows.Forms.Button();
            this.bAdd = new System.Windows.Forms.Button();
            this.cbShowPasswords = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_Login,
            this.dataGridView_Password,
            this.dataGridView_Name,
            this.dataGridView_Phone,
            this.dataGridView_Role,
            this.dataGridView_Comment});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(734, 473);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
            // 
            // dataGridView_Login
            // 
            this.dataGridView_Login.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Login.DataPropertyName = "login";
            this.dataGridView_Login.FillWeight = 50F;
            this.dataGridView_Login.HeaderText = "Логин";
            this.dataGridView_Login.Name = "dataGridView_Login";
            this.dataGridView_Login.ReadOnly = true;
            // 
            // dataGridView_Password
            // 
            this.dataGridView_Password.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Password.DataPropertyName = "password";
            this.dataGridView_Password.FillWeight = 40F;
            this.dataGridView_Password.HeaderText = "Пароль";
            this.dataGridView_Password.Name = "dataGridView_Password";
            this.dataGridView_Password.Visible = false;
            // 
            // dataGridView_Name
            // 
            this.dataGridView_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Name.DataPropertyName = "name";
            this.dataGridView_Name.HeaderText = "ФИО";
            this.dataGridView_Name.Name = "dataGridView_Name";
            // 
            // dataGridView_Phone
            // 
            this.dataGridView_Phone.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_Phone.DataPropertyName = "phone_number";
            this.dataGridView_Phone.HeaderText = "Телефон";
            this.dataGridView_Phone.Name = "dataGridView_Phone";
            this.dataGridView_Phone.Width = 90;
            // 
            // dataGridView_Role
            // 
            this.dataGridView_Role.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_Role.DataPropertyName = "role";
            this.dataGridView_Role.HeaderText = "Роль";
            this.dataGridView_Role.Items.AddRange(new object[] {
            "registrator",
            "inspector",
            "administrator"});
            this.dataGridView_Role.Name = "dataGridView_Role";
            this.dataGridView_Role.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Role.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridView_Role.Width = 90;
            // 
            // dataGridView_Comment
            // 
            this.dataGridView_Comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Comment.DataPropertyName = "comment";
            this.dataGridView_Comment.HeaderText = "Комментарий";
            this.dataGridView_Comment.Name = "dataGridView_Comment";
            // 
            // panel
            // 
            this.panel.Controls.Add(this.bDelete);
            this.panel.Controls.Add(this.bAdd);
            this.panel.Controls.Add(this.cbShowPasswords);
            this.panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel.Location = new System.Drawing.Point(0, 473);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(734, 38);
            this.panel.TabIndex = 1;
            // 
            // bDelete
            // 
            this.bDelete.Location = new System.Drawing.Point(93, 6);
            this.bDelete.Name = "bDelete";
            this.bDelete.Size = new System.Drawing.Size(75, 23);
            this.bDelete.TabIndex = 2;
            this.bDelete.Text = "Удалить";
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
            // 
            // bAdd
            // 
            this.bAdd.Location = new System.Drawing.Point(12, 6);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(75, 23);
            this.bAdd.TabIndex = 1;
            this.bAdd.Text = "Добавить";
            this.bAdd.UseVisualStyleBackColor = true;
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // cbShowPasswords
            // 
            this.cbShowPasswords.AutoSize = true;
            this.cbShowPasswords.Location = new System.Drawing.Point(597, 10);
            this.cbShowPasswords.Name = "cbShowPasswords";
            this.cbShowPasswords.Size = new System.Drawing.Size(125, 17);
            this.cbShowPasswords.TabIndex = 0;
            this.cbShowPasswords.Text = "Отобразить пароли";
            this.cbShowPasswords.UseVisualStyleBackColor = true;
            this.cbShowPasswords.CheckedChanged += new System.EventHandler(this.cbShowPasswords_CheckedChanged);
            // 
            // Users
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 511);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.Name = "Users";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Пользователи";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Users_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.CheckBox cbShowPasswords;
        private System.Windows.Forms.Button bAdd;
        private System.Windows.Forms.Button bDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Login;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Password;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Phone;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridView_Role;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Comment;
    }
}