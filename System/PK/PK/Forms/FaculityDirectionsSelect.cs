﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class FaculityDirectionsSelect : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly string _FacultyShortName;

        public FaculityDirectionsSelect(Classes.DB_Connector connection, string shortName)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _FacultyShortName = shortName;

            foreach (object[] v in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code"))
            {
                if (v[2].ToString().Substring(3, 2) == "03")
                    dgvDirections_.Rows.Add(v[0], false, v[1], v[2], "Бакалавриат");
                else if (v[2].ToString().Substring(3, 2) == "04")
                    dgvDirections_.Rows.Add(v[0], false, v[1], v[2], "Магистратура");
                else if (v[2].ToString().Substring(3, 2) == "05")
                    dgvDirections_.Rows.Add(v[0], false, v[1], v[2], "Специалитет");
            }

            foreach (object[] v in _DB_Connection.Select(DB_Table.DIRECTIONS,
                "faculty_short_name", "direction_id", "short_name"))
                foreach (DataGridViewRow r in dgvDirections_.Rows)
                    if ((r.Cells[0].Value.ToString() == v[1].ToString()) && (v[0].ToString() == _FacultyShortName))
                    {
                        r.Cells[1].Value = true;
                        r.Cells[5].Value = v[2].ToString();
                    }
                        

            dgvDirections_.Sort(dgvDirections_.Columns[4], System.ComponentModel.ListSortDirection.Ascending);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            List<object[]> select = _DB_Connection.Select(DB_Table.DIRECTIONS,
                "faculty_short_name", "direction_id");
            bool stop = false;            
            foreach (DataGridViewRow r in dgvDirections_.Rows)
            {
                bool found = false;
                if (((bool)r.Cells[1].Value) && ((r.Cells[5].Value == null)||(r.Cells[5].Value.ToString() == "")))
                {
                    MessageBox.Show("Не указано сокразение для направления " + r.Cells[3].Value + " \"" + r.Cells[2].Value + "\".");
                    stop = true;
                }
                    
                else if ((select.Count == 0) && ((bool)r.Cells[1].Value))
                    _DB_Connection.Insert(DB_Table.DIRECTIONS, new Dictionary<string, object>
                        { { "faculty_short_name", _FacultyShortName }, { "direction_id", r.Cells[0].Value }, { "short_name", r.Cells[5].Value.ToString()} });
                else
                {
                    found = select.Exists(x => (x[0].ToString() == _FacultyShortName) && (x[1].ToString() == r.Cells[0].Value.ToString()));

                    if (((bool)r.Cells[1].Value) && (!found))
                        _DB_Connection.Insert(DB_Table.DIRECTIONS, new Dictionary<string, object>
                        { { "faculty_short_name", _FacultyShortName }, { "direction_id", r.Cells[0].Value }, { "short_name", r.Cells[5].Value.ToString()} });

                    else if ((!(bool)r.Cells[1].Value) && (found))
                        _DB_Connection.Delete(DB_Table.DIRECTIONS, new Dictionary<string, object>
                            { { "faculty_short_name", _FacultyShortName }, { "direction_id", r.Cells[0].Value } });
                }
            }
            if (!stop)
                DialogResult = DialogResult.OK;
        }
    }
}
