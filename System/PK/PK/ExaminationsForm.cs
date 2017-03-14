using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace PK
{
    partial class ExaminationsForm : Form
    {
        readonly DB_Connector _DB_Connection;

        public ExaminationsForm(DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            dataGridView_Date.ValueType = typeof(DateTime);
            dataGridView_RegStartDate.ValueType = typeof(DateTime);
            dataGridView_RegEndDate.ValueType = typeof(DateTime);
            #endregion

            _DB_Connection = connection;

            UpdateTable();
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            _DB_Connection.Delete(DB_Table.EXAMINATIONS, new Dictionary<string, object> { { "id", e.Row.Cells[0].Value } });
        }

        private void toolStrip_Add_Click(object sender, EventArgs e)
        {
            ExaminationEditForm form = new ExaminationEditForm(_DB_Connection, null);
            form.ShowDialog();
            UpdateTable();
        }

        private void toolStrip_Edit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                ExaminationEditForm form = new ExaminationEditForm(_DB_Connection, (uint)dataGridView.SelectedRows[0].Cells[0].Value);
                form.ShowDialog();
                UpdateTable();
            }
            else
                MessageBox.Show("Выберите экзамен.");
        }

        private void toolStrip_Marks_Click(object sender, EventArgs e)
        {
            ExaminationMarksForm form = new ExaminationMarksForm(_DB_Connection, (uint)dataGridView.SelectedRows[0].Cells[0].Value);
            form.ShowDialog();
        }

        void UpdateTable()
        {
            Dictionary<uint, string> subjects = _DB_Connection.Select(
                DB_Table.DICTIONARIES_ITEMS,
                new string[] { "item_id", "name" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,1)
                }).ToDictionary(s1 => (uint)s1[0], s2 => s2[1].ToString());

            dataGridView.Rows.Clear();
            foreach (object[] row in _DB_Connection.Select(DB_Table.EXAMINATIONS))
                dataGridView.Rows.Add(
                    row[0],
                    subjects[(uint)row[2]],
                    row[3],
                    row[4],
                    row[5]
                    );
        }
    }
}
