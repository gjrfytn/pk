using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PK
{
    public partial class DirectionsProfilesForm : Form
    {
        DB_Connector _DB_Connection;
        List<object[]> directions;
        List<string> faculties;

        void UpdateTable()
        {
            dgvDirections.Rows.Clear();
            directions = new List<object[]>();

            foreach (var v in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS,"id", "name", "code" ))
            {
                if (v[2].ToString().Substring(3, 2) == "03")
                {
                    directions.Add(new object[] { v[1].ToString(), v[2].ToString(), "Бакалавриат", v[0] });
                    dgvDirections.Rows.Add(v[0].ToString(), "Н", v[1].ToString(), v[2].ToString(), "Бакалавриат");
                }
                else if (v[2].ToString().Substring(3, 2) == "05")
                {
                    directions.Add(new object[] { v[1].ToString(), v[2].ToString(), "Специалитет", v[0] });
                    dgvDirections.Rows.Add(v[0].ToString(), "Н", v[1].ToString(), v[2].ToString(), "Специалитет");
                }
                else if (v[2].ToString().Substring(3, 2) == "04")
                {
                    directions.Add(new object[] { v[1].ToString(), v[2].ToString(), "Магистратура", v[0] });
                    dgvDirections.Rows.Add(v[0].ToString(), "Н", v[1].ToString(), v[2].ToString(), "Магистратура");
                }
                if(!(dgvDirections.Rows.Count==0))
                    for (int i = 0; i < dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells.Count; i++)
                    {
                        dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells[i].Style.Font = new Font(
                            dgvDirections.Font.Name.ToString(),
                            dgvDirections.Font.Size + 1,
                            FontStyle.Bold);
                        dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells[i].Style.BackColor = Color.LightGray;
                    }
            }
            dgvDirections.Sort(cCode, ListSortDirection.Ascending);

            foreach (var v in _DB_Connection.Select(DB_Table.PROFILES, "name", "direction_id"))
                for (int i = 0; i < dgvDirections.Rows.Count; i++)
                {
                    if ((dgvDirections.Rows[i].Cells[1].Value.ToString()=="Н")&&(dgvDirections.Rows[i].Cells[0].Value.ToString()==v[1].ToString()))
                        dgvDirections.Rows.Insert(i+1, v[1].ToString(),"П", v[0].ToString(), 
                            dgvDirections.Rows[i].Cells[2].Value.ToString(), dgvDirections.Rows[i].Cells[3].Value.ToString());
                }

            faculties = new List<string>();
            foreach (var v in _DB_Connection.Select(DB_Table.FACULTIES, "short_name"))
                faculties.Add(v[0].ToString());
            cbFaculties.DataSource = faculties;
        }

        void EnableDisableControls (bool enable)
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

        public DirectionsProfilesForm()
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();

            UpdateTable();
            rbBacc.Checked = true;
        }

        private void rbBacc_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBacc.Checked)
            {
                cbDirections.Items.Clear();
                for (int i = 0; i < directions.Count; i++)
                    if (directions[i][2].ToString() == "Бакалавриат")
                        cbDirections.Items.Add(directions[i][1].ToString() + " " + directions[i][0].ToString());
            }
        }

        private void rbSpec_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSpec.Checked)
            {
                cbDirections.Items.Clear();
                for (int i = 0; i < directions.Count; i++)
                    if (directions[i][2].ToString() == "Специалитет")
                        cbDirections.Items.Add(directions[i][1].ToString() + " " + directions[i][0].ToString());
            }
        }

        private void rbMag_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMag.Checked)
            {
                cbDirections.Items.Clear();
                for (int i = 0; i < directions.Count; i++)
                    if (directions[i][2].ToString() == "Магистратура")
                        cbDirections.Items.Add(directions[i][1].ToString() + " " + directions[i][0].ToString());
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (cbDirections.SelectedIndex == -1)
                MessageBox.Show("Не выбрано направление");
            else if (cbFaculties.SelectedIndex == -1)
                MessageBox.Show("Не выбран факультет");
            else if (tbName.Text.Length == 0)
                MessageBox.Show("Не указано название профиля");
            else
            {
                object[] temp = directions.Find(x => x[1].ToString() == cbDirections.SelectedItem.ToString().Substring(0, 8));
                uint id = _DB_Connection.Insert(DB_Table.PROFILES, new Dictionary<string, object>
                {
                    { "direction_id",  temp[3]},
                    { "name", tbName.Text }
                });

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
            if ((dgvDirections.SelectedRows.Count == 0)||(dgvDirections.SelectedRows[0].Cells[1].Value.ToString()=="Н"))
                MessageBox.Show("Выберить профиль");
            else
            {
                _DB_Connection.Delete(DB_Table.PROFILES, new Dictionary<string, object>
                { { "direction_id", Convert.ToInt32(dgvDirections.SelectedRows[0].Cells[0].Value.ToString())}, { "name", dgvDirections.SelectedRows[0].Cells[2].Value.ToString()} });
                UpdateTable();
            }
        }
    }
}
