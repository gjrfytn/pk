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
                try
                {
                    _DB_Connection.Delete(DB_Table.TARGET_ORGANIZATIONS, new Dictionary<string, object>
                    { { "id", dgvTargetOrganizations.SelectedRows[0].Cells[0].Value }, { "name", dgvTargetOrganizations.SelectedRows[0].Cells[1].Value } });
                    UpdateTable();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    if (ex.Number == 1217 || ex.Number == 1451)
                    {
                        List<object[]> appEntrances = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "application_id" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("target_organization_id", Relation.EQUAL, dgvTargetOrganizations.SelectedRows[0].Cells[0].Value)
                        });
                        if (appEntrances.Count > 0)
                            MessageBox.Show("С данной целевой организацией связано заявление. Удаление невозможно.");
                        else if (Classes.Utility.ShowChoiceMessageWithConfirmation("Целевая организация включена в кампанию. Выполнить удаление?", "Связь с кампанией"))
                        {
                            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
                            {
                                _DB_Connection.Delete(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new Dictionary<string, object>
                                { { "target_organization_id", dgvTargetOrganizations.SelectedRows[0].Cells[0].Value } }, transaction);

                                _DB_Connection.Delete(DB_Table.TARGET_ORGANIZATIONS, new Dictionary<string, object>
                                { { "id", dgvTargetOrganizations.SelectedRows[0].Cells[0].Value } }, transaction);

                                transaction.Commit();
                            }
                            UpdateTable();
                        }
                    }
                }
            }
        }
    }
}
