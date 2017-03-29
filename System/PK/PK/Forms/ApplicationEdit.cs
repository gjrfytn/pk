using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class ApplicationEdit : Form
    {
        public struct QDoc
        {
            public string cause;

            public string medCause;
            public int medDocSerie;
            public int medDocNumber;
            public string disabilityGroup;
            public int conclusionNumber;
            public DateTime conclusionDate;

            public string orphanhoodDocType;
            public string orphanhoodDocName;
            public DateTime orphanhoodDocDate;
            public string orphanhoodDocOrg;
        }

        public struct ODoc
        {
            public string olympType;
            public string olympName;
            public int olympDocNumber;
            public string diplomaType;
            public int olympID;
            public string olympProfile;
            public int olympClass;
            public string olympDist;
            public string country;
        }

        public struct SDoc
        {
            public string diplomaType;
            public string docName;
            public DateTime docDate;
            public string orgName;
        }

        public QDoc QouteDoc;
        public ODoc OlympicDoc;
        public SDoc SportDoc;
        
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private Classes.KLADR _KLADR;

        private uint _ApplicationID;
        private int _CurrCampainID;
        private string _RegistratorsLogin;
        private DateTime _EditingDateTime;

        public ApplicationEdit(int campaignID, string registratorsLogin)
        {
            InitializeComponent();

            if (_DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS).Count == 0)
            {
                MessageBox.Show("Справочник направлний ФИС пуст. Чтобы загрузить его, выберите:\nГлавное Меню -> Справка -> Справочник направлений ФИС -> Обновить");
                DialogResult = DialogResult.Abort;
            }
            else if (_DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS).Count == 0)
            {
                MessageBox.Show("Справочники пусты. Чтобы загрузить их, выберите:\nГлавное Меню -> Справка -> Справочники ФИС -> Обновить");
                DialogResult = DialogResult.Abort;
            }

            _DB_Connection = new Classes.DB_Connector();
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _KLADR = new Classes.KLADR();
            _CurrCampainID = campaignID;
            _RegistratorsLogin = registratorsLogin;

            FillComboBox(cbIDDocType, 22);
            FillComboBox(cbSex, 5);
            FillComboBox(cbNationality, 7);
            FillComboBox(cbExamsDocType, 22);
            cbFirstTime.SelectedIndex = 0;
            cbForeignLanguage.SelectedIndex = 0;
            rbCertificate.Checked = true;
            tbRegion.AutoCompleteCustomSource.AddRange(_KLADR.GetRegions().ToArray());

            FillDirectionsProfilesCombobox(cbDirection21, true, "Очная", "");

            for (int i = DateTime.Now.Year; i >= 1950; i--)
                cbGraduationYear.Items.Add(i);
            cbGraduationYear.SelectedIndex = 0;

            List<string> years = new List<string>
            {
                "-",
                DateTime.Now.Year.ToString(),
                (DateTime.Now.Year - 1).ToString(),
                (DateTime.Now.Year - 2).ToString(),
                (DateTime.Now.Year - 3).ToString(),
                (DateTime.Now.Year - 4).ToString(),
                (DateTime.Now.Year - 5).ToString(),
            };

            dgvExams.Rows.Add("Математика", null, "", "0", 32);
            dgvExams.Rows.Add("Русский язык", null, "", "0", 32);
            dgvExams.Rows.Add("Физика", null, "", "0", 32);
            dgvExams.Rows.Add("Обществознание", null, "", "0", 32);
            dgvExams.Rows.Add("Иностранный язык", null, "", "0", 32);

            for (int j = 0; j < dgvExams.Rows.Count; j++)
            {
                ((DataGridViewComboBoxCell)dgvExams.Rows[j].Cells[1]).DataSource = years;
                dgvExams.Rows[j].Cells[1].Value = DateTime.Now.Year.ToString();
            }

            foreach (TabPage tab in tbDirections.Controls)
            {
                string eduFormName = "";
                switch (tab.Name.Split('_')[2])
                {
                    case "o":
                        eduFormName = "Очная форма";
                        break;
                    case "oz":
                        eduFormName = "Очно-заочная (вечерняя)";
                        break;
                    case "z":
                        eduFormName = "Заочная форма";
                        break;
                }
                if (tab.Name.Split('_')[1] == "paid")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, true, eduFormName, "");
                    }
                else if (tab.Name.Split('_')[1] == "budget")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, "Бюджетные места");
                    }
                else if (tab.Name.Split('_')[1] == "target")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, "Целевой прием");
                    }
                else if (tab.Name.Split('_')[1] == "quote")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, "Квота приема лиц, имеющих особое право");
                    }
            }
            cbInstitutionType.SelectedIndex = 0;
        }

        private void FillComboBox(ComboBox cb, int dictionaryNumber)
        {
            foreach (object[] v in _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                new List<Tuple<string, Relation, object>>
                {
                new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, dictionaryNumber)
                }))
                cb.Items.Add(v[0]);
            if (cb.Items.Count > 0)
                cb.SelectedIndex = 0;
        }

        private void FillDirectionsProfilesCombobox(ComboBox combobox, bool isProfileList, string eduForm, string eduSource)
        {
            if (isProfileList)
                foreach (var record in _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_faculty",  "profiles_name",
                "places_paid_o", "places_paid_oz", "places_paid_z", "profiles_direction_id" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CurrCampainID)
                }))
                {
                    if ((eduForm == "Очная форма") && (int.Parse(record[2].ToString()) > 0) || (eduForm == "Очно-заочная (вечерняя)") && (int.Parse(record[3].ToString()) > 0)
                        || (eduForm == "Заочная форма") && (int.Parse(record[4].ToString()) > 0))
                    {
                        string eduLevel = "";
                        string directionCode = _DB_Helper.GetDirectionCodeByID(int.Parse(record[5].ToString()));
                        if (directionCode.ToString().Split('.')[1] == "03")
                            eduLevel = "Бакалавр";
                        else if (directionCode.ToString().Split('.')[1] == "04")
                            eduLevel = "Магистр";
                        else if (directionCode.ToString().Split('.')[1] == "05")
                            eduLevel = "Специалист";
                        combobox.Items.Add("(" + record[0] + ", " + eduLevel + ") " + record[1]);
                    }
                }
            else
            {
                string placesCountColumnName = "";
                if ((eduForm == "Очная форма") && (eduSource == "Бюджетные места"))
                    placesCountColumnName = "places_budget_o";
                else if ((eduForm == "Очная форма") && (eduSource == "Целевой прием"))
                    placesCountColumnName = "places_target_o";
                else if ((eduForm == "Очная форма") && (eduSource == "Квота приема лиц, имеющих особое право"))
                    placesCountColumnName = "places_quota_o";
                else if ((eduForm == "Очно-заочная (вечерняя)") && (eduSource == "Бюджетные места"))
                    placesCountColumnName = "places_budget_oz";
                else if ((eduForm == "Очно-заочная (вечерняя)") && (eduSource == "Целевой прием"))
                    placesCountColumnName = "places_target_oz";
                else if ((eduForm == "Очно-заочная (вечерняя)") && (eduSource == "Квота приема лиц, имеющих особое право"))
                    placesCountColumnName = "places_quota_oz";
                else if ((eduForm == "Заочная форма") && (eduSource == "Бюджетные места"))
                    placesCountColumnName = "places_budget_z";
                else if ((eduForm == "Заочная форма") && (eduSource == "Целевой прием"))
                    placesCountColumnName = "places_target_z";
                else if ((eduForm == "Заочная форма") && (eduSource == "Квота приема лиц, имеющих особое право"))
                    placesCountColumnName = "places_quota_z";

                foreach (var record in _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[] { "direction_faculty", "direction_id", placesCountColumnName }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                        }))
                {
                    if (int.Parse(record[2].ToString()) > 0)
                    {
                        string eduLevel = "";
                        string directionCode = _DB_Helper.GetDirectionCodeByID(int.Parse(record[1].ToString()));
                        if (directionCode.ToString().Split('.')[1] == "03")
                            eduLevel = "Бакалавр";
                        else if (directionCode.ToString().Split('.')[1] == "04")
                            eduLevel = "Магистр";
                        else if (directionCode.ToString().Split('.')[1] == "05")
                            eduLevel = "Специалист";
                        combobox.Items.Add(" (" + record[0] + ", " + eduLevel + ") " + _DB_Helper.GetDirectionNameByID(int.Parse(record[1].ToString())));
                    }
                }
            }
        }

        private void btAddDir1_Click(object sender, EventArgs e)
        {
            //if (!cbDirection11.Visible)
            //{
            //    cbDirection11.Visible = true;
            //    cbDirection11.Enabled = true;
            //    btRemoveDir11.Visible = true;
            //    btRemoveDir11.Enabled = true;
            //}
            //else if (!cbDirection12.Visible)
            //{
            //    cbDirection12.Visible = true;
            //    cbDirection12.Enabled = true;
            //    btRemoveDir12.Visible = true;
            //    btRemoveDir12.Enabled = true;
            //}
            //else if (!cbDirection13.Visible)
            //{
            //    cbDirection13.Visible = true;
            //    cbDirection13.Enabled = true;
            //    btRemoveDir13.Visible = true;
            //    btRemoveDir13.Enabled = true;
            //}

            //char parentNumber = this.Parent.Name.ToString()[this.Parent.Name.Length-1];
            //if (!(this.Parent.Controls.Find(("cbDirection" + parentNumber + "1"),false)[0] as Control).Visible)
            //{
            //    (this.Parent.Controls.Find(("cbDirection" + parentNumber + "1"), false)[0] as Control).Visible = true;
            //    (this.Parent.Controls.Find(("cbDirection" + parentNumber + "1"), false)[0] as Control).Enabled = true;
            //    (this.Parent.Controls.Find(("btRemoveDir" + parentNumber + "1"), false)[0] as Control).Visible = true;
            //    (this.Parent.Controls.Find(("btRemoveDir" + parentNumber + "1"), false)[0] as Control).Enabled = true;
            //}
            //else if (!(this.Parent.Controls.Find(("cbDirection" + parentNumber + "2"), false)[0] as Control).Visible)
            //{
            //    (this.Parent.Controls.Find(("cbDirection" + parentNumber + "2"), false)[0] as Control).Visible = true;
            //    (this.Parent.Controls.Find(("cbDirection" + parentNumber + "2"), false)[0] as Control).Enabled = true;
            //    (this.Parent.Controls.Find(("btRemoveDir" + parentNumber + "2"), false)[0] as Control).Visible = true;
            //    (this.Parent.Controls.Find(("btRemoveDir" + parentNumber + "2"), false)[0] as Control).Enabled = true;
            //}
            //else if (!(this.Parent.Controls.Find(("cbDirection" + parentNumber + "3"), false)[0] as Control).Visible)
            //{
            //    (this.Parent.Controls.Find(("cbDirection" + parentNumber + "3"), false)[0] as Control).Visible = true;
            //    (this.Parent.Controls.Find(("cbDirection" + parentNumber + "3"), false)[0] as Control).Enabled = true;
            //    (this.Parent.Controls.Find(("btRemoveDir" + parentNumber + "3"), false)[0] as Control).Visible = true;
            //    (this.Parent.Controls.Find(("btRemoveDir" + parentNumber + "3"), false)[0] as Control).Enabled = true;
            //}
        }

        private void cbSpecial_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSpecial.Checked)
            {
                QuotDocsForm form = new QuotDocsForm(this);
                form.ShowDialog();
                cbMedCertificate.Enabled = true;
            }
            else
            {
                cbMedCertificate.Enabled = false;
                cbMedCertificate.Checked = false;
            }
            DirectionDocEnableDisable();
        }

        private void cbSport_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSport.Checked)
            {
                SportDocsForm form = new SportDocsForm(this);
                form.ShowDialog();
            }
        }

        private void cbMADIOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMADIOlympiad.Checked)
            {
                MADIOlimps form = new MADIOlimps(this);
                form.ShowDialog();
            }
        }

        private void cbOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            DirectionDocEnableDisable();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            uint entrantId = _DB_Connection.Insert(DB_Table.ENTRANTS, new Dictionary<string, object> { { "last_name", tbLastName.Text},
                { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},{ "gender_dict_id", 5},
                { "gender_id", _DB_Helper.GetDictionaryItemID( 5, cbSex.SelectedItem.ToString())},
                { "email", mtbEMail.Text},{ "is_from_krym", null}, { "home_phone", mtbHomePhone.Text}, { "mobile_phone", mtbMobilePhone.Text}});

            Random randNumber = new Random();
            _ApplicationID = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "number", randNumber.Next()},
                { "entrant_id", entrantId}, { "registration_time", DateTime.Now}, { "needs_hostel", cbHostleNeeded.Checked}, { "registrator_login", _RegistratorsLogin},
            { "status_dict_id", 4},{ "status_id", _DB_Helper.GetDictionaryItemID( 4, "Новое")}, { "language", cbForeignLanguage.SelectedItem.ToString()} });

            uint idDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "identity" },
                { "series", tbIDDocSeries.Text}, { "number", tbIDDocNumber.Text}, { "date", dtpIDDocDate.Value} , { "organization", tbIssuedBy.Text} });

            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID},
                { "documents_id", idDocUid } });

            _DB_Connection.Insert(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", idDocUid},
                { "last_name", tbLastName.Text}, { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},
                { "gender_dict_id", 5},{ "gender_id", _DB_Helper.GetDictionaryItemID(5,cbSex.SelectedItem.ToString())},
                { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", 22},
                { "type_id", _DB_Helper.GetDictionaryItemID(22,cbIDDocType.SelectedItem.ToString())},
                { "nationality_dict_id", 7}, { "nationality_id", _DB_Helper.GetDictionaryItemID(7,cbNationality.SelectedItem.ToString())},
                { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}});

            string eduDocType = "";

            if (rbSpravka.Checked)
                eduDocType = "academic_diploma";
            else if ((cbInstitutionType.SelectedItem.ToString() == "Средняя школа") || (cbInstitutionType.SelectedItem.ToString() == "Лицей")
            || (cbInstitutionType.SelectedItem.ToString() == "Гимназия"))
                eduDocType = "school_certificate";
            else if ((cbInstitutionType.SelectedItem.ToString() == "УВК") || (cbInstitutionType.SelectedItem.ToString() == "ПТУ")
                || (cbInstitutionType.SelectedItem.ToString() == "СПТУ") || (cbInstitutionType.SelectedItem.ToString() == "ПУ")
                || (cbInstitutionType.SelectedItem.ToString() == "Техникум") || (cbInstitutionType.SelectedItem.ToString() == "Колледж")
                )
                eduDocType = "middle_edu_diploma";
            else if ((cbInstitutionType.SelectedItem.ToString() == "Университет") || (cbInstitutionType.SelectedItem.ToString() == "Академия")
                || (cbInstitutionType.SelectedItem.ToString() == "Институт"))
                eduDocType = "high_edu_diploma";

            if (cbOriginal.Checked)
            {
                int eduDocID = (int)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", eduDocType }, { "series",  tbEduDocSeries.Text},
                    { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}, { "original_recieved_date", DateTime.Now}}));
                _DB_Connection.Insert(DB_Table.DIPLOMA_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", eduDocID }, { "end_year", cbGraduationYear.SelectedItem } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", eduDocID } });
            }
            else
            {
                int eduDocID = (int)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", eduDocType }, { "series",  tbEduDocSeries.Text},
                    { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}  }));
                _DB_Connection.Insert(DB_Table.DIPLOMA_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", eduDocID }, { "end_year", cbGraduationYear.SelectedItem } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", eduDocID } });
            }

            uint examsDocId = 0;
            if (cbPassportMatch.Checked)
            {
                examsDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "ege" }, { "series",  tbIDDocSeries.Text},
                    { "number", tbIDDocNumber.Text} });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", examsDocId } });
            }

            else
            {
                examsDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "identity" },
                { "series", tbEduDocSeries.Text}, { "number", tbExamsDocNumber.Text} });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", examsDocId } });
            }


            foreach (DataGridViewRow row in dgvExams.Rows)
            {
                if (row.Cells[3].Value.ToString() != "0")
                    _DB_Connection.Insert(DB_Table.DOCUMENTS_SUBJECTS_DATA, new Dictionary<string, object> { { "document_id", examsDocId},
                        { "subject_dict_id", 1} , { "subject_id", _DB_Helper.GetDictionaryItemID( 1, row.Cells[0].Value.ToString())} ,
                        { "value", row.Cells[3].Value} });
            }

            if (cbSpecial.Checked)
                if (QouteDoc.cause == "Сиротство")
                {
                    uint orphDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "orphan" },
                    { "date", QouteDoc.orphanhoodDocDate} , { "organization", QouteDoc.orphanhoodDocOrg} });
                    _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", orphDocUid},
                        { "name", QouteDoc.orphanhoodDocName}, { "dictionaries_dictionary_id", 42},
                        { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( 42, QouteDoc.orphanhoodDocType)} });
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", orphDocUid } });
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", 31},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(31,"Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                            { "reason_document_id", orphDocUid},{ "benefit_kind_dict_id", 30},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } });
                }
                else if (QouteDoc.cause == "Медицинские показатели")
                {
                    uint allowEducationDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                    { { "number", QouteDoc.conclusionNumber}, { "date", QouteDoc.conclusionDate} });
                    if (QouteDoc.medCause == "Справква об установлении инвалидности")
                    {
                        uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "disability" },
                        { "series", QouteDoc.medDocSerie},  { "number", QouteDoc.medDocNumber} });
                        _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {
                            { "document_id", medDocUid}, { "dictionaries_dictionary_id",23},
                            { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(23,QouteDoc.disabilityGroup)} });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", allowEducationDocUid } });
                        _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", 31},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(31,"Справка об установлении инвалидности")},
                            { "reason_document_id", medDocUid},{ "allow_education_document_id", allowEducationDocUid}, { "benefit_kind_dict_id", 30},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } });
                    }
                    else if (QouteDoc.medCause == "Заключение психолого-медико-педагогической комиссии")
                    {
                        uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "medical" },
                        { "series", QouteDoc.medDocSerie},  { "number", QouteDoc.medDocNumber} });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", medDocUid } });
                        _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", 31},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID( 31, "Заключение психолого-медико-педагогической комиссии")},
                            { "reason_document_id", medDocUid},{ "allow_education_document_id", allowEducationDocUid}, { "benefit_kind_dict_id", 30},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } });
                    }
                }

            if (cbSport.Checked)
            {
                uint sportDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                { { "type", "sport" }, { "date", SportDoc.docDate}, { "organization", SportDoc.orgName} });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                { { "document_id", sportDocUid}, { "name", SportDoc.docName} });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", sportDocUid } });

                uint achevmentCategoryId = 0;
                switch (SportDoc.diplomaType)
                {
                    case "Диплом чемпиона/призера Олимпийских игр":
                        achevmentCategoryId = _DB_Helper.GetDictionaryItemID(36, "Статус чемпиона и призера Олимпийских игр");
                        break;
                    case "Диплом чемпиона/призера Паралимпийских игр":
                        achevmentCategoryId = _DB_Helper.GetDictionaryItemID(36, "Статус чемпиона и призера Паралимпийских игр");
                        break;
                    case "Диплом чемпиона/призера Сурдлимпийских игр":
                        achevmentCategoryId = _DB_Helper.GetDictionaryItemID(36, "Статус чемпиона и призера Сурдлимпийских игр");
                        break;
                    case "Диплом чемпиона мира":
                        achevmentCategoryId = _DB_Helper.GetDictionaryItemID(36, "Чемпион Мира");
                        break;
                    case "Диплом чемпиона Европы":
                        achevmentCategoryId = _DB_Helper.GetDictionaryItemID(36, "Чемпион Европы");
                        break;
                }

                List<object[]> achievments = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                    new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("category_dict_id", Relation.EQUAL, 36),
                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, achevmentCategoryId)
                });

                uint achievementUid;
                if (achievments.Count != 0)
                    achievementUid = uint.Parse(achievments[0][0].ToString());
                else
                {
                    achievementUid = _DB_Connection.Insert(DB_Table.INSTITUTION_ACHIEVEMENTS, new Dictionary<string, object>
                    { { "name", _DB_Helper.GetDictionaryItemName( 36, achevmentCategoryId) }, { "category_dict_id", 36}, { "category_id", achevmentCategoryId},
                        { "max_value", 1}, { "campaign_id", 1} });
                }

                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                    { "institution_achievement_id", achievementUid}, { "mark", _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "max_value" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("id", Relation.EQUAL, achievementUid)
                    })[0][0]}, { "document_id", sportDocUid} });
            }

            if (cbMADIOlympiad.Checked)
            {
                string docType = "";
                uint benefitDocType = 0;
                uint olympicDocId = 0;
                switch (OlympicDoc.olympType)
                {
                    case "Диплом победителя/призера олимпиады школьников":
                        docType = "olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(31, "Диплом победителя/призера олимпиады школьников");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "diploma_type_dict_id", 18 },
                    { "diploma_type_id", _DB_Helper.GetDictionaryItemID(18, OlympicDoc.diplomaType) },
                    { "olympic_id", (int)((uint)_DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_id" },
                        new System.Collections.Generic.List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("olympic_number", Relation.EQUAL, OlympicDoc.olympID)
                        })[0][0])},
                    { "class_number", OlympicDoc.olympClass }, { "olympic_profile", OlympicDoc.olympProfile },
                    { "profile_dict_id", 39 }, { "profile_id", _DB_Helper.GetDictionaryItemID(39, OlympicDoc.olympProfile) },
                    { "olympic_subject_dict_id", 1 }, { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(1, OlympicDoc.olympDist) } });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;

                    case "Диплом победителя/призера всероссийской олимпиады школьников":
                        docType = "olympic_total";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(31, "Диплом победителя/призера всероссийской олимпиады школьников");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "diploma_type_dict_id", 18 },
                    { "diploma_type_id", _DB_Helper.GetDictionaryItemID(18, OlympicDoc.diplomaType) },
                    { "olympic_id", (int)((uint)_DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_id" },
                        new System.Collections.Generic.List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("olympic_number", Relation.EQUAL, OlympicDoc.olympID)
                        })[0][0])},
                    { "class_number", OlympicDoc.olympClass }, { "olympic_profile", OlympicDoc.olympProfile },
                    { "profile_dict_id", 39 }, { "profile_id", _DB_Helper.GetDictionaryItemID(39, OlympicDoc.olympProfile) },
                    { "olympic_subject_dict_id", 1 }, { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(1, OlympicDoc.olympDist) } });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;

                    case "Диплом 4 этапа всеукраинской олимпиады":
                        docType = "ukraine_olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(31, "Диплом победителя/призера IV  всеукраинской ученической олимпиады");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "diploma_type_dict_id", 18 },
                    { "diploma_type_id", _DB_Helper.GetDictionaryItemID(18, OlympicDoc.diplomaType) }, { "olympic_name", OlympicDoc.olympName }, { "olympic_profile", OlympicDoc.olympProfile },
                    { "profile_dict_id", 39 }, { "profile_id", _DB_Helper.GetDictionaryItemID(39, OlympicDoc.olympProfile) }});
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;

                    case "Диплом международной олимпиады":
                        docType = "international_olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(31, "Документ об участии в международной олимпиаде");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "olympic_name", OlympicDoc.olympName },
                            { "olympic_profile", OlympicDoc.olympProfile },
                    { "country_dict_id", 7 }, { "country_id", _DB_Helper.GetDictionaryItemID(7, OlympicDoc.country) },
                    { "profile_dict_id", 39 }, { "profile_id", _DB_Helper.GetDictionaryItemID(39, OlympicDoc.olympProfile) } });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;
                }
            }

            foreach (TabPage tab in tbDirections.Controls)
            {
                uint eduForm = 0;
                uint eduSource = 0;

                switch (tab.Name.Split('_')[2])
                {
                    case "o":
                        eduForm = _DB_Helper.GetDictionaryItemID(14, "Очная форма");
                        break;
                    case "oz":
                        eduForm = _DB_Helper.GetDictionaryItemID(14, "Очно-заочная (вечерняя)");
                        break;
                    case "z":
                        eduForm = _DB_Helper.GetDictionaryItemID(14, "Заочная форма");
                        break;
                }

                if (tab.Name.Split('_')[1] == "budget")
                    eduSource = _DB_Helper.GetDictionaryItemID(15, "Бюджетные места");
                else if (tab.Name.Split('_')[1] == "paid")
                    eduSource = _DB_Helper.GetDictionaryItemID(15, "С оплатой обучения");

                else if (tab.Name.Split('_')[1] == "quote")
                    eduSource = _DB_Helper.GetDictionaryItemID(15, "Квота приема лиц, имеющих особое право");
                else if (tab.Name.Split('_')[1] == "target")
                    eduSource = _DB_Helper.GetDictionaryItemID(15, "Целевой прием");

                if (tab.Name.Split('_')[1] != "paid")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                string facultyShortName = (cb.SelectedItem.ToString().Split(')')[0]).Split(',')[0];
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                    { "faculty_short_name", facultyShortName.Split('(')[1] },
                                    { "direction_id",_DB_Helper.GetDirectionIDByName(cb.SelectedItem.ToString().Split(')')[1].Remove(0, 1))},
                                    { "edu_form_dict_id", 14}, { "edu_form_id", eduForm}, { "edu_source_dict_id", 15}, { "edu_source_id", eduSource}, { "is_for_spo_and_vo", false} });
                            }
                    }
                else
                {
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                uint dirID = (uint)_DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_id" }, new List<Tuple<string, Relation, object>>
                                    {
                                        new Tuple<string, Relation, object>("profiles_name", Relation.EQUAL, cb.SelectedItem.ToString().Split(')')[1].Remove(0, 1)),
                                        new Tuple<string, Relation, object> ("profiles_direction_faculty", Relation.EQUAL, ((cb.SelectedItem.ToString().Split(')')[0]).Split(',')[0]).Split('(')[1])
                                    })[0][0];
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                        { "faculty_short_name",  cb.SelectedItem.ToString().Split(')')[0].Split(',')[0].Remove(0,1) },
                                        { "direction_id", dirID }, { "edu_form_dict_id", 14}, { "edu_form_id", eduForm}, { "edu_source_dict_id", 15}, { "edu_source_id", eduSource},
                                    { "is_for_spo_and_vo", false}, { "profile_name", cb.SelectedItem.ToString().Split(')')[1].Remove(0, 1)} });
                            }
                    }
                }
            }

            if (cbAppEnrollment.Checked)
            {
                uint otherDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "institution" } });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", otherDocID }, { "name", "Заявление о согласии на зачисление" } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", otherDocID } });
            }
            if (cbDirectionDoc.Checked)
            {
                uint otherDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "institution" } });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", otherDocID }, { "name", "Направление ПК" } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", otherDocID } });
            }
            if (cbMedCertificate.Checked)
            {
                uint otherDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "institution" } });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", otherDocID }, { "name", "Медицинская справка" } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", otherDocID } });
            }
            if (cbCertificateHRD.Checked)
            {
                uint otherDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "institution" } });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", otherDocID }, { "name", "Справка отдела кадров" } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", otherDocID } });
            }
            if (cbPhotos.Checked)
            {
                uint otherDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "institution" } });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", otherDocID }, { "name", "4 фотографии 3x4" } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", otherDocID } });
            }
        }

        private void cbPassportMatch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPassportMatch.Checked)
            {
                tbExamsDocSeries.Enabled = false;
                tbExamsDocNumber.Enabled = false;
                cbExamsDocType.Enabled = false;
                label33.Enabled = false;
                label34.Enabled = false;
                label35.Enabled = false;
            }
            else
            {
                tbExamsDocSeries.Enabled = true;
                tbExamsDocNumber.Enabled = true;
                cbExamsDocType.Enabled = true;
                label33.Enabled = true;
                label34.Enabled = true;
                label35.Enabled = true;
            }
        }

        private void EduDocTypeChanged()
        {
            if (rbCertificate.Checked)
                cbCertificateCopy.Enabled = true;
            else
            {
                cbCertificateCopy.Enabled = false;
                cbCertificateCopy.Checked = false;
            }
            if (rbDiploma.Checked)
                cbDiplomaCopy.Enabled = true;
            else
            {
                cbDiplomaCopy.Enabled = false;
                cbDiplomaCopy.Checked = false;
            }
            if (rbSpravka.Checked)
                cbCertificateHRD.Enabled = true;
            else
            {
                cbCertificateHRD.Enabled = false;
                cbCertificateHRD.Checked = false;
            }
        }

        private void DirectionDocEnableDisable()
        {
            if ((cbChernobyl.Checked) || (cbSpecial.Checked) || (cbOlympiad.Checked) || (cbPrerogative.Checked))
                cbDirectionDoc.Enabled = true;
            else
            {
                cbDirectionDoc.Enabled = false;
                cbDirectionDoc.Checked = false;
            }
        }

        private void rbCertificate_CheckedChanged(object sender, EventArgs e)
        {
            EduDocTypeChanged();
        }

        private void rbDiploma_CheckedChanged(object sender, EventArgs e)
        {
            EduDocTypeChanged();
        }

        private void rbSpravka_CheckedChanged(object sender, EventArgs e)
        {
            EduDocTypeChanged();
        }

        private void cbChernobyl_CheckedChanged(object sender, EventArgs e)
        {
            DirectionDocEnableDisable();
        }

        private void cbPrerogative_CheckedChanged(object sender, EventArgs e)
        {
            DirectionDocEnableDisable();
        }

        private void tbDistrict_Enter(object sender, EventArgs e)
        {
            tbDistrict.AutoCompleteCustomSource.Clear();
            tbDistrict.AutoCompleteCustomSource.AddRange(_KLADR.GetDistricts(tbRegion.Text).ToArray());

            if (tbDistrict.AutoCompleteCustomSource.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", tbDistrict, 3000);
            }
        }

        private void tbCity_Enter(object sender, EventArgs e)
        {
            tbCity.AutoCompleteCustomSource.Clear();
            tbCity.AutoCompleteCustomSource.AddRange(_KLADR.GetTowns(tbRegion.Text, tbDistrict.Text).ToArray());
            tbCity.AutoCompleteCustomSource.AddRange(_KLADR.GetSettlements(tbRegion.Text, tbDistrict.Text, "").ToArray());

            if (tbCity.AutoCompleteCustomSource.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", tbCity, 3000);
            }
        }

        private void tbStreet_Enter(object sender, EventArgs e)
        {
            tbStreet.AutoCompleteCustomSource.Clear();
            tbStreet.AutoCompleteCustomSource.AddRange(_KLADR.GetStreets(tbRegion.Text, tbDistrict.Text, tbCity.Text, "").ToArray());
            tbStreet.AutoCompleteCustomSource.AddRange(_KLADR.GetStreets(tbRegion.Text, tbDistrict.Text, "", tbCity.Text).ToArray());

            if (tbStreet.AutoCompleteCustomSource.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", tbStreet, 3000);
            }
        }

        private void tbHouse_Enter(object sender, EventArgs e)
        {
            tbHouse.AutoCompleteCustomSource.Clear();
            tbHouse.AutoCompleteCustomSource.AddRange(_KLADR.GetHouses(tbRegion.Text, tbDistrict.Text, tbCity.Text, "", tbStreet.Text).ToArray());
            tbHouse.AutoCompleteCustomSource.AddRange(_KLADR.GetHouses(tbRegion.Text, tbDistrict.Text, "", tbCity.Text, tbStreet.Text).ToArray());

            if (tbHouse.AutoCompleteCustomSource.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", tbStreet, 3000);
            }
        }
    }
}
