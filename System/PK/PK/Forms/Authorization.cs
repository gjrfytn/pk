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

            while (true)
            {
                try
                {
                    _DB_Connection = new Classes.DB_Connector("default", "");
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    if (ex.Number == 1042 && !Classes.Utility.ShowChoiceMessageBox("Подключён ли кабель локальной сети к компьютеру?", "Ошибка подключения"))
                    {
                        MessageBox.Show("Выполните подключение.", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    MessageBox.Show("Обратитесь к администратору. Не закрывайте это сообщение.", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show("Информация об ошибке:\n" + ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (Classes.Utility.ShowChoiceMessageBox("Закрыть приложение?","Действие"))
                    {
                        Load += (s, e) => DialogResult = DialogResult.Abort;
                        break;
                    }

                    continue;
                }

                foreach (var v in _DB_Connection.Select(DB_Table.USERS, "login"))
                    cbLogin.Items.Add(v[0]);
                break;
            }
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
                MessageBox.Show("Логин не найден.","Ошибка авторизации", MessageBoxButtons.OK,MessageBoxIcon.Error);
            else if (user[1].ToString() == tbPassword.Text)
            {
                UsersRole = user[2].ToString();
                UsersLogin = cbLogin.Text;
                DialogResult = DialogResult.OK;
            }
            else
                MessageBox.Show("Неверный пароль", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //private void Authorization_Load(object sender, EventArgs e)
        //{
        //    DialogResult = DialogResult.OK;
        //}
    }
}
