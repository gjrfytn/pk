using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK.Forms
{
    partial class ExaminationDocsPrint : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly uint _ExaminationID;
        private readonly string _ExamName;
        private readonly string _ExamDate;
        private readonly List<string[]> _EntrantsTable;
        private readonly List<object[]> _Audiences;
        private readonly List<Tuple<char, string>> _Distribution;
        private readonly IEnumerable<Tuple<uint, string, string, string>> _Entrants;

        public ExaminationDocsPrint(Classes.DB_Connector connection, uint examinationID)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _ExaminationID = examinationID;

            object[] buf = _DB_Connection.Select(
                DB_Table.EXAMINATIONS,
                new string[] { "subject_id", "date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id",Relation.EQUAL,_ExaminationID)
                })[0];

            _ExamName = new Classes.DB_Helper(_DB_Connection).GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)buf[0]);
            _ExamDate = ((DateTime)buf[1]).ToShortDateString();

            var entrantsIDs = _DB_Connection.Select(
                DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                new string[] { "entrant_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,_ExaminationID)
                });

            _Entrants = _DB_Connection.Select(
                DB_Table.ENTRANTS,
                new string[] { "id", "last_name", "first_name", "middle_name" }
                ).Join(
                entrantsIDs,
                en => en[0],
                i => i[0],
                (s1, s2) => new Tuple<uint, string, string, string>
                (
                    (uint)s1[0],
                    s1[1].ToString(),
                    s1[2].ToString(),
                    s1[3].ToString()
                    ));

            _Audiences = _DB_Connection.Select(DB_Table.EXAMINATIONS_AUDIENCES,
                new string[] { "number", "capacity" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,_ExaminationID)
                });

            _Distribution = Classes.Utility.DistributeAbiturients(
                 _Audiences.ToDictionary(k => k[0].ToString(), v => (ushort)v[1]),
                 _Entrants.Select(en => en.Item2[0]).GroupBy(en => en).ToDictionary(k => k.Key, v => (ushort)v.Count())
                 );

            Dictionary<char, string> nameCodes = new Dictionary<char, string>
            {
                { 'А',"Q" },{'Б',"W" },{'В',"E" },{'Г',"R" },{'Д',"T" },{'Е',"Y" },{'Ё',"U" },
                { 'Ж',"I" },{'З',"O" },{'И',"P" },{'Й',"A" },{'К',"S" },{'Л',"D" },{'М',"F" },
                { 'Н',"G" },{'О',"H" },{'П',"J" },{'Р',"K" },{'С',"L" },{'Т',"Z" },{'У',"X" },
                { 'Ф',"C" },{'Х',"V" },{'Ц',"B" },{'Ч',"N" },{'Ш',"M" },{'Щ',"GQ" },{'Э',"KI" },
                { 'Ю',"AC" },{'Я',"MK" }
            };

            Dictionary<string, Tuple<ushort, ushort>> fill = _Audiences.ToDictionary(k => k[0].ToString(), v => new Tuple<ushort, ushort>(0, (ushort)v[1]));
            _EntrantsTable = new List<string[]>(_Entrants.Count());
            ushort count = 1;
            foreach (var entr in _Entrants)
            {
                string name = entr.Item2 + " " + entr.Item3 + " " + entr.Item4;
                foreach (Tuple<char, string> aud in _Distribution.FindAll(c => c.Item1 == name[0]))
                    if (fill[aud.Item2].Item1 < fill[aud.Item2].Item2)
                    {
                        fill[aud.Item2] = new Tuple<ushort, ushort>((ushort)(fill[aud.Item2].Item1 + 1), fill[aud.Item2].Item2);

                        _EntrantsTable.Add(new string[]
                        {
                            count.ToString(),
                            entr.Item1.ToString(),
                            name,
                            nameCodes[entr.Item2[0]]+nameCodes[entr.Item3[0]]+"."+_ExaminationID.ToString()+count.ToString(),
                            aud.Item2,
                            fill[aud.Item2].Item1.ToString()
                        });

                        break;
                    }

                count++;
            }
        }

        private void bAlphaCodes_Click(object sender, System.EventArgs e)
        {
            string doc = ".\\temp\\AlphaCodes";
            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "AlphaCodes.xml",
                doc,
                new string[] { _ExamName, _ExamDate },
                new List<string[]>[] { _EntrantsTable.Select(s => new string[] { s[0], s[1], s[2], s[3] }).ToList() }
                );
            Classes.Utility.Print(doc + ".docx");
        }

        private void bAlphaAuditories_Click(object sender, EventArgs e)
        {
            string doc = ".\\temp\\AlphaAuditories";
            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "AlphaAuditories.xml",
                doc,
                new string[] { _ExamName, _ExamDate },
                new List<string[]>[] { _EntrantsTable.Select(s => new string[] { s[0], s[1], s[2], s[4] }).ToList() }
                );
            Classes.Utility.Print(doc + ".docx");
        }

        private void bAbitAudDistib_Click(object sender, EventArgs e)
        {
            List<string[]> distribTable = new List<string[]>(_Audiences.Count);
            foreach (object[] aud in _Audiences)
            {
                var letters = _Distribution.Where(d => d.Item2 == aud[0].ToString()).Select(s => s.Item1);
                distribTable.Add(new string[]
                {
                    aud[0].ToString(),
                    letters.Any()?letters.Aggregate("",(a,d)=> a+= d+", ",s=>s.Remove(s.Length- 2)):"-",
                    aud[1].ToString(),
                    _Entrants.Where(en=>letters.Contains(en.Item2[0])).Count().ToString()
                });
            }

            string doc = ".\\temp\\AbitAudDistrib";
            Classes.DocumentCreator.Create(
               Classes.Utility.DocumentsTemplatesPath + "AbitAudDistrib.xml",
               doc,
               new string[]
               {
                   _ExamName,
                   _ExamDate,
                   _Entrants.Count().ToString(),
                   _Audiences.Sum(a=>(ushort)a[1]).ToString()
               },
               new List<string[]>[] { distribTable }
               );
            Classes.Utility.Print(doc + ".docx");
        }

        private void bExamCardsSheet_Click(object sender, EventArgs e)
        {
            foreach (Tuple<char, string> group in _Distribution)
            {
                string doc = ".\\temp\\ExamCardsSheet_" + group.Item1 + "_" + group.Item2;
                Classes.DocumentCreator.Create(
                    Classes.Utility.DocumentsTemplatesPath + "ExamCardsSheet.xml",
                    doc,
                    new string[] { group.Item2, group.Item1.ToString(), _ExamName, _ExamDate },
                    new List<string[]>[]
                    {
                        _EntrantsTable.Where(en=>en[2][0]== group.Item1&&en[4]==group.Item2).Select(s=>new string[] {s[1],s[2],s[5] }).ToList()
                    });
                Classes.Utility.Print(doc + ".docx");
            }
        }
    }
}
