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

        public NewTargetOrganizationForm()
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            uint organizationUID =_DB_Connection.Insert(DB_Table.TARGET_ORGANIZATIONS, new Dictionary<string, object> { { "name", rtbOrganizationName.Text } });
            Close();
        }
    }
}
