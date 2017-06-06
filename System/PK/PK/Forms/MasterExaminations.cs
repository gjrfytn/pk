using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class MasterExaminations : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;

        public MasterExaminations(Classes.DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            dataGridView_Mark.ValueType = typeof(short);
            dataGridView_Bonus.ValueType = typeof(ushort);
            dataGridView_Date.ValueType = typeof(DateTime);
            #endregion

            _DB_Connection = connection;

            var marks = _DB_Connection.Select(
                DB_Table.MASTERS_EXAMS_MARKS,
                new string[] { "entrant_id", "faculty", "direction_id", "profile_short_name", "date", "mark", "bonus" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Utility.CurrentCampaignID) }
                ).GroupJoin(
                _DB_Connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "id", "entrant_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Utility.CurrentCampaignID), }
                    ),
                k1 => k1[0],
                k2 => k2[1],
                (s1, s2) => new
                {
                    ID = (uint)s1[0],
                    ApplIDs = string.Join(", ", s2.Select(s => s[0].ToString())),
                    Mark = (short)s1[5],
                    Bonus = (ushort)s1[6],
                    Date = s1[4] as DateTime?,
                    Faculty = s1[1].ToString(),
                    Direction = (uint)s1[2],
                    Profile = s1[3].ToString()
                });

            var table = marks.Join(
                _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                k1 => k1.ID,
                k2 => k2[0],
                (s1, s2) => new
                {
                    Data = s1,
                    Name = s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString(),
                }).Join(
                _DB_Connection.Select(DB_Table.PROFILES, "faculty_short_name", "direction_id", "short_name", "name"),
                k1 => Tuple.Create(k1.Data.Faculty, k1.Data.Direction, k1.Data.Profile),
                k2 => Tuple.Create(k2[0].ToString(), (uint)k2[1], k2[2].ToString()),
                (s1, s2) => new
                {
                    s1.Data,
                    s1.Name,
                    Program = s2[3].ToString().Split('|')[0],
                    Chair = s2[3].ToString().Split('|')[1]
                });

            foreach (var row in table)
                dataGridView.Rows.Add(
                    row.Data.ID,
                    row.Data.ApplIDs,
                    row.Data.Faculty,
                    row.Data.Direction,
                    row.Data.Profile,
                    row.Name,
                    row.Data.Mark,
                    row.Data.Bonus,
                    row.Data.Date,
                    row.Chair,
                    row.Data.Profile
                    );
        }

        private void bSetDate_Click(object sender, EventArgs e)
        {
            if (Classes.Utility.ShowUnrevertableActionMessageBox())
                foreach (DataGridViewRow row in dataGridView.Rows)
                    row.Cells[dataGridView_Date.Index].Value = dtpDate.Value.Date;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                _DB_Connection.Update(
                    DB_Table.MASTERS_EXAMS_MARKS,
                    new Dictionary<string, object>
                    {
                        { "date", row.Cells[dataGridView_Date.Index].Value},
                        { "mark",  row.Cells[dataGridView_Mark.Index].Value},
                        { "bonus",  row.Cells[dataGridView_Bonus.Index].Value}
                    },
                    new Dictionary<string, object>
                    {
                        {"campaign_id" ,Classes.Utility.CurrentCampaignID},
                        { "entrant_id" ,row.Cells[dataGridView_ID.Index].Value},
                        {"faculty" ,row.Cells[dataGridView_Faculty.Index].Value},
                        {"direction_id" ,row.Cells[dataGridView_Direction.Index].Value},
                        {"profile_short_name" ,row.Cells[dataGridView_Profile.Index].Value}
                    });
            }

            Classes.Utility.ShowChangesSavedMessage();
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
            else if (e.ColumnIndex == dataGridView_Bonus.Index)
            {
                ushort bonus;
                if (!ushort.TryParse(e.FormattedValue.ToString(), out bonus) || bonus > 100) //TODO > 100?
                {
                    MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void MasterExaminations_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !Classes.Utility.ShowChangesLossMessageBox();
        }
    }
}
