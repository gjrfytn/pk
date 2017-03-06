using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK
{
    public partial class CampaignsForm : Form
    {
        DB_Connector _DB_Connection;

        public CampaignsForm()
        {
            InitializeComponent();

            _DB_Connection = new DB_Connector();

            UpdateTable();
        }

        private void UpdateTable()
        {
            dgvPriemComp.Rows.Clear();
            foreach (object[] v in _DB_Connection.Select(DB_Table.CAMPAIGNS,
                new string[] { "uid", "name", "start_year", "end_year", "status_dict_id", "status_id" }))
            {
                dgvPriemComp.Rows.Add(v[0], v[1], v[2].ToString() + " - " + v[3].ToString(), "",
                    _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, v[4]),
                        new Tuple<string, Relation, object>("item_id", Relation.EQUAL, v[5])
                    })[0][0].ToString());

                foreach (object[] r in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("dictionaries_items_dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object>("campaigns_uid", Relation.EQUAL, dgvPriemComp.Rows[dgvPriemComp.Rows.Count-1].Cells[0].Value)
                    }))
                {
                    string levelName = _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, 2),
                    new Tuple<string, Relation, object>("item_id", Relation.EQUAL, r[0])
                })[0][0].ToString();

                    if (dgvPriemComp.Rows[dgvPriemComp.Rows.Count - 1].Cells[3].Value.ToString() == "")
                        dgvPriemComp.Rows[dgvPriemComp.Rows.Count - 1].Cells[3].Value = levelName;
                    else
                        dgvPriemComp.Rows[dgvPriemComp.Rows.Count - 1].Cells[3].Value = dgvPriemComp.Rows[dgvPriemComp.Rows.Count - 1].Cells[3].Value.ToString() + " - " + levelName;
                }
            }
        }

        private void btCreatePriemComp_Click(object sender, EventArgs e)
        {
            NewCampaignForm form = new NewCampaignForm(null);
            form.ShowDialog();
            UpdateTable();
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (dgvPriemComp.SelectedRows.Count == 0)
                MessageBox.Show("Выберите кампанию в списке.");
            else
            {
                NewCampaignForm form = new NewCampaignForm((uint)dgvPriemComp.SelectedRows[0].Cells[0].Value);
                form.ShowDialog();
                UpdateTable();
            }
        }
    }
}
