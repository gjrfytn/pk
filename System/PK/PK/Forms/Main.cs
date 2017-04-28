using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class Main : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly string _UserLogin;

        private uint _CurrCampaignID;

        public Main(string userRole, string usersLogin)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector(userRole, new Classes.DB_Connector("initial", "1234").Select(
                DB_Table.ROLES_PASSWORDS,
                new string[] { "password" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("role", Relation.EQUAL, userRole) }
                )[0][0].ToString());
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _UserLogin = usersLogin;

            System.IO.Directory.CreateDirectory(Classes.Utility.TempPath);            
            dtpRegDate.Value = dtpRegDate.MinDate;
            SetCurrentCampaign();
        }

        #region IDisposable Support
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                try
                {
                    if (disposing)
                    {
                        _DB_Connection.Dispose();

                        if (components != null)
                            components.Dispose();
                    }

                    System.IO.Directory.Delete(Classes.Utility.TempPath, true);
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        ~Main()
        {
            Dispose(false);
        }
        #endregion
        
        private void UpdateApplicationsTable()
        {
            dgvApplications.Rows.Clear();
            List<object[]> apps = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "id", "entrant_id", "registration_time", "registrator_login", "edit_time", "status_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampaignID)
                });
            if (apps.Count > 0)
                foreach (var application in apps)
                {
                    object[] names = _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, new string[] { "last_name", "first_name", "middle_name" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)application[1])
                        })[0];
                    var appDocuments = _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, (uint)application[0])
                    });
                    dgvApplications.Rows.Add(application[0], names[0], names[1], names[2], null, null, application[2] as DateTime?, application[4] as DateTime?, null, null, null,
                        application[3], _DB_Helper.GetDictionaryItemName(FIS_Dictionary.APPLICATION_STATUS,(uint)application[5]));
                    foreach (var document in _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "id", "type", "original_recieved_date" }).Join(
                        appDocuments,
                        docs => docs[0],
                        appdocs => appdocs[0],
                        (s1, s2) => new
                        {
                            Type = s1[1].ToString(),
                            OriginalRecievedDate = s1[2] as DateTime?
                        }
                        ).Select(s => new { Value = new Tuple<string, DateTime?>(s.Type, s.OriginalRecievedDate) }).ToList())
                        if (((document.Value.Item1 == "school_certificate") || (document.Value.Item1 == "high_edu_diploma") || (document.Value.Item1 == "academic_diploma")) && (document.Value.Item2!=null))
                            dgvApplications.Rows[dgvApplications.Rows.Count-1].Cells[dgvApplications_Original.Index].Value = true;
                    //foreach (object[] entrance in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "is_agreed_date", "is_disagreed_date" }, new List<Tuple<string, Relation, object>>
                    //{
                    //    new Tuple<string, Relation, object>("application_id", Relation.EQUAL, (uint)application[0])
                    //}))
                    //{

                    //}
                }
            dgvApplications.Sort(dgvApplications_LastName, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void menuStrip_Campaign_Campaigns_Click(object sender, EventArgs e)
        {
            Campaigns form = new Campaigns(_DB_Connection);
            form.ShowDialog();
            SetCurrentCampaign();
        }

        private void menuStrip_CreateApplication_Click(object sender, EventArgs e)
        {
            if (_CurrCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                ApplicationEdit form = new ApplicationEdit(_DB_Connection, _CurrCampaignID, _UserLogin, null);
                form.ShowDialog();
                UpdateApplicationsTable();
            }
        }

        private void toolStrip_CreateApplication_Click(object sender, EventArgs e)
        {
            if (_CurrCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                ApplicationEdit form = new ApplicationEdit(_DB_Connection, _CurrCampaignID, _UserLogin, null);
                form.ShowDialog();
                UpdateApplicationsTable();
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
            form.ShowDialog();
        }

        private void menuStrip_DirDictionary_Click(object sender, EventArgs e)
        {
            DirectionsDictionary form = new DirectionsDictionary(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_OlympDictionary_Click(object sender, EventArgs e)
        {
            OlympicsDictionary form = new OlympicsDictionary(_DB_Connection);
            form.ShowDialog();
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
            Examinations form = new Examinations(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_Users_Click(object sender, EventArgs e)
        {
            Users form = new Users(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_FIS_Export_Click(object sender, EventArgs e)
        {
            Classes.Utility.ConnectToFIS("****")?.Export(Classes.FIS_Packager.MakePackage(_DB_Connection));
        }

        private void menuStrip_InstitutionAchievements_Click(object sender, EventArgs e)
        {
            if (_CurrCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                InstitutionAchievementsEdit form = new InstitutionAchievementsEdit(_DB_Connection);
                form.ShowDialog();
            }
        }

        private void menuStrip_Orders_Click(object sender, EventArgs e)
        {
            Orders form = new Orders(_DB_Connection);
            form.ShowDialog();
            UpdateApplicationsTable();
        }

        private void dgvApplications_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_CurrCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                if (!(bool)_DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "master_appl" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)dgvApplications.SelectedRows[0].Cells[0].Value)
                })[0][0])
                {
                    ApplicationEdit form = new ApplicationEdit(_DB_Connection, _CurrCampaignID, _UserLogin, (uint)dgvApplications.SelectedRows[0].Cells[0].Value);
                    form.ShowDialog();
                }
                else
                {
                    ApplicationMagEdit form = new ApplicationMagEdit(_DB_Connection, _CurrCampaignID, _UserLogin, (uint)dgvApplications.SelectedRows[0].Cells[0].Value);
                    form.ShowDialog();
                }
                UpdateApplicationsTable();
            }
        }

        private void menuStrip_Constants_Click(object sender, EventArgs e)
        {
            if (_CurrCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                Constants form = new Constants(_DB_Connection, _CurrCampaignID);
                form.ShowDialog();
            }
        }

        private void toolStrip_RegJournal_Click(object sender, EventArgs e)
        {
            DateChoice form = new DateChoice();
            form.ShowDialog();
            Classes.OutDocuments.RegistrationJournal(_DB_Connection, form.dateTimePicker.Value);
        }

        private void menuStrip_CreateMagApplication_Click(object sender, EventArgs e)
        {
            if (_CurrCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                ApplicationMagEdit form = new ApplicationMagEdit(_DB_Connection, _CurrCampaignID, _UserLogin, null);
                form.ShowDialog();
                UpdateApplicationsTable();
            }
        }

        private void toolStripMain_CreateMagApplication_Click(object sender, EventArgs e)
        {
            if (_CurrCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                ApplicationMagEdit form = new ApplicationMagEdit(_DB_Connection, _CurrCampaignID, _UserLogin, null);
                form.ShowDialog();
                UpdateApplicationsTable();
            }
        }

        private void menuStrip_KLADR_Update_Click(object sender, EventArgs e)
        {
            KLADR_Update form = new KLADR_Update(_DB_Connection.User, _DB_Connection.Password);
            form.ShowDialog();
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
                FilterAppsTable();
        }

        private void cbDateSearch_CheckedChanged(object sender, EventArgs e)
        {
            dtpRegDate.Enabled = cbDateSearch.Checked;
            if (!dtpRegDate.Enabled)
                dtpRegDate.Value = dtpRegDate.MinDate;
            else
                dtpRegDate.Value = DateTime.Now;
        }

        private void cbFilter_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
                if ((sender as CheckBox).Text == "Новые")
                {
                    cbPickUp.Checked = false;
                    cbEnroll.Checked = false;
                }
            else if ((sender as CheckBox).Checked)
                    if ((sender as CheckBox).Text == "Зачисленные")
                    {
                        cbNew.Checked = false;
                        cbPickUp.Checked = false;                        
                    }
            else
                    {
                        cbNew.Checked = false;
                        cbEnroll.Checked = false;
                    }

            foreach (DataGridViewRow row in dgvApplications.Rows)
                if (cbNew.Checked && (row.Cells[dgvApplications_Status.Index].Value.ToString() != "Новое"))
                    row.Visible = false;
                else if (cbPickUp.Checked && (row.Cells[dgvApplications_Status.Index].Value.ToString() != "Отозвано"))
                    row.Visible = false;
                else if (cbEnroll.Checked && (row.Cells[dgvApplications_Status.Index].Value.ToString() != "Принято"))
                    row.Visible = false;
                else row.Visible = true;
        }

        private void FilterAppsTable()
        {
            foreach (DataGridViewRow row in dgvApplications.Rows)
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
        }

        private void SetCurrentCampaign()
        {
            List<object[]> currCampaigns = _DB_Connection.Select(DB_Table.CONSTANTS, "current_campaign_id");
            if (currCampaigns.Count > 0)
            {
                _CurrCampaignID = (uint)currCampaigns[0][0];
                UpdateApplicationsTable();
            }
            else
                _CurrCampaignID = 0;
        }
    }
}
