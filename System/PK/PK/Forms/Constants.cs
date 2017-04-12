using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class Constants : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private uint _CurrCampaignID;

        public Constants()
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            foreach (var campaign in _DB_Connection.Select(DB_Table.CAMPAIGNS, "name"))
            {
                cbCampaign.Items.Add(campaign[0]);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCampaign.SelectedIndex != -1)
            {
                _CurrCampaignID = (uint)_DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "id" },
                    new List<Tuple<string, Relation, object>>
                    {
                                        new Tuple<string, Relation, object>("name", Relation.EQUAL, cbCampaign.SelectedItem.ToString())
                    })[0][0];
                UpdateTable();
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (cbCampaign.SelectedIndex == -1)
                MessageBox.Show("Не выбрана кампания");
            else if (_DB_Connection.Select(DB_Table.CONSTANTS, new string[] { "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark",
                "min_foreign_mark" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("current_campaign_id", Relation.EQUAL, _CurrCampaignID)
                }).Count > 0)
                _DB_Connection.Update(DB_Table.CONSTANTS, new Dictionary<string, object>
                    {{ "min_math_mark", dgvConstants.Rows[0].Cells[2].Value }, { "min_russian_mark", dgvConstants.Rows[1].Cells[2].Value },
                    { "min_physics_mark", dgvConstants.Rows[2].Cells[2].Value }, { "min_social_mark", dgvConstants.Rows[3].Cells[2].Value },
                    { "min_foreign_mark", dgvConstants.Rows[4].Cells[2].Value } }, new Dictionary<string, object> { { "current_campaign_id", _CurrCampaignID } });
            else
            {
                _DB_Connection.Insert(DB_Table.CONSTANTS, new Dictionary<string, object> { { "current_campaign_id", _CurrCampaignID },
                    { "min_math_mark", dgvConstants.Rows[0].Cells[2].Value }, { "min_russian_mark", dgvConstants.Rows[1].Cells[2].Value },
                    { "min_physics_mark", dgvConstants.Rows[2].Cells[2].Value }, { "min_social_mark", dgvConstants.Rows[3].Cells[2].Value },
                    { "min_foreign_mark", dgvConstants.Rows[4].Cells[2].Value } });
            }
        }

        private void UpdateTable()
        {
            dgvConstants.Rows.Clear();

            List<object[]> constants = _DB_Connection.Select(DB_Table.CONSTANTS, new string[] { "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark",
                "min_foreign_mark" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("current_campaign_id", Relation.EQUAL, _CurrCampaignID)
                });
            if (constants.Count > 0)
            {
                dgvConstants.Rows.Add("min_math_mark", "Минибальный балл ЕГЭ по МАТЕМАТИКЕ", constants[0][0]);
                dgvConstants.Rows.Add("min_russian_mark", "Минибальный балл ЕГЭ по РУССКОМУ ЯЗЫКУ", constants[0][1]);
                dgvConstants.Rows.Add("min_physics_mark", "Минибальный балл ЕГЭ по ФИЗИКЕ", constants[0][2]);
                dgvConstants.Rows.Add("min_social_mark", "Минибальный балл ЕГЭ по ОБЩЕСТВОЗНАНИЮ", constants[0][3]);
                dgvConstants.Rows.Add("min_foreign_mark", "Минибальный балл ЕГЭ по ИНОСТРАННОМУ ЯЗЫКУ", constants[0][4]);
            }
            else
            {
                dgvConstants.Rows.Add("min_math_mark", "Минибальный балл ЕГЭ по МАТЕМАТИКЕ", 0);
                dgvConstants.Rows.Add("min_russian_mark", "Минибальный балл ЕГЭ по РУССКОМУ ЯЗЫКУ", 0);
                dgvConstants.Rows.Add("min_physics_mark", "Минибальный балл ЕГЭ по ФИЗИКЕ", 0);
                dgvConstants.Rows.Add("min_social_mark", "Минибальный балл ЕГЭ по ОБЩЕСТВОЗНАНИЮ", 0);
                dgvConstants.Rows.Add("min_foreign_mark", "Минибальный балл ЕГЭ по ИНОСТРАННОМУ ЯЗЫКУ", 0);
            }
        }
    }
}
