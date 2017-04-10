using System.Xml.Linq;
using System.Collections.Generic;

namespace PK.Classes
{
    static partial class DocumentCreator
    {
        static class Excel
        {
            public static void CreateFromTemplate(DB_Connector connection, Dictionary<string, Font> fonts, XElement excelTemplateElement, uint[] ids, string resultFile)
            {
                List<string> colNames;
                Dictionary<byte, ushort> colWidths;
                List<System.Tuple<string, string>> colFonts;
                GetTableFormatting(excelTemplateElement, out colNames, out colWidths, out colFonts);

                List<object[]> rows;

                if (excelTemplateElement.Element("Placeholder") != null)
                {
                    foreach (XElement column in excelTemplateElement.Element("Structure").Elements())
                        if (column.Element("Placeholder") != null)
                            throw new System.Exception("Нельзя задавать плейсхолдеры для отдельных столбцов, если задан табличный плейсхолдер. Значение: " + column.Element("Placeholder").Value);

                    rows = connection.RunProcedure(_PH_Table[excelTemplateElement.Element("Placeholder").Value], ids[0]);
                }
                else
                {
                    List<string> placeholders = new List<string>();
                    string placeholderGroup = null;

                    foreach (XElement column in excelTemplateElement.Element("Structure").Elements())
                    {
                        string placeholder = column.Element("Placeholder").Value;
                        if (placeholderGroup == null)
                            placeholderGroup = GetPlaceholderGroup(placeholder);
                        else
                            if (placeholderGroup != GetPlaceholderGroup(placeholder))
                            throw new System.Exception("В одном шаблоне не могут использоваться плейсхолдеры из разных групп. Конфликтная группа: " + GetPlaceholderGroup(placeholder));

                        placeholders.Add(placeholder);
                    }

                    rows = new List<object[]>();
                    foreach (uint id in ids)
                    {
                        rows.Add(new object[placeholders.Count]);
                        for (byte i = 0; i < placeholders.Count; ++i)
                            rows[rows.Count - 1][i] = SelectByPlaceholder(connection, id, placeholders[i]);
                    }
                }

                if (bool.Parse(excelTemplateElement.Element("Numeration").Value))
                    for (byte i = 0; i < rows.Count; ++i)
                    {
                        object[] buf = new object[rows[i].Length + 1];
                        buf[0] = i + 1;
                        for (byte j = 0; j < rows[i].Length; ++j)
                            buf[j + 1] = rows[i][j];
                    }

                Create(colNames, colWidths, fonts, colFonts, rows, resultFile);
            }


            private static void Create(List<string> columnsNames, Dictionary<byte, ushort> columnsWidth, Dictionary<string, Font> fonts, List<System.Tuple<string, string>> columnsFonts, List<object[]> rows, string resultFile)
            {
                XNamespace ss = "urn:schemas-microsoft-com:office:spreadsheet";
                XElement styles = new XElement(ss + "Styles", new XAttribute("xmlns", ss.NamespaceName));

                Dictionary<string, string> fontsIDs = new Dictionary<string, string>();
                byte count = 0;
                foreach (var font in fonts)
                {
                    XElement fontElement = new XElement(ss + "Font");

                    if (font.Value.Name != null)
                        fontElement.Add(new XAttribute(ss + "FontName", font.Value.Name));
                    if (font.Value.Size.HasValue)
                        fontElement.Add(new XAttribute(ss + "Size", font.Value.Size));
                    if (font.Value.Style != null)
                        fontElement.Add(new XAttribute(ss + font.Value.Style, 1));

                    if (font.Value.Color.HasValue)
                        fontElement.Add(new XAttribute(ss + "Color",
                          "#" + font.Value.Color.Value.R.ToString("X2") + font.Value.Color.Value.G.ToString("X2") + font.Value.Color.Value.B.ToString("X2")
                          ));

                    string id = "s" + count;
                    styles.Add(new XElement(ss + "Style", new XAttribute(ss + "ID", id), fontElement));
                    fontsIDs.Add(font.Key, id);
                    count++;
                }

                List<XElement> colElements = new List<XElement>();
                foreach (var col in columnsWidth)
                    colElements.Add(new XElement(ss + "Column",
                        new XAttribute(ss + "Index", col.Key + 1),
                        new XAttribute(ss + "Width", col.Value)
                        ));

                List<XElement> rowElements = new List<XElement>();

                XElement captionRow = new XElement(ss + "Row");
                for (byte i = 0; i < columnsNames.Count; ++i)
                {
                    XElement cell = new XElement(ss + "Cell",
                        new XElement(ss + "Data",
                            new XAttribute(ss + "Type", "String"),
                            columnsNames[i]
                        ));

                    if (columnsFonts[i].Item1 != null)
                        cell.Add(new XAttribute(ss + "StyleID", fontsIDs[columnsFonts[i].Item1]));

                    captionRow.Add(cell);
                }

                rowElements.Add(captionRow);

                foreach (object[] row in rows)
                {
                    XElement xmlRow = new XElement(ss + "Row");
                    for (byte i = 0; i < row.Length; ++i)
                    {
                        XElement cell = new XElement(ss + "Cell",
                        new XElement(ss + "Data",
                            new XAttribute(ss + "Type", "String"),
                            row[i]
                        ));

                        if (columnsFonts[i].Item2 != null)
                            cell.Add(new XAttribute(ss + "StyleID", fontsIDs[columnsFonts[i].Item2]));

                        xmlRow.Add(cell);
                    }

                    rowElements.Add(xmlRow);
                }

                XNamespace x = "urn:schemas-microsoft-com:office:excel";
                XNamespace o = "urn:schemas-microsoft-com:office:office";

                XDocument doc = new XDocument(
                                    new XProcessingInstruction("mso-application", "progid=\"Excel.Sheet\""),
                                    new XElement(
                                        ss + "Workbook",
                                        new XAttribute("xmlns", ss.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "x", x.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "ss", ss.NamespaceName),
                                        new XElement(
                                            o + "DocumentProperties",
                                            new XElement(o + "Author", "IS PK MADI"),
                                            new XElement(o + "Created", System.DateTime.Now)
                                            ),
                                        styles,
                                        new XElement(ss + "Worksheet",
                                            new XAttribute("xmlns", ss.NamespaceName),
                                            new XAttribute(ss + "Name", "Лист1"),
                                            new XElement(ss + "Table",
                                            colElements.ToArray(),
                                            rowElements.ToArray()
                                            ))));

                doc.Save(resultFile + ".xml");
            }
        }
    }
}
