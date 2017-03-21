namespace PK.Forms
{
    partial class Authorization
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lPassword = new System.Windows.Forms.Label();
            this.lLogin = new System.Windows.Forms.Label();
            this.cbLogin = new System.Windows.Forms.ComboBox();
            this.bAuth = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(69, 88);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(148, 20);
            this.tbPassword.TabIndex = 3;
            // 
            // lPassword
            // 
            this.lPassword.AutoSize = true;
            this.lPassword.Location = new System.Drawing.Point(99, 72);
            this.lPassword.Name = "lPassword";
            this.lPassword.Size = new System.Drawing.Size(91, 13);
            this.lPassword.TabIndex = 2;
            this.lPassword.Text = "Введите пароль:";
            // 
            // lLogin
            // 
            this.lLogin.AutoSize = true;
            this.lLogin.Location = new System.Drawing.Point(98, 29);
            this.lLogin.Name = "lLogin";
            this.lLogin.Size = new System.Drawing.Size(92, 13);
            this.lLogin.TabIndex = 0;
            this.lLogin.Text = "Выберите логин:";
            // 
            // cbLogin
            // 
            this.cbLogin.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbLogin.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbLogin.FormattingEnabled = true;
            this.cbLogin.Location = new System.Drawing.Point(69, 45);
            this.cbLogin.Name = "cbLogin";
            this.cbLogin.Size = new System.Drawing.Size(148, 21);
            this.cbLogin.TabIndex = 1;
            // 
            // bAuth
            // 
            this.bAuth.Location = new System.Drawing.Point(102, 125);
            this.bAuth.Name = "bAuth";
            this.bAuth.Size = new System.Drawing.Size(75, 23);
            this.bAuth.TabIndex = 4;
            this.bAuth.Text = "Войти";
            this.bAuth.UseVisualStyleBackColor = true;
            this.bAuth.Click += new System.EventHandler(this.bAuth_Click);
            // 
            // Authorization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 175);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.lPassword);
            this.Controls.Add(this.lLogin);
            this.Controls.Add(this.cbLogin);
            this.Controls.Add(this.bAuth);
            this.Name = "Authorization";
            this.Text = "Авторизация";
            this.Load += new System.EventHandler(this.Authorization_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lPassword;
        private System.Windows.Forms.Label lLogin;
        private System.Windows.Forms.ComboBox cbLogin;
        private System.Windows.Forms.Button bAuth;
    }
}