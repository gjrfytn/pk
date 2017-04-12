using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class ApplicationEdit : Form
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

        public QDoc QuoteDoc;
        public ODoc OlympicDoc;
        public SDoc SportDoc;

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly Classes.KLADR _KLADR=new Classes.KLADR();

        private uint? _ApplicationID;
        private uint? _EntrantID;
        private uint _CurrCampainID;
        private string _RegistratorLogin;
        private DateTime _EditingDateTime;
        private bool _Loading;
        private uint? _TargetOrganizationID;
        private Dictionary<string, string> _Towns = new Dictionary<string, string>();

        public ApplicationEdit(Classes.DB_Connector connection,uint campaignID, string registratorsLogin, uint? applicationId)
        {
            #region Components
            InitializeComponent();

            dgvExams_EGE.ValueType = typeof(byte);
            #endregion

            _DB_Connection = connection;

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

            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _CurrCampainID = campaignID;
            _RegistratorLogin = registratorsLogin;
            _ApplicationID = applicationId;

            FillComboBox(cbIDDocType, 22);
            FillComboBox(cbSex, 5);
            FillComboBox(cbNationality, 7);
            FillComboBox(cbExamsDocType, 22);
            cbFirstTime.SelectedIndex = 0;
            cbForeignLanguage.SelectedIndex = 0;
            rbCertificate.Checked = false;
            rbCertificate.Checked = true;
            tbRegion.AutoCompleteCustomSource.AddRange(_KLADR.GetRegions().ToArray());

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

            dgvExams.Rows.Add("Математика", null, null, (byte)0, 32);
            dgvExams.Rows.Add("Русский язык", null, null, (byte)0, 32);
            dgvExams.Rows.Add("Физика", null, null, (byte)0, 32);
            dgvExams.Rows.Add("Обществознание", null, null, (byte)0, 32);
            dgvExams.Rows.Add("Иностранный язык", null, null, (byte)0, 32);

            for (int j = 0; j < dgvExams.Rows.Count; j++)
            {
                ((DataGridViewComboBoxCell)dgvExams.Rows[j].Cells[1]).DataSource = years;
                dgvExams.Rows[j].Cells[1].Value = DateTime.Now.Year.ToString();
            }

            //List<object[]>

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
                            FillDirectionsProfilesCombobox(cb, true, false, eduFormName, "");
                    }
                else if (tab.Name.Split('_')[1] == "budget")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, false, eduFormName, "Бюджетные места");
                    }
                else if (tab.Name.Split('_')[1] == "target")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false,true, eduFormName, "Целевой прием");
                    }
                else if (tab.Name.Split('_')[1] == "quote")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, false, eduFormName, "Квота приема лиц, имеющих особое право");
                    }
            }
            cbInstitutionType.SelectedIndex = 0;

            if (_ApplicationID != null)
            {
                _Loading = true;
                LoadApplication();
                _Loading = false;
                btPrint.Enabled = true;
            }
                
        }

        private void SaveApplication()
        {
            SaveBasic();
            SaveDiploma();
            SaveExams();
            if (cbQuote.Checked)
                SaveQuote();
            if (cbSport.Checked)
                SaveSport();
            if (cbMADIOlympiad.Checked || cbOlympiad.Checked)
                SaveOlympic();
            SaveDirections();
        }

        private void LoadApplication()
        {
            object[] application = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "needs_hostel", "language", "first_high_edu",
                "mcado", "chernobyl", "passing_examinations", "priority_right", "special_conditions", "entrant_id" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, _ApplicationID)
                })[0];

            _EntrantID = (uint)application[8];
            cbHostelNeeded.Checked = (bool)application[0];
            cbForeignLanguage.SelectedItem = application[1];
            if ((bool)application[2])
                cbFirstTime.SelectedItem = "Впервые";
            else cbFirstTime.SelectedItem = "Повторно";
            cbMCDAO.Checked = (bool)application[3];
            cbChernobyl.Checked = (bool)application[4];
            cbExams.Checked = (bool)application[5];
            cbPrerogative.Checked = (bool)application[6];
            cbSpecialConditions.Checked = (bool)application[7];
            cbPassportCopy.Checked = true;
            cbAppAdmission.Checked = true;

            object[] entrant = _DB_Connection.Select(DB_Table.ENTRANTS, new string[] { "last_name", "first_name","middle_name","gender_id","email",
                "home_phone","mobile_phone"}, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, application[8])
                })[0];
            tbLastName.Text = entrant[0].ToString();
            tbFirstName.Text = entrant[1].ToString();
            tbMidleName.Text = entrant[2].ToString();
            cbSex.SelectedItem = _DB_Helper.GetDictionaryItemName(5, (uint)entrant[3]);
            mtbEMail.Text = entrant[4].ToString();
            mtbHomePhone.Text = entrant[5].ToString();
            mtbMobilePhone.Text = entrant[6].ToString();

            if (_DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(36, Classes.DB_Helper.MedalAchievement)),
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                })[0][0])
            }).Count > 0)
                cbMedal.Checked = true;

            List<object[]> appDocuments = new List<object[]>();
            foreach (var documentID in _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, _ApplicationID)
            }))

                appDocuments.Add(_DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "id", "type", "series", "number", "date", "organization", "original_recieved_date" },
                    new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, documentID[0])
                })[0]);
            
            foreach (var document in appDocuments)
            {
                if (document[1].ToString() == "identity")
                {
                    tbIDDocSeries.Text = document[2].ToString();
                    tbIDDocNumber.Text = document[3].ToString();
                    dtpIDDocDate.Value = (DateTime)document[4];
                    tbIssuedBy.Text = document[5].ToString();

                    object[] passport = _DB_Connection.Select(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new string[]{ "subdivision_code", "type_id", "nationality_id",
                        "birth_date", "birth_place", "reg_region", "reg_district", "reg_town", "reg_street", "reg_house", "reg_index" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                        })[0];
                    tbSubdivisionCode.Text = passport[0].ToString();
                    cbIDDocType.SelectedItem = _DB_Helper.GetDictionaryItemName(22, (uint)passport[1]);
                    cbNationality.SelectedItem = _DB_Helper.GetDictionaryItemName(7, (uint)passport[2]);
                    dtpDateOfBirth.Value = (DateTime)passport[3];
                    tbPlaceOfBirth.Text = passport[4].ToString();
                    tbRegion.Text = passport[5].ToString();
                    tbDistrict.Text = passport[6].ToString();
                    tbTown.Text = passport[7].ToString();
                    tbStreet.Text = passport[8].ToString();
                    tbHouse.Text = passport[9].ToString();
                    tbPostcode.Text = passport[10].ToString();
                }
                else if ((document[1].ToString() == "school_certificate") || (document[1].ToString() == "high_edu_diploma") || (document[1].ToString() == "academic_diploma"))
                {
                    if (document[1].ToString() == "school_certificate")
                    {
                        rbCertificate.Checked = true;
                        cbCertificateCopy.Checked = true;
                    }
                    else if (document[1].ToString() == "high_edu_diploma")
                    {
                        rbDiploma.Checked = true;
                        cbDiplomaCopy.Checked = true;
                    }
                    else if (document[1].ToString() == "academic_diploma")
                    {
                        rbSpravka.Checked = true;
                        cbCertificateHRD.Checked = true;
                    }

                    tbEduDocSeries.Text = document[2].ToString();
                    tbEduDocNumber.Text = document[3].ToString();
                    if (document[6] != null)
                        cbOriginal.Checked = true;
                    if (document[5].ToString().Split('|').Count() > 1)
                    {
                        cbInstitutionType.SelectedItem = document[5].ToString().Split('|')[0];
                        tbInstitutionNumber.Text = document[5].ToString().Split('|')[1];
                        tbInstitutionLocation.Text = document[5].ToString().Split('|')[2];
                    }

                    object[] diploma = _DB_Connection.Select(DB_Table.DIPLOMA_DOCS_ADDITIONAL_DATA, new string[] { "end_year", "red_diploma" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, document[0])
                    })[0];
                    cbGraduationYear.SelectedItem = diploma[0];
                }
                else if (document[1].ToString() == "ege")
                {
                    tbExamsDocSeries.Text = document[2].ToString();
                    tbExamsDocNumber.Text = document[3].ToString();

                    List<object[]> subjects = _DB_Connection.Select(DB_Table.DOCUMENTS_SUBJECTS_DATA, new string[] { "subject_id", "value" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    });
                    foreach (var subject in subjects)
                        foreach (DataGridViewRow row in dgvExams.Rows)
                            if (row.Cells[0].Value.ToString() == _DB_Helper.GetDictionaryItemName(1, (uint)subject[0]))
                                row.Cells[3].Value = (byte)(uint)subject[1];
                }
                else if (document[1].ToString() == "photos")
                    cbPhotos.Checked = true;
                else if (document[1].ToString() == "sport")
                {
                    SportDoc.docDate = (DateTime)document[4];
                    SportDoc.orgName = document[5].ToString();
                    object[] sportDocData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "document_id", "name" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    })[0];
                    SportDoc.docName = sportDocData[1].ToString();
                    string achievementName = _DB_Helper.GetDictionaryItemName(36, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                        new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    (uint) _DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "institution_achievement_id"}, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    })[0][0])})[0][0]);

                    switch (achievementName)
                    {
                        case "Статус чемпиона и призера Олимпийских игр":
                            SportDoc.diplomaType = "Диплом чемпиона/призера Олимпийских игр";
                            break;
                        case "Статус чемпиона и призера Паралимпийских игр":
                            SportDoc.diplomaType = "Диплом чемпиона/призера Паралимпийских игр";
                            break;
                        case "Статус чемпиона и призера Сурдлимпийских игр":
                            SportDoc.diplomaType = "Диплом чемпиона/призера Сурдлимпийских игр";
                            break;
                        case "Чемпион мира":
                            SportDoc.diplomaType = "Диплом чемпиона мира";
                            break;
                        case "Чемпион Европы":
                            SportDoc.diplomaType = "Диплом чемпиона Европы";
                            break;
                    }
                    cbSport.Checked = true;
                }
                else if (document[1].ToString() == "orphan")
                {
                    QuoteDoc.cause = "Сиротство";
                    cbQuote.Checked = true;
                    QuoteDoc.orphanhoodDocDate = (DateTime)document[4];
                    QuoteDoc.orphanhoodDocOrg = document[5].ToString();

                    object[] orphanDoc = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name", "dictionaries_item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0]),
                        new Tuple<string, Relation, object>("dictionaries_dictionary_id", Relation.EQUAL, 42)
                    })[0];
                    QuoteDoc.orphanhoodDocName = orphanDoc[0].ToString();
                    QuoteDoc.orphanhoodDocType = _DB_Helper.GetDictionaryItemName(42, (uint)orphanDoc[1]);
                }
                else if (document[1].ToString() == "disability")
                {
                    QuoteDoc.cause = "Медицинские показатели";
                    QuoteDoc.medCause = "Справква об установлении инвалидности";
                    cbQuote.Checked = true;
                    QuoteDoc.medDocSerie = int.Parse(document[2].ToString());
                    QuoteDoc.medDocNumber = int.Parse(document[3].ToString());
                    QuoteDoc.disabilityGroup = _DB_Helper.GetDictionaryItemName(23, (uint)_DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA,new string[] { "dictionaries_item_id"},
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL,(uint)document[0]),
                            new Tuple<string, Relation, object>("dictionaries_dictionary_id", Relation.EQUAL, 23)
                        })[0][0]);
                    object[] allowDocument = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "number", "date" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    (uint)(_DB_Connection.Select(DB_Table.APPLICATION_COMMON_BENEFITS, new string[] { "allow_education_document_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(31,"Справка об установлении инвалидности"))
                    }))[0][0])})[0];
                    QuoteDoc.conclusionNumber = int.Parse(allowDocument[0].ToString());
                    QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                }
                else if (document[1].ToString() == "medical")
                {
                    QuoteDoc.cause = "Медицинские показатели";
                    QuoteDoc.medCause = "Заключение психолого-медико-педагогической комиссии";
                    cbQuote.Checked = true;
                    QuoteDoc.medDocSerie = int.Parse(document[2].ToString());
                    QuoteDoc.medDocNumber = int.Parse(document[3].ToString());
                    object[] allowDocument = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "number", "date" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    (uint)(_DB_Connection.Select(DB_Table.APPLICATION_COMMON_BENEFITS, new string[] { "allow_education_document_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(31,"Заключение психолого-медико-педагогической комиссии"))
                    }))[0][0])})[0];
                    QuoteDoc.conclusionNumber = int.Parse(allowDocument[0].ToString());
                    QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                }
                else if (document[1].ToString()=="olimpic")
                {

                }
                else if (document[1].ToString() == "olympic_total")
                {

                }
                else if (document[1].ToString() == "ukraine_olimpic")
                {

                }
                else if (document[1].ToString() == "international_olimpic")
                {

                }
            }

            List < object[] > records = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "faculty_short_name", "direction_id", "is_agreed_date",
            "profile_name", "edu_form_id", "edu_source_id", "target_organization_id"}, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID)
            });

            foreach (object[] entrancesData in records)
            {
                if (entrancesData[2]!=null)
                    cbAgreed.Checked = true;

                string eduLevel = "";
                string temp = _DB_Helper.GetDirectionsDictionaryNameAndCode((uint)entrancesData[1])[1].Split('.')[1];
                switch (_DB_Helper.GetDirectionsDictionaryNameAndCode((uint)entrancesData[1])[1].Split('.')[1])
                {
                    case ("03"):
                        eduLevel = "Бакалавр";
                        break;
                    case ("04"):
                        eduLevel = "Магистр";
                        break;
                    case ("05"):
                        eduLevel = "Специалист";
                        break;
                }
                string cbDirItem = "(" + entrancesData[0] + ", " + eduLevel + ") " + _DB_Helper.GetDirectionsDictionaryNameAndCode((uint)entrancesData[1])[0];
                string cbPrItem = "(" + entrancesData[0] + ", " + eduLevel + ") " + entrancesData[3];

                if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "Бюджетные места")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Очная форма")))
                {
                    if (cbDirection11.SelectedIndex == -1)
                    {
                        cbDirection11.SelectedItem = cbDirItem;
                    }
                    else if (cbDirection12.SelectedIndex == -1)
                    {
                        cbDirection12.SelectedItem = cbDirItem;
                    }
                    else if (cbDirection13.SelectedIndex == -1)
                    {
                        cbDirection13.SelectedItem = cbDirItem;
                    }
                }

                else if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "С оплатой обучения")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Очная форма")))
                {
                    cbDirection21.SelectedItem = cbPrItem;
                }

                else if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "Бюджетные места")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Очно-заочная (вечерняя)")))
                {
                    if (cbDirection31.SelectedIndex == -1)
                    {
                        cbDirection31.SelectedItem = cbDirItem;
                    }
                    else if (cbDirection32.SelectedIndex == -1)
                    {
                        cbDirection32.SelectedItem = cbDirItem;
                    }
                }

                else if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "С оплатой обучения")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Очно-заочная (вечерняя)")))
                {
                    cbDirection41.SelectedItem = cbPrItem;
                }

                else if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "С оплатой обучения")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Заочная форма")))
                {
                    cbDirection51.SelectedItem = cbPrItem;
                }

                else if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "Квота приема лиц, имеющих особое право")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Очная форма")))
                {
                    cbQuote.Checked = true;
                    if (cbDirection61.SelectedIndex == -1)
                    {
                        cbDirection61.SelectedItem = cbDirItem;
                    }
                }

                else if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "Целевой прием")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Очная форма")))
                {
                    cbTarget.Checked = true;
                    _TargetOrganizationID = (uint)entrancesData[6];
                    if (cbDirection71.SelectedIndex == -1)
                    {
                        cbDirection71.SelectedItem = cbDirItem;
                    }
                    else if (cbDirection72.SelectedIndex == -1)
                    {
                        cbDirection72.SelectedItem = cbDirItem;
                    }
                    else if (cbDirection73.SelectedIndex == -1)
                    {
                        cbDirection73.SelectedItem = cbDirItem;
                    }
                }

                else if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "Квота приема лиц, имеющих особое право")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Очно-заочная (вечерняя)")))
                {
                    cbQuote.Checked = true;
                    if (cbDirection81.SelectedIndex == -1)
                    {
                        cbDirection81.SelectedItem = cbDirItem;
                    }
                }

                else if (((uint)entrancesData[5] == _DB_Helper.GetDictionaryItemID(15, "Целевой прием")) && ((uint)entrancesData[4] == _DB_Helper.GetDictionaryItemID(14, "Очно-заочная (вечерняя)")))
                {
                    cbTarget.Checked = true;
                    _TargetOrganizationID = (uint)entrancesData[6];
                    if (cbDirection91.SelectedIndex == -1)
                    {
                        cbDirection91.SelectedItem = cbDirItem;
                    }
                    else if (cbDirection92.SelectedIndex == -1)
                    {
                        cbDirection92.SelectedItem = cbDirItem;
                    }
                }                
            }
        }

        private void UpdateApplication()
        {
            _DB_Connection.Update(DB_Table.ENTRANTS, new Dictionary<string, object> { { "last_name", tbLastName.Text},
                { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},{ "gender_dict_id", 5},
                { "gender_id", _DB_Helper.GetDictionaryItemID( 5, cbSex.SelectedItem.ToString())},
                { "email", mtbEMail.Text}, { "home_phone", mtbHomePhone.Text}, { "mobile_phone", mtbMobilePhone.Text}}, new Dictionary<string, object>
                { { "id", _EntrantID } });

            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;
            Random randNumber = new Random();

            _DB_Connection.Update(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "needs_hostel", cbHostelNeeded.Checked}, { "edit_time", _EditingDateTime},
                { "status_dict_id", 4}, { "status_id", _DB_Helper.GetDictionaryItemID( 4, "Новое")}, { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "first_high_edu", firstHightEdu}, { "mcado", cbMCDAO.Checked}, { "chernobyl", cbChernobyl.Checked}, { "passing_examinations",cbExams.Checked },
                { "priority_right", cbPrerogative.Checked}, { "special_conditions", cbSpecialConditions.Checked} }, new Dictionary<string, object> { { "id", _ApplicationID}});

            List<object[]> appDocumentsLinks = _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, _ApplicationID)
            });

            bool qouteFound = false;
            bool sportFound = false;
            bool MADIOlympFound = false;
            bool olympFound = false;

            if (appDocumentsLinks.Count > 0)
            {
                List<object[]> appDocuments = new List<object[]>();
                foreach (var documentID in appDocumentsLinks)
                {
                    appDocuments.Add(_DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "id", "type", "series", "number", "date", "organization", "original_recieved_date" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)documentID[0])
                    })[0]);
                }
                foreach (object[] document in appDocuments)
                {
                    if (document[1].ToString() == "identity")
                    {
                        _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", tbIDDocSeries.Text}, { "number", tbIDDocNumber.Text}, { "date", dtpIDDocDate.Value} ,
                            { "organization", tbIssuedBy.Text} }, new Dictionary<string, object> { { "id", (uint)document[0] } });

                        _DB_Connection.Update(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {
                            { "last_name", tbLastName.Text}, { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},
                            { "gender_dict_id", 5},{ "gender_id", _DB_Helper.GetDictionaryItemID(5,cbSex.SelectedItem.ToString())},
                            { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", 22},
                            { "type_id", _DB_Helper.GetDictionaryItemID(22,cbIDDocType.SelectedItem.ToString())},
                            { "nationality_dict_id", 7}, { "nationality_id", _DB_Helper.GetDictionaryItemID(7,cbNationality.SelectedItem.ToString())},
                            { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", tbRegion.Text}, { "reg_district", tbDistrict.Text},
                            { "reg_town", tbTown.Text}, { "reg_street", tbStreet.Text}, { "reg_house", tbHouse.Text}, { "reg_index", tbPostcode.Text} },
                            new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                    }

                    else if ((document[1].ToString() == "school_certificate") || (document[1].ToString() == "high_edu_diploma") || (document[1].ToString() == "academic_diploma"))
                    {
                        string eduDocType = "";
                        if (rbSpravka.Checked)
                            eduDocType = "academic_diploma";
                        if (rbCertificate.Checked)
                            eduDocType = "school_certificate";
                        if (rbDiploma.Checked)
                            eduDocType = "high_edu_diploma";

                        if ((document[6].ToString() != "") && (cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } });
                        else if ((document[6].ToString() != "") && (!cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "original_recieved_date", null }, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } });
                        else if ((document[6].ToString() == "") && (cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "original_recieved_date", DateTime.Now }, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } });

                        _DB_Connection.Update(DB_Table.DIPLOMA_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "end_year", cbGraduationYear.SelectedItem },
                                { "red_diploma", cbMedal.Checked} }, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                        if ((cbMedal.Checked) && (_DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                                new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(36, Classes.DB_Helper.MedalAchievement)),
                                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                                })[0][0])
                            }).Count == 0))
                            _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(36, Classes.DB_Helper.MedalAchievement)),
                                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                                })[0][0] }, { "document_id", (uint)document[0] } });
                    }

                    else if (document[1].ToString() == "ege")
                    {
                        if (cbPassportMatch.Checked)
                        {
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", tbIDDocSeries.Text }, { "number", tbIDDocNumber.Text } },
                                new Dictionary<string, object> { { "id", (uint)document[0] } });
                        }
                        else
                        {
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", tbExamsDocSeries.Text }, { "number", tbExamsDocNumber.Text } },
                                new Dictionary<string, object> { { "id", (uint)document[0] } });
                        }
                        string[] fieldNames = new string[] { "document_id", "subject_dict_id", "subject_id", "value" };
                        string[] keysNames = new string[] { "document_id", "subject_dict_id", "subject_id" };
                        List<object[]> oldData = _DB_Connection.Select(DB_Table.DOCUMENTS_SUBJECTS_DATA, fieldNames, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            });
                        List<object[]> newData = new List<object[]>();
                        foreach (DataGridViewRow row in dgvExams.Rows)
                        {
                            if (int.Parse(row.Cells[3].Value.ToString()) != 0)
                                newData.Add(new object[] { (uint)document[0], 1, _DB_Helper.GetDictionaryItemID(1, row.Cells[0].Value.ToString()), row.Cells[3].Value });
                        }
                        UpdateData(DB_Table.DOCUMENTS_SUBJECTS_DATA, oldData, newData, fieldNames, false, keysNames);
                    }

                    else if (document[1].ToString() == "sport")
                    {
                        if (cbSport.Checked)
                        {
                            sportFound = true;
                            uint achevmentCategoryIdNew = 0;
                            switch (SportDoc.diplomaType)
                            {
                                case "Диплом чемпиона/призера Олимпийских игр":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(36, "Статус чемпиона и призера Олимпийских игр");
                                    break;
                                case "Диплом чемпиона/призера Паралимпийских игр":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(36, "Статус чемпиона и призера Паралимпийских игр");
                                    break;
                                case "Диплом чемпиона/призера Сурдлимпийских игр":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(36, "Статус чемпиона и призера Сурдлимпийских игр");
                                    break;
                                case "Диплом чемпиона мира":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(36, "Чемпион Мира");
                                    break;
                                case "Диплом чемпиона Европы":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(36, "Чемпион Европы");
                                    break;
                            }
                            uint achevmentCategoryIdOld =(uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("id", Relation.EQUAL, 
                        
                            (uint)_DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "institution_achievement_id" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            })[0][0])})[0][0];

                            if (achevmentCategoryIdNew == achevmentCategoryIdOld)
                            {
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "date", SportDoc.docDate }, { "organization", SportDoc.orgName } },
                                    new Dictionary<string, object> { { "id", (uint)document[0] } });
                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "name", SportDoc.docName } },
                                    new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                            }
                            else
                            {
                                _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID } });
                                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID},
                                    { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                                    new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                                new Tuple<string, Relation, object>("category_id", Relation.EQUAL, achevmentCategoryIdNew)
                            })[0][0]}, { "document_id", (uint)document[0]} });
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "date", SportDoc.docDate }, { "organization", SportDoc.orgName } },
                                    new Dictionary<string, object> { { "id", (uint)document[0] } });
                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "name", SportDoc.docName } },
                                    new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                            }
                        }
                        else
                        {
                            _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID } });
                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID}, { "documents_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", (uint)document[0] } });
                        }
                    }

                    else if ((document[1].ToString() == "orphan") || (document[1].ToString() == "disability") || (document[1].ToString() == "medical"))
                    {
                        if ((cbQuote.Checked) && (document[1].ToString() == "orphan") && (QuoteDoc.cause == "Сиротство"))
                        {
                            qouteFound = true;
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "date", QuoteDoc.orphanhoodDocDate }, { "organization", QuoteDoc.orphanhoodDocOrg } },
                                    new Dictionary<string, object> { { "id", (uint)document[0] } });

                            _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "name", QuoteDoc.orphanhoodDocName}, { "dictionaries_dictionary_id", 42},
                            { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( 42, QuoteDoc.orphanhoodDocType)}}, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                            _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", 31},
                                { "document_type_id",  _DB_Helper.GetDictionaryItemID(31, "Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                                { "benefit_kind_dict_id", 30}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } },
                            new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                        }
                        else if ((cbQuote.Checked) && ((document[1].ToString() == "disability") || (document[1].ToString() == "medical")) && (QuoteDoc.cause == "Медецинские показатели"))
                        {
                            qouteFound = true;
                            uint allowEducationDocUid = (uint)(appDocuments.Find(x => x[1].ToString() == "allow_education")[0]);
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "number", QuoteDoc.conclusionNumber }, { "date", QuoteDoc.conclusionDate } },
                                new Dictionary<string, object> { { "id", allowEducationDocUid } });

                            if (document[1].ToString() == "disability")
                            {
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", QuoteDoc.medDocSerie }, { "number", QuoteDoc.medDocNumber } },
                                    new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {{ "dictionaries_dictionary_id",23},
                                { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(23,QuoteDoc.disabilityGroup)} }, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                                _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", 31},
                                    { "document_type_id",  _DB_Helper.GetDictionaryItemID(31,"Справка об установлении инвалидности")}, { "allow_education_document_id", allowEducationDocUid},
                                    { "benefit_kind_dict_id", 30}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } },
                                    new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                            }
                            else if (document[1].ToString() == "medical")
                            {
                                qouteFound = true;
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", QuoteDoc.medDocSerie }, { "number", QuoteDoc.medDocNumber } },
                                    new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                                _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", 31},
                                    { "document_type_id",  _DB_Helper.GetDictionaryItemID( 31, "Заключение психолого-медико-педагогической комиссии")}, { "allow_education_document_id", allowEducationDocUid},
                                    { "benefit_kind_dict_id", 30}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } },
                                    new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                            }
                        }
                        else if (document[1].ToString() == "orphan")
                        {
                            _DB_Connection.Delete(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", (uint)document[0] } });
                        }
                        else
                        {
                            uint allowEducationDocUid = (uint)(appDocuments.Find(x => x[1].ToString() == "allow_education")[0]);
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "number", QuoteDoc.conclusionNumber }, { "date", QuoteDoc.conclusionDate } },
                                new Dictionary<string, object> { { "id", allowEducationDocUid } });
                            _DB_Connection.Delete(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", allowEducationDocUid } });
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", allowEducationDocUid } });
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", (uint)document[0] } });
                        }
                    }
                }
                    if (cbQuote.Checked && !qouteFound)
                    {
                        SaveQuote();
                    }
                    if (cbMADIOlympiad.Checked && !MADIOlympFound)
                    {
                        SaveOlympic();
                    }
                    if (cbOlympiad.Checked && !olympFound)
                    {
                        SaveOlympic();
                    }
                    if (cbSport.Checked && !sportFound)
                    {
                        SaveSport();
                    }
            }
            List<object[]> oldD = new List<object[]>();
            List<object[]> newD = new List<object[]>();
            string[] fieldsList = new string[] { "application_id", "faculty_short_name", "direction_id", "edu_form_dict_id", "edu_form_id",
                "edu_source_dict_id", "edu_source_id", "is_agreed_date", "is_for_spo_and_vo", "profile_name", "target_organization_id", "profile_actual" };
            foreach (object[] record in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, fieldsList, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object> ("application_id", Relation.EQUAL, _ApplicationID)
            }))
                oldD.Add(record);
             
            DateTime? agreedDate;
            if (cbAgreed.Checked)
                agreedDate = DateTime.Now;
            else agreedDate = null;

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

                if ((tab.Name.Split('_')[1] != "paid")&&(tab.Name.Split('_')[1] != "target"))
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                string facultyShortName = (cb.SelectedItem.ToString().Split(')')[0]).Split(',')[0].Remove(0, 1);
                                newD.Add(new object[] { _ApplicationID , facultyShortName , _DB_Helper.GetDirectionIDByName((cb.SelectedItem.ToString().Split(')')[1]).Remove(0, 1)),
                                14, eduForm, 15, eduSource, agreedDate, false, null, null, false});
                            }
                    }
                else if (tab.Name.Split('_')[1] != "target")
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
                                newD.Add(new object[] { _ApplicationID , cb.SelectedItem.ToString().Split(')')[0].Split(',')[0].Remove(0,1) , dirID,
                                14, eduForm, 15, eduSource, agreedDate, false, cb.SelectedItem.ToString().Split(')')[1].Remove(0, 1), null, true});
                            }
                    }
                }
                else if (tab.Name.Split('_')[1] == "target")
                {
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                string facultyShortName = (cb.SelectedItem.ToString().Split(')')[0]).Split(',')[0].Remove(0, 1);
                                newD.Add(new object[] { _ApplicationID , facultyShortName , _DB_Helper.GetDirectionIDByName((cb.SelectedItem.ToString().Split(')')[1]).Remove(0, 1)),
                                14, eduForm, 15, eduSource, agreedDate, false, null, _TargetOrganizationID, false});
                            }
                    }
                }
            }
            UpdateData(DB_Table.APPLICATIONS_ENTRANCES, oldD, newD, fieldsList, false, new string[] { "application_id", "faculty_short_name", "direction_id", "edu_form_dict_id", "edu_form_id",
                "edu_source_dict_id", "edu_source_id" });
        }

        private void UpdateData (DB_Table table, List<object[]> oldDataList, List<object[]> newDataList, string[] fieldNames, bool autoGeneratedKey, string[] keyFieldsNames)
        {
            List<object[]> oldList = oldDataList;
            List<object[]> newList = newDataList;
            int j = 0;
            while (j < oldList.Count)
            {
                int index = 0;
                bool keysMatch = true;
                bool valuesMatch =true;

                foreach (object[] newItem in newList)
                {
                    keysMatch = true;
                    valuesMatch = true;
                    index = newList.IndexOf(newItem);
                    for (int i = 0; i < fieldNames.Length; i++)
                    {
                        if ((keyFieldsNames.Contains(fieldNames[i])) && (keysMatch) && (oldList[j][i].ToString() != newItem[i].ToString()))
                        {
                            keysMatch = false;
                            break;
                        }                                
                        else if ((!keyFieldsNames.Contains(fieldNames[i])) && (valuesMatch) && (oldList[j][i]!=null) && (newItem[i]!=null) && (oldList[j][i].ToString() != newItem[i].ToString()))
                            valuesMatch = false;                                
                    }

                    if (keysMatch && valuesMatch)
                    {
                        newList.RemoveAt(index);
                        oldList.RemoveAt(oldList.IndexOf(oldList[j]));
                        break;
                    }
                    else if (keysMatch && !valuesMatch)
                    {
                        Dictionary<string, object> columnsAndValues = new Dictionary<string, object>();
                        Dictionary<string, object> keyAndValues = new Dictionary<string, object>();

                        for (int i = 0; i < fieldNames.Length; i++)
                            if (keyFieldsNames.Contains(fieldNames[i]))
                                keyAndValues.Add(fieldNames[i], newItem[i]);
                            else columnsAndValues.Add(fieldNames[i], newItem[i]);

                        _DB_Connection.Update(table, columnsAndValues, keyAndValues);
                        newList.RemoveAt(index);
                        oldList.Remove(oldList[j]);
                        break;
                    }
                }
                if (!keysMatch)
                    j++;
            }
            if ((oldList.Count>0)&&(newList.Count==0))
                foreach (object[] oldItem in oldList)
                {
                    Dictionary<string, object> keyAndValues = new Dictionary<string, object>();
                    for (int i = 0; i < fieldNames.Length; i++)
                        if (keyFieldsNames.Contains(fieldNames[i]))
                            keyAndValues.Add(fieldNames[i], oldItem[i]);

                    _DB_Connection.Delete(table, keyAndValues);
                }
            else if (newList.Count>0)
                foreach (object[] newItem in newList)
                {
                    Dictionary<string, object> columnsAndValues = new Dictionary<string, object>();
                    if (autoGeneratedKey)
                    {
                        for (int i = 0; i < fieldNames.Length; i++)
                            if (!keyFieldsNames.Contains(fieldNames[i]))
                                columnsAndValues.Add(fieldNames[i], newItem[i]);
                    }
                    else
                    {
                        for (int i = 0; i < fieldNames.Length; i++)
                            columnsAndValues.Add(fieldNames[i], newItem[i]);
                    }
                    _DB_Connection.Insert(table, columnsAndValues);
                }
        }        

        private void SaveBasic ()
        {
            _EntrantID = _DB_Connection.Insert(DB_Table.ENTRANTS, new Dictionary<string, object> { { "last_name", tbLastName.Text},
                { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},{ "gender_dict_id", 5},
                { "gender_id", _DB_Helper.GetDictionaryItemID( 5, cbSex.SelectedItem.ToString())},
                { "email", mtbEMail.Text}, { "home_phone", mtbHomePhone.Text}, { "mobile_phone", mtbMobilePhone.Text}});

            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;

            _ApplicationID = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "entrant_id", _EntrantID.Value}, { "registration_time", DateTime.Now},
                { "needs_hostel", cbHostelNeeded.Checked}, { "registrator_login", _RegistratorLogin},
            { "status_dict_id", 4},{ "status_id", _DB_Helper.GetDictionaryItemID( 4, "Новое")}, { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "first_high_edu", firstHightEdu}, { "mcado", cbMCDAO.Checked}, { "chernobyl", cbChernobyl.Checked}, { "passing_examinations",cbExams.Checked },
                { "priority_right", cbPrerogative.Checked}, { "special_conditions", cbSpecialConditions.Checked} });

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
                { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", tbRegion.Text}, { "reg_district", tbDistrict.Text},
                { "reg_town", tbTown.Text}, { "reg_street", tbStreet.Text}, { "reg_house", tbHouse.Text}, { "reg_index", tbPostcode.Text}, { "reg_flat", tbAppartment.Text} });

            if (cbPhotos.Checked)
            {
                uint otherDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "photos" } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", otherDocID } });
            }
        }

        private void SaveDiploma ()
        {
            string eduDocType = "";
            int eduDocID = 0;
            if (rbSpravka.Checked)
                eduDocType = "academic_diploma";
            else if (rbCertificate.Checked)
                eduDocType = "school_certificate";
            else if (rbDiploma.Checked)
                eduDocType = "high_edu_diploma";

            if (cbOriginal.Checked)
            {
                eduDocID = (int)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", eduDocType }, { "series",  tbEduDocSeries.Text},
                    { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text},
                    { "original_recieved_date", DateTime.Now}}));
                _DB_Connection.Insert(DB_Table.DIPLOMA_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", eduDocID },
                    { "end_year", cbGraduationYear.SelectedItem }, { "red_diploma", cbMedal.Checked} });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", eduDocID } });
            }
            else
            {
                eduDocID = (int)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", eduDocType }, { "series",  tbEduDocSeries.Text},
                    { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}  }));
                _DB_Connection.Insert(DB_Table.DIPLOMA_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", eduDocID },
                    { "end_year", cbGraduationYear.SelectedItem }, { "red_diploma", cbMedal.Checked } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", eduDocID } });
            }
            if (cbMedal.Checked)
                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                    { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(36, Classes.DB_Helper.MedalAchievement)),
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                    })[0][0] }, { "document_id", eduDocID } });
        }

        private void SaveQuote ()
        {
                if (QuoteDoc.cause == "Сиротство")
                {
                    uint orphDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "orphan" },
                    { "date", QuoteDoc.orphanhoodDocDate} , { "organization", QuoteDoc.orphanhoodDocOrg} });
                    _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", orphDocUid},
                        { "name", QuoteDoc.orphanhoodDocName}, { "dictionaries_dictionary_id", 42},
                        { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( 42, QuoteDoc.orphanhoodDocType)} });
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", orphDocUid } });
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", 31},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(31,"Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                            { "reason_document_id", orphDocUid},{ "benefit_kind_dict_id", 30},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } });
                }
                else if (QuoteDoc.cause == "Медицинские показатели")
                {
                    uint allowEducationDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                    { { "number", QuoteDoc.conclusionNumber}, { "date", QuoteDoc.conclusionDate}, { "type", "allow_education"} });
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", allowEducationDocUid } });

                    if (QuoteDoc.medCause == "Справква об установлении инвалидности")
                    {
                        uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "disability" },
                        { "series", QuoteDoc.medDocSerie},  { "number", QuoteDoc.medDocNumber} });
                        _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {
                            { "document_id", medDocUid}, { "dictionaries_dictionary_id",23},
                            { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(23,QuoteDoc.disabilityGroup)} });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", medDocUid } });
                        _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", 31},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(31,"Справка об установлении инвалидности")},
                            { "reason_document_id", medDocUid},{ "allow_education_document_id", allowEducationDocUid}, { "benefit_kind_dict_id", 30},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } });
                    }
                    else if (QuoteDoc.medCause == "Заключение психолого-медико-педагогической комиссии")
                    {
                        uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "medical" },
                        { "series", QuoteDoc.medDocSerie},  { "number", QuoteDoc.medDocNumber} });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", medDocUid } });
                        _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", 31},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID( 31, "Заключение психолого-медико-педагогической комиссии")},
                            { "reason_document_id", medDocUid},{ "allow_education_document_id", allowEducationDocUid}, { "benefit_kind_dict_id", 30},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } });
                    }
                }
        }

        private void SaveSport ()
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

                uint achievementUid = 0;
                if (achievments.Count != 0)
                    achievementUid = uint.Parse(achievments[0][0].ToString());

                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                    { "institution_achievement_id", achievementUid}, { "document_id", sportDocUid} });
        }

        private void SaveOlympic ()
        {
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
        }

        private void SaveExams ()
        {
            uint examsDocId = 0;
            if (cbPassportMatch.Checked)
            {
                examsDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "ege" }, { "series", tbIDDocSeries.Text }, { "number", tbIDDocNumber.Text } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", examsDocId } });
            }

            else
            {
                examsDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "ege" }, { "series", tbEduDocSeries.Text }, { "number", tbExamsDocNumber.Text } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", examsDocId } });
            }

            foreach (DataGridViewRow row in dgvExams.Rows)
            {
                if ((byte)row.Cells[3].Value != 0)
                    _DB_Connection.Insert(DB_Table.DOCUMENTS_SUBJECTS_DATA, new Dictionary<string, object> { { "document_id", examsDocId},
                        { "subject_dict_id", 1} , { "subject_id", _DB_Helper.GetDictionaryItemID( 1, row.Cells[0].Value.ToString())} ,
                        { "value", row.Cells[3].Value} });
            }
        }

        private void SaveDirections ()
        {
            DateTime? agreedDate;
            if (cbAgreed.Checked)
                agreedDate = DateTime.Now;
            else agreedDate = null;

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

                if ((tab.Name.Split('_')[1] != "paid")&&(tab.Name.Split('_')[1] != "target"))
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                string facultyShortName = (cb.SelectedItem.ToString().Split(')')[0]).Split(',')[0];
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                    { "faculty_short_name", facultyShortName.Split('(')[1] }, { "is_agreed_date", agreedDate}, { "profile_actual", false},
                                    { "direction_id", _DB_Helper.GetDirectionIDByName(cb.SelectedItem.ToString().Split(')')[1].Remove(0, 1))},
                                    { "edu_form_dict_id", 14}, { "edu_form_id", eduForm}, { "edu_source_dict_id", 15}, { "edu_source_id", eduSource}, { "is_for_spo_and_vo", false} });
                            }
                    }
                else if ((tab.Name.Split('_')[1] == "paid") && (tab.Name.Split('_')[1] != "target"))
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
                                        { "faculty_short_name",  cb.SelectedItem.ToString().Split(')')[0].Split(',')[0].Remove(0,1) }, { "is_agreed_date", agreedDate},
                                        { "direction_id", dirID }, { "edu_form_dict_id", 14}, { "edu_form_id", eduForm}, { "edu_source_dict_id", 15}, { "edu_source_id", eduSource},
                                    { "is_for_spo_and_vo", false}, { "profile_name", cb.SelectedItem.ToString().Split(')')[1].Remove(0, 1)}, { "profile_actual", true}, });
                            }
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
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                     { "faculty_short_name",  cb.SelectedItem.ToString().Split(')')[0].Split(',')[0].Remove(0,1) }, { "is_agreed_date", agreedDate},
                                    { "direction_id", _DB_Helper.GetDirectionIDByName(cb.SelectedItem.ToString().Split(')')[1].Remove(0, 1))}, { "edu_form_dict_id", 14},
                                    { "edu_form_id", eduForm}, { "edu_source_dict_id", 15}, { "edu_source_id", eduSource}, { "is_for_spo_and_vo", false},
                                    { "target_organization_id", _TargetOrganizationID}, { "profile_actual", false} });
                            }
                    }
                }
            }
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

        private void FillDirectionsProfilesCombobox(ComboBox combobox, bool isProfileList, bool isTarget, string eduForm, string eduSource)
        {
            if (isProfileList && !isTarget)
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
            else if (!isTarget)
            {
                string placesCountColumnName = "";
                if ((eduForm == "Очная форма") && (eduSource == "Бюджетные места"))
                    placesCountColumnName = "places_budget_o";
                else if ((eduForm == "Очная форма") && (eduSource == "Квота приема лиц, имеющих особое право"))
                    placesCountColumnName = "places_quota_o";
                else if ((eduForm == "Очно-заочная (вечерняя)") && (eduSource == "Бюджетные места"))
                    placesCountColumnName = "places_budget_oz";
                else if ((eduForm == "Очно-заочная (вечерняя)") && (eduSource == "Квота приема лиц, имеющих особое право"))
                    placesCountColumnName = "places_quota_oz";

               // string[][] eduLevelsCodes = new string[][] { new string[]{ "03", "Бакалавр" }, new string[] { "04", "Магистр" }, new string[] { "05", "Специалист" } };

               //var directionsData = _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code");

               // var selectedDirs = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[] { "direction_id", "direction_faculty", placesCountColumnName },
               //     new List<Tuple<string, Relation, object>>
               //         {
               //             new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
               //         }).Join(
               //     directionsData,
               //     campDirs => campDirs[0],
               //     dirsData => dirsData[0],
               //     (s1, s2) => new
               //     {
               //         Id = (uint)s1[0],
               //         Faculty = s1[1].ToString(),
               //         Level = eduLevelsCodes.First(x => x[0] == s2[2].ToString().Split('.')[1])[1],
               //         Name = s2[1].ToString()
               //     }).Select( s => new { Value = new Tuple<uint, string, string, string>(s.Id, s.Faculty, s.Level, s.Name),
               //         Display = "(" + s.Faculty + ", " + s.Level + ") " + s.Name }).ToList();
               // cbDirection11.ValueMember = "Value";
               // cbDirection11.DisplayMember = "Display";
               // cbDirection11.DataSource = selectedDirs;
               // cbDirection11.SelectedIndex = -1;

                foreach (var record in _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[] { "direction_faculty", "direction_id", placesCountColumnName },
                    new List<Tuple<string, Relation, object>>
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
                        combobox.Items.Add("(" + record[0] + ", " + eduLevel + ") " + _DB_Helper.GetDirectionNameByID(int.Parse(record[1].ToString())));
                    }
                }
            }
            else if (isTarget)
            {
                string placesCountColumnName = "";
                if ((eduForm == "Очная форма") && (eduSource == "Целевой прием"))
                    placesCountColumnName = "places_o";
                else if ((eduForm == "Очно-заочная (вечерняя)") && (eduSource == "Целевой прием"))
                    placesCountColumnName = "places_oz";
                else if ((eduForm == "Заочная форма") && (eduSource == "Целевой прием"))
                    placesCountColumnName = "places_z";

                foreach (object[] record in _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new string[] { "direction_faculty", "direction_id",
                    placesCountColumnName }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                    }))
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
                        combobox.Items.Add("(" + record[0] + ", " + eduLevel + ") " + _DB_Helper.GetDirectionNameByID(int.Parse(record[1].ToString())));
                    }
            }
        }

        private void cbSpecial_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbQuote.Checked)&&(!_Loading))
            {
                QuotDocsForm form = new QuotDocsForm(_DB_Connection,this);
                form.ShowDialog();
                cbMedCertificate.Enabled = true;
                foreach (Control c in tbDirections.TabPages[5].Controls)
                    c.Enabled = true;
                foreach (Control c in tbDirections.TabPages[7].Controls)
                    c.Enabled = true;
            }
            else if ((cbQuote.Checked) && (_Loading))
            {
                cbMedCertificate.Enabled = true;
                cbMedCertificate.Checked = true;
                foreach (Control c in tbDirections.TabPages[5].Controls)
                    c.Enabled = true;
                foreach (Control c in tbDirections.TabPages[7].Controls)
                    c.Enabled = true;
            }
            else
            {
                cbMedCertificate.Enabled = false;
                cbMedCertificate.Checked = false;
                foreach (Control c in tbDirections.TabPages[5].Controls)
                    c.Enabled = false;
                foreach (Control c in tbDirections.TabPages[7].Controls)
                    c.Enabled = false;
            }
            DirectionDocEnableDisable();
        }

        private void cbSport_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbSport.Checked)&&(!_Loading))
            {
                SportDocsForm form = new SportDocsForm(_DB_Connection,this);
                form.ShowDialog();
            }
        }

        private void cbMADIOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbMADIOlympiad.Checked)&&(!_Loading))
            {
                MADIOlimps form = new MADIOlimps(_DB_Connection,this);
                form.ShowDialog();
            }
        }

        private void cbOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            DirectionDocEnableDisable();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if ((tbLastName.Text == "") || (tbFirstName.Text == "") || (tbMidleName.Text == "") || (tbIDDocSeries.Text == "") || (tbIDDocNumber.Text == "")
                || (tbPlaceOfBirth.Text == "") || (tbRegion.Text == "") || (tbPostcode.Text == ""))
                MessageBox.Show("Обязательные поля в разделе \"Из паспорта\" не заполнены.");
            else if ((rbDiploma.Checked) && (tbEduDocSeries.Text == "") || (tbEduDocNumber.Text == ""))
                MessageBox.Show("Обязательные поля в разделе \"Из аттестата\" не заполнены.");
            else
            {
                bool found = false;
                foreach (TabPage tab in tbDirections.TabPages)
                    foreach (Control control in tab.Controls)
                    {
                        ComboBox cb = control as ComboBox;
                        if ((cb != null) && (cb.SelectedIndex != -1))
                            found = true;
                    }
                if (!found)
                    MessageBox.Show("Не выбрано ни одно направление или профиль.");                
                else
                {
                    bool stop = false;
                    if (cbSport.Checked)
                    {
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
                        new Tuple<string, Relation, object>("category_id", Relation.EQUAL, achevmentCategoryId),
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                        });

                        if (achievments.Count == 0)
                        {
                            MessageBox.Show("Спортивное достижение \"" + _DB_Helper.GetDictionaryItemName(36, achevmentCategoryId) + "\" отсутствует для данной кампании в списке университета.");
                            stop = true;
                        }                            
                    }
                    if (!stop)
                        if (!cbPassportMatch.Checked && (tbExamsDocSeries.Text == "") && (tbExamsDocNumber.Text == ""))
                            MessageBox.Show("Не заполнены обязательные поля в разделе \"Сведения о документе регистрации на ЕГЭ\".");
                        else if (mtbEMail.Text == "")
                            MessageBox.Show("Поле \"Email\" не заполнено");
                        else if (!cbAppAdmission.Checked
                            || (cbChernobyl.Checked || cbQuote.Checked || cbOlympiad.Checked || cbPrerogative.Checked) && !cbDirectionDoc.Checked
                            || (cbQuote.Checked && !cbMedCertificate.Checked)
                            || rbCertificate.Checked && !cbCertificateCopy.Checked
                            || rbDiploma.Checked && !cbDiplomaCopy.Checked
                            || rbSpravka.Checked && !cbCertificateHRD.Checked
                            || !cbPhotos.Checked)
                            MessageBox.Show("В разделе \"Забираемые документы\" не отмечены обязательные поля.");
                        else if (_ApplicationID == null)
                        {
                            SaveApplication();
                            btPrint.Enabled = true;
                        }
                        else
                        {
                            _EditingDateTime = DateTime.Now;
                            UpdateApplication();
                        }
                }
            }
        }

        private void cbPassportMatch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPassportMatch.Checked)
            {
                tbExamsDocSeries.Enabled = false;
                tbExamsDocSeries.Text = tbIDDocSeries.Text;
                tbExamsDocNumber.Enabled = false;
                tbExamsDocNumber.Text = tbIDDocNumber.Text;
                cbExamsDocType.Enabled = false;
                cbExamsDocType.SelectedItem = cbIDDocType.SelectedItem.ToString();
                label33.Enabled = false;
                label34.Enabled = false;
                label35.Enabled = false;
            }
            else
            {
                tbExamsDocSeries.Enabled = true;
                tbExamsDocSeries.Clear();
                tbExamsDocNumber.Enabled = true;
                tbExamsDocNumber.Clear();
                cbExamsDocType.Enabled = true;
                cbIDDocType.SelectedIndex = 0;
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
            if (((cbChernobyl.Checked) || (cbQuote.Checked) || (cbOlympiad.Checked) || (cbPrerogative.Checked))&&(!_Loading))
                cbDirectionDoc.Enabled = true;
            else if (((cbChernobyl.Checked) || (cbQuote.Checked) || (cbOlympiad.Checked) || (cbPrerogative.Checked)) && (_Loading))
            {
                cbDirectionDoc.Enabled = true;
                cbDirectionDoc.Checked = true;
            }
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
        
        private void tbTown_Enter(object sender, EventArgs e)
        {
            _Towns.Clear();
            foreach (string town in _KLADR.GetTowns(tbRegion.Text, tbDistrict.Text))
            {
                _Towns.Add(town, null);
                foreach (string settl in _KLADR.GetSettlements(tbRegion.Text, tbDistrict.Text, town))
                    _Towns.Add(settl, town);
            }

            foreach (string settl in _KLADR.GetSettlements(tbRegion.Text, tbDistrict.Text, ""))
                _Towns.Add(settl, "");

            tbTown.AutoCompleteCustomSource.Clear();
            tbTown.AutoCompleteCustomSource.AddRange(_Towns.Keys.ToArray());

            if (tbTown.AutoCompleteCustomSource.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", tbTown, 3000);
            }
        }

        private void tbStreet_Enter(object sender, EventArgs e)
        {
            tbStreet.AutoCompleteCustomSource.Clear();
            if (_Towns.ContainsKey(tbTown.Text))
            {
                if (_Towns[tbTown.Text]==null)
                    tbStreet.AutoCompleteCustomSource.AddRange(_KLADR.GetStreets(tbRegion.Text, tbDistrict.Text, tbTown.Text, "").ToArray());
                else
                    tbStreet.AutoCompleteCustomSource.AddRange(_KLADR.GetStreets(tbRegion.Text, tbDistrict.Text, _Towns[tbTown.Text], tbTown.Text).ToArray());
            }
            else if(tbTown.Text=="")
                tbStreet.AutoCompleteCustomSource.AddRange(_KLADR.GetStreets(tbRegion.Text, tbDistrict.Text, "", "").ToArray());

            if (tbStreet.AutoCompleteCustomSource.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", tbStreet, 3000);
            }
        }

        private void tbHouse_Enter(object sender, EventArgs e)
        {
            tbHouse.AutoCompleteCustomSource.Clear();
            tbHouse.AutoCompleteCustomSource.AddRange(_KLADR.GetHouses(tbRegion.Text, tbDistrict.Text, tbTown.Text, "", tbStreet.Text).ToArray());
            tbHouse.AutoCompleteCustomSource.AddRange(_KLADR.GetHouses(tbRegion.Text, tbDistrict.Text, "", tbTown.Text, tbStreet.Text).ToArray());

            if (tbHouse.AutoCompleteCustomSource.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", tbHouse, 3000);
            }
        }

        private void ApplicationEdit_Load(object sender, EventArgs e)
        {
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
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            Dictionary<TabPage, Tuple<string, string>> streams = new Dictionary<TabPage, Tuple<string, string>>
            {
                {tpDir_budget_o,new Tuple<string, string>("Очная (дневная) бюджетная","ОБ") },
                {tpDir_paid_o,new Tuple<string, string>("Очная (дневная) платная","ОП") },
                {tpDir_budget_oz,new Tuple<string, string>("Очно-заочная (вечерняя) бюджетная","ОЗБ") },
                {tpDir_paid_oz,new Tuple<string, string>("Очно-заочная (вечерняя) платная","ОЗП") },
                {tpDir_paid_z,new Tuple<string, string>("Заочная платная","ЗП") },
                {tpDir_quote_o,new Tuple<string, string>("Квота, очная (дневная)","ОКВ") },
                {tpDir_target_o,new Tuple<string, string>("Целевой прием, очная (дневная)","ОЦП") },
                {tpDir_quote_oz,new Tuple<string, string>("Квота, очно-заочная (вечерняя)","ОЗКВ") },
                {tpDir_target_oz,new Tuple<string, string>("Целевой прием, очно-заочная (вечерняя)","ОЗЦП") }
            };

            string doc = "moveJournal";
            Classes.DocumentCreator.Create(_DB_Connection, Classes.Utility.DocumentsTemplatesPath + "MoveJournal.xml", doc, _ApplicationID.Value);
            Classes.Utility.Print(doc + ".docx");
            
            List<string[]>[] tableParams = new List<string[]>[] { new List<string[]>(), new List<string[]>() };
            
            foreach (TabPage tab in tbDirections.Controls)
                if (tab.Controls.Cast<Control>().Any(c => c.GetType() == typeof(ComboBox) && ((ComboBox)c).SelectedIndex != -1))
                    tableParams[0].Add(new string[] { streams[tab].Item1 + " форма обучения" });

            foreach (CheckBox cb in gbWithdrawDocs.Controls)
                if (cb.Checked)
                    tableParams[1].Add(new string[] { cb.Text }); //TODO Оригинал

            doc = "inventory";
            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "Inventory.xml",
                doc,
                new string[]
                {
                    _EntrantID.Value.ToString(),
                    tbLastName.Text.ToUpper(),
                    (tbFirstName.Text+" "+tbMidleName.Text).ToUpper(),
                    _DB_Connection.Select(
                        DB_Table.USERS,
                        new string[] {"name" },
                        new List<Tuple<string, Relation, object>> {new Tuple<string, Relation, object>("login",Relation.EQUAL,_RegistratorLogin) }
                        )[0][0].ToString().Split(' ')[0],
                    SystemInformation.ComputerName,
                    DateTime.Now.ToString() //TODO Брать из переменной
                },
                tableParams
                );
            Classes.Utility.Print(doc + ".docx");

            List<string> parameters = new List<string>
            {
                tbLastName.Text.ToUpper(),
                    tbFirstName.Text[0]+".",
                    tbMidleName.Text[0]+".",
                    _EntrantID.Value.ToString(),
                    tbLastName.Text,
                    tbFirstName.Text,
                    tbMidleName.Text,
                    DateTime.Now.Year.ToString(), //TODO Брать из переменной
            };

            foreach (string[] form in tableParams[0])
                parameters.Add(form[0]);

            while (parameters.Count != 13)
                parameters.Add("");

            doc = "percRecordFace";
            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "PercRecordFace.xml",
                doc,
                parameters.ToArray(),
                null
                );
            Classes.Utility.Print(doc + ".docx");

            tableParams[0].Clear();
            foreach (TabPage tab in tbDirections.Controls)
            {
                var cbs = tab.Controls.Cast<Control>().Where(c => c.GetType() == typeof(ComboBox) && ((ComboBox)c).SelectedIndex != -1);
                if (cbs.Count() != 0)
                {
                    tableParams[0].Add(new string[] { streams[tab].Item1 + " форма обучения" });
                    foreach (ComboBox cb in cbs)
                    {
                        string faculty = cb.Text.Split(')')[0].Split(',')[0].Substring(1);
                        string name = cb.Text.Split(')')[1].Remove(0, 1);
                        if (tab.Name.Split('_')[1] != "paid")
                            tableParams[0].Add(new string[] { "          - " + _DB_Connection.Select(
                            DB_Table.DIRECTIONS,
                            new string[] { "short_name" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, faculty),
                                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,  _DB_Helper.GetDirectionIDByName(name))
                            })[0][0].ToString()+ " (" + faculty + ") " + name});
                        else
                        {
                            uint dirID = (uint)_DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_id" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("profiles_direction_faculty", Relation.EQUAL, faculty),
                                new Tuple<string, Relation, object>("profiles_name", Relation.EQUAL, name)
                            })[0][0];

                            tableParams[0].Add(new string[] { "          - " + _DB_Connection.Select(
                            DB_Table.PROFILES,
                            new string[] { "short_name" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, faculty),
                                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,  dirID),
                                new Tuple<string, Relation, object>("name",Relation.EQUAL,name)
                            })[0][0].ToString()+ " (" + faculty + ") " + name});
                        }
                    }
                    tableParams[0].Add(new string[] { "" });
                }
            }

            doc = "receipt";
            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "Receipt.xml",
                doc,
                new string[]
                {
                    _EntrantID.Value.ToString() ,
                    tbLastName.Text.ToUpper(),
                    (tbFirstName.Text+" "+tbMidleName.Text).ToUpper(),
                    _DB_Connection.Select(
                        DB_Table.USERS,
                        new string[] {"name" },
                        new List<Tuple<string, Relation, object>> {new Tuple<string, Relation, object>("login",Relation.EQUAL,_RegistratorLogin) }
                        )[0][0].ToString().Split(' ')[0],
                    SystemInformation.ComputerName,
                    DateTime.Now.ToString(), //TODO Брать из переменной
                  _EditingDateTime.ToString()
                },
                tableParams
                );
            Classes.Utility.Print(doc + ".docx");

            string indAchValue = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, "id", "value").Join(
                _DB_Connection.Select(
                    DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                    new string[] { "institution_achievement_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id",Relation.EQUAL,_ApplicationID)
                    }),
                k1 => k1[0],
                k2 => k2[0],
                (s1, s2) => s1[1]
                ).Max()?.ToString();

            parameters = new List<string>
            {
                tbLastName.Text+" "+tbFirstName.Text+" "+tbMidleName.Text,
                    mtbHomePhone.Text,
                    mtbMobilePhone.Text,
                    cbQuote.Checked?"+":"",
                    cbPrerogative.Checked?"+":"",
                    dgvExams[3,0].Value.ToString(),
                    cbHostelNeeded.Checked?"+":"",
                    cbExams.Checked?"+":"",
                    dgvExams[3,1].Value.ToString(),
                    cbChernobyl.Checked?"+":"",
                    cbMCDAO.Checked?"+":"",
                    dgvExams[3,2].Value.ToString(),
                    cbTarget.Checked?"+":"",
                    cbOriginal.Checked?"+":"",
                    dgvExams[3,3].Value.ToString(),
                    dgvExams[3,4].Value.ToString(),
                    indAchValue,
                    ((byte)dgvExams[3,0].Value+(byte)dgvExams[3,1].Value+(byte)dgvExams[3,2].Value).ToString(),
                    ((byte)dgvExams[3,0].Value+(byte)dgvExams[3,1].Value+(byte)dgvExams[3,3].Value).ToString(),
                    ((byte)dgvExams[3,1].Value+(byte)dgvExams[3,3].Value+(byte)dgvExams[3,4].Value).ToString()
            };

            foreach (TabPage tab in tbDirections.Controls)
            {
                var cbs = tab.Controls.Cast<Control>().Where(c => c.GetType() == typeof(ComboBox) && ((ComboBox)c).SelectedIndex != -1);
                if (cbs.Count() != 0)
                {
                    parameters.Add(streams[tab].Item2);
                    byte count = 0;
                    foreach (ComboBox cb in cbs)
                    {
                        string faculty = cb.Text.Split(')')[0].Split(',')[0].Substring(1);
                        string name = cb.Text.Split(')')[1].Remove(0, 1);
                        if (tab.Name.Split('_')[1] != "paid")
                            parameters.Add(_DB_Connection.Select(
                            DB_Table.DIRECTIONS,
                            new string[] { "short_name" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, faculty),
                                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,  _DB_Helper.GetDirectionIDByName(name))
                            })[0][0].ToString());
                        else
                        {
                            uint dirID = (uint)_DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_id" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("profiles_direction_faculty", Relation.EQUAL, faculty),
                                new Tuple<string, Relation, object>("profiles_name", Relation.EQUAL, name)
                            })[0][0];

                            parameters.Add(_DB_Connection.Select(
                            DB_Table.PROFILES,
                            new string[] { "short_name" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, faculty),
                                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,  dirID),
                                new Tuple<string, Relation, object>("name",Relation.EQUAL,  name)
                            })[0][0].ToString());
                        }

                        count++;
                    }

                    while (count != 3)
                    {
                        parameters.Add("");
                        count++;
                    }
                }
            }

            while (parameters.Count != 40)
                parameters.Add("");

            doc = "percRecordBack";
            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "PercRecordBack.xml",
                doc,
                parameters.ToArray(),
                null
                );
            Classes.Utility.Print(doc + ".docx");
        }

        private void cbTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTarget.Checked && !_Loading)
            {
                TargetOrganizationSelect form = new TargetOrganizationSelect(_DB_Connection,_TargetOrganizationID);
                form.ShowDialog();
                _TargetOrganizationID = form.OrganizationID;
                foreach (Control c in tbDirections.TabPages[6].Controls)
                    c.Enabled = true;
                foreach (Control c in tbDirections.TabPages[8].Controls)
                    c.Enabled = true;
            }
            else if (!cbTarget.Checked && !_Loading)
            {
                foreach (Control c in tbDirections.TabPages[6].Controls)
                    c.Enabled = false;
                foreach (Control c in tbDirections.TabPages[8].Controls)
                    c.Enabled = false;
            }
            else if (_Loading)
            {
                foreach (Control c in tbDirections.TabPages[6].Controls)
                    c.Enabled = true;
                foreach (Control c in tbDirections.TabPages[8].Controls)
                    c.Enabled = true;
            }
        }

        private void cbOriginal_CheckedChanged(object sender, EventArgs e)
        {
            if (cbOriginal.Checked)
            {
                cbCertificateCopy.Text = "Оригинал аттестата";
                cbDiplomaCopy.Text = "Оригинал диплома";
            }
            else
            {
                cbCertificateCopy.Text = "Копия аттестата";
                cbDiplomaCopy.Text = "Копия диплома";
            }
        }

        private void btAddDir_Click(object sender, EventArgs e)
        {
            int tabNumber = int.Parse((sender as Control).Name.Substring((sender as Control).Name.Length - 1, 1));
            ComboBox combo = tbDirections.TabPages[tabNumber - 1].Controls.Find("cbDirection" + tabNumber.ToString() + "1", false)[0] as ComboBox;
            if (combo.SelectedIndex == -1)
            {
                combo.Visible = true;
                combo.Enabled = true;
                Button bt = tbDirections.TabPages[tabNumber - 1].Controls.Find("btRemoveDir" + tabNumber.ToString() + "1", false)[0] as Button;
                bt.Visible = true;
                bt.Enabled = true;
            }
            else if ((tabNumber == 1)|| (tabNumber == 3) || (tabNumber == 7) || (tabNumber == 9))
            {
                combo = tbDirections.TabPages[tabNumber - 1].Controls.Find("cbDirection" + tabNumber.ToString() + "2", false)[0] as ComboBox;
                if (combo.SelectedIndex == -1)
                {
                    combo.Visible = true;
                    combo.Enabled = true;
                    Button bt = tbDirections.TabPages[tabNumber - 1].Controls.Find("btRemoveDir" + tabNumber.ToString() + "2", false)[0] as Button;
                    bt.Visible = true;
                    bt.Enabled = true;
                }
                else if ((tabNumber == 1) || (tabNumber == 7))
                {
                    combo = tbDirections.TabPages[tabNumber - 1].Controls.Find("cbDirection" + tabNumber.ToString() + "3", false)[0] as ComboBox;
                    if (combo.SelectedIndex == -1)
                    {
                        combo.Visible = true;
                        combo.Enabled = true;
                        Button bt = tbDirections.TabPages[tabNumber - 1].Controls.Find("btRemoveDir" + tabNumber.ToString() + "3", false)[0] as Button;
                        bt.Visible = true;
                        bt.Enabled = true;
                    }
                }
            }
        }

        private void btRemoveDir_Click(object sender, EventArgs e)
        {
            int tabNumber = int.Parse((sender as Control).Name.Substring((sender as Control).Name.Length - 2, 1));
            int comboNumber = int.Parse((sender as Control).Name.Substring((sender as Control).Name.Length - 1, 1));
            foreach (Control control in tbDirections.TabPages[tabNumber - 1].Controls)
            {
                ComboBox cb = control as ComboBox;
                if ((cb!=null) && (cb.Name == "cbDirection" + tabNumber + comboNumber))
                {
                    cb.Enabled = false;
                    cb.Visible = false;
                    cb.SelectedIndex = -1;
                }
            }
            (sender as Control).Enabled = false;
            (sender as Control).Visible = false;
        }
    }
}
