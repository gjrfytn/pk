using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class TargetOrganizationSelect : Form
    {
        public uint? OrganizationID;
        public string OrganizationName;

        private readonly Dictionary<uint, string> _All_Items = new Dictionary<uint, string>();
        private readonly Classes.DB_Connector _DB_Connection;

        public TargetOrganizationSelect(Classes.DB_Connector connection, uint? orgID)
        {
            InitializeComponent();

            _DB_Connection = connection;
            OrganizationID = orgID;

            foreach (object[] v in _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, "id", "name"))
            {
                _All_Items.Add((uint)v[0], v[1].ToString());
                lbSelection.Items.Add(v[1]);
            }
            if (OrganizationID != null)
                tbSearchString.Text = _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, OrganizationID)
                })[0][0].ToString();
            tbSearchString.Select();
        }

        private void tbSearchString_TextChanged(object sender, EventArgs e)
        {
            lbSelection.Items.Clear();
            foreach (var v in _All_Items)
                if (v.Value.ToLower().Contains(tbSearchString.Text.ToLower()))
                    lbSelection.Items.Add(v.Value);
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            if ((lbSelection.Items.Count>1)&&(lbSelection.SelectedIndex == -1))
                MessageBox.Show("Выберите организацию в списке.");
            else if (lbSelection.SelectedIndex != -1)
            {
                OrganizationName = lbSelection.SelectedItem.ToString();
                OrganizationID = System.Linq.Enumerable.First(_All_Items, x => x.Value == OrganizationName).Key;

                DialogResult = DialogResult.OK;
            }
            else
            {
                OrganizationName = lbSelection.Items[0].ToString();
                OrganizationID = System.Linq.Enumerable.First(_All_Items, x => x.Value == OrganizationName).Key;

                DialogResult = DialogResult.OK;
            }
        }
    }
}
