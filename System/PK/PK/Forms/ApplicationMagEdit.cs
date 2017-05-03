using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ProgramTuple = System.Tuple<uint, string, uint, uint, string, string>; //Id, Faculty, EduSource, EduForm, ProfileShortName, ProfileName

namespace PK.Forms
{
    partial class ApplicationMagEdit : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly Classes.KLADR _KLADR;

        private uint? _ApplicationID;
        private uint? _EntrantID;
        private uint _CurrCampainID;
        private string _RegistratorLogin;
        private DateTime _EditingDateTime;
        private bool _Loading;
        private uint? _TargetOrganizationID;
        private Dictionary<string, string> _Towns = new Dictionary<string, string>();

        public ApplicationMagEdit(Classes.DB_Connector connection, uint campaignID, string registratorsLogin, uint? applicationId)
        {
            InitializeComponent();

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
            _KLADR = new Classes.KLADR(connection.User, connection.Password);
            _CurrCampainID = campaignID;
            _RegistratorLogin = registratorsLogin;
            _ApplicationID = applicationId;

            cbIDDocType.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.IDENTITY_DOC_TYPE).Values.ToArray());
            cbIDDocType.SelectedIndex = 0;
            cbSex.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.GENDER).Values.ToArray());
            cbSex.SelectedIndex = 0;
            cbNationality.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.COUNTRY).Values.ToArray());
            cbNationality.SelectedIndex = 0;
            cbFirstTime.SelectedIndex = 0;
            tbRegion.AutoCompleteCustomSource.AddRange(_KLADR.GetRegions().ToArray());

            foreach (TabPage tab in tcPrograms.Controls)
            {
                string eduFormName = "";
                string eduSourceName = "";
                switch (tab.Name.Split('_')[2])
                {
                    case "o":
                        eduFormName = Classes.DB_Helper.EduFormO;
                        break;
                    case "oz":
                        eduFormName = Classes.DB_Helper.EduFormOZ;
                        break;
                    case "z":
                        eduFormName = Classes.DB_Helper.EduFormZ;
                        break;
                }

                switch (tab.Name.Split('_')[1])
                {
                    case "budget":
                        eduSourceName = Classes.DB_Helper.EduSourceB;
                        break;
                    case "paid":
                        eduSourceName = Classes.DB_Helper.EduSourceP;
                        break;
                    case "target":
                        eduSourceName = Classes.DB_Helper.EduSourceT;
                        break;
                    case "quote":
                        eduSourceName = Classes.DB_Helper.EduSourceQ;
                        break;
                }
                foreach (Control c in tab.Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        FillProgramsCombobox(cb, eduFormName, eduSourceName);
                }
            }
            if (_ApplicationID != null)
            {
                _Loading = true;
                LoadApplication();
                _Loading = false;
                btPrint.Enabled = true;
            }
        }


        private void btSave_Click(object sender, EventArgs e)
        {
            if ((tbLastName.Text == "") || (tbFirstName.Text == "") || (tbMidleName.Text == "") || (tbIDDocSeries.Text == "") || (tbIDDocNumber.Text == "")
                || (tbPlaceOfBirth.Text == "") || (tbRegion.Text == "") || (tbPostcode.Text == ""))
                MessageBox.Show("Обязательные поля в разделе \"Из паспорта\" не заполнены.");
            else if (tbInstitution.Text == "")
                MessageBox.Show("Обязательные поля в разделе \"Документ об образовании\" не заполнены.");
            else
            {
                bool found = false;
                foreach (TabPage tab in tcPrograms.TabPages)
                    foreach (Control control in tab.Controls)
                    {
                        ComboBox cb = control as ComboBox;
                        if ((cb != null) && (cb.SelectedIndex != -1))
                            found = true;
                    }
                if (!found)
                    MessageBox.Show("Не выбрано ни одно направление или профиль.");
                else if (mtbEMail.Text == "")
                    MessageBox.Show("Поле \"Email\" не заполнено");
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

        private void btPrint_Click(object sender, EventArgs e)
        {

        }

        private void btClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cbSpecialRights_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSpecialRights.Checked && !_Loading)
            {
                //QuotDocs form = new QuotDocs(_DB_Connection, this);
                //form.ShowDialog();
            }
            else if (!cbSpecialRights.Checked)
                cbProgram_quote_o.SelectedIndex = -1;
            foreach (Control c in tcPrograms.TabPages[5].Controls)
            {
                Control cb = c as Control;
                if (cb != null)
                    cb.Enabled = cbSpecialRights.Checked;
            }
        }

        private void btGetIndex_Click(object sender, EventArgs e)
        {
            Tuple<string, string> buf = GetTownSettlement();
            tbPostcode.Text = _KLADR.GetIndex(tbRegion.Text, tbDistrict.Text, buf.Item1, buf.Item2, tbStreet.Text, cbHouse.Text);
            if (tbPostcode.Text == "")
                tbPostcode.Enabled = true;
            else
                tbPostcode.Enabled = false;
        }

        private void cbTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTarget.Checked && !_Loading)
            {
                TargetOrganizationSelect form = new TargetOrganizationSelect(_DB_Connection, _TargetOrganizationID);
                form.ShowDialog();
                _TargetOrganizationID = form.OrganizationID;
            }
            else if (!cbTarget.Checked)
                cbProgram_target_o.SelectedIndex = -1;
            foreach (Control c in tcPrograms.TabPages[4].Controls)
            {
                Control cb = c as Control;
                if (cb != null)
                    cb.Enabled = cbTarget.Checked;
            }
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
                if (_Towns[tbTown.Text] == null)
                    tbStreet.AutoCompleteCustomSource.AddRange(_KLADR.GetStreets(tbRegion.Text, tbDistrict.Text, tbTown.Text, "").ToArray());
                else
                    tbStreet.AutoCompleteCustomSource.AddRange(_KLADR.GetStreets(tbRegion.Text, tbDistrict.Text, _Towns[tbTown.Text], tbTown.Text).ToArray());
            }
            else if (tbTown.Text == "")
                tbStreet.AutoCompleteCustomSource.AddRange(_KLADR.GetStreets(tbRegion.Text, tbDistrict.Text, "", "").ToArray());

            if (tbStreet.AutoCompleteCustomSource.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", tbStreet, 3000);
            }
        }

        private void cbHouse_Enter(object sender, EventArgs e)
        {
            cbHouse.Items.Clear();
            cbHouse.Items.AddRange(_KLADR.GetHouses(tbRegion.Text, tbDistrict.Text, tbTown.Text, "", tbStreet.Text).ToArray());
            cbHouse.Items.AddRange(_KLADR.GetHouses(tbRegion.Text, tbDistrict.Text, "", tbTown.Text, tbStreet.Text).ToArray());

            if (cbHouse.Items.Count == 0)
            {
                toolTip.Show("Не найдено адресов.", cbHouse, 3000);
            }
        }

        private void tbNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) || (e.KeyChar == '\b')) return;
            else
                e.Handled = true;
        }

        private void tbCyrilic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'А' && e.KeyChar <= 'я') || (e.KeyChar == '\b') || (e.KeyChar == '.') || (e.KeyChar == 'ё') || (e.KeyChar == 'Ё')) return;
            else
                e.Handled = true;
        }

        
        private void SaveApplication()
        {
            Cursor.Current = Cursors.WaitCursor;
            SaveBasic();
            SaveDiploma();
            if (cbSpecialRights.Checked)
                SaveQuote();
            SaveDirections();
            Cursor.Current = Cursors.Default;
        }

        private void LoadApplication()
        {
            Cursor.Current = Cursors.WaitCursor;
            LoadBasic();
            LoadDocuments();
            LoadDirections();
            Cursor.Current = Cursors.Default;
        }

        private void UpdateApplication()
        {
            Cursor.Current = Cursors.WaitCursor;
            UpdateBasic();
            UpdateDocuments();
            UpdateDirections();
            Cursor.Current = Cursors.Default;
        }


        private void SaveBasic()
        {
            _EntrantID = _DB_Connection.Insert(DB_Table.ENTRANTS, new Dictionary<string, object> { { "email", mtbEMail.Text }, { "home_phone", mtbHomePhone.Text }, { "mobile_phone", mtbMobilePhone.Text } });
            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;

            _ApplicationID = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "entrant_id", _EntrantID.Value}, { "registration_time", DateTime.Now},
                { "needs_hostel", cbHostelNeeded.Checked}, { "registrator_login", _RegistratorLogin}, { "campaign_id", _CurrCampainID }, { "recommendation", cbRecomendation.Checked },                
                { "first_high_edu", firstHightEdu}, { "special_conditions", cbSpecialConditions.Checked}, { "master_appl", true } });

            uint idDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "identity" },
                { "series", tbIDDocSeries.Text}, { "number", tbIDDocNumber.Text}, { "date", dtpIDDocDate.Value} , { "organization", tbIssuedBy.Text} });

            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID},
                { "documents_id", idDocUid } });

            _DB_Connection.Insert(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", idDocUid},
                { "last_name", tbLastName.Text}, { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},
                { "gender_dict_id", (uint)FIS_Dictionary.GENDER},{ "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER,cbSex.SelectedItem.ToString())},
                { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY}, { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", tbRegion.Text}, { "reg_district", tbDistrict.Text},
                { "reg_town", tbTown.Text}, { "reg_street", tbStreet.Text}, { "reg_house", cbHouse.Text}, { "reg_index", tbPostcode.Text}, { "reg_flat", tbAppartment.Text} });
        }

        private void SaveDiploma()
        {
            int eduDocID = 0;
            eduDocID = (int)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "high_edu_diploma" }, { "series",  tbEduDocSeries.Text},
                { "number", tbEduDocNumber.Text}, { "organization", tbInstitution.Text}, { "date", dtpDiplomaDate.Value }  }));
            _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", eduDocID }, { "text_data", tbSpecialty.Text } });
            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", eduDocID } });            
        }

        private void SaveQuote()
        {
            
        }

        private void SaveDirections()
        {
            foreach (TabPage tab in tcPrograms.TabPages)
            {
                if (tab.Name.Split('_')[1] == "target" && cbTarget.Checked)
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                    { "faculty_short_name", ((ProgramTuple)cb.SelectedValue).Item2 }, { "direction_id", ((ProgramTuple)cb.SelectedValue).Item1},                                    
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM}, { "edu_form_id", ((ProgramTuple)cb.SelectedValue).Item4},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE}, { "edu_source_id", ((ProgramTuple)cb.SelectedValue).Item3},
                                    { "target_organization_id", _TargetOrganizationID }, { "profile_short_name", ((ProgramTuple)cb.SelectedValue).Item5} });
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
                                Control sdf = cb.Parent;
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                     { "faculty_short_name",  ((ProgramTuple)cb.SelectedValue).Item2 }, { "direction_id", ((ProgramTuple)cb.SelectedValue).Item1},                                    
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM}, { "edu_form_id", ((ProgramTuple)cb.SelectedValue).Item4},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE}, { "edu_source_id", ((ProgramTuple)cb.SelectedValue).Item3},
                                    { "profile_short_name", ((ProgramTuple)cb.SelectedValue).Item5} });
                            }
                    }
                }
            }
        }


        private void LoadBasic()
        {
            Text += " № " + _ApplicationID;
            object[] application = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "needs_hostel", "language", "first_high_edu", "special_conditions", "entrant_id" }, 
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, _ApplicationID)
                })[0];

            _EntrantID = (uint)application[4];
            cbHostelNeeded.Checked = (bool)application[0];
            if ((bool)application[2])
                cbFirstTime.SelectedItem = "Впервые";
            else cbFirstTime.SelectedItem = "Повторно";            
            cbSpecialConditions.Checked = (bool)application[3];

            object[] entrant = _DB_Connection.Select(DB_Table.ENTRANTS, new string[] { "email", "home_phone", "mobile_phone" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, application[4])
                })[0];
            mtbEMail.Text = entrant[0].ToString();
            mtbHomePhone.Text = entrant[1].ToString();
            mtbMobilePhone.Text = entrant[2].ToString();
        }

        private void LoadDocuments()
        {
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
                        "birth_date", "birth_place", "reg_region", "reg_district", "reg_town", "reg_street", "reg_house", "reg_index", "reg_flat", "last_name", "first_name",
                    "middle_name"}, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                        })[0];
                    tbSubdivisionCode.Text = passport[0].ToString();
                    cbIDDocType.SelectedItem = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.IDENTITY_DOC_TYPE, (uint)passport[1]);
                    cbNationality.SelectedItem = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.COUNTRY, (uint)passport[2]);
                    dtpDateOfBirth.Value = (DateTime)passport[3];
                    tbPlaceOfBirth.Text = passport[4].ToString();
                    tbRegion.Text = passport[5].ToString();
                    tbDistrict.Text = passport[6].ToString();
                    tbTown.Text = passport[7].ToString();
                    tbStreet.Text = passport[8].ToString();
                    cbHouse.Text = passport[9].ToString();
                    tbPostcode.Text = passport[10].ToString();
                    tbAppartment.Text = passport[11].ToString();
                    tbLastName.Text = passport[12].ToString();
                    tbFirstName.Text = passport[13].ToString();
                    tbMidleName.Text = passport[14].ToString();
                }
                else if (document[1].ToString() == "high_edu_diploma")
                {
                    tbEduDocSeries.Text = document[2].ToString();
                    tbEduDocNumber.Text = document[3].ToString();
                    dtpDiplomaDate.Value = (DateTime)document[4];
                    tbInstitution.Text = document[5].ToString();
                    tbSpecialty.Text = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "text_data" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, document[0])
                    })[0][0].ToString();
                }
                else if (document[1].ToString() == "orphan")
                {
                    //QuoteDoc.cause = "Сиротство";
                    //cbQuote.Checked = true;
                    //QuoteDoc.orphanhoodDocDate = (DateTime)document[4];
                    //QuoteDoc.orphanhoodDocOrg = document[5].ToString();

                    //object[] orphanDoc = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name", "dictionaries_item_id" }, new List<Tuple<string, Relation, object>>
                    //{
                    //    new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0]),
                    //    new Tuple<string, Relation, object>("dictionaries_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.ORPHAN_DOC_TYPE)
                    //})[0];
                    //QuoteDoc.orphanhoodDocName = orphanDoc[0].ToString();
                    //QuoteDoc.orphanhoodDocType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.ORPHAN_DOC_TYPE, (uint)orphanDoc[1]);
                }
                else if (document[1].ToString() == "disability")
                {
                    //QuoteDoc.cause = "Медицинские показатели";
                    //QuoteDoc.medCause = "Справква об установлении инвалидности";
                    //cbQuote.Checked = true;
                    //QuoteDoc.medDocSerie = int.Parse(document[2].ToString());
                    //QuoteDoc.medDocNumber = int.Parse(document[3].ToString());
                    //QuoteDoc.disabilityGroup = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DISABILITY_GROUP, (uint)_DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "dictionaries_item_id" },
                    //    new List<Tuple<string, Relation, object>>
                    //    {
                    //        new Tuple<string, Relation, object>("document_id", Relation.EQUAL,(uint)document[0]),
                    //        new Tuple<string, Relation, object>("dictionaries_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.DISABILITY_GROUP)
                    //    })[0][0]);
                    //object[] allowDocument = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "number", "date" }, new List<Tuple<string, Relation, object>>
                    //{
                    //    new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    //(uint)(_DB_Connection.Select(DB_Table.APPLICATION_COMMON_BENEFITS, new string[] { "allow_education_document_id" }, new List<Tuple<string, Relation, object>>
                    //{
                    //    new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                    //    new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности"))
                    //}))[0][0])})[0];
                    //QuoteDoc.conclusionNumber = int.Parse(allowDocument[0].ToString());
                    //QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                }
                else if (document[1].ToString() == "medical")
                {
                    //QuoteDoc.cause = "Медицинские показатели";
                    //QuoteDoc.medCause = "Заключение психолого-медико-педагогической комиссии";
                    //cbQuote.Checked = true;
                    //QuoteDoc.medDocSerie = int.Parse(document[2].ToString());
                    //QuoteDoc.medDocNumber = int.Parse(document[3].ToString());
                    //object[] allowDocument = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "number", "date" }, new List<Tuple<string, Relation, object>>
                    //{
                    //    new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    //(uint)(_DB_Connection.Select(DB_Table.APPLICATION_COMMON_BENEFITS, new string[] { "allow_education_document_id" }, new List<Tuple<string, Relation, object>>
                    //{
                    //    new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                    //    new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Заключение психолого-медико-педагогической комиссии"))
                    //}))[0][0])})[0];
                    //QuoteDoc.conclusionNumber = int.Parse(allowDocument[0].ToString());
                    //QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                }
            }
        }

        private void LoadDirections()
        {          
            var profilesData = _DB_Connection.Select(DB_Table.PROFILES, new string[] { "short_name", "name", "faculty_short_name", "direction_id" });
            var records = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "direction_id", "faculty_short_name", "is_agreed_date",
            "profile_short_name", "edu_form_id", "edu_source_id", "target_organization_id"}, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID)
            }).Join(
                profilesData,
                campDirs => new Tuple<uint, string, string>((uint)campDirs[0], campDirs[1].ToString(), campDirs[3].ToString()),
                profsData => new Tuple<uint, string, string>((uint)profsData[3], profsData[2].ToString(), profsData[0].ToString()),
                (s1, s2) => new
                {
                    Id = (uint)s1[0],
                    Faculty = s1[1].ToString(),
                    EduSource = (uint)s1[5],
                    EduForm = (uint)s1[4],
                    ProfileShortName = s1[3].ToString(),
                    ProfileName = s2[1].ToString().Split('|')[0],
                    Kafedra = s2[1].ToString().Split('|')[1]
                }).Select(s => new
                {
                    Value = new ProgramTuple(s.Id, s.Faculty, s.EduSource, s.EduForm, s.ProfileShortName, s.ProfileName),
                    Display = s.ProfileName + " (" + s.Kafedra + ", " + s.Faculty + ", " + _DB_Helper.GetDirectionNameAndCode(s.Id).Item2 + ")"
                }).ToList();
            
            foreach (var entrancesData in records)
            {
                if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceB))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbProgram_budget_o.SelectedValue = entrancesData.Value;
                    cbProgram_budget_o.Visible = true;
                    cbProgram_budget_o.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbProgram_paid_o.SelectedValue = entrancesData.Value;
                    cbProgram_paid_o.Visible = true;
                    cbProgram_paid_o.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
                    cbProgram_paid_oz.SelectedValue = entrancesData.Value;
                    cbProgram_paid_oz.Visible = true;
                    cbProgram_paid_oz.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormZ)))
                {
                    cbProgram_paid_z.SelectedValue = entrancesData.Value;
                    cbProgram_paid_z.Visible = true;
                    cbProgram_paid_z.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbSpecialRights.Checked = true;
                    cbProgram_quote_o.SelectedValue = entrancesData.Value;
                    cbProgram_quote_o.Visible = true;
                    cbProgram_quote_o.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbTarget.Checked = true;
                    _TargetOrganizationID = (uint)_DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "target_organization_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, entrancesData.Value.Item2),
                        new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, entrancesData.Value.Item1),
                        new Tuple<string, Relation, object>("edu_form_id", Relation.EQUAL, entrancesData.Value.Item4),
                        new Tuple<string, Relation, object>("edu_source_id", Relation.EQUAL, entrancesData.Value.Item3),
                    })[0][0];
                    cbProgram_target_o.SelectedValue = entrancesData.Value;
                    cbProgram_target_o.Visible = true;
                    cbProgram_target_o.Enabled = true;                
                }
            }
        }


        private void UpdateBasic()
        {
            _DB_Connection.Update(DB_Table.ENTRANTS, new Dictionary<string, object> { { "email", mtbEMail.Text }, { "home_phone", GetNumber(true) }, { "mobile_phone", GetNumber(false) } },
                new Dictionary<string, object> { { "id", _EntrantID } });

            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;
            Random randNumber = new Random();

            _DB_Connection.Update(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "needs_hostel", cbHostelNeeded.Checked}, { "edit_time", _EditingDateTime},
                { "first_high_edu", firstHightEdu}, { "special_conditions", cbSpecialConditions.Checked} }, new Dictionary<string, object> { { "id", _ApplicationID } });
        }

        private void UpdateDocuments()
        {
            List<object[]> appDocumentsLinks = _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, _ApplicationID)
            });

            bool qouteFound = false;

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
                            { "gender_dict_id", (uint)FIS_Dictionary.GENDER},{ "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER,cbSex.SelectedItem.ToString())},
                            { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                            { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                            { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY}, { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                            { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", tbRegion.Text}, { "reg_district", tbDistrict.Text},
                            { "reg_town", tbTown.Text}, { "reg_street", tbStreet.Text}, { "reg_house", cbHouse.Text}, { "reg_flat", tbAppartment.Text}, { "reg_index", tbPostcode.Text} },
                            new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                    }

                    else if (document[1].ToString() == "high_edu_diploma")
                    {
                        if (document[6] as DateTime? != null)
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "type", "high_edu_diploma" },
                                { "number", tbEduDocNumber.Text}, { "organization", tbInstitution.Text }, { "date", dtpDiplomaDate.Value } },
                                new Dictionary<string, object> { { "id", (uint)document[0] } });

                        else if (document[6] as DateTime? == null)
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "type", "high_edu_diploma" },
                                { "number", tbEduDocNumber.Text}, { "organization", tbInstitution.Text }, { "date", dtpDiplomaDate.Value } },
                                new Dictionary<string, object> { { "id", (uint)document[0] } });

                        _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "text_data", tbSpecialty.Text } },
                            new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                    }
                    //else if ((document[1].ToString() == "orphan") || (document[1].ToString() == "disability") || (document[1].ToString() == "medical"))
                    //{
                    //    if ((cbQuote.Checked) && (document[1].ToString() == "orphan") && (QuoteDoc.cause == "Сиротство"))
                    //    {
                    //        qouteFound = true;
                    //        _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "date", QuoteDoc.orphanhoodDocDate }, { "organization", QuoteDoc.orphanhoodDocOrg } },
                    //                new Dictionary<string, object> { { "id", (uint)document[0] } });

                    //        _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "name", QuoteDoc.orphanhoodDocName}, { "dictionaries_dictionary_id", (uint)FIS_Dictionary.ORPHAN_DOC_TYPE},
                    //        { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.ORPHAN_DOC_TYPE, QuoteDoc.orphanhoodDocType)}}, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                    //        _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                    //            { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                    //            { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } },
                    //        new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                    //    }
                    //    else if ((cbQuote.Checked) && ((document[1].ToString() == "disability") || (document[1].ToString() == "medical")) && (QuoteDoc.cause == "Медецинские показатели"))
                    //    {
                    //        qouteFound = true;
                    //        uint allowEducationDocUid = (uint)(appDocuments.Find(x => x[1].ToString() == "allow_education")[0]);
                    //        _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "number", QuoteDoc.conclusionNumber }, { "date", QuoteDoc.conclusionDate } },
                    //            new Dictionary<string, object> { { "id", allowEducationDocUid } });

                    //        if (document[1].ToString() == "disability")
                    //        {
                    //            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", QuoteDoc.medDocSerie }, { "number", QuoteDoc.medDocNumber } },
                    //                new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                    //            _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {{ "dictionaries_dictionary_id",(uint)FIS_Dictionary.DISABILITY_GROUP},
                    //            { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DISABILITY_GROUP,QuoteDoc.disabilityGroup)} }, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                    //            _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                    //                { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности")}, { "allow_education_document_id", allowEducationDocUid},
                    //                { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } },
                    //                new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                    //        }
                    //        else if (document[1].ToString() == "medical")
                    //        {
                    //            qouteFound = true;
                    //            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", QuoteDoc.medDocSerie }, { "number", QuoteDoc.medDocNumber } },
                    //                new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                    //            _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                    //                { "document_type_id",  _DB_Helper.GetDictionaryItemID( FIS_Dictionary.DOCUMENT_TYPE, "Заключение психолого-медико-педагогической комиссии")}, { "allow_education_document_id", allowEducationDocUid},
                    //                { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } },
                    //                new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                    //        }
                    //    }
                    //    else if (document[1].ToString() == "orphan")
                    //    {
                    //        _DB_Connection.Delete(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                    //        _DB_Connection.Delete(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                    //        _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", (uint)document[0] } });
                    //        _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", (uint)document[0] } });
                    //    }
                    //    else
                    //    {
                    //        uint allowEducationDocUid = (uint)(appDocuments.Find(x => x[1].ToString() == "allow_education")[0]);
                    //        _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "number", QuoteDoc.conclusionNumber }, { "date", QuoteDoc.conclusionDate } },
                    //            new Dictionary<string, object> { { "id", allowEducationDocUid } });
                    //        _DB_Connection.Delete(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                    //        _DB_Connection.Delete(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                    //        _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", (uint)document[0] } });
                    //        _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", allowEducationDocUid } });
                    //        _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", allowEducationDocUid } });
                    //        _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", (uint)document[0] } });
                    //    }
                    //}
                }
            }
                if (cbSpecialRights.Checked && !qouteFound)
                {
                    SaveQuote();
                }
        }

        private void UpdateDirections()
        {
            List<object[]> oldD = new List<object[]>();
            List<object[]> newD = new List<object[]>();
            string[] fieldsList = new string[] { "application_id", "faculty_short_name", "direction_id", "edu_form_dict_id", "edu_form_id",
                "edu_source_dict_id", "edu_source_id", "profile_short_name", "target_organization_id" };
            foreach (object[] record in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, fieldsList, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object> ("application_id", Relation.EQUAL, _ApplicationID)
            }))
                oldD.Add(record);

            foreach (TabPage tab in tcPrograms.Controls)
            {
                if (tab.Name.Split('_')[1] != "target")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                newD.Add(new object[] { _ApplicationID , ((ProgramTuple)cb.SelectedValue).Item2, ((ProgramTuple)cb.SelectedValue).Item1,
                                    (uint)FIS_Dictionary.EDU_FORM, ((ProgramTuple)cb.SelectedValue).Item4, (uint)FIS_Dictionary.EDU_SOURCE,
                                    ((ProgramTuple)cb.SelectedValue).Item3, ((ProgramTuple)cb.SelectedValue).Item5, null, true });
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
                                newD.Add(new object[] { _ApplicationID , ((ProgramTuple)cb.SelectedValue).Item2, ((ProgramTuple)cb.SelectedValue).Item1,
                                    (uint)FIS_Dictionary.EDU_FORM, ((ProgramTuple)cb.SelectedValue).Item4, (uint)FIS_Dictionary.EDU_SOURCE,
                                    ((ProgramTuple)cb.SelectedValue).Item3, ((ProgramTuple)cb.SelectedValue).Item5, _TargetOrganizationID, true });
                            }
                    }
                }
            }
            UpdateData(DB_Table.APPLICATIONS_ENTRANCES, oldD, newD, fieldsList, false, new string[] { "application_id", "faculty_short_name", "direction_id", "edu_form_dict_id", "edu_form_id",
                "edu_source_dict_id", "edu_source_id" });
        }


        private void FillProgramsCombobox(ComboBox combobox, string eduForm, string eduSource)
        {
            var profilesData = _DB_Connection.Select(DB_Table.PROFILES, new string[] { "short_name", "name", "faculty_short_name", "direction_id" });

            if (eduSource == Classes.DB_Helper.EduSourceB || eduSource == Classes.DB_Helper.EduSourceQ)
            {
                string placesCountColumnName = "";
                if ((eduForm == Classes.DB_Helper.EduFormO) && (eduSource == Classes.DB_Helper.EduSourceB))
                    placesCountColumnName = "places_budget_o";
                else if ((eduForm == Classes.DB_Helper.EduFormO) && (eduSource == Classes.DB_Helper.EduSourceQ))
                    placesCountColumnName = "places_quota_o";

                var selectedDirs = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[] { "direction_id", "direction_faculty", placesCountColumnName },
                   new List<Tuple<string, Relation, object>>
                       {
                            new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                            new Tuple<string, Relation, object>(placesCountColumnName, Relation.GREATER, 0)
                       }).Join(
                    profilesData,
                    campProfiles => new Tuple<uint, string>((uint)campProfiles[0], campProfiles[1].ToString()),
                    profData => new Tuple<uint, string>((uint)profData[3], profData[2].ToString()),
                    (s1, s2) => new
                    {
                        Id = (uint)s1[0],
                        Faculty = s1[1].ToString(),
                        EduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, eduSource),
                        EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm),
                        ProfileShortName = s2[0].ToString(),
                        ProfileName = s2[1].ToString().Split('|')[0],
                        Kafedra = s2[1].ToString().Split('|')[1]
                    }).Select(s => new
                    {
                        Value = new ProgramTuple(s.Id, s.Faculty, s.EduSource, s.EduForm, s.ProfileShortName, s.ProfileName),
                        Display = s.ProfileName + " (" + s.Kafedra + ", " + s.Faculty + ", " + _DB_Helper.GetDirectionNameAndCode(s.Id).Item2 + ")"
                    }).ToList();
                combobox.ValueMember = "Value";
                combobox.DisplayMember = "Display";
                combobox.DataSource = selectedDirs;
                combobox.SelectedIndex = -1;
            }
            else if (eduSource == Classes.DB_Helper.EduSourceT)
            {
                string placesCountColumnName = "places_o";

                var selectedDirs = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new string[] { "direction_id", "direction_faculty", placesCountColumnName },
                    new List<Tuple<string, Relation, object>>
                       {
                            new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                            new Tuple<string, Relation, object>(placesCountColumnName, Relation.GREATER, 0)
                       }).Join(
                    profilesData,
                    campProfiles => new Tuple<uint, string>((uint)campProfiles[0], campProfiles[1].ToString()),
                    profData => new Tuple<uint, string>((uint)profData[3], profData[2].ToString()),
                    (s1, s2) => new
                    {
                        Id = (uint)s1[0],
                        Faculty = s1[1].ToString(),
                        EduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, eduSource),
                        EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm),
                        ProfileShortName = s2[0].ToString(),
                        ProfileName = s2[1].ToString().Split('|')[0],
                        Kafedra = s2[1].ToString().Split('|')[1]
                    }).Select(s => new
                    {
                        Value = new ProgramTuple(s.Id, s.Faculty, s.EduSource, s.EduForm, s.ProfileShortName, s.ProfileName),
                        Display = s.ProfileName + " (" + s.Kafedra + ", " + s.Faculty + ", " + _DB_Helper.GetDirectionNameAndCode(s.Id).Item2 + ")"
                    }).Distinct().ToList();
                combobox.ValueMember = "Value";
                combobox.DisplayMember = "Display";
                combobox.DataSource = selectedDirs;
                combobox.SelectedIndex = -1;
            }
            else
            {
                string placesCountColumnName = "";

                if (eduForm == Classes.DB_Helper.EduFormO)
                    placesCountColumnName = "places_paid_o";
                else if (eduForm == Classes.DB_Helper.EduFormOZ)
                    placesCountColumnName = "places_paid_oz";
                else if (eduForm == Classes.DB_Helper.EduFormZ)
                    placesCountColumnName = "places_paid_z";

                var selectedDirs = _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_id", "profiles_direction_faculty", "profiles_short_name", placesCountColumnName },
                   new List<Tuple<string, Relation, object>>
                       {
                            new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CurrCampainID),
                            new Tuple<string, Relation, object>(placesCountColumnName, Relation.GREATER, 0)
                       }).Join(
                    profilesData,
                    campProfiles => new Tuple<uint, string, string>((uint)campProfiles[0], campProfiles[1].ToString(), campProfiles[2].ToString()),
                    profData => new Tuple<uint, string, string>((uint)profData[3], profData[2].ToString(), profData[0].ToString()),
                    (s1, s2) => new
                    {
                        Id = (uint)s1[0],
                        Faculty = s1[1].ToString(),
                        EduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, eduSource),
                        EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm),
                        ProfileShortName = s1[2].ToString(),
                        ProfileName = s2[1].ToString().Split('|')[0],
                        Kafedra = s2[1].ToString().Split('|')[1]
                    }).Select(s => new
                    {
                        Value = new ProgramTuple(s.Id, s.Faculty, s.EduSource, s.EduForm, s.ProfileShortName, s.ProfileName),
                        Display = s.ProfileName + " (" + s.Kafedra + ", " + s.Faculty + ", " + _DB_Helper.GetDirectionNameAndCode(s.Id).Item2 + ")"
                    }).ToList();
                combobox.ValueMember = "Value";
                combobox.DisplayMember = "Display";
                combobox.DataSource = selectedDirs;
                combobox.SelectedIndex = -1;
            }
        }

        private string GetNumber(bool home)
        {
            MaskedTextBox textBox;
            if (home)
                textBox = mtbHomePhone;
            else
                textBox = mtbMobilePhone;

            string number = "";
            for (int i = 1; i <= 3; i++)
                if (char.IsDigit(textBox.Text[i]))
                    number += textBox.Text[i];
            for (int i = 5; i <= 7; i++)
                if (char.IsDigit(textBox.Text[i]))
                    number += textBox.Text[i];
            for (int i = 9; i <= 10; i++)
                if (char.IsDigit(textBox.Text[i]))
                    number += textBox.Text[i];
            if (textBox.Text.Length >= 14)
                for (int i = 12; i <= 13; i++)
                    if (char.IsDigit(textBox.Text[i]))
                        number += textBox.Text[i];
            return number;
        }

        private void UpdateData(DB_Table table, List<object[]> oldDataList, List<object[]> newDataList, string[] fieldNames, bool autoGeneratedKey, string[] keyFieldsNames)
        {
            List<object[]> oldList = oldDataList;
            List<object[]> newList = newDataList;
            int j = 0;
            while (j < oldList.Count)
            {
                int index = 0;
                bool keysMatch = true;
                bool valuesMatch = true;

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
                        else if ((!keyFieldsNames.Contains(fieldNames[i])) && (valuesMatch) && (oldList[j][i] != null) && (newItem[i] != null) && (oldList[j][i].ToString() != newItem[i].ToString()))
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
            if ((oldList.Count > 0) && (newList.Count == 0))
                foreach (object[] oldItem in oldList)
                {
                    Dictionary<string, object> keyAndValues = new Dictionary<string, object>();
                    for (int i = 0; i < fieldNames.Length; i++)
                        if (keyFieldsNames.Contains(fieldNames[i]))
                            keyAndValues.Add(fieldNames[i], oldItem[i]);

                    _DB_Connection.Delete(table, keyAndValues);
                }
            else if (newList.Count > 0)
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

        private Tuple<string, string> GetTownSettlement()
        {
            if (_Towns.ContainsKey(tbTown.Text))
            {
                if (_Towns[tbTown.Text] == null)
                    return new Tuple<string, string>(tbTown.Text, "");
                else
                {
                    string[] buf = tbTown.Text.Split('(');
                    if (buf.Length == 4)
                        return new Tuple<string, string>(_Towns[tbTown.Text], buf[0] + "(" + buf[1]);
                    else
                        return new Tuple<string, string>(_Towns[tbTown.Text], tbTown.Text);
                }
            }
            else if (tbTown.Text == "")
                return new Tuple<string, string>("", "");

            return null;
        }
    }
}
