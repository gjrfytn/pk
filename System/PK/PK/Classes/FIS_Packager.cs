using System.Collections.Generic;
using System.Linq;
using SharedClasses.DB;

using static SharedClasses.FIS.FIS_ExportClasses;

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
                OZ = oz;
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

        public static PackageData MakePackage(
            DB_Connector connection,
            uint campaignID,
            bool campaignData,
            System.Tuple<System.DateTime, System.DateTime> applicationsDateRange,
            bool orders,
            System.Tuple<System.DateTime, System.DateTime> ordersDateRange)
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
                applicationsDateRange != null ? PackApplications(connection, campaignID, applicationsDateRange) : null,
                orders ? PackOrders(connection, campaignID, ordersDateRange) : null
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
            DB_Helper dbHelper = new DB_Helper(connection);
            bool isMasterCampaign = dbHelper.GetCampaignType(campaignID)==DB_Helper.CampaignType.MASTER;

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

                uint levelID = SharedClasses.Utility.DirCodesEduLevels[dbHelper.GetDirectionNameAndCode(admData.DirID).Item2.Split('.')[1]];

                EduSourcePlaces budgetPlaces;
                EduSourcePlaces quotaPlaces;
                if (isMasterCampaign)
                {
                    budgetPlaces = new EduSourcePlaces(
                        (ushort)(admData.BudgetPlaces.O + admData.QuotaPlaces.O),
                        (ushort)(admData.BudgetPlaces.OZ + admData.QuotaPlaces.OZ),
                        (ushort)(admData.BudgetPlaces.Z + admData.QuotaPlaces.Z)
                        );
                    quotaPlaces = new EduSourcePlaces(0, 0, 0);
                }
                else
                {
                    budgetPlaces = admData.BudgetPlaces;
                    quotaPlaces = admData.QuotaPlaces;
                }

                admissionVolumes.Add(new AVItem(
                    new TUID(admData.CampID.ToString() + admData.DirID.ToString()),
                    new TUID(admData.CampID),
                    levelID,
                    admData.DirID,
                    budgetPlaces.O != 0 ? (ushort?)budgetPlaces.O : null,
                    budgetPlaces.OZ != 0 ? (ushort?)budgetPlaces.OZ : null,
                    budgetPlaces.Z != 0 ? (ushort?)budgetPlaces.Z : null,
                    paidPlaces.O != 0 ? (ushort?)paidPlaces.O : null,
                    paidPlaces.OZ != 0 ? (ushort?)paidPlaces.OZ : null,
                    paidPlaces.Z != 0 ? (ushort?)paidPlaces.Z : null,
                    targetPlaces.O != 0 ? (ushort?)targetPlaces.O : null,
                    targetPlaces.OZ != 0 ? (ushort?)targetPlaces.OZ : null,
                    targetPlaces.Z != 0 ? (ushort?)targetPlaces.Z : null,
                    quotaPlaces.O != 0 ? (ushort?)quotaPlaces.O : null,
                    quotaPlaces.OZ != 0 ? (ushort?)quotaPlaces.OZ : null,
                    quotaPlaces.Z != 0 ? (ushort?)quotaPlaces.Z : null
                    ));

                foreach (CompetitiveGroupItem.Variants v in System.Enum.GetValues(typeof(CompetitiveGroupItem.Variants)))
                {
                    uint eduForm, eduSource;
                    ushort places;

                    ChooseFormAndSourceAndPlaces(
                        v, budgetPlaces, paidPlaces, quotaPlaces, targetPlaces,
                        out eduForm, out eduSource, out places
                        );


                    if (places != 0)
                    {
                        CompetitiveGroupItem compGroupItem = null;
                        List<TargetOrganization> organizations = null;
                        if (eduSource == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceT))
                            organizations = GetTargetOrganizations(connection, campaignID, admData.DirID);
                        else
                            compGroupItem = new CompetitiveGroupItem(v, places);

                        TUID compGroupUID = ComposeCompGroupUID(admData.CampID, admData.DirID, eduForm, eduSource);

                        List<EntranceTestItem> entranceTests = new List<EntranceTestItem>();

                        if (isMasterCampaign)
                        {
                            uint subjectID = dbHelper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Технология");
                            entranceTests.Add(new EntranceTestItem(
                                new TUID(compGroupUID.Value + subjectID.ToString()),
                                1, //TODO
                                1,
                                new TEntranceTestSubject(subjectID),
                                null //TODO
                                ));
                        }
                        else
                            foreach (object[] etRow in connection.Select(
                                DB_Table.ENTRANCE_TESTS,
                                new string[] { "subject_id", "priority" },
                                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("direction_id", Relation.EQUAL, admData.DirID) }
                                ))
                                if (!entranceTests.Any(s => s.EntranceTestSubject.SubjectID.Value == (uint)etRow[0]))
                                    entranceTests.Add(new EntranceTestItem(
                                        new TUID(compGroupUID.Value + etRow[0].ToString()),
                                        1,
                                        (ushort)etRow[1],
                                        new TEntranceTestSubject((uint)etRow[0]),
                                        dbHelper.GetMinMark((uint)etRow[0])
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
                            null,
                            compGroupItem,
                            organizations,
                            null,
                            entranceTests
                            ));
                    }
                }
            }

            if (admissionVolumes.Count != 0)
                return new AdmissionInfo(admissionVolumes, null, competitiveGroups.Count != 0 ? competitiveGroups : null);

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

        private static List<Application> PackApplications(DB_Connector connection, uint campaignID, System.Tuple<System.DateTime, System.DateTime> dateRange)
        {
            List<Application> applications = new List<Application>();

            var applicationsBD = connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "id", "entrant_id", "registration_time", "needs_hostel", "status", "comment", "special_conditions", "priority_right", "withdraw_date"},
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, campaignID),
                    new System.Tuple<string, Relation, object>("registration_time",Relation.GREATER_EQUAL,dateRange.Item1.Date),
                    new System.Tuple<string, Relation, object>("registration_time",Relation.LESS,dateRange.Item2.Date.AddDays(1))
                }).Join(
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
                    PriorityRight = s1[7] as bool?,
                    Info = s2[1] as string,
                    Email = s2[2].ToString(),
					ReturnDate = s1[8] as System.DateTime?

				});

            if (applicationsBD.Count() == 0)
                return null;

            DB_Helper dbHelper = new DB_Helper(connection);

            var paidAdmDates = connection.Select(DB_Table.ORDERS_HAS_APPLICATIONS, "orders_number", "applications_id").Join(
                connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "number", "edu_form_id", "direction_id", "date" },
                    new List<System.Tuple<string, Relation, object>>
                    {
                        new System.Tuple<string, Relation, object>("campaign_id",Relation.EQUAL, Settings.CurrentCampaignID),
                        new System.Tuple<string, Relation, object>("type",Relation.EQUAL, "admission"),
                        new System.Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL, dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP))
                    }),
                k1 => k1[0],
                k2 => k2[0],
                (s1, s2) => new { ApplID = (uint)s1[1], EduForm = (uint)s2[1], Direction = (uint)s2[2], Date = (System.DateTime)s2[3] }
                );

            IEnumerable<DB_Queries.Mark> marks = DB_Queries.GetMarks(connection, applicationsBD.Select(s => s.ID), campaignID);

            bool isMasterCampaign = dbHelper.GetCampaignType(campaignID)==DB_Helper.CampaignType.MASTER;
            foreach (var appl in applicationsBD)
            {
                var finSourceEduForms = connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "direction_id", "edu_form_id", "edu_source_id", "target_organization_id", "is_agreed_date", "is_disagreed_date" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("application_id", Relation.EQUAL, appl.ID) }
                    ).GroupBy(
                    k => System.Tuple.Create((uint)k[0], (uint)k[1], (uint)k[2]),
                    e => new
                    {
                        TargetOrg = e[3] as uint?,
                        AgreedDate = (uint)e[2] == dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP) ?
                        paidAdmDates.LastOrDefault(s => s.ApplID == appl.ID && s.EduForm == (uint)e[1] && s.Direction == (uint)e[0])?.Date : //TODO !!!
                        e[4] as System.DateTime?,
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
                                    k.Item2,
                                    k.Item3
                                    ),
                                buf.TargetOrg != null ? new TUID(buf.TargetOrg) : null,
                                buf.AgreedDate != null ? new TDateTime(buf.AgreedDate) : null,
                                buf.DisagreedDate != null ? new TDateTime(buf.DisagreedDate) : null
                                )
                        };
                    });
				uint returnType = 0;
				if (appl.Status == _StatusesMap["withdrawn"])
				{
					returnType = dbHelper.GetDictionaryItemID(FIS_Dictionary.RETURN_TYPE, "Передача лично или через доверенное лицо");
				}

				IEnumerable<DB_Queries.Document> docs = DB_Queries.GetApplicationDocuments(connection, appl.ID);

                ApplicationDocuments packedDocs = PackApplicationDocuments(
                    connection,
                    docs,
                    isMasterCampaign && finSourceEduForms.Any(s => s.FSEF.IsAgreedDate != null) ? (System.DateTime?)appl.RegTime : null,					
					finSourceEduForms.Any(s => s.FSEF.IsAgreedDate != null && s.FSEF.IsDisagreedDate == null) ?
                    (System.DateTime?)System.DateTime.Parse(finSourceEduForms.First(s => s.FSEF.IsAgreedDate != null && s.FSEF.IsDisagreedDate == null).FSEF.IsAgreedDate.Value) :
                    null
                    );

                List<ApplicationCommonBenefit> benefits = PackApplicationBenefits(
                    connection,
                    appl.ID,
                    finSourceEduForms.Select(s => System.Tuple.Create(s.FSEF.CompetitiveGroupUID, s.EduSource == 20)),//TODO
                    docs
                    );

                List<EntranceTestResult> testsResults = PackApplicationTestResults(
                    connection,
                    campaignID,
                    finSourceEduForms.Select(s => System.Tuple.Create(s.DirID, s.FSEF.CompetitiveGroupUID)),
                    marks.Where(s => s.ApplID == appl.ID)
                    );

                List<CustomDocument> customDocs = packedDocs.CustomDocuments == null ? customDocs = new List<CustomDocument>() : customDocs = new List<CustomDocument>(packedDocs.CustomDocuments);

                List<IndividualAchievement> achievements = PackApplicationAchievements(connection, campaignID, appl.ID, appl.PriorityRight, docs, customDocs);

                if (customDocs.Count == 0)
                    customDocs = null;

                List<IdentityDocument> otherIdentityDocs = connection.Select(
                    DB_Table.APPLICATION_EGE_RESULTS,
                    new string[] { "series", "number" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("application_id", Relation.EQUAL, appl.ID) })
                    .GroupBy(
                    k => System.Tuple.Create(k[0] as string, k[1] as string),
                    (k, g) => new IdentityDocument(
                        new TUID(appl.ID.ToString() + "_" + k.Item1 + k.Item2),
                        packedDocs.IdentityDocument.LastName,
                        packedDocs.IdentityDocument.FirstName,
                        k.Item2,
                        packedDocs.IdentityDocument.DocumentDate,
                        9, //TODO
                        packedDocs.IdentityDocument.NationalityTypeID,
                        packedDocs.IdentityDocument.BirthDate,
                        null,
                        packedDocs.IdentityDocument.MiddleName,
                        packedDocs.IdentityDocument.GenderID,
                        !string.IsNullOrWhiteSpace(k.Item1) ? k.Item1 : null
                        )).ToList();

                if (otherIdentityDocs.Count == 0)
                    otherIdentityDocs = null;

                packedDocs = new ApplicationDocuments(
                    packedDocs.IdentityDocument,
                    packedDocs.EgeDocuments,
                    packedDocs.GiaDocuments,
                    otherIdentityDocs,
                    packedDocs.EduDocuments,
                    packedDocs.MilitaryCardDocument,
                    packedDocs.StudentDocument,
                    packedDocs.OrphanDocuments,
                    packedDocs.VeteranDocuments,
                    packedDocs.SportDocuments,
                    packedDocs.CompatriotDocuments,
                    packedDocs.PauperDocuments,
                    packedDocs.ParentsLostDocuments,
                    packedDocs.StateEmployeeDocuments,
                    packedDocs.RadiationWorkDocuments,
                    customDocs
                    );
				
                applications.Add(new Application(
                    ComposeApplicationUID(campaignID, appl.ID),
                    campaignID.ToString() + "_" + appl.ID.ToString(),
                    new Entrant(
                        new TUID(appl.Entr_ID),
                        packedDocs.IdentityDocument.LastName,
                        packedDocs.IdentityDocument.FirstName,
                        packedDocs.IdentityDocument.GenderID.Value,
                        new EmailOrMailAddress(appl.Email.ToString()),
                        packedDocs.IdentityDocument.MiddleName,
                        appl.Info
                        ),
                    new TDateTime(appl.RegTime),
                    appl.Hostel,
                    appl.Status,
                    finSourceEduForms.Select(s => s.FSEF).ToList(),
                    packedDocs,
                    appl.Comment,
                    returnType,
                    appl.ReturnDate,
                    benefits.Count != 0 ? benefits : null,
                    testsResults.Count != 0 ? testsResults : null,
                    achievements.Count != 0 ? achievements : null
                    ));
		}

			return applications;
        }

        private static Orders PackOrders(DB_Connector connection, uint campaignID, System.Tuple<System.DateTime, System.DateTime> dateRange)
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
                    new System.Tuple<string, Relation, object>("type",Relation.NOT_EQUAL,"hostel"),
                    new System.Tuple<string, Relation, object>("date",Relation.GREATER_EQUAL,dateRange.Item1.Date),
                    new System.Tuple<string, Relation, object>("date",Relation.LESS,dateRange.Item2.Date.AddDays(1))
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
                        ComposeOrderUID(campaignID, order.Number),
                        new TUID(campaignID),
                        "Зачисление " + order.Number,
                        order.Number + " " + campaignID.ToString(),
                        new TDate(order.Date),
                        null,
                        order.EduForm,
                        order.EduSource,
                        null,
                        (uint)(System.DateTime.Today < new System.DateTime(System.DateTime.Today.Year, 8, 4) ? 1 : 2)
                        ));

                    foreach (object[] appl in connection.Select(
                        DB_Table.ORDERS_HAS_APPLICATIONS,
                        new string[] { "applications_id" },
                        new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("orders_number", Relation.EQUAL, order.Number) }
                        ))
                        applications.Add(new ApplicationOrd(
                            ComposeApplicationUID(campaignID, (uint)appl[0]),
                            ComposeOrderUID(campaignID, order.Number),
                            1,
                            ComposeCompGroupUID(campaignID, order.Direction, order.EduForm, order.EduSource),
                            order.EduSource != 15 ? (uint?)1 : null //TODO
                            ));
                }
                else
                {
                    exceptionOrders.Add(new OrderOfException(
                        ComposeOrderUID(campaignID, order.Number),
                        new TUID(campaignID),
                        "Отчисление " + order.Number,
                        order.Number + " " + campaignID.ToString(),
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
                            ComposeApplicationUID(campaignID, (uint)appl[0]),
                            ComposeOrderUID(campaignID, order.Number),
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

        private static TUID ComposeApplicationUID(uint campaignID, uint applID)
        {
            return new TUID(campaignID.ToString() + "_" + applID.ToString());
        }

        private static TUID ComposeOrderUID(uint campaignID, string orderNumber)
        {
            return new TUID(orderNumber + " " + campaignID.ToString());
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

        private static List<TargetOrganization> GetTargetOrganizations(DB_Connector connection, uint campaignID, uint directionID)
        {
            List<TargetOrganization> organizations = new List<TargetOrganization>();
            foreach (var organization in connection.Select(
                DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA,
                new string[] { "target_organization_id", "places_o", "places_oz" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,campaignID),
                    new System.Tuple<string, Relation, object>("direction_id",Relation.EQUAL,directionID)
                }).GroupBy(
                k => k[0],
                (k, g) => new
                {
                    ID = g.First()[0].ToString(),
                    O = (ushort)g.Sum(s => (ushort)s[1]),
                    OZ = (ushort)g.Sum(s => (ushort)s[2])
                }))
            {
                if (organization.O != 0)
                    organizations.Add(new TargetOrganization(
                        new TUID(organization.ID),
                        new CompetitiveGroupTargetItem(CompetitiveGroupTargetItem.Variants.NumberTargetO, organization.O)
                        ));

                if (organization.OZ != 0)
                    organizations.Add(new TargetOrganization(
                        new TUID(organization.ID),
                        new CompetitiveGroupTargetItem(CompetitiveGroupTargetItem.Variants.NumberTargetOZ, organization.OZ)
                        ));
            }

            return organizations;
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

        private static ApplicationDocuments PackApplicationDocuments(DB_Connector connection, IEnumerable<DB_Queries.Document> docs, System.DateTime? isMasterApplDate, System.DateTime? paidOrdDateDate)
        {
            IdentityDocument identDoc = null;
            EduDocument eduDoc = null;
            OrphanDocument orphanDoc = null;
            SportDocument sportDoc = null;
            List<CustomDocument> customDocs = new List<CustomDocument>();
            foreach (var doc in docs)
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
                                !string.IsNullOrWhiteSpace(doc.Series) ? doc.Series : null,
                                idDoc[4] as string,
                                doc.Organization,
                                idDoc[8].ToString()
                                );
                        }
                        break;
                    case "school_certificate":
                    case "high_edu_diploma":
                    case "academic_diploma":
                    case "middle_edu_diploma":
                        {
                            uint? year = connection.Select(
                                DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                                new string[] { "year" },
                                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID) }
                                )[0][0] as uint?;

                            if (doc.Type == "academic_diploma")
                                eduDoc = new EduDocument(new TAcademicDiplomaDocument(
                                    new TUID(doc.ID),
                                    new TDocumentNumber(doc.Number),
                                    new TDocumentSeries(doc.Series),
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : (paidOrdDateDate.HasValue ? new TDate(paidOrdDateDate.Value) : null),
                                    null,
                                    doc.Date.HasValue ? new TDate(doc.Date.Value) : null,
                                    doc.Organization
                                    ));
                            else if (doc.Type == "school_certificate")
                                eduDoc = new EduDocument(new TSchoolCertificateDocument(
                                    new TUID(doc.ID),
                                    new TDocumentNumber(doc.Number),
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : (paidOrdDateDate.HasValue ? new TDate(paidOrdDateDate.Value) : null),
                                    !string.IsNullOrWhiteSpace(doc.Series) ? new TDocumentSeries(doc.Series) : null,
                                    doc.Date.HasValue ? new TDate(doc.Date.Value) : null,
                                    doc.Organization,
                                    year
                                    ));
                            else if (doc.Type == "middle_edu_diploma")
                                eduDoc = new EduDocument(new TMiddleEduDiplomaDocument(
                                    new TUID(doc.ID),
                                    new TDocumentNumber(doc.Number),
                                    new TDocumentSeries(doc.Series),
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : (paidOrdDateDate.HasValue ? new TDate(paidOrdDateDate.Value) : null),
                                    doc.Date.HasValue ? new TDate(doc.Date.Value) : null,
                                    doc.Organization,
                                    null,
                                    null,
                                    null,
                                    null,
                                    year
                                    ));
                            else
                                eduDoc = new EduDocument(new THighEduDiplomaDocument(
                                    new TUID(doc.ID),
                                    new TDocumentNumber(doc.Number),
                                    new TDocumentSeries(doc.Series),
                                    isMasterApplDate.HasValue ? new TDate(isMasterApplDate.Value) : (doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : (paidOrdDateDate.HasValue ? new TDate(paidOrdDateDate.Value) : null)),
                                    doc.Date.HasValue ? new TDate(doc.Date.Value) : null,
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

                            uint? category = data[1] as uint?; //TODO Записывать изначально как custom

                            if (category.HasValue)
                                sportDoc = new SportDocument(new TSportDocument(
                                    new TUID(doc.ID),
                                    category.Value,
                                    data[0].ToString(),
                                    new TDate(doc.Date.Value),
                                    doc.Organization
                                    ));
                            else
                                customDocs.Add(new CustomDocument(new TCustomDocument(
                                    new TUID(doc.ID),
                                    data[0].ToString(),
                                    new TDate(doc.Date.Value),
                                    doc.Organization
                                    )));
                        }
                        break;
                    case "custom":
                        {
                            object[] data = connection.Select(
                                DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                                new string[] { "name", "text_data" },
                                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID) }
                                )[0];

                            customDocs.Add(new CustomDocument(new TCustomDocument(
                                new TUID(doc.ID),
                                data[0].ToString(),
                                new TDate(doc.Date.Value),
                                doc.Organization,
                                null,
                                null,
                                null,
                                data[1].ToString()
                                )));
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
                customDocs.Count != 0 ? customDocs : null
                );
        }

        private static List<ApplicationCommonBenefit> PackApplicationBenefits(DB_Connector connection, uint applID, IEnumerable<System.Tuple<TUID, bool>> compGroupsWithQuotaFlag, IEnumerable<DB_Queries.Document> docs)
        {
            List<ApplicationCommonBenefit> benefits = new List<ApplicationCommonBenefit>();

            List<object[]> benefitsBD = connection.Select(
                DB_Table.APPLICATION_COMMON_BENEFITS,
                new string[] { "id", "document_type_id", "reason_document_id", "allow_education_document_id", "benefit_kind_id" },
                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("application_id", Relation.EQUAL, applID) }
                );

            if (benefitsBD.Any())
            {
                foreach (object[] row in benefitsBD)
                {
                    DocumentReason reasonDoc = null;
                    DB_Queries.Document doc = docs.Single(s => s.ID == (uint)row[2]);
                    var whereExpr = new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID) };
                    object[] data;
                    DB_Queries.Document allowDoc;

                    switch ((uint)row[1])//TODO
                    {
                        case 9:
                            data = connection.Select(
                                DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA,
                                new string[] { "diploma_type_id", "olympic_id", "profile_id", "class_number", "olympic_subject_id" },
                                whereExpr
                                )[0];

                            reasonDoc = new DocumentReason(new TOlympicDocument(
                                new TUID(doc.ID),
                                (uint)data[0],
                                (uint)data[1],
                                (uint)data[2],
                                (uint)data[3],
                                doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                doc.Series != null ? new TDocumentSeries(doc.Series) : null,
                                doc.Number != null ? new TDocumentNumber(doc.Number) : null,
                                doc.Date.HasValue ? new TDate(doc.Date.Value) : null,
                                data[4] as uint?
                                ));
                            break;
                        case 10:
                            data = connection.Select(
                                DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA,
                                new string[] { "diploma_type_id", "class_number", "olympic_id" },
                                whereExpr
                                )[0];

                            reasonDoc = new DocumentReason(new TOlympicTotalDocument(
                                new TUID(doc.ID),
                                new TDocumentNumber(doc.Number),
                                (uint)data[0],
                                (uint)data[1],
                                (uint)data[2],
                                new List<SubjectID> { null },//TODO
                                doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                doc.Series != null ? new TDocumentSeries(doc.Series) : null
                                ));
                            break;
                        case 11:
                            data = connection.Select(
                                DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                                new string[] { "dictionaries_item_id" },
                                whereExpr
                                )[0];
                            allowDoc = docs.Single(s => s.ID == (uint)row[3]);

                            reasonDoc = new DocumentReason(new MedicalDocuments(
                                new BenefitDocument(new TDisabilityDocument(
                                    new TUID(doc.ID),
                                    new TDocumentSeries(doc.Series),
                                    new TDocumentNumber(doc.Number),
                                    (uint)data[0],
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                    doc.Date.HasValue ? new TDate(doc.Date.Value) : null,
                                    doc.Organization
                                    )),
                                new TAllowEducationDocument(
                                    new TUID(allowDoc.ID),
                                    new TDocumentNumber(doc.Number),
                                    doc.Date.HasValue ? new TDate(doc.Date.Value) : new TDate(System.DateTime.Now), //TODO Now
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                    doc.Organization
                                )));
                            break;
                        case 12:
                            allowDoc = docs.Single(s => s.ID == (uint)row[3]);

                            reasonDoc = new DocumentReason(new MedicalDocuments(
                                new BenefitDocument(new TMedicalDocument(
                                    new TUID(doc.ID),
                                    new TDocumentNumber(doc.Number),
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                    doc.Date.HasValue ? new TDate(doc.Date.Value) : null,
                                    doc.Organization
                                    )),
                                new TAllowEducationDocument(
                                    new TUID(allowDoc.ID),
                                    new TDocumentNumber(doc.Number),
                                    new TDate(doc.Date.Value),
                                    doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                    doc.Organization
                                    )));
                            break;
                        case 27:
                            data = connection.Select(
                                DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA,
                                new string[] { "diploma_type_id", "olympic_name", "profile_id" },
                                whereExpr
                                )[0];

                            reasonDoc = new DocumentReason(new TUkraineOlympic(
                                new TUID(doc.ID),
                                new TDocumentNumber(doc.Number),
                                (uint)data[0],
                                data[1].ToString(),
                                new DB_Helper(connection).GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)data[2]),
                                doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                doc.Series != null ? new TDocumentSeries(doc.Series) : null,
                                doc.Date.HasValue ? new TDate(doc.Date.Value) : null
                                ));
                            break;
                        case 28:
                            data = connection.Select(
                                DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA,
                                new string[] { "country_id", "olympic_name", "profile_id" },
                                whereExpr
                                )[0];

                            reasonDoc = new DocumentReason(new TInternationalOlympic(
                                new TUID(doc.ID),
                                new TDocumentNumber(doc.Number),
                                (uint)data[0],
                                data[1].ToString(),
                                new DB_Helper(connection).GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)data[2]),
                                doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                doc.Series != null ? new TDocumentSeries(doc.Series) : null,
                                doc.Date.HasValue ? new TDate(doc.Date.Value) : null
                                ));
                            break;
                        case 30:
                            data = connection.Select(
                                DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                                new string[] { "dictionaries_item_id", "name" },
                                whereExpr
                                )[0];

                            reasonDoc = new DocumentReason(new TOrphanDocument(
                                new TUID(doc.ID),
                                (uint)data[0],
                                new TDocumentName(data[1].ToString()),
                                new TDate(doc.Date.Value),
                                doc.Organization,
                                doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                                doc.Series != null ? new TDocumentSeries(doc.Series) : null,
                                doc.Number != null ? new TDocumentNumber(doc.Number) : null
                                ));
                            break;
                    }

                    if ((uint)row[4] == 4)
                        foreach (System.Tuple<TUID, bool> entrance in (uint)row[4] == 4 ? compGroupsWithQuotaFlag.Where(s => s.Item2) : compGroupsWithQuotaFlag) //TODO
                            benefits.Add(new ApplicationCommonBenefit(
                                new TUID(row[0].ToString() + "_" + entrance.Item1.Value),
                                entrance.Item1,
                                (uint)row[1],
                                reasonDoc,
                                (uint)row[4]
                                ));
                }
            }

            return benefits;
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
                IEnumerable<uint> dir_subjects = DB_Queries.GetDirectionEntranceTests(connection, campaignID, entr.Item1).Distinct(); //TODO distinct пока не поменяли связь таблицы

                foreach (var res in dir_subjects.Join(
                    applMarks,
                    k1 => k1,
                    k2 => k2.SubjID,
                    (s1, s2) => s2
                    ).GroupBy(k => k.SubjID, (k, g) => g.First(s => s.Value == g.Max(m => m.Value))))
                    testsResults.Add(new EntranceTestResult(
                        new TUID(entr.Item2.Value + res.SubjID.ToString()),
                        res.Value,
                        (uint)(res.FromExamDate.HasValue ? 2 : 1),//TODO
                        new TEntranceTestSubject(res.SubjID),
                        1,
                        entr.Item2,
                        res.FromExamDate.HasValue ? new ResultDocument(new TInstitutionDocument(
                            new TDocumentNumber("Экзаменационная ведомость от " + res.FromExamDate.Value.ToShortDateString()),
                            new TDate(res.FromExamDate.Value),
                            1
                            )) : null
                        ));
            }

            return testsResults;
        }

        private static List<IndividualAchievement> PackApplicationAchievements(DB_Connector connection, uint campaignID, uint applID, bool? priorityRight, IEnumerable<DB_Queries.Document> docs, ICollection<CustomDocument> customDocs)
        {
            List<IndividualAchievement> achievements = new List<IndividualAchievement>();

            var achievementsBD = connection.Select(
                                DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                                new string[] { "id", "institution_achievement_id", "document_id" },
                                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("application_id", Relation.EQUAL, applID) }
                                ).Select(s => new { ID = (uint)s[0], InstAchID = (uint)s[1], DocID = (uint)s[2] });

            if (achievementsBD.Any())
            {
                Dictionary<uint, ushort> instAch = connection.Select(
                    DB_Table.INSTITUTION_ACHIEVEMENTS,
                    new string[] { "id", "value" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, campaignID) }
                    ).ToDictionary(k => (uint)k[0], e => (ushort)e[1]);

                foreach (var ach in achievementsBD)
                {
                    DB_Queries.Document doc = docs.Single(s => s.ID == ach.DocID);
                    if (doc.Type != "high_edu_diploma")
                    {
                        TUID docUID = new TUID(doc.ID.ToString() + "C");
                        customDocs.Add(new CustomDocument(new TCustomDocument(
                            docUID,
                            doc.Type,
                            new TDate(System.DateTime.Now),//TODO Год выдачи
                            doc.Organization,
                            doc.OrigDate.HasValue ? new TDate(doc.OrigDate.Value) : null,
                            !string.IsNullOrEmpty(doc.Series) ? new TDocumentSeries(doc.Series) : null, //TODO
                            doc.Number != null ? new TDocumentNumber(doc.Number) : null
                            )));
                        achievements.Add(new IndividualAchievement(
                            new TUID(campaignID.ToString() + "_" + ach.ID.ToString()),
                            new TUID(ach.InstAchID),
                            docUID,
                            instAch[ach.InstAchID],
                            priorityRight.HasValue ? priorityRight : null
                            ));
                    }
                }
            }

            return achievements;
        }
    }
}
