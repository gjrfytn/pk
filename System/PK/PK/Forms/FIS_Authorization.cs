using System;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class FIS_Authorization : Form
    {
        public FIS_Authorization()
        {
            InitializeComponent();

            tbLogin.Text = Properties.Settings.Default.FIS_Login;

            if (tbLogin.Text != "")
                tbPassword.Select();
        }

        private void bAuth_Click(object sender, EventArgs e)
        {
            if (tbLogin.Text == "" || tbPassword.Text == "")
            {
                MessageBox.Show("Поля не могут быть пустыми.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Properties.Settings.Default.FIS_Login = tbLogin.Text;
            Properties.Settings.Default.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
