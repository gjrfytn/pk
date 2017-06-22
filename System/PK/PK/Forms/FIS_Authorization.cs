using System;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class FIS_Authorization : Form
    {
        public interface ILoginSetting
        {
            string Value { get; set; }
            void Save();
        }

        private ILoginSetting _Login;

        public FIS_Authorization(ILoginSetting login)
        {
            InitializeComponent();

            _Login = login;
            tbLogin.Text = _Login.Value;

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

            _Login.Value = tbLogin.Text;
            _Login.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
