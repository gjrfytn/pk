using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    public partial class HR_DepartmentPrint : Form
    {
        private readonly DB_Connector _DB_Connection;

        public HR_DepartmentPrint(DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;

            #region Components
            if (new DB_Helper(_DB_Connection).IsMasterCampaign(Classes.Settings.CurrentCampaignID))
            {
                cbActs.Enabled = false;
                cbReceipts.Enabled = false;
            }
            #endregion
        }

        private void rbDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpStart.Enabled = rbDate.Checked;
            dtpEnd.Enabled = rbDate.Checked;
            tbNumber.Enabled = !rbDate.Checked;
        }

        private void bPrint_Click(object sender, EventArgs e)
        {
            if (!cbActs.Checked && !cbReceipts.Checked && !cbExamSheets.Checked)
            {
                MessageBox.Show("Не отмечена информация к печати.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            IEnumerable<string> ordersNumbers;
            if (rbDate.Checked)
                ordersNumbers = _DB_Connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "number" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("date",Relation.GREATER_EQUAL,dtpStart.Value.Date),
                        new Tuple<string, Relation, object>("date",Relation.LESS,dtpEnd.Value.Date.AddDays(1))
                    }).Select(s => s[0].ToString());
            else
                ordersNumbers = new string[] { tbNumber.Text };

            DB_Helper dbHelper = new DB_Helper(_DB_Connection);
            var orders = ordersNumbers.Join(
                _DB_Connection.Select(
                    DB_Table.ORDERS,//TODO убрать лишние поля
                    new string[] { "number", "type", "date", "protocol_number", "protocol_date", "edu_form_id", "edu_source_id", "faculty_short_name", "direction_id", "profile_short_name", "campaign_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL, Classes.Settings.CurrentCampaignID),
                        new Tuple<string, Relation, object>("type",Relation.EQUAL,"admission"),
                        new Tuple<string, Relation, object>("protocol_number",Relation.NOT_EQUAL,null)
                    }),
                k1 => k1,
                k2 => k2[0],
                (s1, s2) => new
                {
                    Number = s1,
                    Date = (DateTime)s2[2],
                    ProtocolNumber = (ushort)s2[3],
                    ProtocolDate = (DateTime)s2[4],
                    EduForm = (uint)s2[5],
                    EduSource = (uint)s2[6],
                    Faculty = s2[7].ToString(),
                    Direction = (uint)s2[8],
                    Profile = s2[9] as string,
                    Master = dbHelper.IsMasterCampaign((uint)s2[10])
                });

            if (orders.Count() == 0)
            {
                MessageBox.Show("Не найдено соответствующих приказов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Dictionary<uint, string> forms = new Dictionary<uint, string>
                {
                    { 11, "очной формы"},
                    { 12, "очно-заочной (вечерней) формы"},
                    { 10, "заочной формы" }
                };

            uint mathID = dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectMath);
            uint physID = dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectPhis);
            uint rusID = dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectRus);
            uint socID = dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectObsh);
            uint forID = dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectForen);

            List<Classes.DocumentCreator.DocumentParameters> actsDocs = new List<Classes.DocumentCreator.DocumentParameters>();
            List<Classes.DocumentCreator.DocumentParameters> receiptsDocs = new List<Classes.DocumentCreator.DocumentParameters>();
            List<Classes.DocumentCreator.DocumentParameters> sheetsDocs = new List<Classes.DocumentCreator.DocumentParameters>();
            foreach (var order in orders)
            {
                var applications = _DB_Connection.Select(
                    DB_Table.ORDERS_HAS_APPLICATIONS,
                    new string[] { "applications_id", "record_book_number" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, order.Number) }
                    ).Join(
                    _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id", "status"),
                    k1 => k1[0],
                    k2 => k2[0],
                    (s1, s2) => new { ApplID = (uint)s2[0], EntrID = (uint)s2[1], RecordBook = s1[1].ToString() }
                    ).Join(
                    _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                    k1 => k1.EntrID,
                    k2 => k2[0],
                    (s1, s2) => new { s1.ApplID, s1.EntrID, s1.RecordBook, LastName = s2[1].ToString(), FirstName = s2[2].ToString(), MiddleName = s2[3] as string }
                    );

                applications = applications.Where(s =>
                {
                    IEnumerable<DateTime> admDates = _DB_Connection.Select(
                        DB_Table.ORDERS_HAS_APPLICATIONS,
                        new string[] { "orders_number" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, s.ApplID) }
                        ).Join(
                        _DB_Connection.Select(DB_Table.ORDERS, new string[] { "number", "date" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("type",Relation.EQUAL,"exception"),
                            new Tuple<string, Relation, object>("protocol_number",Relation.NOT_EQUAL,null),
                            new Tuple<string, Relation, object>("edu_form_id",Relation.EQUAL,order.EduForm),
                            new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,order.EduSource),
                            new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,order.Faculty),
                            new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,order.Direction),
                            order.Profile!=null?new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,order.Profile):null
                        }),
                        k1 => k1[0],
                        k2 => k2[0],
                        (s1, s2) => (DateTime)s2[1]
                        );

                    return !admDates.Any(d => d > order.Date);
                });

                if (applications.Count() == 0)
                    continue;

                if (dbHelper.IsMasterCampaign(Classes.Settings.CurrentCampaignID))
                {
                    var table = applications.Join(
                        _DB_Connection.Select(
                            DB_Table.MASTERS_EXAMS_MARKS,
                            new string[] { "entrant_id", "mark", "bonus" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL, Classes.Settings.CurrentCampaignID),
                                new Tuple<string, Relation, object>("faculty",Relation.EQUAL, order.Faculty),
                                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL, order.Direction),
                                new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL, order.Profile),
                            }),
                        k1 => k1.EntrID,
                        k2 => k2[0],
                        (s1, s2) => new { s1.ApplID, s1.LastName, s1.FirstName, s1.MiddleName, s1.RecordBook, Mark = (short)s2[1] + (ushort)s2[2] }
                        );

                    //Tuple<string, string> dirNameCode = dbHelper.GetDirectionNameAndCode(order.Direction);
                    //string dirSocr = dbHelper.GetDirectionShortName(order.Faculty, order.Direction);
                    //if (cbActs.Checked)
                    //    actsDocs.Add(new Classes.DocumentCreator.DocumentParameters(
                    //        Classes.Settings.DocumentsTemplatesPath + "Act.xml",
                    //        null,
                    //        null,
                    //        new string[]
                    //        {
                    //        DateTime.Now.ToShortDateString(),
                    //        order.Date.Year.ToString(),
                    //        forms[order.EduForm] + " обучения" +
                    //        (order.EduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP) ? " по договорам с оплатой стоимости обучения" : ""),
                    //        order.Faculty,
                    //        dirNameCode.Item2,
                    //        dirNameCode.Item1,
                    //        order.Master?"Магистерская программа: " :(order.Profile != null ?(dirNameCode.Item2.Split('.')[1]=="05"?"Специализация": "Профиль")+": " : ""),
                    //        order.Profile != null ? order.Profile + " - " + DB_Queries.GetProfileName(_DB_Connection,order.Faculty,order.Direction,order.Profile).Split('|')[0] : ""
                    //        },
                    //        new IEnumerable<string[]>[] { table.Select(s=>new
                    //    {
                    //        Name = s.LastName + " " + s.FirstName + " " + s.MiddleName,
                    //        s.RecordBook
                    //    }).OrderBy(s => s.Name).Select(s => new string[] { s.Name, s.RecordBook }) }));

                    foreach (var appl in table)
                    {
                        //if (cbReceipts.Checked)
                        //    receiptsDocs.Add(new Classes.DocumentCreator.DocumentParameters(
                        //        Classes.Settings.DocumentsTemplatesPath + "AdmReceipt.xml",
                        //        null,
                        //        null,
                        //        new string[]
                        //        {
                        //        order.Number,
                        //        order.Date.ToShortDateString(),
                        //        (appl.LastName+" "+appl.FirstName[0]+"."+(appl.MiddleName.Length!=0?appl.MiddleName[0].ToString()+".":"")).ToUpper(),
                        //        order.Faculty,
                        //        order.Profile != null ?order.Profile:dirSocr
                        //        },
                        //        null
                        //        ));

                        if (cbExamSheets.Checked)
                            sheetsDocs.Add(new Classes.DocumentCreator.DocumentParameters(
                                Classes.Settings.DocumentsTemplatesPath + "ExamSheetM.xml",
                                null,
                                null,
                                new string[]
                                {
                                    appl.Mark.ToString(),
                                    dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM,order.EduForm),
                                    appl.ApplID.ToString(),
                                    appl.LastName.ToUpper(),
                                    appl.FirstName.ToUpper(),
                                    appl.MiddleName.ToUpper(),
                                    order.Profile
                                },
                                null
                                ));
                    }
                }
                else
                {
                    IEnumerable<DB_Queries.Mark> marks = DB_Queries.GetMarks(_DB_Connection, applications.Select(s => s.ApplID), Classes.Settings.CurrentCampaignID);
                    var table = applications.Join(
                        marks.GroupBy(
                            k => Tuple.Create(k.ApplID, k.SubjID),
                            (k, g) => new { g.First().ApplID, Mark = g.First(s => s.Value == g.Max(m => m.Value)) }
                            ).GroupBy(
                            k => k.ApplID,
                            (k, g) => new { g.First().ApplID, Subjects = g.Select(s => s.Mark) }
                            ),
                        k1 => k1.ApplID,
                        k2 => k2.ApplID,
                        (s1, s2) => new { s1.ApplID, s1.LastName, s1.FirstName, s1.MiddleName, s1.RecordBook, s2.Subjects }
                        );

                    Tuple<string, string> dirNameCode = dbHelper.GetDirectionNameAndCode(order.Direction);
                    string dirSocr = dbHelper.GetDirectionShortName(order.Faculty, order.Direction);
                    if (cbActs.Checked)
                        actsDocs.Add(new Classes.DocumentCreator.DocumentParameters(
                            Classes.Settings.DocumentsTemplatesPath + "Act.xml",
                            null,
                            null,
                            new string[]
                            {
                                DateTime.Now.ToShortDateString(),
                                order.Date.Year.ToString(),
                                forms[order.EduForm] + " обучения" +
                                (order.EduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP) ? " по договорам с оплатой стоимости обучения" : ""),
                                order.Faculty,
                                dirNameCode.Item2,
                                dirNameCode.Item1,
                                order.Master?"Магистерская программа: " :(order.Profile != null ?(dirNameCode.Item2.Split('.')[1]=="05"?"Специализация": "Профиль")+": " : ""),
                                order.Profile != null ? order.Profile + " - " + DB_Queries.GetProfileName(_DB_Connection,order.Faculty,order.Direction,order.Profile).Split('|')[0] : ""
                            },
                            new IEnumerable<string[]>[] { table.Select(s=>new
                            {
                                Name = s.LastName + " " + s.FirstName + " " + s.MiddleName,
                                s.RecordBook
                            }).OrderBy(s => s.Name).Select(s => new string[] { s.Name, s.RecordBook }) }));

                    foreach (var appl in table)
                    {
                        if (cbReceipts.Checked)
                            receiptsDocs.Add(new Classes.DocumentCreator.DocumentParameters(
                                Classes.Settings.DocumentsTemplatesPath + "AdmReceipt.xml",
                                null,
                                null,
                                new string[]
                                {
                                    order.Number,
                                    order.Date.ToShortDateString(),
                                    (appl.LastName+" "+appl.FirstName[0]+"."+(appl.MiddleName.Length!=0?appl.MiddleName[0].ToString()+".":"")).ToUpper(),
                                    order.Faculty,
                                    order.Profile != null ?order.Profile:dirSocr
                                },
                                null
                                ));

                        if (cbExamSheets.Checked)
                            sheetsDocs.Add(new Classes.DocumentCreator.DocumentParameters(
                                Classes.Settings.DocumentsTemplatesPath + "ExamSheet.xml",
                                null,
                                null,
                                new string[]
                                {
                                    appl.Subjects.SingleOrDefault(s=>s.SubjID==mathID)?.Value.ToString(),
                                    dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM,order.EduForm),
                                    appl.Subjects.SingleOrDefault(s=>s.SubjID==physID)?.Value.ToString(),
                                    appl.ApplID.ToString(),
                                    appl.Subjects.SingleOrDefault(s=>s.SubjID==rusID)?.Value.ToString(),
                                    appl.LastName.ToUpper(),
                                    appl.Subjects.SingleOrDefault(s=>s.SubjID==socID)?.Value.ToString(),
                                    appl.FirstName.ToUpper(),
                                    appl.Subjects.SingleOrDefault(s=>s.SubjID==forID)?.Value.ToString(),
                                    appl.MiddleName.ToUpper(),
                                    order.Profile != null ?order.Profile:dirSocr
                                },
                                null
                                ));
                    }
                }
            }

            if (cbActs.Checked)
            {
                string doc = Classes.Settings.TempPath + "acts" + new Random().Next();
                Classes.DocumentCreator.Create(doc, actsDocs, false);
                System.Diagnostics.Process.Start(doc + ".docx");
            }

            if (cbReceipts.Checked)
            {
                string doc = Classes.Settings.TempPath + "receipts" + new Random().Next();
                Classes.DocumentCreator.Create(doc, receiptsDocs, false);
                System.Diagnostics.Process.Start(doc + ".docx");
            }

            if (cbExamSheets.Checked)
            {
                string doc = Classes.Settings.TempPath + "examSheets" + new Random().Next();
                Classes.DocumentCreator.Create(doc, sheetsDocs, false);
                System.Diagnostics.Process.Start(doc + ".docx");
            }

            Cursor.Current = Cursors.Default;
        }
    }
}
