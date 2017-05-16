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
            public string medDocSerie;
            public string medDocNumber;
            public string disabilityGroup;
            public string conclusionNumber;
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

        
        public ODoc OlympicDoc;
        public SDoc SportDoc;

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
        private string _MADIOlympName;
        private bool _Agreed;
        private QDoc _QuoteDoc;
        private string[] _DirsMed = { "13.03.02", "23.03.01", "23.03.02", "23.03.03", "23.05.01", "23.05.02" };

        private bool _DistrictNeedsReload;
        private bool _TownNeedsReload;
        private bool _StreetNeedsReload;
        private bool _HouseNeedsReload;

        private const int _AgreedChangeMaxCount = 2;

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
            _KLADR = new Classes.KLADR(connection.User, connection.Password);
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
            cbRegion.Items.AddRange(_KLADR.GetRegions().ToArray());

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
            object[] minMarks;
            if (_DB_Connection.Select(DB_Table.CONSTANTS, new string[] { }).Count > 0)
                minMarks = _DB_Connection.Select(DB_Table.CONSTANTS, new string[] { "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark", "min_foreign_mark" })[0];
            else minMarks = new object[] { 0, 0, 0, 0, 0 };
            dgvExams.Rows.Add("Математика", null, null, (byte)0, minMarks[0], false);
            dgvExams.Rows.Add("Русский язык", null, null, (byte)0, minMarks[1], false);
            dgvExams.Rows.Add("Физика", null, null, (byte)0, minMarks[2], false);
            dgvExams.Rows.Add("Обществознание", null, null, (byte)0, minMarks[3], false);
            dgvExams.Rows.Add("Иностранный язык", null, null, (byte)0, minMarks[4], false);

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
                QuotDocs form = new QuotDocs(_DB_Connection, _QuoteDoc);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (_QuoteDoc.cause == null || _QuoteDoc.cause == ""))
                     cbQuote.Checked = false;
                else
                {
                    QDoc quoteDoc = form._Document;
                    _QuoteDoc.cause = quoteDoc.cause;
                    _QuoteDoc.medCause = quoteDoc.medCause;
                    _QuoteDoc.medDocSerie = quoteDoc.medDocSerie;
                    _QuoteDoc.medDocNumber = quoteDoc.medDocNumber;
                    _QuoteDoc.disabilityGroup = quoteDoc.disabilityGroup;
                    _QuoteDoc.conclusionDate = quoteDoc.conclusionDate;
                    _QuoteDoc.conclusionNumber = quoteDoc.conclusionNumber;
                    _QuoteDoc.orphanhoodDocDate = quoteDoc.orphanhoodDocDate;
                    _QuoteDoc.orphanhoodDocName = quoteDoc.orphanhoodDocName;
                    _QuoteDoc.orphanhoodDocOrg = quoteDoc.orphanhoodDocOrg;
                    _QuoteDoc.orphanhoodDocType = quoteDoc.orphanhoodDocType;
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
                
            }
            else if ((cbQuote.Checked) && (_Loading))
            {
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
                if (form.DialogResult != DialogResult.OK && (SportDoc.diplomaType == null || SportDoc.diplomaType == ""))
                    cbSport.Checked = false;
            }
        }

        private void cbMADIOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbMADIOlympiad.Checked) && (!_Loading))
            {
                MADIOlymps form = new MADIOlymps(_DB_Connection, _MADIOlympName);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (form.OlympName == null || form.OlympName == ""))
                    cbMADIOlympiad.Checked = false;                    
                else _MADIOlympName = form.OlympName;
            }
        }

        private void cbOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbOlympiad.Checked)&&(!_Loading))
            {
                Olymps form = new Olymps(_DB_Connection,this);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (OlympicDoc.olympType == null || OlympicDoc.olympType == ""))
                    cbOlympiad.Checked = false;
            }
            DirectionDocEnableDisable();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (tbLastName.Text == "" || tbFirstName.Text == "" || tbIDDocSeries.Text == "" || tbIDDocNumber.Text == ""
                || tbPlaceOfBirth.Text == "" || cbRegion.Text == "" || tbPostcode.Text == "")
                MessageBox.Show("Обязательные поля в разделе \"Из паспорта\" не заполнены.");
            else if ((rbDiploma.Checked || rbCertificate.Checked || rbSpravka.Checked) && (tbEduDocSeries.Text == "" || tbEduDocNumber.Text == ""))
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
                else if (!cbPassportMatch.Checked && (tbExamsDocSeries.Text == "") && (tbExamsDocNumber.Text == ""))
                    MessageBox.Show("Не заполнены обязательные поля в разделе \"Сведения о документе регистрации на ЕГЭ\".");
                else if (mtbEMail.Text == "")
                    MessageBox.Show("Поле \"Email\" не заполнено");
                else if (!cbAppAdmission.Checked
                    || (cbChernobyl.Checked || cbQuote.Checked || cbOlympiad.Checked || cbPriority.Checked) && !cbDirectionDoc.Checked
                    || (rbCertificate.Checked || rbDiploma.Checked) && !cbEduDoc.Checked
                    || rbSpravka.Checked && !cbCertificateHRD.Checked)
                    MessageBox.Show("В разделе \"Забираемые документы\" не отмечены обязательные поля.");
                else
                {
                    bool applicationMissed = false;
                    foreach (TabPage page in tcDirections.TabPages)
                        foreach (Control control in page.Controls)
                        {
                            CheckBox checkBox = control as CheckBox;
                            if (checkBox != null && checkBox.Checked && !cbAgreed.Checked)
                            {
                                MessageBox.Show("Не отмечено поле \"Заявление о согласии на зачисление\" в разделе \"Забираемые документы\".");
                                applicationMissed = true;
                                break;
                            }
                        }
                    if (!applicationMissed)
                    {
                        bool dateOk = true;
                        if ((dtpDateOfBirth.Tag.ToString() == "false" || dtpIDDocDate.Tag.ToString() == "false")
                            && !Classes.Utility.ShowChoiceMessageBox("Значения некоторых полей дат не были изменены. Продолжить?", "Даты не изменены"))
                            dateOk = false;
                        if (dateOk)
                        {
                            if (_ApplicationID == null)
                            {
                                SaveApplication();
                                btPrint.Enabled = true;
                                btWithdraw.Enabled = true;
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

        private void DirectionDocEnableDisable()
        {
            if (((cbChernobyl.Checked) || (cbQuote.Checked) || (cbOlympiad.Checked) || (cbPriority.Checked))&&(!_Loading))
                cbDirectionDoc.Enabled = true;
            else if (((cbChernobyl.Checked) || (cbQuote.Checked) || (cbOlympiad.Checked) || (cbPriority.Checked)) && (_Loading))
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

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCertificate.Checked)
            {
                if (cbOriginal.Checked)
                    cbEduDoc.Text = "Оригинал аттестата";
                else
                    cbEduDoc.Text = "Копия аттестата";
                cbEduDoc.Enabled = true;
                cbCertificateHRD.Enabled = false;
                cbCertificateHRD.Checked = false;
            }
            else if (rbDiploma.Checked)
            {
                if (cbOriginal.Checked)
                    cbEduDoc.Text = "Оригинал диплома";
                else
                    cbEduDoc.Text = "Копия диплома";
                cbEduDoc.Enabled = true;
                cbCertificateHRD.Enabled = false;
                cbCertificateHRD.Checked = false;
            }
            else if (rbSpravka.Checked)
            {
                cbCertificateHRD.Enabled = true;
                cbEduDoc.Enabled = false;
                cbEduDoc.Checked = false;
            }
        }

        private void cbChernobyl_CheckedChanged(object sender, EventArgs e)
        {
            DirectionDocEnableDisable();
        }

        private void cbPrerogative_CheckedChanged(object sender, EventArgs e)
        {
            DirectionDocEnableDisable();
        }

        private void cbDistrict_Enter(object sender, EventArgs e)
        {
            if (_DistrictNeedsReload)
            {
                cbDistrict.Items.Clear();
                cbDistrict.Items.AddRange(_KLADR.GetDistricts(cbRegion.Text).ToArray());

                if (cbDistrict.Items.Count == 0)
                {
                    toolTip.Show("Не найдено адресов.", cbDistrict, 3000);
                }
                _DistrictNeedsReload = false;
            }
        }
        
        private void cbTown_Enter(object sender, EventArgs e)
        {
            if (_TownNeedsReload)
            {
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
                cbStreet.Items.Clear();                
                cbStreet.Items.AddRange(_KLADR.GetStreets(cbRegion.Text, cbDistrict.Text, cbTown.Text).ToArray());

                if (cbStreet.Items.Count == 0)
                {
                    toolTip.Show("Не найдено адресов.", cbStreet, 3000);
                }
                _StreetNeedsReload = false;
            }
        }

        private void cbHouse_Enter(object sender, EventArgs e)
        {
            if (_HouseNeedsReload)
            {
                cbHouse.Items.Clear();
                cbHouse.Items.AddRange(_KLADR.GetHouses(cbRegion.Text, cbDistrict.Text, cbTown.Text, cbStreet.Text).ToArray());

                if (cbHouse.Items.Count == 0)
                {
                    toolTip.Show("Не найдено адресов.", cbHouse, 3000);
                }
                _HouseNeedsReload = false;
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
            ApplicationDocsPrint form = new ApplicationDocsPrint(_DB_Connection, _ApplicationID.Value);
            form.ShowDialog();
        }

        private void cbTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTarget.Checked && !_Loading)
            {
                TargetOrganizationSelect form = new TargetOrganizationSelect(_DB_Connection,_TargetOrganizationID);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (form.OrganizationID == null || form.OrganizationID == 0))
                    cbTarget.Checked = false;
                else
                {
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
                if (tabNumber != 2 && tabNumber != 4 && tabNumber != 5)
                {
                    CheckBox cb = tcDirections.TabPages[tabNumber - 1].Controls.Find("cbAgreed" + tabNumber.ToString() + "1", false)[0] as CheckBox;
                    cb.Visible = true;
                }
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
                CheckBox chb = control as CheckBox;
                if ((chb != null) && (chb.Name == "cbAgreed" + tabNumber + comboNumber))
                {
                    chb.Enabled = false;
                    chb.Visible = false;
                }
            }
            (sender as Control).Enabled = false;
            (sender as Control).Visible = false;
        }

        private void btGetIndex_Click(object sender, EventArgs e)
        {
            tbPostcode.Text = _KLADR.GetIndex(cbRegion.Text, cbDistrict.Text, cbTown.Text, cbStreet.Text, cbHouse.Text);
            if (tbPostcode.Text == "")
                tbPostcode.Enabled = true;
            else
                tbPostcode.Enabled = false;
        }

        private void cbAgreed_CheckedChanged(object sender, EventArgs e)
        {
            if (!_Agreed)
                if (((CheckBox)sender).Checked)
                    if (!_Loading)
                    {
                        int agreedCount = 0;
                        foreach (object[] data in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "is_agreed_date" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("application_id", Relation.EQUAL,_ApplicationID)
                        }))
                            if ((data[0] as DateTime?) != null)
                                agreedCount++;
                        if (agreedCount >= _AgreedChangeMaxCount)
                        {
                            MessageBox.Show("Нельзя изменить согласие на зачисление больше " + _AgreedChangeMaxCount + " раз.");
                            _Agreed = true;
                            ((CheckBox)sender).Checked = false;
                        }
                        else if (Classes.Utility.ShowChoiceMessageWithConfirmation("Дать согласие на зачисление на данную специальность?", "Согласие на зачисление"))
                        {
                            //UpdateDirections();
                            //foreach (Control control in ((CheckBox)sender).Parent.Controls)
                            //{
                            //    ComboBox comboBox = control as ComboBox;
                            //    if (comboBox != null && comboBox.Name == "cbDirection" + ((CheckBox)sender).Name.Substring(8) && ((ComboBox)comboBox).SelectedIndex != -1)
                            //        _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "is_agreed_date", DateTime.Now } },
                            //            new Dictionary<string, object> { { "faculty_short_name", ((DirTuple)comboBox.SelectedValue).Item2 },
                            //            { "direction_id", ((DirTuple)comboBox.SelectedValue).Item1 }, { "edu_form_id", ((DirTuple)comboBox.SelectedValue).Item5 },
                            //            { "edu_source_id", ((DirTuple)comboBox.SelectedValue).Item4 }, { "application_id", _ApplicationID } });
                            //}
                            ChangeAgreedChBs(false);
                            BlockDirChange();
                            cbOriginal.Enabled = false;
                            cbAgreed.Enabled = true;
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
                else
                {
                    int disagreedCount = 0;
                    foreach (object[] data in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "is_disagreed_date" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("application_id", Relation.EQUAL,_ApplicationID)
                            }))
                        if ((data[0] as DateTime?) != null)
                            disagreedCount++;
                    if (disagreedCount >= _AgreedChangeMaxCount)
                    {
                        MessageBox.Show("Нельзя изменить согласие на зачисление больше " + _AgreedChangeMaxCount + " раз.");
                        _Agreed = true;
                        ((CheckBox)sender).Checked = true;
                    }
                    else if (Classes.Utility.ShowChoiceMessageWithConfirmation("Отменить согласие на зачисление на данную специальность?", "Согласие на зачисление"))
                    {
                        //foreach (Control control in ((CheckBox)sender).Parent.Controls)
                        //{
                        //    ComboBox comboBox = control as ComboBox;
                        //    if (comboBox != null && comboBox.Name == "cbDirection" + ((CheckBox)sender).Name.Substring(8) && ((ComboBox)comboBox).SelectedIndex != -1)
                        //        _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "is_disagreed_date", DateTime.Now } },
                        //            new Dictionary<string, object> { { "faculty_short_name", ((DirTuple)comboBox.SelectedValue).Item2 },
                        //            { "direction_id", ((DirTuple)comboBox.SelectedValue).Item1 }, { "edu_form_id", ((DirTuple)comboBox.SelectedValue).Item5 },
                        //            { "edu_source_id", ((DirTuple)comboBox.SelectedValue).Item4 }, { "application_id", _ApplicationID } });
                        //}
                        if (disagreedCount < _AgreedChangeMaxCount - 1)
                            ChangeAgreedChBs(true);
                        else
                            ((CheckBox)sender).Enabled = false;
                        cbAgreed.Checked = false;
                        cbAgreed.Enabled = false;
                        cbOriginal.Enabled = true;
                    }
                    else
                    {
                        _Agreed = true;
                        ((CheckBox)sender).Checked = true;
                    }
                }
            _Agreed = false;
        }

        private void tbNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || (e.KeyChar == '\b'))
                return;
            else
                e.Handled = true;
        }

        private void tbCyrilic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'А' && e.KeyChar <= 'я') || (e.KeyChar == '\b') || (e.KeyChar == '.') || (e.KeyChar == 'ё') || (e.KeyChar == 'Ё'))
                return;
            else
                e.Handled = true;
        }

        private void tbKLADR_Leave(object sender, EventArgs e)
        {
            tbPostcode.Text = "";
        }

        private void btWithdraw_Click(object sender, EventArgs e)
        {
            if (Classes.Utility.ShowChoiceMessageWithConfirmation("Забрать документы?", "Забрать документы"))
            {
                UpdateApplication();
                _DB_Connection.Update(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "status", "withdrawn" } }, new Dictionary<string, object>
                { { "id", _ApplicationID } });

                foreach (Control control in panel1.Controls)
                    control.Enabled = false;
                btClose.Enabled = true;
            }
        }

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void dgvExams_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (((DataGridView)sender).CurrentCell.ColumnIndex == dgvExams_EGE.Index)
            {
                try
                {
                    if (byte.Parse(e.FormattedValue.ToString()) > 100)
                    {
                        MessageBox.Show("Значение не должно превышать 100.");
                        e.Cancel = true;
                    }
                }
                catch { }
            }
        }
        
        private void dgvExams_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).CurrentCell != null && ((DataGridView)sender).CurrentCell.ColumnIndex == dgvExams_EGE.Index && ((DataGridView)sender).CurrentCell.Value.ToString() == "")
                ((DataGridView)sender).CurrentCell.Value = (byte)0;
        }
        
        private void cbAdress_TextChanged(object sender, EventArgs e)
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

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            ((DateTimePicker)sender).Tag = true;
        }

        private void btFillRand_Click(object sender, EventArgs e)
        {
            tbLastName.Text = "";
            tbFirstName.Text = "";
            tbMiddleName.Text = "";
            tbPlaceOfBirth.Text = "";
            tbIssuedBy.Text = "";
            tbInstitutionLocation.Text = "";
            tbIDDocSeries.Text = "";
            tbIDDocNumber.Text = "";
            tbSubdivisionCode.Text = "";
            tbEduDocSeries.Text = "";
            tbEduDocNumber.Text = "";
            tbInstitutionNumber.Text = "";
            tbPostcode.Text = "";
            mtbEMail.Text = "";
            mtbHomePhone.Text = "";
            mtbMobilePhone.Text = "";

            Random rand = new Random();
            int stringLength = 10;
            int numberLength = 5;
            char[] cyrillicLetters = { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т',
                'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я' };
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] latinLetters = { 'a', 'b', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            string[] emailEndings = { "gmail.com", "yandex.ru", "rambler.ru", "list.ru", "mail.ru" };
            for (int i = 0; i < stringLength; i++)
            {
                tbLastName.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
                tbFirstName.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
                tbMiddleName.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
                tbPlaceOfBirth.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
                tbIssuedBy.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
                tbInstitutionLocation.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
            }
            for (int i = 0; i < numberLength; i++)
            {
                tbIDDocSeries.Text += digits[rand.Next(digits.Length)];
                tbIDDocNumber.Text += digits[rand.Next(digits.Length)];
                tbSubdivisionCode.Text += digits[rand.Next(digits.Length)];
                tbEduDocSeries.Text += digits[rand.Next(digits.Length)];
                tbEduDocNumber.Text += digits[rand.Next(digits.Length)];
                tbExamsDocSeries.Text += digits[rand.Next(digits.Length)];
                tbExamsDocNumber.Text += digits[rand.Next(digits.Length)];
                tbInstitutionNumber.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
            }

            for (int i = 0; i < 6; i++)
                tbPostcode.Text += digits[rand.Next(digits.Length)];
            for (int i = 0; i < 10; i++)
                mtbEMail.Text += latinLetters[rand.Next(latinLetters.Length)];
            mtbEMail.Text += "@" + emailEndings[rand.Next(emailEndings.Length)];
            string phone = "";
            if (rand.Next(2) > 0)
                for (int i = 0; i < 10; i++)
                    phone += digits[rand.Next(digits.Length)];
            mtbHomePhone.Text = phone;
            phone = "";
            if (rand.Next(2) > 0)
                for (int i = 0; i < 10; i++)
                    phone += digits[rand.Next(digits.Length)];
            mtbMobilePhone.Text = phone;

            cbSex.SelectedIndex = rand.Next(cbSex.Items.Count);
            cbRegion.SelectedIndex = rand.Next(cbRegion.Items.Count);
            cbInstitutionType.SelectedIndex = rand.Next(cbInstitutionType.Items.Count);
            cbFirstTime.SelectedIndex = rand.Next(cbFirstTime.Items.Count);
            cbForeignLanguage.SelectedIndex = rand.Next(cbForeignLanguage.Items.Count);

            int year = rand.Next(1990, DateTime.Now.Year - 16);
            int month = rand.Next(1, 12);
            int day = rand.Next(1, DateTime.DaysInMonth(year, month));
            DateTime randDate = new DateTime(year, month, day);
            dtpDateOfBirth.Value = randDate;

            if (DateTime.Now.Year - dtpDateOfBirth.Value.Year >= 14 && DateTime.Now.Year - dtpDateOfBirth.Value.Year < 20)
                year = dtpDateOfBirth.Value.Year + 14;
            else if (DateTime.Now.Year - dtpDateOfBirth.Value.Year >= 20 && DateTime.Now.Year - dtpDateOfBirth.Value.Year < 45)
                if (DateTime.Now.Year - dtpDateOfBirth.Value.Year == 20 && DateTime.Now.Month < dtpDateOfBirth.Value.Month)
                    year = dtpDateOfBirth.Value.Year + 14;
                else year = dtpDateOfBirth.Value.Year + 20;
            else if (DateTime.Now.Year - dtpDateOfBirth.Value.Year == 45 && DateTime.Now.Month < dtpDateOfBirth.Value.Month)
                year = dtpDateOfBirth.Value.Year + 20;
            else year = dtpDateOfBirth.Value.Year + 45;
            if (year == DateTime.Now.Year)
                month = rand.Next(dtpDateOfBirth.Value.Month, DateTime.Now.Month);
            else month = rand.Next(1, 12);
            day = rand.Next(1, DateTime.DaysInMonth(year, month));
            dtpIDDocDate.Value = new DateTime(year, month, day);

            cbGraduationYear.SelectedItem = rand.Next(dtpDateOfBirth.Value.Year + 16, DateTime.Now.Year);
        }

        private void cbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbMedCertificate.Enabled = false;
            foreach (TabPage page in tcDirections.TabPages)
                foreach (Control control in page.Controls)
                {
                    ComboBox combo = control as ComboBox;
                    if (combo != null && combo.SelectedIndex != -1 && _DirsMed.Contains(_DB_Helper.GetDirectionNameAndCode(((DirTuple)combo.SelectedValue).Item1).Item2))
                        cbMedCertificate.Enabled = true;
                }
            if (!cbMedCertificate.Enabled)
                cbMedCertificate.Checked = false;
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
            LoadExaminationsMarks();
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
                int passwordLength = 12;
                string password = "";
                Random rand = new Random();
                for (int i = 0; i < passwordLength; i++)
                    password +=  passwordChars[rand.Next(passwordChars.Length)];
                _EntrantID = _DB_Connection.Insert(DB_Table.ENTRANTS, new Dictionary<string, object> { { "email", mtbEMail.Text }, { "personal_password", password },
                    { "home_phone", string.Concat(mtbHomePhone.Text.Where(s => char.IsNumber(s))) }, { "mobile_phone", string.Concat(mtbMobilePhone.Text.Where(s => char.IsNumber(s))) } });
            }
            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;

            _ApplicationID = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "entrant_id", _EntrantID.Value}, { "registration_time", DateTime.Now},
                { "needs_hostel", cbHostelNeeded.Checked}, { "registrator_login", _RegistratorLogin}, { "campaign_id", _CurrCampainID },
                { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "first_high_edu", firstHightEdu}, { "mcado", cbMCADO.Checked}, { "chernobyl", cbChernobyl.Checked}, { "passing_examinations",cbExams.Checked },
                { "priority_right", cbPriority.Checked}, { "special_conditions", cbSpecialConditions.Checked}, { "master_appl", false} });

            uint idDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "identity" },
                { "series", tbIDDocSeries.Text}, { "number", tbIDDocNumber.Text}, { "date", dtpIDDocDate.Value} , { "organization", tbIssuedBy.Text} });

            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID},
                { "documents_id", idDocUid } });

            _DB_Connection.Insert(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", idDocUid},
                { "last_name", tbLastName.Text}, { "first_name", tbFirstName.Text}, { "middle_name", tbMiddleName.Text},
                { "gender_dict_id", (uint)FIS_Dictionary.GENDER},{ "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER, cbSex.SelectedItem.ToString())},
                { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY}, { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", cbRegion.Text}, { "reg_district", cbDistrict.Text},
                { "reg_town", cbTown.Text}, { "reg_street", cbStreet.Text}, { "reg_house", cbHouse.Text}, { "reg_index", tbPostcode.Text}, { "reg_flat", tbAppartment.Text} });

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
            if (_QuoteDoc.cause == "Сиротство")
            {
                uint orphDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "orphan" },
                    { "date", _QuoteDoc.orphanhoodDocDate} , { "organization", _QuoteDoc.orphanhoodDocOrg} });
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", orphDocUid},
                        { "name", _QuoteDoc.orphanhoodDocName}, { "dictionaries_dictionary_id", (uint)FIS_Dictionary.ORPHAN_DOC_TYPE},
                        { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.ORPHAN_DOC_TYPE, _QuoteDoc.orphanhoodDocType)} });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", orphDocUid } });
                _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                            { "reason_document_id", orphDocUid},{ "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } });
            }
            else if (_QuoteDoc.cause == "Медицинские показатели")
            {
                uint allowEducationDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                    { { "number", _QuoteDoc.conclusionNumber}, { "date", _QuoteDoc.conclusionDate}, { "type", "allow_education"} });
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", allowEducationDocUid } });

                if (_QuoteDoc.medCause == "Справка об установлении инвалидности")
                {
                    uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "disability" },
                        { "series", _QuoteDoc.medDocSerie},  { "number", _QuoteDoc.medDocNumber} });
                    _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {
                            { "document_id", medDocUid}, { "dictionaries_dictionary_id",(uint)FIS_Dictionary.DISABILITY_GROUP},
                            { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DISABILITY_GROUP, _QuoteDoc.disabilityGroup)} });
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", medDocUid } });
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", _ApplicationID}, { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности")},
                            { "reason_document_id", medDocUid},{ "allow_education_document_id", allowEducationDocUid}, { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } });
                }
                else if (_QuoteDoc.medCause == "Заключение психолого-медико-педагогической комиссии")
                {
                    uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "medical" },
                        { "series", _QuoteDoc.medDocSerie},  { "number", _QuoteDoc.medDocNumber} });
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
            uint sportDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                { { "type", "sport" }, { "date", SportDoc.docDate}, { "organization", SportDoc.orgName} });
            _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                { { "document_id", sportDocID}, { "name", SportDoc.docName} });
            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", sportDocID } });

            uint achevmentCategoryId = 0;
            switch (SportDoc.diplomaType)
            {
                case Classes.DB_Helper.SportDocTypeOlympic:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementOlympic);
                    break;
                case Classes.DB_Helper.SportDocTypeParalympic:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementParalympic);
                    break;
                case Classes.DB_Helper.SportDocTypeDeaflympic:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementDeaflympic);
                    break;
                case Classes.DB_Helper.SportDocTypeWorldChampion:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementWorldChampion);
                    break;
                case Classes.DB_Helper.SportDocTypeEuropeChampion:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementEuropeChampion);
                    break;
                case Classes.DB_Helper.SportAchievementGTO:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementGTO);
                    break;
                case Classes.DB_Helper.SportAchievementWorldChampionship:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementWorldChampionship);
                    break;
                case Classes.DB_Helper.SportAchievementEuropeChampionship:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementEuropeChampionship);
                    break;
            }
                List<object[]> achievments = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                    new List<Tuple<string, Relation, object>>
                {
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                        new Tuple<string, Relation, object>("category_id", Relation.EQUAL, achevmentCategoryId)
                });

                if (achievments.Count == 0)
                    MessageBox.Show("В данной кампании отсутствует спортивное достижение \"" + _DB_Helper.GetDictionaryItemName(FIS_Dictionary.IND_ACH_CATEGORIES, achevmentCategoryId) + "\".");
                else
                    _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID },
                        { "institution_achievement_id", uint.Parse(achievments[0][0].ToString())}, { "document_id", sportDocID} });
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
                    { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) }, { "olympic_id", (uint)OlympicDoc.olympID}, { "class_number", OlympicDoc.olympClass },                                        
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
                    { "olympic_id", (uint)OlympicDoc.olympID}, { "class_number", OlympicDoc.olympClass },                  
                    { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES }, { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) },
                    { "olympic_subject_dict_id", (uint)FIS_Dictionary.SUBJECTS }, { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, OlympicDoc.olympDist) } });
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;

                    case "Диплом 4 этапа всеукраинской олимпиады":
                        docType = "ukraine_olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера IV этапа всеукраинской ученической олимпиады");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                    { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) }, { "olympic_name", OlympicDoc.olympName },
                    { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES }, { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) }});
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", _ApplicationID }, { "documents_id", olympicDocId } });
                        break;

                    case "Диплом международной олимпиады":
                        docType = "international_olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Документ об участии в международной олимпиаде");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", docType }, { "number", OlympicDoc.olympDocNumber } });

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", olympicDocId }, { "olympic_name", OlympicDoc.olympName },
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
                    _DB_Connection.Insert(DB_Table.DOCUMENTS_SUBJECTS_DATA, new Dictionary<string, object> { { "document_id", examsDocId}, { "year", row.Cells[dgvExams_Year.Index].Value },
                        { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS} , { "subject_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.SUBJECTS, row.Cells[0].Value.ToString())} ,
                        { "value", row.Cells[3].Value}, { "checked", false } });
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
                                    { "profile_short_name", ((DirTuple)cb.SelectedValue).Item6} });
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
            Text += " № " + _ApplicationID;
            object[] application = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "needs_hostel", "language", "first_high_edu",
                "mcado", "chernobyl", "passing_examinations", "priority_right", "special_conditions", "entrant_id", "status" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, _ApplicationID)
                })[0];
            if (application[9].ToString() == "withdrawn")
            {
                foreach (Control control in panel1.Controls)
                    control.Enabled = false;
                btClose.Enabled = true;
            }
            else
            {
                btWithdraw.Enabled = true;
                btPrint.Enabled = true;
            }
            _EntrantID = (uint)application[8];
            cbHostelNeeded.Checked = (bool)application[0];
            cbForeignLanguage.SelectedItem = application[1];
            if ((bool)application[2])
                cbFirstTime.SelectedItem = "Впервые";
            else cbFirstTime.SelectedItem = "Повторно";
            cbMCADO.Checked = (bool)application[3];
            cbChernobyl.Checked = (bool)application[4];
            cbExams.Checked = (bool)application[5];
            cbPriority.Checked = (bool)application[6];
            cbSpecialConditions.Checked = (bool)application[7];
            cbPassportCopy.Checked = true;
            cbAppAdmission.Checked = true;

            object[] entrant = _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, new string[] { "last_name", "first_name", "middle_name" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("id", Relation.EQUAL, application[8])
            })[0];
            tbLastName.Text = entrant[0].ToString();
            tbFirstName.Text = entrant[1].ToString();
            tbMiddleName.Text = entrant[2].ToString();

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
                new Tuple<string, Relation, object>("institution_achievement_id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.MedalAchievement)),
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                })[0][0])
            }).Count > 0)
                cbMedal.Checked = true;
        }

        private void LoadExaminationsMarks()
        {
            var exMarks = _DB_Connection.Select(DB_Table.ENTRANTS_EXAMINATIONS_MARKS, new string[] { "examination_id", "mark" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("entrant_id", Relation.EQUAL, _EntrantID)
            });

            var examinations = _DB_Connection.Select(DB_Table.EXAMINATIONS, new string[] { "id", "subject_id", "date" }).Join(
                exMarks,
                ex => ex[0],
                exM => exM[0],
                (s1,s2) => new Tuple<uint,int,DateTime>((uint)s1[1], int.Parse(s2[1].ToString()), (DateTime)s1[2])
                ).ToArray();
            foreach (var examData in examinations)
                foreach (DataGridViewRow row in dgvExams.Rows)
                    if (row.Cells[dgvExams_Subject.Index].Value.ToString() == _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, examData.Item1))
                    {
                        row.Cells[dgvExams_Exam.Index].Value = examData.Item2;
                        row.Cells[dgvExams_Exam.Index].ToolTipText = examData.Item3.ToShortDateString();
                    }
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
                    cbRegion.Text = passport[5].ToString();
                    cbDistrict.Text = passport[6].ToString();
                    cbTown.Text = passport[7].ToString();
                    cbStreet.Text = passport[8].ToString();
                    cbHouse.Text = passport[9].ToString();
                    tbPostcode.Text = passport[10].ToString();
                    tbAppartment.Text = passport[11].ToString();
                    cbSex.SelectedItem = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.GENDER, (uint)passport[12]);
                }
                else if ((document[1].ToString() == "school_certificate") || (document[1].ToString() == "high_edu_diploma") || (document[1].ToString() == "academic_diploma"))
                {
                    if (document[1].ToString() == "school_certificate")
                    {
                        rbCertificate.Checked = true;
                        cbEduDoc.Checked = true;
                    }
                    else if (document[1].ToString() == "high_edu_diploma")
                    {
                        rbDiploma.Checked = true;
                        cbEduDoc.Checked = true;
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
                    if (tbExamsDocSeries.Text == tbIDDocSeries.Text && tbExamsDocNumber.Text == tbIDDocNumber.Text && cbExamsDocType.SelectedItem.ToString() == cbIDDocType.SelectedItem.ToString())
                        cbPassportMatch.Checked = true;

                    List<object[]> subjects = _DB_Connection.Select(DB_Table.DOCUMENTS_SUBJECTS_DATA, new string[] { "subject_id", "value", "checked", "year" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    });
                    foreach (var subject in subjects)
                        foreach (DataGridViewRow row in dgvExams.Rows)
                            if (row.Cells[dgvExams_Subject.Index].Value.ToString() == _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)subject[0]))
                            {
                                row.Cells[dgvExams_EGE.Index].Value = (byte)(uint)subject[1];
                                row.Cells[dgvExams_Checked.Index].Value = (bool)subject[2];
                                row.Cells[dgvExams_Year.Index].Value = subject[3].ToString();
                                if ((bool)subject[2])
                                    row.ReadOnly = true;
                            }
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
                        case Classes.DB_Helper.SportAchievementOlympic:
                            SportDoc.diplomaType = Classes.DB_Helper.SportDocTypeOlympic;
                            break;
                        case Classes.DB_Helper.SportAchievementParalympic:
                            SportDoc.diplomaType = Classes.DB_Helper.SportDocTypeParalympic;
                            break;
                        case Classes.DB_Helper.SportAchievementDeaflympic:
                            SportDoc.diplomaType = Classes.DB_Helper.SportDocTypeDeaflympic;
                            break;
                        case Classes.DB_Helper.SportAchievementWorldChampion:
                            SportDoc.diplomaType = Classes.DB_Helper.SportDocTypeWorldChampion;
                            break;
                        case Classes.DB_Helper.SportDocTypeEuropeChampion:
                            SportDoc.diplomaType = Classes.DB_Helper.SportDocTypeEuropeChampion;
                            break;
                        case Classes.DB_Helper.SportAchievementGTO:
                            SportDoc.diplomaType = Classes.DB_Helper.SportAchievementGTO;
                            break;
                        case Classes.DB_Helper.SportAchievementWorldChampionship:
                            SportDoc.diplomaType = Classes.DB_Helper.SportAchievementWorldChampionship;
                            break;
                        case Classes.DB_Helper.SportAchievementEuropeChampionship:
                            SportDoc.diplomaType = Classes.DB_Helper.SportAchievementEuropeChampionship;
                            break;
                    }
                    cbSport.Checked = true;
                }
                else if (document[1].ToString() == "orphan")
                {
                    _QuoteDoc.cause = "Сиротство";
                    cbQuote.Checked = true;
                    _QuoteDoc.orphanhoodDocDate = (DateTime)document[4];
                    _QuoteDoc.orphanhoodDocOrg = document[5].ToString();

                    object[] orphanDoc = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name", "dictionaries_item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0]),
                        new Tuple<string, Relation, object>("dictionaries_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.ORPHAN_DOC_TYPE)
                    })[0];
                    _QuoteDoc.orphanhoodDocName = orphanDoc[0].ToString();
                    _QuoteDoc.orphanhoodDocType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.ORPHAN_DOC_TYPE, (uint)orphanDoc[1]);
                }
                else if (document[1].ToString() == "disability")
                {
                    _QuoteDoc.cause = "Медицинские показатели";
                    _QuoteDoc.medCause = "Справка об установлении инвалидности";
                    cbQuote.Checked = true;
                    _QuoteDoc.medDocSerie = document[2].ToString();
                    _QuoteDoc.medDocNumber = document[3].ToString();
                    _QuoteDoc.disabilityGroup = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DISABILITY_GROUP, (uint)_DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "dictionaries_item_id" },
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
                    _QuoteDoc.conclusionNumber = allowDocument[0].ToString();
                    _QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                }
                else if (document[1].ToString() == "medical")
                {
                    _QuoteDoc.cause = "Медицинские показатели";
                    _QuoteDoc.medCause = "Заключение психолого-медико-педагогической комиссии";
                    cbQuote.Checked = true;
                    _QuoteDoc.medDocSerie = document[2].ToString();
                    _QuoteDoc.medDocNumber = document[3].ToString();
                    object[] allowDocument = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "number", "date" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    (uint)(_DB_Connection.Select(DB_Table.APPLICATION_COMMON_BENEFITS, new string[] { "allow_education_document_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Заключение психолого-медико-педагогической комиссии"))
                    }))[0][0])})[0];
                    _QuoteDoc.conclusionNumber = allowDocument[0].ToString();
                    _QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                }
                else if (document[1].ToString() == "olympic")
                {
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    foreach (object[] olympDocData in _DB_Connection.Select(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new string[] { "diploma_type_id", "olympic_id", "class_number",
                        "profile_id", "olympic_subject_id" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                        }))
                    {
                        OlympicDoc.diplomaType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DIPLOMA_TYPE, (uint)olympDocData[0]);
                        OlympicDoc.olympID = int.Parse(olympDocData[1].ToString());
                        OlympicDoc.olympClass = int.Parse(olympDocData[2].ToString());
                        OlympicDoc.olympProfile = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)olympDocData[3]);
                        OlympicDoc.olympDist = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)olympDocData[4]);
                        OlympicDoc.olympType = "Диплом победителя/призера олимпиады школьников";
                    }
                    cbOlympiad.Checked = true;
                }
                else if (document[1].ToString() == "olympic_total")
                {
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    foreach (object[] olympDocData in _DB_Connection.Select(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new string[] { "diploma_type_id", "olympic_id", "class_number",
                        "profile_id", "olympic_subject_id" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                        }))
                    {
                        OlympicDoc.diplomaType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DIPLOMA_TYPE, (uint)olympDocData[0]);
                        OlympicDoc.olympID = int.Parse(olympDocData[1].ToString());
                        OlympicDoc.olympClass = int.Parse(olympDocData[2].ToString());
                        OlympicDoc.olympProfile = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)olympDocData[3]);
                        OlympicDoc.olympDist = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)olympDocData[4]);
                        OlympicDoc.olympType = "Диплом победителя/призера всероссийской олимпиады школьников";
                    }
                    cbOlympiad.Checked = true;
                }
                else if (document[1].ToString() == "ukraine_olympic")
                {
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    foreach (object[] olympDocData in _DB_Connection.Select(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new string[] { "diploma_type_id", "class_number",
                        "profile_id", "olympic_subject_id", "olympic_name" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                        }))
                    {
                        OlympicDoc.diplomaType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DIPLOMA_TYPE, (uint)olympDocData[0]);
                        OlympicDoc.olympProfile = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)olympDocData[2]);
                        OlympicDoc.olympName = olympDocData[4].ToString();
                        OlympicDoc.olympType = "Диплом 4 этапа всеукраинской олимпиады";
                    }
                    cbOlympiad.Checked = true;
                }
                else if (document[1].ToString() == "international_olympic")
                {
                    OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
                    foreach (object[] olympDocData in _DB_Connection.Select(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new string[] { "class_number",
                        "profile_id", "olympic_subject_id", "olympic_name", "country_id" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                        }))
                    {
                        OlympicDoc.olympProfile = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)olympDocData[1]);
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
            "profile_short_name", "edu_form_id", "edu_source_id", "target_organization_id", "is_disagreed_date" }, new List<Tuple<string, Relation, object>>
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

            foreach (var entrancesData in records)
            {
                if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceB))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    if (cbDirection11.SelectedIndex == -1)
                    {
                        cbDirection11.SelectedValue = entrancesData.Value;
                        cbDirection11.Visible = true;
                        cbDirection11.Enabled = true;
                        btRemoveDir11.Visible = true;
                        btRemoveDir11.Enabled = true;
                        cbAgreed11.Visible = true;
                    }
                    else if (cbDirection12.SelectedIndex == -1)
                    {
                        cbDirection12.SelectedValue = entrancesData.Value;
                        cbDirection12.Visible = true;
                        cbDirection12.Enabled = true;
                        btRemoveDir12.Visible = true;
                        btRemoveDir12.Enabled = true;
                        cbAgreed12.Visible = true;
                    }
                    else if (cbDirection13.SelectedIndex == -1)
                    {
                        cbDirection13.SelectedValue = entrancesData.Value;
                        cbDirection13.Visible = true;
                        cbDirection13.Enabled = true;
                        btRemoveDir13.Visible = true;
                        btRemoveDir13.Enabled = true;
                        cbAgreed13.Visible = true;
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbDirection21.SelectedValue = entrancesData.Value;
                    cbDirection21.Visible = true;
                    cbDirection21.Enabled = true;
                    btRemoveDir21.Visible = true;
                    btRemoveDir21.Enabled = true;
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceB))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
                    if (cbDirection31.SelectedIndex == -1)
                    {
                        cbDirection31.SelectedValue = entrancesData.Value;
                        cbDirection31.Visible = true;
                        cbDirection31.Enabled = true;
                        btRemoveDir31.Visible = true;
                        btRemoveDir31.Enabled = true;
                        cbAgreed31.Visible = true;
                    }
                    else if (cbDirection32.SelectedIndex == -1)
                    {
                        cbDirection32.SelectedValue = entrancesData.Value;
                        cbDirection32.Visible = true;
                        cbDirection32.Enabled = true;
                        btRemoveDir32.Visible = true;
                        btRemoveDir32.Enabled = true;
                        cbAgreed32.Visible = true;
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
                    cbDirection41.SelectedValue = entrancesData.Value;
                    cbDirection41.Visible = true;
                    cbDirection41.Enabled = true;
                    btRemoveDir41.Visible = true;
                    btRemoveDir41.Enabled = true;
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormZ)))
                {
                    cbDirection51.SelectedValue = entrancesData.Value;
                    cbDirection51.Visible = true;
                    cbDirection51.Enabled = true;
                    btRemoveDir51.Visible = true;
                    btRemoveDir51.Enabled = true;
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbQuote.Checked = true;
                    if (cbDirection61.SelectedIndex == -1)
                    {
                        cbDirection61.SelectedValue = entrancesData.Value;
                        cbDirection61.Visible = true;
                        cbDirection61.Enabled = true;
                        btRemoveDir61.Visible = true;
                        btRemoveDir61.Enabled = true;
                        cbAgreed61.Visible = true;
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
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
                    }
                    else if (cbDirection72.SelectedIndex == -1)
                    {
                        cbDirection72.SelectedValue = entrancesData.Value;
                        cbDirection72.Visible = true;
                        cbDirection72.Enabled = true;
                        btRemoveDir72.Visible = true;
                        btRemoveDir72.Enabled = true;
                        cbAgreed72.Visible = true;
                    }
                    else if (cbDirection73.SelectedIndex == -1)
                    {
                        cbDirection73.SelectedValue = entrancesData.Value;
                        cbDirection73.Visible = true;
                        cbDirection73.Enabled = true;
                        btRemoveDir73.Visible = true;
                        btRemoveDir73.Enabled = true;
                        cbAgreed73.Visible = true;
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
                    cbQuote.Checked = true;
                    if (cbDirection81.SelectedIndex == -1)
                    {
                        cbDirection81.SelectedValue = entrancesData.Value;
                        cbDirection81.Visible = true;
                        cbDirection81.Enabled = true;
                        btRemoveDir81.Visible = true;
                        btRemoveDir81.Enabled = true;
                        cbAgreed81.Visible = true;
                    }
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ)))
                {
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
                    }
                    else if (cbDirection92.SelectedIndex == -1)
                    {
                        cbDirection92.SelectedValue = entrancesData.Value;
                        cbDirection92.Visible = true;
                        cbDirection92.Enabled = true;
                        btRemoveDir92.Visible = true;
                        btRemoveDir92.Enabled = true;
                        cbAgreed92.Visible = true;
                    }
                }
            }
            int disagreedTimes = 0;
            int agreedTimes = 0;
            foreach (object[] appEntrData in appEntrances)
                if (appEntrData[2] as DateTime? != null && appEntrData[7] as DateTime? == null)
                    foreach (TabPage page in tcDirections.TabPages)
                        foreach (Control control in page.Controls)
                        {
                            ComboBox cb = control as ComboBox;
                            if (cb != null && cb.SelectedValue != null && ((DirTuple)cb.SelectedValue).Item1 == (uint)appEntrData[0] && ((DirTuple)cb.SelectedValue).Item2 == appEntrData[1].ToString()
                                && ((DirTuple)cb.SelectedValue).Item4 == (uint)appEntrData[5] && ((DirTuple)cb.SelectedValue).Item5 == (uint)appEntrData[4])
                                foreach (Control c in page.Controls)
                                {
                                    CheckBox chb = c as CheckBox;
                                    if (chb != null && chb.Name == "cbAgreed" + cb.Name.Substring(11))
                                    {
                                        chb.Checked = true;
                                        cbAgreed.Checked = true;
                                        cbOriginal.Enabled = false;
                                        agreedTimes++;
                                    }
                                }
                        }
                else if (appEntrData[2] as DateTime? != null && appEntrData[7] as DateTime? != null)
                    disagreedTimes++;

            if (agreedTimes != 0 || disagreedTimes != 0)
                BlockDirChange();
            if (agreedTimes == 0 && disagreedTimes < _AgreedChangeMaxCount)
                ChangeAgreedChBs(true);
        }


        private void UpdateBasic()
        {
            _DB_Connection.Update(DB_Table.ENTRANTS, new Dictionary<string, object> { { "email", mtbEMail.Text }, { "home_phone", string.Concat(mtbHomePhone.Text.Where(s => char.IsNumber(s))) },
                { "mobile_phone", string.Concat(mtbMobilePhone.Text.Where(s => char.IsNumber(s))) } }, new Dictionary<string, object> { { "id", _EntrantID } });

            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;
            Random randNumber = new Random();

            _DB_Connection.Update(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "needs_hostel", cbHostelNeeded.Checked}, { "edit_time", _EditingDateTime},
               { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "first_high_edu", firstHightEdu}, { "mcado", cbMCADO.Checked}, { "chernobyl", cbChernobyl.Checked}, { "passing_examinations",cbExams.Checked },
                { "priority_right", cbPriority.Checked}, { "special_conditions", cbSpecialConditions.Checked} }, new Dictionary<string, object> { { "id", _ApplicationID } });
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
                            { "last_name", tbLastName.Text}, { "first_name", tbFirstName.Text}, { "middle_name", tbMiddleName.Text},
                            { "gender_dict_id", (uint)FIS_Dictionary.GENDER},{ "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER,cbSex.SelectedItem.ToString())},
                            { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                            { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                            { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY}, { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                            { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}, { "reg_region", cbRegion.Text}, { "reg_district", cbDistrict.Text},
                            { "reg_town", cbTown.Text}, { "reg_street", cbStreet.Text}, { "reg_house", cbHouse.Text}, { "reg_flat", tbAppartment.Text}, { "reg_index", tbPostcode.Text} },
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
                        else if ((document[6] as DateTime?) == null && (cbOriginal.Checked))
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
                        string[] fieldNames = new string[] { "document_id", "subject_dict_id", "subject_id", "value", "year", "checked" };
                        string[] keysNames = new string[] { "document_id", "subject_dict_id", "subject_id" };
                        List<object[]> oldData = _DB_Connection.Select(DB_Table.DOCUMENTS_SUBJECTS_DATA, fieldNames, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            });
                        List<object[]> newData = new List<object[]>();
                        foreach (DataGridViewRow row in dgvExams.Rows)
                        {
                            if (int.Parse(row.Cells[3].Value.ToString()) != 0)
                                newData.Add(new object[] { (uint)document[0], 1, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, row.Cells[0].Value.ToString()),
                                    row.Cells[3].Value, row.Cells[dgvExams_Year.Index].Value, (bool)row.Cells[dgvExams_Checked.Index].Value });
                        }
                        _DB_Helper.UpdateData(DB_Table.DOCUMENTS_SUBJECTS_DATA, oldData, newData, fieldNames, keysNames);
                    }
                    else if (document[1].ToString() == "sport")
                    {
                        if (cbSport.Checked)
                        {
                            sportFound = true;
                            uint achevmentCategoryIdNew = 0;
                            switch (SportDoc.diplomaType)
                            {
                                case Classes.DB_Helper.SportDocTypeOlympic:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementOlympic);
                                    break;
                                case Classes.DB_Helper.SportDocTypeParalympic:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementParalympic);
                                    break;
                                case Classes.DB_Helper.SportDocTypeDeaflympic:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementDeaflympic);
                                    break;
                                case Classes.DB_Helper.SportDocTypeWorldChampion:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementWorldChampion);
                                    break;
                                case Classes.DB_Helper.SportDocTypeEuropeChampion:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementEuropeChampion);
                                    break;
                                case Classes.DB_Helper.SportAchievementGTO:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementGTO);
                                    break;
                                case Classes.DB_Helper.SportAchievementWorldChampionship:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementWorldChampionship);
                                    break;
                                case Classes.DB_Helper.SportAchievementEuropeChampionship:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.SportAchievementEuropeChampionship);
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
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "id", (uint)document[0] } });
                        }
                    }

                    else if ((document[1].ToString() == "orphan") || (document[1].ToString() == "disability") || (document[1].ToString() == "medical"))
                    {
                        if ((cbQuote.Checked) && (document[1].ToString() == "orphan") && (_QuoteDoc.cause == "Сиротство"))
                        {
                            qouteFound = true;
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "date", _QuoteDoc.orphanhoodDocDate }, { "organization", _QuoteDoc.orphanhoodDocOrg } },
                                    new Dictionary<string, object> { { "id", (uint)document[0] } });

                            _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "name", _QuoteDoc.orphanhoodDocName}, { "dictionaries_dictionary_id", (uint)FIS_Dictionary.ORPHAN_DOC_TYPE},
                            { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.ORPHAN_DOC_TYPE, _QuoteDoc.orphanhoodDocType)}}, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                            _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                                { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                                { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } },
                            new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                        }
                        else if ((cbQuote.Checked) && ((document[1].ToString() == "disability") || (document[1].ToString() == "medical")) && (_QuoteDoc.cause == "Медецинские показатели"))
                        {
                            qouteFound = true;
                            uint allowEducationDocUid = (uint)(appDocuments.Find(x => x[1].ToString() == "allow_education")[0]);
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "number", _QuoteDoc.conclusionNumber }, { "date", _QuoteDoc.conclusionDate } },
                                new Dictionary<string, object> { { "id", allowEducationDocUid } });

                            if (document[1].ToString() == "disability")
                            {
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", _QuoteDoc.medDocSerie }, { "number", _QuoteDoc.medDocNumber } },
                                    new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> {{ "dictionaries_dictionary_id",(uint)FIS_Dictionary.DISABILITY_GROUP},
                                { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DISABILITY_GROUP, _QuoteDoc.disabilityGroup)} }, new Dictionary<string, object> { { "document_id", (uint)document[0] } });

                                _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object> { { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                                    { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности")}, { "allow_education_document_id", allowEducationDocUid},
                                    { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND}, { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") } },
                                    new Dictionary<string, object> { { "application_id", _ApplicationID }, { "reason_document_id", (uint)document[0] } });
                            }
                            else if (document[1].ToString() == "medical")
                            {
                                qouteFound = true;
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "series", _QuoteDoc.medDocSerie }, { "number", _QuoteDoc.medDocNumber } },
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
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "number", _QuoteDoc.conclusionNumber }, { "date", _QuoteDoc.conclusionDate } },
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
                "edu_source_dict_id", "edu_source_id", "profile_short_name", "target_organization_id" };
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
            _DB_Helper.UpdateData(DB_Table.APPLICATIONS_ENTRANCES, oldD, newD, fieldsList, new string[] { "application_id", "faculty_short_name", "direction_id", "edu_form_dict_id", "edu_form_id",
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
                            if ((comboBox != null) && (comboBox.Name == "cbDirection" + cb.Name.Substring(8)) && (comboBox.SelectedIndex != -1))
                                _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "is_agreed_date", DateTime.Now } },
                                    new Dictionary<string, object> { { "faculty_short_name", ((DirTuple)comboBox.SelectedValue).Item2 },
                                                { "direction_id", ((DirTuple)comboBox.SelectedValue).Item1 }, { "edu_form_id", eduForm }, { "edu_source_id", eduSource } });
                        }
                    }
                }
                List<object[]> applDirs = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "faculty_short_name", "direction_id", "edu_form_id", "edu_source_id", "is_agreed_date", "is_disagreed_date" },
                    new List<Tuple<string, Relation, object>>
                    {
                    new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID)
                    });
                foreach (Control control in page.Controls)
                {
                    CheckBox cb = control as CheckBox;
                    if (cb != null && !cb.Checked )
                    {
                        foreach (Control c in page.Controls)
                        {
                            ComboBox combo = c as ComboBox;
                            if (combo != null && combo.Name == "cbDirection" + cb.Name.Substring(8) && combo.SelectedValue != null)
                                foreach (object[] record in applDirs)
                                    if ((uint)record[2] == eduForm && (uint)record[3] == eduSource && record[0].ToString() == ((DirTuple)combo.SelectedValue).Item2
                                        && (uint)record[1] == ((DirTuple)combo.SelectedValue).Item1 && record[4] as DateTime? != null && record[5] as DateTime? == null)
                                        _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object> { { "is_disagreed_date", DateTime.Now } },
                                    new Dictionary<string, object> { { "faculty_short_name", ((DirTuple)combo.SelectedValue).Item2 },
                                                { "direction_id", ((DirTuple)combo.SelectedValue).Item1 }, { "edu_form_id", eduForm }, { "edu_source_id", eduSource } });
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

        private void UpdateData(DB_Table table, List<object[]> oldDataList, List<object[]> newDataList, string[] fieldNames, bool autoGeneratedKey, string[] keyFieldsNames)
        {
            List<object[]> oldList = oldDataList;
            List<object[]> newList = newDataList;
            int j = 0;
            while (j < oldList.Count && newList.Count != 0)
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

        private void ChangeAgreedChBs(bool isEnabled)
        {
            if ((isEnabled && cbOriginal.Checked) || !isEnabled)
            foreach (TabPage page in tcDirections.TabPages)
                foreach (Control control in page.Controls)
                {
                    CheckBox cb = control as CheckBox;
                    if (cb != null)
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
    }
}
