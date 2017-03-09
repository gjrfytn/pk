using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace PK
{
    partial class ExaminationsForm : Form
    {
        DB_Connector _DB_Connection;

        public ExaminationsForm(DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            dgvExams_Date.ValueType = typeof(DateTime);
            dgvExams_RegStartDate.ValueType = typeof(DateTime);
            dgvExams_RegEndDate.ValueType = typeof(DateTime);

            dgvExamsAudiences_Number.ValueType = typeof(ushort);
            dgvExamsAudiences_Capacity.ValueType = typeof(short);
            #endregion

            _DB_Connection = connection;

            Dictionary<uint, string> subjects = _DB_Connection.Select(
                DB_Table.DICTIONARIES_ITEMS,
                new string[] { "item_id", "name" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,1)
                }).ToDictionary(s1 => (uint)s1[0], s2 => s2[1].ToString());

            dgvExams_Subject.Items.AddRange(subjects.Values.ToArray());

            foreach (object[] row in _DB_Connection.Select(DB_Table.EXAMINATIONS))
                dgvExams.Rows.Add(
                    row[0],
                    subjects[(uint)row[2]],
                    row[3],
                    row[4],
                    row[5]
                    );
        }

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e) => MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void dgvExams_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dgvExamsAudiences.Rows.Clear();
            foreach (object[] row in _DB_Connection.Select(
                DB_Table.EXAMINATIONS_AUDIENCES,
                new string[] { "number", "capacity" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,dgvExams[0,e.RowIndex].Value)
                }))
                dgvExamsAudiences.Rows.Add(row[0], row[1]);
        }
    }
}
