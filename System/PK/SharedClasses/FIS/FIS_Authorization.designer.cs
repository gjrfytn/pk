namespace SharedClasses.FIS
{
    partial class FIS_Authorization
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
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lPassword = new System.Windows.Forms.Label();
            this.lLogin = new System.Windows.Forms.Label();
            this.bAuth = new System.Windows.Forms.Button();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(12, 68);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(148, 20);
            this.tbPassword.TabIndex = 3;
            // 
            // lPassword
            // 
            this.lPassword.AutoSize = true;
            this.lPassword.Location = new System.Drawing.Point(12, 52);
            this.lPassword.Name = "lPassword";
            this.lPassword.Size = new System.Drawing.Size(48, 13);
            this.lPassword.TabIndex = 2;
            this.lPassword.Text = "Пароль:";
            // 
            // lLogin
            // 
            this.lLogin.AutoSize = true;
            this.lLogin.Location = new System.Drawing.Point(12, 9);
            this.lLogin.Name = "lLogin";
            this.lLogin.Size = new System.Drawing.Size(41, 13);
            this.lLogin.TabIndex = 0;
            this.lLogin.Text = "Логин:";
            // 
            // bAuth
            // 
            this.bAuth.Location = new System.Drawing.Point(46, 94);
            this.bAuth.Name = "bAuth";
            this.bAuth.Size = new System.Drawing.Size(75, 23);
            this.bAuth.TabIndex = 4;
            this.bAuth.Text = "Войти";
            this.bAuth.UseVisualStyleBackColor = true;
            this.bAuth.Click += new System.EventHandler(this.bAuth_Click);
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(12, 25);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(148, 20);
            this.tbLogin.TabIndex = 1;
            // 
            // FIS_Authorization
            // 
            this.AcceptButton = this.bAuth;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(172, 125);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.lPassword);
            this.Controls.Add(this.lLogin);
            this.Controls.Add(this.bAuth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::SharedClasses.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FIS_Authorization";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Авторизация ФИС";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lPassword;
        private System.Windows.Forms.Label lLogin;
        private System.Windows.Forms.Button bAuth;
        public System.Windows.Forms.TextBox tbPassword;
        public System.Windows.Forms.TextBox tbLogin;
    }
}