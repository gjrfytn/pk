using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PK
{
    public partial class GridItemSelection : Form
    {
        List<string> _All_Items;
        List<string> _Displayed_Items;
        DB_Connector _DB_Connection;
        public string organizationName;
        
        public GridItemSelection()
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
            _All_Items = new List<string>();
            _Displayed_Items = new List<string>();
        }

        private void GridItemSelection_Load(object sender, EventArgs e)
        {
            List<object[]> tOList = new List<object[]>();
            tOList = _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, "name");
            foreach (var v in tOList)
                _All_Items.Add(v[0].ToString());

            _Displayed_Items.AddRange(_All_Items);
            lbSelection.DataSource = _Displayed_Items;

            tbSearchString.Select();
        }

        private void tbSearchString_TextChanged(object sender, EventArgs e)
        {
            _Displayed_Items.Clear();
            foreach (var v in _All_Items)
            {
                if (v.Contains(tbSearchString.Text))
                    _Displayed_Items.Add(v);
            }
            lbSelection.DataSource = null;
            lbSelection.DataSource = _Displayed_Items;
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            if (lbSelection.SelectedIndex == -1)
                MessageBox.Show("Выберите организацию в списке.");
            else
            {
                organizationName = lbSelection.SelectedItem.ToString();
                DialogResult = DialogResult.OK;
            }
        }
    }

}
