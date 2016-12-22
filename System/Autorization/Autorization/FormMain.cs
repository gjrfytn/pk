using System;
using System.Data;
using System.Windows.Forms;

namespace Autorization
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        int usersRole = 0;
        bool justLoaded = true;

        public void SetUsersRole (int role)
        {
            usersRole = role;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {            
            FormAuthorization authForm = new FormAuthorization(this);
            authForm.Show();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (justLoaded) this.Hide();
        }

        private void FormMain_VisibleChanged(object sender, EventArgs e)
        {
            tabControl1.SelectTab(usersRole);
        }
    }
}
