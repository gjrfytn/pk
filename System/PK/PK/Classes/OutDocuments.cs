﻿using System.Collections.Generic;
using System.Linq;
using System;

namespace PK.Classes
{
    static class OutDocuments
    {
        public static void RegistrationJournal(DB_Connector connection, DateTime date)
        {
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
                    string.Join(", ",applEntr.Select(en=>streams[new Tuple<uint, uint>( (uint)en[0],(uint)en[1])]).Distinct())
                });
            }

            string doc = Utility.TempPath + "registrationJournal" + new Random().Next();
            DocumentCreator.Create(
                Utility.DocumentsTemplatesPath + "RegistrationJournal.xml",
                doc,
                null,
                new List<string[]>[] { data }
                );
            doc += ".docx";
            Utility.Print(doc);
        }

        public static void Order(DB_Connector connection, string number)
        {
            DB_Helper dbHelper = new DB_Helper(connection);

            object[] order = connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "type", "date", "protocol_number", "protocol_date", "education_form_id", "finance_source_id", "faculty_short_name", "direction_id", "profile_short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("number",Relation.EQUAL,number)
                    })[0];

            Tuple<string, string> dirNameCode = dbHelper.GetDirectionNameAndCode((uint)order[7]);
            string profile = order[8] as string;

            var applications = connection.Select(
                         DB_Table.ORDERS_HAS_APPLICATIONS,
                         new string[] { "applications_id" },
                         new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, number) }
                         ).Join(
                connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id"),
                k1 => k1[0],
                k2 => k2[0],
                (s1, s2) => new { ApplID = (uint)s2[0], EntrID = (uint)s2[1] }
                );

            var dir_subjects = connection.Select(
                 DB_Table.ENTRANCE_TESTS,
                 new string[] { "subject_id" },
                 new List<Tuple<string, Relation, object>>
                 {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,dbHelper.CurrentCampaignID),
                    new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,order[6]),
                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,order[7])
                 }).Select(s => (uint)s[0]);

            var marks = applications.Join(
                connection.Select(DB_Table.APPLICATIONS_EGE_MARKS_VIEW, "applications_id", "subject_id", "value"),
                k1 => k1.ApplID,
                k2 => k2[0],
                (s1, s2) => new { s1.ApplID, s1.EntrID, Subj = (uint)s2[1], Mark = (ushort)(uint)s2[2] }
                ).Concat(
                applications.Join(
                    connection.Select(
                        DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                        new string[] { "entrant_id", "examination_id", "mark" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("mark", Relation.NOT_EQUAL, -1) }
                        ).Join(
                        connection.Select(DB_Table.EXAMINATIONS, "id", "subject_id"),
                        k1 => k1[1],
                        k2 => k2[0],
                        (s1, s2) => new { EntrID = (uint)s1[0], Subj = (uint)s2[1], Mark = (short)s1[2] }
                        ),
                    k1 => k1.EntrID,
                    k2 => k2.EntrID,
                    (s1, s2) => new { s1.ApplID, s1.EntrID, s2.Subj, Mark = (ushort)s2.Mark }
                    ));

            var table = marks.Join(
                dir_subjects,
                k1 => k1.Subj,
                k2 => k2,
                (s1, s2) => s1
                ).GroupBy(
                k => Tuple.Create(k.ApplID, k.Subj),
                (k, g) => new { g.First().ApplID, g.First().EntrID, Mark = g.Max(s => s.Mark) }
                ).GroupBy(
                k => k.ApplID,
                (k, g) => new { g.First().ApplID, g.First().EntrID, Sum = g.Sum(s => s.Mark) }
                ).Join(
                connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                k1 => k1.EntrID,
                k2 => k2[0],
                (s1, s2) => new string[] { s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString(), s1.Sum.ToString() }
                );

            string doc = Utility.TempPath + "AdmOrder" + new Random().Next();
            DocumentCreator.Create(
                Utility.DocumentsTemplatesPath + "AdmOrder.xml",
                doc,
                new string[]
                {
                    ((DateTime)order[1]).ToShortDateString(),
                    number,
                    order[2].ToString(),
                    ((DateTime)order[3]).ToShortDateString(),
                    ((DateTime)order[1]).Year.ToString(),
                    dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM,(uint)order[4]).ToLower()+" обучения"+
                    ((uint)order[5]==dbHelper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE,DB_Helper.EduSourceP)?" по договорам с оплатой стоимости обучения":""),
                    order[6].ToString(),
                    dirNameCode.Item2,
                    dirNameCode.Item1,
                    profile!=null?"Профиль: ":"",
                    profile !=null?profile+" - "+connection.Select(
                        DB_Table.PROFILES,
                        new string[] { "name" },
                        new List<Tuple<string, Relation, object>> {new Tuple<string, Relation, object>("short_name",Relation.EQUAL,profile) }
                        )[0][0].ToString():""
                },
                new IEnumerable<string[]>[] { table }
                );
            Utility.Print(doc + ".docx");
        }
    }
}
