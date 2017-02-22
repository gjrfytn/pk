using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PK
{
    public partial class AutorizationForm : Form
    {
        DB_Connector _DB_Connection;
        MainForm parent;        
        bool _ExitingApp = true;
        public AutorizationForm(MainForm p)
        {
            parent = p;
            InitializeComponent();
        }        

        private void btAuth_Click(object sender, EventArgs e)
        {
            _DB_Connection = new DB_Connector();
            List<object[]> usersdata = new List<object[]>();
            usersdata = _DB_Connection.Select("users", "login" , "password");
            object[] logpass = usersdata.Find(x => x[0].ToString() == cbLogin.Text);
            if (logpass == null)
                MessageBox.Show("Логин не найден");
            else
                if (logpass[1].ToString() == tbPassword.Text)
                {
                    _ExitingApp = false;
                    Close();
                }
            else
                MessageBox.Show("Неверный пароль");
        }

        private void AutorizationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_ExitingApp)
                parent.Show();
            else Application.Exit();        
        }

        private void AutorizationForm_Load(object sender, EventArgs e)
        {
            _DB_Connection = new DB_Connector();
            foreach (var v in _DB_Connection.Select("users", "login"))
            {
                cbLogin.Items.Add(v[0]);
            }
        }
    }
}
