using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class Main : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Connector _DB_UpdateConnection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly string _UserLogin;
        private readonly string _UserRole;

        private readonly Dictionary<string, string> _Statuses = new Dictionary<string, string> { { "new", "Новое" }, { "adm_budget", "Зачислен на бюджет" }, { "adm_paid", "Зачислен на платное" },
            { "adm_both", "Зачислен на бюджет и платное" }, { "withdrawn", "Забрал документы" } };
        private uint _SelectedAppID;

        public Main(string userRole, string usersLogin)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, userRole,
                new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, "initial", "1234").Select(
                DB_Table.ROLES_PASSWORDS,
                new string[] { "password" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("role", Relation.EQUAL, userRole) }
                )[0][0].ToString());

            _DB_UpdateConnection = new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, userRole,
                new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, "initial", "1234").Select(
                DB_Table.ROLES_PASSWORDS,
                new string[] { "password" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("role", Relation.EQUAL, userRole) }
                )[0][0].ToString());

            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _UserLogin = usersLogin;
            _UserRole = userRole;
            SetUserRole();

            dgvApplications.Sort(dgvApplications_LastName, System.ComponentModel.ListSortDirection.Ascending);
            System.IO.Directory.CreateDirectory(Classes.Utility.TempPath);            
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
                    System.IO.Directory.Delete(Classes.Utility.TempPath, true);
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
                if (_DB_Helper.IsMasterCampaign(Classes.Settings.CurrentCampaignID))
                    form = new ApplicationMagEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, null);
                else
                    form = new ApplicationEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, null);

                form.ShowDialog();
                UpdateApplicationsTable();
                FilterAppsTable();
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
            if (_DB_Helper.IsMasterCampaign(Classes.Settings.CurrentCampaignID))
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

        private void menuStrip_Orders_Click(object sender, EventArgs e)
        {
            StopTableAutoUpdating();
            Orders form = new Orders(_DB_Connection);
            form.ShowDialog();
            UpdateApplicationsTable();
            FilterAppsTable();
            timer.Start();
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
            _SelectedAppID = (uint)dgvApplications.CurrentRow.Cells[dgvApplications_ID.Index].Value;

            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                if (!(bool)_DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "master_appl" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)dgvApplications.CurrentRow.Cells[0].Value)
                })[0][0])
                {
                    StopTableAutoUpdating();
                    ApplicationEdit form = new ApplicationEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, (uint)dgvApplications.CurrentRow.Cells[0].Value);
                    form.ShowDialog();
                }
                else
                {
                    StopTableAutoUpdating();
                    ApplicationMagEdit form = new ApplicationMagEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, (uint)dgvApplications.CurrentRow.Cells[0].Value);
                    form.ShowDialog();
                }
                UpdateApplicationsTable();
                FilterAppsTable();
                timer.Start();
            }
        }

        private void dgvApplications_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvApplications.SelectedRows.Count > 0)
                _SelectedAppID = (uint)dgvApplications.SelectedRows[0].Cells[dgvApplications_ID.Index].Value;
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
            FilterAppsTable();
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
            FilterAppsTable();
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
            CompleteUpdateAppsTable((IEnumerable<object[]>)e.Result);
            FilterAppsTable();
        }

        private void dgvApplications_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvApplications_Status.Index)
            {
                e.Value = _Statuses[e.Value.ToString()];
                e.FormattingApplied = true;
            }
        }


        private IEnumerable<object[]> GetAppsTableRows()
        {
            bool isMaster = _DB_Helper.IsMasterCampaign(Classes.Settings.CurrentCampaignID);
            IEnumerable<object[]> rows = _DB_UpdateConnection.CallProcedure("get_main_form_table", Classes.Settings.CurrentCampaignID).Select(s => new object[]
            {
                s[0], // УИД
                s[1], // Фамилия
                s[2], // Имя
                s[3], // Отчество
                isMaster?s[4].ToString().Replace("*", ""):s[4].ToString(), // Направления
                s[5], // ОДО
                s[6], // Дата подачи
                s[7], // Дата изменения
                s[8], // Забрал документы
                (s[9] as DateTime?)?.ToShortDateString(), // Приказ об отчислении
                (s[10] as DateTime?)?.ToShortDateString(), // Приказ о зачислении
                s[11], // Регистратор
                s[12].ToString() // Статус
            });

            if (_UserRole == "registrator")
                return rows.Where(s => s[12].ToString() == _UserLogin && ((DateTime)s[7]).Date == DateTime.Now.Date);
            else if (_UserRole == "inspector")
                return rows.Where(s => ((DateTime)s[7]).Date == DateTime.Now.Date);
            else
                return rows;
        }

        private void UpdateApplicationsTable()
        {
            CompleteUpdateAppsTable(GetAppsTableRows());
        }

        private void CompleteUpdateAppsTable(IEnumerable<object[]> rows)
        {
            DataGridViewColumn sortColumn = dgvApplications.SortedColumn;

            System.ComponentModel.ListSortDirection sortMethod = System.ComponentModel.ListSortDirection.Ascending;
            if (dgvApplications.SortOrder == SortOrder.Ascending)
                sortMethod = System.ComponentModel.ListSortDirection.Ascending;
            else if (dgvApplications.SortOrder == SortOrder.Descending)
                sortMethod = System.ComponentModel.ListSortDirection.Descending;

            int displayedRow = dgvApplications.FirstDisplayedScrollingRowIndex;
            if (dgvApplications.Rows.Count == 0)
                dgvApplications.Rows.AddRange(rows.Select(s =>
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvApplications, s);
                    return row;
                }).ToArray());
            else
            {
                foreach (object[] newRow in rows)
                {
                    bool found = false;
                    foreach (DataGridViewRow oldRow in dgvApplications.Rows)
                        if ((uint)oldRow.Cells[dgvApplications_ID.Index].Value == (uint)newRow[dgvApplications_ID.Index])
                        {
                            found = true;
                            bool match = true;
                            foreach (DataGridViewCell oldCell in oldRow.Cells)
                                if (oldCell.Value != null && newRow[oldCell.ColumnIndex] != null && oldCell.Value.ToString() != newRow[oldCell.ColumnIndex].ToString())
                                {
                                    match = false;
                                    break;
                                }
                            if (!match)
                                oldRow.SetValues(newRow);
                            if ((uint)oldRow.Cells[dgvApplications_ID.Index].Value == _SelectedAppID)
                                oldRow.Selected = true;
                        }
                    if (!found)
                        dgvApplications.Rows.Add(newRow);
                }
                int i = 0;
                while(i < dgvApplications.Rows.Count)
                {
                    bool found = false;
                    foreach (object[] newRow in rows)
                        if ((uint)dgvApplications.Rows[i].Cells[dgvApplications_ID.Index].Value == (uint)newRow[dgvApplications_ID.Index])
                            found = true;
                    if (!found)
                        dgvApplications.Rows.RemoveAt(i);
                    else
                        i++;
                }
            }

            if (sortColumn != null)
                dgvApplications.Sort(sortColumn, sortMethod);
            if (displayedRow >= 0 && dgvApplications.Rows.Count >= displayedRow)
                dgvApplications.FirstDisplayedScrollingRowIndex = displayedRow;
        }

        private void FilterAppsTable()
        {
            ChangeColumnsVisible();
            int visibleCount = 0;
            foreach (DataGridViewRow row in dgvApplications.Rows)
            {
                string status = row.Cells[dgvApplications_Status.Index].Value.ToString();

                row.Visible = !(rbNew.Checked && status != "new" ||
                    rbWithdraw.Checked && status != "withdrawn" ||
                    rbAdm.Checked && status != "adm_budget" && status != "adm_paid" && status != "adm_both");

                if (row.Visible && (tbRegNumber.Text != tbRegNumber.Tag.ToString() || tbLastName.Text != tbLastName.Tag.ToString()
                    || tbFirstName.Text != tbFirstName.Tag.ToString() || tbMiddleName.Text != tbMiddleName.Tag.ToString() || dtpRegDate.Value != dtpRegDate.MinDate))
                {
                    bool matches = true;
                    if ((tbRegNumber.Text != "") && (tbRegNumber.Text != tbRegNumber.Tag.ToString()) && !row.Cells[dgvApplications_ID.Index].Value.ToString().ToLower().StartsWith(tbRegNumber.Text.ToLower()))
                        matches = false;
                    else if ((tbLastName.Text != "") && (tbLastName.Text != tbLastName.Tag.ToString()) && !row.Cells[dgvApplications_LastName.Index].Value.ToString().ToLower().StartsWith(tbLastName.Text.ToLower()))
                        matches = false;
                    else if ((tbFirstName.Text != "") && (tbFirstName.Text != tbFirstName.Tag.ToString()) && !row.Cells[dgvApplications_FirstName.Index].Value.ToString().ToLower().StartsWith(tbFirstName.Text.ToLower()))
                        matches = false;
                    else if ((tbMiddleName.Text != "") && (tbMiddleName.Text != tbMiddleName.Tag.ToString()) && !row.Cells[dgvApplications_MiddleName.Index].Value.ToString().ToLower().StartsWith(tbMiddleName.Text.ToLower()))
                        matches = false;
                    else if ((dtpRegDate.Value != dtpRegDate.MinDate) && ((row.Cells[dgvApplications_RegDate.Index].Value as DateTime?).Value.Date != dtpRegDate.Value.Date))
                        matches = false;
                    row.Visible = matches;
                }

                if (row.Visible)
                    visibleCount++;
            }

            lbDispalyedCount.Text = lbDispalyedCount.Tag.ToString() + visibleCount;
        }

        private void SetCurrentCampaign()
        {
            if (Classes.Settings.CurrentCampaignID != 0)
            {
                toolStrip_CurrCampaign.Text = _DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, Classes.Settings.CurrentCampaignID)
                })[0][0].ToString();

                bool master = _DB_Helper.IsMasterCampaign(Classes.Settings.CurrentCampaignID);
                dgvApplications_Original.Visible = !master;
                dgvApplications_Entrances.HeaderText = master?"Маг. программы":"Направления";

                UpdateApplicationsTable();
                FilterAppsTable();
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
