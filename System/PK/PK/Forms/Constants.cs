using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class Constants : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private uint _CurrCampaignID;

        public Constants(Classes.DB_Connector connection, uint campaignID)
        {
            InitializeComponent();
            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _CurrCampaignID = campaignID;
            lbCurrentCampaign.Text = _DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("id", Relation.EQUAL, _CurrCampaignID)
            })[0][0].ToString();
            UpdateTable();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            string[] fields = { "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark", "min_foreign_mark" };
            object[] buf = _DB_Connection.Select(DB_Table.CONSTANTS, fields)[0];

            Dictionary<string, object> values = new Dictionary<string, object>(fields.Length);
            Dictionary<string, object> where = new Dictionary<string, object>(fields.Length);
            for (byte i = 0; i < fields.Length; ++i)
            {
                values.Add(fields[i], dgvConstants[dgvConstants_Value.Index, i].Value);
                where.Add(fields[i], buf[i]);
            }

            _DB_Connection.Update(DB_Table.CONSTANTS, values, where);
            Close();
        }

        private void UpdateTable()
        {
            dgvConstants.Rows.Clear();

            string[] subjects = { "МАТЕМАТИКЕ", "РУССКОМУ ЯЗЫКУ", "ФИЗИКЕ", "ОБЩЕСТВОЗНАНИЮ", "ИНОСТРАННОМУ ЯЗЫКУ" };
            string[] fields = { "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark", "min_foreign_mark" };
            object[] constants = _DB_Connection.Select(DB_Table.CONSTANTS, fields)[0];

            for (byte i = 0; i < fields.Length; ++i)
                dgvConstants.Rows.Add(fields[i], "Минимальный балл ЕГЭ по " + subjects[i], constants[i]);
        }
    }
}
