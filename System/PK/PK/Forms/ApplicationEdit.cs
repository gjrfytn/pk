using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using DirTuple = System.Tuple<uint, string, string, uint, uint, string, string>; //Id, Faculty, Name (направления), EduSource, EduForm, ProfileShortName, ProfileName

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
        private string _MADIOlympName;
        private bool _Agreed;
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

            FillComboBox(cbIDDocType, FIS_Dictionary.IDENTITY_DOC_TYPE);
            FillComboBox(cbSex, FIS_Dictionary.GENDER);
            FillComboBox(cbNationality, FIS_Dictionary.COUNTRY);
            FillComboBox(cbExamsDocType, FIS_Dictionary.IDENTITY_DOC_TYPE);
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

            foreach (TabPage tab in tcDirections.Controls)
            {
                string eduFormName = "";
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
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, Classes.DB_Helper.EduSourceB);
                    }
                else if (tab.Name.Split('_')[1] == "target")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, Classes.DB_Helper.EduSourceT);
                    }
                else if (tab.Name.Split('_')[1] == "quote")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, Classes.DB_Helper.EduSourceQ);
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

        #region IDisposable Support
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _KLADR.Dispose();

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        private void cbSpecial_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbQuote.Checked)&&(!_Loading))
            {
                QuotDocs form = new QuotDocs(_DB_Connection,this);
                form.ShowDialog();
                cbMedCertificate.Enabled = true;
                foreach (Control c in tcDirections.TabPages[5].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = true;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
                foreach (Control c in tcDirections.TabPages[7].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = true;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
            }
            else if ((cbQuote.Checked) && (_Loading))
            {
                cbMedCertificate.Enabled = true;
                cbMedCertificate.Checked = true;
                foreach (Control c in tcDirections.TabPages[5].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = true;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
                foreach (Control c in tcDirections.TabPages[7].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = true;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
            }
            else
            {
                cbMedCertificate.Enabled = false;
                cbMedCertificate.Checked = false;
                foreach (Control c in tcDirections.TabPages[5].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = false;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = false;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = false;
                }
                foreach (Control c in tcDirections.TabPages[7].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = false;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = false;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = false;
                }
            }
            DirectionDocEnableDisable();
        }

        private void cbSport_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbSport.Checked)&&(!_Loading))
            {
                SportDocs form = new SportDocs(_DB_Connection,this);
                form.ShowDialog();
            }
        }

        private void cbMADIOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbMADIOlympiad.Checked) && (!_Loading))
            {
                MADIOlymps form = new MADIOlymps(_DB_Connection, _MADIOlympName);
                form.ShowDialog();
                _MADIOlympName = form.OlympName;
            }
        }

        private void cbOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbOlympiad.Checked)&&(!_Loading))
            {
                Olymps form = new Olymps(_DB_Connection,this);
                form.ShowDialog();
            }
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
                foreach (TabPage tab in tcDirections.TabPages)
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
                                achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Олимпийских игр");
                                break;
                            case "Диплом чемпиона/призера Паралимпийских игр":
                                achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Паралимпийских игр");
                                break;
                            case "Диплом чемпиона/призера Сурдлимпийских игр":
                                achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Сурдлимпийских игр");
                                break;
                            case "Диплом чемпиона мира":
                                achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Чемпион Мира");
                                break;
                            case "Диплом чемпиона Европы":
                                achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Чемпион Европы");
                                break;
                        }

                        List<object[]> achievments = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                            new List<Tuple<string, Relation, object>>
                        {
                        new Tuple<string, Relation, object> ("category_dict_id", Relation.EQUAL, (uint)FIS_Dictionary.IND_ACH_CATEGORIES),
                        new Tuple<string, Relation, object>("category_id", Relation.EQUAL, achevmentCategoryId),
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                        });

                        if (achievments.Count == 0)
                        {
                            MessageBox.Show("Спортивное достижение \"" + _DB_Helper.GetDictionaryItemName(FIS_Dictionary.IND_ACH_CATEGORIES, achevmentCategoryId) + "\" отсутствует для данной кампании в списке университета.");
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
                            || rbSpravka.Checked && !cbCertificateHRD.Checked)
                            MessageBox.Show("В разделе \"Забираемые документы\" не отмечены обязательные поля.");
                        else if (_ApplicationID == null)
                        {
                            SaveApplication();
                            btPrint.Enabled = true;
                            ChangeAgreedChBs(true);
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
            ApplicationDocsPrint form = new ApplicationDocsPrint();
            DialogResult res = form.ShowDialog();
            if (res != DialogResult.Cancel)
            {
                Dictionary<TabPage, Tuple<string, string>> streams = new Dictionary<TabPage, Tuple<string, string>>
                {
                    { tpDir_budget_o,new Tuple<string, string>("Очная (дневная) бюджетная","ОБ") },
                    { tpDir_paid_o,new Tuple<string, string>("Очная (дневная) платная","ОП") },
                    { tpDir_budget_oz,new Tuple<string, string>("Очно-заочная (вечерняя) бюджетная","ОЗБ") },
                    { tpDir_paid_oz,new Tuple<string, string>("Очно-заочная (вечерняя) платная","ОЗП") },
                    { tpDir_paid_z,new Tuple<string, string>("Заочная платная","ЗП") },
                    { tpDir_quote_o,new Tuple<string, string>("Квота, очная (дневная)","ОКВ") },
                    { tpDir_target_o,new Tuple<string, string>("Целевой прием, очная (дневная)","ОЦП") },
                    { tpDir_quote_oz,new Tuple<string, string>("Квота, очно-заочная (вечерняя)","ОЗКВ") },
                    { tpDir_target_oz,new Tuple<string, string>("Целевой прием, очно-заочная (вечерняя)","ОЗЦП") }
                };

                List<Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>> documents =
                    new List<Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>>();

                if (form.cbMoveJournal.Checked)
                    documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                        Classes.Utility.DocumentsTemplatesPath + "MoveJournal.xml",
                        _DB_Connection,
                        _ApplicationID.Value,
                        null,
                        null
                        ));

                List<string[]>[] inventoryTableParams = new List<string[]>[] { new List<string[]>(), new List<string[]>() };

                foreach (TabPage tab in tcDirections.Controls)
                    if (tab.Controls.Cast<Control>().Any(c => c.GetType() == typeof(ComboBox) && ((ComboBox)c).SelectedIndex != -1))
                        inventoryTableParams[0].Add(new string[] { streams[tab].Item1 + " форма обучения" });

                foreach (CheckBox cb in gbWithdrawDocs.Controls)
                    if (cb.Checked)
                        if (cb == cbCertificateCopy)
                        {
                            string buf = cb.Text + " " + tbEduDocSeries.Text + " " + tbEduDocNumber.Text;
                            if (cb.Text.Contains("Оригинал"))
                            {
                                DateTime origDate = (DateTime)_DB_Connection.RunProcedure("get_application_docs", _ApplicationID).Where(
                                    d => d[1].ToString() == "school_certificate" ||
                                    d[1].ToString() == "high_edu_diploma" ||
                                    d[1].ToString() == "academic_diploma"
                                    ).Single()[6];

                                inventoryTableParams[1].Add(new string[] { buf + " Дата ориг.: " + origDate.ToShortDateString() });
                            }
                            else
                                inventoryTableParams[1].Add(new string[] { buf });
                        }
                        else
                            inventoryTableParams[1].Add(new string[] { cb.Text });

                if (form.cbInventory.Checked)
                    documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                        Classes.Utility.DocumentsTemplatesPath + "Inventory.xml",
                        null,
                        null,
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
                        inventoryTableParams
                        ));

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

                foreach (string[] eduForm in inventoryTableParams[0])
                    parameters.Add(eduForm[0]);

                while (parameters.Count != 13)
                    parameters.Add("");

                if (form.cbPercRecordFace.Checked)
                    documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                        Classes.Utility.DocumentsTemplatesPath + "PercRecordFace.xml",
                        null,
                        null,
                        parameters.ToArray(),
                        null
                        ));

                List<string[]>[] receiptTableParams = new List<string[]>[2];
                receiptTableParams[0] = new List<string[]>();
                receiptTableParams[1] = inventoryTableParams[1];

                foreach (TabPage tab in tcDirections.Controls)
                {
                    var cbs = tab.Controls.Cast<Control>().Where(c => c.GetType() == typeof(ComboBox) && ((ComboBox)c).SelectedIndex != -1);
                    if (cbs.Count() != 0)
                    {
                        receiptTableParams[0].Add(new string[] { streams[tab].Item1 + " форма обучения" });
                        foreach (ComboBox cb in cbs)
                        {
                            DirTuple dir = (DirTuple)cb.SelectedValue;
                            if (tab.Name.Split('_')[1] != "paid")
                                receiptTableParams[0].Add(new string[] { "          - " + _DB_Connection.Select(
                                    DB_Table.DIRECTIONS,
                                    new string[] { "short_name" },
                                    new List<Tuple<string, Relation, object>>
                                    {
                                        new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, dir.Item2),
                                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,  dir.Item1)
                                    })[0][0].ToString()+ " (" + dir.Item2 + ") " + dir.Item3});
                            else
                                receiptTableParams[0].Add(new string[] { "          - " + dir.Item6 + " (" + dir.Item2 + ") " + dir.Item7 });
                        }
                        receiptTableParams[0].Add(new string[] { "" });
                    }
                }

                if (form.cbReceipt.Checked)
                    documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                        Classes.Utility.DocumentsTemplatesPath + "Receipt.xml",
                        null,
                        null,
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
                            DateTime.Now.ToString(), //TODO Брать из переменной
                            _EditingDateTime.ToString()
                        },
                        receiptTableParams
                        ));

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

                foreach (TabPage tab in tcDirections.Controls)
                {
                    var cbs = tab.Controls.Cast<Control>().Where(c => c.GetType() == typeof(ComboBox) && ((ComboBox)c).SelectedIndex != -1);
                    if (cbs.Count() != 0)
                    {
                        parameters.Add(streams[tab].Item2);
                        byte count = 0;
                        foreach (ComboBox cb in cbs)
                        {
                            DirTuple dir = (DirTuple)cb.SelectedValue;
                            if (tab.Name.Split('_')[1] != "paid")
                                parameters.Add(_DB_Connection.Select(
                                DB_Table.DIRECTIONS,
                                new string[] { "short_name" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL, dir.Item2),
                                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,  dir.Item1)
                                })[0][0].ToString());
                            else
                                parameters.Add(dir.Item6);

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

                if (form.cbPercRecordBack.Checked)
                    documents.Add(new Tuple<string, Classes.DB_Connector, uint?, string[], List<string[]>[]>(
                        Classes.Utility.DocumentsTemplatesPath + "PercRecordBack.xml",
                        null,
                        null,
                        parameters.ToArray(),
                        null
                        ));

                string doc = Classes.Utility.TempPath + "abitDocs";
                Classes.DocumentCreator.Create(doc, documents);
                doc += ".docx";

                if (res == DialogResult.Yes)
                    Classes.Utility.Print(doc);
                else
                    System.Diagnostics.Process.Start(doc);
            }
        }

        private void cbTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTarget.Checked && !_Loading)
            {
                TargetOrganizationSelect form = new TargetOrganizationSelect(_DB_Connection,_TargetOrganizationID);
                form.ShowDialog();
                _TargetOrganizationID = form.OrganizationID;
                foreach (Control c in tcDirections.TabPages[6].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = true;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
                foreach (Control c in tcDirections.TabPages[8].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = true;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
            }
            else if (!cbTarget.Checked && !_Loading)
            {
                foreach (Control c in tcDirections.TabPages[6].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = false;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = false;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = false;
                }
                foreach (Control c in tcDirections.TabPages[8].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = false;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = false;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = false; ;
                }
            }
            else if (_Loading)
            {
                foreach (Control c in tcDirections.TabPages[6].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = true;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
                foreach (Control c in tcDirections.TabPages[8].Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Enabled = true;
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
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
            ComboBox combo = tcDirections.TabPages[tabNumber - 1].Controls.Find("cbDirection" + tabNumber.ToString() + "1", false)[0] as ComboBox;
            if (combo.SelectedIndex == -1)
            {
                combo.Visible = true;
                combo.Enabled = true;
                Button bt = tcDirections.TabPages[tabNumber - 1].Controls.Find("btRemoveDir" + tabNumber.ToString() + "1", false)[0] as Button;
                bt.Visible = true;
                bt.Enabled = true;
                CheckBox cb = tcDirections.TabPages[tabNumber - 1].Controls.Find("cbAgreed" + tabNumber.ToString() + "1", false)[0] as CheckBox;
                cb.Visible = true;
            }
            else if ((tabNumber == 1)|| (tabNumber == 3) || (tabNumber == 7) || (tabNumber == 9))
            {
                combo = tcDirections.TabPages[tabNumber - 1].Controls.Find("cbDirection" + tabNumber.ToString() + "2", false)[0] as ComboBox;
                if (combo.SelectedIndex == -1)
                {
                    combo.Visible = true;
                    combo.Enabled = true;
                    Button bt = tcDirections.TabPages[tabNumber - 1].Controls.Find("btRemoveDir" + tabNumber.ToString() + "2", false)[0] as Button;
                    bt.Visible = true;
                    bt.Enabled = true;
                    CheckBox cb = tcDirections.TabPages[tabNumber - 1].Controls.Find("cbAgreed" + tabNumber.ToString() + "2", false)[0] as CheckBox;
                    cb.Visible = true;
                }
                else if ((tabNumber == 1) || (tabNumber == 7))
                {
                    combo = tcDirections.TabPages[tabNumber - 1].Controls.Find("cbDirection" + tabNumber.ToString() + "3", false)[0] as ComboBox;
                    if (combo.SelectedIndex == -1)
                    {
                        combo.Visible = true;
                        combo.Enabled = true;
                        Button bt = tcDirections.TabPages[tabNumber - 1].Controls.Find("btRemoveDir" + tabNumber.ToString() + "3", false)[0] as Button;
                        bt.Visible = true;
                        bt.Enabled = true;
                        CheckBox cb = tcDirections.TabPages[tabNumber - 1].Controls.Find("cbAgreed" + tabNumber.ToString() + "3", false)[0] as CheckBox;
                        cb.Visible = true;
                    }
                }
            }
        }

        private void btRemoveDir_Click(object sender, EventArgs e)
        {
            int tabNumber = int.Parse((sender as Control).Name.Substring((sender as Control).Name.Length - 2, 1));
            int comboNumber = int.Parse((sender as Control).Name.Substring((sender as Control).Name.Length - 1, 1));
            foreach (Control control in tcDirections.TabPages[tabNumber - 1].Controls)
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
        
        private void btGetIndex_Click(object sender, EventArgs e)
        {
            tbPostcode.Text = _KLADR.GetIndex(tbRegion.Text, tbDistrict.Text, tbTown.Text, "", tbStreet.Text, tbHouse.Text);
            if (tbPostcode.Text == "")
                tbPostcode.Text = _KLADR.GetIndex(tbRegion.Text, tbDistrict.Text, "", tbTown.Text, tbStreet.Text, tbHouse.Text);
        }

        private void SaveApplication()
        {
            Cursor.Current = Cursors.WaitCursor;
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

        private void SaveBasic()
        {
            _EntrantID = _DB_Connection.Insert(DB_Table.ENTRANTS, new Dictionary<string, object> { { "email", mtbEMail.Text }, { "home_phone", mtbHomePhone.Text }, { "mobile_phone", mtbMobilePhone.Text } });
            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;

            _ApplicationID = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "entrant_id", _EntrantID.Value}, { "registration_time", DateTime.Now},
                { "needs_hostel", cbHostelNeeded.Checked}, { "registrator_login", _RegistratorLogin}, { "campaign_id", _CurrCampainID },
                { "status_dict_id", (uint)FIS_Dictionary.APPLICATION_STATUS},{ "status_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.APPLICATION_STATUS, "Новое")}, { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "first_high_edu", firstHightEdu}, { "mcado", cbMCDAO.Checked}, { "chernobyl", cbChernobyl.Checked}, { "passing_examinations",cbExams.Checked },
                { "priority_right", cbPrerogative.Checked}, { "special_conditions", cbSpecialConditions.Checked}, { "master_appl", false} });

            uint idDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "identity" },
                { "series", tbIDDocSeries.Text}, { "number", tbIDDocNumber.Text}, { "date", dtpIDDocDate.Value} , { "organization", tbIssuedBy.Text} });

            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID},
                { "documents_id", idDocUid } });

            _DB_Connection.Insert(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", idDocUid},
                { "last_name", tbLastName.Text}, { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},
                { "gender_dict_id", (uint)FIS_Dictionary.GENDER},{ "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER, cbSex.SelectedItem.ToString())},
                { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY}, { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", tbRegion.Text}, { "reg_district", tbDistrict.Text},
                { "reg_town", tbTown.Text}, { "reg_street", tbStreet.Text}, { "reg_house", tbHouse.Text}, { "reg_index", tbPostcode.Text}, { "reg_flat", tbAppartment.Text} });

            if (cbPhotos.Checked)
            {
                uint otherDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "photos" } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", otherDocID } });
            }
        }

        private void SaveDiploma()
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
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", eduDocID }, { "year", cbGraduationYear.SelectedItem } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", eduDocID } });
            }
            else
            {
                eduDocID = (int)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", eduDocType }, { "series",  tbEduDocSeries.Text},
                    { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}  }));
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", eduDocID }, { "year", cbGraduationYear.SelectedItem } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", eduDocID } });
            }
            if (cbMedal.Checked)
                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                    { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.MedalAchievement)),
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                    })[0][0] }, { "document_id", eduDocID } });
        }

        private void SaveQuote()
        {
            if (QuoteDoc.cause == "Сиротство")
            {
                uint orphDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "orphan" },
                    { "date", QuoteDoc.orphanhoodDocDate} , { "organization", QuoteDoc.orphanhoodDocOrg} });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", orphDocUid},
                        { "name", QuoteDoc.orphanhoodDocName}, { "dictionaries_dictionary_id", (uint)FIS_Dictionary.ORPHAN_DOC_TYPE},
                        { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.ORPHAN_DOC_TYPE, QuoteDoc.orphanhoodDocType)} });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", orphDocUid } });
                _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                            { "reason_document_id", orphDocUid},{ "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } });
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
                            { "document_id", medDocUid}, { "dictionaries_dictionary_id",(uint)FIS_Dictionary.DISABILITY_GROUP},
                            { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DISABILITY_GROUP,QuoteDoc.disabilityGroup)} });
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", medDocUid } });
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности")},
                            { "reason_document_id", medDocUid},{ "allow_education_document_id", allowEducationDocUid}, { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } });
                }
                else if (QuoteDoc.medCause == "Заключение психолого-медико-педагогической комиссии")
                {
                    uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "medical" },
                        { "series", QuoteDoc.medDocSerie},  { "number", QuoteDoc.medDocNumber} });
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", medDocUid } });
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID( FIS_Dictionary.DOCUMENT_TYPE, "Заключение психолого-медико-педагогической комиссии")},
                            { "reason_document_id", medDocUid},{ "allow_education_document_id", allowEducationDocUid}, { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } });
                }
            }
        }

        private void SaveSport()
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
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Олимпийских игр");
                    break;
                case "Диплом чемпиона/призера Паралимпийских игр":
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Паралимпийских игр");
                    break;
                case "Диплом чемпиона/призера Сурдлимпийских игр":
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Сурдлимпийских игр");
                    break;
                case "Диплом чемпиона мира":
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Чемпион Мира");
                    break;
                case "Диплом чемпиона Европы":
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Чемпион Европы");
                    break;
            }

            List<object[]> achievments = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                new List<Tuple<string, Relation, object>>
            {
                    new Tuple<string, Relation, object> ("category_dict_id", Relation.EQUAL, (uint)FIS_Dictionary.IND_ACH_CATEGORIES),
                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, achevmentCategoryId)
            });

            uint achievementUid = 0;
            if (achievments.Count != 0)
                achievementUid = uint.Parse(achievments[0][0].ToString());

            _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                    { "institution_achievement_id", achievementUid}, { "document_id", sportDocUid} });
        }

        private void SaveOlympic()
        {
            if (cbOlympiad.Checked)
            {
                string docType = "";
                uint benefitDocType = 0;
                uint olympicDocId = 0;
                switch (OlympicDoc.olympType)
                {
                    case "Диплом победителя/призера олимпиады школьников":
                        docType = "olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера олимпиады школьников");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                    { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) },
                    { "olympic_id", (uint)OlympicDoc.olympID},
                    { "class_number", OlympicDoc.olympClass }, { "olympic_profile", OlympicDoc.olympProfile },
                    { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES }, { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) },
                    { "olympic_subject_dict_id", (uint)FIS_Dictionary.SUBJECTS }, { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, OlympicDoc.olympDist) } });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;

                    case "Диплом победителя/призера всероссийской олимпиады школьников":
                        docType = "olympic_total";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера всероссийской олимпиады школьников");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                    { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) },
                    { "olympic_id", (uint)OlympicDoc.olympID},
                    { "class_number", OlympicDoc.olympClass }, { "olympic_profile", OlympicDoc.olympProfile },
                    { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES }, { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) },
                    { "olympic_subject_dict_id", (uint)FIS_Dictionary.SUBJECTS }, { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, OlympicDoc.olympDist) } });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;

                    case "Диплом 4 этапа всеукраинской олимпиады":
                        docType = "ukraine_olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера IV  всеукраинской ученической олимпиады");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                    { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) }, { "olympic_name", OlympicDoc.olympName }, { "olympic_profile", OlympicDoc.olympProfile },
                    { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES }, { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) }});
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;

                    case "Диплом международной олимпиады":
                        docType = "international_olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Документ об участии в международной олимпиаде");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "olympic_name", OlympicDoc.olympName },
                            { "olympic_profile", OlympicDoc.olympProfile },
                    { "country_dict_id", (uint)FIS_Dictionary.COUNTRY }, { "country_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY, OlympicDoc.country) },
                    { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES }, { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) } });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;
                }
            }
            if (cbMADIOlympiad.Checked)
            {
                uint olympDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "custom" } });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "name", Classes.DB_Helper.MADIOlympDocName },
                    { "text_data", _MADIOlympName}, { "document_id", olympDocID } });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympDocID } });
                List<object[]> achievments = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                    new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("category_dict_id", Relation.EQUAL, (uint)FIS_Dictionary.IND_ACH_CATEGORIES),
                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.OlympAchievementName))
                });

                uint achievementId = 0;
                if (achievments.Count != 0)
                    achievementId = (uint)(achievments[0][0]);

                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                    { "institution_achievement_id", achievementId}, { "document_id", olympDocID} });
            }
        }

        private void SaveExams()
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
                        { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS} , { "subject_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.SUBJECTS, row.Cells[0].Value.ToString())} ,
                        { "value", row.Cells[3].Value} });
            }
        }

        private void SaveDirections()
        {
            foreach (TabPage tab in tcDirections.Controls)
            {
                if ((tab.Name.Split('_')[1] != "paid") && (tab.Name.Split('_')[1] != "target"))
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                    { "faculty_short_name", ((DirTuple)cb.SelectedValue).Item2 },
                                    { "direction_id", ((DirTuple)cb.SelectedValue).Item1},
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM}, { "edu_form_id", ((DirTuple)cb.SelectedValue).Item5},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE}, { "edu_source_id", ((DirTuple)cb.SelectedValue).Item4} });                                
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
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                        { "faculty_short_name",  ((DirTuple)cb.SelectedValue).Item2 },
                                    { "direction_id",((DirTuple)cb.SelectedValue).Item1 },
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM}, { "edu_form_id", ((DirTuple)cb.SelectedValue).Item5},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE}, { "edu_source_id", ((DirTuple)cb.SelectedValue).Item4},
                                    { "profile_short_name", ((DirTuple)cb.SelectedValue).Item6},
                                    { "profile_actual", true} });
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
                                     { "faculty_short_name",  ((DirTuple)cb.SelectedValue).Item2 },
                                    { "direction_id", ((DirTuple)cb.SelectedValue).Item1},
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM}, { "edu_form_id", ((DirTuple)cb.SelectedValue).Item5},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE}, { "edu_source_id", ((DirTuple)cb.SelectedValue).Item4},
                                    { "target_organization_id", _TargetOrganizationID} });
                            }
                    }
                }
            }
        }

        private void LoadBasic()
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

            object[] entrant = _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, new string[] { "last_name", "first_name", "middle_name" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("id", Relation.EQUAL, application[8])
            })[0];
            tbLastName.Text = entrant[0].ToString();
            tbFirstName.Text = entrant[1].ToString();
            tbMidleName.Text = entrant[2].ToString();

            entrant = _DB_Connection.Select(DB_Table.ENTRANTS, new string[] { "email", "home_phone", "mobile_phone" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, application[8])
                })[0];
            mtbEMail.Text = entrant[0].ToString();
            mtbHomePhone.Text = entrant[1].ToString();
            mtbMobilePhone.Text = entrant[2].ToString();

            if (_DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.MedalAchievement)),
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                })[0][0])
            }).Count > 0)
                cbMedal.Checked = true;
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
                        "birth_date", "birth_place", "reg_region", "reg_district", "reg_town", "reg_street", "reg_house", "reg_index", "reg_flat", "gender_id" }, new List<Tuple<string, Relation, object>>
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
                    tbHouse.Text = passport[9].ToString();
                    tbPostcode.Text = passport[10].ToString();
                    tbAppartment.Text = passport[11].ToString();
                    cbSex.SelectedItem = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.GENDER, (uint)passport[12]);
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
                    if (document[6] as DateTime? != null)
                        cbOriginal.Checked = true;
                    if (document[5].ToString().Split('|').Count() > 1)
                    {
                        cbInstitutionType.SelectedItem = document[5].ToString().Split('|')[0];
                        tbInstitutionNumber.Text = document[5].ToString().Split('|')[1];
                        tbInstitutionLocation.Text = document[5].ToString().Split('|')[2];
                    }
                    cbGraduationYear.SelectedItem = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "year" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, document[0])
                    })[0][0];
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
                            if (row.Cells[0].Value.ToString() == _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)subject[0]))
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
                    string achievementName = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.IND_ACH_CATEGORIES, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" }, new List<Tuple<string, Relation, object>>
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
                        new Tuple<string, Relation, object>("dictionaries_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.ORPHAN_DOC_TYPE)
                    })[0];
                    QuoteDoc.orphanhoodDocName = orphanDoc[0].ToString();
                    QuoteDoc.orphanhoodDocType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.ORPHAN_DOC_TYPE, (uint)orphanDoc[1]);
                }
                else if (document[1].ToString() == "disability")
                {
                    QuoteDoc.cause = "Медицинские показатели";
                    QuoteDoc.medCause = "Справква об установлении инвалидности";
                    cbQuote.Checked = true;
                    QuoteDoc.medDocSerie = int.Parse(document[2].ToString());
                    QuoteDoc.medDocNumber = int.Parse(document[3].ToString());
                    QuoteDoc.disabilityGroup = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DISABILITY_GROUP, (uint)_DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "dictionaries_item_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL,(uint)document[0]),
                            new Tuple<string, Relation, object>("dictionaries_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.DISABILITY_GROUP)
                        })[0][0]);
                    object[] allowDocument = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "number", "date" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    (uint)(_DB_Connection.Select(DB_Table.APPLICATION_COMMON_BENEFITS, new string[] { "allow_education_document_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности"))
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
                        new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Заключение психолого-медико-педагогической комиссии"))
                    }))[0][0])})[0];
                    QuoteDoc.conclusionNumber = int.Parse(allowDocument[0].ToString());
                    QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                }
                else if (document[1].ToString() == "olympic")
                {
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    foreach (object[] olympDocData in _DB_Connection.Select(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new string[] { "diploma_type_id", "olympic_id", "class_number",
                        "olympic_profile", "olympic_subject_id" }))
                    {
                        OlympicDoc.diplomaType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DIPLOMA_TYPE, (uint)olympDocData[0]);
                        OlympicDoc.olympID = int.Parse(olympDocData[1].ToString());
                        OlympicDoc.olympClass = int.Parse(olympDocData[2].ToString());
                        OlympicDoc.olympProfile = olympDocData[3].ToString();
                        OlympicDoc.olympDist = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)olympDocData[4]);
                        OlympicDoc.olympType = "Диплом победителя/призера олимпиады школьников";
                    }
                    cbOlympiad.Checked = true;
                }
                else if (document[1].ToString() == "olympic_total")
                {
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    foreach (object[] olympDocData in _DB_Connection.Select(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new string[] { "diploma_type_id", "olympic_id", "class_number",
                        "olympic_profile", "olympic_subject_id" }))
                    {
                        OlympicDoc.diplomaType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DIPLOMA_TYPE, (uint)olympDocData[0]);
                        OlympicDoc.olympID = int.Parse(olympDocData[1].ToString());
                        OlympicDoc.olympClass = int.Parse(olympDocData[2].ToString());
                        OlympicDoc.olympProfile = olympDocData[3].ToString();
                        OlympicDoc.olympDist = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)olympDocData[4]);
                        OlympicDoc.olympType = "Диплом победителя/призера всероссийской олимпиады школьников";
                    }
                    cbOlympiad.Checked = true;
                }
                else if (document[1].ToString() == "ukraine_olympic")
                {
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    foreach (object[] olympDocData in _DB_Connection.Select(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new string[] { "diploma_type_id", "class_number",
                        "olympic_profile", "olympic_subject_id", "olympic_name" }))
                    {
                        OlympicDoc.diplomaType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DIPLOMA_TYPE, (uint)olympDocData[0]);
                        OlympicDoc.olympClass = int.Parse(olympDocData[1].ToString());
                        OlympicDoc.olympProfile = olympDocData[2].ToString();
                        OlympicDoc.olympDist = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)olympDocData[3]);
                        OlympicDoc.olympName = olympDocData[4].ToString();
                        OlympicDoc.olympType = "Диплом 4 этапа всеукраинской олимпиады";
                    }
                    cbOlympiad.Checked = true;
                }
                else if (document[1].ToString() == "international_olympic")
                {
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    foreach (object[] olympDocData in _DB_Connection.Select(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new string[] { "class_number",
                        "olympic_profile", "olympic_subject_id", "olympic_name", "country_id" }))
                    {
                        OlympicDoc.olympClass = int.Parse(olympDocData[0].ToString());
                        OlympicDoc.olympProfile = olympDocData[1].ToString();
                        OlympicDoc.olympDist = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)olympDocData[2]);
                        OlympicDoc.olympName = olympDocData[3].ToString();
                        OlympicDoc.country = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.COUNTRY, (uint)olympDocData[4]);
                        OlympicDoc.olympType = "Диплом международной олимпиады";
                    }
                    cbOlympiad.Checked = true;
                }
                else if (document[1].ToString() == "custom")
                {
                    foreach (object[] documentData in _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name", "text_data" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    }))
                        if (documentData[0].ToString() == Classes.DB_Helper.MADIOlympDocName)
                        {
                            _MADIOlympName = documentData[1].ToString();
                            cbMADIOlympiad.Checked = true;
                        }
                }
            }
        }

        private void LoadDirections()
        {
            string[][] eduLevelsCodes = new string[][] { new string[] { "03", Classes.DB_Helper.EduLevelB }, new string[] { "04", Classes.DB_Helper.EduLevelM }, new string[] { "05", Classes.DB_Helper.EduLevelS } };
            var directionsData = _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code");
            var profilesData = _DB_Connection.Select(DB_Table.PROFILES, new string[] { "short_name", "name" });
            var appEntrances = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "direction_id", "faculty_short_name", "is_agreed_date",
            "profile_short_name", "edu_form_id", "edu_source_id", "target_organization_id"}, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID)
            });

            var profsRecords = appEntrances.Join(
                profilesData,
                campDirs => campDirs[3],
                profsData => profsData[0],
                (s1, s2) => new
                {
                    Id = (uint)s1[0],
                    Faculty = s1[1].ToString(),
                    Level = eduLevelsCodes.First(x => x[0] == _DB_Helper.GetDirectionNameAndCode((uint)s1[0]).Item2.ToString().Split('.')[1])[1],
                    Name = _DB_Helper.GetDirectionNameAndCode((uint)s1[0]).Item1,
                    EduSource = (uint)s1[5],
                    EduForm = (uint)s1[4],
                    ProfileShortName = s1[3].ToString(),
                    ProfileName = s2[1].ToString()
                }).Select(s => new
                {
                    Value = new DirTuple(s.Id, s.Faculty, s.Name, s.EduSource, s.EduForm, s.ProfileShortName, s.ProfileName),
                    Display = "(" + s.Faculty + ", " + s.Level + ") " + s.Name
                }).ToList();

            var dirsRecords = appEntrances.Join(
                directionsData,
                appEnt => appEnt[0],
                dirsData => dirsData[0],
                (s1, s2) => new
                {
                    Id = (uint)s1[0],
                    Faculty = s1[1].ToString(),
                    Level = eduLevelsCodes.First(x => x[0] == s2[2].ToString().Split('.')[1])[1],
                    Name = s2[1].ToString(),
                    EduSource = (uint)s1[5],
                    EduForm = (uint)s1[4],
                }).Select(s => new
                {
                    Value = new DirTuple(s.Id, s.Faculty, s.Name, s.EduSource, s.EduForm, "", ""),
                    Display = "(" + s.Faculty + ", " + s.Level + ") " + s.Name
                }).ToList();

            var records = dirsRecords.Concat(profsRecords);
            string agreedCbName = "";
            DateTime? agreedDate = DateTime.MinValue;

            foreach (var entrancesData in records)
            {
                if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceB))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    if (cbDirection11.SelectedIndex == -1)
                    {
                        cbDirection11.SelectedValue = entrancesData.Value;
                        cbDirection11.Visible = true;
                        cbDirection11.Enabled = true;
                        btRemoveDir11.Visible = true;
                        btRemoveDir11.Enabled = true;
                        cbAgreed11.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed11";
                            agreedDate = apEntrData;
                        }
                    }
                    else if (cbDirection12.SelectedIndex == -1)
                    {
                        cbDirection12.SelectedValue = entrancesData.Value;
                        cbDirection12.Visible = true;
                        cbDirection12.Enabled = true;
                        btRemoveDir12.Visible = true;
                        btRemoveDir12.Enabled = true;
                        cbAgreed12.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed12";
                            agreedDate = apEntrData;
                        }
                    }
                    else if (cbDirection13.SelectedIndex == -1)
                    {
                        cbDirection13.SelectedValue = entrancesData.Value;
                        cbDirection13.Visible = true;
                        cbDirection13.Enabled = true;
                        btRemoveDir13.Visible = true;
                        btRemoveDir13.Enabled = true;
                        cbAgreed13.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed13";
                            agreedDate = apEntrData;
                        }
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    cbDirection21.SelectedValue = entrancesData.Value;
                    cbDirection21.Visible = true;
                    cbDirection21.Enabled = true;
                    btRemoveDir21.Visible = true;
                    btRemoveDir21.Enabled = true;
                    cbAgreed21.Visible = true;
                    if ((apEntrData != null) && (apEntrData > agreedDate))
                    {
                        agreedCbName = "cbAgreed21";
                        agreedDate = apEntrData;
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceB))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    if (cbDirection31.SelectedIndex == -1)
                    {
                        cbDirection31.SelectedValue = entrancesData.Value;
                        cbDirection31.Visible = true;
                        cbDirection31.Enabled = true;
                        btRemoveDir31.Visible = true;
                        btRemoveDir31.Enabled = true;
                        cbAgreed31.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed31";
                            agreedDate = apEntrData;
                        }
                    }
                    else if (cbDirection32.SelectedIndex == -1)
                    {
                        cbDirection32.SelectedValue = entrancesData.Value;
                        cbDirection32.Visible = true;
                        cbDirection32.Enabled = true;
                        btRemoveDir32.Visible = true;
                        btRemoveDir32.Enabled = true;
                        cbAgreed32.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed32";
                            agreedDate = apEntrData;
                        }
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    cbDirection41.SelectedValue = entrancesData.Value;
                    cbDirection41.Visible = true;
                    cbDirection41.Enabled = true;
                    btRemoveDir41.Visible = true;
                    btRemoveDir41.Enabled = true;
                    cbAgreed41.Visible = true;
                    if ((apEntrData != null) && (apEntrData > agreedDate))
                    {
                        agreedCbName = "cbAgreed41";
                        agreedDate = apEntrData;
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormZ)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    cbDirection51.SelectedValue = entrancesData.Value;
                    cbDirection51.Visible = true;
                    cbDirection51.Enabled = true;
                    btRemoveDir51.Visible = true;
                    btRemoveDir51.Enabled = true;
                    cbAgreed51.Visible = true;
                    if ((apEntrData != null) && (apEntrData > agreedDate))
                    {
                        agreedCbName = "cbAgreed51";
                        agreedDate = apEntrData;
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    cbQuote.Checked = true;
                    if (cbDirection61.SelectedIndex == -1)
                    {
                        cbDirection61.SelectedValue = entrancesData.Value;
                        cbDirection61.Visible = true;
                        cbDirection61.Enabled = true;
                        btRemoveDir61.Visible = true;
                        btRemoveDir61.Enabled = true;
                        cbAgreed61.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed61";
                            agreedDate = apEntrData;
                        }
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    cbTarget.Checked = true;
                    _TargetOrganizationID = (uint)_DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "target_organization_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, entrancesData.Value.Item2),
                        new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, entrancesData.Value.Item1),
                        new Tuple<string, Relation, object>("edu_form_id", Relation.EQUAL, entrancesData.Value.Item5),
                        new Tuple<string, Relation, object>("edu_source_id", Relation.EQUAL, entrancesData.Value.Item4),
                    })[0][0];
                    if (cbDirection71.SelectedIndex == -1)
                    {
                        cbDirection71.SelectedValue = entrancesData.Value;
                        cbDirection71.Visible = true;
                        cbDirection71.Enabled = true;
                        btRemoveDir71.Visible = true;
                        btRemoveDir71.Enabled = true;
                        cbAgreed71.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed71";
                            agreedDate = apEntrData;
                        }
                    }
                    else if (cbDirection72.SelectedIndex == -1)
                    {
                        cbDirection72.SelectedValue = entrancesData.Value;
                        cbDirection72.Visible = true;
                        cbDirection72.Enabled = true;
                        btRemoveDir72.Visible = true;
                        btRemoveDir72.Enabled = true;
                        cbAgreed72.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed72";
                            agreedDate = apEntrData;
                        }
                    }
                    else if (cbDirection73.SelectedIndex == -1)
                    {
                        cbDirection73.SelectedValue = entrancesData.Value;
                        cbDirection73.Visible = true;
                        cbDirection73.Enabled = true;
                        btRemoveDir73.Visible = true;
                        btRemoveDir73.Enabled = true;
                        cbAgreed73.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed73";
                            agreedDate = apEntrData;
                        }
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    cbQuote.Checked = true;
                    if (cbDirection81.SelectedIndex == -1)
                    {
                        cbDirection81.SelectedValue = entrancesData.Value;
                        cbDirection81.Visible = true;
                        cbDirection81.Enabled = true;
                        btRemoveDir81.Visible = true;
                        btRemoveDir81.Enabled = true;
                        cbAgreed81.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed81";
                            agreedDate = apEntrData;
                        }
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
                    DateTime? apEntrData = (appEntrances.Find(x => ((uint)x[0] == entrancesData.Value.Item1) && (x[1].ToString() == entrancesData.Value.Item2)
                    && ((uint)x[4] == entrancesData.Value.Item5) && ((uint)x[5] == entrancesData.Value.Item4))[2] as DateTime?);

                    cbTarget.Checked = true;
                    _TargetOrganizationID = (uint)_DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "target_organization_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, entrancesData.Value.Item2),
                        new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, entrancesData.Value.Item1),
                        new Tuple<string, Relation, object>("edu_form_id", Relation.EQUAL, entrancesData.Value.Item5),
                        new Tuple<string, Relation, object>("edu_source_id", Relation.EQUAL, entrancesData.Value.Item4),
                    })[0][0];
                    if (cbDirection91.SelectedIndex == -1)
                    {
                        cbDirection91.SelectedValue = entrancesData.Value;
                        cbDirection91.Visible = true;
                        cbDirection91.Enabled = true;
                        btRemoveDir91.Visible = true;
                        btRemoveDir91.Enabled = true;
                        cbAgreed91.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed91";
                            agreedDate = apEntrData;
                        }
                    }
                    else if (cbDirection92.SelectedIndex == -1)
                    {
                        cbDirection92.SelectedValue = entrancesData.Value;
                        cbDirection92.Visible = true;
                        cbDirection92.Enabled = true;
                        btRemoveDir92.Visible = true;
                        btRemoveDir92.Enabled = true;
                        cbAgreed92.Visible = true;
                        if ((apEntrData != null) && (apEntrData > agreedDate))
                        {
                            agreedCbName = "cbAgreed92";
                            agreedDate = apEntrData;
                        }
                    }
                }
            }
            if (agreedCbName == "")
                ChangeAgreedChBs(true);
            else
                foreach (TabPage page in tcDirections.TabPages)
                    foreach (Control control in page.Controls)
                    {
                        CheckBox cb = control as CheckBox;
                        if ((cb != null) && (cb.Name == agreedCbName))
                            cb.Checked = true;
                    }
        }

        private void UpdateBasic()
        {
            _DB_Connection.Update(DB_Table.ENTRANTS, new Dictionary<string, object> { { "email", mtbEMail.Text }, { "home_phone", mtbHomePhone.Text }, { "mobile_phone", mtbMobilePhone.Text } },
    new Dictionary<string, object> { { "id", _EntrantID } });

            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;
            Random randNumber = new Random();

            _DB_Connection.Update(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "needs_hostel", cbHostelNeeded.Checked}, { "edit_time", _EditingDateTime},
                { "status_dict_id", (uint)FIS_Dictionary.APPLICATION_STATUS}, { "status_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.APPLICATION_STATUS, "Новое")}, { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "first_high_edu", firstHightEdu}, { "mcado", cbMCDAO.Checked}, { "chernobyl", cbChernobyl.Checked}, { "passing_examinations",cbExams.Checked },
                { "priority_right", cbPrerogative.Checked}, { "special_conditions", cbSpecialConditions.Checked} }, new Dictionary<string, object> { { "id", _ApplicationID } });
        }

        private void UpdateDocuments()
        {
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
                            { "gender_dict_id", (uint)FIS_Dictionary.GENDER},{ "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER,cbSex.SelectedItem.ToString())},
                            { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                            { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                            { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY}, { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                            { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", tbRegion.Text}, { "reg_district", tbDistrict.Text},
                            { "reg_town", tbTown.Text}, { "reg_street", tbStreet.Text}, { "reg_house", tbHouse.Text}, { "reg_flat", tbAppartment.Text}, { "reg_index", tbPostcode.Text} },
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

                        if ((document[6] as DateTime?) != null && (cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } });
                        else if ((document[6] as DateTime?) != null && (!cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "original_recieved_date", null }, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } });
                        else if ((document[6] as DateTime?) != null && (cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series",  tbEduDocSeries.Text}, { "original_recieved_date", DateTime.Now }, { "type", eduDocType },
                                { "number", tbEduDocNumber.Text}, { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text + "|" + tbInstitutionLocation.Text}},
                            new Dictionary<string, object> { { "id", (uint)document[0] } });

                        _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "year", cbGraduationYear.SelectedItem } },
                            new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                        if ((cbMedal.Checked) && (_DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                                new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.MedalAchievement)),
                                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                                })[0][0])
                            }).Count == 0))
                            _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                                { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.MedalAchievement)),
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
                                newData.Add(new object[] { (uint)document[0], 1, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, row.Cells[0].Value.ToString()), row.Cells[3].Value });
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
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Олимпийских игр");
                                    break;
                                case "Диплом чемпиона/призера Паралимпийских игр":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Паралимпийских игр");
                                    break;
                                case "Диплом чемпиона/призера Сурдлимпийских игр":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Статус чемпиона и призера Сурдлимпийских игр");
                                    break;
                                case "Диплом чемпиона мира":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Чемпион Мира");
                                    break;
                                case "Диплом чемпиона Европы":
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, "Чемпион Европы");
                                    break;
                            }
                            uint achevmentCategoryIdOld = (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" }, new List<Tuple<string, Relation, object>>
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
                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", (uint)document[0] } });
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

                            _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "name", QuoteDoc.orphanhoodDocName}, { "dictionaries_dictionary_id", (uint)FIS_Dictionary.ORPHAN_DOC_TYPE},
                            { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.ORPHAN_DOC_TYPE, QuoteDoc.orphanhoodDocType)}}, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                            _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                                { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                                { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } },
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

                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {{ "dictionaries_dictionary_id",(uint)FIS_Dictionary.DISABILITY_GROUP},
                                { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DISABILITY_GROUP,QuoteDoc.disabilityGroup)} }, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                                _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                                    { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности")}, { "allow_education_document_id", allowEducationDocUid},
                                    { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } },
                                    new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                            }
                            else if (document[1].ToString() == "medical")
                            {
                                qouteFound = true;
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", QuoteDoc.medDocSerie }, { "number", QuoteDoc.medDocNumber } },
                                    new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                                _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                                    { "document_type_id",  _DB_Helper.GetDictionaryItemID( FIS_Dictionary.DOCUMENT_TYPE, "Заключение психолого-медико-педагогической комиссии")}, { "allow_education_document_id", allowEducationDocUid},
                                    { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } },
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
                    else if (document[1].ToString() == "custom")
                    {
                        if (cbMADIOlympiad.Checked)
                        {
                            foreach (object[] documentData in _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name", "text_data" },
                                new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            }))
                                if (documentData[0].ToString() == Classes.DB_Helper.MADIOlympDocName)
                                {
                                    _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "text_data", _MADIOlympName } },
                                        new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                                    MADIOlympFound = true;
                                }
                        }
                        else
                        {
                            List<object[]> achievments = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                                new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("category_dict_id", Relation.EQUAL, (uint)FIS_Dictionary.IND_ACH_CATEGORIES),
                                new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.OlympAchievementName))
                            });

                            uint achievementId = 0;
                            if (achievments.Count != 0)
                                achievementId = (uint)(achievments[0][0]);

                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "documents_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", (uint)document[0] } });
                            _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "institution_achievement_id", achievementId } });
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
        }

        private void UpdateDirections()
        {
            List<object[]> oldD = new List<object[]>();
            List<object[]> newD = new List<object[]>();
            string[] fieldsList = new string[] { "application_id", "faculty_short_name", "direction_id", "edu_form_dict_id", "edu_form_id",
                "edu_source_dict_id", "edu_source_id", "profile_short_name", "target_organization_id", "profile_actual" };
            foreach (object[] record in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, fieldsList, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object> ("application_id", Relation.EQUAL, _ApplicationID)
            }))
                oldD.Add(record);

            foreach (TabPage tab in tcDirections.Controls)
            {
                if ((tab.Name.Split('_')[1] != "paid") && (tab.Name.Split('_')[1] != "target"))
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            if (cb.SelectedIndex != -1)
                            {
                                newD.Add(new object[] { _ApplicationID , ((DirTuple)cb.SelectedValue).Item2,
                                    ((DirTuple)cb.SelectedValue).Item1,
                                    (uint)FIS_Dictionary.EDU_FORM, ((DirTuple)cb.SelectedValue).Item5,
                                    (uint)FIS_Dictionary.EDU_SOURCE, ((DirTuple)cb.SelectedValue).Item4,
                                    null, null, false});
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
                                newD.Add(new object[] { _ApplicationID , ((DirTuple)cb.SelectedValue).Item2,
                                    ((DirTuple)cb.SelectedValue).Item1,
                                    (uint)FIS_Dictionary.EDU_FORM, ((DirTuple)cb.SelectedValue).Item5,
                                    (uint)FIS_Dictionary.EDU_SOURCE, ((DirTuple)cb.SelectedValue).Item4,
                                    ((DirTuple)cb.SelectedValue).Item6, null, true});
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
                                newD.Add(new object[] { _ApplicationID , ((DirTuple)cb.SelectedValue).Item2,
                                    ((DirTuple)cb.SelectedValue).Item1,
                                    (uint)FIS_Dictionary.EDU_FORM, ((DirTuple)cb.SelectedValue).Item5,
                                    (uint)FIS_Dictionary.EDU_SOURCE, ((DirTuple)cb.SelectedValue).Item4,
                                    null, _TargetOrganizationID, false});
                            }
                    }
                }
            }
            UpdateData(DB_Table.APPLICATIONS_ENTRANCES, oldD, newD, fieldsList, false, new string[] { "application_id", "faculty_short_name", "direction_id", "edu_form_dict_id", "edu_form_id",
                "edu_source_dict_id", "edu_source_id" });
            foreach (TabPage page in tcDirections.Controls)
            {
                uint eduForm = 0;
                uint eduSource = 0;
                if (page.Name.Split('_')[1] == "budget")
                    eduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceB);
                if (page.Name.Split('_')[1] == "paid")
                    eduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP);
                if (page.Name.Split('_')[1] == "target")
                    eduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT);
                if (page.Name.Split('_')[1] == "quote")
                    eduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ);

                if (page.Name.Split('_')[2] == "o")
                    eduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO);
                if (page.Name.Split('_')[2] == "oz")
                    eduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ);
                if (page.Name.Split('_')[2] == "z")
                    eduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormZ);

                foreach (Control control in page.Controls)
                {
                    CheckBox cb = control as CheckBox;
                    if ((cb != null) && (cb.Checked))
                    {
                        foreach (Control c in page.Controls)
                        {
                            ComboBox comboBox = c as ComboBox;
                            if ((c != null) && (c.Name == "cbDirection" + cb.Name.Substring(8)) && (((ComboBox)c).SelectedIndex != -1))
                                foreach (object[] record in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "faculty_short_name", "direction_id", "edu_form_id", "edu_source_id", "is_agreed_date" }))
                                {
                                    if (((uint)record[2] == eduForm) && ((uint)record[3] == eduSource) && (record[0].ToString() == ((DirTuple)((ComboBox)c).SelectedValue).Item2)
                                        && ((uint)record[1] == ((DirTuple)((ComboBox)c).SelectedValue).Item1) && (record[4] as DateTime? == null))
                                        _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "is_agreed_date", DateTime.Now } },
                                            new Dictionary<string, object> { { "faculty_short_name", record[0].ToString() }, { "direction_id", (uint)record[1] }, { "edu_form_id", (uint)record[2] },
                                                { "edu_source_id", (uint)record[3] } });
                                }
                        }
                    }
                }
            }
        }

        private void FillComboBox(ComboBox cb, FIS_Dictionary dictionary)
        {
            cb.Items.AddRange(_DB_Helper.GetDictionaryItems(dictionary).Values.ToArray());
            if (cb.Items.Count > 0)
                cb.SelectedIndex = 0;
        }

        private void FillDirectionsProfilesCombobox(ComboBox combobox, bool isProfileList, string eduForm, string eduSource)
        {
            string[][] eduLevelsCodes = new string[][] { new string[] { "03", Classes.DB_Helper.EduLevelB }, new string[] { "04", Classes.DB_Helper.EduLevelM }, new string[] { "05", Classes.DB_Helper.EduLevelS } };

            var directionsData = _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code");

            if (isProfileList && eduSource != Classes.DB_Helper.EduSourceT)
            {
                string placesCountColumnName = "";
                if (eduForm == Classes.DB_Helper.EduFormO)
                    placesCountColumnName = "places_paid_o";
                else if (eduForm == Classes.DB_Helper.EduFormOZ)
                    placesCountColumnName = "places_paid_oz";
                else if (eduForm == Classes.DB_Helper.EduFormZ)
                    placesCountColumnName = "places_paid_z";

                var profilesData = _DB_Connection.Select(DB_Table.PROFILES, new string[] { "short_name", "name" });
                var selectedDirs = _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_id", "profiles_direction_faculty", "profiles_short_name", placesCountColumnName },
                   new List<Tuple<string, Relation, object>>
                       {
                            new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CurrCampainID),
                            new Tuple<string, Relation, object>(placesCountColumnName, Relation.GREATER, 0)
                       }).Join(
                    profilesData,
                    campProfiles => campProfiles[2],
                    profData => profData[0],
                    (s1, s2) => new
                    {
                        Id = (uint)s1[0],
                        Faculty = s1[1].ToString(),
                        Name = _DB_Helper.GetDirectionNameAndCode((uint)s1[0]).Item1,
                        Level = eduLevelsCodes.First(x => x[0] == _DB_Helper.GetDirectionNameAndCode((uint)s1[0]).Item2.Split('.')[1])[1],
                        EduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP),
                        EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm),
                        ProfileShortName = s1[2].ToString(),
                        ProfileName = s2[1].ToString()
                    }).Select(s => new
                    {
                        Value = new DirTuple(s.Id, s.Faculty, s.Name, s.EduSource, s.EduForm, s.ProfileShortName, s.ProfileName),
                        Display = "(" + s.Faculty + ", " + s.Level + ") " + s.ProfileName
                    }).ToList();
                combobox.ValueMember = "Value";
                combobox.DisplayMember = "Display";
                combobox.DataSource = selectedDirs;
                combobox.SelectedIndex = -1;
            }
            else if (eduSource != Classes.DB_Helper.EduSourceT)
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

                var selectedDirs = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[] { "direction_id", "direction_faculty", placesCountColumnName },
                    new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                            new Tuple<string, Relation, object>(placesCountColumnName, Relation.GREATER, 0)
                        }).Join(
                    directionsData,
                    campDirs => campDirs[0],
                    dirsData => dirsData[0],
                    (s1, s2) => new
                    {
                        Id = (uint)s1[0],
                        Faculty = s1[1].ToString(),
                        Level = eduLevelsCodes.First(x => x[0] == s2[2].ToString().Split('.')[1])[1],
                        Name = s2[1].ToString(),
                        EduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, eduSource),
                        EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm)
                    }).Select(s => new
                    {
                        Value = new DirTuple(s.Id, s.Faculty,s.Name, s.EduSource, s.EduForm, "", ""),
                        Display = "(" + s.Faculty + ", " + s.Level + ") " + s.Name
                    }).ToList();
                combobox.ValueMember = "Value";
                combobox.DisplayMember = "Display";
                combobox.DataSource = selectedDirs;
                combobox.SelectedIndex = -1;
            }
            else if (eduSource == Classes.DB_Helper.EduSourceT)
            {
                string placesCountColumnName = "";
                if ((eduForm == "Очная форма") && (eduSource == "Целевой прием"))
                    placesCountColumnName = "places_o";
                else if ((eduForm == "Очно-заочная (вечерняя)") && (eduSource == "Целевой прием"))
                    placesCountColumnName = "places_oz";

                var selectedDirs = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new string[] { "direction_id", "direction_faculty", placesCountColumnName },
                    new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                            new Tuple<string, Relation, object>(placesCountColumnName, Relation.GREATER, 0)
                        }).Join(
                    directionsData,
                    campDirs => campDirs[0],
                    dirsData => dirsData[0],
                    (s1, s2) => new
                    {
                        Id = (uint)s1[0],
                        Faculty = s1[1].ToString(),
                        Level = eduLevelsCodes.First(x => x[0] == s2[2].ToString().Split('.')[1])[1],
                        Name = s2[1].ToString(),
                        EduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, eduSource),
                        EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm)
                    }).Select(s => new
                    {
                        Value = new DirTuple(s.Id, s.Faculty, s.Name, s.EduSource, s.EduForm, "", ""),
                        Display = "(" + s.Faculty + ", " + s.Level + ") " + s.Name
                    }).Distinct().ToList();
                combobox.ValueMember = "Value";
                combobox.DisplayMember = "Display";
                combobox.DataSource = selectedDirs;
                combobox.SelectedIndex = -1;
            }
        }

        private void ChangeAgreedChBs(bool isEnabled)
        {
            foreach (TabPage page in tcDirections.TabPages)
                foreach (Control control in page.Controls)
                {
                    CheckBox cb = control as CheckBox;
                    if (cb != null)
                        //if (cb.Visible)
                        cb.Enabled = isEnabled;
                }
        }

        private void BlockDirChange()
        {
            foreach (TabPage page in tcDirections.TabPages)
                foreach (Control control in page.Controls)
                {
                    ComboBox cb = control as ComboBox;
                    if (cb != null)
                        cb.Enabled = false;
                    Button bt = control as Button;
                    if (bt != null)
                        bt.Enabled = false;
                }
        }

        private void cbAgreed_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked && !_Agreed)
                if (!_Loading)
                {
                    if (Classes.Utility.ShowChoiceMessageWithConfirmation("Дать согласие на зачисление на данную специальность?", "Согласие на зачисление"))
                    {
                        int agreedCount = 0;
                        foreach (object[] data in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "is_agreed_date" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("application_id", Relation.EQUAL,_ApplicationID)
                        }))
                            if ((data[0] as DateTime?) != null)
                                agreedCount++;
                        if (agreedCount >= 3)
                        {
                            MessageBox.Show("Нельзя изменить согласие на зачисление больше 3 раз.");
                            _Agreed = true;
                            ((CheckBox)sender).Checked = false;
                        }
                        else
                        {
                            ChangeAgreedChBs(false);
                            BlockDirChange();
                        }
                    }
                    else
                    {
                        _Agreed = true;
                        ((CheckBox)sender).Checked = false;
                    }
                }
                else
                {
                    BlockDirChange();
                    ((CheckBox)sender).Enabled = true;
                }
            else if (!_Agreed)
            {
                int agreedCount = 0;
                foreach (object[] data in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "is_agreed_date" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("application_id", Relation.EQUAL,_ApplicationID)
                        }))
                    if ((data[0] as DateTime?) != null)
                        agreedCount++;
                if (agreedCount >= 3)
                {
                    MessageBox.Show("Нельзя изменить согласие на зачисление больше 3 раз.");
                    _Agreed = true;
                    ((CheckBox)sender).Checked = true;

                }
                else ChangeAgreedChBs(true);                
            }
            _Agreed = false;
        }

        private void tbNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) || (e.KeyChar == '\b')) return;
            else
                e.Handled = true;
        }

        private void tbCyrilic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar > 'А' && e.KeyChar < 'я') || (e.KeyChar == '\b') || (e.KeyChar == '.') || (e.KeyChar == 'ё') || (e.KeyChar == 'Ё')) return;
            else
                e.Handled = true;
        }
    }
}
