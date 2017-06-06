using System.Collections.Generic;
using System.Linq;
using PK.Classes.FIS_ExportClasses;

namespace PK.Classes
{
    static class FIS_Packager
    {
        struct EduSourcePlaces
        {
            public readonly ushort O;
            public readonly ushort OZ;
            public readonly ushort Z;

            public EduSourcePlaces(ushort o, ushort oz, ushort z)
            {
                O = o;
                OZ = o;
                Z = z;
            }
        }

        private static readonly Dictionary<uint, char> _EduFormLiterals = new Dictionary<uint, char>()
        {
            { 11, 'Д' }, { 12, 'В' }, { 10, 'З' }
        };

        private static readonly Dictionary<uint, string> _EduSourceLiterals = new Dictionary<uint, string>()
        {
            { 14, "ОО" }, { 15, "СН" }, { 16, "ЦН" }, { 20, "КВ" }
        };

        private static readonly Dictionary<string, uint> _StatusesMap = new Dictionary<string, uint>()
        {
            { "new", 2 },
            { "adm_budget", 4 },
            { "adm_paid", 4 },
            { "adm_both", 4 },
            { "withdrawn", 6 },
        };

        public static PackageData MakePackage(DB_Connector connection, uint campaignID, bool campaignData, bool applications, bool orders)
        {
            #region Contracts
            if (connection == null)
                throw new System.ArgumentNullException(nameof(connection));
            #endregion

            return new PackageData(
                campaignData ? PackCampaignInfo(connection, campaignID) : null,
                campaignData ? PackAdmissionInfo(connection, campaignID) : null,
                campaignData ? PackInstitutionAchievements(connection, campaignID) : null,
                campaignData ? PackTargetOrganizations(connection) : null,
                applications ? PackApplications(connection, campaignID) : null,
                orders ? PackOrders(connection, campaignID) : null
                );
        }

        private static CampaignInfo PackCampaignInfo(DB_Connector connection, uint campaignID)
        {
            List<Campaign> campaigns = new List<Campaign>();

            foreach (object[] row in connection.Select(
                DB_Table.CAMPAIGNS,
                new string[] { "name", "start_year", "end_year", "status_id", "type_id" },
                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("id", Relation.EQUAL, campaignID) }
                ))
            {
                List<EducationFormID> eduForms = new List<EducationFormID>();
                List<EducationLevelID> eduLevels = new List<EducationLevelID>();
                foreach (object[] diRow in connection.Select(
                    DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS,
                    new string[] { "dictionaries_items_dictionary_id", "dictionaries_items_item_id" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, campaignID) }
                    ))
                    if ((uint)diRow[0] == (uint)FIS_Dictionary.EDU_FORM)
                        eduForms.Add(new EducationFormID((uint)diRow[1]));
                    else
                        eduLevels.Add(new EducationLevelID((uint)diRow[1]));

                campaigns.Add(new Campaign(
                    new TUID(campaignID),
                    row[0].ToString(),
                    (uint)row[1],
                    (uint)row[2],
                    eduForms,
                    (uint)row[3],
                    eduLevels,
                    (uint)row[4]
                    ));
            }

            if (campaigns.Count != 0)
                return new CampaignInfo(campaigns);

            return null;
        }

        private static AdmissionInfo PackAdmissionInfo(DB_Connector connection, uint campaignID)
        {
            List<AVItem> admissionVolumes = new List<AVItem>();
            List<CompetitiveGroup> competitiveGroups = new List<CompetitiveGroup>();
            foreach (var admData in connection.Select(
                DB_Table.CAMPAIGNS_DIRECTIONS_DATA,
                new string[] { "direction_id", "places_budget_o", "places_budget_oz", "places_quota_o", "places_quota_oz" },
                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, campaignID) }
                ).GroupBy(
                k => k[0],
                (k, g) => new
                {
                    CampID = campaignID,
                    DirID = (uint)k,
                    BudgetPlaces = new EduSourcePlaces((ushort)g.Sum(s => (ushort)s[1]), (ushort)g.Sum(s => (ushort)s[2]), 0),
                    QuotaPlaces = new EduSourcePlaces((ushort)g.Sum(s => (ushort)s[3]), (ushort)g.Sum(s => (ushort)s[4]), 0)
                }))
            {
                EduSourcePlaces paidPlaces = GetPaidPlaces(connection, admData.CampID, admData.DirID);
                EduSourcePlaces targetPlaces = GetTargetPlaces(connection, admData.CampID, admData.DirID);

                DB_Helper dbHelper = new DB_Helper(connection);
                uint levelID = Utility.DirCodesEduLevels[dbHelper.GetDirectionNameAndCode(admData.DirID).Item2.Split('.')[1]];

                admissionVolumes.Add(new AVItem(
                    new TUID(admData.CampID.ToString() + admData.DirID.ToString()),
                    new TUID(admData.CampID),
                    levelID,
                    admData.DirID,
                    admData.BudgetPlaces.O, admData.BudgetPlaces.OZ, admData.BudgetPlaces.Z,
                    paidPlaces.O, paidPlaces.OZ, paidPlaces.Z,
                    targetPlaces.O, targetPlaces.OZ, targetPlaces.Z,
                    admData.QuotaPlaces.O, admData.QuotaPlaces.OZ, admData.QuotaPlaces.Z
                    ));

                foreach (CompetitiveGroupItem.Variants v in System.Enum.GetValues(typeof(CompetitiveGroupItem.Variants)))
                {
                    uint eduForm, eduSource;
                    ushort places;

                    ChooseFormAndSourceAndPlaces(
                        v, admData.BudgetPlaces, paidPlaces, admData.QuotaPlaces, targetPlaces,
                        out eduForm, out eduSource, out places
                        );

                    if (places != 0)
                    {
                        TUID compGroupUID = ComposeCompGroupUID(admData.CampID, admData.DirID, eduForm, eduSource);

                        List<EntranceTestItem> entranceTests = new List<EntranceTestItem>();
                        foreach (object[] etRow in connection.Select(
                            DB_Table.ENTRANCE_TESTS,
                            new string[] { "subject_id", "priority" },
                            new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("direction_id", Relation.EQUAL, admData.DirID) }
                            ))
                            if (!entranceTests.Any(s => s.EntranceTestSubject.SubjectID.Value == (uint)etRow[0]))
                                entranceTests.Add(new EntranceTestItem(
                                    new TUID(compGroupUID.Value + etRow[0].ToString()),
                                    1,//TODO ?
                                    (ushort)etRow[1],
                                    new TEntranceTestSubject((uint)etRow[0]),
                                    null //TODO Добавить!
                                    ));

                        competitiveGroups.Add(new CompetitiveGroup(
                            compGroupUID,
                            new TUID(admData.CampID),
                            dbHelper.GetDirectionNameAndCode(admData.DirID).Item2 + " " + _EduSourceLiterals[eduSource] + " " + _EduFormLiterals[eduForm],
                            levelID,
                            eduSource,
                            eduForm,
                            admData.DirID,
                            null,
                            null, //TODO ?
                            new CompetitiveGroupItem((CompetitiveGroupItem.Variants)v, places),
                            null,//TODO ?
                            null,//TODO ?
                            entranceTests
                            ));
                    }
                }
            }

            if (admissionVolumes.Count != 0)
                return new AdmissionInfo(admissionVolumes, null, competitiveGroups.Count != 0 ? competitiveGroups : null);//TODO null

            return null;
        }

        private static List<InstitutionAchievement> PackInstitutionAchievements(DB_Connector connection, uint campaignID)
        {
            List<InstitutionAchievement> achievements = new List<InstitutionAchievement>();

            foreach (object[] row in connection.Select(
                DB_Table.INSTITUTION_ACHIEVEMENTS,
                new string[] { "id", "name", "category_id", "value" },
                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, campaignID) }
                ))
                achievements.Add(new InstitutionAchievement(
                    new TUID(row[0].ToString()),
                    row[1].ToString(),
                    (uint)row[2],
                    (ushort)row[3],
                    new TUID(campaignID)
                    ));

            if (achievements.Count != 0)
                return achievements;

            return null;
        }

        private static List<TargetOrganizationImp> PackTargetOrganizations(DB_Connector connection)
        {
            List<TargetOrganizationImp> organizations = new List<TargetOrganizationImp>();

            foreach (object[] row in connection.Select(DB_Table.TARGET_ORGANIZATIONS, "id", "name"))
                organizations.Add(new TargetOrganizationImp(new TUID(row[0].ToString()), row[1].ToString()));

            if (organizations.Count != 0)
                return organizations;

            return null;
        }

        private static List<Application> PackApplications(DB_Connector connection, uint campaignID)
        {
            List<Application> applications = new List<Application>();

            var applicationsBD = connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "id", "entrant_id", "registration_time", "needs_hostel", "status", "comment", "special_conditions" },
                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, campaignID) }
                ).Join(
                connection.Select(DB_Table.ENTRANTS, "id", "custom_information", "email"),
                k1 => k1[1],
                k2 => k2[0],
                (s1, s2) => new
                {
                    ID = (uint)s1[0],
                    Entr_ID = (uint)s1[1],
                    RegTime = (System.DateTime)s1[2],
                    Hostel = (bool)s1[3],
                    Status = _StatusesMap[s1[4].ToString()],
                    Comment = s1[5] as string,
                    SpecialCond = (bool)s1[6],
                    Info = s2[1] as string,
                    Email = s2[2].ToString()
                });

            IEnumerable<DB_Queries.Mark> marks = DB_Queries.GetMarks(connection, applicationsBD.Select(s => s.ID), campaignID);

            foreach (var appl in applicationsBD)
            {
                var finSourceEduForms = connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "direction_id", "edu_form_id", "edu_source_id", "target_organization_id", "is_agreed_date", "is_disagreed_date" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("application_id", Relation.EQUAL, appl.ID) }
                    ).GroupBy(
                    k => System.Tuple.Create((uint)k[0], k[1], (uint)k[2]),
                    e => new
                    {
                        TargetOrg = e[3] as uint?, //TODO Что с целевой орг. делать?
                        AgreedDate = e[4] as System.DateTime?,
                        DisagreedDate = e[5] as System.DateTime?
                    },
                    (k, g) =>
                    {
                        dynamic buf;
                        if (g.All(s => s.DisagreedDate != null))
                            buf = g.First(s => s.DisagreedDate == g.Max(m => m.DisagreedDate));
                        else
                            buf = g.OrderBy(s => s.AgreedDate).Last(s => s.DisagreedDate == null);

                        return new
                        {
                            DirID = k.Item1,
                            EduSource = k.Item3,
                            FSEF = new FinSourceEduForm(
                                ComposeCompGroupUID(
                                    campaignID,
                                    k.Item1,
                                    //Utility.DirCodesEduLevels[new DB_Helper(connection).GetDirectionNameAndCode(k.Item1).Item2.Split('.')[1]],
                                    (uint)k.Item2,
                                    k.Item3
                                    ),
                                buf.TargetOrg != null ? new TUID(buf.TargetOrg) : null,
                                buf.AgreedDate != null ? new TDateTime(buf.AgreedDate) : null,
                                buf.DisagreedDate != null ? new TDateTime(buf.DisagreedDate) : null
                                )
                        };
                    });

                //Сохранять все документы?

                //Подал на квоту
                //Просмотреть таблицу льгот
                //Просмотреть таблицу документов

                List<ApplicationCommonBenefit> benefits = null;

                //if (finSourceEduForms.Any(s => s.EduSource == 20))//TODO
                //{
                //    List<object[]> benefits_bd= connection.Select(
                //           DB_Table.APPLICATION_COMMON_BENEFITS,
                //           new string[] { "id", "document_type_id", "reason_document_id", "allow_education_document_id", "benefit_kind_id" },
                //           new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("application_id", Relation.EQUAL, appl.ID) }
                //           );

                //    if(benefits_bd.Count!=0)
                //        foreach (object[] row in benefits_bd)
                //            benefits.Add(new ApplicationCommonBenefit(
                //                new TUID(row[0].ToString()),
                //                new TUID(
                //                    ));
                //}


                List<IndividualAchievement> achievements = null;

                var achievementsBD = connection.Select(
                    DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                    new string[] { "id", "institution_achievement_id", "document_id" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("application_id", Relation.EQUAL, appl.ID) }
                    ).Select(s => new { ID = (uint)s[0], InstAchID = (uint)s[1], DocID = (uint)s[2] });

                if (achievementsBD.Any())
                {
                    Dictionary<uint, ushort> instAch = connection.Select(
                        DB_Table.INSTITUTION_ACHIEVEMENTS,
                        new string[] { "id", "value" },
                        new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, campaignID) }
                        ).ToDictionary(k => (uint)k[0], e => (ushort)e[1]);

                    achievements = new List<IndividualAchievement>(instAch.Count);
                    foreach (var ach in achievementsBD)
                        achievements.Add(new IndividualAchievement(
                            new TUID(ach.ID),
                            new TUID(ach.InstAchID),
                            new TUID(ach.DocID),
                            instAch[ach.InstAchID]
                            ));//TODO Преимущественное право?
                }

                ApplicationDocuments docs = PackApplicationDocuments(connection, appl.ID);

                List<EntranceTestResult> testsResults = PackApplicationTestResults(
                    connection,
                    campaignID,
                    finSourceEduForms.Select(s => System.Tuple.Create(s.DirID, s.FSEF.CompetitiveGroupUID)),
                    marks.Where(s => s.ApplID == appl.ID)
                    );

                applications.Add(new Application(
                    new TUID(appl.ID),
                    appl.ID.ToString(),
                    new Entrant(
                        new TUID(appl.Entr_ID),
                        docs.IdentityDocument.LastName,
                        docs.IdentityDocument.FirstName,
                        docs.IdentityDocument.GenderID.Value,
                        new EmailOrMailAddress(appl.Email.ToString()),
                        docs.IdentityDocument.MiddleName,
                        appl.Info
                        ),
                    new TDateTime(appl.RegTime),
                    appl.Hostel,
                    appl.Status,
                    finSourceEduForms.Select(s => s.FSEF).ToList(),
                    docs,
                    appl.Comment,
                    benefits,//TODO 
                    testsResults,
                    achievements
                    ));
            }

            if (applications.Count != 0)
                return applications;

            return null;
        }

        private static Orders PackOrders(DB_Connector connection, uint campaignID)
        {
            List<OrderOfAdmission> admissionOrders = new List<OrderOfAdmission>();
            List<OrderOfException> exceptionOrders = new List<OrderOfException>();
            List<ApplicationOrd> applications = new List<ApplicationOrd>();

            foreach (var order in connection.Select(
                DB_Table.ORDERS,
                new string[] { "number", "type", "date", "protocol_number", "protocol_date", "edu_form_id", "edu_source_id", "faculty_short_name", "direction_id", "profile_short_name" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,campaignID),
                    new System.Tuple<string, Relation, object>("protocol_number",Relation.NOT_EQUAL,null),
                    new System.Tuple<string, Relation, object>("type",Relation.NOT_EQUAL,"hostel")
                }).Select(s => new
                {
                    Number = s[0].ToString(),
                    Type = s[1].ToString(),
                    Date = (System.DateTime)s[2],
                    ProtocolNumber = (ushort)s[3],
                    ProtocolDate = (System.DateTime)s[4],
                    EduForm = (uint)s[5],
                    EduSource = (uint)s[6],
                    Faculty = s[7].ToString(),
                    Direction = (uint)s[8],
                    Profile = s[9] as string
                }))
                if (order.Type == "admission")
                {
                    admissionOrders.Add(new OrderOfAdmission(
                        new TUID(order.Number),
                        new TUID(campaignID),
                        "Зачисление " + order.Number,
                        order.Number,
                       new TDate(order.Date),
                       null,
                       order.EduForm,
                       order.EduSource
                        ));

                    foreach (object[] appl in connection.Select(
                        DB_Table.ORDERS_HAS_APPLICATIONS,
                        new string[] { "applications_id" },
                        new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("orders_number", Relation.EQUAL, order.Number) }
                        ))
                        applications.Add(new ApplicationOrd(
                            new TUID(appl[0].ToString()),
                            new TUID(order.Number),
                            1,
                            ComposeCompGroupUID(campaignID, order.Direction, order.EduForm, order.EduSource)
                            ));
                }
                else
                {
                    exceptionOrders.Add(new OrderOfException(
                        new TUID(order.Number),
                        new TUID(campaignID),
                        "Отчисление " + order.Number,
                        order.Number,
                       new TDate(order.Date),
                       null,
                       order.EduForm,
                       order.EduSource
                        ));

                    foreach (object[] appl in connection.Select(
                        DB_Table.ORDERS_HAS_APPLICATIONS,
                        new string[] { "applications_id" },
                        new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("orders_number", Relation.EQUAL, order.Number) }
                        ).Join(
                        connection.Select(
                            DB_Table.APPLICATIONS_ENTRANCES,
                            new string[] { "application_id", "is_disagreed_date" },
                            new List<System.Tuple<string, Relation, object>>
                            {
                                new System.Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,order.Faculty),
                                new System.Tuple<string, Relation, object>("direction_id",Relation.EQUAL,order.Direction),
                                new System.Tuple<string, Relation, object>("edu_form_id",Relation.EQUAL,order.EduForm),
                                new System.Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,order.EduSource),
                                new System.Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,order.Profile),
                            }),
                        k1 => k1[0],
                        k2 => k2[0],
                        (s1, s2) => s2
                        ))
                        applications.Add(new ApplicationOrd(
                            new TUID(appl[0].ToString()),
                            new TUID(order.Number),
                            2,
                            ComposeCompGroupUID(campaignID, order.Direction, order.EduForm, order.EduSource),
                            null,
                            null,
                            appl[1] is System.DateTime ? new TDateTime((System.DateTime)appl[1]) : null
                            ));
                }

            if (admissionOrders.Count != 0 || exceptionOrders.Count != 0)
                return new Orders(admissionOrders.Count != 0 ? admissionOrders : null, exceptionOrders.Count != 0 ? exceptionOrders : null, applications);

            return null;
        }

        private static TUID ComposeCompGroupUID(uint campID, uint dirID, /*uint level,*/ uint eduForm, uint eduSource)
        {
            return new TUID(campID.ToString() + dirID.ToString() + /*level.ToString() + */eduForm.ToString() + eduSource.ToString());
        }

        private static EduSourcePlaces GetPaidPlaces(DB_Connector connection, uint campaignID, uint directionID)
        {
            ushort o = 0, oz = 0, z = 0;
            foreach (object[] row in connection.Select(
                DB_Table.CAMPAIGNS_PROFILES_DATA,
                new string[] { "places_paid_o, places_paid_oz, places_paid_z" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("campaigns_id",Relation.EQUAL,campaignID),
                    new System.Tuple<string, Relation, object>("profiles_direction_id",Relation.EQUAL,directionID)
                }))
            {
                o += (ushort)row[0];
                oz += (ushort)row[1];
                z += (ushort)row[2];
            }

            return new EduSourcePlaces(o, oz, z);
        }

        private static EduSourcePlaces GetTargetPlaces(DB_Connector connection, uint campaignID, uint directionID)
        {
            ushort o = 0, oz = 0;
            foreach (object[] row in connection.Select(
                DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA,
                new string[] { "places_o, places_oz" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,campaignID),
                    new System.Tuple<string, Relation, object>("direction_id",Relation.EQUAL,directionID)
                }))
            {
                o += (ushort)row[0];
                oz += (ushort)row[1];
            }

            return new EduSourcePlaces(o, oz, 0);
        }

        private static void ChooseFormAndSourceAndPlaces(
            CompetitiveGroupItem.Variants variant,
            EduSourcePlaces budget,
            EduSourcePlaces paid,
            EduSourcePlaces quota,
            EduSourcePlaces target,
            out uint eduForm,
            out uint eduSource,
            out ushort places)
        {
            switch (variant)
            {
                case CompetitiveGroupItem.Variants.NumberBudgetO:
                    eduForm = 11;
                    eduSource = 14;
                    places = budget.O;
                    break;
                case CompetitiveGroupItem.Variants.NumberBudgetOZ:
                    eduForm = 12;
                    eduSource = 14;
                    places = budget.OZ;
                    break;
                case CompetitiveGroupItem.Variants.NumberBudgetZ:
                    eduForm = 10;
                    eduSource = 14;
                    places = budget.Z;
                    break;
                case CompetitiveGroupItem.Variants.NumberPaidO:
                    eduForm = 11;
                    eduSource = 15;
                    places = paid.O;
                    break;
                case CompetitiveGroupItem.Variants.NumberPaidOZ:
                    eduForm = 12;
                    eduSource = 15;
                    places = paid.OZ;
                    break;
                case CompetitiveGroupItem.Variants.NumberPaidZ:
                    eduForm = 10;
                    eduSource = 15;
                    places = paid.Z;
                    break;
                case CompetitiveGroupItem.Variants.NumberQuotaO:
                    eduForm = 11;
                    eduSource = 20;
                    places = quota.O;
                    break;
                case CompetitiveGroupItem.Variants.NumberQuotaOZ:
                    eduForm = 12;
                    eduSource = 20;
                    places = quota.OZ;
                    break;
                case CompetitiveGroupItem.Variants.NumberQuotaZ:
                    eduForm = 10;
                    eduSource = 20;
                    places = quota.Z;
                    break;
                case CompetitiveGroupItem.Variants.NumberTargetO:
                    eduForm = 11;
                    eduSource = 16;
                    places = target.O;
                    break;
                case CompetitiveGroupItem.Variants.NumberTargetOZ:
                    eduForm = 12;
                    eduSource = 16;
                    places = target.OZ;
                    break;
                case CompetitiveGroupItem.Variants.NumberTargetZ:
                    eduForm = 10;
                    eduSource = 16;
                    places = target.Z;
                    break;
                default:
                    throw new System.Exception("Unreachable reached.");
            }
        }

        private static ApplicationDocuments PackApplicationDocuments(DB_Connector connection, uint applicationID)
        {
            IdentityDocument identDoc = null;
            EduDocument eduDoc = null;
            OrphanDocument orphanDoc = null;
            SportDocument sportDoc = null;
            CustomDocument customDoc = null;
            foreach (var doc in connection.CallProcedure("get_application_docs", applicationID).Select(
                s => new
                {
                    ID = (uint)s[0],
                    Type = s[1].ToString(),
                    Series = s[2].ToString(),
                    Number = s[3].ToString(),
                    Date = s[4] as System.DateTime?,
                    Organization = s[5].ToString(),
                    OrigDate = s[6] as System.DateTime?
                }))
                switch (doc.Type)
                {
                    case "identity":
                        {
                            object[] idDoc = connection.Select(
                            DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA,
                            new string[] { "last_name", "first_name", "middle_name", "gender_id", "subdivision_code", "type_id", "nationality_id", "birth_date", "birth_place" },
                            new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID) }
                            )[0];

                            identDoc = new IdentityDocument(
                                new TUID(doc.ID),
                                idDoc[0].ToString(),
                                idDoc[1].ToString(),
                                doc.Number,
                                new TDate(doc.Date.Value),
                                (uint)idDoc[5],
                                (uint)idDoc[6],
                                new TDate((System.DateTime)idDoc[7]),
                                doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                idDoc[2].ToString(),
                                (uint)idDoc[3],
                                doc.Series,
                                idDoc[4].ToString(),
                                doc.Organization,
                                idDoc[8].ToString()
                                );
                        }
                        break;
                    case "school_certificate":
                    case "high_edu_diploma":
                    case "academic_diploma":
                        {
                            uint year = (uint)connection.Select(
                           DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                           new string[] { "year" },
                           new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID) }
                           )[0][0];

                            if (doc.Type == "academic_diploma")
                                eduDoc = new EduDocument(new TAcademicDiplomaDocument(
                                    new TUID(doc.ID),
                                    new TDocumentSeries(doc.Series),
                                    new TDocumentNumber(doc.Number),
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                    null,
                                    null,
                                    doc.Organization
                                    ));
                            else if (doc.Type == "school_certificate")
                                eduDoc = new EduDocument(new TSchoolCertificateDocument(
                                    new TUID(doc.ID),
                                    new TDocumentNumber(doc.Number),
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                    new TDocumentSeries(doc.Series),
                                    null,
                                    doc.Organization,
                                    year
                                    ));
                            else
                                eduDoc = new EduDocument(new THighEduDiplomaDocument(
                                    new TUID(doc.ID),
                                    new TDocumentSeries(doc.Series),
                                    new TDocumentNumber(doc.Number),
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                    null,
                                    doc.Organization,
                                    null,
                                    null,
                                    null,
                                    null,
                                    year
                                    ));
                        }
                        break;
                    case "orphan":
                        {
                            object[] data = connection.Select(
                                DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                                new string[] { "name", "dictionaries_item_id" },
                                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID) }
                                )[0];

                            orphanDoc = new OrphanDocument(new TOrphanDocument(
                                new TUID(doc.ID),
                                (uint)data[1],
                                new TDocumentName(data[0].ToString()),
                                new TDate(doc.Date.Value),
                                doc.Organization
                                ));
                        }
                        break;
                    case "sport":
                        {
                            object[] data = connection.Select(
                                DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                                new string[] { "name", "dictionaries_item_id" },
                                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID) }
                                )[0];

                            sportDoc = new SportDocument(new TSportDocument(
                                new TUID(doc.ID),
                                123,//TODO (uint)data[1],
                                data[0].ToString(),
                                new TDate(doc.Date.Value),
                                doc.Organization
                                ));
                        }
                        break;
                    case "custom":
                        {
                            object[] data = connection.Select(
                                DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                                new string[] { "name", "text_data" },
                                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID) }
                                )[0];

                            customDoc = new CustomDocument(new TCustomDocument(
                                new TUID(doc.ID),
                                data[0].ToString(),
                                null, //TODO
                                null, //TODO
                                null,
                                null,
                                null,
                                data[1].ToString()
                                ));
                        }
                        break;
                }

            return new ApplicationDocuments(
                identDoc,
                null,
                null,
                null,
                new List<EduDocument> { eduDoc },
                null,
                null,
                orphanDoc != null ? new List<OrphanDocument> { orphanDoc } : null,
                null,
                sportDoc != null ? new List<SportDocument> { sportDoc } : null,
                null,
                null,
                null,
                null,
                null,
                customDoc != null ? new List<CustomDocument> { customDoc } : null
                );
        }

        private static List<EntranceTestResult> PackApplicationTestResults(
            DB_Connector connection,
            uint campaignID,
            IEnumerable<System.Tuple<uint, TUID>> directionsAndCompGroupsIDs,
            IEnumerable<DB_Queries.Mark> applMarks)
        {
            List<EntranceTestResult> testsResults = new List<EntranceTestResult>();
            foreach (var entr in directionsAndCompGroupsIDs)
            {
                IEnumerable<uint> dir_subjects = DB_Queries.GetDirectionEntranceTests(connection, campaignID, entr.Item1).Distinct(); //TODO distinct пока не поменяли связть таблицы

                foreach (var res in dir_subjects.Join(
                    applMarks,
                    k1 => k1,
                    k2 => k2.SubjID,
                    (s1, s2) => s2
                    ).GroupBy(k => k.SubjID, (k, g) => g.First(s => s.Value == g.Max(m => m.Value))))
                    testsResults.Add(new EntranceTestResult(
                        new TUID(entr.Item2.Value + res.SubjID.ToString()),
                        res.Value,
                        (uint)(res.FromExam ? 2 : 1),//TODO
                        new TEntranceTestSubject(res.SubjID),
                        1,
                        entr.Item2,
                        //res.Item5 ? new ResultDocument(new TInstitutionDocument(new TDocumentNumber("1241415"))):null, TODO Для испытаний ОО
                        null//,
                            //TODO ? appl.SpecialCond
                        ));
            }

            return testsResults;
        }
    }
}
