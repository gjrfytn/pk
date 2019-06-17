﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class EGE_Check : Form
    {
        class EGE_Result
        {
            public readonly uint ApplID;
            public readonly string Series;
            public readonly string Number;
            public readonly string LastName;
            //public readonly string FirstName;
            //public readonly string MiddleName;
            public readonly uint SubjectID;
            public readonly ushort Value;
            public readonly bool Checked;

            public EGE_Result(uint applID, string series, string number, string lastName/*, string firstName, string middleName*/, uint subjectID, ushort value, bool checked_)
            {
                ApplID = applID;
                Series = series;
                Number = number;
                LastName = lastName;
                //FirstName = firstName;
                //MiddleName = middleName;
                SubjectID = subjectID;
                Value = value;
                Checked = checked_;
            }
        }

        private readonly DB_Connector _DB_Connection;
        private readonly DB_Helper _DB_Helper;

        public EGE_Check(DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new DB_Helper(_DB_Connection);
        }

        private void bExport_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                var identities = DB_Queries.GetDocuments(
                    _DB_Connection,
                    _DB_Connection.Select(
                        DB_Table.APPLICATIONS,
                    new string[] { "id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID) }
                    ).Select(s => (uint)s[0])).Where(s => s.Type == "identity").Join(
                    _DB_Connection.Select(
                        DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA,
                        new string[] { "document_id", "last_name"/*, "first_name", "middle_name"*/ },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("type_id",Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,DB_Helper.PassportName))
                        }),
                    k1 => k1.ID,
                    k2 => k2[0],
                    (s1, s2) => Tuple.Create(s2[1].ToString(),/* s2[2].ToString(), s2[3] as string,*/ s1.Series, s1.Number));

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.GetEncoding(1251)))
                {
                    foreach (var appl in GetEgeResults().Select(s =>
                    Tuple.Create(s.LastName/*, s.FirstName, s.MiddleName*/, s.Series, s.Number)
                    ).Concat(identities).Distinct())
                        //writer.WriteLine(appl.Item1 + "%" + appl.Item2 + "%" + appl.Item3 + "%" + appl.Item4 + "%" + appl.Item5);
                        writer.WriteLine(appl.Item1 + "%%%" + appl.Item2 + "%" + appl.Item3);
                }

                Cursor.Current = Cursors.Default;
            }
        }

        private void bImport_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] subjects = new string[]
                {
                    DB_Helper.SubjectMath,
                    DB_Helper.SubjectRus,
                    DB_Helper.SubjectPhis,
                    DB_Helper.SubjectObsh,
                    DB_Helper.SubjectForen,
                    "Английский язык",
                    "Немецкий язык",
                    "Французский язык"
                };

                Cursor.Current = Cursors.WaitCursor;

                List<string[]> lines = new List<string[]>();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(openFileDialog.FileName, System.Text.Encoding.GetEncoding(1251)))
                {
                    while (!reader.EndOfStream)
                        lines.Add(reader.ReadLine().Split('%'));
                }

                var importedResults = lines.Where(s => subjects.Contains(s[5])).Select(s =>
                {
                    if (s[5] != "Русский язык" && s[5].Contains(" язык"))
                        s[5] = "Иностранный язык";

                    return new
                    {
                        LastName = s[0],
                        FirstName = s[1],
                        MiddleName = s[2],
                        Series = s[3],
                        Number = s[4],
                        Subject = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, s[5]),
                        Value = ushort.Parse(s[6]),
                        Year = ushort.Parse(s[7])
                    };
                });

                uint insertCount = 0;
                uint updateCount = 0;
                List<string[]> notFoundList = new List<string[]>();

                List<EGE_Result> savedResults = GetEgeResults().ToList();
                var identities = _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id").Join(
                    _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "series", "number"),
                    k1 => k1[1],
                    k2 => k2[0],
                    (s1, s2) => new
                    {
                        ApplID = (uint)s1[0],
                        Series = s2[1] as string,
                        Number = s2[2].ToString()
                    });

                try
                {
                    foreach (var res in importedResults)
                    {
                        //Стоит ли проверять имена вообще?
                        IEnumerable<uint> applIDs = savedResults.Where(s =>
                        s.LastName.Equals(res.LastName, StringComparison.OrdinalIgnoreCase) &&
                        //s.FirstName.Equals(res.FirstName, StringComparison.OrdinalIgnoreCase) &&
                        //s.MiddleName.Equals(res.MiddleName, StringComparison.OrdinalIgnoreCase) &&
                        s.Series == res.Series &&
                        s.Number == res.Number
                        ).GroupBy(k => k.ApplID, (k, g) => k).Concat(identities.Where(s => s.Series == res.Series && s.Number == res.Number).Select(s => s.ApplID)).Distinct();

                        if (applIDs.Count() == 0)
                            notFoundList.Add(new string[] { res.LastName, res.FirstName, res.MiddleName, res.Series, res.Number });

                        foreach (uint applID in applIDs)
                        {
                            EGE_Result foundResult = savedResults.SingleOrDefault(s => s.ApplID == applID && s.SubjectID == res.Subject);

                            if (foundResult != null)
                            {
                                if (!foundResult.Checked || foundResult.Value < res.Value)
                                {
                                    _DB_Connection.Update(
                                        DB_Table.APPLICATION_EGE_RESULTS,
                                        new Dictionary<string, object>
                                        {
                                            { "series", res.Series },
                                            { "number", res.Number },
                                            { "year", res.Year },
                                            { "value", res.Value },
                                            { "checked", true }
                                        },
                                        new Dictionary<string, object>
                                        {
                                            { "application_id", applID},
                                            { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                                            { "subject_id", res.Subject }
                                        });

                                    savedResults.Remove(foundResult);

                                    savedResults.Add(new EGE_Result(
                                        applID,
                                        res.Series,
                                        res.Number,
                                        res.LastName,
                                        //res.FirstName,
                                        //res.MiddleName,
                                        res.Subject,
                                        res.Value,
                                        true
                                        ));

                                    updateCount++;
                                }
                            }
                            else
                            {
                                _DB_Connection.Insert(
                                    DB_Table.APPLICATION_EGE_RESULTS,
                                    new Dictionary<string, object>
                                    {
                                        { "application_id", applID},
                                        { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                                        { "subject_id", res.Subject },
                                        { "series", res.Series },
                                        { "number", res.Number },
                                        { "year", res.Year },
                                        { "value", res.Value },
                                        { "checked", true }
                                    });

                                savedResults.Add(new EGE_Result(
                                    applID,
                                    res.Series,
                                    res.Number,
                                    res.LastName,
                                    //res.FirstName,
                                    //res.MiddleName,
                                    res.Subject,
                                    res.Value,
                                    true
                                    ));

                                insertCount++;
                            }
                        }
                    }
                }
                finally
                {
                    Cursor.Current = Cursors.Default;

                    MessageBox.Show("Новых результатов: " + insertCount +
                        "\nОбновлённых результатов: " + updateCount +
                        (notFoundList.Count != 0 ? "\nНе найдено соответсвий для:\n" + string.Join("\n", notFoundList.Select(s => string.Join(" ", s))) : ""), "Результаты загружены", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private IEnumerable<EGE_Result> GetEgeResults()
        {
            var applications = _DB_Connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "id,entrant_id" }//,
                    //new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID) }
                    ).Join(
                    _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                    k1 => k1[1],
                    k2 => k2[0],
                    (s1, s2) => new { ApplID = (uint)s1[0], LastN = s2[1].ToString(), FirstN = s2[2].ToString(), MiddleN = s2[3].ToString(), }
                    );

            return applications.Join(
                _DB_Connection.Select(DB_Table.APPLICATION_EGE_RESULTS, "application_id", "series", "number", "subject_id", "value", "checked"),
                k1 => k1.ApplID,
                k2 => k2[0],
                (s1, s2) => new EGE_Result
                (
                    s1.ApplID,
                    s2[1].ToString(),
                    s2[2].ToString(),
                    s1.LastN,
                    //s1.FirstN,
                    //s1.MiddleN,
                    (uint)s2[3],
                    (ushort)s2[4],
                    (bool)s2[5]
                ));
        }
    }
}
