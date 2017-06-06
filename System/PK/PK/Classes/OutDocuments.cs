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
                public uint EntrantID;
                public bool Hostel;
                public bool? MCADO;
                public bool? Chernobyl;
                public bool? PassingExam;
                public bool? Priority;
                public bool Original;
                public bool Quota;
                public string LastName;
                public string FirstName;
                public string MiddleName;
                public string HomePhone;
                public string MobilePhone;
                public string Password;
                public DateTime RegistrationTime;
                public DateTime? EditTime;
                public string RegistratorName;
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

                bool master = (bool)connection.Select(
                     DB_Table.APPLICATIONS,
                     new string[] { "master_appl" },
                     new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, applID) }
                     )[0][0];

                string[] medCertDirections;
                if (master)
                    medCertDirections = new string[] { "13.04.02", "23.04.01", "23.04.02", "23.04.03" };
                else
                    medCertDirections = new string[] { "13.03.02", "23.03.01", "23.03.02", "23.03.03", "23.05.01", "23.05.02" };

                object[] applDataBuf = connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "registration_time", "edit_time", "registrator_login", "needs_hostel", "mcado", "chernobyl", "passing_examinations", "priority_right" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, applID) }
                    )[0];

                ApplicationData applData = new ApplicationData
                {
                    Hostel = (bool)applDataBuf[3],
                    MCADO = applDataBuf[4] as bool?,
                    Chernobyl = applDataBuf[5] as bool?,
                    PassingExam = applDataBuf[6] as bool?,
                    Priority = applDataBuf[7] as bool?,
                    Original = false,
                    Quota = false,
                    RegistrationTime = (DateTime)applDataBuf[0],
                    EditTime = applDataBuf[1] as DateTime?,
                    RegistratorName = connection.Select(
                        DB_Table.USERS,
                        new string[] { "name" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("login", Relation.EQUAL, applDataBuf[2]) }
                        )[0][0].ToString().Split(' ')[0]
                };

                object[] entrant = connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "entrant_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, applID) }
                    ).Join(
                    connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                    k1 => k1[0],
                    k2 => k2[0],
                    (s1, s2) => s2).First();

                applData.EntrantID = (uint)entrant[0];
                applData.LastName = entrant[1].ToString();
                applData.FirstName = entrant[2].ToString();
                applData.MiddleName = entrant[3].ToString();

                object[] entrantData = connection.Select(
                   DB_Table.ENTRANTS,
                   new string[] { "home_phone", "mobile_phone", "personal_password" },
                   new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, applData.EntrantID) }
                   )[0];

                applData.HomePhone = entrantData[0].ToString();
                applData.MobilePhone = entrantData[1].ToString();
                applData.Password = entrantData[2].ToString();

                List<DocumentCreator.DocumentParameters> documents = new List<DocumentCreator.DocumentParameters>();

                if (moveJournal)
                {
                    documents.Add(new DocumentCreator.DocumentParameters(
                        Utility.DocumentsTemplatesPath + "MoveJournal.xml",
                        null,
                        null,
                        new string[]
                        {
                            applData.LastName.ToUpper(),
                            applData.FirstName,
                            applData.MiddleName,
                            applID.ToString()+(master?"м":""),
                            System.Windows.Forms.SystemInformation.ComputerName,
                            applData.RegistratorName,
                            applData.RegistrationTime.ToString()
                        },
                        null
                        ));
                }

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

                if ((applData.Chernobyl ?? false) || (applData.Priority ?? false) || docs.Any(s =>
                    s.Type == "orphan" ||
                    s.Type == "disability" ||
                    s.Type == "medical" ||
                    s.Type == "olympic" ||
                    s.Type == "olympic_total" ||
                    s.Type == "ukraine_olympic" ||
                    s.Type == "international_olympic"
                ))
                    inventoryTableParams[1].Add(new string[] { "Направление ПК" }); //TODO

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

                if (inventory)
                    documents.Add(new DocumentCreator.DocumentParameters(
                        Utility.DocumentsTemplatesPath + "Inventory.xml",
                        null,
                        null,
                        new string[]
                        {
                            applID.ToString()+(master?"м":""),
                            applData.LastName.ToUpper(),
                            (applData.FirstName+" "+applData.MiddleName).ToUpper(),
                            applData.RegistratorName,
                            System.Windows.Forms.SystemInformation.ComputerName,
                            applData.RegistrationTime.ToString()
                        },
                        inventoryTableParams
                        ));

                if (percRecordFace)
                    if (master)
                        AddMastPercRecordFace(documents, connection, entrances, applID, applData);
                    else
                        AddBachPercRecordFace(documents, inventoryTableParams[0], applID, applData);

                if (receipt)
                {
                    AddReceipt(documents, connection, inventoryTableParams[1], entrances, applID, applData, master);

                    object[] buf = connection.Select(
                        DB_Table.APPLICATIONS,
                        new string[] { "passing_examinations", "registration_time" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, applID) }
                        )[0];

                    bool? passingExams = buf[0] as bool?;
                    if (passingExams.HasValue && passingExams.Value)
                        AddExamShedule(documents, connection, applID, (DateTime)buf[1]);
                }

                if (master)
                {
                    byte redDiplomaMark = 0; //TODO

                    if (percRecordBack)
                    {
                        string eduDocSeries = "5555"; //TODO
                        string eduDocNumber = "12341234";
                        ushort eduDocYear = 2020;
                        string eduDocLevel = "TODO";
                        AddMastPercRecordBack(documents, connection, applData, entrances, eduDocSeries, eduDocNumber, eduDocYear, eduDocLevel, redDiplomaMark);
                    }

                    if (admAgreement)
                        AddMastAdmAgreement(documents, connection, applID, applData, redDiplomaMark);
                }
                else
                {
                    var marks = DB_Queries.GetMarks(connection, new uint[] { applID }, Utility.CurrentCampaignID).GroupBy(
                    k => k.SubjID,
                    (k, g) => Tuple.Create(k, g.Any(s => s.Checked) ? g.Where(s => s.Checked).Max(s => s.Value) : g.Max(s => s.Value))
                    );

                    if (percRecordBack)
                        AddBachPercRecordBack(documents, connection, applID, applData, entrances, marks);

                    if (admAgreement)
                        AddBachAdmAgreement(documents, connection, applID, applData, marks);
                }

                string doc = Utility.TempPath + "abitDocs" + new Random().Next();
                DocumentCreator.Create(doc, documents, true);
                return doc + ".docx";
            }

            private static void AddBachPercRecordFace(List<DocumentCreator.DocumentParameters> documents, List<string[]> streamsTableParams, uint applID, ApplicationData applData)
            {
                List<string> parameters = new List<string>
                {
                    applData.LastName.ToUpper(),
                    applData.FirstName[0].ToString(),
                    applData.MiddleName.Length!=0?applData.MiddleName[0].ToString():"",
                    applID.ToString(),
                    applData. LastName,
                    applData.FirstName,
                    applData. MiddleName,
                    applData.RegistrationTime.Year.ToString()
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

            private static void AddMastPercRecordFace(List<DocumentCreator.DocumentParameters> documents, DB_Connector connection, IEnumerable<Stream> entrances, uint applID, ApplicationData applData)
            {
                DB_Helper dbHelper = new DB_Helper(connection);
                foreach (Stream stream in entrances)
                    foreach (Stream.Entrance entrance in stream.Directions)
                    {
                        string[] parameters =
                            {
                            applData.LastName,
                            applData.FirstName[0].ToString(),
                            applData.MiddleName.Length!=0?applData.MiddleName[0].ToString():"",
                            applID.ToString()+"м",
                            entrance.Profile,
                            dbHelper.GetDirectionNameAndCode(entrance.DirectionID).Item1,
                            DB_Queries.GetProfileName(connection, entrance.Faculty, entrance.DirectionID, entrance.Profile),
                            dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM, stream.EduForm),
                            dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE, stream.EduSource),
                            applData. LastName,
                            applData.FirstName,
                            applData. MiddleName,
                            applData.RegistrationTime.Year.ToString()
                        };

                        documents.Add(new DocumentCreator.DocumentParameters(
                            Utility.DocumentsTemplatesPath + "PercRecordFaceM.xml",
                            null,
                            null,
                            parameters,
                            null
                            ));
                    }
            }

            private static void AddReceipt(
                List<DocumentCreator.DocumentParameters> documents,
                DB_Connector connection,
                List<string[]> entrDocumentsTableParams,
                IEnumerable<Stream> entrances,
                uint applID,
                ApplicationData applData,
                bool master)
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
                        applID.ToString()+(master?"м":""),
                        applData.LastName.ToUpper(),
                        (applData.FirstName+" "+applData.MiddleName).ToUpper(),
                        applData.RegistratorName,
                        System.Windows.Forms.SystemInformation.ComputerName,
                        applData.RegistrationTime.ToString(),
                        applData.EditTime.HasValue?applData.EditTime.ToString():"нет",
                        master ?"":"Проверить всю информацию можно в личном кабинете абитуриента по адресу: www.priem-madi.ru/тут_что-то_будет",
                        master ?"":"Логин: "+ applData.EntrantID.ToString(),
                        master ?"":"Пароль: "+ applData.Password
                    },
                    receiptTableParams
                    ));
            }

            private static void AddBachPercRecordBack(
                List<DocumentCreator.DocumentParameters> documents,
                DB_Connector connection,
                uint applID,
                ApplicationData applData,
                IEnumerable<Stream> entrances,
                IEnumerable<Tuple<uint, byte>> marks)
            {
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
                List<string> parameters = new List<string>
                {
                    applData.LastName,
                    applData.FirstName,
                    applData.MiddleName,
                    applData.HomePhone,
                    applData.MobilePhone,
                    applData.Quota?"+":"",
                    applData.Priority.Value?"+":"",
                    math.ToString(),
                    applData.Hostel?"+":"",
                    applData.PassingExam.Value?"+":"",
                    rus.ToString(),
                    applData.Chernobyl.Value?"+":"",
                    applData.MCADO.Value?"+":"",
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

                while (parameters.Count != 42)
                    parameters.Add("");

                documents.Add(new DocumentCreator.DocumentParameters(
                    Utility.DocumentsTemplatesPath + "PercRecordBack.xml",
                    null,
                    null,
                    parameters.ToArray(),
                    null
                    ));
            }

            private static void AddMastPercRecordBack(
                List<DocumentCreator.DocumentParameters> documents,
                DB_Connector connection,
                ApplicationData applData,
                IEnumerable<Stream> entrances,
                string eduDocSeries,
                string eduDocNumber,
                ushort eduDocYear,
                string eduDocLevel,
                byte redDiplomaMark)
            {
                DB_Helper dbHelper = new DB_Helper(connection);
                foreach (Stream stream in entrances)
                    foreach (Stream.Entrance entrance in stream.Directions)
                    {
                        var mark = connection.Select(
                            DB_Table.MASTERS_EXAMS_MARKS,
                            new string[] { "mark", "bonus" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("entrant_id",Relation.EQUAL,applData.EntrantID),
                                new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Utility.CurrentCampaignID),
                                new Tuple<string, Relation, object>("faculty",Relation.EQUAL,entrance.Faculty),
                                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,entrance.DirectionID),
                                new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,entrance.Profile)
                            }).Select(s => new { Exam = (short)s[0], Bonus = (ushort)s[1] }).Single();

                        string[] parameters =
                            {
                            applData.LastName,
                            applData.FirstName,
                            applData.MiddleName,
                            dbHelper.GetDirectionNameAndCode(entrance.DirectionID).Item1,
                            DB_Queries.GetProfileName(connection, entrance.Faculty, entrance.DirectionID, entrance.Profile),
                            entrance.Profile,
                            applData.HomePhone,
                            applData.MobilePhone,
                            eduDocSeries,
                            eduDocNumber,
                            eduDocYear.ToString(),
                            eduDocLevel,
                            applData.Quota?"Да":"Нет",
                            redDiplomaMark.ToString(),
                            applData.Hostel?"Да":"Нет",
                            mark.Bonus.ToString(),
                            mark.Exam!=-1?mark.Exam.ToString():"нет",
                            (redDiplomaMark +mark.Bonus+(mark.Exam!=-1?mark.Exam:0)).ToString()
                        };

                        documents.Add(new DocumentCreator.DocumentParameters(
                            Utility.DocumentsTemplatesPath + "PercRecordBackM.xml",
                            null,
                            null,
                            parameters,
                            null
                            ));
                    }
            }

            private static void AddBachAdmAgreement(
                List<DocumentCreator.DocumentParameters> documents,
                DB_Connector connection,
                uint applID,
                ApplicationData applData,
                IEnumerable<Tuple<uint, byte>> marks)
            {
                Tuple<string, uint, string, uint, DateTime> agreedDir;
                string dirShortName;
                GetAgreedDirData(connection, applID, out agreedDir, out dirShortName);

                ushort sum = 0;
                foreach (uint subj in DB_Queries.GetDirectionEntranceTests(connection, Utility.CurrentCampaignID, agreedDir.Item1, agreedDir.Item2))
                {
                    var mark = marks.SingleOrDefault(s => s.Item1 == subj);
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
                        dbHelper.GetDirectionNameAndCode(agreedDir.Item2).Item1+" ("+dirShortName+")",
                        dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM,agreedDir.Item4),
                        applID.ToString(),
                        sum.ToString(),
                        applData.LastName.ToUpper()+" "+applData.FirstName[0]+"."+(applData.MiddleName.Length!=0?applData.MiddleName[0]+".":""),
                        agreedDir.Item5.ToShortDateString()
                    },
                    null
                    ));
            }

            private static void AddMastAdmAgreement(
               List<DocumentCreator.DocumentParameters> documents,
               DB_Connector connection,
               uint applID,
               ApplicationData applData,
               byte redDiplomaMark)
            {
                Tuple<string, uint, string, uint, DateTime> agreedDir;
                string dirShortName;
                GetAgreedDirData(connection, applID, out agreedDir, out dirShortName);

                var mark = connection.Select(
                    DB_Table.MASTERS_EXAMS_MARKS,
                    new string[] { "mark", "bonus" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("entrant_id",Relation.EQUAL,applData.EntrantID),
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Utility.CurrentCampaignID),
                        new Tuple<string, Relation, object>("faculty",Relation.EQUAL,agreedDir.Item1),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,agreedDir.Item2),
                        new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,agreedDir.Item3)
                    }).Select(s => new { Exam = (short)s[0], Bonus = (ushort)s[1] }).Single();

                DB_Helper dbHelper = new DB_Helper(connection);
                documents.Add(new DocumentCreator.DocumentParameters(
                    Utility.DocumentsTemplatesPath + "AdmAgreement.xml",
                    null,
                    null,
                    new string[]
                    {
                        (applData.LastName + " " + applData.FirstName + " " + applData.MiddleName).ToUpper(),
                        dbHelper.GetDirectionNameAndCode(agreedDir.Item2).Item1+" ("+dirShortName+")",
                        dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM,agreedDir.Item4),
                        applID.ToString()+"м",
                        ((mark.Exam!=-1?mark.Exam:0)+mark.Bonus+redDiplomaMark).ToString(),
                        applData.LastName.ToUpper()+" "+applData.FirstName[0]+"."+(applData.MiddleName.Length!=0?applData.MiddleName[0]+".":""),
                        agreedDir.Item5.ToShortDateString()
                    },
                    null
                    ));
            }

            private static void AddExamShedule(
                List<DocumentCreator.DocumentParameters> documents,
                DB_Connector connection,
                uint applID,
                DateTime registrationTime)
            {
                var entrances = connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "faculty_short_name", "direction_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL,applID),
                    }).Select(s => new { Faculty = s[0].ToString(), Direction = (uint)s[1] });

                List<uint> subjects = new List<uint>();

                foreach (var entrance in entrances)
                    subjects.AddRange(DB_Queries.GetDirectionEntranceTests(connection, Utility.CurrentCampaignID, entrance.Faculty, entrance.Direction));

                IEnumerable<DB_Queries.Exam> exams = DB_Queries.GetCampaignExams(connection, Utility.CurrentCampaignID).Where(s =>
                registrationTime >= s.RegStartDate && registrationTime < s.RegEndDate + new TimeSpan(1, 0, 0, 0)
                );

                DB_Helper dbHelper = new DB_Helper(connection);
                IEnumerable<string[]> table = subjects.Distinct().Join(
                    exams,
                    k1 => k1,
                    k2 => k2.SubjID,
                    (s1, s2) => new { Subj = s1, s2.Date }
                    ).OrderBy(s => s.Date).Select(s => new string[] { dbHelper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, s.Subj), s.Date.ToShortDateString() });

                documents.Add(new DocumentCreator.DocumentParameters(
                    Utility.DocumentsTemplatesPath + "ExamShedule.xml",
                    null,
                    null,
                    null,
                    new List<string[]>[] { table.ToList() }
                    ));
            }

            private static void GetAgreedDirData(DB_Connector connection, uint applID, out Tuple<string, uint, string, uint, DateTime> agreedDir, out string dirShortName)
            {
                agreedDir = connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "faculty_short_name", "direction_id", "profile_short_name", "edu_form_id", "is_agreed_date" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL,applID),
                        new Tuple<string, Relation, object>("is_agreed_date", Relation.NOT_EQUAL,null),
                        new Tuple<string, Relation, object>("is_disagreed_date", Relation.EQUAL,null),
                    }).Select(s => Tuple.Create(s[0].ToString(), (uint)s[1], s[2] as string, (uint)s[3], (DateTime)s[4])).Single();

                dirShortName = connection.Select(
                    DB_Table.DIRECTIONS,
                    new string[] { "short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("direction_id", Relation.EQUAL,agreedDir.Item2)
                    })[0][0].ToString();
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
                }).Join(
                connection.Select(
                DB_Table.ENTRANTS_VIEW,
                "id", "last_name", "first_name", "middle_name"
                ),
                k1 => k1.EntrID,
                k2 => k2[0],
                (s1, s2) => new
                {
                    s1.ApplID,
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
                });

            foreach (var appl in applications)
            {
                object[] idDoc = connection.Select(
                    DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA,
                    new string[] { "reg_index", "reg_region", "reg_district", "reg_town", "reg_street", "reg_house", "reg_flat" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id",Relation.EQUAL,applDocs.Single(d=>d.ApplID==appl.ApplID&&d.DocType=="identity").DocID)
                    })[0];

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
                    });

                data.Add(new string[]
                {
                    appl.ApplID.ToString(),
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

            Dictionary<uint, string> forms = new Dictionary<uint, string>
            {
                { 11, "очной формы"},
                { 12, "очно-заочной (вечерней) формы"},
                { 10, "заочной формы" }
            };

            DB_Helper dbHelper = new DB_Helper(connection);

            var order = connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "type", "date", "protocol_number", "protocol_date", "edu_form_id", "edu_source_id", "faculty_short_name", "direction_id", "profile_short_name", "campaign_id" },
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
                        Profile = s[8] as string,
                        Master = dbHelper.IsMasterCampaign((uint)s[9])
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
                (s1, s2) => new { s1.ApplID, s1.EntrID, Name = s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString() }
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
                    new IEnumerable<string[]>[] { applications.OrderBy(s => s.Name).Select(s => new string[] { s.Name }) }
                    );
            }
            else
            {
                Tuple<string, string> dirNameCode = dbHelper.GetDirectionNameAndCode(order.Direction.Value);

                if (order.Type == "admission")
                    paramaters.Add(order.Date.Year.ToString());

                paramaters.AddRange(new string[]
                {
                    forms[order.EduForm] + " обучения" +
                    (order.EduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP) ? " по договорам с оплатой стоимости обучения" : ""),
                    order.Faculty,
                    dirNameCode.Item2,
                    dirNameCode.Item1,
                    order.Master?"Магистерская программа: " :(order.Profile != null ? "Профиль: " : ""),
                    order.Profile != null ? order.Profile + " - " + DB_Queries.GetProfileName(connection,order.Faculty,order.Direction.Value,order.Profile).Split('|')[0] : ""
                });

                if (order.Type == "admission")
                {
                    IEnumerable<string[]> table;
                    if (order.Master)
                    {
                        var marks = connection.Select(
                            DB_Table.MASTERS_EXAMS_MARKS,
                            new string[] { "entrant_id", "mark", "bonus" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Utility.CurrentCampaignID),
                                new Tuple<string, Relation, object>("faculty",Relation.EQUAL,order.Faculty),
                                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,order.Direction.Value),
                                new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,order.Profile)
                            }).Select(s => new { EntrID = (uint)s[0], Exam = (short)s[1], Bonus = (ushort)s[2] });

                        table = applications.Join(
                            marks,
                            k1 => k1.EntrID,
                            k2 => k2.EntrID,
                            (s1, s2) => new string[] { s1.Name, (s2.Exam + s2.Bonus).ToString() } //TODO диплом с отличием
                            );
                    }
                    else
                    {
                        IEnumerable<uint> dir_subjects = DB_Queries.GetDirectionEntranceTests(connection, Utility.CurrentCampaignID, order.Faculty, order.Direction.Value);

                        IEnumerable<DB_Queries.Mark> marks = DB_Queries.GetMarks(connection, applications.Select(s => s.ApplID), Utility.CurrentCampaignID);

                        table = applications.Join(
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
                            (s1, s2) => new { s1.Name, s2.Sum }
                            ).OrderByDescending(s => s.Sum).Select(s => new string[] { s.Name, s.Sum.ToString() });
                    }

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
                        new IEnumerable<string[]>[] { applications.OrderBy(s => s.Name).Select(s => new string[] { s.Name }) }
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
