using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace PK.Forms
{
    partial class Examinations : Form
    {
        private readonly Dictionary<char, string> _NameCodes = new Dictionary<char, string>
        {
            {'А',"Q" },{'Б',"W" },{'В',"E" },{'Г',"R" },{'Д',"T" },{'Е',"Y" },{'Ё',"U" },
            {'Ж',"I" },{'З',"O" },{'И',"P" },{'Й',"A" },{'К',"S" },{'Л',"D" },{'М',"F" },
            {'Н',"G" },{'О',"H" },{'П',"J" },{'Р',"K" },{'С',"L" },{'Т',"Z" },{'У',"X" },
            {'Ф',"C" },{'Х',"V" },{'Ц',"B" },{'Ч',"N" },{'Ш',"M" },{'Щ',"GQ" },{'Э',"KI" },
            {'Ю',"AC" },{'Я',"MK" }
        };

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        private uint SelectedExamID
        {
            get
            {
                return (uint)dataGridView.SelectedRows[0].Cells[0].Value;
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
            _DB_Connection.Delete(DB_Table.EXAMINATIONS, new Dictionary<string, object> { { "id", e.Row.Cells[0].Value } });
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

            var applications = _DB_Connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "entrant_id", "registration_time" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("passing_examinations",Relation.EQUAL,1)
                }
                ).Where(a => (DateTime)a[1] >= (DateTime)dataGridView.SelectedRows[0].Cells[3].Value &&
               (DateTime)a[1] < (DateTime)dataGridView.SelectedRows[0].Cells[4].Value
                );

            var entrantsIDs = _DB_Connection.Select(DB_Table.ENTRANTS, "id").Join(
                applications,
                en => en[0],
                a => a[0],
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
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите экзамен.");
                return;
            }

            var entrantsIDs = _DB_Connection.Select(
                DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                new string[] { "entrant_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,SelectedExamID)
                });

            var entrants = _DB_Connection.Select(
                DB_Table.ENTRANTS,
                new string[] { "id", "last_name", "first_name", "middle_name" }
                ).Join(
                entrantsIDs,
                en => en[0],
                i => i[0],
                (s1, s2) => new
                {
                    ID = s1[0],
                    LastName = s1[1].ToString(),
                    FirstName = s1[2].ToString(),
                    MiddleName = s1[3].ToString()
                });

            List<string[]> entrantsTable = new List<string[]>(entrants.Count());
            ushort count = 1;
            foreach (var entr in entrants)
            {
                entrantsTable.Add(new string[]
                {
                    count.ToString(),
                    entr.ID.ToString(),
                    entr.LastName+" "+entr.FirstName+" "+entr.MiddleName,
                    _NameCodes[entr.LastName[0]]+_NameCodes[entr.FirstName[0]]+"."+SelectedExamID.ToString()+count.ToString()
                });
                count++;
            }

            string examName = dataGridView.SelectedRows[0].Cells[1].Value.ToString();
            string examDate = ((DateTime)dataGridView.SelectedRows[0].Cells[2].Value).ToShortDateString();

            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "AlphaCodes.xml",
                "AlphaCodes",
                new string[] { examName, examDate },
                new List<string[]>[] { entrantsTable }
                );

            List<object[]> audiences = _DB_Connection.Select(DB_Table.EXAMINATIONS_AUDIENCES,
                new string[] { "number", "capacity" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,SelectedExamID)
                });

            List<Tuple<char, string>> distribution = Classes.Utility.DistributeAbiturients(
                audiences.ToDictionary(k => k[0].ToString(), v => (ushort)v[1]),
                entrants.Select(en => en.LastName[0]).GroupBy(en => en).ToDictionary(k => k.Key, v => (ushort)v.Count())
                );

            List<string[]> distribTable = new List<string[]>(audiences.Count);
            foreach (object[] aud in audiences)
            {
                var letters = distribution.Where(d => d.Item2 == aud[0].ToString()).Select(s => s.Item1);
                distribTable.Add(new string[]
                {
                    aud[0].ToString(),
                    letters.Any()?letters.Aggregate("",(a,d)=> a+= d+", ",s=>s.Remove(s.Length- 2)):"-",
                    aud[1].ToString(),
                    entrants.Where(en=>letters.Contains(en.LastName[0])).Count().ToString()
                });
            }

            Classes.DocumentCreator.Create(
               Classes.Utility.DocumentsTemplatesPath + "AbitAudDistrib.xml",
               "AbitAudDistrib",
               new string[]
               {
                   examName,
                   examDate,
                   entrants.Count().ToString(),
                   audiences.Sum(a=>(ushort)a[1]).ToString()
               },
               new List<string[]>[] { distribTable }
               );

            Dictionary<string, Tuple<ushort, ushort>> fill = audiences.ToDictionary(k => k[0].ToString(), v => new Tuple<ushort, ushort>(0, (ushort)v[1]));
            for (ushort i = 0; i < entrantsTable.Count; ++i)
            {
                foreach (Tuple<char, string> aud in distribution.FindAll(c => c.Item1 == entrantsTable[i][2][0]))
                {
                    if (fill[aud.Item2].Item1 < fill[aud.Item2].Item2)
                    {
                        fill[aud.Item2] = new Tuple<ushort, ushort>((ushort)(fill[aud.Item2].Item1 + 1), fill[aud.Item2].Item2);

                        entrantsTable[i] = new string[]
                        {
                            entrantsTable[i][0],
                            entrantsTable[i][1],
                            entrantsTable[i][2],
                            aud.Item2,
                            fill[aud.Item2].Item1.ToString()
                        };
                    }
                }
            }

            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "AlphaAuditories.xml",
                "AlphaAuditories",
                new string[] { examName, examDate },
                new List<string[]>[] { entrantsTable.Select(s => new string[] { s[0], s[1], s[2], s[3] }).ToList() }
                );

            System.IO.Directory.CreateDirectory("ExamCardsSheets");
            foreach (Tuple<char, string> group in distribution)
            {
                Classes.DocumentCreator.Create(
                    Classes.Utility.DocumentsTemplatesPath + "ExamCardsSheet.xml",
                    ".\\ExamCardsSheets\\ExamCardsSheet_" + group.Item1 + "_" + group.Item2,
                    new string[] { group.Item2, group.Item1.ToString(), examName, examDate },
                    new List<string[]>[]
                    {
                        entrantsTable.Where(en=>en[2][0]== group.Item1&&en[3]==group.Item2).Select(s=>new string[] {s[0],s[1],s[2],s[4] }).ToList()
                    });
            }
        }

        private void UpdateTable()
        {
            Dictionary<uint, string> subjects = _DB_Helper.GetDictionaryItems(1);

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
