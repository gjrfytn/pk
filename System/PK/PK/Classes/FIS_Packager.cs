using System.Collections.Generic;
//using System.Linq;
using PK.Classes.ImportClasses;
//using System;

namespace PK.Classes
{
    static class FIS_Packager
    {
        static readonly Dictionary<uint, char> EduFormLiterals = new Dictionary<uint, char>() { { 11, 'Д' }, { 12, 'В' }, { 10, 'З' } };
        static readonly Dictionary<uint, string> EduSourceLiterals = new Dictionary<uint, string>() { { 14, "ОО" }, { 15, "СН" }, { 16, "ЦН" }, { 20, "КВ" } };

        public static PackageData MakePackage(DB_Connector connection)
        {
            return new PackageData(
                PackCampaignInfo(connection),
                PackAdmissionInfo(connection),
                PackInstitutionAchievements(connection),
                PackTargetOrganizations(connection),
                PackApplications(connection),
                PackOrders(connection)
                );
        }

        static CampaignInfo PackCampaignInfo(DB_Connector connection)
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
                    if ((uint)diRow[1] == 14)
                        eduForms.Add(new EducationFormID((uint)diRow[2]));
                    else
                        eduLevels.Add(new EducationLevelID((uint)diRow[2]));

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

        static AdmissionInfo PackAdmissionInfo(DB_Connector connection)
        {
            List<AVItem> admissionVolumes = new List<AVItem>();
            List<CompetitiveGroup> competitiveGroups = new List<CompetitiveGroup>();
            foreach (object[] row in connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA))
            {
                ushort paid_o = 0, paid_oz = 0, paid_z = 0;
                foreach (object[] profRow in connection.Select(
                    DB_Table.CAMPAIGNS_PROFILES_DATA,
                    new string[] { "places_paid_o, places_paid_oz, places_paid_z" },
                    new List<System.Tuple<string, Relation, object>>
                    {
                        new System.Tuple<string, Relation, object>("campaigns_id",Relation.EQUAL,row[0]),
                        new System.Tuple<string, Relation, object>("profiles_direction_id",Relation.EQUAL,row[1]),
                        new System.Tuple<string, Relation, object>("profiles_direction_faculty",Relation.EQUAL,row[2])
                    }))
                {
                    paid_o += (ushort)profRow[0];
                    paid_oz += (ushort)profRow[1];
                    paid_z += (ushort)profRow[2];
                }

                uint levelID = uint.Parse(connection.Select(
                    DB_Table.DICTIONARY_10_ITEMS,
                    new string[] { "code" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("id", Relation.EQUAL, row[2]) }
                    )[0][0].ToString().Split('.')[1]);

                admissionVolumes.Add(new AVItem(
                    new TUID(row[0].ToString() + row[1].ToString() + row[2]),
                    new TUID(row[0].ToString()),
                    levelID,
                    (uint)row[2],
                    (uint)row[3], (uint)row[4], (uint)row[5],
                    paid_o, paid_oz, paid_z,
                    (uint)row[6], (uint)row[7], (uint)row[8],
                    (uint)row[9], (uint)row[10], (uint)row[11]
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
                            places = (ushort)row[3];
                            break;
                        case CompetitiveGroupItem.Variants.NumberBudgetOZ:
                            eduForm = 12;
                            eduSource = 14;
                            places = (ushort)row[4];
                            break;
                        case CompetitiveGroupItem.Variants.NumberBudgetZ:
                            eduForm = 10;
                            eduSource = 14;
                            places = (ushort)row[5];
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
                            places = (ushort)row[6];
                            break;
                        case CompetitiveGroupItem.Variants.NumberQuotaOZ:
                            eduForm = 12;
                            eduSource = 20;
                            places = (ushort)row[7];
                            break;
                        case CompetitiveGroupItem.Variants.NumberQuotaZ:
                            eduForm = 10;
                            eduSource = 20;
                            places = (ushort)row[8];
                            break;
                        case CompetitiveGroupItem.Variants.NumberTargetO:
                            eduForm = 11;
                            eduSource = 16;
                            places = (ushort)row[9];
                            break;
                        case CompetitiveGroupItem.Variants.NumberTargetOZ:
                            eduForm = 12;
                            eduSource = 16;
                            places = (ushort)row[10];
                            break;
                        case CompetitiveGroupItem.Variants.NumberTargetZ:
                            eduForm = 10;
                            eduSource = 16;
                            places = (ushort)row[11];
                            break;
                        default:
                            throw new System.Exception("Unreachable reached.");
                    }

                    if (places != 0)
                    {
                        string compGroupUID = row[0].ToString() + row[2].ToString() + levelID.ToString() + eduForm.ToString() + eduSource.ToString();

                        List<EntranceTestItem> entranceTests = new List<EntranceTestItem>();
                        foreach (object[] etRow in connection.Select(
                            DB_Table.ENTRANCE_TESTS,
                            new string[] { "subject_id", "priority" },
                            new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("direction_id", Relation.EQUAL, row[2]) }
                            ))
                            entranceTests.Add(new EntranceTestItem(
                                new TUID(compGroupUID + row[0].ToString()),
                                1,//TODO ?
                                (uint)etRow[1],
                                new TEntranceTestSubject((uint)row[0]),
                                null //TODO Добавить!
                                ));

                        competitiveGroups.Add(new CompetitiveGroup(
                            new TUID(compGroupUID),
                            new TUID(row[0].ToString()),
                            row[0].ToString() + " " +
                            connection.Select(
                                DB_Table.DICTIONARY_10_ITEMS,
                                new string[] { "code" },
                                new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("id", Relation.EQUAL, row[2]) }
                                )[0][0].ToString() +
                            " " + EduSourceLiterals[eduSource] + " " + EduFormLiterals[eduForm],
                            levelID,
                            eduSource,
                            eduForm,
                            (uint)row[2],
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

        static List<InstitutionAchievement> PackInstitutionAchievements(DB_Connector connection)
        {
            List<InstitutionAchievement> achievements = new List<InstitutionAchievement>();

            foreach (object[] campRow in connection.Select(DB_Table.CAMPAIGNS, "id"))
                foreach (object[] row in connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS))
                    achievements.Add(new InstitutionAchievement(
                        new TUID(row[0].ToString()),
                        row[1].ToString(),
                        (uint)row[3],
                        (decimal)row[4],
                        new TUID(campRow[0].ToString())
                        ));

            return achievements;
        }

        static List<TargetOrganizationImp> PackTargetOrganizations(DB_Connector connection)
        {
            List<TargetOrganizationImp> organizations = new List<TargetOrganizationImp>();

            foreach (object[] row in connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS))
                organizations.Add(new TargetOrganizationImp(new TUID(row[0].ToString()), row[1].ToString()));

            return organizations;
        }

        static List<Application> PackApplications(DB_Connector connection)
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
                    foreach (object[] entranceRow in connection.Select(DB_Table.APPLICATIONS_ENTRANCES))
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

        static Orders PackOrders(DB_Connector connection)
        {
            return null;
        }
    }
}
