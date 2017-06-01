using System.Collections.Generic;
using System.Linq;
using PK.Classes.FIS_ExportClasses;

namespace PK.Classes
{
    static class FIS_Packager
    {
        private static readonly Dictionary<uint, char> EduFormLiterals = new Dictionary<uint, char>() { { 11, 'Д' }, { 12, 'В' }, { 10, 'З' } };
        private static readonly Dictionary<uint, string> EduSourceLiterals = new Dictionary<uint, string>() { { 14, "ОО" }, { 15, "СН" }, { 16, "ЦН" }, { 20, "КВ" } };

        public static PackageData MakePackage(DB_Connector connection, uint campaignID, bool campaignData, bool applications, bool orders)
        {
            return new PackageData(
                campaignData ? PackCampaignInfo(connection) : null,
                campaignData ? PackAdmissionInfo(connection) : null,
                campaignData ? PackInstitutionAchievements(connection) : null,
                campaignData ? PackTargetOrganizations(connection) : null,
                applications ? PackApplications(connection) : null,
                orders ? PackOrders(connection) : null
                );
        }

        private static CampaignInfo PackCampaignInfo(DB_Connector connection)
        {
            List<Campaign> campaigns = new List<Campaign>();
            foreach (object[] row in connection.Select(DB_Table.CAMPAIGNS))
            {
                List<EducationFormID> eduForms = new List<EducationFormID>();
                List<EducationLevelID> eduLevels = new List<EducationLevelID>();
                foreach (object[] diRow in connection.Select(
                    DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS,
                    new string[] { "dictionaries_items_dictionary_id", "dictionaries_items_item_id" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, row[0]) }
                    ))
                    if ((uint)diRow[0] == (uint)FIS_Dictionary.EDU_FORM)
                        eduForms.Add(new EducationFormID((uint)diRow[1]));
                    else
                        eduLevels.Add(new EducationLevelID((uint)diRow[1]));

                campaigns.Add(new Campaign(
                    new TUID(row[0].ToString()),
                    row[1].ToString(),
                    (uint)row[2],
                    (uint)row[3],
                    eduForms,
                    (uint)row[5],
                    eduLevels,
                    (uint)row[7]
                    ));
            }

            return new CampaignInfo(campaigns);
        }

        private static AdmissionInfo PackAdmissionInfo(DB_Connector connection)
        {
            List<AVItem> admissionVolumes = new List<AVItem>();
            List<CompetitiveGroup> competitiveGroups = new List<CompetitiveGroup>();
            foreach (var admData in connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, "campaign_id", "direction_id", "places_budget_o", "places_budget_oz", "places_quota_o", "places_quota_oz")
                .GroupBy(
                k => new System.Tuple<uint, uint>((uint)k[0],
                (uint)k[1]), (k, g) => new
                {
                    CampID = k.Item1,
                    DirID = k.Item2,
                    BO = g.Sum(s => (ushort)s[2]),
                    BOZ = g.Sum(s => (ushort)s[3]),
                    BZ = 0,
                    QO = g.Sum(s => (ushort)s[4]),
                    QOZ = g.Sum(s => (ushort)s[5]),
                    QZ = 0
                }))
            {
                ushort paid_o = 0, paid_oz = 0, paid_z = 0;
                foreach (object[] profRow in connection.Select(
                    DB_Table.CAMPAIGNS_PROFILES_DATA,
                    new string[] { "places_paid_o, places_paid_oz, places_paid_z" },
                    new List<System.Tuple<string, Relation, object>>
                    {
                        new System.Tuple<string, Relation, object>("campaigns_id",Relation.EQUAL,admData.CampID),
                        new System.Tuple<string, Relation, object>("profiles_direction_id",Relation.EQUAL,admData.DirID)
                    }))
                {
                    paid_o += (ushort)profRow[0];
                    paid_oz += (ushort)profRow[1];
                    paid_z += (ushort)profRow[2];
                }

                ushort target_o = 0, target_oz = 0, target_z = 0;
                foreach (object[] targetRow in connection.Select(
                    DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA,
                    new string[] { "places_o, places_oz" },
                    new List<System.Tuple<string, Relation, object>>
                    {
                        new System.Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,admData.CampID),
                        new System.Tuple<string, Relation, object>("direction_id",Relation.EQUAL,admData.DirID)
                    }))
                {
                    target_o += (ushort)targetRow[0];
                    target_oz += (ushort)targetRow[1];
                }

                DB_Helper dbHelper = new DB_Helper(connection);
                uint levelID = Utility.DirCodesEduLevels[dbHelper.GetDirectionNameAndCode(admData.DirID).Item2.Split('.')[1]];

                admissionVolumes.Add(new AVItem(
                    new TUID(admData.CampID.ToString() + admData.DirID.ToString()),
                    new TUID(admData.CampID.ToString()),
                    levelID,
                    admData.DirID,
                    (ushort)admData.BO, (ushort)admData.BOZ, (ushort)admData.BZ,
                    paid_o, paid_oz, paid_z,
                    target_o, target_oz, target_z,
                    (ushort)admData.QO, (ushort)admData.QOZ, (ushort)admData.QZ
                    ));

                foreach (object v in System.Enum.GetValues(typeof(CompetitiveGroupItem.Variants)))
                {
                    uint eduForm, eduSource;
                    ushort places;
                    switch ((CompetitiveGroupItem.Variants)v)
                    {
                        case CompetitiveGroupItem.Variants.NumberBudgetO:
                            eduForm = 11;
                            eduSource = 14;
                            places = (ushort)admData.BO;
                            break;
                        case CompetitiveGroupItem.Variants.NumberBudgetOZ:
                            eduForm = 12;
                            eduSource = 14;
                            places = (ushort)admData.BOZ;
                            break;
                        case CompetitiveGroupItem.Variants.NumberBudgetZ:
                            eduForm = 10;
                            eduSource = 14;
                            places = (ushort)admData.BZ;
                            break;
                        case CompetitiveGroupItem.Variants.NumberPaidO:
                            eduForm = 11;
                            eduSource = 15;
                            places = paid_o;
                            break;
                        case CompetitiveGroupItem.Variants.NumberPaidOZ:
                            eduForm = 12;
                            eduSource = 15;
                            places = paid_oz;
                            break;
                        case CompetitiveGroupItem.Variants.NumberPaidZ:
                            eduForm = 10;
                            eduSource = 15;
                            places = paid_z;
                            break;
                        case CompetitiveGroupItem.Variants.NumberQuotaO:
                            eduForm = 11;
                            eduSource = 20;
                            places = (ushort)admData.QO;
                            break;
                        case CompetitiveGroupItem.Variants.NumberQuotaOZ:
                            eduForm = 12;
                            eduSource = 20;
                            places = (ushort)admData.QOZ;
                            break;
                        case CompetitiveGroupItem.Variants.NumberQuotaZ:
                            eduForm = 10;
                            eduSource = 20;
                            places = (ushort)admData.QZ;
                            break;
                        case CompetitiveGroupItem.Variants.NumberTargetO:
                            eduForm = 11;
                            eduSource = 16;
                            places = target_o;
                            break;
                        case CompetitiveGroupItem.Variants.NumberTargetOZ:
                            eduForm = 12;
                            eduSource = 16;
                            places = target_oz;
                            break;
                        case CompetitiveGroupItem.Variants.NumberTargetZ:
                            eduForm = 10;
                            eduSource = 16;
                            places = target_z;
                            break;
                        default:
                            throw new System.Exception("Unreachable reached.");
                    }

                    if (places != 0)
                    {
                        string compGroupUID = admData.CampID.ToString() + admData.DirID.ToString() + levelID.ToString() + eduForm.ToString() + eduSource.ToString();

                        List<EntranceTestItem> entranceTests = new List<EntranceTestItem>();
                        foreach (object[] etRow in connection.Select(
                            DB_Table.ENTRANCE_TESTS,
                            new string[] { "subject_id", "priority" },
                            new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("direction_id", Relation.EQUAL, admData.DirID) }
                            ))
                            if (!entranceTests.Any(s => s.EntranceTestSubject.SubjectID.Value == (uint)etRow[0]))
                                entranceTests.Add(new EntranceTestItem(
                                    new TUID(compGroupUID + etRow[0].ToString()),
                                    1,//TODO ?
                                    (ushort)etRow[1],
                                    new TEntranceTestSubject((uint)etRow[0]),
                                    null //TODO Добавить!
                                    ));

                        competitiveGroups.Add(new CompetitiveGroup(
                            new TUID(compGroupUID),
                            new TUID(admData.CampID.ToString()),
                            dbHelper.GetDirectionNameAndCode(admData.DirID).Item2 + " " + EduSourceLiterals[eduSource] + " " + EduFormLiterals[eduForm],
                            levelID,
                            eduSource,
                            eduForm,
                            admData.DirID,
                            null,
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

            return new AdmissionInfo(admissionVolumes, null, competitiveGroups);//TODO null
        }

        private static List<InstitutionAchievement> PackInstitutionAchievements(DB_Connector connection)
        {
            List<InstitutionAchievement> achievements = new List<InstitutionAchievement>();

            foreach (object[] row in connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, "id", "campaign_id", "name", "category_id", "value"))
                achievements.Add(new InstitutionAchievement(
                    new TUID(row[0].ToString()),
                    row[2].ToString(),
                    (uint)row[3],
                    (ushort)row[4],
                    new TUID(row[1].ToString())
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

        private static List<Application> PackApplications(DB_Connector connection)
        {
            /*List<Application> applications = new List<Application>();

            foreach (object[] entRow in connection.Select(DB_Table.ENTRANTS, "id", "last_name", "first_name", "middle_name", "gender_id", "custom_information", "email"))
                foreach (object[] row in connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "id", "number", "registration_time", "needs_hostel", "status_id", "status_comment" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("entrant_id", Relation.EQUAL, entRow[0]) }
                    ))
                {
                    System.DateTime regDT = (System.DateTime)row[2];

                    List<FinSourceEduForm> finSourceEduForms = new List<FinSourceEduForm>();
                    foreach (object[] entranceRow in connection.Select(DB_Table.APPLICATIONS_ENTRANCES,))
                        finSourceEduForms.Add(new FinSourceEduForm(new TUID(
                            connection.Select(
                            DB_Table.CAMPAIGNS,
                            new string[] { "id" },
                            new List<System.Tuple<string, Relation, object>> //TODO ?
                            {
                                new System.Tuple<string, Relation, object>("start_year",Relation.LESS_EQUAL,regDT.Year),
                                new System.Tuple<string, Relation, object>("end_year",Relation.GREATER_EQUAL,regDT.Year)
                            }
                            )[0][0].ToString()
                             + entranceRow[2].ToString() + uint.Parse(entranceRow[2].ToString().Split('.')[1]) + entranceRow[4].ToString() + entranceRow[6].ToString()
                             ))); //TODO ?

                    applications.Add(new Application(
                        new TUID(row[0].ToString()),
                        row[1].ToString(),
                        new Entrant(
                            new TUID(entRow[0].ToString()),
                            entRow[1].ToString(),
                            entRow[2].ToString(),
                            (uint)entRow[4],
                            new EmailOrMailAddress(entRow[6].ToString()),
                            entRow[3].ToString(),
                            entRow[5].ToString()
                            ),
                        new TDateTime((ushort)regDT.Year, (byte)regDT.Month, (byte)regDT.Day, (byte)regDT.Hour, (byte)regDT.Minute, (byte)regDT.Second),
                        row[3],
                        row[4],
                        finSourceEduForms,
                        ,
                        row[5],
                        null,//TODO ?
                        ,


                        ));
                }

            return applications;*/
            return null;
        }

        private static Orders PackOrders(DB_Connector connection)
        {
            return null;
        }
    }
}
