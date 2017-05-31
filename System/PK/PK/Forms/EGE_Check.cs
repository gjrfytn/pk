using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using EGE_Result = System.Tuple<uint, string, string, string, string, string, System.Collections.Generic.IEnumerable<uint>>;

namespace PK.Forms
{
    partial class EGE_Check : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly IEnumerable<EGE_Result> _ApplsEgeResults;

        public EGE_Check(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;

            var applications = connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "id,entrant_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Utility.CurrentCampaignID) }
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

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog.FileName))
                {
                    foreach (EGE_Result appl in _ApplsEgeResults)
                        writer.WriteLine(appl.Item4 + "%" + appl.Item5 + "%" + appl.Item6 + "%" + appl.Item2 + "%" + appl.Item3);
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
                using (System.IO.StreamReader reader = new System.IO.StreamReader(openFileDialog.FileName))
                {
                    while (!reader.EndOfStream)
                        lines.Add(reader.ReadLine().Split('%'));
                }

                foreach (EGE_Result appl in _ApplsEgeResults)
                {
                    var applResults = lines.Where(
                        s => s[0] == appl.Item4 && s[1] == appl.Item5 && s[2] == appl.Item6 && s[3] == appl.Item2 && s[4] == appl.Item3
                        );

                    var subjects = applResults.GroupBy(
                        k => new Classes.DB_Helper(_DB_Connection).GetDictionaryItemID(FIS_Dictionary.SUBJECTS, k[5]),
                        el => new { Value = el[6], Year = el[7] },
                        (k, g) => new { Subject = k, Results = g.OrderBy(s => s.Value) }
                        );

                    foreach (var subj in subjects)
                    {
                        _DB_Connection.InsertOnDuplicateUpdate(
                            DB_Table.DOCUMENTS_SUBJECTS_DATA,
                            new Dictionary<string, object>
                            {
                                { "document_id", appl.Item1 },
                                { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                                { "subject_id", subj.Subject },
                                { "value", subj.Results.Last().Value},
                                { "year", subj.Results.Last().Year },
                                { "checked",true }
                            });
                    }
                }

                Cursor.Current = Cursors.Default;
            }
        }
    }
}
