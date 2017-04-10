using System;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class Authorization : Form
    {
        public string UsersRole { get; private set; }
        public string UsersLogin { get; private set; }

        private readonly Classes.DB_Connector _DB_Connection;

        public Authorization()
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector("default", "");
            foreach (var v in _DB_Connection.Select(DB_Table.USERS, "login"))
                cbLogin.Items.Add(v[0]);
        }

        #region IDisposable Support
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _DB_Connection.Dispose();

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        private void bAuth_Click(object sender, EventArgs e)
        {
            object[] user = _DB_Connection.Select(DB_Table.USERS, "login", "password", "role").Find(x => x[0].ToString() == cbLogin.Text);

            if (user == null)
                MessageBox.Show("Логин не найден");
            else if (user[1].ToString() == tbPassword.Text)
            {
                UsersRole = user[2].ToString();
                UsersLogin = cbLogin.Text;
                DialogResult = DialogResult.OK;
            }
            else
                MessageBox.Show("Неверный пароль");
        }

        //private void Authorization_Load(object sender, EventArgs e)
        //{
        //    DialogResult = DialogResult.OK;
        //}
    }
}
