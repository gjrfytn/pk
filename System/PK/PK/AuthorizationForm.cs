using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK
{
    public partial class AuthorizationForm : Form
    {
        DB_Connector _DB_Connection;
        public byte UsersRole { get; private set; }

        public AuthorizationForm()
        {
            InitializeComponent();

            _DB_Connection = new DB_Connector();
            foreach (var v in _DB_Connection.Select(DB_Table.USERS, "login"))
                cbLogin.Items.Add(v[0]);
        }

        private void bAuth_Click(object sender, EventArgs e)
        {
            List<object[]> usersdata = _DB_Connection.Select(DB_Table.USERS, "login", "password");
            object[] logpass = usersdata.Find(x => x[0].ToString() == cbLogin.Text);

            if (logpass == null)
                MessageBox.Show("Логин не найден");
            else if (logpass[1].ToString() == tbPassword.Text)
            {
                // UsersRole = <?>;
                DialogResult = DialogResult.OK;
            }
            else
                MessageBox.Show("Неверный пароль");
        }
    }
}
