﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedClasses.DB
{
    public static class DB_Queries
    {
        public class Mark
        {
            public readonly uint ApplID;
            public readonly uint SubjID;
            public readonly byte Value;
            public readonly bool Checked;
            public readonly DateTime? FromExamDate;

            public Mark(uint applID, uint subjID, byte value, bool checked_, DateTime? fromExamDate)
            {
                ApplID = applID;
                SubjID = subjID;
                Value = value;
                Checked = checked_;
                FromExamDate = fromExamDate;
            }
        }

        public class Exam
        {
            public readonly uint ID;
            public readonly uint SubjID;
            public readonly DateTime Date;
            public readonly DateTime RegStartDate;
            public readonly DateTime RegEndDate;

            public Exam(uint id, uint subjID, DateTime date, DateTime regStartDate, DateTime regEndDate)
            {
                ID = id;
                SubjID = subjID;
                Date = date;
                RegStartDate = regStartDate;
                RegEndDate = regEndDate;
            }
        }

        public class Document
        {
            public readonly uint ApplID;
            public readonly uint ID;
            public readonly string Type;
            public readonly string Series;
            public readonly string Number;
            public readonly DateTime? Date;
            public readonly string Organization;
            public readonly DateTime? OrigDate;

            public Document(uint applID, uint id, string type, string series, string number, DateTime? date, string organization, DateTime? origDate)
            {
                ApplID = applID;
                ID = id;
                Type = type;
                Series = series;
                Number = number;
                Date = date;
                Organization = organization;
                OrigDate = origDate;
            }
        }

        public static IEnumerable<Mark> GetMarks(DB_Connector connection, IEnumerable<uint> applications, uint campaignID)
        {
            #region Contracts
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (applications == null)
                throw new ArgumentNullException(nameof(applications));
            if (applications.Count() == 0)
                throw new ArgumentException("Коллекция с заявлениями должена содержать хотя бы один элемент.", nameof(applications));
            #endregion

            object[] campStartEnd = connection.Select(
                DB_Table.CAMPAIGNS,
                new string[] { "start_year", "end_year" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, campaignID) }
                )[0];

            return applications.Join(
                connection.Select(DB_Table.APPLICATION_EGE_RESULTS, "application_id", "subject_id", "value", "checked"),
                k1 => k1,
                k2 => k2[0],
                (s1, s2) => new { ApplID = s1, Subj = (uint)s2[1], Mark = (byte)(ushort)s2[2], Checked = (bool)s2[3], ExamDate = (DateTime?)null }
                ).Concat(
                applications.Join(
                connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id"),
                k1 => k1,
                k2 => k2[0],
                (s1, s2) => new { ApplID = s1, EntrID = (uint)s2[1] }
                ).Join(
                    connection.Select(
                        DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                        new string[] { "entrant_id", "examination_id", "mark" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("mark", Relation.NOT_EQUAL, -1) }
                        ).Join(
                        connection.Select(DB_Table.EXAMINATIONS, "id", "subject_id", "date")
                        .Where(s => ((DateTime)s[2]).Year == (uint)campStartEnd[0] || ((DateTime)s[2]).Year == (uint)campStartEnd[1]),
                        k1 => k1[1],
                        k2 => k2[0],
                        (s1, s2) => new { EntrID = (uint)s1[0], Subj = (uint)s2[1], Mark = (short)s1[2], Date = (DateTime)s2[2] }
                        ),
                    k1 => k1.EntrID,
                    k2 => k2.EntrID,
                    (s1, s2) => new { s1.ApplID, s2.Subj, Mark = (byte)s2.Mark, Checked = true, ExamDate = (DateTime?)s2.Date }
                    )).Select(s => new Mark(s.ApplID, s.Subj, s.Mark, s.Checked, s.ExamDate));
        }

        //TODO faculty_short_name и direction_id будут не нужны, если краткое имя профиля уникально.
        public static string GetProfileName(DB_Connector connection, string facultyShortName, uint directionID, string shortName)
        {
            #region Contracts
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(facultyShortName))
                throw new ArgumentException("Некорректное краткое имя факультета.", nameof(facultyShortName));
            if (string.IsNullOrWhiteSpace(shortName))
                throw new ArgumentException("Некорректное краткое имя профиля.", nameof(shortName));
            #endregion

            return connection.Select(
                DB_Table.PROFILES,
                new string[] { "name" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,facultyShortName),
                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,directionID),
                    new Tuple<string, Relation, object>("short_name",Relation.EQUAL,shortName)
                })[0][0].ToString();
        }

        public static Tuple<uint, uint> GetCampaignStartEnd(DB_Connector connection, uint campaignID)
        {
            #region Contracts
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            #endregion

            return connection.Select(
                DB_Table.CAMPAIGNS,
                new string[] { "start_year", "end_year" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, campaignID) }
                ).Select(s => Tuple.Create((uint)s[0], (uint)s[1])).Single();
        }

        public static IEnumerable<Exam> GetCampaignExams(DB_Connector connection, uint campaignID)
        {
            Tuple<uint, uint> curCampStartEnd = GetCampaignStartEnd(connection, campaignID);

            return connection.Select(DB_Table.EXAMINATIONS,
                new string[] { "id", "subject_id", "date", "reg_start_date", "reg_end_date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("date",Relation.GREATER_EQUAL,new DateTime((int)curCampStartEnd.Item1,1,1)),
                    new Tuple<string, Relation, object>("date",Relation.LESS_EQUAL,new DateTime((int)curCampStartEnd.Item2,12,31))
                }).Select(s => new Exam((uint)s[0], (uint)s[1], (DateTime)s[2], (DateTime)s[3], (DateTime)s[4]));
        }

        //TODO faculty будет не нужно, если изменится связь таблицы.
        public static IEnumerable<uint> GetDirectionEntranceTests(DB_Connector connection, uint campaignID, string faculty, uint direction)
        {
            return connection.Select(
                DB_Table.ENTRANCE_TESTS,
                new string[] { "subject_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,campaignID),
                    new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,faculty),
                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,direction)
                }).Select(s => (uint)s[0]);
        }

        //TODO  можно будет объединить с функцией выше, если изменится связь таблицы.
        public static IEnumerable<uint> GetDirectionEntranceTests(DB_Connector connection, uint campaignID, uint direction)
        {
            return connection.Select(
                DB_Table.ENTRANCE_TESTS,
                new string[] { "subject_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,campaignID),
                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,direction)
                }).Select(s => (uint)s[0]);
        }

        public static IEnumerable<Document> GetDocuments(DB_Connector connection, IEnumerable<uint> applications)
        {
            return applications.Join(
                connection.Select(DB_Table.APPLICATIONS_DOCUMENTS_VIEW),
                k1 => k1,
                k2 => k2[0],
                (s1, s2) => new Document(
                    s1,
                    (uint)s2[1],
                    s2[2].ToString(),
                    s2[3] as string,
                    s2[4] as string,
                    s2[5] as DateTime?,
                    s2[6] as string,
                    s2[7] as DateTime?
                    ));
        }

        public static IEnumerable<Document> GetApplicationDocuments(DB_Connector connection, uint applicationID)
        {
            return connection.CallProcedure("get_application_docs", new Dictionary<string, object> { { "id", applicationID } }).Select(s => new Document(
                applicationID,
                (uint)s[0],
                s[1].ToString(),
                s[2] as string,
                s[3] as string,
                s[4] as DateTime?,
                s[5] as string,
                s[6] as DateTime?
                ));
        }

        public static ushort GetApplicationIndAchMaxValue(DB_Connector connection, uint applicationID)
        {
            IEnumerable<ushort> list = connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, "id", "value").Join(
                connection.Select(
                    DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                    new string[] { "institution_achievement_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id",Relation.EQUAL, applicationID)
                    }),
                k1 => k1[0],
                k2 => k2[0],
                (s1, s2) => (ushort)s1[1]
                );

            return list.Count() != 0 ? list.Max() : (ushort)0;
        }

        public static bool ExaminationHasMarks(DB_Connector connection, uint id)
        {
            return connection.Select(
                DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                new string[] { "entrant_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("examination_id",Relation.EQUAL, id)
                }).Count != 0;
        }
    }
}
