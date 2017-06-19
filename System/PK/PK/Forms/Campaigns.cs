using System;
using System.Collections.Generic;
using System.Linq;
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

            UpdateTableAndCombobox();
        }

        private void btCreatePriemComp_Click(object sender, EventArgs e)
        {
            CampaignEdit form = new CampaignEdit(_DB_Connection,null);
            form.ShowDialog();
            UpdateTableAndCombobox();
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCampaigns.SelectedRows.Count == 0)
                MessageBox.Show("Выберите кампанию в списке.");
            else
            {
                CampaignEdit form = new CampaignEdit(_DB_Connection,(uint)dgvCampaigns.SelectedRows[0].Cells[0].Value);
                form.ShowDialog();
                UpdateTableAndCombobox();
            }
        }

        private void cbCurrentCampaign_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbCurrentCampaign.SelectedIndex != -1 && (uint)cbCurrentCampaign.SelectedValue != Classes.Utility.CurrentCampaignID)
            {
                Properties.Settings.Default.CampaignID = (uint)cbCurrentCampaign.SelectedValue;
                Properties.Settings.Default.Save();
            }
        }

        private void dgvCampaigns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCampaigns.SelectedRows.Count > 0)
            {
                CampaignEdit form = new CampaignEdit(_DB_Connection, (uint)dgvCampaigns.SelectedRows[0].Cells[0].Value);
                form.ShowDialog();
                UpdateTableAndCombobox();
            }
        }


        private void UpdateTableAndCombobox()
        {
            dgvCampaigns.Rows.Clear();
            List<object[]> campaigns = _DB_Connection.Select(DB_Table.CAMPAIGNS,
                new string[] { "id", "name", "start_year", "end_year", "status_dict_id", "status_id" });
            foreach (object[] v in campaigns)
            {
                dgvCampaigns.Rows.Add(v[0], v[1], v[2].ToString() + " - " + v[3].ToString(), "", _DB_Helper.GetDictionaryItemName((FIS_Dictionary)v[4], (uint)v[5]));

                foreach (object[] r in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("dictionaries_items_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.EDU_LEVEL),
                        new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, dgvCampaigns.Rows[dgvCampaigns.Rows.Count-1].Cells[0].Value)
                    }))
                {
                    string levelName = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_LEVEL, (uint)r[0]);

                    if (dgvCampaigns.Rows[dgvCampaigns.Rows.Count - 1].Cells[3].Value.ToString() == "")
                        dgvCampaigns.Rows[dgvCampaigns.Rows.Count - 1].Cells[3].Value = levelName;
                    else
                        dgvCampaigns.Rows[dgvCampaigns.Rows.Count - 1].Cells[3].Value = dgvCampaigns.Rows[dgvCampaigns.Rows.Count - 1].Cells[3].Value.ToString() + " - " + levelName;
                }
            }
            cbCurrentCampaign.ValueMember = "Value";
            cbCurrentCampaign.DisplayMember = "Display";
            cbCurrentCampaign.DataSource = campaigns.Select(s => new
            {
                Value = (uint)s[0],
                Display = s[1].ToString()
            }).ToArray();

            if (Classes.Utility.CurrentCampaignID != 0)
                cbCurrentCampaign.SelectedValue = Classes.Utility.CurrentCampaignID;
            else
                cbCurrentCampaign.SelectedIndex = -1;
        }
    }
}
