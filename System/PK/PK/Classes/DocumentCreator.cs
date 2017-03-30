using System.Xml.Linq;
using System.Xml.Schema;
using System.Collections.Generic;

namespace PK.Classes
{
    static partial class DocumentCreator
    {
        struct Font
        {
            public string Name;
            public ushort? Size;
            public string Style;
            public System.Drawing.Color? Color;
        }

        private static readonly Dictionary<string, System.Drawing.Color> _Colors = new Dictionary<string, System.Drawing.Color>
        {
            {"Red",System.Drawing.Color.Red },
            {"Violet",System.Drawing.Color.Violet }
        };

        private static readonly XmlSchemaSet _SchemaSet = new XmlSchemaSet();

        public static void Create(DB_Connector connection, string templateFile, string resultFile, uint id)
        {
            XDocument template = XDocument.Load(templateFile, LoadOptions.PreserveWhitespace);

            Validate(template);

            if (template.Root.Element("Document").Element("Word") != null)
                Word.CreateFromTemplate(connection, GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), id, resultFile);
            else
                throw new System.ArgumentException("Эта перегрузка принимат только тип шаблона \"Word\".", "templateFile");
        }

        public static void Create(string templateFile, string resultFile, string[] singleParams, List<string[]>[] tableParams)
        {
            XDocument template = XDocument.Load(templateFile, LoadOptions.PreserveWhitespace);

            Validate(template);

            if (template.Root.Element("Document").Element("Word") != null)
                Word.CreateFromTemplate(GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), singleParams, tableParams, resultFile);
            else
                throw new System.ArgumentException("Эта перегрузка принимат только тип шаблона \"Word\".", "templateFile");
        }

        public static void Create(DB_Connector connection, string templateFile, string resultFile, uint[] ids = null)
        {
            XDocument template = XDocument.Load(templateFile, LoadOptions.PreserveWhitespace);

            Validate(template);

            if (template.Root.Element("Document").Element("Excel") != null)
                Excel.CreateFromTemplate(connection, GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Excel"), ids, resultFile);
            else
                throw new System.ArgumentException("Эта перегрузка принимат только тип шаблона \"Excel\".", "templateFile");
        }

        private static void Validate(XDocument doc)
        {
            _SchemaSet.Add(null, "D:\\Dmitry\\Documents\\GitHub\\pk\\System\\Schemas\\DocumentSchema.xsd");
            doc.Validate(_SchemaSet, (sender, e) => { throw e.Exception; });
        }

        private static Dictionary<string, Font> GetFonts(XElement fonts)
        {
            Dictionary<string, Font> result = new Dictionary<string, Font>();
            foreach (XElement font in fonts.Elements())
            {
                result.Add(font.Attribute("ID").Value,
                    new Font
                    {
                        Name = font.Element("Name")?.Value ?? null,
                        Size = font.Element("Size") != null ? (ushort?)ushort.Parse(font.Element("Size").Value) : null,
                        Style = font.Element("Style")?.Value ?? null,
                        Color = font.Element("Color") != null ? (System.Drawing.Color?)_Colors[font.Element("Color").Value] : null
                    });
            }

            return result;
        }

        private static void GetTableFormatting(XElement tableElement, out List<string> colNames, out Dictionary<byte, ushort> colWidths, out List<System.Tuple<string, string>> colFonts)
        {
            colNames = new List<string>();
            colWidths = new Dictionary<byte, ushort>();
            colFonts = new List<System.Tuple<string, string>>();
            byte count = 0;
            foreach (XElement column in tableElement.Element("Structure").Elements())
            {
                colNames.Add(column.Element("Caption").Value);
                if (column.Element("Width") != null)
                    colWidths.Add(count, ushort.Parse(column.Element("Width").Value));

                colFonts.Add(new System.Tuple<string, string>(column.Element("CaptionFontID")?.Value, column.Element("DataFontID")?.Value));

                count++;
            }
        }

        private static string GetPlaceholderGroup(string placeholder)
        {
            return new string(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.TakeWhile(placeholder, c => char.IsLower(c))));
        }

        private static string SelectByPlaceholder(DB_Connector connection, uint id, string placeholder)
        {
            string[] placeholderAndFunction = placeholder.Split('|');
            string[] placeholderValue = _PH_Single[placeholderAndFunction[0]].Split('.', ':');

            List<System.Tuple<string, Relation, object>> whereClause;
            if (placeholderValue.Length == 2)
                whereClause = new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("id", Relation.EQUAL, id)
                };
            else
            {
                whereClause = new List<System.Tuple<string, Relation, object>>();
                string[] equalities = placeholderValue[2].Split(',');
                foreach (string equality in equalities)
                {
                    string[] parts = equality.Split('=');

                    string rightValue;
                    if (parts[1][0] == '@')
                        rightValue = SelectByPlaceholder(connection, id, parts[1].Substring(1)); //TODO group?
                    else
                        rightValue = parts[1];

                    whereClause.Add(new System.Tuple<string, Relation, object>(parts[0], Relation.EQUAL, rightValue));
                }
            }
            var selectRes = connection.Select(
                (DB_Table)System.Enum.Parse(typeof(DB_Table), placeholderValue[0], true),
                new string[] { placeholderValue[1] },
                whereClause
                );

            if (selectRes.Count != 1)
                throw new System.Exception("По условию Placeholder возвращена не одна строка. Значение: " + placeholder);

            if (placeholderAndFunction.Length == 1)
                return selectRes[0][0].ToString();
            else
                return _PH_Functions[placeholderAndFunction[1]](selectRes[0][0]);
        }
    }
}
