using System;
using System.Collections.Generic;
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

            UpdateTable();
        }

        private void UpdateTable()
        {
            dgvTargetOrganizations.Rows.Clear();
            foreach (object[] v in _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, "uid", "name"))
                dgvTargetOrganizations.Rows.Add(v);
            dgvTargetOrganizations.Sort(cOrgName, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void btNewTargetOrganization_Click(object sender, EventArgs e)
        {
            NewTargetOrganizationForm form = new NewTargetOrganizationForm();
            form.ShowDialog();
            UpdateTable();
        }

        private void btRename_Click(object sender, EventArgs e)
        {
            if (dgvTargetOrganizations.SelectedRows.Count == 0)
                MessageBox.Show("Выберите строку");
            else
            {
                NewTargetOrganizationForm form = new NewTargetOrganizationForm((uint)dgvTargetOrganizations.SelectedRows[0].Cells[0].Value);
                form.ShowDialog();
                UpdateTable();
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (dgvTargetOrganizations.SelectedRows.Count == 0)
                MessageBox.Show("Выберите строку");
            else
            {
                _DB_Connection.Delete(DB_Table.TARGET_ORGANIZATIONS, new Dictionary<string, object>
                { { "uid",dgvTargetOrganizations.SelectedRows[0].Cells[0].Value }, { "name", dgvTargetOrganizations.SelectedRows[0].Cells[1].Value} });
                UpdateTable();
            }
        }
    }
}
