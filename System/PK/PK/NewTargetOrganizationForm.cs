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
    public partial class NewTargetOrganizationForm : Form
    {
        DB_Connector _DB_Connection;
        bool _Updating = false;
        int _Code;

        public NewTargetOrganizationForm()
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
        }

        public NewTargetOrganizationForm(int organizationCode)
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
            _Code = organizationCode;
            _Updating = true;
            rtbOrganizationName.Text = _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, new string[] { "name" },
                new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("uid", Relation.EQUAL, _Code)
            })[0][0].ToString();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if(!_Updating)
            {
            uint organizationUID =_DB_Connection.Insert(DB_Table.TARGET_ORGANIZATIONS, 
                new Dictionary<string, object> { { "name", rtbOrganizationName.Text } });
            Close();
            }
            else
            {
                _DB_Connection.Update(DB_Table.TARGET_ORGANIZATIONS, 
                    new Dictionary<string, object> { { "name", rtbOrganizationName.Text } }, 
                    new Dictionary<string, object> { { "uid", _Code } });
                Close();
            }
        }
    }
}