using System;
using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    static class DB_Queries
    {
        public static IEnumerable<Tuple<uint, uint, byte, bool>> GetMarks(DB_Connector connection, IEnumerable<uint> applications, uint campaignID)
        {
            object[] campStartEnd = connection.Select(
                DB_Table.CAMPAIGNS,
                new string[] { "start_year", "end_year" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, campaignID) }
                )[0];

            return applications.Join(
                connection.Select(DB_Table.APPLICATIONS_EGE_MARKS_VIEW, "applications_id", "subject_id", "value", "checked"),
                k1 => k1,
                k2 => k2[0],
                (s1, s2) => new { ApplID = s1, Subj = (uint)s2[1], Mark = (byte)(uint)s2[2], Checked = (bool)s2[3] }
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
                    (s1, s2) => new { s1.ApplID, s2.Subj, Mark = (byte)s2.Mark, Checked = true }
                    )).Select(s => Tuple.Create(s.ApplID, s.Subj, s.Mark, s.Checked));
        }
    }
}
