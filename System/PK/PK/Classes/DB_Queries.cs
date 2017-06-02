using System;
using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    static class DB_Queries
    {
        public class Mark
        {
            public readonly uint ApplID;
            public readonly uint SubjID;
            public readonly byte Value;
            public readonly bool Checked;
            public readonly bool FromExam;

            public Mark(uint applID, uint subjID, byte value, bool checked_, bool fromExam)
            {
                ApplID = applID;
                SubjID = subjID;
                Value = value;
                Checked = checked_;
                FromExam = fromExam;
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
                connection.Select(DB_Table.APPLICATIONS_EGE_MARKS_VIEW, "applications_id", "subject_id", "value", "checked"),
                k1 => k1,
                k2 => k2[0],
                (s1, s2) => new { ApplID = s1, Subj = (uint)s2[1], Mark = (byte)(uint)s2[2], Checked = (bool)s2[3], Exam = false }
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
                        (s1, s2) => new { EntrID = (uint)s1[0], Subj = (uint)s2[1], Mark = (short)s1[2] }
                        ),
                    k1 => k1.EntrID,
                    k2 => k2.EntrID,
                    (s1, s2) => new { s1.ApplID, s2.Subj, Mark = (byte)s2.Mark, Checked = true, Exam = true }
                    )).Select(s => new Mark(s.ApplID, s.Subj, s.Mark, s.Checked, s.Exam));
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
    }
}
