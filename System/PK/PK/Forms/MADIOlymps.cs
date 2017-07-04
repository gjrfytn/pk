using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class MADIOlymps : Form
    {
        public string OlympName;
        public string OlympOrg;
        public DateTime OlympDate;

        private readonly DB_Connector _DB_Connection;
        private readonly DB_Helper _DB_Helper;

        public MADIOlymps(DB_Connector connection, Forms.ApplicationEdit.MODoc olympData)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new DB_Helper(_DB_Connection);

            List<string> olympsNames = new List<string>();
            foreach (object[] olymp in _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_name" }, new System.Collections.Generic.List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("year", Relation.GREATER_EQUAL, DateTime.Now.Year -1)
            }))
                olympsNames.Add(olymp[0].ToString());
            foreach(string name in olympsNames.Distinct())
                cbOlympName.Items.Add(name);

            if (olympData.olympName != null && olympData.olympName != "")
            {
                cbOlympName.Text = olympData.olympName;
                OlympName = olympData.olympName;
                tbOrganization.Text = olympData.olypmOrg;
                OlympOrg = olympData.olypmOrg;
                dtpDate.Value = olympData.olympDate;
                OlympDate = olympData.olympDate;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (cbOlympName.Text == "" || tbOrganization.Text == "")
                MessageBox.Show("Все поля должны быть заполнены");
            else
            {
                OlympName = cbOlympName.Text;
                OlympOrg = tbOrganization.Text;
                OlympDate = dtpDate.Value;
                DialogResult = DialogResult.OK;
            }                   
        }
    }
}
