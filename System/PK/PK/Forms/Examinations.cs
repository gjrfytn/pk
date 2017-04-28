using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace PK.Forms
{
    partial class Examinations : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        private uint SelectedExamID
        {
            get
            {
                return (uint)dataGridView.SelectedRows[0].Cells["dataGridView_ID"].Value;
            }
        }

        public Examinations(Classes.DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            dataGridView_Date.ValueType = typeof(DateTime);
            dataGridView_RegStartDate.ValueType = typeof(DateTime);
            dataGridView_RegEndDate.ValueType = typeof(DateTime);
            #endregion

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            UpdateTable();
        }

        private void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
                ToggleButtons();
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (ExaminationHasMarks((uint)e.Row.Cells["dataGridView_ID"].Value))
            {
                MessageBox.Show("Невозможно удалить экзамен с распределёнными абитуриентами. Сначала очистите список оценок.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (Classes.Utility.ShowUnrevertableActionMessageBox())
                _DB_Connection.Delete(DB_Table.EXAMINATIONS, new Dictionary<string, object> { { "id", e.Row.Cells["dataGridView_ID"].Value } });
            else
                e.Cancel = true;
        }

        private void toolStrip_Add_Click(object sender, EventArgs e)
        {
            ExaminationEdit form = new ExaminationEdit(_DB_Connection, null);
            form.ShowDialog();
            UpdateTable();
        }

        private void toolStrip_Edit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                ExaminationEdit form = new ExaminationEdit(_DB_Connection, SelectedExamID);
                form.ShowDialog();
                UpdateTable();
            }
            else
                MessageBox.Show("Выберите экзамен.");
        }

        private void toolStrip_Distribute_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите экзамен.");
                return;
            }

            if (ExaminationHasMarks(SelectedExamID))
                MessageBox.Show("В экзамен уже включены абитуриенты. При повторном распределении они не будут удалены.");

            var applsDirections = _DB_Connection.Select(
                DB_Table.APPLICATIONS_ENTRANCES,
                "application_id", "faculty_short_name", "direction_id"
                );

            var subjectDirections = _DB_Connection.Select(
                DB_Table.ENTRANCE_TESTS,
                new string[] { "direction_faculty", "direction_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID),
                    new Tuple<string, Relation, object>(
                        "subject_id",
                        Relation.EQUAL,
                        _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, dataGridView.SelectedRows[0].Cells["dataGridView_Subject"].Value.ToString())
                        )
                }
                );

            var applsSubjects = applsDirections.Join(
                subjectDirections,
                k1 => new Tuple<object, object>(k1[1], k1[2]),
                k2 => new Tuple<object, object>(k2[0], k2[1]),
                (s1, s2) => s1[0]
                ).Distinct();

            var applications = _DB_Connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "id", "entrant_id", "registration_time" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("passing_examinations",Relation.EQUAL,1)
                }
                ).Where(a => (DateTime)a[2] >= (DateTime)dataGridView.SelectedRows[0].Cells["dataGridView_RegStartDate"].Value &&
               (DateTime)a[2] < (DateTime)dataGridView.SelectedRows[0].Cells["dataGridView_RegEndDate"].Value
                );

            var applications2 = applications.Join(
                 applsSubjects,
                 k1 => k1[0],
                 k2 => k2,
                 (s1, s2) => s1[1]
                 );

            var entrantsIDs = _DB_Connection.Select(DB_Table.ENTRANTS, "id").Join(
                applications2,
                en => en[0],
                a => a,
                (s1, s2) => s1[0]
              ).Distinct();//TODO Нужно?

            foreach (object entrID in entrantsIDs)
                _DB_Connection.InsertOnDuplicateUpdate(
                    DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                    new Dictionary<string, object> { { "entrant_id", entrID }, { "examination_id", SelectedExamID } }
                    );

            ToggleButtons();
        }

        private void toolStrip_Marks_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                ExaminationMarks form = new ExaminationMarks(_DB_Connection, SelectedExamID);
                form.ShowDialog();
                ToggleButtons();
            }
            else
                MessageBox.Show("Выберите экзамен.");
        }

        private void toolStrip_Print_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                ExaminationDocsPrint form = new ExaminationDocsPrint(_DB_Connection, SelectedExamID);
                form.ShowDialog();
            }
            else
                MessageBox.Show("Выберите экзамен.");
        }

        private void UpdateTable()
        {
            Dictionary<uint, string> subjects = _DB_Helper.GetDictionaryItems(FIS_Dictionary.SUBJECTS);

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

        private void ToggleButtons()
        {
            bool hasMarks = ExaminationHasMarks(SelectedExamID);
            toolStrip_Edit.Enabled = !hasMarks;
            toolStrip_Marks.Enabled = hasMarks;
            toolStrip_Print.Enabled = hasMarks;
        }

        private bool ExaminationHasMarks(uint id)
        {
            return _DB_Connection.Select(
                     DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                     new string[] { "entrant_id" },
                     new List<Tuple<string, Relation, object>>
                     {
                        new Tuple<string, Relation, object> ("examination_id",Relation.EQUAL, id)
                     }).Count != 0;
        }
    }
}
