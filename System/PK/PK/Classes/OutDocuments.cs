using System.Collections.Generic;
using System.Linq;
using System;

namespace PK.Classes
{
    static class OutDocuments
    {
        public class Entrant
        {
            class Stream
            {
                public class Entrance
                {
                    public readonly string Faculty;
                    public readonly uint DirectionID;
                    public readonly string Profile;
                    public readonly DateTime? AgreedDate;
                    public readonly DateTime? DisagreedDate;

                    public Entrance(string faculty, uint directionID, string profile, DateTime? agreedDate, DateTime? disagreedDate)
                    {
                        Faculty = faculty;
                        DirectionID = directionID;
                        Profile = profile;
                        AgreedDate = agreedDate;
                        DisagreedDate = disagreedDate;
                    }
                }

                public readonly uint EduForm;
                public readonly uint EduSource;
                public readonly System.Collections.ObjectModel.ReadOnlyCollection<Entrance> Directions;

                //public readonly IEnumerable<Direction> Directions
                //{
                //    get { return _Directions.AsReadOnly(); }
                //}

                public Stream(uint eduForm, uint eduSource, System.Collections.ObjectModel.ReadOnlyCollection<Entrance> directions)
                {
                    EduForm = eduForm;
                    EduSource = eduSource;
                    Directions = directions;
                }
            }

            class ApplicationData
            {
                public bool Hostel;
                public bool MCADO;
                public bool Chernobyl;
                public bool PassingExam;
                public bool Priority;
                public bool Original;
                public bool Quota;
                public string LastName;
                public string FirstName;
                public string MiddleName;
                public string HomePhone;
                public string MobilePhone;
            }

            private static readonly Dictionary<Tuple<uint, uint>, Tuple<string, string>> _Streams = new Dictionary<Tuple<uint, uint>, Tuple<string, string>>
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

            public static string Documents(DB_Connector connection, uint applID, bool moveJournal, bool inventory, bool percRecordFace, bool receipt, bool percRecordBack, bool admAgreement)
            {
                #region Contracts
                if (connection == null)
                    throw new ArgumentNullException(nameof(connection));
                #endregion

                string[] medCertDirections = { "13.03.02", "23.03.01", "23.03.02", "23.03.03", "23.05.01", "23.05.02" };

                List<DocumentCreator.DocumentParameters> documents = new List<DocumentCreator.DocumentParameters>();

                if (moveJournal)
                    documents.Add(new DocumentCreator.DocumentParameters(
                        Utility.DocumentsTemplatesPath + "MoveJournal.xml",
                        connection,
                        applID,
                        null,
                        null
                        ));

                List<string[]>[] inventoryTableParams = new List<string[]>[] { new List<string[]>(), new List<string[]>() };

                IEnumerable<Stream> entrances = connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "faculty_short_name", "direction_id", "edu_form_id", "edu_source_id", "profile_short_name", "is_agreed_date", "is_disagreed_date" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("application_id", Relation.EQUAL, applID) }
                    ).GroupBy(
                    k => Tuple.Create(k[2], k[3]),
                    el => new Stream.Entrance(
                        el[0].ToString(),
                        (uint)el[1],
                        el[4].ToString(),
                        el[5] as DateTime?,
                        el[6] as DateTime?
                        ),
                    (k, g) => new Stream((uint)k.Item1, (uint)k.Item2, g.ToList().AsReadOnly())
                    );

                foreach (Stream stream in entrances)
                    inventoryTableParams[0].Add(new string[] { _Streams[Tuple.Create(stream.EduForm, stream.EduSource)].Item1 + " форма обучения" });

                var docs = connection.CallProcedure("get_application_docs", applID).Select(
                    s => new
                    {
                        ID = (uint)s[0],
                        Type = s[1].ToString(),
                        Series = s[2].ToString(),
                        Number = s[3].ToString(),
                        OrigDate = s[6] as DateTime?
                    });

                object[] applDataBuf = connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "registration_time", "edit_time", "registrator_login", "needs_hostel", "mcado", "chernobyl", "passing_examinations", "priority_right" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, applID) }
                    )[0];

                DateTime regTime = (DateTime)applDataBuf[0];
                DateTime? editTime = applDataBuf[1] as DateTime?;
                string registratorName = applDataBuf[2].ToString().Split(' ')[0];
                ApplicationData applData = new ApplicationData
                {
                    Hostel = (bool)applDataBuf[3],
                    MCADO = (bool)applDataBuf[4],
                    Chernobyl = (bool)applDataBuf[5],
                    PassingExam = (bool)applDataBuf[6],
                    Priority = (bool)applDataBuf[7],
                    Original = false,
                    Quota = false,
                };

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
                        applData.Original = true;
                    }
                    else
                        inventoryTableParams[1].Add(new string[] { "Копия" + buf });
                }

                if (applData.Chernobyl || applData.Priority || docs.Any(s =>
                s.Type == "orphan" ||
                s.Type == "disability" ||
                s.Type == "medical" ||
                s.Type == "olympic" ||
                s.Type == "olympic_total" ||
                s.Type == "ukraine_olympic" ||
                s.Type == "international_olympic"
                ))
                    inventoryTableParams[1].Add(new string[] { "Направление ПК" });

                foreach (var medDoc in docs.Where(s => s.Type == "medical"))
                {
                    List<object[]> certificateData = connection.Select(
                        DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                        new string[] { "name" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("document_id", Relation.EQUAL, medDoc.ID) }
                        );

                    if (certificateData.Count > 0 && certificateData[0][0].ToString() == DB_Helper.MedCertificate)
                        inventoryTableParams[1].Add(new string[] { "Медицинская справка" });
                    else
                        applData.Quota = true;
                }

                if (docs.Any(s => s.Type == "photos"))
                    inventoryTableParams[1].Add(new string[] { "4 фотографии 3х4" });

                if (docs.Any(s => s.Type == "orphan" || s.Type == "disability"))
                    applData.Quota = true;

                object[] entrant = connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "entrant_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, applID) }
                    ).Join(
                    connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                    k1 => k1[0],
                    k2 => k2[0],
                    (s1, s2) => s2).First();

                uint entrantID = (uint)entrant[0];
                applData.LastName = entrant[1].ToString();
                applData.FirstName = entrant[2].ToString();
                applData.MiddleName = entrant[3].ToString();

                object[] entrantData = connection.Select(
                   DB_Table.ENTRANTS,
                   new string[] { "home_phone", "mobile_phone" },
                   new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, entrantID) }
                   )[0];
                applData.HomePhone = entrantData[0].ToString();
                applData.MobilePhone = entrantData[1].ToString();

                if (inventory)
                    documents.Add(new DocumentCreator.DocumentParameters(
                        Utility.DocumentsTemplatesPath + "Inventory.xml",
                        null,
                        null,
                        new string[]
                        {
                            entrantID.ToString(),
                            applData.LastName.ToUpper(),
                            (applData.FirstName+" "+applData.MiddleName).ToUpper(),
                            registratorName,
                            System.Windows.Forms.SystemInformation.ComputerName,
                            regTime.ToString()
                        },
                        inventoryTableParams
                        ));

                if (percRecordFace)
                    AddPercRecordFace(documents, inventoryTableParams[0], regTime, applData, entrantID);

                if (receipt)
                    AddReceipt(documents, connection, inventoryTableParams[1], entrances, regTime, editTime, registratorName, applData, entrantID);

                var marks = DB_Queries.GetMarks(connection, new uint[] { applID }, Utility.CurrentCampaignID).GroupBy(
                    k => k.SubjID,
                    (k, g) => Tuple.Create(k, g.Any(s => s.Checked) ? g.Where(s => s.Checked).Max(s => s.Value) : g.Max(s => s.Value))
                    );

                if (percRecordBack)
                    AddPercRecordBack(documents, connection, applID, applData, entrances, marks);

                if (admAgreement)
                    AddAdmAgreement(documents, connection, applID, applData, marks);

                string doc = Utility.TempPath + "abitDocs" + new Random().Next();
                DocumentCreator.Create(doc, documents, true);
                return doc + ".docx";
            }

            private static void AddPercRecordFace(List<DocumentCreator.DocumentParameters> documents, List<string[]> streamsTableParams, DateTime regTime, ApplicationData applData, uint entrantID)
            {
                List<string> parameters = new List<string>
                {
                    applData.LastName.ToUpper(),
                    applData.FirstName[0]+".",
                    applData.MiddleName.Length!=0?applData.MiddleName[0]+".":"",
                    entrantID.ToString(),
                    applData. LastName,
                    applData.FirstName,
                    applData. MiddleName,
                    regTime.Year.ToString()
                };

                foreach (string[] eduStream in streamsTableParams)
                    parameters.Add(eduStream[0]);

                while (parameters.Count != 13)
                    parameters.Add("");


                documents.Add(new DocumentCreator.DocumentParameters(
                    Utility.DocumentsTemplatesPath + "PercRecordFace.xml",
                    null,
                    null,
                    parameters.ToArray(),
                    null
                    ));
            }

            private static void AddReceipt(
                List<DocumentCreator.DocumentParameters> documents,
                DB_Connector connection,
                List<string[]> entrDocumentsTableParams,
                IEnumerable<Stream> entrances,
                DateTime regTime,
                DateTime? editTime,
                string registratorName,
                ApplicationData applData,
                uint entrantID)
            {
                List<string[]>[] receiptTableParams = new List<string[]>[2];
                receiptTableParams[0] = new List<string[]>();
                receiptTableParams[1] = entrDocumentsTableParams;

                DB_Helper dbHelper = new DB_Helper(connection);
                foreach (Stream stream in entrances)
                {
                    receiptTableParams[0].Add(new string[] { _Streams[Tuple.Create(stream.EduForm, stream.EduSource)].Item1 + " форма обучения" });
                    foreach (Stream.Entrance entrance in stream.Directions)
                    {
                        if (stream.EduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP))
                            receiptTableParams[0].Add(new string[] { "     - " + entrance.Profile + " (" + entrance.Faculty + ") " +
                            DB_Queries.GetProfileName(connection,entrance.Faculty,entrance.DirectionID,entrance.Profile)});
                        else
                            receiptTableParams[0].Add(new string[] { "     - " + connection.Select(
                                DB_Table.DIRECTIONS,
                                new string[] { "short_name" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, entrance.Faculty),
                                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL, entrance.DirectionID)
                                })[0][0].ToString()+ " (" + entrance.Faculty + ") " + dbHelper.GetDirectionNameAndCode(entrance.DirectionID).Item1});
                    }
                    receiptTableParams[0].Add(new string[] { "" });
                }

                documents.Add(new DocumentCreator.DocumentParameters(
                    Utility.DocumentsTemplatesPath + "Receipt.xml",
                    null,
                    null,
                    new string[]
                    {
                        entrantID.ToString(),
                        applData.LastName.ToUpper(),
                        (applData.FirstName+" "+applData.MiddleName).ToUpper(),
                        registratorName,
                        System.Windows.Forms.SystemInformation.ComputerName,
                        regTime.ToString(),
                        editTime.HasValue?editTime.ToString():"нет"
                    },
                    receiptTableParams
                    ));
            }

            private static void AddPercRecordBack(
                List<DocumentCreator.DocumentParameters> documents,
                DB_Connector connection,
                uint applID,
                ApplicationData applData,
                IEnumerable<Stream> entrances,
                IEnumerable<Tuple<uint, byte>> marks)
            {
                List<string> parameters;
                string indAchValue = connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, "id", "value").Join(
                    connection.Select(
                        DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                        new string[] { "institution_achievement_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("application_id",Relation.EQUAL, applID)
                        }),
                    k1 => k1[0],
                    k2 => k2[0],
                    (s1, s2) => s1[1]
                    ).Max()?.ToString();

                bool target = connection.Select(
                DB_Table.APPLICATIONS_ENTRANCES,
                new string[] { "target_organization_id" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("application_id", Relation.EQUAL, applID) }
                ).Any(s => s[0] as uint? != null);

                DB_Helper dbHelper = new DB_Helper(connection);
                byte? math = MaxOrNull(marks.Where(s => s.Item1 == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Математика")).Select(s => s.Item2));
                byte? rus = MaxOrNull(marks.Where(s => s.Item1 == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Русский язык")).Select(s => s.Item2));
                byte? phys = MaxOrNull(marks.Where(s => s.Item1 == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Физика")).Select(s => s.Item2));
                byte? soc = MaxOrNull(marks.Where(s => s.Item1 == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Обществознание")).Select(s => s.Item2));
                byte? foreign = MaxOrNull(marks.Where(s => s.Item1 == dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Иностранный язык")).Select(s => s.Item2));
                parameters = new List<string>
                {
                    applData.LastName +" "+applData.FirstName+" "+applData.MiddleName,
                    applData.HomePhone,
                    applData.MobilePhone,
                    applData.Quota?"+":"",
                    applData.Priority?"+":"",
                    math.ToString(),
                    applData.Hostel?"+":"",
                    applData.PassingExam?"+":"",
                    rus.ToString(),
                    applData.Chernobyl?"+":"",
                    applData.MCADO?"+":"",
                    phys.ToString(),
                    target?"+":"",
                    applData.Original?"+":"",
                    soc.ToString(),
                    foreign.ToString(),
                    indAchValue,
                    (math+rus+phys).ToString(),
                    (math+rus+soc).ToString(),
                    (rus+soc+foreign).ToString()
                };

                foreach (Stream stream in entrances)
                {
                    parameters.Add(_Streams[Tuple.Create(stream.EduForm, stream.EduSource)].Item2);
                    byte count = 0;
                    foreach (Stream.Entrance entrance in stream.Directions)
                    {
                        if (stream.EduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP))
                            parameters.Add(entrance.Profile);
                        else
                            parameters.Add(connection.Select(
                                DB_Table.DIRECTIONS,
                                new string[] { "short_name" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, entrance.Faculty),
                                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL, entrance.DirectionID)
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

                documents.Add(new DocumentCreator.DocumentParameters(
                    Utility.DocumentsTemplatesPath + "PercRecordBack.xml",
                    null,
                    null,
                    parameters.ToArray(),
                    null
                    ));
            }

            private static void AddAdmAgreement(
                List<DocumentCreator.DocumentParameters> documents,
                DB_Connector connection,
                uint applID,
                ApplicationData applData,
                IEnumerable<Tuple<uint, byte>> marks)
            {
                object[] agreedDir = connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "faculty_short_name", "direction_id", "edu_form_id", "is_agreed_date" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL,applID),
                        new Tuple<string, Relation, object>("is_agreed_date", Relation.NOT_EQUAL,null),
                        new Tuple<string, Relation, object>("is_disagreed_date", Relation.EQUAL,null),
                    })[0];

                string dirShortName = connection.Select(
                    DB_Table.DIRECTIONS,
                    new string[] { "short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("direction_id", Relation.EQUAL,agreedDir[1])
                    })[0][0].ToString();

                ushort sum = 0;
                foreach (object[] subj in connection.Select(
                    DB_Table.ENTRANCE_TESTS,
                    new string[] { "subject_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Utility.CurrentCampaignID),
                        new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,agreedDir[0]),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,agreedDir[1])
                    }
                    ))
                {
                    var mark = marks.SingleOrDefault(s => s.Item1 == (uint)subj[0]);
                    if (mark != null)
                        sum += mark.Item2;
                }

                DB_Helper dbHelper = new DB_Helper(connection);
                documents.Add(new DocumentCreator.DocumentParameters(
                    Utility.DocumentsTemplatesPath + "AdmAgreement.xml",
                    null,
                    null,
                    new string[]
                    {
                        (applData.LastName + " " + applData.FirstName + " " + applData.MiddleName).ToUpper(),
                        dbHelper.GetDirectionNameAndCode((uint)agreedDir[1]).Item1+" ("+dirShortName+")",
                        dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM,(uint)agreedDir[2]),
                        applID.ToString(),
                        sum.ToString(),
                        applData.LastName.ToUpper()+" "+applData.FirstName[0]+"."+(applData.MiddleName.Length!=0?applData.MiddleName[0]+".":""),
                        ((DateTime) agreedDir[3]).ToShortDateString()
                    },
                    null
                    ));
            }
        }

        public static string RegistrationJournal(DB_Connector connection, DateTime date)
        {
            #region Contracts
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            #endregion

            Dictionary<Tuple<uint, uint>, string> streams = new Dictionary<Tuple<uint, uint>, string>
            {
                {new Tuple<uint,uint>(11,14),"ОБ" },
                {new Tuple<uint,uint>(11,15),"ОП" },
                {new Tuple<uint,uint>(12,14),"ОЗБ" },
                {new Tuple<uint,uint>(12,15),"ОЗП" },
                {new Tuple<uint,uint>(10,15),"ЗП" },
                {new Tuple<uint,uint>(11,20),"ОКВ" },
                {new Tuple<uint,uint>(11,16),"ОЦП" },
                {new Tuple<uint,uint>(12,20),"ОЗКВ" },
                {new Tuple<uint,uint>(12,16),"ОЗЦП" }
            };

            List<string[]> data = new List<string[]>();

            date = date.Date;

            var applications = connection.Select(
                DB_Table.APPLICATIONS,
                "id", "entrant_id", "registration_time"
                ).Where(a => ((DateTime)a[2]).Date == date).Join(
                connection.Select(
                DB_Table.ENTRANTS,
                "id", "home_phone", "mobile_phone"
                ),
                k1 => k1[1],
                k2 => k2[0],
                (s1, s2) => new
                {
                    ApplID = (uint)s1[0],
                    EntrID = (uint)s1[1],
                    HomePhone = s2[1].ToString(),
                    MobilePhone = s2[2].ToString()
                }
                ).Join(
                connection.Select(
                DB_Table.ENTRANTS_VIEW,
                "id", "last_name", "first_name", "middle_name"
                ),
                k1 => k1.EntrID,
                k2 => k2[0],
                (s1, s2) => new
                {
                    s1.ApplID,
                    s1.EntrID,
                    LastName = s2[1].ToString(),
                    FirstName = s2[2].ToString(),
                    MiddleName = s2[3].ToString(),
                    s1.HomePhone,
                    s1.MobilePhone
                });

            var applDocs = connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS).Join(
                    connection.Select(DB_Table.DOCUMENTS, "id", "type", "series", "number", "original_recieved_date"),
                k1 => k1[1],
                k2 => k2[0],
                (s1, s2) => new
                {
                    ApplID = (uint)s1[0],
                    DocID = (uint)s2[0],
                    DocType = s2[1].ToString(),
                    DocSeries = s2[2].ToString(),
                    DocNumber = s2[3].ToString(),
                    Original = s2[4] as DateTime?
                }
                );

            foreach (var appl in applications)
            {
                object[] idDoc = connection.Select(
                    DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA,
                    new string[] { "reg_index", "reg_region", "reg_district", "reg_town", "reg_street", "reg_house", "reg_flat" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id",Relation.EQUAL,applDocs.Single(d=>d.ApplID==appl.ApplID&&d.DocType=="identity").DocID)
                    }
                    )[0];

                var eduDoc = applDocs.Single(d =>
                d.ApplID == appl.ApplID &&
                (d.DocType == "academic_diploma" ||
                d.DocType == "school_certificate" ||
                d.DocType == "middle_edu_diploma" ||
                d.DocType == "high_edu_diploma")
                );

                string eduDocName;
                switch (eduDoc.DocType)
                {
                    case "academic_diploma":
                        eduDocName = "справки";
                        break;
                    case "school_certificate":
                        eduDocName = "аттестата";
                        break;
                    case "middle_edu_diploma":
                        eduDocName = "диплома";
                        break;
                    case "high_edu_diploma":
                        eduDocName = "диплома";
                        break;
                    default:
                        throw new Exception("Unreacheble reached.");
                }

                List<object[]> applEntr = connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "edu_form_id", "edu_source_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id",Relation.EQUAL,appl.ApplID)
                    }
                    );

                data.Add(new string[]
                {
                    appl.EntrID.ToString(),
                    appl.LastName+" "+ appl.FirstName +" "+appl.MiddleName,
                    string.Join(", ",idDoc.Where(o=>o.ToString()!=""))   + "\n\n"+appl.HomePhone+", "+appl.MobilePhone,
                    (eduDoc.Original!=null?"Оригинал ":"Копия ")+eduDocName+" "+eduDoc.DocSeries+eduDoc.DocNumber,
                    string.Join(", ",applEntr.Select(en=>streams[Tuple.Create((uint)en[0],(uint)en[1])]).Distinct())
                });
            }

            string doc = Utility.TempPath + "registrationJournal" + new Random().Next();
            DocumentCreator.Create(
                Utility.DocumentsTemplatesPath + "RegistrationJournal.xml",
                doc,
                null,
                new List<string[]>[] { data }
                );

            return doc + ".docx";
        }

        public static string Order(DB_Connector connection, string number)
        {
            #region Contracts
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Некорректный номер приказа.", nameof(number));
            #endregion

            DB_Helper dbHelper = new DB_Helper(connection);

            var order = connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "type", "date", "protocol_number", "protocol_date", "edu_form_id", "edu_source_id", "faculty_short_name", "direction_id", "profile_short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("number",Relation.EQUAL,number)
                    }).Select(s => new
                    {
                        Type = s[0].ToString(),
                        Date = (DateTime)s[1],
                        ProtocolNumber = (ushort)s[2],
                        ProtocolDate = (DateTime)s[3],
                        EduForm = (uint)s[4],
                        EduSource = (uint)s[5],
                        Faculty = s[6].ToString(),
                        Direction = s[7] as uint?,
                        Profile = s[8] as string
                    }).Single();

            var applications = connection.Select(
                DB_Table.ORDERS_HAS_APPLICATIONS,
                new string[] { "applications_id" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, number) }
                ).Join(
                connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id"),
                k1 => k1[0],
                k2 => k2[0],
                (s1, s2) => new { ApplID = (uint)s2[0], EntrID = (uint)s2[1] }
                ).Join(
                connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                k1 => k1.EntrID,
                k2 => k2[0],
                (s1, s2) => new { s1.ApplID, Name = s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString() }
                );

            string doc;
            List<string> paramaters = new List<string>
            {
                order.Date.ToShortDateString(),
                number,
                order.ProtocolNumber.ToString(),
                order.ProtocolDate.ToShortDateString()
            };

            if (order.Type == "hostel")
            {
                paramaters.AddRange(new string[]
                {
                    dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE,order.EduSource).ToLower(),
                    order.Faculty
                });

                doc = Utility.TempPath + "HostelOrder" + new Random().Next();
                DocumentCreator.Create(
                    Utility.DocumentsTemplatesPath + "HostelOrder.xml",
                    doc,
                    paramaters.ToArray(),
                    new IEnumerable<string[]>[] { applications.Select(s => new string[] { s.Name }) }
                    );
            }
            else
            {
                Tuple<string, string> dirNameCode = dbHelper.GetDirectionNameAndCode(order.Direction.Value);

                if (order.Type == "admission")
                    paramaters.Add(order.Date.Year.ToString());

                paramaters.AddRange(new string[]
                {
                    dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM, order.EduForm).ToLower() + " обучения" +
                    (order.EduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP) ? " по договорам с оплатой стоимости обучения" : ""),
                    order.Faculty,
                    dirNameCode.Item2,
                    dirNameCode.Item1,
                    order.Profile != null ? "Профиль: " : "",
                    order.Profile != null ? order.Profile + " - " + DB_Queries.GetProfileName(connection,order.Faculty,order.Direction.Value,order.Profile) : ""
                });

                if (order.Type == "admission")
                {
                    var dir_subjects = connection.Select(
                        DB_Table.ENTRANCE_TESTS,
                        new string[] { "subject_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Utility.CurrentCampaignID),
                            new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,order.Faculty),
                            new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,order.Direction)
                        }).Select(s => (uint)s[0]);

                    IEnumerable<DB_Queries.Mark> marks = DB_Queries.GetMarks(connection, applications.Select(s => s.ApplID), Utility.CurrentCampaignID);

                    var table = applications.Join(
                        marks.Join(
                            dir_subjects,
                            k1 => k1.SubjID,
                            k2 => k2,
                            (s1, s2) => s1
                            ).GroupBy(
                            k => Tuple.Create(k.ApplID, k.SubjID),
                            (k, g) => new { g.First().ApplID, Mark = g.Max(s => s.Value) }
                            ).GroupBy(
                            k => k.ApplID,
                            (k, g) => new { g.First().ApplID, Sum = g.Sum(s => s.Mark) }
                            ),
                        k1 => k1.ApplID,
                        k2 => k2.ApplID,
                        (s1, s2) => new string[] { s1.Name, s2.Sum.ToString() }
                        );

                    doc = Utility.TempPath + "AdmOrder" + new Random().Next();
                    DocumentCreator.Create(
                        Utility.DocumentsTemplatesPath + "AdmOrder.xml",
                        doc,
                        paramaters.ToArray(),
                        new IEnumerable<string[]>[] { table }
                        );
                }
                else
                {
                    doc = Utility.TempPath + "ExcOrder" + new Random().Next();
                    DocumentCreator.Create(
                        Utility.DocumentsTemplatesPath + "ExcOrder.xml",
                        doc,
                        paramaters.ToArray(),
                        new IEnumerable<string[]>[] { applications.Select(s => new string[] { s.Name }) }
                        );
                }
            }

            return doc + ".docx";
        }

        private static byte? MaxOrNull(IEnumerable<byte> list)
        {
            if (list.Count() != 0)
                return list.Max();

            return null;
        }
    }
}
