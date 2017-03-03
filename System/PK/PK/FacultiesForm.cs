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
    public partial class FacultiesForm : Form
    {
        DB_Connector _DB_Connection;
        bool _Updating = false;

        public FacultiesForm()
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
        }

        private void UpdateTable()
        {
            dgvFaculties.Rows.Clear();
            List<object[]> tOList = new List<object[]>();
            tOList = _DB_Connection.Select(DB_Table.FACULTIES);
            foreach (var v in tOList)
                dgvFaculties.Rows.Add(v[0], v[1]);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (
                !(_DB_Connection.Select(DB_Table.FACULTIES, new string[] { "name" },
                                new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("short_name", Relation.EQUAL, tbFacultyShortName.Text)
            }).Count == 0)
            )
                if (!_Updating)
                    MessageBox.Show("Факультет с таким сокращением уже существует");
                else
                {
                    _DB_Connection.Update(DB_Table.FACULTIES, 
                        new Dictionary<string, object> { { "name", tbFacultyName.Text } }, 
                        new Dictionary<string, object> { { "short_name", tbFacultyShortName.Text } });
                    _Updating = false;
                }
            else
            {
                uint faculyUID = _DB_Connection.Insert(DB_Table.FACULTIES, 
                new Dictionary<string, object> { { "name", tbFacultyName.Text }, { "short_name", tbFacultyShortName.Text} });
            }

            UpdateTable();
            tbFacultyName.Clear();
            tbFacultyShortName.Clear();

            tbFacultyName.Visible = false;
            tbFacultyName.Enabled = false;
            tbFacultyShortName.Visible = false;
            tbFacultyShortName.Enabled = false;
            label1.Visible = false;
            label1.Enabled = false;
            label2.Visible = false;
            label2.Enabled = false;
            btSave.Visible = false;
            btSave.Enabled = false;
        }

        private void btNewFaculty_Click(object sender, EventArgs e)
        {
            tbFacultyName.Visible = true;
            tbFacultyName.Enabled = true;
            tbFacultyShortName.Visible = true;
            tbFacultyShortName.Enabled = true;
            label1.Visible = true;
            label1.Enabled = true;
            label2.Visible = true;
            label2.Enabled = true;
            btSave.Visible = true;
            btSave.Enabled = true;
        }

        private void FacultiesForm_Load(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (dgvFaculties.SelectedRows.Count == 0)
                MessageBox.Show("Выберите факультет");
            else
            {
                tbFacultyName.Visible = true;
                tbFacultyName.Enabled = true;
                tbFacultyShortName.Visible = true;
                tbFacultyShortName.Enabled = true;
                label1.Visible = true;
                label1.Enabled = true;
                label2.Visible = true;
                label2.Enabled = true;
                btSave.Visible = true;
                btSave.Enabled = true;

                tbFacultyName.Text = dgvFaculties.SelectedRows[0].Cells[1].Value.ToString();
                tbFacultyShortName.Text = dgvFaculties.SelectedRows[0].Cells[0].Value.ToString();
                _Updating = true;
            }
        }
    }
}
