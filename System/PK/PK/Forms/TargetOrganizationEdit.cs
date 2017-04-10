using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class TargetOrganizationEdit : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly uint? _UpdatingCode;

        public TargetOrganizationEdit(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
        }

        public TargetOrganizationEdit(Classes.DB_Connector connection, uint organizationCode)
        {
            InitializeComponent();

            _DB_Connection = connection;
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

            DialogResult = DialogResult.OK;
        }
    }
}
