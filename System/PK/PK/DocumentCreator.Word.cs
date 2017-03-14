using System.Collections.Generic;
using System.Xml.Linq;
using Novacode;

namespace PK
{
    static partial class DocumentCreator
    {
        static class Word
        {
            static readonly Dictionary<string, Alignment> _Alignments = new Dictionary<string, Alignment>
            {
                { "Center",Alignment.center},
                { "Right",Alignment.right},
                { "Both",Alignment.both}
            };

            public static void CreateFromTemplate(DB_Connector connection, Dictionary<string, Font> fonts, XElement wordTemplateElement, uint id, string resultFile)
            {
                DocX doc = DocX.Create(resultFile + ".docx");

                AddCoreProperties(doc);

                if (wordTemplateElement.Element("Properties") != null)
                    ApplyProperties(doc, wordTemplateElement.Element("Properties"));

                string placeholderGroup = null;
                foreach (XElement element in wordTemplateElement.Element("Structure").Elements())
                {
                    if (element.Element("Paragraph") != null)
                    {
                        XElement parElem = element.Element("Paragraph");
                        Paragraph paragraph = doc.InsertParagraph();

                        if (parElem.Element("Alighment") != null)
                            paragraph.Alignment = _Alignments[parElem.Element("Alighment").Value];

                        string parFontID = parElem.Element("FontID")?.Value;

                        if (parElem.Element("Parts") != null)
                            foreach (XElement part in parElem.Element("Parts").Elements())
                            {
                                XElement text = part.Element("Text");

                                if (text.Element("String") != null)
                                    paragraph.Append(text.Element("String").Value);
                                else
                                {
                                    string placeholder = text.Element("Placeholder").Value;
                                    if (placeholderGroup == null)
                                        placeholderGroup = GetPlaceholderGroup(placeholder);
                                    else
                                        if (placeholderGroup != GetPlaceholderGroup(placeholder))
                                        throw new System.Exception("В одном шаблоне не могут использоваться плейсхолдеры из разных групп. Конфликтная группа: " + GetPlaceholderGroup(placeholder));

                                    paragraph.Append(SelectByPlaceholder(connection, id, placeholder));
                                }

                                SetFont(paragraph, fonts, part.Element("FontID")?.Value ?? parFontID);
                            }
                    }
                    else
                    {
                        XElement tableElem = element.Element("Table");

                        List<string> colNames;
                        Dictionary<byte, ushort> colWidths;
                        List<System.Tuple<string, string>> colFonts;
                        GetTableFormatting(tableElem, out colNames, out colWidths, out colFonts);

                        List<object[]> rows = connection.RunProcedure(_PH_Table[tableElem.Element("Placeholder").Value], id);

                        bool numeration = bool.Parse(tableElem.Element("Numeration").Value);

                        Table table = doc.InsertTable(1, colNames.Count);
                        table.Design = TableDesign.TableGrid;

                        for (byte i = 0; i < colNames.Count; ++i)
                        {
                            Paragraph paragraph = table.Rows[0].Cells[i].InsertParagraph(colNames[i]);
                            SetFont(paragraph, fonts, colFonts[i].Item1);
                            // if (colWidths.ContainsKey(i))
                            //     table.SetColumnWidth(i, colWidths[i]); //TODO
                        }

                        byte count = 1;
                        foreach (object[] row in rows)
                        {
                            table.InsertRow();
                            if (numeration)
                            {
                                Paragraph paragraph = table.Rows[table.Rows.Count - 1].Cells[0].InsertParagraph(count.ToString());
                                SetFont(paragraph, fonts, colFonts[0].Item2);
                                count++;
                            }

                            for (byte i = 0; i < row.Length; ++i)
                            {
                                Paragraph paragraph = table.Rows[table.Rows.Count - 1].Cells[i + (byte)(numeration ? 1 : 0)].InsertParagraph(row[i].ToString());
                                SetFont(paragraph, fonts, colFonts[i].Item2);
                            }
                        }
                    }
                }

                doc.Save();
            }

            static void AddCoreProperties(DocX doc)
            {
                System.IO.Packaging.PackagePart coreProp = doc.PackagePart.Package.CreatePart(
                    new System.Uri("/docProps/core.xml", System.UriKind.Relative),
                    "application/vnd.openxmlformats-package.core-properties+xml",
                    System.IO.Packaging.CompressionOption.Normal
                    );

                using (System.IO.TextWriter tr = new System.IO.StreamWriter(coreProp.GetStream(System.IO.FileMode.Open, System.IO.FileAccess.Write)))
                {
                    System.DateTime current = System.DateTime.UtcNow;
                    tr.Write(@"<cp:coreProperties
xmlns:cp=""http://schemas.openxmlformats.org/package/2006/metadata/core-properties""
xmlns:dc=""http://purl.org/dc/elements/1.1/""
xmlns:dcterms=""http://purl.org/dc/terms/""
xmlns:dcmitype=""http://purl.org/dc/dcmitype/""
xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
<dc:creator>MADI PK</dc:creator>
<dcterms:created xsi:type=""dcterms:W3CDTF"">" + current.ToString("o") + @"</dcterms:created>
</cp:coreProperties>"
);
                }

                doc.PackagePart.Package.CreateRelationship(coreProp.Uri,
                    System.IO.Packaging.TargetMode.Internal,
                    "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties"
                    );
            }

            static void ApplyProperties(DocX doc, XElement properties)
            {
                if (properties.Element("Borders") != null)
                    AddBorders(doc);

                if (properties.Element("A5") != null)
                {
                    doc.PageWidth = 419.5f;
                    doc.PageHeight = 595.2f;
                }

                if (properties.Element("Album") != null)
                {
                    float buf = doc.PageWidth;
                    doc.PageWidth = doc.PageHeight;
                    doc.PageHeight = buf;
                }

                if (properties.Element("Margins") != null)
                {
                    XElement margins = properties.Element("Margins");

                    if (margins.Element("Left") != null)
                        doc.MarginLeft = float.Parse(margins.Element("Left").Value);
                    if (margins.Element("Top") != null)
                        doc.MarginTop = float.Parse(margins.Element("Top").Value);
                    if (margins.Element("Right") != null)
                        doc.MarginRight = float.Parse(margins.Element("Right").Value);
                    if (margins.Element("Bottom") != null)
                        doc.MarginBottom = float.Parse(margins.Element("Bottom").Value);
                }
            }

            static void SetFont(Paragraph paragraph, Dictionary<string, Font> fonts, string fontID)
            {
                if (fontID != null)
                {
                    if (fonts[fontID].Name != null)
                        paragraph.Font(new System.Drawing.FontFamily(fonts[fontID].Name));

                    if (fonts[fontID].Size.HasValue)
                        paragraph.FontSize(fonts[fontID].Size.Value);

                    if (fonts[fontID].Style != null)
                        switch (fonts[fontID].Style)
                        {
                            case "Bold":
                                paragraph.Bold();
                                break;
                            case "Italic":
                                paragraph.Italic();
                                break;
                            default:
                                throw new System.Exception("Reached unreacheable.");
                        }

                    if (fonts[fontID].Color.HasValue)
                        paragraph.Color(fonts[fontID].Color.Value);

                }
            }

            static void AddBorders(DocX doc)
            {
                XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                XAttribute borderVal = new XAttribute(w + "val", "single");
                XAttribute borderSz = new XAttribute(w + "sz", 4);
                XAttribute borderSpace = new XAttribute(w + "space", 24);
                XAttribute borderColor = new XAttribute(w + "color", "auto");

                doc.PageLayout.Xml.Add(new XElement(w + "pgBorders",
                    new XAttribute(w + "offsetFrom", "page"),
                    new XElement(w + "top", borderVal, borderSz, borderSpace, borderColor),
                    new XElement(w + "left", borderVal, borderSz, borderSpace, borderColor),
                    new XElement(w + "bottom", borderVal, borderSz, borderSpace, borderColor),
                    new XElement(w + "right", borderVal, borderSz, borderSpace, borderColor)
                    ));
            }
        }
    }
}
