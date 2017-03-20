namespace PK.Forms
{
    partial class UsersForm
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
            this.panel = new System.Windows.Forms.Panel();
            this.bAdd = new System.Windows.Forms.Button();
            this.cbShowPasswords = new System.Windows.Forms.CheckBox();
            this.dataGridView_Login = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Password = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Phone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Role = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridView_Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
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
            this.dataGridView.Size = new System.Drawing.Size(672, 388);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // panel
            // 
            this.panel.Controls.Add(this.bAdd);
            this.panel.Controls.Add(this.cbShowPasswords);
            this.panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel.Location = new System.Drawing.Point(0, 388);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(672, 48);
            this.panel.TabIndex = 1;
            // 
            // bAdd
            // 
            this.bAdd.Location = new System.Drawing.Point(12, 6);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(75, 23);
            this.bAdd.TabIndex = 1;
            this.bAdd.Text = "Добавить";
            this.bAdd.UseVisualStyleBackColor = true;
            // 
            // cbShowPasswords
            // 
            this.cbShowPasswords.AutoSize = true;
            this.cbShowPasswords.Location = new System.Drawing.Point(535, 10);
            this.cbShowPasswords.Name = "cbShowPasswords";
            this.cbShowPasswords.Size = new System.Drawing.Size(125, 17);
            this.cbShowPasswords.TabIndex = 0;
            this.cbShowPasswords.Text = "Отобразить пароли";
            this.cbShowPasswords.UseVisualStyleBackColor = true;
            this.cbShowPasswords.CheckedChanged += new System.EventHandler(this.cbShowPasswords_CheckedChanged);
            // 
            // dataGridView_Login
            // 
            this.dataGridView_Login.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Login.DataPropertyName = "login";
            this.dataGridView_Login.FillWeight = 65F;
            this.dataGridView_Login.HeaderText = "Логин";
            this.dataGridView_Login.Name = "dataGridView_Login";
            this.dataGridView_Login.ReadOnly = true;
            // 
            // dataGridView_Password
            // 
            this.dataGridView_Password.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Password.DataPropertyName = "password";
            this.dataGridView_Password.FillWeight = 75F;
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
            this.dataGridView_Phone.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Phone.DataPropertyName = "phone_number";
            this.dataGridView_Phone.FillWeight = 50F;
            this.dataGridView_Phone.HeaderText = "Телефон";
            this.dataGridView_Phone.Name = "dataGridView_Phone";
            // 
            // dataGridView_Role
            // 
            this.dataGridView_Role.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Role.DataPropertyName = "role";
            this.dataGridView_Role.FillWeight = 55F;
            this.dataGridView_Role.HeaderText = "Роль";
            this.dataGridView_Role.Items.AddRange(new object[] {
            "registrator",
            "inspector",
            "administrator"});
            this.dataGridView_Role.Name = "dataGridView_Role";
            this.dataGridView_Role.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Role.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridView_Comment
            // 
            this.dataGridView_Comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Comment.DataPropertyName = "comment";
            this.dataGridView_Comment.HeaderText = "Комментарий";
            this.dataGridView_Comment.Name = "dataGridView_Comment";
            // 
            // UsersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 436);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel);
            this.Name = "UsersForm";
            this.Text = "Пользователи";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Login;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Password;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Phone;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridView_Role;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Comment;
    }
}