using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class TargetOrganizations : Form
    {
        Classes.DB_Connector _DB_Connection;

        public TargetOrganizations()
        {
            InitializeComponent();
            _DB_Connection = new Classes.DB_Connector();

            UpdateTable();
        }

        private void UpdateTable()
        {
            dgvTargetOrganizations.Rows.Clear();
            foreach (object[] v in _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, "id", "name"))
                dgvTargetOrganizations.Rows.Add(v);
            dgvTargetOrganizations.Sort(dgvTargetOrganizations_Name, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void btNewTargetOrganization_Click(object sender, EventArgs e)
        {
            TargetOrganizationEdit form = new TargetOrganizationEdit();
            form.ShowDialog();
            UpdateTable();
        }

        private void btRename_Click(object sender, EventArgs e)
        {
            if (dgvTargetOrganizations.SelectedRows.Count == 0)
                MessageBox.Show("Выберите строку");
            else
            {
                TargetOrganizationEdit form = new TargetOrganizationEdit((uint)dgvTargetOrganizations.SelectedRows[0].Cells[0].Value);
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
                { { "id",dgvTargetOrganizations.SelectedRows[0].Cells[0].Value }, { "name", dgvTargetOrganizations.SelectedRows[0].Cells[1].Value} });
                UpdateTable();
            }
        }
    }
}
