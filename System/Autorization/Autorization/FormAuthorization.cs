using System;
using System.Data;
using System.Windows.Forms;

namespace Autorization
{
    public partial class FormAuthorization : Form
    {
        FormMain parent;
        bool exitingApp = true;

        public FormAuthorization(FormMain p)
        {
         parent = p;
         InitializeComponent();
        }

        private void btAuth_Click(object sender, EventArgs e)
        {
            exitingApp = false;
            for (int i = 0; i < usersBindingSource.Count; ++i)
            {
                pk_dbDataSet.usersRow row = (pk_dbDataSet.usersRow)(((DataRowView)usersBindingSource[i]).Row);
                
                if (cbLogin.Text == row.user_login)
                {
                    if (tbPassword.Text == row.user_password)
                    {
                        MessageBox.Show("Авторизация выполнена. Роль пользователя " + row.user_role + ".");
                        switch (row.user_role)
                        {
                            case "registrator":
                                this.parent.SetUsersRole(0);
                                break;
                            case "inspector":
                                this.parent.SetUsersRole(1);
                                break;
                            case "administrator":
                                this.parent.SetUsersRole(2);
                                break;                            
                        }
                        this.parent.Show();
                        this.Close();
                    }
                        
                    else
                        MessageBox.Show("Неверный пароль.");
                    return;
                }
            }
            MessageBox.Show("Логин не найден.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            usersTableAdapter1.Fill(this.pk_dbDataSet.users);
            cbLogin.DisplayMember = "user_login";
        }

        private void FormAuthorization_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (exitingApp) Application.Exit();
        }
    }
}
