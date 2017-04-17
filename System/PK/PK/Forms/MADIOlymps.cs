using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class MADIOlymps : Form
    {
        public string OlympName;

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        public MADIOlymps(Classes.DB_Connector connection, string name)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            OlympName = name;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (tbOlympName.Text == "")
                MessageBox.Show("Введите название олимпиады");
            else
            {
                OlympName = tbOlympName.Text;
                DialogResult = DialogResult.OK;
            }                   
        }
    }
}
