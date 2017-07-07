using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    public partial class Main : Form
    {
        private readonly DB_Connector _DB_Connection;
        private readonly DB_Connector _DB_UpdateConnection;
        private readonly DB_Helper _DB_Helper;
        private readonly string _UserLogin;
        private readonly string _UserRole;

        private readonly Dictionary<string, string> _Statuses = new Dictionary<string, string>
        {
            { "new", "Новое" },
            { "adm_budget", "Зачислен на бюджет" },
            { "adm_paid", "Зачислен на платное" },
            { "adm_both", "Зачислен на бюджет и платное" },
            { "withdrawn", "Забрал документы" }
        };
        private uint _SelectedAppID;
        bool _MasterCampaign;

        public Main(string userRole, string usersLogin)
        {
            InitializeComponent();

            _DB_Connection = new DB_Connector(Properties.Settings.Default.pk_db_CS, userRole,
                new DB_Connector(Properties.Settings.Default.pk_db_CS, "initial", "1234").Select(
                DB_Table.ROLES_PASSWORDS,
                new string[] { "password" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("role", Relation.EQUAL, userRole) }
                )[0][0].ToString());

            _DB_UpdateConnection = new DB_Connector(Properties.Settings.Default.pk_db_CS, userRole,
                new DB_Connector(Properties.Settings.Default.pk_db_CS, "initial", "1234").Select(
                DB_Table.ROLES_PASSWORDS,
                new string[] { "password" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("role", Relation.EQUAL, userRole) }
                )[0][0].ToString());

            _DB_Helper = new DB_Helper(_DB_Connection);
            _UserLogin = usersLogin;
            _UserRole = userRole;
            SetUserRole();

            dgvApplications.Sort(dgvApplications_LastName, System.ComponentModel.ListSortDirection.Ascending);
            System.IO.Directory.CreateDirectory(Classes.Settings.TempPath);            
            dtpRegDate.Value = dtpRegDate.MinDate;
            SetCurrentCampaign();
            rbNew.Checked = true;

            lFilter.BackColor = toolStrip.BackColor;
            rbAdm.BackColor = toolStrip.BackColor;
            rbNew.BackColor = toolStrip.BackColor;
            rbWithdraw.BackColor = toolStrip.BackColor;
        }

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _DB_Connection.Dispose();

                    if (components != null)
                        components.Dispose();
                }

                try
                {
                    System.IO.Directory.Delete(Classes.Settings.TempPath, true);
                }
                catch (System.IO.IOException) { }

                base.Dispose(disposing);
            }
        }

        ~Main()
        {
            Dispose(false);
        }
        #endregion

        private void menuStrip_Campaign_Campaigns_Click(object sender, EventArgs e)
        {
            StopTableAutoUpdating();
            Campaigns form = new Campaigns(_DB_Connection);
            form.ShowDialog();
            SetCurrentCampaign();
        }

        private void CreateApplication_Click(object sender, EventArgs e)
        {
            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                StopTableAutoUpdating();
                Form form;
                if (_MasterCampaign)
                    form = new ApplicationMagEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, null);
                else
                    form = new ApplicationEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, null);

                form.ShowDialog();
                UpdateApplicationsTable();
                timer.Start();
            }
        }

        private void menuStrip_TargetOrganizations_Click(object sender, EventArgs e)
        {
            TargetOrganizations form = new TargetOrganizations(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_Dictionaries_Click(object sender, EventArgs e)
        {
            Dictionaries form = new Dictionaries(_DB_Connection);
            form.Show();
        }

        private void menuStrip_DirDictionary_Click(object sender, EventArgs e)
        {
            DirectionsDictionary form = new DirectionsDictionary(_DB_Connection);
            form.Show();
        }

        private void menuStrip_OlympDictionary_Click(object sender, EventArgs e)
        {
            OlympicsDictionary form = new OlympicsDictionary(_DB_Connection);
            form.Show();
        }

        private void menuStrip_Faculties_Click(object sender, EventArgs e)
        {
            Faculties form = new Faculties(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_Directions_Click(object sender, EventArgs e)
        {
            DirectionsProfiles form = new DirectionsProfiles(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_Campaign_Exams_Click(object sender, EventArgs e)
        {
            if (_MasterCampaign)
            {
                MasterExaminations form = new MasterExaminations(_DB_Connection);
                form.ShowDialog();
            }
            else
            {
                Examinations form = new Examinations(_DB_Connection);
                form.ShowDialog();
            }
        }

        private void menuStrip_InstitutionAchievements_Click(object sender, EventArgs e)
        {
            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                InstitutionAchievementsEdit form = new InstitutionAchievementsEdit(_DB_Connection);
                form.ShowDialog();
            }
        }

        private void menuStrip_Campaign_Orders_Click(object sender, EventArgs e)
        {
            StopTableAutoUpdating();
            Orders form = new Orders(_DB_Connection);
            form.ShowDialog();
            UpdateApplicationsTable();
            timer.Start();
        }

        private void menuStrip_Campaign_Statistics_Click(object sender, EventArgs e)
        {
            Statistics form = new Statistics(_DB_Connection);
            form.Show();
        }

        private void menuStrip_Constants_Click(object sender, EventArgs e)
        {
            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                Constants form = new Constants(_DB_Connection, Classes.Settings.CurrentCampaignID);
                form.ShowDialog();
            }
        }

        private void menuStrip_KLADR_Update_Click(object sender, EventArgs e)
        {
            KLADR_Update form = new KLADR_Update(_DB_Connection.User, _DB_Connection.Password);
            form.ShowDialog();
        }


        private void toolStrip_Users_Click(object sender, EventArgs e)
        {
            Users form = new Users(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_FIS_Export_Click(object sender, EventArgs e)
        {
            FIS_Export form = new FIS_Export(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_RegJournal_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Classes.OutDocuments.RegistrationJournal(_DB_Connection));
        }
        
        private void menuStrip_DirsPlaces_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Classes.OutDocuments.DirectionsPlaces(_DB_Connection));
        }

        private void menuStrip_ProfilesPlaces_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Classes.OutDocuments.ProfilesPlaces(_DB_Connection));
        }

        private void menuStrip_CheckEgeMarks_Click(object sender, EventArgs e)
        {
            EGE_Check form = new EGE_Check(_DB_Connection);
            form.ShowDialog();
        }

        private void dgvApplications_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            _SelectedAppID = (uint)(int)dgvApplications.CurrentRow.Cells[dgvApplications_ID.Index].Value;

            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                if (!(bool)_DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "master_appl" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, dgvApplications.CurrentRow.Cells[0].Value)
                })[0][0])
                {
                    StopTableAutoUpdating();
                    ApplicationEdit form = new ApplicationEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, (uint)(int)dgvApplications.CurrentRow.Cells[0].Value);
                    form.ShowDialog();
                }
                else
                {
                    StopTableAutoUpdating();
                    ApplicationMagEdit form = new ApplicationMagEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, (uint)dgvApplications.CurrentRow.Cells[0].Value);
                    form.ShowDialog();
                }
                UpdateApplicationsTable();
                timer.Start();
            }
        }

        private void dgvApplications_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvApplications.SelectedRows.Count > 0)
                _SelectedAppID = (uint)(int)dgvApplications.SelectedRows[0].Cells[dgvApplications_ID.Index].Value;
        }


        private void tbField_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = (sender as TextBox).Tag.ToString();
                (sender as TextBox).ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void tbField_Enter(object sender, EventArgs e)
        {
            if ((sender as TextBox).ForeColor == System.Drawing.Color.Gray)
            {
                (sender as TextBox).Text = "";
                (sender as TextBox).ForeColor = System.Drawing.Color.Black;
            }
        }

        private void tbField_TextChanged(object sender, EventArgs e)
        {
            StopTableAutoUpdating();
            UpdateApplicationsTable();
            timer.Start();
        }

        private void cbDateSearch_CheckedChanged(object sender, EventArgs e)
        {
            dtpRegDate.Enabled = cbDateSearch.Checked;
            if (!dtpRegDate.Enabled)
                dtpRegDate.Value = dtpRegDate.MinDate;
            else
                dtpRegDate.Value = DateTime.Now;
        }

        private void rbFilter_CheckedChanged(object sender, EventArgs e)
        {
            StopTableAutoUpdating();
            UpdateApplicationsTable();
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
            timer.Start();
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            e.Result = GetAppsTableRows();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            CompleteUpdateAppsTable((System.Data.DataTable)e.Result);
        }

        private void dgvApplications_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvApplications_Status.Index)
            {
                e.Value = _Statuses[e.Value.ToString()];
                e.FormattingApplied = true;
            }
            else if(e.ColumnIndex == dgvApplications_Entrances.Index)
            {
                e.Value = _MasterCampaign ? e.Value.ToString().Replace("*", ""): e.Value.ToString();
                e.FormattingApplied = true;
            }
        }
        

        private System.Data.DataTable GetAppsTableRows()
        {
            return _DB_UpdateConnection.CallProcedureDataTable(
                "get_main_form_table",
                new Dictionary<string, object>
                {
                    { "campaign_id", Classes.Settings.CurrentCampaignID },
                    { "id", tbRegNumber.Text != tbRegNumber.Tag.ToString()?tbRegNumber.Text:"" },
                    { "last_name", tbLastName.Text != tbLastName.Tag.ToString()?tbLastName.Text:"" },
                    { "first_name", tbFirstName.Text != tbFirstName.Tag.ToString()?tbFirstName.Text:"" },
                    { "middle_name", tbMiddleName.Text != tbMiddleName.Tag.ToString()?tbMiddleName.Text:"" },
                    { "date",_UserRole!="administrator"?DateTime.Now.Date:(cbDateSearch.Checked?(DateTime?)dtpRegDate.Value:null) },
                    { "status", rbNew.Checked?"new":(rbWithdraw.Checked?"withdrawn":null) }
                }                );
        }

        private void UpdateApplicationsTable()
        {
            CompleteUpdateAppsTable(GetAppsTableRows());
        }

        private void CompleteUpdateAppsTable(System.Data.DataTable table)
        {
            string sortColumnName = dgvApplications.SortedColumn.Name;

            System.ComponentModel.ListSortDirection sortMethod = System.ComponentModel.ListSortDirection.Ascending;
            if (dgvApplications.SortOrder == SortOrder.Ascending)
                sortMethod = System.ComponentModel.ListSortDirection.Ascending;
            else if (dgvApplications.SortOrder == SortOrder.Descending)
                sortMethod = System.ComponentModel.ListSortDirection.Descending;

            int displayedRow = dgvApplications.FirstDisplayedScrollingRowIndex;

            dgvApplications.AutoGenerateColumns = false;
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = table;
            if (_UserRole == "registrator")
                bindingSource.Filter = "registrator_login = '" + _UserLogin+"'";

            dgvApplications.DataSource = bindingSource;

            dgvApplications.Sort(dgvApplications.Columns[sortColumnName], sortMethod);

            if (displayedRow != -1 && dgvApplications.Rows.Count > displayedRow)
                dgvApplications.FirstDisplayedScrollingRowIndex = displayedRow;

            lbDispalyedCount.Text = lbDispalyedCount.Tag.ToString() + dgvApplications.Rows.Count;
        }

        private void SetCurrentCampaign()
        {
            if (Classes.Settings.CurrentCampaignID != 0)
            {
                toolStrip_CurrCampaign.Text = _DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, Classes.Settings.CurrentCampaignID)
                })[0][0].ToString();

                _MasterCampaign = _DB_Helper.IsMasterCampaign(Classes.Settings.CurrentCampaignID);
                dgvApplications_Original.Visible = !_MasterCampaign;
                dgvApplications_Entrances.HeaderText = _MasterCampaign ? "Маг. программы":"Направления";

                UpdateApplicationsTable();
                if (_UserRole != "registrator")
                    timer.Start();
            }
        }

        private void SetUserRole()
        {
            List<string> roles = new List<string>();

            if (_UserRole == "registrator")
                menuStrip.Enabled = false;
            else
            {
                if (_UserRole == "inspector")
                    roles.AddRange(new string[] { "registrator", "inspector" });
                else if (_UserRole == "administrator")
                    roles.AddRange(new string[] { "registrator", "inspector", "administrator" });

                foreach (ToolStripMenuItem menuStrip in MainMenuStrip.Items)
                {
                    foreach (ToolStripItem submenuItem in menuStrip.DropDownItems)
                        if (submenuItem.Tag != null && !roles.Contains(submenuItem.Tag.ToString()))
                            submenuItem.Enabled = false;

                    bool enabled = false;
                    foreach (ToolStripItem submenuItem in menuStrip.DropDownItems)
                        if (submenuItem.Enabled)
                            enabled = true;

                    if (!enabled)
                        menuStrip.Enabled = false;
                }
            }
        }

        private void ChangeColumnsVisible()
        {
            dgvApplications_PickUpDate.Visible = rbWithdraw.Checked;
            dgvApplications_EnrollmentDate.Visible = rbNew.Checked || rbAdm.Checked;
            dgvApplications_DeductionDate.Visible = rbNew.Checked;
            dgvApplications_Status.Visible = rbAdm.Checked;
        }

        private void StopTableAutoUpdating()
        {
            backgroundWorker.CancelAsync();
            timer.Stop();
        }
    }
}
