using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK
{
    public partial class GridItemSelection : Form
    {
        public uint OrganizationID;
        public string OrganizationName;

        Dictionary<uint, string> _All_Items = new Dictionary<uint, string>();
        Classes.DB_Connector _DB_Connection;

        public GridItemSelection()
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();

            foreach (object[] v in _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, "uid", "name"))
            {
                _All_Items.Add((uint)v[0], v[1].ToString());
                lbSelection.Items.Add(v[1]);
            }

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
            if (lbSelection.SelectedIndex == -1)
                MessageBox.Show("Выберите организацию в списке.");
            else
            {
                OrganizationName = lbSelection.SelectedItem.ToString();
                OrganizationID = System.Linq.Enumerable.First(_All_Items, x => x.Value == OrganizationName).Key;

                DialogResult = DialogResult.OK;
            }
        }
    }

}
