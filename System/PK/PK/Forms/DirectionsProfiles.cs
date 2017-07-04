using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class DirectionsProfiles : Form
    {
        private readonly DB_Connector _DB_Connection;
        private readonly DB_Helper _DB_Helper;

        public DirectionsProfiles(DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new DB_Helper(_DB_Connection);
            UpdateTable();
            cbDirections.ValueMember = "Value";
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (cbDirections.SelectedIndex == -1)
                MessageBox.Show("Не выбрано направление.");
            else if (cbFaculties.SelectedIndex == -1)
                MessageBox.Show("Не выбран факультет.");
            else if (tbName.Text == "")
                MessageBox.Show("Не указано название профиля.");
            else if (tbShortName.Text == "")
                MessageBox.Show("Не указано сокращенное название профиля.");
            else if (rbMag.Checked && tbKafedra.Text == "")
                MessageBox.Show("Не указана кафедра.");
            else if (_DB_Connection.Select(DB_Table.PROFILES, new string[] { "direction_id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("short_name", Relation.EQUAL, tbShortName.Text)
            }).Count > 0)
                MessageBox.Show("Профиль/программа с таким сокращением уже существует.");
            else
            {
                if (rbMag.Checked)
                {
                    _DB_Connection.Insert(DB_Table.PROFILES, new Dictionary<string, object> { { "faculty_short_name", cbFaculties.SelectedItem.ToString() },
                    { "direction_id", (cbDirections.SelectedValue as Tuple<uint, string>).Item1 },
                    { "name", tbName.Text + "|" + tbKafedra.Text }, { "short_name", tbShortName.Text } });
                }
                else
                {
                    _DB_Connection.Insert(DB_Table.PROFILES, new Dictionary<string, object> { { "faculty_short_name", cbFaculties.SelectedItem.ToString() },
                    { "direction_id", (cbDirections.SelectedValue as Tuple<uint, string>).Item1 },
                    { "name", tbName.Text}, { "short_name", tbShortName.Text } });
                }
                tbKafedra.Text = "";
                tbName.Text = "";
                tbShortName.Text = "";
                btAddProfile.Enabled = true;
                EnableDisableControls(false);
                UpdateTable();
            }
        }

        private void btAddProfile_Click(object sender, EventArgs e)
        {
            EnableDisableControls(true);
            if (dgvDirections.SelectedRows.Count != 0)
            {
                if (dgvDirections.SelectedRows[0].Cells[dgvDirections_EduLevel.Index].Value.ToString() == DB_Helper.EduLevelB)
                    rbBacc.Checked = true;
                else if (dgvDirections.SelectedRows[0].Cells[dgvDirections_EduLevel.Index].Value.ToString() == DB_Helper.EduLevelS)
                    rbSpec.Checked = true;
                else if (dgvDirections.SelectedRows[0].Cells[dgvDirections_EduLevel.Index].Value.ToString() == DB_Helper.EduLevelM)
                    rbMag.Checked = true;
                cbDirections.SelectedIndex = cbDirections.FindString(_DB_Helper.GetDirectionNameAndCode((uint)dgvDirections.SelectedRows[0].Cells[dgvDirections_ID.Index].Value).Item2);
                btAddProfile.Enabled = false;
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if ((dgvDirections.SelectedRows.Count == 0) || (dgvDirections.SelectedRows[0].Cells[dgvDirections_Type.Index].Value.ToString() == "Н"))
                MessageBox.Show("Выберите профиль");
            else if (SharedClasses.Utility.ShowChoiceMessageBox("Удалить выбранный профиль?", "Удаление профиля"))
            {
                try
                {
                    _DB_Connection.Delete(DB_Table.PROFILES, new Dictionary<string, object>
                { { "faculty_short_name", dgvDirections.SelectedRows[0].Cells[dgvDirections_FacultyName.Index].Value },
                    { "direction_id", dgvDirections.SelectedRows[0].Cells[dgvDirections_ID.Index].Value },
                    { "short_name", dgvDirections.SelectedRows[0].Cells[dgvDirections_ShortName.Index].Value } });
                    UpdateTable();
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1217 || ex.Number == 1451)
                    {
                        List<object[]> appEntrances = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "application_id" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, dgvDirections.SelectedRows[0].Cells[dgvDirections_FacultyName.Index].Value),
                            new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, dgvDirections.SelectedRows[0].Cells[dgvDirections_ID.Index].Value),
                            new Tuple<string, Relation, object>("profile_short_name", Relation.EQUAL, dgvDirections.SelectedRows[0].Cells[dgvDirections_ShortName.Index].Value)
                        });
                        if (appEntrances.Count > 0)
                            MessageBox.Show("На данный профиль подано заявление. Удаление невозможно.");
                        else if (SharedClasses.Utility.ShowChoiceMessageWithConfirmation("Профиль включен в кампанию. Выполнить удаление?", "Связь с кампанией"))
                        {
                            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
                            {
                                _DB_Connection.Delete(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object>
                            { { "profiles_direction_faculty", dgvDirections.SelectedRows[0].Cells[dgvDirections_FacultyName.Index].Value },
                                { "profiles_direction_id", dgvDirections.SelectedRows[0].Cells[dgvDirections_ID.Index].Value },
                                { "profiles_short_name", dgvDirections.SelectedRows[0].Cells[dgvDirections_ShortName.Index].Value } }, transaction);

                            _DB_Connection.Delete(DB_Table.PROFILES, new Dictionary<string, object>
                                { { "faculty_short_name", dgvDirections.SelectedRows[0].Cells[dgvDirections_FacultyName.Index].Value },
                                { "direction_id", dgvDirections.SelectedRows[0].Cells[dgvDirections_ID.Index].Value },
                                { "short_name", dgvDirections.SelectedRows[0].Cells[dgvDirections_ShortName.Index].Value } }, transaction);

                                transaction.Commit();
                            }

                            UpdateTable();
                        }
                    }
                }
            }
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBacc.Checked)
            {
                FillDirCombobox("03");
                tbKafedra.Enabled = false;
                label5.Enabled = false;
                tbName.MaxLength = 300;
            }                
            else if (rbMag.Checked)
            {
                FillDirCombobox("04");
                tbKafedra.Enabled = true;
                label5.Enabled = true;
                tbName.MaxLength = 150;
            }                
            else if (rbSpec.Checked)
            {
                FillDirCombobox("05");
                tbKafedra.Enabled = false;
                label5.Enabled = false;
                tbName.MaxLength = 300;
            } 
        }

        private void dgvDirections_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ((dgvDirections.CurrentRow.Cells[dgvDirections_Kafedra.Index].Value == null) || (dgvDirections.CurrentRow.Cells[dgvDirections_Kafedra.Index].Value.ToString() == ""))
                _DB_Connection.Update(DB_Table.PROFILES, new Dictionary<string, object> { { "name", dgvDirections.CurrentRow.Cells[dgvDirections_Name.Index].Value.ToString() } },
                    new Dictionary<string, object> { { "faculty_short_name", dgvDirections.CurrentRow.Cells[dgvDirections_FacultyName.Index].Value.ToString() },
                        { "direction_id", (uint)dgvDirections.CurrentRow.Cells[dgvDirections_ID.Index].Value }, { "short_name", dgvDirections.CurrentRow.Cells[dgvDirections_ShortName.Index].Value.ToString() } });
            else
                _DB_Connection.Update(DB_Table.PROFILES, new Dictionary<string, object> { { "name", dgvDirections.CurrentRow.Cells[dgvDirections_Name.Index].Value.ToString()
                        + "|" + dgvDirections.CurrentRow.Cells[dgvDirections_Kafedra.Index].Value.ToString() } },
                    new Dictionary<string, object> { { "faculty_short_name", dgvDirections.CurrentRow.Cells[dgvDirections_FacultyName.Index].Value.ToString() },
                        { "direction_id", (uint)dgvDirections.CurrentRow.Cells[dgvDirections_ID.Index].Value }, { "short_name", dgvDirections.CurrentRow.Cells[dgvDirections_ShortName.Index].Value.ToString() } });
        }

        private void cbDirections_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillFacultiesCombobox();
        }

        private void UpdateTable()
        {
            dgvDirections.Rows.Clear();

            foreach (object[] v in _DB_Connection.Select(DB_Table.DIRECTIONS, "direction_id"))
            {
                Tuple<string, string> dirData;

                bool found = false;
                foreach (DataGridViewRow row in dgvDirections.Rows)
                    if (row.Cells[dgvDirections_ID.Index].Value.ToString() == v[0].ToString())
                        found = true;
                if (!found)
                {
                    dirData = _DB_Helper.GetDirectionNameAndCode((uint)v[0]);
                    if (dirData.Item2.Substring(3, 2) == "03")
                    {
                        dgvDirections.Rows.Add(v[0], "Н", dirData.Item1, "", dirData.Item2, "Бакалавриат");
                    }
                    else if (dirData.Item2.Substring(3, 2) == "05")
                    {
                        dgvDirections.Rows.Add(v[0], "Н", dirData.Item1, "", dirData.Item2, "Специалитет");
                    }
                    else if (dirData.Item2.Substring(3, 2) == "04")
                    {
                        dgvDirections.Rows.Add(v[0], "Н", dirData.Item1, "", dirData.Item2, "Магистратура");
                    }
                    if (dgvDirections.Rows.Count != 0)
                        for (int i = 0; i < dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells.Count; i++)
                        {
                            dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells[i].Style.Font = new Font(
                                dgvDirections.Font.Name.ToString(),
                                dgvDirections.Font.Size + 1,
                                FontStyle.Bold);
                            dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells[i].Style.BackColor = Color.LightGray;
                        }
                }
            }
            foreach (DataGridViewRow row in dgvDirections.Rows)
                row.ReadOnly = true;
            dgvDirections.Sort(dgvDirections_Code, System.ComponentModel.ListSortDirection.Ascending);

            foreach (object[] v in _DB_Connection.Select(DB_Table.PROFILES, "name", "direction_id", "faculty_short_name", "short_name"))
                for (int i = 0; i < dgvDirections.Rows.Count; i++)
                {
                    if ((dgvDirections.Rows[i].Cells[dgvDirections_Type.Index].Value.ToString() == "Н")
                        && (dgvDirections.Rows[i].Cells[dgvDirections_ID.Index].Value.ToString() == v[1].ToString()))
                        if (dgvDirections.Rows[i].Cells[dgvDirections_EduLevel.Index].Value.ToString() != DB_Helper.EduLevelM)
                            dgvDirections.Rows.Insert(i + 1, v[1], "П", v[0], v[3].ToString(),
                            dgvDirections.Rows[i].Cells[4].Value, dgvDirections.Rows[i].Cells[5].Value, v[2]);                    
                        else
                            dgvDirections.Rows.Insert(i + 1, v[1], "П", v[0].ToString().Split('|')[0], v[3].ToString(),
                            dgvDirections.Rows[i].Cells[4].Value, dgvDirections.Rows[i].Cells[5].Value, v[2], v[0].ToString().Split('|')[1]);
                }
        }

        private void FillDirCombobox(string filter)
        {
            List<object[]> directionsData = new List<object[]>();
            foreach (var record in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code"))
                if (record[2].ToString().Split('.')[1] == filter)
                    directionsData.Add(record);

            cbDirections.DataSource = _DB_Connection.Select(DB_Table.DIRECTIONS, new string[] { "direction_id" }).Join(
                directionsData,
                campDirs => campDirs[0],
                dirsData => dirsData[0],
                (s1, s2) => new
                {
                    Id = (uint)s1[0],
                    Name = s2[1].ToString(),
                    Code = s2[2]
                }).Select(s => new
                {
                    Value = new Tuple<uint, string>(s.Id, s.Name),
                    Display = s.Code + " " + s.Name
                }).Distinct().ToList();
            cbDirections.DisplayMember = "Display";
        }

        private void FillFacultiesCombobox()
        {
            cbFaculties.Items.Clear();
            if (cbDirections.SelectedValue != null)
                foreach (object[] direction in _DB_Connection.Select(DB_Table.DIRECTIONS, new string[] { "direction_id", "faculty_short_name" }))
                    if ((cbDirections.SelectedValue as Tuple<uint, string>).Item1 == (uint)direction[0])
                        cbFaculties.Items.Add(direction[1]);
            cbFaculties.Text = "";
        }

        private void EnableDisableControls(bool enable)
        {
            gbType.Enabled = enable;
            cbDirections.Enabled = enable;
            cbFaculties.Enabled = enable;
            label1.Enabled = enable;
            label2.Enabled = enable;
            label3.Enabled = enable;
            label4.Enabled = enable;
            tbName.Enabled = enable;
            tbShortName.Enabled = enable;
            btSave.Enabled = enable;
            if (!enable)
            {
                tbKafedra.Enabled = false;
                label5.Enabled = false;
            }
        }
    }
}
