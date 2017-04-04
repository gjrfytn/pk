using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class DirectionsProfiles : Form
    {
        Classes.DB_Connector _DB_Connection;
        List<object[]> _Directions;
        List<string> _Faculties;

        public DirectionsProfiles()
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();

            UpdateTable();
            rbBacc.Checked = true;
        }

        void UpdateTable()
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

        void EnableDisableControls(bool enable)
        {
            if (enable)
            {
                gbType.Enabled = true;
                cbDirections.Enabled = true;
                cbFaculties.Enabled = true;
                label1.Enabled = true;
                label2.Enabled = true;
                label3.Enabled = true;
                tbName.Enabled = true;
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
                tbName.Enabled = false;
                tbName.Clear();
                btSave.Enabled = false;
            }
        }

        private void rbBacc_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBacc.Checked)
            {
                cbDirections.Items.Clear();
                for (int i = 0; i < _Directions.Count; i++)
                    if (_Directions[i][2].ToString() == "Бакалавриат")
                        cbDirections.Items.Add(_Directions[i][1].ToString() + " " + _Directions[i][0].ToString());
            }
        }

        private void rbSpec_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSpec.Checked)
            {
                cbDirections.Items.Clear();
                for (int i = 0; i < _Directions.Count; i++)
                    if (_Directions[i][2].ToString() == "Специалитет")
                        cbDirections.Items.Add(_Directions[i][1].ToString() + " " + _Directions[i][0].ToString());
            }
        }

        private void rbMag_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMag.Checked)
            {
                cbDirections.Items.Clear();
                for (int i = 0; i < _Directions.Count; i++)
                    if (_Directions[i][2].ToString() == "Магистратура")
                        cbDirections.Items.Add(_Directions[i][1].ToString() + " " + _Directions[i][0].ToString());
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
                object[] temp = _Directions.Find(x => x[1].ToString() == cbDirections.SelectedItem.ToString().Substring(0, 8));
                _DB_Connection.Insert(DB_Table.PROFILES, new Dictionary<string, object>
                { { "faculty_short_name", cbFaculties.SelectedItem.ToString()}, { "direction_id",  temp[3]},  { "name", tbName.Text }, { "short_name", tbShortName.Text} });

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
    }
}
