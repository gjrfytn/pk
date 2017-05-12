using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PK.Forms
{
    partial class ApplicationDocsPrint : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly uint _ID;

        public ApplicationDocsPrint(Classes.DB_Connector connection, uint id)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _ID = id;

            cbAdmAgreement.Enabled = _DB_Connection.Select(
                DB_Table.APPLICATIONS_ENTRANCES,
                new string[] { "is_agreed_date", "is_disagreed_date" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ID) }
                ).Any(s => s[0] as DateTime? != null && s[1] as DateTime? == null);
            cbAdmAgreement.Checked = cbAdmAgreement.Enabled;
        }

        private void bPrint_Click(object sender, EventArgs e)
        {
            Classes.Utility.Print(MakeDocuments());
            DialogResult = DialogResult.OK;
        }

        private void bOpen_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(MakeDocuments());
            DialogResult = DialogResult.OK;
        }

        private string MakeDocuments()
        {
            Classes.DB_Helper dbHelper = new Classes.DB_Helper(_DB_Connection);

            Dictionary<Tuple<uint, uint>, Tuple<string, string>> streams = new Dictionary<Tuple<uint, uint>, Tuple<string, string>>
                {
                    { new Tuple<uint,uint>(11,14),Tuple.Create("Очная (дневная) бюджетная","ОБ") },
                    { new Tuple<uint,uint>(11,15),Tuple.Create("Очная (дневная) платная","ОП") },
                    { new Tuple<uint,uint>(12,14),Tuple.Create("Очно-заочная (вечерняя) бюджетная","ОЗБ") },
                    { new Tuple<uint,uint>(12,15),Tuple.Create("Очно-заочная (вечерняя) платная","ОЗП") },
                    { new Tuple<uint,uint>(10,15),Tuple.Create("Заочная платная","ЗП") },
                    { new Tuple<uint,uint>(11,20),Tuple.Create("Квота, очная (дневная)","ОКВ") },
                    { new Tuple<uint,uint>(11,16),Tuple.Create("Целевой прием, очная (дневная)","ОЦП") },
                    { new Tuple<uint,uint>(12,20),Tuple.Create("Квота, очно-заочная (вечерняя)","ОЗКВ") },
                    { new Tuple<uint,uint>(12,16),Tuple.Create("Целевой прием, очно-заочная (вечерняя)","ОЗЦП") }
                };

            string[] medCertDirections = { "13.03.02", "23.03.01", "23.03.02", "23.03.03", "23.05.01", "23.05.02" };

            List<Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>> documents =
                new List<Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>>();

            if (cbMoveJournal.Checked)
                documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                    Classes.Utility.DocumentsTemplatesPath + "MoveJournal.xml",
                    _DB_Connection,
                    _ID,
                    null,
                    null
                    ));

            List<string[]>[] inventoryTableParams = new List<string[]>[] { new List<string[]>(), new List<string[]>() };

            var entrances = _DB_Connection.Select(
                DB_Table.APPLICATIONS_ENTRANCES,
                new string[] { "faculty_short_name", "direction_id", "edu_form_id", "edu_source_id", "profile_short_name", "is_agreed_date", "is_disagreed_date" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ID) }
                ).GroupBy(
                k => Tuple.Create(k[2], k[3]),
                el => new
                {
                    Faculty = el[0].ToString(),
                    Direction = (uint)el[1],
                    Profile = el[4].ToString(),
                    AgreedDate = el[5] as DateTime?,
                    DisagreedDate = el[6] as DateTime?
                },
                (k, g) => new
                {
                    EduForm = (uint)k.Item1,
                    EduSource = (uint)k.Item2,
                    Directions = g
                });

            foreach (var stream in entrances)
                inventoryTableParams[0].Add(new string[] { streams[Tuple.Create(stream.EduForm, stream.EduSource)].Item1 + " форма обучения" });

            var docs = _DB_Connection.CallProcedure("get_application_docs", _ID).Select(
                s => new
                {
                    Type = s[1].ToString(),
                    Series = s[2].ToString(),
                    Number = s[3].ToString(),
                    OrigDate = s[6] as DateTime?
                });

            object[] applData = _DB_Connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "registration_time", "edit_time", "registrator_login", "needs_hostel", "mcado", "chernobyl", "passing_examinations", "priority_right" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, _ID) }
                )[0];
            DateTime regTime = (DateTime)applData[0];
            DateTime? editTime = applData[1] as DateTime?;
            string registratorName = applData[2].ToString().Split(' ')[0];
            bool hostel = (bool)applData[3];
            bool mcado = (bool)applData[4];
            bool chernobyl = (bool)applData[5];
            bool passing = (bool)applData[6];
            bool priority = (bool)applData[7];

            bool original = false;
            bool quota = false;

            inventoryTableParams[1].Add(new string[] { "Заявление на поступление" });
            if (entrances.Any(s => s.Directions.Any(e => e.AgreedDate != null && e.DisagreedDate == null)))
                inventoryTableParams[1].Add(new string[] { "Заявление о согласии на зачисление" });

            if (docs.Any(s => s.Type == "identity"))
                inventoryTableParams[1].Add(new string[] { "Копия паспорта" });

            var eduDoc = docs.Single(s => s.Type == "school_certificate" || s.Type == "high_edu_diploma" || s.Type == "academic_diploma");
            if (eduDoc.Type == "academic_diploma")
                inventoryTableParams[1].Add(new string[] { "Справка отдела кадров" });
            else
            {
                string buf = eduDoc.Series + " " + eduDoc.Number;
                if (eduDoc.Type == "school_certificate")
                    buf = " аттестата " + buf;
                else
                    buf = " диплома " + buf;

                if (eduDoc.OrigDate.HasValue)
                {
                    inventoryTableParams[1].Add(new string[] { "Оригинал" + buf + " Дата ориг.: " + eduDoc.OrigDate.Value.ToShortDateString() });
                    original = true;
                }
                else
                    inventoryTableParams[1].Add(new string[] { "Копия" + buf });
            }

            if (chernobyl || priority || docs.Any(s =>
              s.Type == "orphan" ||
              s.Type == "disability" ||
              s.Type == "medical" ||
              s.Type == "olympic" ||
              s.Type == "olympic_total" ||
              s.Type == "ukraine_olympic" ||
              s.Type == "international_olympic"
            ))
                inventoryTableParams[1].Add(new string[] { "Направление ПК" });

            if (entrances.Any(s => s.Directions.Any(e => medCertDirections.Contains(dbHelper.GetDirectionNameAndCode(e.Direction).Item2))))
            {
                inventoryTableParams[1].Add(new string[] { "Медицинская справка" });
                quota = true;
            }

            if (docs.Any(s => s.Type == "photos"))
                inventoryTableParams[1].Add(new string[] { "4 фотографии 3х4" });

            object[] entrant = _DB_Connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "entrant_id" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, _ID) }
                ).Join(
                _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                k1 => k1[0],
                k2 => k2[0],
                (s1, s2) => s2).First();
            uint entrantID = (uint)entrant[0];
            string lastName = entrant[1].ToString();
            string firstName = entrant[2].ToString();
            string middleName = entrant[3].ToString();

            object[] entrantData = _DB_Connection.Select(
               DB_Table.ENTRANTS,
               new string[] { "home_phone", "mobile_phone" },
               new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, entrantID) }
               )[0];
            string homePhone = entrantData[0].ToString();
            string mobilePhone = entrantData[1].ToString();

            if (cbInventory.Checked)
                documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                    Classes.Utility.DocumentsTemplatesPath + "Inventory.xml",
                    null,
                    null,
                    new string[]
                    {
                            entrantID.ToString(),
                            lastName.ToUpper(),
                            (firstName+" "+middleName).ToUpper(),
                            registratorName,
                            SystemInformation.ComputerName,
                            regTime.ToString()
                    },
                    inventoryTableParams
                    ));

            List<string> parameters = new List<string>
                {
                    lastName.ToUpper(),
                    firstName[0]+".",
                    middleName[0]+".",
                    entrantID.ToString(),
                    lastName,
                    firstName,
                    middleName,
                    regTime.Year.ToString()
                };

            foreach (string[] eduForm in inventoryTableParams[0])
                parameters.Add(eduForm[0]);

            while (parameters.Count != 13)
                parameters.Add("");

            if (cbPercRecordFace.Checked)
                documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                    Classes.Utility.DocumentsTemplatesPath + "PercRecordFace.xml",
                    null,
                    null,
                    parameters.ToArray(),
                    null
                    ));

            List<string[]>[] receiptTableParams = new List<string[]>[2];
            receiptTableParams[0] = new List<string[]>();
            receiptTableParams[1] = inventoryTableParams[1];

            foreach (var stream in entrances)
            {
                receiptTableParams[0].Add(new string[] { streams[Tuple.Create(stream.EduForm, stream.EduSource)].Item1 + " форма обучения" });
                foreach (var entrance in stream.Directions)
                {
                    if (stream.EduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                        receiptTableParams[0].Add(new string[] { "     - " + entrance.Profile + " (" + entrance.Faculty + ") " +  _DB_Connection.Select(
                                DB_Table.PROFILES,
                                    new string[] { "name" },
                                    new List<Tuple<string, Relation, object>>
                                    {
                                        new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, entrance.Faculty),
                                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL, entrance.Direction),
                                        new Tuple<string, Relation, object>("short_name",Relation.EQUAL, entrance.Profile),
                                    })[0][0].ToString()});
                    else
                        receiptTableParams[0].Add(new string[] { "     - " + _DB_Connection.Select(
                                    DB_Table.DIRECTIONS,
                                    new string[] { "short_name" },
                                    new List<Tuple<string, Relation, object>>
                                    {
                                        new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, entrance.Faculty),
                                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL, entrance.Direction)
                                    })[0][0].ToString()+ " (" + entrance.Faculty + ") " + dbHelper.GetDirectionNameAndCode(entrance.Direction).Item1});
                }
                receiptTableParams[0].Add(new string[] { "" });
            }

            if (cbReceipt.Checked)
                documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                    Classes.Utility.DocumentsTemplatesPath + "Receipt.xml",
                    null,
                    null,
                    new string[]
                    {
                            entrantID.ToString(),
                            lastName.ToUpper(),
                            (firstName+" "+middleName).ToUpper(),
                            registratorName,
                            SystemInformation.ComputerName,
                            regTime.ToString(),
                            editTime.ToString()
                    },
                    receiptTableParams
                    ));

            if (cbPercRecordBack.Checked)
            {
                string indAchValue = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, "id", "value").Join(
                _DB_Connection.Select(
                    DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                    new string[] { "institution_achievement_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                            new Tuple<string, Relation, object>("application_id",Relation.EQUAL,_ID)
                    }),
                k1 => k1[0],
                k2 => k2[0],
                (s1, s2) => s1[1]
                ).Max()?.ToString();

                bool target = _DB_Connection.Select(
                DB_Table.APPLICATIONS_ENTRANCES,
                new string[] { "target_organization_id" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ID) }
                ).Any(s => s[0] as uint? != null);

                List<object[]> marks = _DB_Connection.Select(
                    DB_Table.APPLICATIONS_EGE_MARKS_VIEW,
                    new string[] { "subject_id", "value" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, _ID) }
                    );

                uint? math = marks.SingleOrDefault(s => (uint)s[0] == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Математика"))?[1] as uint?;
                uint? rus = marks.SingleOrDefault(s => (uint)s[0] == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Русский язык"))?[1] as uint?;
                uint? phys = marks.SingleOrDefault(s => (uint)s[0] == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Физика"))?[1] as uint?;
                uint? soc = marks.SingleOrDefault(s => (uint)s[0] == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Обществознание"))?[1] as uint?;
                uint? foreign = marks.SingleOrDefault(s => (uint)s[0] == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Иностранный язык"))?[1] as uint?;
                parameters = new List<string>
                    {
                        lastName+" "+firstName+" "+middleName,
                        homePhone,
                        mobilePhone,
                        quota?"+":"",
                        priority?"+":"",
                        math.ToString(),
                        hostel?"+":"",
                        passing?"+":"",
                        rus.ToString(),
                        chernobyl?"+":"",
                        mcado?"+":"",
                        phys.ToString(),
                        target?"+":"",
                        original?"+":"",
                        soc.ToString(),
                        foreign.ToString(),
                        indAchValue,
                        (math+rus+phys).ToString(),
                        (math+rus+soc).ToString(),
                        (rus+soc+foreign).ToString()
                    };

                foreach (var stream in entrances)
                {
                    parameters.Add(streams[Tuple.Create(stream.EduForm, stream.EduSource)].Item2);
                    byte count = 0;
                    foreach (var entrance in stream.Directions)
                    {
                        if (stream.EduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                            parameters.Add(entrance.Profile);
                        else
                            parameters.Add(_DB_Connection.Select(
                                DB_Table.DIRECTIONS,
                                new string[] { "short_name" },
                                new List<Tuple<string, Relation, object>>
                                {
                                            new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, entrance.Faculty),
                                            new Tuple<string, Relation, object>("direction_id",Relation.EQUAL, entrance.Direction)
                                })[0][0].ToString());

                        count++;
                    }

                    while (count != 3)
                    {
                        parameters.Add("");
                        count++;
                    }
                }

                while (parameters.Count != 40)
                    parameters.Add("");

                documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                    Classes.Utility.DocumentsTemplatesPath + "PercRecordBack.xml",
                    null,
                    null,
                    parameters.ToArray(),
                    null
                    ));
            }

            if (cbAdmAgreement.Checked)
            {
                object[] agreedDir = _DB_Connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "faculty_short_name", "direction_id", "edu_form_id", "is_agreed_date" },
                    new List<Tuple<string, Relation, object>>
                    {
                            new Tuple<string, Relation, object>("application_id", Relation.EQUAL,_ID),
                            new Tuple<string, Relation, object>("is_agreed_date", Relation.NOT_EQUAL,null),
                            new Tuple<string, Relation, object>("is_disagreed_date", Relation.EQUAL,null),
                    })[0];

                string dirShortName = _DB_Connection.Select(
                    DB_Table.DIRECTIONS,
                    new string[] { "short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                            new Tuple<string, Relation, object>("direction_id", Relation.EQUAL,agreedDir[1])
                    })[0][0].ToString();

                var marks = _DB_Connection.Select(
                    DB_Table.APPLICATIONS_EGE_MARKS_VIEW,
                    new string[] { "subject_id", "value" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, _ID) }
                    ).Select(s => new { Subj = (uint)s[0], Value = (uint)s[1] });

                ushort sum = 0;
                foreach (object[] subj in _DB_Connection.Select(
                    DB_Table.ENTRANCE_TESTS,
                    new string[] { "subject_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                            new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,dbHelper.CurrentCampaignID),
                            new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,agreedDir[0]),
                            new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,agreedDir[1])
                    }
                    ))
                {
                    var mark = marks.SingleOrDefault(s => s.Subj == (uint)subj[0]);
                    if (mark != null)
                        sum += (ushort)mark.Value;
                }

                documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                    Classes.Utility.DocumentsTemplatesPath + "AdmAgreement.xml",
                    null,
                    null,
                    new string[]
                    {
                            (lastName + " " + firstName + " " + middleName).ToUpper(),
                            dbHelper.GetDirectionNameAndCode((uint)agreedDir[1]).Item1+" ("+dirShortName+")",
                            dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM,(uint)agreedDir[2]),
                            _ID.ToString(),
                            sum.ToString(),
                            lastName.ToUpper()+" "+firstName[0]+"."+middleName[0]+".",
                            ((DateTime) agreedDir[3]).ToShortDateString()
                    },
                    null
                    ));
            }

            string doc = Classes.Utility.TempPath + "abitDocs" + new Random().Next();
            Classes.DocumentCreator.Create(doc, documents);
            doc += ".docx";

            return doc;
        }
    }
}
