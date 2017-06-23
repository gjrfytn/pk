using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PK.Forms
{
    public partial class Authorization : Form
    {
        public string UsersRole { get; private set; }
        public string UsersLogin { get; private set; }

        private readonly Classes.DB_Connector _DB_Connection;

        public Authorization()
        {
            #region Components
            InitializeComponent();

            cbLogin.Text = Properties.Settings.Default.Login;

            if (cbLogin.Text != "")
                tbPassword.Select();
            #endregion

            while (true)
            {
                try
                {
                    _DB_Connection = new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, "initial", "1234");
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    if (ex.Number == 1042 && !Classes.Utility.ShowChoiceMessageBox("Подключён ли кабель локальной сети к компьютеру?", "Ошибка подключения"))
                    {
                        MessageBox.Show("Выполните подключение.", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue; //TODO
                    }

                    MessageBox.Show("Обратитесь к администратору. Не закрывайте это сообщение.", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show("Информация об ошибке:\n" + ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (Classes.Utility.ShowChoiceMessageBox("Закрыть приложение?", "Действие"))
                    {
                        Load += (s, e) => DialogResult = DialogResult.Abort;
                        break;
                    }

                    continue;
                }

                foreach (var v in _DB_Connection.Select(DB_Table.USERS, "login"))
                    cbLogin.Items.Add(v[0]);

                AssureConstants();

                break;
            }
        }

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _DB_Connection.Dispose();

                    if (components != null)
                        components.Dispose();
                }

                base.Dispose(disposing);
            }
        }
        #endregion

        private void bAuth_Click(object sender, EventArgs e)
        {
            if (cbLogin.Text == "" || tbPassword.Text == "")
            {
                MessageBox.Show("Поля не могут быть пустыми.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<object[]> results = _DB_Connection.Select(
                DB_Table.USERS,
                new string[] { "password", "role" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("login", Relation.EQUAL, cbLogin.Text) }
                );

            if (results.Count == 0)
                MessageBox.Show("Логин не найден.", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (results[0][0].ToString() == tbPassword.Text)
            {
                UsersRole = results[0][1].ToString();
                UsersLogin = cbLogin.Text;

                Properties.Settings.Default.Login = cbLogin.Text;
                Properties.Settings.Default.Save();

                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Неверный пароль.", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbPassword.Clear();
            }
        }

        private void AssureConstants()
        {
            string[] fields = { "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark", "min_foreign_mark" };

            List<object[]> buf = _DB_Connection.Select(DB_Table.CONSTANTS, fields);
            if (buf.Count == 0)
                _DB_Connection.Insert(
                    DB_Table.CONSTANTS,
                    System.Linq.Enumerable.ToDictionary(fields, k => k, e => (object)0)
                    );
        }
    }
}
