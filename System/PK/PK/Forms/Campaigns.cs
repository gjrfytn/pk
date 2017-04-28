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
        private uint? _CurrCampaignID;

        public Campaigns(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            List<object[]> currCampaigns = _DB_Connection.Select(DB_Table.CONSTANTS, "current_campaign_id");
            if (currCampaigns.Count > 0)
                _CurrCampaignID = (uint)currCampaigns[0][0];

            UpdateTableAndCombobox();
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
            if (_CurrCampaignID != null)
                cbCurrentCampaign.SelectedValue = _CurrCampaignID;
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
            if (cbCurrentCampaign.SelectedIndex != -1 && (uint)cbCurrentCampaign.SelectedValue != _CurrCampaignID)
            {
                if (_DB_Connection.Select(DB_Table.CONSTANTS, new string[] { "current_campaign_id" }).Count > 0)
                    _DB_Connection.Update(DB_Table.CONSTANTS, new Dictionary<string, object> { { "current_campaign_id", (uint)cbCurrentCampaign.SelectedValue } },
                        new Dictionary<string, object> { { "current_campaign_id", _CurrCampaignID } });
                else
                    _DB_Connection.Insert(DB_Table.CONSTANTS, new Dictionary<string, object> { { "current_campaign_id", (uint)cbCurrentCampaign.SelectedValue },
                        { "min_math_mark", 0 }, { "min_russian_mark", 0 }, { "min_physics_mark", 0 }, { "min_social_mark", 0 }, { "min_foreign_mark", 0 } });

                _CurrCampaignID = (uint)cbCurrentCampaign.SelectedValue;
            }
        }
    }
}
