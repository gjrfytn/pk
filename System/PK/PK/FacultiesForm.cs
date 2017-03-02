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
            uint faculyUID = _DB_Connection.Insert(DB_Table.FACULTIES, new Dictionary<string, object> { { "name", tbFacultyName.Text }, { "short_name", tbFacultyShortName.Text} });
            UpdateTable();
            tbFacultyName.Clear();
            tbFacultyShortName.Clear();
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
    }
}
