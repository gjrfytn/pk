using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class DirectionsProfiles : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        private List<object[]> _Directions;
        private List<string> _Faculties;

        public DirectionsProfiles(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            cbDirections.ValueMember = "Value";
        }

        private void UpdateTable()
        {
            dgvDirections.Rows.Clear();
            _Directions = new List<object[]>();

            foreach (object[] v in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code"))
            {
                if (v[2].ToString().Substring(3, 2) == "03")
                {
                    _Directions.Add(new object[] { v[1], v[2], "Бакалавриат", v[0] });
                    dgvDirections.Rows.Add(v[0], "Н", v[1], "", v[2], "Бакалавриат");
                }
                else if (v[2].ToString().Substring(3, 2) == "05")
                {
                    _Directions.Add(new object[] { v[1], v[2], "Специалитет", v[0] });
                    dgvDirections.Rows.Add(v[0], "Н", v[1], "", v[2], "Специалитет");
                }
                else if (v[2].ToString().Substring(3, 2) == "04")
                {
                    _Directions.Add(new object[] { v[1], v[2], "Магистратура", v[0] });
                    dgvDirections.Rows.Add(v[0], "Н", v[1], "", v[2], "Магистратура");
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
            dgvDirections.Sort(dgvDirections_Code, System.ComponentModel.ListSortDirection.Ascending);

            foreach (object[] v in _DB_Connection.Select(DB_Table.PROFILES, "name", "direction_id", "faculty_short_name", "short_name"))
                for (int i = 0; i < dgvDirections.Rows.Count; i++)
                {
                    if ((dgvDirections.Rows[i].Cells[1].Value.ToString() == "Н") && (dgvDirections.Rows[i].Cells[0].Value.ToString() == v[1].ToString()))
                        dgvDirections.Rows.Insert(i + 1, v[1], "П", v[0], v[3].ToString(),
                            dgvDirections.Rows[i].Cells[4].Value, dgvDirections.Rows[i].Cells[5].Value, v[2]);
                }

            _Faculties = new List<string>();
            foreach (object[] v in _DB_Connection.Select(DB_Table.FACULTIES, "short_name"))
                _Faculties.Add(v[0].ToString());
            cbFaculties.DataSource = _Faculties;
        }

        private void FillDirCombobox(string filter)
        {
            List<object[]> directionsData = new List<object[]>();
            foreach (var record in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code"))
                if (record[2].ToString().Split('.')[1] == filter)
                    directionsData.Add(record);

            cbDirections.DataSource = _DB_Connection.Select(DB_Table.DIRECTIONS, new string[] { "direction_id", "faculty_short_name" }).Join(
                directionsData,
                campDirs => campDirs[0],
                dirsData => dirsData[0],
                (s1, s2) => new
                {
                    Id = (uint)s1[0],
                    Faculty = s1[1].ToString(),
                    Name = s2[1].ToString(),
                    Code = s2[2]
                }).Select(s => new
                {
                    Value = new Tuple<uint, string, string>(s.Id, s.Faculty, s.Name),
                    Display = s.Code + " " + s.Name
                }).ToList();
            cbDirections.DisplayMember = "Display";
        }

        private void FillFacultiesCombobox()
        {
            cbFaculties.Items.Clear();
            if (cbDirections.SelectedIndex != -1)
            foreach (object[] faculty in _DB_Connection.Select(DB_Table.FACULTIES, "short_name"))
                if ((cbDirections.SelectedValue as Tuple<uint, string, string>).Item2 == faculty[0].ToString())
                    cbFaculties.Items.Add(faculty[0].ToString());

            cbFaculties.Text = "";
        }

        private void EnableDisableControls(bool enable)
        {
            if (enable)
            {
                gbType.Enabled = true;
                cbDirections.Enabled = true;
                cbFaculties.Enabled = true;
                label1.Enabled = true;
                label2.Enabled = true;
                label3.Enabled = true;
                label4.Enabled = true;
                tbName.Enabled = true;
                tbShortName.Enabled = true;
                btSave.Enabled = true;
            }
            else
            {
                gbType.Enabled = false;
                cbDirections.Enabled = false;
                cbFaculties.Enabled = false;
                label1.Enabled = false;
                label2.Enabled = false;
                label3.Enabled = false;
                label4.Enabled = false;
                tbName.Enabled = false;
                tbName.Clear();
                tbShortName.Enabled = true;
                tbShortName.Clear();
                btSave.Enabled = false;
            }
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
            else
            {
                _DB_Connection.Insert(DB_Table.PROFILES, new Dictionary<string, object> { { "faculty_short_name", cbFaculties.SelectedItem.ToString() },
                    { "direction_id", (cbDirections.SelectedValue as Tuple<uint, string, string>).Item1 },
                    { "name", tbName.Text}, { "short_name", tbShortName.Text } });

                EnableDisableControls(false);
                UpdateTable();
            }
        }

        private void btAddProfile_Click(object sender, EventArgs e)
        {
            EnableDisableControls(true);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if ((dgvDirections.SelectedRows.Count == 0) || (dgvDirections.SelectedRows[0].Cells[1].Value.ToString() == "Н"))
                MessageBox.Show("Выберите профиль");
            else
            {
                _DB_Connection.Delete(DB_Table.PROFILES, new Dictionary<string, object>
                { { "faculty_short_name", dgvDirections.SelectedRows[0].Cells[5].Value}, { "direction_id", dgvDirections.SelectedRows[0].Cells[0].Value},
                    { "name", dgvDirections.SelectedRows[0].Cells[2].Value} });
                UpdateTable();
            }
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBacc.Checked)
                FillDirCombobox("03");
            else if (rbMag.Checked)
                FillDirCombobox("04");
            else if (rbSpec.Checked)
                FillDirCombobox("05");
        }

        private void cbDirections_SelectedValueChanged(object sender, EventArgs e)
        {
            FillFacultiesCombobox();
        }
    }
}
