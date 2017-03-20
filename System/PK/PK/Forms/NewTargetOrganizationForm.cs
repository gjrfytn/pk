using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class NewTargetOrganizationForm : Form
    {
        Classes.DB_Connector _DB_Connection;
        uint? _UpdatingCode;

        public NewTargetOrganizationForm()
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
        }

        public NewTargetOrganizationForm(uint organizationCode)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
            _UpdatingCode = organizationCode;
            rtbOrganizationName.Text = _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, new string[] { "name" },
                new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("uid", Relation.EQUAL, _UpdatingCode)
            })[0][0].ToString();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (!_UpdatingCode.HasValue)
                _DB_Connection.Insert(DB_Table.TARGET_ORGANIZATIONS,
                    new Dictionary<string, object> { { "name", rtbOrganizationName.Text } });
            else
                _DB_Connection.Update(DB_Table.TARGET_ORGANIZATIONS,
                    new Dictionary<string, object> { { "name", rtbOrganizationName.Text } },
                    new Dictionary<string, object> { { "uid", _UpdatingCode } });

            Close();
        }
    }
}
