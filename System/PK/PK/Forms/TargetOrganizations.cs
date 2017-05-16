using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class TargetOrganizations : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;

        public TargetOrganizations(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;

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
            TargetOrganizationEdit form = new TargetOrganizationEdit(_DB_Connection);
            form.ShowDialog();
            UpdateTable();
        }

        private void btRename_Click(object sender, EventArgs e)
        {
            if (dgvTargetOrganizations.SelectedRows.Count == 0)
                MessageBox.Show("Выберите строку");
            else
            {
                TargetOrganizationEdit form = new TargetOrganizationEdit(_DB_Connection,(uint)dgvTargetOrganizations.SelectedRows[0].Cells[0].Value);
                form.ShowDialog();
                UpdateTable();
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (dgvTargetOrganizations.SelectedRows.Count == 0)
                MessageBox.Show("Выберите строку");
            else if (Classes.Utility.ShowChoiceMessageBox("Удалить выбранную организацию?", "Удаление организации"))
            {
                _DB_Connection.Delete(DB_Table.TARGET_ORGANIZATIONS, new Dictionary<string, object>
                { { "id",dgvTargetOrganizations.SelectedRows[0].Cells[0].Value }, { "name", dgvTargetOrganizations.SelectedRows[0].Cells[1].Value} });
                UpdateTable();
            }
        }
    }
}
