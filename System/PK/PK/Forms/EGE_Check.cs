using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class EGE_Check : Form
    {
        class EGE_Result
        {
            public readonly uint DocID;
            public readonly string Series;
            public readonly string Number;
            public readonly string LastName;
            public readonly string FirstName;
            public readonly string MiddleName;
            public readonly IEnumerable<uint> Subjects;

            public EGE_Result(uint docID, string series, string number, string lastName, string firstName, string middleName, IEnumerable<uint> subjects)
            {
                DocID = docID;
                Series = series;
                Number = number;
                LastName = lastName;
                FirstName = firstName;
                MiddleName = middleName;
                Subjects = subjects;
            }
        }

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly IEnumerable<EGE_Result> _ApplsEgeResults;

        public EGE_Check(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;

            var applications = connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "id,entrant_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID) }
                    ).Join(
                    connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                    k1 => k1[1],
                    k2 => k2[0],
                    (s1, s2) => new { ApplID = (uint)s1[0], LastN = s2[1].ToString(), FirstN = s2[2].ToString(), MiddleN = s2[3].ToString(), }
                    );

            var egeDocs = connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS).Join(
                connection.Select(
                    DB_Table.DOCUMENTS,
                    new string[] { "id", "series", "number" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("type", Relation.EQUAL, "ege") }
                    ),
                k1 => k1[1],
                k2 => k2[0],
                (s1, s2) => new { ApplID = (uint)s1[0], DocID = (uint)s2[0], Series = s2[1].ToString(), Number = s2[2].ToString() }
                );

            var marks = connection.Select(
                DB_Table.APPLICATIONS_EGE_MARKS_VIEW,
                new string[] { "applications_id", "subject_id" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("checked", Relation.EQUAL, false) }
                ).Select(s => new { ApplID = (uint)s[0], Subject = (uint)s[1] });

            _ApplsEgeResults = applications.Join(
                egeDocs,
                k1 => k1.ApplID,
                k2 => k2.ApplID,
                (s1, s2) => new { s1, s2.DocID, s2.Series, s2.Number }
                ).GroupJoin(
                marks,
                k1 => k1.s1.ApplID,
                k2 => k2.ApplID,
                (s1, s2) => new EGE_Result
                (
                    s1.DocID,
                    s1.Series,
                    s1.Number,
                    s1.s1.LastN,
                    s1.s1.FirstN,
                    s1.s1.MiddleN,
                    s2.Select(s => s.Subject)
                ));
        }

        private void bExport_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.GetEncoding(1251)))
                {
                    foreach (EGE_Result appl in _ApplsEgeResults)
                        writer.WriteLine(appl.LastName + "%" + appl.FirstName + "%" + appl.MiddleName + "%" + appl.Series + "%" + appl.Number);
                }

                Cursor.Current = Cursors.Default;
            }
        }

        private void bImport_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                List<string[]> lines = new List<string[]>();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(openFileDialog.FileName, System.Text.Encoding.GetEncoding(1251)))
                {
                    while (!reader.EndOfStream)
                        lines.Add(reader.ReadLine().Split('%'));
                }

                uint count = 0;

                foreach (EGE_Result appl in _ApplsEgeResults)
                {
                    Classes.DB_Helper dbHelper = new Classes.DB_Helper(_DB_Connection);

                    var applResults = lines.Where(
                        s => s[0] == appl.LastName && s[1] == appl.FirstName && s[2] == appl.MiddleName && s[3] == appl.Series && s[4] == appl.Number
                        ).Where(s => s[5] != "" && s[5] != "Сочинение").Select(s =>
                        {
                            if (s[5] != "Русский язык" && s[5].Contains(" язык"))
                                s[5] = "Иностранный язык";

                            return new
                            {
                                Subject = dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, s[5]),
                                Value = s[6],
                                Year = s[7]
                            };
                        });


                    var subjects = applResults.GroupBy(
                        k => k.Subject,
                        el => new { el.Value, el.Year },
                        (k, g) => new { Subject = k, Results = g.OrderBy(s => s.Value) }
                        );

                    foreach (var subj in subjects)
                    {
                        _DB_Connection.InsertOnDuplicateUpdate(
                            DB_Table.DOCUMENTS_SUBJECTS_DATA,
                            new Dictionary<string, object>
                            {
                                { "document_id", appl.DocID },
                                { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                                { "subject_id", subj.Subject },
                                { "value", subj.Results.Last().Value},
                                { "year", subj.Results.Last().Year },
                                { "checked",true }
                            });
                    }

                    count = (uint)subjects.Count();
                }

                Cursor.Current = Cursors.Default;

                MessageBox.Show("Всего результатов: " + count, "Результаты загружены", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
