using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class FaculityDirectionsSelect : Form
    {
        Classes.DB_Connector _DB_Connection;
        string _FacultyShortName;

        public FaculityDirectionsSelect(string shortName)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
            _FacultyShortName = shortName;

            foreach (object[] v in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code"))
            {
                if (v[2].ToString().Substring(3, 2) == "03")
                    dgvDirections.Rows.Add(v[0], false, v[1], v[2], "Бакалавриат");
                else if (v[2].ToString().Substring(3, 2) == "04")
                    dgvDirections.Rows.Add(v[0], false, v[1], v[2], "Магистратура");
                else if (v[2].ToString().Substring(3, 2) == "05")
                    dgvDirections.Rows.Add(v[0], false, v[1], v[2], "Специалитет");
            }

            foreach (object[] v in _DB_Connection.Select(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS,
                "faculties_short_name", "dictionary_10_items_id"))
                foreach (DataGridViewRow r in dgvDirections.Rows)
                    if ((r.Cells[0].Value.ToString() == v[1].ToString()) && (v[0].ToString() == _FacultyShortName))
                        r.Cells[1].Value = true;

            dgvDirections.Sort(dgvDirections.Columns[4], System.ComponentModel.ListSortDirection.Ascending);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            List<object[]> select = _DB_Connection.Select(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS,
                "faculties_short_name", "dictionary_10_items_id");
            bool found = false;
            foreach (DataGridViewRow r in dgvDirections.Rows)
            {
                if ((select.Count == 0) && ((bool)r.Cells[1].Value))
                    _DB_Connection.Insert(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS, new Dictionary<string, object>
                        { { "faculties_short_name", _FacultyShortName }, { "dictionary_10_items_id", r.Cells[0].Value } });
                else
                {
                    found = select.Exists(x => (x[0].ToString() == _FacultyShortName) && (x[1].ToString() == r.Cells[0].Value.ToString()));

                    if (((bool)r.Cells[1].Value) && (!found))
                        _DB_Connection.Insert(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS, new Dictionary<string, object>
                        { { "faculties_short_name", _FacultyShortName }, { "dictionary_10_items_id", r.Cells[0].Value } });

                    else if ((!(bool)r.Cells[1].Value) && (found))
                        _DB_Connection.Delete(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS, new Dictionary<string, object>
                            { { "faculties_short_name", _FacultyShortName }, { "dictionary_10_items_id", r.Cells[0].Value } });
                }
                found = false;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
