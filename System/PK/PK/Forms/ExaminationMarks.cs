using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace PK.Forms
{
    partial class ExaminationMarks : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly uint _ExaminationID;
        private readonly uint _SubjectID;
        private readonly DateTime _Date;

        public ExaminationMarks(Classes.DB_Connector connection, uint examinationID)
        {
            #region Components
            InitializeComponent();

            dataGridView_UID.ValueType = typeof(uint);
            dataGridView_Mark.ValueType = typeof(short); //sbyte почему-то превращается в short при значении -1 в ячейке.
            #endregion

            _DB_Connection = connection;
            _ExaminationID = examinationID;

            object[] exam = _DB_Connection.Select(
                DB_Table.EXAMINATIONS,
                new string[] { "subject_dict_id", "subject_id", "date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL,examinationID)
                })[0];

            _SubjectID = (uint)exam[1];
            _Date = (DateTime)exam[2];
            Text = new Classes.DB_Helper(_DB_Connection).GetDictionaryItemName((FIS_Dictionary)exam[0], _SubjectID) + " " + _Date.ToShortDateString();

            foreach (object[] row in _DB_Connection.Select(
                DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                new string[] { dataGridView_UID.DataPropertyName, dataGridView_Mark.DataPropertyName },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id", Relation.EQUAL,examinationID)
                }
                ))
            {
                object[] entrant = _DB_Connection.Select(
                    DB_Table.ENTRANTS_VIEW,
                    new string[] { "last_name", "first_name", "middle_name" },
                    new List<Tuple<string, Relation, object>> { Tuple.Create("id", Relation.EQUAL, row[0]) }
                    )[0];
                dataGridView.Rows.Add(row[0], entrant[0].ToString() + " " + entrant[1].ToString() + " " + entrant[2].ToString(), row[1]);
            }

            dataGridView.Sort(dataGridView_Name, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void toolStrip_Print_Click(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

            Dictionary<uint, string> minMarksConsts = new Dictionary<uint, string>
            {
                { 1,"min_russian_mark" },
                { 2,"min_math_mark" },
                { 6,"min_foreign_mark" },
                { 9,"min_social_mark" },
                { 10,"min_physics_mark" }
            };

            string[] singleParams = new string[]
            {
                _DB_Connection.Select(DB_Table.CONSTANTS,minMarksConsts[_SubjectID])[0][0].ToString(),
                _Date.Year.ToString(),
                _Date.ToShortDateString(),
                new Classes.DB_Helper(_DB_Connection).GetDictionaryItemName(FIS_Dictionary.SUBJECTS,_SubjectID)
            };

            List<string[]> table = new List<string[]>(dataGridView.Rows.Count);
            foreach (DataGridViewRow row in dataGridView.Rows)
                table.Add(new string[]
                {
                    row.Cells[dataGridView_UID.Index].Value.ToString(),
                    row.Cells[dataGridView_Name.Index].Value.ToString(),
                    ((short)row.Cells[dataGridView_Mark.Index].Value==-1)?"неявка": row.Cells[dataGridView_Mark.Index].Value.ToString(),
                });

            string doc = Classes.Utility.TempPath + "AlphaMarks" + new Random().Next();
            Classes.DocumentCreator.Create(Classes.Utility.DocumentsTemplatesPath + "AlphaMarks.xml", doc, singleParams, new IEnumerable<string[]>[] { table.OrderBy(s => s[1]) });
            Classes.Utility.Print(doc + ".docx");
        }

        private void toolStrip_Clear_Click(object sender, EventArgs e)
        {
            if (Classes.Utility.ShowUnrevertableActionMessageBox())
            {
                _DB_Connection.Delete(DB_Table.ENTRANTS_EXAMINATIONS_MARKS, new Dictionary<string, object> { { "examination_id", _ExaminationID } });
                dataGridView.Rows.Clear();
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
                _DB_Connection.Update(DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                    new Dictionary<string, object>
                    {
                        {dataGridView_Mark.DataPropertyName,dataGridView[e.ColumnIndex,e.RowIndex].Value }
                    },
                    new Dictionary<string, object>
                    {
                        {dataGridView_UID.DataPropertyName,dataGridView[0,e.RowIndex].Value },
                        { "examination_id", _ExaminationID}
                    });
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ExaminationMarks_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }
}
