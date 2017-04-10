using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class Campaigns : Form
    {
        private readonly  Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        public Campaigns(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            UpdateTable();
        }

        private void UpdateTable()
        {
            dgvCampaigns.Rows.Clear();
            foreach (object[] v in _DB_Connection.Select(DB_Table.CAMPAIGNS,
                new string[] { "id", "name", "start_year", "end_year", "status_dict_id", "status_id" }))
            {
                dgvCampaigns.Rows.Add(v[0], v[1], v[2].ToString() + " - " + v[3].ToString(), "", _DB_Helper.GetDictionaryItemName((uint)v[4], (uint)v[5]));

                foreach (object[] r in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("dictionaries_items_dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, dgvCampaigns.Rows[dgvCampaigns.Rows.Count-1].Cells[0].Value)
                    }))
                {
                    string levelName = _DB_Helper.GetDictionaryItemName(2, (uint)r[0]);

                    if (dgvCampaigns.Rows[dgvCampaigns.Rows.Count - 1].Cells[3].Value.ToString() == "")
                        dgvCampaigns.Rows[dgvCampaigns.Rows.Count - 1].Cells[3].Value = levelName;
                    else
                        dgvCampaigns.Rows[dgvCampaigns.Rows.Count - 1].Cells[3].Value = dgvCampaigns.Rows[dgvCampaigns.Rows.Count - 1].Cells[3].Value.ToString() + " - " + levelName;
                }
            }
        }

        private void btCreatePriemComp_Click(object sender, EventArgs e)
        {
            CampaignEdit form = new CampaignEdit(_DB_Connection,null);
            form.ShowDialog();
            UpdateTable();
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCampaigns.SelectedRows.Count == 0)
                MessageBox.Show("Выберите кампанию в списке.");
            else
            {
                CampaignEdit form = new CampaignEdit(_DB_Connection,(uint)dgvCampaigns.SelectedRows[0].Cells[0].Value);
                form.ShowDialog();
                UpdateTable();
            }
        }
    }
}
