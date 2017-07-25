using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class Examinations : Form
    {
        private readonly DB_Connector _DB_Connection;
        private readonly DB_Helper _DB_Helper;

        private uint SelectedExamID
        {
            get { return (uint)dataGridView.SelectedRows[0].Cells[dataGridView_ID.Index].Value; }
        }

        public Examinations(DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            // Для отображения без компонента времени:
            dataGridView_Date.ValueType = typeof(DateTime);
            dataGridView_RegStartDate.ValueType = typeof(DateTime);
            dataGridView_RegEndDate.ValueType = typeof(DateTime);
            #endregion

            _DB_Connection = connection;
            _DB_Helper = new DB_Helper(_DB_Connection);

            UpdateTable();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            ToggleButtons();
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (DB_Queries.ExaminationHasMarks(_DB_Connection, (uint)e.Row.Cells[dataGridView_ID.Index].Value))
            {
                MessageBox.Show("Невозможно удалить экзамен с распределёнными абитуриентами. Сначала очистите список оценок.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (SharedClasses.Utility.ShowUnrevertableActionMessageBox())
                _DB_Connection.Delete(DB_Table.EXAMINATIONS, new Dictionary<string, object> { { "id", e.Row.Cells[dataGridView_ID.Index].Value } });
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
            ExaminationEdit form = new ExaminationEdit(_DB_Connection, SelectedExamID);
            form.ShowDialog();
            UpdateTable();
        }

        private void toolStrip_Delete_Click(object sender, EventArgs e)
        {
            if (SharedClasses.Utility.ShowUnrevertableActionMessageBox())
            {
                _DB_Connection.Delete(DB_Table.EXAMINATIONS, new Dictionary<string, object> { { "id", SelectedExamID } });
                UpdateTable();
            }
        }

        private void toolStrip_Distribute_Click(object sender, EventArgs e)
        {
            if (DB_Queries.ExaminationHasMarks(_DB_Connection, SelectedExamID))
                MessageBox.Show("В экзамен уже включены абитуриенты. При повторном распределении они не будут удалены.");

            Cursor.Current = Cursors.WaitCursor;

            var applications = _DB_Connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "id", "registration_time", "entrant_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("passing_examinations",Relation.EQUAL, true),
                    new Tuple<string, Relation, object>("status",Relation.EQUAL,"new")
                }).Where(a => (DateTime)a[1] >= (DateTime)dataGridView.SelectedRows[0].Cells[dataGridView_RegStartDate.Index].Value &&
                (DateTime)a[1] < (DateTime)dataGridView.SelectedRows[0].Cells[dataGridView_RegEndDate.Index].Value + new TimeSpan(1, 0, 0, 0)
                ).Select(s => new { ApplID = (uint)s[0], EntrID = (uint)s[2] });

            uint subjectID = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, dataGridView.SelectedRows[0].Cells[dataGridView_Subject.Index].Value.ToString());

            IEnumerable<uint> excludedAppls = DB_Queries.GetMarks(
                _DB_Connection,
                applications.Select(s => s.ApplID),
                Classes.Settings.CurrentCampaignID
                ).Where(s =>
                s.FromExamDate.HasValue &&
                (s.SubjID == subjectID ||
                s.Value < _DB_Helper.GetMinMark(s.SubjID)
                )).Select(s => s.ApplID);

            applications = applications.Where(s => !excludedAppls.Contains(s.ApplID));

            var applsDirections = _DB_Connection.Select(
                DB_Table.APPLICATIONS_ENTRANCES,
                "application_id", "faculty_short_name", "direction_id"
                );

            var subjectDirections = _DB_Connection.Select(
                DB_Table.ENTRANCE_TESTS,
                new string[] { "direction_faculty", "direction_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Classes.Settings.CurrentCampaignID),
                    new Tuple<string, Relation, object>("subject_id",Relation.EQUAL,subjectID)
                });

            var applsSubjects = applsDirections.Join(
                subjectDirections,
                k1 => Tuple.Create(k1[1], k1[2]),
                k2 => Tuple.Create(k2[0], k2[1]),
                (s1, s2) => (uint)s1[0]
                ).Distinct();

            applications = applications.Join(
                 applsSubjects,
                 k1 => k1.ApplID,
                 k2 => k2,
                 (s1, s2) => s1
                 );

            if (applications.Count() == 0)
            {
                MessageBox.Show("Ни один абитуриент не попал в экзамен по условиям фильтрации.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var entrantsIDs = _DB_Connection.Select(DB_Table.ENTRANTS, "id").Join(
                applications,
                en => en[0],
                a => a.EntrID,
                (s1, s2) => s2.EntrID
              ).Distinct();//TODO Нужно?

            foreach (object entrID in entrantsIDs)
                _DB_Connection.InsertOnDuplicateUpdate(
                    DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                    new Dictionary<string, object> { { "entrant_id", entrID }, { "examination_id", SelectedExamID } }
                    );

            ToggleButtons();

            Cursor.Current = Cursors.Default;
        }

        private void toolStrip_Marks_Click(object sender, EventArgs e)
        {
            ExaminationMarks form = new ExaminationMarks(_DB_Connection, SelectedExamID);
            form.ShowDialog();
            ToggleButtons();
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
            dataGridView.Rows.Clear();

            Dictionary<uint, string> subjects = _DB_Helper.GetDictionaryItems(FIS_Dictionary.SUBJECTS);
            foreach (DB_Queries.Exam exam in DB_Queries.GetCampaignExams(_DB_Connection, Classes.Settings.CurrentCampaignID))
                dataGridView.Rows.Add(
                    exam.ID,
                    subjects[exam.SubjID],
                    exam.Date,
                    exam.RegStartDate,
                    exam.RegEndDate
                    );
        }

        private void ToggleButtons()
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                toolStrip_Edit.Enabled = false;
                toolStrip_Delete.Enabled = false;
                toolStrip_Distribute.Enabled = false;
                toolStrip_Marks.Enabled = false;
                toolStrip_Print.Enabled = false;
            }
            else
            {
                bool hasMarks = DB_Queries.ExaminationHasMarks(_DB_Connection, SelectedExamID);
                toolStrip_Edit.Enabled = true;
                toolStrip_Delete.Enabled = !hasMarks;
                toolStrip_Distribute.Enabled = true;
                toolStrip_Marks.Enabled = hasMarks;
                toolStrip_Print.Enabled = hasMarks;
            }
        }
    }
}
