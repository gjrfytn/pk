using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class MADIOlymps : Form
    {
        public string OlympName;
        public string OlympOrg;
        public DateTime OlympDate;

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        public MADIOlymps(Classes.DB_Connector connection, Forms.ApplicationEdit.MODoc olympData)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            if (olympData.olympName != null && olympData.olympName != "")
            {
                tbOlympName.Text = olympData.olympName;
                OlympName = olympData.olympName;
                tbOrganization.Text = olympData.olypmOrg;
                OlympOrg = olympData.olypmOrg;
                dtpDate.Value = olympData.olympDate;
                OlympDate = olympData.olympDate;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (tbOlympName.Text == "" || tbOrganization.Text == "")
                MessageBox.Show("Все поля должны быть заполнены");
            else
            {
                OlympName = tbOlympName.Text;
                OlympOrg = tbOrganization.Text;
                OlympDate = dtpDate.Value;
                DialogResult = DialogResult.OK;
            }                   
        }
    }
}
