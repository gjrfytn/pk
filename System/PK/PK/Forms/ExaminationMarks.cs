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

            var marks = _DB_Connection.Select(
                DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                new string[] { dataGridView_UID.DataPropertyName, dataGridView_Mark.DataPropertyName },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("examination_id", Relation.EQUAL, examinationID) }
                ).GroupJoin(
                _DB_Connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "id", "entrant_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID), }
                    ),
                k1 => k1[0],
                k2 => k2[1],
                (s1, s2) => new
                {
                    ID = (uint)s1[0],
                    ApplIDs = string.Join(", ", s2.Select(s => s[0].ToString())),
                    Mark = (short)s1[1]
                });

            var table = marks.Join(
                _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                k1 => k1.ID,
                k2 => k2[0],
                (s1, s2) => new
                {
                    s1.ID,
                    s1.ApplIDs,
                    s1.Mark,
                    Name = s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString()
                });

            foreach (var row in table)
                dataGridView.Rows.Add(row.ID, row.ApplIDs, row.Name, row.Mark);

            dataGridView.Sort(dataGridView_Name, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void toolStrip_Print_Click(object sender, EventArgs e)
        {
            if (!TryApplyCellChanges())
                return;

            Cursor.Current = Cursors.WaitCursor;

            if (dataGridView.IsCurrentCellDirty)
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

            Dictionary<uint, string> subjConsts = new Dictionary<uint, string>
            {
                { 1, "русскому языку" },
                { 2, "математике" },
                { 6, "иностранному языку" },
                { 9, "обществознанию" },
                { 10, "физике"}
            };

            string[] singleParams = new string[]
            {
                new Classes.DB_Helper(_DB_Connection).GetMinMark(_SubjectID).ToString(),
                _Date.Year.ToString(),
                _Date.ToShortDateString(),
                new Classes.DB_Helper(_DB_Connection).GetDictionaryItemName(FIS_Dictionary.SUBJECTS,_SubjectID),
                subjConsts[_SubjectID]
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
            Classes.DocumentCreator.Create(Classes.Settings.DocumentsTemplatesPath + "AlphaMarks.xml", doc, singleParams, new IEnumerable<string[]>[] { table.OrderBy(s => s[1]) });
            System.Diagnostics.Process.Start(doc + ".docx");

            Cursor.Current = Cursors.Default;
        }

        private void toolStrip_Clear_Click(object sender, EventArgs e)
        {
            if (!TryApplyCellChanges())
                return;

            if (Classes.Utility.ShowUnrevertableActionMessageBox())
            {
                _DB_Connection.Delete(DB_Table.ENTRANTS_EXAMINATIONS_MARKS, new Dictionary<string, object> { { "examination_id", _ExaminationID } });
                dataGridView.Rows.Clear();
            }
        }

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == dataGridView_Mark.Index)
            {
                short mark;
                if (!short.TryParse(e.FormattedValue.ToString(), out mark) || mark > 100 || mark < -1)
                {
                    MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
                _DB_Connection.Update(DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                    new Dictionary<string, object> { { dataGridView_Mark.DataPropertyName, dataGridView[e.ColumnIndex, e.RowIndex].Value } },
                    new Dictionary<string, object>
                    {
                        { dataGridView_UID.DataPropertyName,dataGridView[dataGridView_UID.Index,e.RowIndex].Value },
                        { "examination_id", _ExaminationID}
                    });
        }

        private void ExaminationMarks_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !TryApplyCellChanges();
        }

        private bool TryApplyCellChanges()
        {
            if (dataGridView.IsCurrentCellDirty)
                try
                {
                    dataGridView.CurrentCell = null;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }

            return true;
        }
    }
}
