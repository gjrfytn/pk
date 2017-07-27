using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class ApplicationSPOEdit : Form
    {
        private readonly DB_Connector _DB_Connection;
        private readonly DB_Helper _DB_Helper;
        private readonly Classes.KLADR _KLADR;

        private uint? _ApplicationID;
        private uint? _EntrantID;
        private uint _CurrCampainID;
        private string _RegistratorLogin;
        private DateTime _EditingDateTime;
        private bool _Loading;

        private List<string[]> _InstitutionTypes = new List<string[]>()
        {
            new string[] {"Средняя школа", "school_certificate"},
            new string[] { "Лицей", "school_certificate"},
            new string[] { "Гимназия", "school_certificate"},
            new string[] { "УВК", "school_certificate"},
            new string[] { "ПТУ", "middle_edu_diploma"},
            new string[] { "СПТУ", "middle_edu_diploma"},
            new string[] { "ПУ", "middle_edu_diploma"},
            new string[] { "Техникум", "middle_edu_diploma"},
            new string[] { "Колледж", "middle_edu_diploma"},
            new string[] {"Университет", "high_edu_diploma"},
            new string[] { "Академия", "high_edu_diploma"},
            new string[] { "Институт", "high_edu_diploma"}
        };

        private bool _DistrictNeedsReload;
        private bool _TownNeedsReload;
        private bool _StreetNeedsReload;
        private bool _HouseNeedsReload;

        public ApplicationSPOEdit(DB_Connector connection, uint campaignID, string registratorsLogin, uint? applicationId)
        {
            _DB_Connection = connection;
            _RegistratorLogin = registratorsLogin;

            InitializeComponent();

            _DB_Helper = new DB_Helper(_DB_Connection);
            _KLADR = new Classes.KLADR(connection.User, connection.Password);
            _CurrCampainID = Classes.Settings.CurrentCampaignID;
            _ApplicationID = applicationId;

            FillComboBox(cbIDDocType, FIS_Dictionary.IDENTITY_DOC_TYPE);
            FillComboBox(cbSex, FIS_Dictionary.GENDER);
            FillComboBox(cbNationality, FIS_Dictionary.COUNTRY);
            cbForeignLanguage.SelectedIndex = 0;
            rbCertificate.Checked = false;
            rbCertificate.Checked = true;
            cbRegion.Items.AddRange(_KLADR.GetRegions().ToArray());
            dtpDateOfBirth.MaxDate = DateTime.Now;
            dtpIDDocDate.MaxDate = DateTime.Now;
            for (int i = DateTime.Now.Year; i >= 1950; i--)
                cbGraduationYear.Items.Add(i);
            cbGraduationYear.SelectedIndex = 0;
            LoadSPOProgram();

            if (_ApplicationID != null)
            {
                _Loading = true;
                LoadApplication();
                _Loading = false;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (cbIDDocType.SelectedItem.ToString() == DB_Helper.PassportName && (string.IsNullOrWhiteSpace(tbLastName.Text)
                || string.IsNullOrWhiteSpace(tbFirstName.Text) || string.IsNullOrWhiteSpace(tbIDDocSeries.Text) || string.IsNullOrWhiteSpace(tbIDDocNumber.Text)
                || string.IsNullOrWhiteSpace(tbPlaceOfBirth.Text) || string.IsNullOrWhiteSpace(cbRegion.Text) || string.IsNullOrWhiteSpace(tbPostcode.Text)))
                MessageBox.Show("Обязательные поля в разделе \"Из паспорта\" не заполнены.");
            else if (cbIDDocType.SelectedItem.ToString() == DB_Helper.PassportName && !mtbSubdivisionCode.MaskFull)
                MessageBox.Show("Код подразделения в разделе \"Из паспорта\" не заполнен.");
            else if ((rbDiploma.Checked || rbCertificate.Checked && (int)cbGraduationYear.SelectedItem < 2014)
                && (string.IsNullOrWhiteSpace(tbEduDocSeries.Text) || string.IsNullOrWhiteSpace(tbEduDocNumber.Text)))
                MessageBox.Show("Обязательные поля в разделе \"Из аттестата\" не заполнены.");
            else if ((int)cbGraduationYear.SelectedItem >= 2014 && rbCertificate.Checked && (!string.IsNullOrWhiteSpace(tbEduDocSeries.Text) || tbEduDocNumber.Text.Length != 14))
                MessageBox.Show("Неправильный формат серии и номера аттестата для этого года окончания.");
            else if (!cbAppAdmission.Checked || !cbEduDoc.Checked || !cbPassportCopy.Checked)
                MessageBox.Show("В разделе \"Забираемые документы\" не отмечены обязательные поля.");
            else
            {
                    bool dateOk = true;
                    if ((dtpDateOfBirth.Tag.ToString() == "false" || dtpIDDocDate.Tag.ToString() == "false")
                        && !SharedClasses.Utility.ShowChoiceMessageBox("Значения некоторых полей дат не были изменены. Продолжить?", "Даты не изменены"))
                        dateOk = false;
                    if (dateOk)
                    {
                        bool passportOK = true;
                        List<object[]> passportFound = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "id" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("type", Relation.EQUAL, "identity"),
                                new Tuple<string, Relation, object>("series", Relation.EQUAL, tbIDDocSeries.Text),
                                new Tuple<string, Relation, object>("number", Relation.EQUAL, tbIDDocNumber.Text)
                            });
                        if (passportFound.Count > 0)
                        {
                            List<object[]> oldApplications = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "status", "campaign_id", "id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "applications_id" },
                                    new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("documents_id", Relation.EQUAL, (uint)passportFound[0][0])})[0][0])
                                });
                            foreach (object[] app in oldApplications)
                                if ((uint)app[1] == _CurrCampainID && (uint)app[2] != _ApplicationID)
                                {
                                    passportOK = false;
                                    if (app[0].ToString() != "withdrawn")
                                    {
                                        MessageBox.Show("В данной кампании уже существует действующее заявление на этот паспорт.");
                                        break;
                                    }
                                    else if (SharedClasses.Utility.ShowChoiceMessageBox("В данной кампании уже существует заявление на этот паспорт, по которому забрали документы. Создать новое заявление на этот паспорт?", "Паспорт уже существует"))
                                        passportOK = true;
                                }
                        }
                        if (passportOK)
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
                            {
                                if (_ApplicationID == null)
                                {
                                    SaveApplication(transaction);
                                    btPrint.Enabled = true;
                                    btWithdraw.Enabled = true;
                                }
                                else
                                {
                                    _EditingDateTime = DateTime.Now;
                                    UpdateApplication(transaction);
                                }
                                transaction.Commit();
                            }
                            Cursor.Current = Cursors.Default;
                        }
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

        private void btWithdraw_Click(object sender, EventArgs e)
        {

        }

        private void btGetIndex_Click(object sender, EventArgs e)
        {
            tbPostcode.Text = "Поиск...";
            btGetIndex.Enabled = false;
            cbRegion.Enabled = false;
            cbDistrict.Enabled = false;
            cbTown.Enabled = false;
            cbStreet.Enabled = false;
            cbHouse.Enabled = false;
            tbAppartment.Enabled = false;
            backgroundWorker.RunWorkerAsync(Tuple.Create(cbRegion.Text, cbDistrict.Text, cbTown.Text, cbStreet.Text, cbHouse.Text));
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var address = (Tuple<string, string, string, string, string>)e.Argument;
            e.Result = _KLADR.GetIndex(address.Item1, address.Item2, address.Item3, address.Item4, address.Item5);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            tbPostcode.Text = e.Result as string;
            if (tbPostcode.Text == "")
                tbPostcode.Enabled = true;
            else
                tbPostcode.Enabled = false;

            btGetIndex.Enabled = true;
            cbRegion.Enabled = true;
            cbDistrict.Enabled = true;
            cbTown.Enabled = true;
            cbStreet.Enabled = true;
            cbHouse.Enabled = true;
            tbAppartment.Enabled = true;
        }

        private void cbDistrict_Enter(object sender, EventArgs e)
        {
            if (_DistrictNeedsReload)
            {
                _DistrictNeedsReload = false;
                cbDistrict.Items.Clear();
                cbDistrict.Items.AddRange(_KLADR.GetDistricts(cbRegion.Text).ToArray());

                if (cbDistrict.Items.Count == 0)
                {
                    toolTip.Show("Не найдено адресов.", cbDistrict, 3000);
                }
            }
        }

        private void cbTown_Enter(object sender, EventArgs e)
        {
            if (_TownNeedsReload)
            {
                _TownNeedsReload = false;
                cbTown.Items.Clear();
                cbTown.Items.AddRange(_KLADR.GetTownsAndSettlements(cbRegion.Text, cbDistrict.Text).ToArray());

                if (cbTown.Items.Count == 0)
                {
                    toolTip.Show("Не найдено адресов.", cbTown, 3000);
                }
            }
        }

        private void cbStreet_Enter(object sender, EventArgs e)
        {
            if (_StreetNeedsReload)
            {
                _StreetNeedsReload = false;
                cbStreet.Items.Clear();
                cbStreet.Items.AddRange(_KLADR.GetStreets(cbRegion.Text, cbDistrict.Text, cbTown.Text).ToArray());

                if (cbStreet.Items.Count == 0)
                {
                    toolTip.Show("Не найдено адресов.", cbStreet, 3000);
                }
            }
        }

        private void cbHouse_Enter(object sender, EventArgs e)
        {
            if (_HouseNeedsReload)
            {
                _HouseNeedsReload = false;
                cbHouse.Items.Clear();
                cbHouse.Items.AddRange(_KLADR.GetHouses(cbRegion.Text, cbDistrict.Text, cbTown.Text, cbStreet.Text).ToArray());

                if (cbHouse.Items.Count == 0)
                {
                    toolTip.Show("Не найдено адресов.", cbHouse, 3000);
                }
            }
        }

        private void cbAddress_TextChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).Name != "cbHouse")
            {
                _HouseNeedsReload = true;
                cbHouse.Text = "";
                if (((ComboBox)sender).Name != "cbStreet")
                {
                    _StreetNeedsReload = true;
                    cbStreet.Text = "";
                    if (((ComboBox)sender).Name != "cbTown")
                    {
                        _TownNeedsReload = true;
                        cbTown.Text = "";
                        if (((ComboBox)sender).Name != "cbDistrict")
                        {
                            _DistrictNeedsReload = true;
                            cbDistrict.Text = "";
                        }
                    }
                }
            }
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCertificate.Checked)
            {
                if (cbOriginal.Checked)
                    cbEduDoc.Text = "Оригинал аттестата";
                else
                    cbEduDoc.Text = "Копия аттестата";
                cbInstitutionType.Items.Clear();
                foreach (string[] type in _InstitutionTypes.Where(s => s[1] == "school_certificate"))
                    cbInstitutionType.Items.Add(type[0]);
                cbInstitutionType.SelectedIndex = 0;
            }
            else if (rbDiploma.Checked)
            {
                if (cbOriginal.Checked)
                    cbEduDoc.Text = "Оригинал диплома";
                else
                    cbEduDoc.Text = "Копия диплома";
                cbInstitutionType.Items.Clear();
                foreach (string[] type in _InstitutionTypes.Where(s => s[1] == "middle_edu_diploma" || s[1] == "high_edu_diploma"))
                    cbInstitutionType.Items.Add(type[0]);
                cbInstitutionType.SelectedIndex = 0;
            }
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            ((DateTimePicker)sender).Tag = true;
        }


        private void SaveApplication(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            SaveBasic(transaction);
            SaveDiploma(transaction);
            if (cbMedCertificate.Checked)
                SaveCertificate(transaction);
            SaveDirections(transaction);
        }

        private void LoadApplication()
        {
            Cursor.Current = Cursors.WaitCursor;
            LoadBasic();
            LoadDocuments();
            LoadDirections();
            Cursor.Current = Cursors.Default;
        }

        private void UpdateApplication(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            UpdateBasic(transaction);
            UpdateDocuments(transaction);
            UpdateDirections(transaction);
        }


        private void SaveBasic(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            List<object[]> passportFound = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("type", Relation.EQUAL, "identity"),
                new Tuple<string, Relation, object>("series", Relation.EQUAL, tbIDDocSeries.Text),
                new Tuple<string, Relation, object>("number", Relation.EQUAL, tbIDDocNumber.Text)
            });
            if (passportFound.Count > 0)
                _EntrantID = (uint)_DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "entrant_id" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS,
                    new string[] { "applications_id" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("documents_id", Relation.EQUAL, (uint)passportFound[0][0])
                })[0][0])})[0][0];
            else
            {
                char[] passwordChars = { 'a', 'b', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                int passwordLength = 6;
                string password = "";
                Random rand = new Random();
                for (int i = 0; i < passwordLength; i++)
                    password += passwordChars[rand.Next(passwordChars.Length)];
                _EntrantID = _DB_Connection.Insert(DB_Table.ENTRANTS, new Dictionary<string, object>
                {
                    { "email", mtbEMail.Text },
                    { "home_phone", tbHomePhone.Text },
                    { "mobile_phone", tbMobilePhone.Text },
                    { "personal_password", password }
                }, transaction);
            }

            _ApplicationID = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object>
            {
                { "entrant_id", _EntrantID.Value},
                { "registration_time", DateTime.Now},
                { "needs_hostel", false},
                { "registrator_login", _RegistratorLogin},
                { "campaign_id", _CurrCampainID },
                { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "priority_right", false},
                { "special_conditions", false},
                { "master_appl", false},
                { "first_high_edu", true },
                { "comment", tbReturnAdress.Text.Trim()}
            }, transaction);

            uint idDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
            {
                { "type", "identity" },
                { "series", tbIDDocSeries.Text.Trim()},
                { "number", tbIDDocNumber.Text.Trim()},
                { "date", dtpIDDocDate.Value},
                { "organization", tbIssuedBy.Text.Trim()}
            }, transaction);

            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
            {
                { "applications_id", _ApplicationID},
                { "documents_id", idDocUid }
            }, transaction);

            _DB_Connection.Insert(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
            {
                { "document_id", idDocUid},
                { "last_name", tbLastName.Text.Trim()},
                { "first_name", tbFirstName.Text.Trim()},
                { "middle_name", tbMiddleName.Text.Trim()},
                { "gender_dict_id", (uint)FIS_Dictionary.GENDER},
                { "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER, cbSex.SelectedItem.ToString())},
                { "subdivision_code", mtbSubdivisionCode.MaskFull? mtbSubdivisionCode.Text: null },
                { "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY},
                { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                { "birth_date", dtpDateOfBirth.Value},
                { "birth_place", tbPlaceOfBirth.Text.Trim()},
                { "reg_region", cbRegion.Text},
                { "reg_district", cbDistrict.Text},
                { "reg_town", cbTown.Text},
                { "reg_street", cbStreet.Text},
                { "reg_house", cbHouse.Text},
                { "reg_index", tbPostcode.Text},
                { "reg_flat", tbAppartment.Text}
            }, transaction);

            if (cbPhotos.Checked)
            {
                uint otherDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", "photos" }
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", _ApplicationID },
                    { "documents_id", otherDocID }
                }, transaction);
            }
        }

        private void SaveDiploma(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            string eduDocType = _InstitutionTypes.First(s => s[0] == cbInstitutionType.SelectedItem.ToString())[1];
            uint eduDocID = 0;

            if (cbOriginal.Checked)
            {
                eduDocID = (uint)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", eduDocType },
                    { "series",  tbEduDocSeries.Text.Trim()},
                    { "number", tbEduDocNumber.Text.Trim()},
                    { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text.Trim() + "|" + tbInstitutionLocation.Text.Trim()},
                    { "original_recieved_date", DateTime.Now}
                }, transaction));
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "document_id", eduDocID },
                    { "year", cbGraduationYear.SelectedItem }
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", _ApplicationID },
                    { "documents_id", eduDocID }
                }, transaction);
            }
            else
            {
                eduDocID = (uint)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", eduDocType },
                    { "series",  tbEduDocSeries.Text.Trim()},
                    { "number", tbEduDocNumber.Text.Trim()},
                    { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text.Trim() + "|" + tbInstitutionLocation.Text.Trim()}
                }, transaction));
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "document_id", eduDocID },
                    { "year", cbGraduationYear.SelectedItem }
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", _ApplicationID },
                    { "documents_id", eduDocID }
                }, transaction);
            }
        }

        private void SaveCertificate(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            uint spravkaID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
            {
                { "type", "medical" }
            }, transaction);
            _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
            {
                { "document_id", spravkaID },
                { "name", DB_Helper.MedCertificate }
            }, transaction);
            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
            {
                { "applications_id", _ApplicationID },
                { "documents_id", spravkaID }
            }, transaction);
        }

        private void SaveDirections(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            //TODO
        }


        private void LoadBasic()
        {
            Text += " № " + _ApplicationID;
            object[] application = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[]{ "needs_hostel", "language", "first_high_edu",
                "mcado", "chernobyl", "passing_examinations", "priority_right", "special_conditions", "entrant_id", "status", "comment" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, _ApplicationID)
                })[0];
            if (application[9].ToString() == "withdrawn")
            {
                foreach (Control control in Controls)
                    control.Enabled = false;
                btClose.Enabled = true;
            }
            else
            {
                btWithdraw.Enabled = true;
                btPrint.Enabled = true;
            }
            _EntrantID = (uint)application[8];
            cbForeignLanguage.SelectedItem = application[1];
            cbPassportCopy.Checked = true;
            cbAppAdmission.Checked = true;
            tbReturnAdress.Text = application[10].ToString();

            object[] entrant = _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, new string[] { "last_name", "first_name", "middle_name" },
                new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("id", Relation.EQUAL, application[8])
            })[0];
            tbLastName.Text = entrant[0].ToString();
            tbFirstName.Text = entrant[1].ToString();
            tbMiddleName.Text = entrant[2].ToString();

            entrant = _DB_Connection.Select(DB_Table.ENTRANTS, new string[] { "email", "home_phone", "mobile_phone" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, application[8])
                })[0];
            mtbEMail.Text = entrant[0].ToString();
            tbMobilePhone.Text = entrant[2].ToString();
            tbHomePhone.Text = entrant[1].ToString();
        }

        private void LoadDocuments()
        {
            List<object[]> appDocuments = new List<object[]>();
            foreach (var documentID in _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" },
                new List<Tuple<string, Relation, object>>
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
                        "birth_date", "birth_place", "reg_region", "reg_district", "reg_town", "reg_street", "reg_house", "reg_index", "reg_flat", "gender_id" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                        })[0];
                    mtbSubdivisionCode.Text = passport[0].ToString();
                    cbIDDocType.SelectedItem = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.IDENTITY_DOC_TYPE, (uint)passport[1]);
                    cbNationality.SelectedItem = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.COUNTRY, (uint)passport[2]);
                    dtpDateOfBirth.Value = (DateTime)passport[3];
                    tbPlaceOfBirth.Text = passport[4].ToString();
                    cbRegion.Text = passport[5].ToString();
                    cbDistrict.Text = passport[6].ToString();
                    cbTown.Text = passport[7].ToString();
                    cbStreet.Text = passport[8].ToString();
                    cbHouse.Text = passport[9].ToString();
                    tbPostcode.Text = passport[10].ToString();
                    tbAppartment.Text = passport[11].ToString();
                    cbSex.SelectedItem = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.GENDER, (uint)passport[12]);
                }
                else if (document[1].ToString() == "school_certificate" || document[1].ToString() == "middle_edu_diploma" || document[1].ToString() == "high_edu_diploma")
                {
                    if (document[1].ToString() == "school_certificate")
                    {
                        rbCertificate.Checked = true;
                        cbEduDoc.Checked = true;
                    }
                    else if (document[1].ToString() == "middle_edu_diploma" || document[1].ToString() == "high_edu_diploma")
                    {
                        rbDiploma.Checked = true;
                        cbEduDoc.Checked = true;
                    }

                    tbEduDocSeries.Text = document[2].ToString();
                    tbEduDocNumber.Text = document[3].ToString();
                    if (document[6] as DateTime? != null)
                        cbOriginal.Checked = true;
                    if (document[5].ToString().Split('|').Length > 1)
                    {
                        cbInstitutionType.SelectedItem = document[5].ToString().Split('|')[0];
                        tbInstitutionNumber.Text = document[5].ToString().Split('|')[1];
                        tbInstitutionLocation.Text = document[5].ToString().Split('|')[2];
                    }
                    cbGraduationYear.SelectedItem = int.Parse(_DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "year" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, document[0])
                    })[0][0].ToString());
                }
                else if (document[1].ToString() == "photos")
                    cbPhotos.Checked = true;

                else if (document[1].ToString() == "medical")
                {
                    List<object[]> spravkaData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    });
                    if (spravkaData.Count > 0 && spravkaData[0][0].ToString() == DB_Helper.MedCertificate)
                        cbMedCertificate.Checked = true;
                }
            }
        }

        private void LoadDirections()
        {
            //TODO
        }


        private void UpdateBasic(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            _DB_Connection.Update(DB_Table.ENTRANTS, new Dictionary<string, object>
            {
                { "email", mtbEMail.Text },
                { "home_phone", tbHomePhone.Text },
                { "mobile_phone", tbMobilePhone.Text }
            }, new Dictionary<string, object>
            {
                { "id", _EntrantID }
            }, transaction);

            _DB_Connection.Update(DB_Table.APPLICATIONS, new Dictionary<string, object>
            {
                { "edit_time", _EditingDateTime},
                { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "comment", tbReturnAdress.Text.Trim()}
            }, new Dictionary<string, object>
            {
                { "id", _ApplicationID }
            }, transaction);
        }

        private void UpdateDocuments(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            List<object[]> appDocumentsLinks = _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, _ApplicationID)
            });

            bool certificateFound = false;
            bool photosFound = false;

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
                            { "organization", tbIssuedBy.Text} }, new Dictionary<string, object> { { "id", (uint)document[0] } }, transaction);

                        _DB_Connection.Update(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {
                            { "last_name", tbLastName.Text}, { "first_name", tbFirstName.Text}, { "middle_name", tbMiddleName.Text},
                            { "gender_dict_id", (uint)FIS_Dictionary.GENDER},{ "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER,cbSex.SelectedItem.ToString())},
                            { "subdivision_code", mtbSubdivisionCode.MaskFull? mtbSubdivisionCode.Text: null },{ "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                            { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                            { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY}, { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                            { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", cbRegion.Text}, { "reg_district", cbDistrict.Text},
                            { "reg_town", cbTown.Text}, { "reg_street", cbStreet.Text}, { "reg_house", cbHouse.Text}, { "reg_flat", tbAppartment.Text}, { "reg_index", tbPostcode.Text} },
                            new Dictionary<string, object> { { "document_id", (uint)document[0] } }, transaction);
                    }

                    else if (document[1].ToString() == "school_certificate" || document[1].ToString() == "middle_edu_diploma"
                        || document[1].ToString() == "high_edu_diploma")
                    {
                        string eduDocType = _InstitutionTypes.First(s => s[0] == cbInstitutionType.SelectedItem.ToString())[1];

                        if ((document[6] as DateTime?) != null && (cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } }, transaction);
                        else if ((document[6] as DateTime?) != null && (!cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "original_recieved_date", null }, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } }, transaction);
                        else if ((document[6] as DateTime?) == null && (cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "original_recieved_date", DateTime.Now }, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } }, transaction);
                        else
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } }, transaction);

                        _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "year", cbGraduationYear.SelectedItem } },
                            new Dictionary<string, object> { { "document_id", (uint)document[0] } }, transaction);
                    }
                    else if (document[1].ToString() == "medical")
                    {
                        List<object[]> spravkaData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            });
                        if (spravkaData.Count > 0 && spravkaData[0][0].ToString() == DB_Helper.MedCertificate)
                        {
                            certificateFound = true;
                            if (!cbMedCertificate.Checked)
                                _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", (uint)document[0] } }, transaction);
                        }
                    }
                    else if (document[1].ToString() == "photos")
                    {
                        photosFound = true;
                        if (!cbPhotos.Checked)
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", (uint)document[0] } }, transaction);
                    }
                }
                if (cbMedCertificate.Checked && !certificateFound)
                {
                    SaveCertificate(transaction);
                }

                if (cbPhotos.Checked && !photosFound)
                    _DB_Connection.Insert(
                        DB_Table._APPLICATIONS_HAS_DOCUMENTS,
                        new Dictionary<string, object>
                        {
                            { "applications_id", _ApplicationID },
                            { "documents_id", _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "photos" } },transaction)}
                        },
                        transaction);
            }
        }

        private void UpdateDirections(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {

        }


        private void FillComboBox(ComboBox cb, FIS_Dictionary dictionary)
        {
            cb.Items.AddRange(_DB_Helper.GetDictionaryItems(dictionary).Values.ToArray());
            if (cb.Items.Count > 0)
                cb.SelectedIndex = 0;
        }

        private void LoadSPOProgram()
        {
            List<object[]> spoDirs = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[] { "direction_faculty", "direction_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                    new Tuple<string, Relation, object>("places_budget_o", Relation.GREATER_EQUAL, 0)
                });
            foreach (object[] dir in spoDirs)
                dgvDirection.Rows.Add((uint)dir[1], dir[0].ToString(), _DB_Helper.GetDirectionNameAndCode((uint)dir[1]).Item1, "Специалист", "Очная платная");//TODO Проверить
        }
    }
}
