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
            directions = new List<object[]>();

            foreach (var v in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "code", "name","id"))
            {
                if (v[0].ToString().Substring(3, 2) == "03")
                {
                    directions.Add(new object[] { v[1].ToString(), v[0].ToString(), "Бакалавриат", v[2] });
                    dgvDirections.Rows.Add(v[1].ToString(), v[0].ToString(), "Бакалавриат", "");
                }
                else if (v[0].ToString().Substring(3, 2) == "05")
                {
                    directions.Add(new object[] { v[1].ToString(), v[0].ToString(), "Специалитет", v[2] });
                    dgvDirections.Rows.Add(v[1].ToString(), v[0].ToString(), "Специалитет", "");
                }
                else if (v[0].ToString().Substring(3, 2) == "04")
                {
                    directions.Add(new object[] { v[1].ToString(), v[0].ToString(), "Магистратура", v[2] });
                    dgvDirections.Rows.Add(v[1].ToString(), v[0].ToString(), "Магистратура", "");
                }

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

            foreach (var v in _DB_Connection.Select(DB_Table.PROFILES, "name", "direction_dir_id", "direction_faculty"))
                for (int i = 0; i < dgvDirections.Rows.Count; i--)
                {
                    if (dgvDirections.Rows[i].Cells[1].Value.ToString()==v[1].ToString())
                        dgvDirections.Rows.Insert(i + 1, v[0].ToString(), v[1].ToString(), dgvDirections.Rows[i].Cells[2].Value.ToString(), v[2].ToString());
                }

            faculties = new List<string>();
            foreach (var v in _DB_Connection.Select(DB_Table.FACULTIES, "short_name"))
                faculties.Add(v[0].ToString());
            cbFaculties.DataSource = faculties;
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
                cbDirections.SelectedIndex = 0;
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
                cbDirections.SelectedIndex = 0;
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
                cbDirections.SelectedIndex = 0;
            }
        }

        private void btAddPrifile_Click(object sender, EventArgs e)
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

        private void btSave_Click(object sender, EventArgs e)
        {
            object[] temp = directions.Find(x => x[1].ToString() == cbDirections.SelectedItem.ToString().Substring(0, 8));
            uint id = _DB_Connection.Insert(DB_Table.PROFILES, new Dictionary<string, object>
            {
                { "direction_faculty", cbFaculties.SelectedItem.ToString() },
                { "direction_dir_id",  temp[3]},
                { "name", tbName.Text }
            });

            gbType.Enabled = false;
            cbDirections.Enabled = false;
            cbFaculties.Enabled = false;
            label1.Enabled = false;
            label2.Enabled = false;
            label3.Enabled = false;
            tbName.Enabled = false;
            btSave.Enabled = false;

            UpdateTable();
        }
    }
}
