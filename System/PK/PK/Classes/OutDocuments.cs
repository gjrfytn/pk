using System.Collections.Generic;
using System.Linq;
using System;
using SharedClasses.DB;

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

                applData.HomePhone = entrantData[0] as string;
                applData.MobilePhone = entrantData[1] as string;
                applData.Password = entrantData[2].ToString();

                List<DocumentCreator.DocumentParameters> documents = new List<DocumentCreator.DocumentParameters>();

                if (moveJournal)
                {
                    documents.Add(new DocumentCreator.DocumentParameters(
                        Settings.DocumentsTemplatesPath + "MoveJournal.xml",
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
                        el[4] as string,
                        el[5] as DateTime?,
                        el[6] as DateTime?
                        ),
                    (k, g) => new Stream((uint)k.Item1, (uint)k.Item2, g.ToList().AsReadOnly())
                    );

                foreach (Stream stream in entrances)
                    inventoryTableParams[0].Add(new string[] { _Streams[Tuple.Create(stream.EduForm, stream.EduSource)].Item1 + " форма обучения" });

                IEnumerable<DB_Queries.Document> docs = DB_Queries.GetApplicationDocuments(connection, applID);

                inventoryTableParams[1].Add(new string[] { "Заявление на поступление" });
                if (entrances.Any(s => s.Directions.Any(e => e.AgreedDate != null && e.DisagreedDate == null)))
                    inventoryTableParams[1].Add(new string[] { "Заявление о согласии на зачисление" });

                if (docs.Any(s => s.Type == "identity"))
                    inventoryTableParams[1].Add(new string[] { "Копия паспорта" });

                var eduDoc = docs.Single(s => s.Type == "school_certificate" || s.Type == "high_edu_diploma" || s.Type == "middle_edu_diploma" || s.Type == "academic_diploma");
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

                if ((applData.Chernobyl ?? false) ||
                    (applData.Priority ?? false) ||
                    entrances.Any(s => s.EduSource == new DB_Helper(connection).GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceT)) ||
                    docs.Any(s =>
                    s.Type == "orphan" ||
                    s.Type == "disability" ||
                    //s.Type == "medical" ||
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

                if (inventory)
                    documents.Add(new DocumentCreator.DocumentParameters(
                        Settings.DocumentsTemplatesPath + "Inventory.xml",
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
                    byte redDiplomaMark = (byte)connection.Select(
                        DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                        new string[] { "institution_achievement_id" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("application_id", Relation.EQUAL, applID) }
                        ).Join(
                        connection.Select(
                            DB_Table.INSTITUTION_ACHIEVEMENTS,
                            new string[] { "id", "value" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Settings.CurrentCampaignID),
                                new Tuple<string, Relation, object>("category_id", Relation.EQUAL, 13)//TODO
                            }),
                        k1 => k1[0],
                        k2 => k2[0],
                        (s1, s2) => (ushort)s2[1]
                        ).SingleOrDefault();

                    if (percRecordBack)
                    {
                        var highEduDoc = docs.Single(s => s.Type == "high_edu_diploma");

                        string eduDocSeries = highEduDoc.Series;
                        string eduDocNumber = highEduDoc.Number;
                        ushort eduDocYear = (ushort)highEduDoc.Date.Value.Year;
                        string eduDocLevel = ""; //TODO

                        AddMastPercRecordBack(documents, connection, applData, entrances, eduDocSeries, eduDocNumber, eduDocYear, eduDocLevel, redDiplomaMark);
                    }

                    if (admAgreement)
                        AddMastAdmAgreement(documents, connection, applID, applData, redDiplomaMark);
                }
                else
                {
                    var marks = DB_Queries.GetMarks(connection, new uint[] { applID }, Settings.CurrentCampaignID).GroupBy(
                    k => k.SubjID,
                    (k, g) => Tuple.Create(k, g.Any(s => s.Checked) ? g.Where(s => s.Checked).Max(s => s.Value) : g.Max(s => s.Value))
                    );

                    if (percRecordBack)
                        AddBachPercRecordBack(documents, connection, applID, applData, entrances, marks);

                    if (admAgreement)
                        AddBachAdmAgreement(documents, connection, applID, applData, marks);
                }

                string doc = Settings.TempPath + "abitDocs" + new Random().Next();
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
                    Settings.DocumentsTemplatesPath + "PercRecordFace.xml",
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
                            Settings.DocumentsTemplatesPath + "PercRecordFaceM.xml",
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
                        if (entrance.Profile != null)
                            receiptTableParams[0].Add(new string[] { "     - " + entrance.Profile + " (" + entrance.Faculty + ") " +
                            DB_Queries.GetProfileName(connection,entrance.Faculty,entrance.DirectionID,entrance.Profile)});
                        else
                            receiptTableParams[0].Add(new string[] { "     - " +
                                dbHelper.GetDirectionShortName( entrance.Faculty, entrance.DirectionID)+
                                " (" + entrance.Faculty + ") " + dbHelper.GetDirectionNameAndCode(entrance.DirectionID).Item1});
                    }
                    receiptTableParams[0].Add(new string[] { "" });
                }

                documents.Add(new DocumentCreator.DocumentParameters(
                    Settings.DocumentsTemplatesPath + "Receipt.xml",
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
                        master ?"":"Проверить всю информацию можно в личном кабинете абитуриента по адресу: http://www.madi.ru/abit/list.php",
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
                ushort indAchValue = DB_Queries.GetApplicationIndAchMaxValue(connection, applID);

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

                IEnumerable<string> agreements = entrances.SelectMany(s1 =>
                s1.Directions.Where(s2 => s2.AgreedDate.HasValue).OrderBy(s => s.AgreedDate.Value).Select(s2 => dbHelper.GetDirectionShortName(s2.Faculty, s2.DirectionID))
                );

                List<string> parameters = new List<string>
                {
                    applData.LastName,
                    applData.FirstName,
                    applData.MiddleName,
                    !string.IsNullOrEmpty(applData.HomePhone)?applData.HomePhone:"-",
                    !string.IsNullOrEmpty(applData.MobilePhone)?applData.MobilePhone:"-",
                    applData.Quota?"XXX":"",
                    applData.Priority.Value?"XXX":"",
                    math.ToString(),
                    applData.Hostel?"XXX":"",
                    applData.PassingExam.Value?"XXX":"",
                    rus.ToString(),
                    applData.Chernobyl.Value?"XXX":"",
                    applData.MCADO.Value?"XXX":"",
                    phys.ToString(),
                    target?"XXX":"",
                    applData.Original?"XXX":"",
                    soc.ToString(),
                    foreign.ToString(),
                    indAchValue!=0?indAchValue.ToString():"",
                    (math+rus+phys).ToString(),
                    (math+rus+soc).ToString(),
                    (rus+soc+foreign).ToString(),
                    agreements.ElementAtOrDefault(0),
                    agreements.ElementAtOrDefault(1) //23
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
                            parameters.Add(dbHelper.GetDirectionShortName(entrance.Faculty, entrance.DirectionID));

                        count++;
                    }

                    while (count != 3)
                    {
                        parameters.Add("");
                        count++;
                    }
                }

                while (parameters.Count != 44)
                    parameters.Add("");

                documents.Add(new DocumentCreator.DocumentParameters(
                    Settings.DocumentsTemplatesPath + "PercRecordBack.xml",
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
                                new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Settings.CurrentCampaignID),
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
                            mark.Exam!=-1?mark.Exam.ToString():"",
                            mark.Exam!=-1?(redDiplomaMark +mark.Bonus+(mark.Exam!=-1?mark.Exam:0)).ToString():""
                        };

                        documents.Add(new DocumentCreator.DocumentParameters(
                            Settings.DocumentsTemplatesPath + "PercRecordBackM.xml",
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

                sum += DB_Queries.GetApplicationIndAchMaxValue(connection, applID);

                foreach (uint subj in DB_Queries.GetDirectionEntranceTests(connection, Settings.CurrentCampaignID, agreedDir.Item1, agreedDir.Item2))
                {
                    var mark = marks.SingleOrDefault(s => s.Item1 == subj);
                    if (mark != null)
                        sum += mark.Item2;
                }

                DB_Helper dbHelper = new DB_Helper(connection);
                documents.Add(new DocumentCreator.DocumentParameters(
                    Settings.DocumentsTemplatesPath + "AdmAgreement.xml",
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
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Settings.CurrentCampaignID),
                        new Tuple<string, Relation, object>("faculty",Relation.EQUAL,agreedDir.Item1),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,agreedDir.Item2),
                        new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,agreedDir.Item3)
                    }).Select(s => new { Exam = (short)s[0], Bonus = (ushort)s[1] }).Single();

                DB_Helper dbHelper = new DB_Helper(connection);
                documents.Add(new DocumentCreator.DocumentParameters(
                    Settings.DocumentsTemplatesPath + "AdmAgreement.xml",
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
                    subjects.AddRange(DB_Queries.GetDirectionEntranceTests(connection, Settings.CurrentCampaignID, entrance.Faculty, entrance.Direction));

                IEnumerable<DB_Queries.Exam> exams = DB_Queries.GetCampaignExams(connection, Settings.CurrentCampaignID).Where(s =>
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
                    Settings.DocumentsTemplatesPath + "ExamShedule.xml",
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

                dirShortName = new DB_Helper(connection).GetDirectionShortName(agreedDir.Item1, agreedDir.Item2);
            }
        }

        public static string RegistrationJournal(DB_Connector connection)
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

            var dateGroups = connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "id", "entrant_id", "registration_time", "status" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Settings.CurrentCampaignID) }
                ).Join(
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
                    MobilePhone = s2[2].ToString(),
                    RegTime = (DateTime)s1[2],
                    Status = s1[3].ToString()
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
                    s1.MobilePhone,
                    s1.RegTime,
                    s1.Status
                }).GroupBy(k => k.RegTime.Date);

            var applDocs = connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS).Join(
                    connection.Select(DB_Table.DOCUMENTS, "id", "type", "series", "number", "original_recieved_date"),
                k1 => k1[1],
                k2 => k2[0],
                (s1, s2) => new
                {
                    ApplID = (uint)s1[0],
                    DocID = (uint)s2[0],
                    DocType = s2[1].ToString(),
                    DocSeries = s2[2] as string,
                    DocNumber = s2[3] as string,
                    Original = s2[4] as DateTime?
                });

            List<DocumentCreator.DocumentParameters> documents = new List<DocumentCreator.DocumentParameters>();
            uint count = 1;
            foreach (var dateGroup in dateGroups)
            {
                List<string[]> data = new List<string[]>();
                foreach (var appl in dateGroup)
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
                            eduDocName = eduDoc.Original.HasValue ? "Справка" : "Копия справки";
                            break;
                        case "school_certificate":
                            eduDocName = eduDoc.Original.HasValue ? "Аттестат" : "Копия аттестата";
                            break;
                        case "middle_edu_diploma":
                        case "high_edu_diploma":
                            eduDocName = eduDoc.Original.HasValue ? "Диплом" : "Копия диплома";
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

                    if (appl.Status != "withdrawn")
                        data.Add(new string[]
                        {
                            count.ToString(),
                            appl.ApplID.ToString(),
                            appl.LastName+" "+ appl.FirstName +" "+appl.MiddleName,
                            string.Join(", ",idDoc.Where(o=>o.ToString()!=""))   + "\n\n"+appl.HomePhone+", "+appl.MobilePhone,
                            eduDocName+" "+eduDoc.DocSeries+eduDoc.DocNumber,
                            string.Join(", ",applEntr.Select(en=>streams[Tuple.Create((uint)en[0],(uint)en[1])]).Distinct()),
                            dateGroup.Key.ToShortDateString()
                        });
                    else
                        data.Add(new string[] { count.ToString(), appl.ApplID.ToString(), "ЗАБРАЛ ДОКУМЕНТЫ\n\n", "", "", "", "" });

                    count++;
                }

                documents.Add(new DocumentCreator.DocumentParameters(
                    Settings.DocumentsTemplatesPath + "RegistrationJournal.xml",
                    null,
                    null,
                    null,
                    new List<string[]>[] { data }
                    ));
            }

            string doc = Settings.TempPath + "registrationJournal" + new Random().Next();
            DocumentCreator.Create(doc, documents, false);
            return doc + ".docx";
        }

        public static string DirectionsPlaces(DB_Connector connection)
        {
            #region Contracts
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            #endregion

            DB_Helper dbHelper = new DB_Helper(connection);

            List<string[]> data = connection.Select(
                DB_Table.CAMPAIGNS_DIRECTIONS_DATA,
                new string[] { "direction_faculty", "direction_id", "places_budget_o", "places_budget_oz", "places_quota_o", "places_quota_oz" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Settings.CurrentCampaignID) }
                ).Select(s => new
                {
                    Faculty = s[0].ToString(),
                    Direction = (uint)s[1],
                    Places = (ushort)s[2] + (ushort)s[3] + (ushort)s[4] + (ushort)s[5]
                }).GroupJoin(
                connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "faculty_short_name", "direction_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("edu_source_id",Relation.NOT_EQUAL, dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE,DB_Helper.EduSourceP))
                    }),
                k1 => Tuple.Create(k1.Faculty, k1.Direction),
                k2 => Tuple.Create(k2[0].ToString(), (uint)k2[1]),
                (e, g) => new
                {
                    e.Faculty,
                    e.Direction,
                    ApplCount = g.Count(),
                    e.Places
                }).Where(s => s.ApplCount != 0).Join(
                connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name"),
                k1 => k1.Direction,
                k2 => k2[0],
                (s1, s2) => new { s1.Faculty, s1.Direction, s1.Places, Name = s2[1].ToString(), s1.ApplCount }
                ).Join(
                connection.Select(DB_Table.DIRECTIONS, "faculty_short_name", "direction_id", "short_name"),
                k1 => Tuple.Create(k1.Faculty, k1.Direction),
                k2 => Tuple.Create(k2[0].ToString(), (uint)k2[1]),
                (s1, s2) => new string[] { s2[2].ToString(), s1.Name, s1.ApplCount.ToString(), s1.Places.ToString() }
                ).ToList();

            string doc = Settings.TempPath + "directionsPlaces" + new Random().Next();
            DocumentCreator.Create(
                Settings.DocumentsTemplatesPath + "DirectionsPlaces.xml",
                doc,
                null,
                new List<string[]>[] { data }
                );

            return doc + ".docx";
        }

        public static string ProfilesPlaces(DB_Connector connection)
        {
            #region Contracts
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            #endregion

            DB_Helper dbHelper = new DB_Helper(connection);

            List<string[]> data = connection.Select(
                DB_Table.CAMPAIGNS_PROFILES_DATA,
                new string[] { "profiles_direction_faculty", "profiles_direction_id", "profiles_short_name", "places_paid_o", "places_paid_oz", "places_paid_z" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, Settings.CurrentCampaignID) }
                ).Select(s => new
                {
                    Faculty = s[0].ToString(),
                    Direction = (uint)s[1],
                    Profile = s[2].ToString(),
                    Places = (ushort)s[3] + (ushort)s[4] + (ushort)s[5]
                }).GroupJoin(
                connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES, "faculty_short_name", "direction_id", "profile_short_name"),
                k1 => Tuple.Create(k1.Faculty, k1.Direction, k1.Profile),
                k2 => Tuple.Create(k2[0].ToString(), (uint)k2[1], k2[2].ToString()),
                (e, g) => new { ShortName = e.Profile, ApplCount = g.Count(), e.Places }).Join(
                connection.Select(DB_Table.PROFILES, "short_name", "name"),
                k1 => k1.ShortName,
                k2 => k2[0],
                (s1, s2) => new string[] { s1.ShortName, s2[1].ToString(), s1.ApplCount.ToString(), s1.Places.ToString() }
                ).ToList();

            string doc = Settings.TempPath + "profilesPlaces" + new Random().Next();
            DocumentCreator.Create(
                Settings.DocumentsTemplatesPath + "DirectionsPlaces.xml",
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
                        ProtocolNumber = s[2] as ushort?,
                        ProtocolDate = s[3] as DateTime?,
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
                order.ProtocolNumber.HasValue?number:"______",
                order.ProtocolNumber.HasValue?order.ProtocolNumber.Value.ToString():"______",
                order.ProtocolDate.HasValue?order.ProtocolDate.Value.ToShortDateString():"______"
            };

            if (order.Type == "hostel")
                paramaters.AddRange(new string[]
                {
                    dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE,order.EduSource).ToLower(),
                    order.Faculty
                });
            else
            {
                if (order.Type == "admission")
                    paramaters.Add(order.Date.Year.ToString());

                Tuple<string, string> dirNameCode = dbHelper.GetDirectionNameAndCode(order.Direction.Value);
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
            }

            if (order.Type == "exception")
            {
                doc = Settings.TempPath + "ExcOrder" + new Random().Next();
                DocumentCreator.Create(
                    Settings.DocumentsTemplatesPath + "ExcOrder.xml",
                    doc,
                    paramaters.ToArray(),
                    new IEnumerable<string[]>[] { applications.OrderBy(s => s.Name).Select(s => new string[] { s.Name }) }
                    );
            }
            else
            {
                IEnumerable<Tuple<string, ushort>> table;
                if (order.Type == "hostel")
                {
                    if (order.Master)
                    {
                        List<Tuple<string, ushort>> result = new List<Tuple<string, ushort>>();

                        foreach (var appl in applications)
                        {
                            var orders = connection.Select(
                                DB_Table.ORDERS_HAS_APPLICATIONS,
                                new string[] { "orders_number" },
                                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, appl.ApplID) }
                                ).Join(
                                connection.Select(
                                    DB_Table.ORDERS,
                                    new string[] { "number", "type", "date", "direction_id", "profile_short_name" },
                                    new List<Tuple<string, Relation, object>>
                                    {
                                        new Tuple<string, Relation, object>("protocol_number",Relation.NOT_EQUAL,null),
                                        new Tuple<string, Relation, object>("type", Relation.NOT_EQUAL, "hostel"),
                                        new Tuple<string, Relation, object>("edu_form_id", Relation.EQUAL, dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM,DB_Helper.EduFormO)),
                                        new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, order.Faculty)
                                    }),
                                k1 => k1[0],
                                k2 => k2[0],
                                (s1, s2) => new { Type = s2[1].ToString(), Date = (DateTime)s2[2], Direction = (uint)s2[3], Profile = s2[4].ToString() }
                                ).GroupBy(
                                k => Tuple.Create(k.Direction, k.Profile),
                                (k, g) => new
                                {

                                });
                        }

                        table = result;

                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    if (order.Master)
                    {
                        var marks = connection.Select(
                            DB_Table.MASTERS_EXAMS_MARKS,
                            new string[] { "entrant_id", "mark", "bonus" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Settings.CurrentCampaignID),
                                new Tuple<string, Relation, object>("faculty",Relation.EQUAL,order.Faculty),
                                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,order.Direction.Value),
                                new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,order.Profile)
                            }).Select(s => new { EntrID = (uint)s[0], Exam = (short)s[1], Bonus = (ushort)s[2] });

                        var redDiplomaID_Bonus = connection.Select(
                            DB_Table.INSTITUTION_ACHIEVEMENTS,
                            new string[] { "id", "value" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Settings.CurrentCampaignID),
                                new Tuple<string, Relation, object>("category_id", Relation.EQUAL, 13)//TODO
                            }).Select(s => new { ID = (uint)s[0], Bonus = (ushort)s[1] }).Single();

                        var redDimplomaAppls = applications.Join(
                            connection.Select(
                                DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                                new string[] { "application_id", },
                                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("institution_achievement_id", Relation.EQUAL, redDiplomaID_Bonus.ID) }),
                            k1 => k1.ApplID,
                            k2 => k2[0],
                            (s1, s2) => new { s1.ApplID }
                            );

                        table = applications.Select(
                            s => new { s.EntrID, s.Name, RedDiplomaBonus = redDimplomaAppls.Any(a => a.ApplID == s.ApplID) ? redDiplomaID_Bonus.Bonus : 0 }
                            ).Join(
                            marks,
                            k1 => k1.EntrID,
                            k2 => k2.EntrID,
                            (s1, s2) => Tuple.Create(s1.Name, (ushort)(s2.Exam + s2.Bonus + s1.RedDiplomaBonus))
                            );
                    }
                    else
                    {
                        IEnumerable<uint> dir_subjects = DB_Queries.GetDirectionEntranceTests(connection, Settings.CurrentCampaignID, order.Faculty, order.Direction.Value);

                        IEnumerable<DB_Queries.Mark> marks = DB_Queries.GetMarks(connection, applications.Select(s => s.ApplID), Settings.CurrentCampaignID);

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
                            (s1, s2) => Tuple.Create(s1.Name, (ushort)s2.Sum)
                            );
                    }
                }
                string filename = order.Type == "admission" ? "AdmOrder" : "HostelOrder";
                doc = Settings.TempPath + filename + new Random().Next();
                DocumentCreator.Create(
                    Settings.DocumentsTemplatesPath + filename + ".xml",
                    doc,
                    paramaters.ToArray(),
                    new IEnumerable<string[]>[] { table.OrderByDescending(s => s.Item2).Select(s => new string[] { s.Item1, s.Item2.ToString() }) }
                    );
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
