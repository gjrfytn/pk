using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    static class OutDocuments
    {
        public static void RegistrationJournal(DB_Connector connection, System.DateTime date)
        {
            Dictionary<System.Tuple<uint, uint>, string> streams = new Dictionary<System.Tuple<uint, uint>, string>
            {
                {new System.Tuple<uint,uint>(11,14),"ОБ" },
                {new System.Tuple<uint,uint>(11,15),"ОП" },
                {new System.Tuple<uint,uint>(12,14),"ОЗБ" },
                {new System.Tuple<uint,uint>(12,15),"ОЗП" },
                {new System.Tuple<uint,uint>(10,15),"ЗП" },
                {new System.Tuple<uint,uint>(11,20),"ОКВ" },
                {new System.Tuple<uint,uint>(11,16),"ОЦП" },
                {new System.Tuple<uint,uint>(12,20),"ОЗКВ" },
                {new System.Tuple<uint,uint>(12,16),"ОЗЦП" }
            };

            List<string[]> data = new List<string[]>();

            date = date.Date;

            var applications = connection.Select(
                DB_Table.APPLICATIONS,
                "id", "entrant_id", "registration_time"
                ).Where(a => ((System.DateTime)a[2]).Date == date).Join(
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
                    ApplID = s1.ApplID,
                    EntrID = s1.EntrID,
                    LastName = s2[1].ToString(),
                    FirstName = s2[2].ToString(),
                    MiddleName = s2[3].ToString(),
                    HomePhone = s1.HomePhone,
                    MobilePhone = s1.MobilePhone
                }
                );

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
                    Original = s2[4] as System.DateTime?
                }
                );

            foreach (var appl in applications)
            {
                object[] idDoc = connection.Select(
                    DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA,
                    new string[] { "reg_index", "reg_region", "reg_district", "reg_town", "reg_street", "reg_house", "reg_flat" },
                    new List<System.Tuple<string, Relation, object>>
                    {
                        new System.Tuple<string, Relation, object>("document_id",Relation.EQUAL,applDocs.Single(d=>d.ApplID==appl.ApplID&&d.DocType=="identity").DocID)
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
                        throw new System.Exception("Unreacheble reached.");
                }

                List<object[]> applEntr = connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "edu_form_id", "edu_source_id" },
                    new List<System.Tuple<string, Relation, object>>
                    {
                        new System. Tuple<string, Relation, object>("application_id",Relation.EQUAL,appl.ApplID)
                    }
                    );

                data.Add(new string[]
                {
                    appl.EntrID.ToString(),
                    appl.LastName+" "+ appl.FirstName +" "+appl.MiddleName,
                    string.Join(", ",idDoc.Where(o=>o.ToString()!=""))   + "\n\n"+appl.HomePhone+", "+appl.MobilePhone,
                    (eduDoc.Original!=null?"Оригинал ":"Копия ")+eduDocName+" "+eduDoc.DocSeries+eduDoc.DocNumber,
                    string.Join(", ",applEntr.Select(en=>streams[new System.Tuple<uint, uint>( (uint)en[0],(uint)en[1])]).Distinct())
                });
            }

            string doc = Utility.TempPath + "registrationJournal";
            DocumentCreator.Create(
                Utility.DocumentsTemplatesPath + "RegistrationJournal.xml",
                doc,
                null,
                new List<string[]>[] { data }
                );
            doc += ".docx";
            Utility.Print(doc);
        }
    }
}
