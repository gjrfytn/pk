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
    public partial class FaculityDirectionsSelect : Form
    {

        DB_Connector _DB_Connection;
        string facultyShortName;

        public FaculityDirectionsSelect(string shortName)
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
            facultyShortName = shortName;

            foreach (var v in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code"))
            {
                if (v[2].ToString().Substring(3, 2) == "03")
                    dgvDirections.Rows.Add(v[0].ToString(), false, v[1].ToString(),v[2].ToString(),"Бакалавриат");
                else if (v[2].ToString().Substring(3, 2) == "04")
                    dgvDirections.Rows.Add(v[0].ToString(), false, v[1].ToString(), v[2].ToString(), "Магистратура");
                else if (v[2].ToString().Substring(3, 2) == "05")
                    dgvDirections.Rows.Add(v[0].ToString(), false, v[1].ToString(), v[2].ToString(), "Специалитет");
            }

            foreach (var v in _DB_Connection.Select(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS,
                "faculties_short_name", "dictionary_10_items_id"))
                foreach (DataGridViewRow r in dgvDirections.Rows)
                    if ((r.Cells[0].Value.ToString() == v[1].ToString())&&(v[0].ToString()==facultyShortName))
                        (r.Cells[1] as DataGridViewCheckBoxCell).Value = true;

            dgvDirections.Sort(dgvDirections.Columns[4], ListSortDirection.Ascending);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            List<object[]> select = new List<object[]>();
                select = _DB_Connection.Select(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS,
                "faculties_short_name", "dictionary_10_items_id");
            bool found = false;
            foreach (DataGridViewRow r in dgvDirections.Rows)
            {                
                if ((select.Count == 0)&& ((bool)r.Cells[1].Value == true))
                    _DB_Connection.Insert(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS, new Dictionary<string, object>
                        { { "faculties_short_name", facultyShortName }, { "dictionary_10_items_id", r.Cells[0].Value } });
                else
                {
                    foreach (var v in select)
                        if ((v[0].ToString() == facultyShortName) && (v[1].ToString() == r.Cells[0].Value.ToString()))
                            found = true;

                if (((bool)r.Cells[1].Value == true)&&(!found))
                    _DB_Connection.Insert(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS, new Dictionary<string, object>
                        { { "faculties_short_name", facultyShortName }, { "dictionary_10_items_id", r.Cells[0].Value } });

                else if (((bool)r.Cells[1].Value == false)&&(found))
                        _DB_Connection.Delete(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS, new Dictionary<string, object>
                            { { "faculties_short_name", facultyShortName }, { "dictionary_10_items_id", r.Cells[0].Value } });
                }
                found = false;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
