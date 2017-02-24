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
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            _DB_Connection = new DB_Connector();
            uint organizationUID =_DB_Connection.Insert("target_organizations", new Dictionary<string, object> { { "name", rtbOrganizationName.Text } });
            Close();
        }
    }
}
