using System.Xml.Linq;
using System.Xml.Schema;
using System.Collections.Generic;
using SharedClasses.DB;
using System.Linq;

namespace PK.Classes
{
    static partial class DocumentCreator
    {
        public class DocumentParameters
        {
            public readonly string Template;
            public readonly DB_Connector Connection;
            public readonly uint? ID;
            public readonly string[] SingleParameters;
            public readonly IEnumerable<string[]>[] TableParameters;
			private string v;
			private object p1;
			private object p2;
			private List<string> parameters;
			private object p3;

			public DocumentParameters(string template, DB_Connector connection, uint? id, string[] singleParameters, IEnumerable<string[]>[] tableParameters)
            {
                #region Contracts
                if (string.IsNullOrWhiteSpace(template))
                    throw new System.ArgumentException("Некорректное имя файла шаблона.", nameof(template));
                if (connection == null ^ id == null)
                    throw new System.ArgumentException("Подключение и ID нельзя передавать отдельно.");
                else if (connection == null && id == null)
                {
                    if (singleParameters == null && tableParameters == null)
                        throw new System.ArgumentException("Все аргументы, кроме имени шаблона, содержат null.");
                    if (singleParameters != null && singleParameters.Length == 0)
                        throw new System.ArgumentException("Массив с одиночными параметрами должен содержать хотя бы один элемент.", nameof(singleParameters));
                    if (tableParameters != null && tableParameters.Length == 0)
                        throw new System.ArgumentException("Массив с табличными параметрами должен содержать хотя бы один элемент.", nameof(tableParameters));
                }
                else if (singleParameters != null || tableParameters != null)
                    throw new System.ArgumentException("Одиночные или табличные параметры нельзя передавать вместе с подключением и ID.");
                #endregion

                Template = template;
                Connection = connection;
                ID = id;
                SingleParameters = singleParameters;
                TableParameters = tableParameters;
            }

			public DocumentParameters(string v, object p1, object p2, List<string> parameters, object p3)
			{
				this.v = v;
				this.p1 = p1;
				this.p2 = p2;
				this.parameters = parameters;
				this.p3 = p3;
			}
		}

        struct Font
        {
            public readonly string Name;
            public readonly ushort? Size;
            public readonly string Style;
            public readonly System.Drawing.Color? Color;

            public Font(string name, ushort? size, string style, System.Drawing.Color? color)
            {
                Name = name;
                Size = size;
                Style = style;
                Color = color;
            }
        }

        private static readonly string SchemaPath = Properties.Settings.Default.SchemasPath + "DocumentSchema.xsd";

        private static readonly Dictionary<string, System.Drawing.Color> _Colors = new Dictionary<string, System.Drawing.Color>
        {
            {"Red",System.Drawing.Color.Red },
            {"Violet",System.Drawing.Color.Violet }
        };

        private static readonly XmlSchemaSet _SchemaSet = new XmlSchemaSet();

        public static void Create(DB_Connector connection, string templateFile, string resultFile, uint id, bool readOnly = false)
        {
            #region Contracts
            CheckConnectionAndFilenames(connection, templateFile, resultFile);
            #endregion

            XDocument template = XDocument.Load(templateFile, LoadOptions.PreserveWhitespace);

            Validate(template);

            if (template.Root.Element("Document").Element("Word") != null)
            {
<<<<<<< HEAD
				Xceed.Words.NET.DocX doc = Word.CreateFromTemplate(connection, GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), id, resultFile);
=======
                Xceed.Words.NET.DocX doc = Word.CreateFromTemplate(connection, GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), id, resultFile);
>>>>>>> origin/master

                if (readOnly)
                    doc.AddProtection(Xceed.Words.NET.EditRestrictions.readOnly);
                doc.Save();
            }
            else
                throw new System.ArgumentException("Эта перегрузка принимат только тип шаблона \"Word\".", nameof(templateFile));
        }

        public static void Create(string templateFile, string resultFile, string[] singleParams, IEnumerable<string[]>[] tableParams, bool readOnly = false)
        {
            #region Contracts
            CheckFilenames(templateFile, resultFile);
            if (singleParams == null && tableParams == null)
                throw new System.ArgumentException("Необходимо передать хотя бы один одиночный или табличный параметр.");
            if (singleParams != null && singleParams.Length == 0)
                throw new System.ArgumentException("Массив с одиночными параметрами должен содержать хотя бы один элемент.", nameof(singleParams));
            if (tableParams != null && tableParams.Length == 0)
                throw new System.ArgumentException("Массив с табличными параметрами должен содержать хотя бы один элемент.", nameof(tableParams));
            #endregion

            XDocument template = XDocument.Load(templateFile, LoadOptions.PreserveWhitespace);

            Validate(template);

            if (template.Root.Element("Document").Element("Word") != null)
            {
<<<<<<< HEAD
				Xceed.Words.NET.DocX doc = Word.CreateFromTemplate(GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), singleParams, tableParams, resultFile);
=======
                Xceed.Words.NET.DocX doc = Word.CreateFromTemplate(GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), singleParams, tableParams, resultFile);
>>>>>>> origin/master

                if (readOnly)
                    doc.AddProtection(Xceed.Words.NET.EditRestrictions.readOnly);
                doc.Save();
            }
            else
                throw new System.ArgumentException("Эта перегрузка принимат только тип шаблона \"Word\".", nameof(templateFile));
        }

        public static void Create(string resultFile, IEnumerable<DocumentParameters> documents, bool readOnly = false)
        {
            #region Contracts
            if (string.IsNullOrWhiteSpace(resultFile))
                throw new System.ArgumentException("Некорректное имя выходного файла.", nameof(resultFile));
            if (System.Linq.Enumerable.Count(documents) == 0)
                throw new System.ArgumentException("Коллекция с документами должна содержать хотя бы один элемент.", nameof(documents));
			#endregion

<<<<<<< HEAD
			Xceed.Words.NET.DocX doc = null;
            foreach (var document in documents)
=======
            void AddItemToMagDirectionTable(Xceed.Words.NET.Table table, Xceed.Words.NET.Row rowPattern, string[] direction)
            {
                // Insert a copy of the rowPattern at the last index in the table.
                var newItem = table.InsertRow(rowPattern, table.RowCount - 1);

                // Replace the default values of the newly inserted row.
                newItem.ReplaceText("%faculty%", direction[0]);
                newItem.ReplaceText("%directionAndCode%", direction[1]);
                newItem.ReplaceText("%mag_program%", direction[4]);
                newItem.ReplaceText("%chair%", direction[5]);
                newItem.ReplaceText("%edu_form%", direction[2] + ", " + direction[3]);
            }
            void AddItemToBachDirectionTable(Xceed.Words.NET.Table table, Xceed.Words.NET.Row rowPattern, string[] direction)
>>>>>>> origin/master
            {
                // Insert a copy of the rowPattern at the last index in the table.
                var newItem = table.InsertRow(rowPattern, table.RowCount - 1);

                // Replace the default values of the newly inserted row.
                newItem.ReplaceText("%fack%", direction[0]);
                newItem.ReplaceText("%dir_prof%", direction[1]);
                newItem.ReplaceText("%level%", direction[2]);
                newItem.ReplaceText("%dir_prof_short_name%", direction[3]);
                newItem.ReplaceText("%edu_form%", direction[4]);
            }

            Xceed.Words.NET.DocX doc = null;
            foreach (var document in documents)
            {
                if (document.Template.IndexOf("ApplicationTemplate")>=0)
                {
                    Xceed.Words.NET.DocX buf = Xceed.Words.NET.DocX.Load(document.Template);
                    buf.ReplaceText("<FIO>", document.SingleParameters[0]);
                    buf.ReplaceText("<gender>", document.SingleParameters[1]);
                    buf.ReplaceText("<birth_date>", document.SingleParameters[2]);
                    buf.ReplaceText("<nationality>", document.SingleParameters[3]);
                    buf.ReplaceText("<identity_type>", document.SingleParameters[4]);
                    buf.ReplaceText("<identity_series>", document.SingleParameters[5]);
                    buf.ReplaceText("<identity_number>", document.SingleParameters[6]);
                    buf.ReplaceText("<identity_date>", document.SingleParameters[7]);
                    buf.ReplaceText("<identity_organization>", document.SingleParameters[8]);
                    buf.ReplaceText("<identity_subdivision_code>", document.SingleParameters[9]);
                    buf.ReplaceText("<identity_birth_place>", document.SingleParameters[10]);
                    buf.ReplaceText("<identity_reg>", document.SingleParameters[11]);
                    buf.ReplaceText("<cell_phone>", document.SingleParameters[12]);
                    buf.ReplaceText("<home_phone>", document.SingleParameters[13]);
                    buf.ReplaceText("<email>", document.SingleParameters[14]);
                    buf.ReplaceText("<edu_organization>", document.SingleParameters[15]);
                    buf.ReplaceText("<edu_series>", document.SingleParameters[16]);
                    buf.ReplaceText("<edu_number>", document.SingleParameters[17]);
                    buf.ReplaceText("<edu_date>", document.SingleParameters[18]);
                    buf.ReplaceText("<edu_speciality>", document.SingleParameters[19]);
                    buf.ReplaceText("<need_hostel>", document.SingleParameters[20]=="True"? "нуждаюсь" : "не нуждаюсь");
                    buf.ReplaceText("<red_diplom_ball>", document.SingleParameters[21]!="0"?document.SingleParameters[21]:"");
                    buf.ReplaceText("<hight_edu_first>", document.SingleParameters[22] == "True" ? "впервые" : "повторно");
                    buf.ReplaceText("<special_conditions>", document.SingleParameters[23] == "True" ? "имею" : "не имею");
                    buf.ReplaceText("<FIO_SHORT>", document.SingleParameters[24]);

                    var applEntranceTableParams = buf.Tables.FirstOrDefault(t => t.TableCaption == "Directions");
                    if (applEntranceTableParams.RowCount > 1)
                    {
                        // Get the row pattern of the second row.
                        var rowPattern = applEntranceTableParams.Rows[1];

                        foreach (var direction in document.TableParameters[0])
                        {
                            AddItemToMagDirectionTable(applEntranceTableParams, rowPattern, direction);
                        }
                        // Remove the pattern row.
                        rowPattern.Remove();
                    }

                    if (doc == null)
                    {
                        doc = Xceed.Words.NET.DocX.Create(resultFile + ".docx");
                        doc.InsertDocument(buf);
                    }
                    else
                    {
                        doc.InsertSectionPageBreak();
                        doc.InsertDocument(buf);
                    }
                }
                else if (document.Template.IndexOf("ApplicationBachTemplate") >= 0)
                {
<<<<<<< HEAD
					Xceed.Words.NET.DocX buf;
                    if (document.Connection != null)
                        buf = Word.CreateFromTemplate(document.Connection, GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), document.ID.Value, resultFile);
=======
                    void InsertExam(Xceed.Words.NET.DocX local_buf, string subject, string year, string ball, string sn)
                    {
                        local_buf.ReplaceText("<" + subject + "_year>", year);
                        local_buf.ReplaceText("<" + subject + "_ball>", ball);
                        local_buf.ReplaceText("<" + subject + "_sn>", sn);
                    }

                    Xceed.Words.NET.DocX buf = Xceed.Words.NET.DocX.Load(document.Template);
                    buf.ReplaceText("<FIO>", document.SingleParameters[0]);
                    buf.ReplaceText("<gender>", document.SingleParameters[1]);
                    buf.ReplaceText("<birth_date>", document.SingleParameters[2]);
                    buf.ReplaceText("<nationality>", document.SingleParameters[3]);
                    buf.ReplaceText("<identity_type>", document.SingleParameters[4]);
                    buf.ReplaceText("<identity_series>", document.SingleParameters[5]);
                    buf.ReplaceText("<identity_number>", document.SingleParameters[6]);
                    buf.ReplaceText("<identity_date>", document.SingleParameters[7]);
                    buf.ReplaceText("<identity_organization>", document.SingleParameters[8]);
                    buf.ReplaceText("<identity_subdivision_code>", document.SingleParameters[9]);
                    buf.ReplaceText("<identity_birth_place>", document.SingleParameters[10]);
                    buf.ReplaceText("<identity_reg>", document.SingleParameters[11]);
                    buf.ReplaceText("<cell_phone>", document.SingleParameters[12]);
                    buf.ReplaceText("<home_phone>", document.SingleParameters[13]);
                    buf.ReplaceText("<email>", document.SingleParameters[14]);
                    buf.ReplaceText("<edu_organization>", document.SingleParameters[15]);
                    buf.ReplaceText("<edu_series>", document.SingleParameters[16]);
                    buf.ReplaceText("<edu_number>", document.SingleParameters[17]);
                    buf.ReplaceText("<foreign_language>", document.SingleParameters[22]);
                    buf.ReplaceText("<is_quota>", document.SingleParameters[23] == "True" ? "да" : "нет");
                    if (document.SingleParameters[23] == "True")
                    {
                        string quota_type = "";
                        if (document.SingleParameters[24] == "True") quota_type += "особая квота, ";
                        if (document.SingleParameters[25] == "True") quota_type += "целевая квота, ";
                        if (document.SingleParameters[26] == "True") quota_type += "без экзаменов, ";
                        buf.ReplaceText("<quota_type>", "(" + quota_type.Substring(0, quota_type.Length - 2) + ")"); 
                    }
>>>>>>> origin/master
                    else
                    {
                        buf.ReplaceText("<quota_type>", "");
                    }
                    buf.ReplaceText("<need_hostel>", document.SingleParameters[18] == "True" ? "нуждаюсь" : "не нуждаюсь");
                    if (document.TableParameters[1].Count() == 0)
                    {
                        InsertExam(buf, "m", "", "", "");
                        InsertExam(buf, "r", "", "", "");
                        InsertExam(buf, "p", "", "", "");
                        InsertExam(buf, "o", "", "", "");
                        InsertExam(buf, "fl", "", "", "");
                    }
                    foreach (var exam in document.TableParameters[1])                    
                        InsertExam(buf, exam[0], exam[1], exam[2], exam[3]);

                    buf.ReplaceText("<is_sport>", document.SingleParameters[27] == "True" ? "ИМЕЮ" : "НЕ ИМЕЮ");
                    buf.ReplaceText("<is_gm>", document.SingleParameters[28] == "True" ? "ИМЕЮ" : "НЕ ИМЕЮ");
                    buf.ReplaceText("<is_olymp_conf>", document.SingleParameters[29] == "True" ? "ИМЕЮ" : "НЕ ИМЕЮ");
                    buf.ReplaceText("<sport>", document.SingleParameters[30]);
                    buf.ReplaceText("<gm>", document.SingleParameters[31]);
                    buf.ReplaceText("<olymp_conf>", document.SingleParameters[32]);
                    buf.ReplaceText("<hight_edu_first>", document.SingleParameters[19] == "True" ? "впервые" : "повторно");
                    buf.ReplaceText("<special_conditions>", document.SingleParameters[20] == "True" ? "имею" : "не имею");
                    buf.ReplaceText("<FIO_SHORT>", document.SingleParameters[21]);
                    buf.ReplaceText("<is_gmX>", document.SingleParameters[28] == "True" ? "" : "X");
                    buf.ReplaceText("<is_MCADO>", document.SingleParameters[33] == "True" ? "" : "X");
                    buf.ReplaceText("<is_cher>", document.SingleParameters[34] == "True" ? "" : "X");
                    buf.ReplaceText("<is_exam>", document.SingleParameters[35] == "True" ? "" : "X");
                    buf.ReplaceText("<is_hostel>", document.SingleParameters[18] == "True" ? "" : "X");
                    buf.ReplaceText("<is_pk>", document.SingleParameters[37] == "True" ? "X" : "");
                    buf.ReplaceText("<ia_ball>", document.SingleParameters[36]);

                    var applEntranceTableParams = buf.Tables.FirstOrDefault(t => t.TableCaption == "Directions");
                    if (applEntranceTableParams.RowCount > 1)
                    {
                        // Get the row pattern of the second row.
                        var rowPattern = applEntranceTableParams.Rows[1];

                        foreach (var direction in document.TableParameters[0])
                        {
                            AddItemToBachDirectionTable(applEntranceTableParams, rowPattern, direction);
                        }
                        // Remove the pattern row.
                        rowPattern.Remove();
                    }

                    if (doc == null)
                    {
                        doc = Xceed.Words.NET.DocX.Create(resultFile + ".docx");
                        doc.InsertDocument(buf);
                    }
                    else
                    {
                        doc.InsertSectionPageBreak();
                        doc.InsertDocument(buf);
                    }
                }
                else
                {
                    XDocument template = XDocument.Load(document.Template, LoadOptions.PreserveWhitespace);

                    Validate(template);

                    if (template.Root.Element("Document").Element("Word") != null)
                    {
                        Xceed.Words.NET.DocX buf;
                        if (document.Connection != null)
                            buf = Word.CreateFromTemplate(document.Connection, GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), document.ID.Value, resultFile);
                        else
                            buf = Word.CreateFromTemplate(GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Word"), document.SingleParameters, document.TableParameters, resultFile);

                        if (doc == null)
                            doc = buf;
                        else
                        {
                            doc.InsertSectionPageBreak();
                            doc.InsertDocument(buf);
                        }
                    }
                    else
                        throw new System.ArgumentException("Эта перегрузка принимат только тип шаблонов \"Word\".", nameof(documents));
                }
                if (readOnly)
                    doc.AddProtection(Xceed.Words.NET.EditRestrictions.readOnly);
                doc.Save();
            }
        }

        public static void Create(DB_Connector connection, string templateFile, string resultFile, uint[] ids = null)
        {
            #region Contracts
            CheckConnectionAndFilenames(connection, templateFile, resultFile);
            #endregion

            XDocument template = XDocument.Load(templateFile, LoadOptions.PreserveWhitespace);

            Validate(template);

            if (template.Root.Element("Document").Element("Excel") != null)
                Excel.CreateFromTemplate(connection, GetFonts(template.Root.Element("Fonts")), template.Root.Element("Document").Element("Excel"), ids, resultFile);
            else
                throw new System.ArgumentException("Эта перегрузка принимат только тип шаблона \"Excel\".", nameof(templateFile));
        }

        private static void Validate(XDocument doc)
        {
            _SchemaSet.Add(null, SchemaPath);
            doc.Validate(_SchemaSet, (sender, e) => { throw e.Exception; });
        }

        private static Dictionary<string, Font> GetFonts(XElement fonts)
        {
            Dictionary<string, Font> result = new Dictionary<string, Font>();
            foreach (XElement font in fonts.Elements())
            {
                result.Add(font.Attribute("ID").Value,
                    new Font(
                        font.Element("Name")?.Value ?? null,
                        font.Element("Size") != null ? (ushort?)ushort.Parse(font.Element("Size").Value) : null,
                        font.Element("Style")?.Value ?? null,
                        font.Element("Color") != null ? (System.Drawing.Color?)_Colors[font.Element("Color").Value] : null
                        ));
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

        private static void CheckConnectionAndFilenames(DB_Connector connection, string templateFile, string resultFile)
        {
            if (connection == null)
                throw new System.ArgumentNullException(nameof(connection));
            CheckFilenames(templateFile, resultFile);
        }

        private static void CheckFilenames(string templateFile, string resultFile)
        {
            if (string.IsNullOrWhiteSpace(templateFile))
                throw new System.ArgumentException("Некорректное имя файла шаблона.", nameof(templateFile));
            if (string.IsNullOrWhiteSpace(resultFile))
                throw new System.ArgumentException("Некорректное имя выходного файла.", nameof(resultFile));
        }
    }
}
