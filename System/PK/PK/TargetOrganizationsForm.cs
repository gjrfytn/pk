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
    public partial class TargetOrganizationsForm : Form
    {
        DB_Connector _DB_Connection;

        public TargetOrganizationsForm()
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
        }

        private void UpdateTable ()
        {
            dgvTargetOrganizations.Rows.Clear();
            List<object[]> tOList = new List<object[]>();
            tOList = _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, "name");
            foreach (var v in tOList)
                dgvTargetOrganizations.Rows.Add(v);
        }

        private void TargetOrganizationsForm_Load(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void btNewTargetOrganization_Click(object sender, EventArgs e)
        {
            NewTargetOrganizationForm form = new NewTargetOrganizationForm();
            form.ShowDialog();
            UpdateTable();
        }
    }
}
