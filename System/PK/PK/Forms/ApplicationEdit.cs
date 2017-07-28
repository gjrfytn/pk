using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SharedClasses.DB;

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

        public struct MODoc
        {
            public string olympName;
            public string olypmOrg;
            public DateTime olympDate;
        }


        public ODoc OlympicDoc;
        public SDoc SportDoc;
        public MODoc MADIOlympDoc;

        private readonly DB_Connector _DB_Connection;
        private readonly DB_Helper _DB_Helper;
        private readonly Classes.KLADR _KLADR;

        private uint? _ApplicationID;
        private uint? _EntrantID;
        private uint _CurrCampainID;
        private string _RegistratorLogin;
        private DateTime _EditingDateTime;
        private bool _Loading;
        private uint? _TargetOrganizationID;
        private bool _Agreed;
        private QDoc _QuoteDoc;
        private string[] _DirsMed = { "13.03.02", "23.03.01", "23.03.02", "23.03.03", "23.05.01", "23.05.02" };
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

        private const int _AgreedChangeMaxCount = 2;
        private const int _SelectedDirsMaxCount = 5;

        public ApplicationEdit(DB_Connector connection, uint campaignID, string registratorsLogin, uint? applicationId)
        {
            _DB_Connection = connection;
            _RegistratorLogin = registratorsLogin;

            #region Components
            InitializeComponent();

            dgvExams_EGE.ValueType = typeof(ushort);

            if (_DB_Connection.Select(
                DB_Table.USERS,
                new string[] { "role" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("login", Relation.EQUAL, _RegistratorLogin) }
                )[0][0].ToString() == "registrator")
                btWithdraw.Visible = false;

#if !DEBUG
            btFillRand.Visible = false;
#endif
            #endregion

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

            _DB_Helper = new DB_Helper(_DB_Connection);
            _KLADR = new Classes.KLADR(connection.User, connection.Password);
            _CurrCampainID = campaignID;
            _ApplicationID = applicationId;

            FillComboBox(cbIDDocType, FIS_Dictionary.IDENTITY_DOC_TYPE);
            FillComboBox(cbSex, FIS_Dictionary.GENDER);
            FillComboBox(cbNationality, FIS_Dictionary.COUNTRY);
            cbFirstTime.SelectedIndex = 0;
            cbForeignLanguage.SelectedIndex = 0;
            rbCertificate.Checked = false;
            rbCertificate.Checked = true;
            cbRegion.Items.AddRange(_KLADR.GetRegions().ToArray());
            dtpDateOfBirth.MaxDate = DateTime.Now;
            dtpIDDocDate.MaxDate = DateTime.Now;
            tbMobilePhone.Text = tbMobilePhone.Tag.ToString();
            tbMobilePhone.ForeColor = System.Drawing.Color.Gray;
            tbHomePhone.Text = tbHomePhone.Tag.ToString();
            tbHomePhone.ForeColor = System.Drawing.Color.Gray;
            BlockExam();

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

            object[] minMarks = _DB_Connection.Select(DB_Table.CONSTANTS, new string[] { "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark", "min_foreign_mark" })[0];
            dgvExams.Rows.Add("Математика", null, null, (ushort)0, minMarks[0], false);
            dgvExams.Rows.Add("Русский язык", null, null, (ushort)0, minMarks[1], false);
            dgvExams.Rows.Add("Физика", null, null, (ushort)0, minMarks[2], false);
            dgvExams.Rows.Add("Обществознание", null, null, (ushort)0, minMarks[3], false);
            dgvExams.Rows.Add("Иностранный язык", null, null, (ushort)0, minMarks[4], false);

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
                        eduFormName = DB_Helper.EduFormO;
                        break;
                    case "oz":
                        eduFormName = DB_Helper.EduFormOZ;
                        break;
                    case "z":
                        eduFormName = DB_Helper.EduFormZ;
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
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, DB_Helper.EduSourceB);
                    }
                else if (tab.Name.Split('_')[1] == "target")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, DB_Helper.EduSourceT);
                    }
                else if (tab.Name.Split('_')[1] == "quote")
                    foreach (Control c in tab.Controls)
                    {
                        ComboBox cb = c as ComboBox;
                        if (cb != null)
                            FillDirectionsProfilesCombobox(cb, false, eduFormName, DB_Helper.EduSourceQ);
                    }
            }
            if (_ApplicationID != null)
            {
                _Loading = true;
                LoadApplication(true);
                _Loading = false;
            }
        }

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _KLADR.Dispose();

                    if (components != null)
                        components.Dispose();
                }

                base.Dispose(disposing);
            }
        }
        #endregion

        private void cbSpecial_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbQuote.Checked) && (!_Loading))
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
            if ((cbSport.Checked) && (!_Loading))
            {
                SportDocs form = new SportDocs(_DB_Connection, this);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (SportDoc.diplomaType == null || SportDoc.diplomaType == ""))
                    cbSport.Checked = false;
            }
        }

        private void cbMADIOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbMADIOlympiad.Checked) && (!_Loading))
            {
                MADIOlymps form = new MADIOlymps(_DB_Connection, MADIOlympDoc);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (form.OlympName == null || form.OlympName == ""))
                    cbMADIOlympiad.Checked = false;
                else
                {
                    MADIOlympDoc.olympName = form.OlympName;
                    MADIOlympDoc.olypmOrg = form.OlympOrg;
                    MADIOlympDoc.olympDate = form.OlympDate;
                }
            }
        }

        private void cbOlympiad_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbOlympiad.Checked) && (!_Loading))
            {
                Olymps form = new Olymps(_DB_Connection, this);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (OlympicDoc.olympType == null || OlympicDoc.olympType == ""))
                    cbOlympiad.Checked = false;
            }
            DirectionDocEnableDisable();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (cbIDDocType.SelectedItem.ToString() == DB_Helper.PassportName && (string.IsNullOrWhiteSpace(tbLastName.Text)
                || string.IsNullOrWhiteSpace(tbFirstName.Text) || string.IsNullOrWhiteSpace(tbIDDocSeries.Text) || string.IsNullOrWhiteSpace(tbIDDocNumber.Text)
                || string.IsNullOrWhiteSpace(tbPlaceOfBirth.Text) || string.IsNullOrWhiteSpace(cbRegion.Text) || string.IsNullOrWhiteSpace(tbPostcode.Text)))
                MessageBox.Show("Обязательные поля в разделе \"Из паспорта\" не заполнены.");
            else if (cbIDDocType.SelectedItem.ToString() == DB_Helper.PassportName && !mtbSubdivisionCode.MaskFull)
                MessageBox.Show("Код подразделения в разделе \"Из паспорта\" не заполнен.");
            else if (cbIDDocType.SelectedItem.ToString() == DB_Helper.PassportName && cbNationality.SelectedItem.ToString() != DB_Helper.NationalityRus)
                MessageBox.Show("Для паспорта РФ возможно только российское гражданство.");
            else if ((cbIDDocType.SelectedItem.ToString() == DB_Helper.ResidenceName || cbIDDocType.SelectedItem.ToString() == DB_Helper.IntPassportName)
                && cbNationality.SelectedItem.ToString() == DB_Helper.NationalityRus)
                MessageBox.Show("Для вида на жительство или паспорта гражданина иностранного государства невозможно выбрать российское гражданство.");
            else if ((rbDiploma.Checked || rbSpravka.Checked || rbCertificate.Checked && (int)cbGraduationYear.SelectedItem < 2014)
                && (string.IsNullOrWhiteSpace(tbEduDocSeries.Text) || string.IsNullOrWhiteSpace(tbEduDocNumber.Text)))
                MessageBox.Show("Обязательные поля в разделе \"Из аттестата\" не заполнены.");
            else
            {
                bool certificateOK = true;
                if ((int)cbGraduationYear.SelectedItem >= 2014 && rbCertificate.Checked && (!string.IsNullOrWhiteSpace(tbEduDocSeries.Text) || tbEduDocNumber.Text.Length != 14))
                    if (!SharedClasses.Utility.ShowChoiceMessageBox("Серия и номер не соответствуют российскому аттестату 2014 года и новее. Выполнить сохранение?", "Нестандартные данные аттестата"))
                        certificateOK = false;

                if (certificateOK)
                {
                    List<DirTuple> selectedDirs = new List<DirTuple>();
                    bool found = false;
                    bool twice = false;
                    foreach (TabPage tab in tcDirections.TabPages)
                        foreach (Control control in tab.Controls)
                        {
                            ComboBox cb = control as ComboBox;
                            if ((cb != null) && (cb.SelectedIndex != -1))
                            {
                                found = true;
                                foreach (DirTuple dir in selectedDirs)
                                    if (dir.Equals((DirTuple)cb.SelectedValue))
                                    {
                                        twice = true;
                                        break;
                                    }
                                selectedDirs.Add((DirTuple)cb.SelectedValue);
                            }
                            if (twice)
                                break;
                        }
                    if (!found)
                        MessageBox.Show("Не выбрано ни одно направление или профиль.");
                    else if (twice)
                        MessageBox.Show("В одном потоке (вкладке) выбраны два одинаковых направления/профиля.");
                    else
                    {
                        bool quoteOK = false;
                        bool targetOK = false;
                        if (cbQuote.Checked || cbTarget.Checked)
                        {
                            foreach (TabPage page in tcDirections.TabPages)
                                if (page.Name.Split('_')[1] == "quote")
                                    foreach (Control control in page.Controls)
                                    {
                                        ComboBox combo = control as ComboBox;
                                        if (combo != null && combo.SelectedIndex != -1)
                                            quoteOK = true;
                                    }
                                else if (page.Name.Split('_')[1] == "target")
                                    foreach (Control control in page.Controls)
                                    {
                                        ComboBox combo = control as ComboBox;
                                        if (combo != null && combo.SelectedIndex != -1)
                                            targetOK = true;
                                    }
                        }
                        if (cbQuote.Checked && !quoteOK)
                            MessageBox.Show("Выбрана особая квота, но не указано направление на вкладках особой квоты.");
                        else if (cbTarget.Checked && !targetOK)
                            MessageBox.Show("Выбрана целевая организация, но не указано направление на вкладках целевых направлений.");
                        else
                        {
                            bool egeFound = false;
                            bool egePassportDataOK = true;
                            foreach (DataGridViewRow row in dgvExams.Rows)
                                if ((ushort)row.Cells[dgvExams_EGE.Index].Value > 0)
                                {
                                    egeFound = true;
                                    if (string.IsNullOrWhiteSpace(row.Cells[dgvExams_Number.Index].Value as string))
                                        egePassportDataOK = false;
                                }
                            if (egeFound || cbExams.Checked || SharedClasses.Utility.ShowChoiceMessageBox("Не указаны результаты ЕГЭ и не отмечен пункт \"Сдает экзамены\". Выполнить сохранение?", "Отсутствует ЕГЭ и \"Сдает экзамены\""))
                                if (!egePassportDataOK)
                                    MessageBox.Show("Для результата ЕГЭ не указаны паспортные данные.");
                                else if (string.IsNullOrWhiteSpace(mtbEMail.Text))
                                    MessageBox.Show("Поле \"Email\" не заполнено");
                                else if (!cbAppAdmission.Checked
                                    || (cbChernobyl.Checked || cbQuote.Checked || cbOlympiad.Checked || cbPriority.Checked || cbTarget.Checked || cbCompatriot.Checked) && !cbDirectionDoc.Checked
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
                                            && !SharedClasses.Utility.ShowChoiceMessageBox("Значения некоторых полей дат не были изменены. Продолжить?", "Даты не изменены"))
                                            dateOk = false;
                                        if (dateOk)
                                        {
                                            Cursor.Current = Cursors.WaitCursor;

                                            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
                                            {
                                                uint applID;
                                                if (!_ApplicationID.HasValue)
                                                    applID = SaveApplication(transaction);
                                                else
                                                {
                                                    applID = _ApplicationID.Value;
                                                    _EditingDateTime = DateTime.Now;
                                                    UpdateApplication(transaction);
                                                }

                                                transaction.Commit();

                                                if (!_ApplicationID.HasValue)
                                                    ChangeAgreedChBs(true);

                                                _ApplicationID = applID;
                                                btPrint.Enabled = true;
                                                btWithdraw.Enabled = true;
                                            }

                                            Cursor.Current = Cursors.Default;
                                        }
                                    }
                                }
                        }
                    }
                }
            }
        } //Сохранение!!!

        private void cbPassportMatch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPassportMatch.Checked && !_Loading)
            {
                foreach (DataGridViewRow row in dgvExams.Rows)
                {
                    row.Cells[dgvExams_Series.Index].Value = tbIDDocSeries.Text;
                    row.Cells[dgvExams_Number.Index].Value = tbIDDocNumber.Text;
                }
                dgvExams_Series.ReadOnly = true;
                dgvExams_Number.ReadOnly = true;
            }
            else
            {
                dgvExams_Series.ReadOnly = false;
                dgvExams_Number.ReadOnly = false;
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
                cbEduDoc.Enabled = true;
                cbCertificateHRD.Enabled = false;
                cbCertificateHRD.Checked = false;
                cbInstitutionType.Items.Clear();
                foreach (string[] type in _InstitutionTypes.Where(s => s[1] == "middle_edu_diploma" || s[1] == "high_edu_diploma"))
                    cbInstitutionType.Items.Add(type[0]);
                cbInstitutionType.SelectedIndex = 0;
            }
            else if (rbSpravka.Checked)
            {
                cbCertificateHRD.Enabled = true;
                cbEduDoc.Enabled = false;
                cbEduDoc.Checked = false;
                cbInstitutionType.Items.Clear();
                foreach (string[] type in _InstitutionTypes)
                    cbInstitutionType.Items.Add(type[0]);
                cbInstitutionType.SelectedIndex = 0;
            }
            if (cbOriginal.Checked && _ApplicationID != null)
            {
                int agreedCount = 0;
                foreach (object[] data in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "is_agreed_date" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("application_id", Relation.EQUAL,_ApplicationID)
                        }))
                    if ((data[0] as DateTime?) != null)
                        agreedCount++;
                if (agreedCount <= 2)
                    ChangeAgreedChBs(true);
            }
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
            if (MessageBox.Show("Выполнить сохранение?","Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
                {
                    _EditingDateTime = DateTime.Now;
                    UpdateApplication(transaction);
                    transaction.Commit();
                }
                Cursor.Current = Cursors.Default;
            }
            ApplicationDocsPrint form = new ApplicationDocsPrint(_DB_Connection, _ApplicationID.Value);
            form.ShowDialog();
        }

        private void cbTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTarget.Checked && !_Loading)
            {
                TargetOrganizationSelect form = new TargetOrganizationSelect(_DB_Connection, _TargetOrganizationID);
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
                        {
                            cb.Enabled = true;
                            FillDirectionsProfilesCombobox(cb, false, DB_Helper.EduFormO, DB_Helper.EduSourceT);
                        }
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
                        {
                            cb.Enabled = true;
                            FillDirectionsProfilesCombobox(cb, false, DB_Helper.EduFormOZ, DB_Helper.EduSourceT);
                        }
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
                    {
                        cb.Enabled = false;
                        FillDirectionsProfilesCombobox(cb, false, DB_Helper.EduFormO, DB_Helper.EduSourceT);
                    }
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
                    {
                        cb.Enabled = false;
                        FillDirectionsProfilesCombobox(cb, false, DB_Helper.EduFormOZ, DB_Helper.EduSourceT);
                    }
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
                    {
                        cb.Enabled = true;
                        FillDirectionsProfilesCombobox(cb, false, DB_Helper.EduFormO, DB_Helper.EduSourceT);
                    }
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
                    {
                        cb.Enabled = true;
                        FillDirectionsProfilesCombobox(cb, false, DB_Helper.EduFormOZ, DB_Helper.EduSourceT);
                    }
                    Button bt = c as Button;
                    if (bt != null)
                        bt.Enabled = true;
                    Label lb = c as Label;
                    if (lb != null)
                        lb.Enabled = true;
                }
            }
            DirectionDocEnableDisable();
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
            else if ((tabNumber == 1) || (tabNumber == 3) || (tabNumber == 7) || (tabNumber == 9))
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
                if ((cb != null) && (cb.Name == "cbDirection" + tabNumber + comboNumber))
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
            tbPostcode.Text = "Поиск...";
            btGetIndex.Enabled = false;
            cbRegion.Enabled = false;
            cbDistrict.Enabled = false;
            cbTown.Enabled = false;
            cbStreet.Enabled = false;
            cbHouse.Enabled = false;
            tbAppartment.Enabled = false;

            btSave.Enabled = false;

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

            btSave.Enabled = true;
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
                        else if (SharedClasses.Utility.ShowChoiceMessageWithConfirmation("Дать согласие на зачисление на данную специальность?", "Согласие на зачисление"))
                        {
                            ChangeAgreedChBs(false);
                            BlockDirChange();
                            ((CheckBox)sender).Enabled = true;
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
                    int agreedCount = 0;
                    foreach (object[] data in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "is_disagreed_date", "is_agreed_date" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("application_id", Relation.EQUAL,_ApplicationID)
                            }))
                    {
                        if ((data[0] as DateTime?) != null)
                            disagreedCount++;
                        if ((data[1] as DateTime?) != null)
                            agreedCount++;
                    }
                    if (disagreedCount >= _AgreedChangeMaxCount)
                    {
                        MessageBox.Show("Нельзя изменить согласие на зачисление больше " + _AgreedChangeMaxCount + " раз.");
                        _Agreed = true;
                        ((CheckBox)sender).Checked = true;
                    }
                    else if (SharedClasses.Utility.ShowChoiceMessageWithConfirmation("Отменить согласие на зачисление на данную специальность?", "Согласие на зачисление"))
                    {
                        if (agreedCount == _AgreedChangeMaxCount && disagreedCount == _AgreedChangeMaxCount - 1)
                            ((CheckBox)sender).Enabled = false;
                        else
                            ChangeAgreedChBs(true);
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
            if (char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == '-' || e.KeyChar == '(' || e.KeyChar == ')')
                return;
            else
                e.Handled = true;
        }

        private void tbCyrillic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 'А' && e.KeyChar <= 'я' || e.KeyChar == '\b' || e.KeyChar == '.' || e.KeyChar == '-' || e.KeyChar == 'ё' || e.KeyChar == 'Ё' || e.KeyChar == ' ')
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
            if (SharedClasses.Utility.ShowChoiceMessageWithConfirmation("Забрать документы?", "Забрать документы"))
            {
                Cursor.Current = Cursors.WaitCursor;

                using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
                {
                    _EditingDateTime = DateTime.Now;
                    UpdateApplication(transaction);

                    _DB_Connection.Update(
                        DB_Table.APPLICATIONS,
                        new Dictionary<string, object> { { "status", "withdrawn" }, { "withdraw_date", DateTime.Now.Date } },
                        new Dictionary<string, object> { { "id", _ApplicationID } },
                        transaction
                        );

                    transaction.Commit();
                }
                foreach (Control control in panel1.Controls)
                    control.Enabled = false;
                btClose.Enabled = true;

                Cursor.Current = Cursors.Default;
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
                    if (ushort.Parse(e.FormattedValue.ToString()) > 100)
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
            DataGridView dgv = (DataGridView)sender;
            if (dgv.CurrentCell != null && dgv.CurrentCell.ColumnIndex == dgvExams_EGE.Index)
            {
                if (dgv.CurrentCell.Value.ToString() == "")
                    dgv.CurrentCell.Value = (ushort)0;

                if ((ushort)dgv.CurrentCell.Value != 0 && (ushort)dgv.CurrentCell.Value < (ushort)dgv[dgvExams_Min.Index, dgv.CurrentCell.RowIndex].Value)
                    dgv.CurrentCell.Style.BackColor = System.Drawing.Color.LightPink;
                else
                    dgv.CurrentCell.Style.BackColor = System.Drawing.Color.White;

                cbOlympiad.Enabled = false;
                foreach (DataGridViewRow row in dgvExams.Rows)
                    if ((ushort)row.Cells[dgvExams_EGE.Index].Value >= 75)
                        cbOlympiad.Enabled = true;
            }
            if (dgv.CurrentRow != null && !_Loading && _ApplicationID.HasValue)
                dgv.CurrentRow.Cells[dgvExams_Checked.Index].Value = false;
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
            mtbSubdivisionCode.Text = "";
            tbEduDocSeries.Text = "";
            tbEduDocNumber.Text = "";
            tbInstitutionNumber.Text = "";
            tbPostcode.Text = "";
            mtbEMail.Text = "";
            tbMobilePhone.Text = "";
            tbHomePhone.Text = "";

            Random rand = new Random();
            int stringLength = 10;
            int numberLength = 5;
            char[] cyrillicLetters = { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т',
                'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я' };
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] latinLetters = { 'a', 'b', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            string[] emailEndings = { "gmail.com", "yandex.ru", "rambler.ru", "list.ru", "mail.ru" };

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
                tbInstitutionNumber.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
                if ((int)cbGraduationYear.SelectedItem < 2014)
                {
                    tbEduDocSeries.Text += digits[rand.Next(digits.Length)];
                    tbEduDocNumber.Text += digits[rand.Next(digits.Length)];
                }
            }
            if ((int)cbGraduationYear.SelectedItem >= 2014)
            {
                for (int i = 0; i < 14; i++)
                    tbEduDocNumber.Text += digits[rand.Next(digits.Length)];
                tbEduDocSeries.Text = "";
            }
            string subCode = "";
            for (int i = 0; i < 6; i++)
            {
                tbPostcode.Text += digits[rand.Next(digits.Length)];
                subCode += digits[rand.Next(digits.Length)];
            }
            mtbSubdivisionCode.Text = subCode;
            for (int i = 0; i < 10; i++)
                mtbEMail.Text += latinLetters[rand.Next(latinLetters.Length)];
            mtbEMail.Text += "@" + emailEndings[rand.Next(emailEndings.Length)];
            string phone = "";
            if (rand.Next(2) > 0)
                for (int i = 0; i < 10; i++)
                    phone += digits[rand.Next(digits.Length)];
            tbMobilePhone.Text = phone;
            phone = "";
            if (rand.Next(2) > 0)
                for (int i = 0; i < 10; i++)
                    phone += digits[rand.Next(digits.Length)];
            tbHomePhone.Text = phone;

            cbSex.SelectedIndex = rand.Next(cbSex.Items.Count);
            cbRegion.SelectedIndex = rand.Next(cbRegion.Items.Count);
            cbInstitutionType.SelectedIndex = rand.Next(cbInstitutionType.Items.Count);
            cbFirstTime.SelectedIndex = rand.Next(cbFirstTime.Items.Count);
            cbForeignLanguage.SelectedIndex = rand.Next(cbForeignLanguage.Items.Count);
        }

        private void cbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedDirsCount = 0;
            foreach (TabPage page in tcDirections.TabPages)
            {
                bool pageFilled = false;
                foreach (Control control in page.Controls)
                {
                    ComboBox combo = control as ComboBox;
                    if (combo != null && combo.SelectedIndex != -1)
                    {
                        pageFilled = true;
                    }
                }
                if (pageFilled)
                    selectedDirsCount++;
            }
            if (!_Loading && selectedDirsCount > _SelectedDirsMaxCount)
            {
                ((ComboBox)sender).SelectedIndex = -1;
                MessageBox.Show("Нельзя выбрать более " + _SelectedDirsMaxCount + " потоков.");
            }

            bool medCertificateNeeded = false;
            foreach (TabPage page in tcDirections.TabPages)
                foreach (Control control in page.Controls)
                {
                    ComboBox combo = control as ComboBox;
                    if (combo != null && combo.SelectedIndex != -1 && _DirsMed.Contains(_DB_Helper.GetDirectionNameAndCode(((DirTuple)combo.SelectedValue).Item1).Item2))
                    {
                        medCertificateNeeded = true;
                    }
                }
            if (medCertificateNeeded)
                cbMedCertificate.Enabled = true;
            else if (!_Loading)
            {
                cbMedCertificate.Enabled = false;
                cbMedCertificate.Checked = false;
            }
        }

        private void cbIDDocType_SelectedIndexChanged(object sender, EventArgs e)//TODO
        {
            if (cbIDDocType.Text == "Паспорт гражданина РФ")
            {
                if (tbIDDocSeries.TextLength > 4)
                    tbIDDocSeries.Clear();

                tbIDDocSeries.MaxLength = 4;

                if (tbIDDocNumber.TextLength > 6)
                    tbIDDocNumber.Clear();

                tbIDDocNumber.MaxLength = 6;
            }
            else
            {
                tbIDDocSeries.MaxLength = 20;
                tbIDDocNumber.MaxLength = 100;
            }
        }

        private void ApplicationEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !SharedClasses.Utility.ShowFormCloseMessageBox();
        }

        private void tbPhone_Enter(object sender, EventArgs e)
        {
            if ((sender as TextBox).ForeColor == System.Drawing.Color.Gray)
            {
                (sender as TextBox).Text = "";
                (sender as TextBox).ForeColor = System.Drawing.Color.Black;
            }
        }

        private void tbPhone_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = (sender as TextBox).Tag.ToString();
                (sender as TextBox).ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void tbPhone_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text != (sender as TextBox).Tag.ToString())
                (sender as TextBox).ForeColor = System.Drawing.Color.Black;
        }

        private void cbAttribute_CheckedChanged(object sender, EventArgs e)
        {
            DirectionDocEnableDisable();
        }

        private void tbIDDoc_Leave(object sender, EventArgs e)
        {
            List<object[]> passportFound = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "id" },
                new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("type", Relation.EQUAL, "identity"),
                                new Tuple<string, Relation, object>("series", Relation.EQUAL, tbIDDocSeries.Text),
                                new Tuple<string, Relation, object>("number", Relation.EQUAL, tbIDDocNumber.Text)
                            });
            if (passportFound.Count > 0)
            {
                List<object[]> oldApplications = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "status", "campaign_id", "id" }, new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "applications_id" }, new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("documents_id", Relation.EQUAL, (uint)passportFound[0][0])})[0][0])
                                });
                foreach (object[] app in oldApplications)
                    if ((uint)app[1] == _CurrCampainID && (uint)app[2] != _ApplicationID)
                    {
                        if (app[0].ToString() != "withdrawn")
                        {
                            MessageBox.Show("В данной кампании уже существует действующее заявление на этот паспорт.");
                            tbIDDocSeries.Text = "";
                            tbIDDocNumber.Text = "";
                            break;
                        }
                        else if (SharedClasses.Utility.ShowChoiceMessageBox("В данной кампании уже существует заявление на этот паспорт, по которому забрали документы. Загрузить данные из прошлого заявления?", "Паспорт уже существует"))
                        {
                            _ApplicationID = (uint)app[2];
                            _Loading = true;
                            LoadApplication(false);
                            _Loading = false;
                            _ApplicationID = null;
                            cbDirectionDoc.Checked = false;
                        }
                        else
                        {
                            tbIDDocSeries.Text = "";
                            tbIDDocNumber.Text = "";
                            break;
                        }
                    }
            }
        }


        private uint SaveApplication(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            uint applID = SaveBasic(transaction);
            SaveDiploma(applID, transaction);
            SaveExams(applID, transaction);
            if (cbQuote.Checked)
                SaveQuote(applID, transaction);
            if (cbSport.Checked)
                SaveSport(applID, transaction);
            if (cbMADIOlympiad.Checked || cbOlympiad.Checked)
                SaveOlympic(applID, transaction);
            if (cbMedCertificate.Checked)
                SaveCertificate(applID, transaction);
            SaveDirections(applID, transaction);

            return applID;
        }

        private void LoadApplication(bool fullLoad)
        {
            Cursor.Current = Cursors.WaitCursor;
            LoadBasic(fullLoad);
            LoadDocuments(fullLoad);
            LoadMarks();
            if (fullLoad)
                LoadDirections();
            Cursor.Current = Cursors.Default;
        }

        private void UpdateApplication(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            UpdateBasic(transaction);
            UpdateEGEMarks(transaction);
            UpdateDocuments(transaction);
            UpdateDirections(transaction);
        }


        private uint SaveBasic(MySql.Data.MySqlClient.MySqlTransaction transaction)
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
                    { "email", mtbEMail.Text.Trim() },
                    { "personal_password", password },
                    { "home_phone", tbHomePhone.Text!=tbHomePhone.Tag.ToString()? tbHomePhone.Text.Trim() : null},
                    { "mobile_phone", tbMobilePhone.Text!=tbMobilePhone.Tag.ToString()? tbMobilePhone.Text.Trim() : null}
                }, transaction);
            }
            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;

            uint applID = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object>
            {
                { "entrant_id", _EntrantID.Value},
                { "registration_time", DateTime.Now},
                { "needs_hostel", cbHostelNeeded.Checked},
                { "registrator_login", _RegistratorLogin},
                { "campaign_id", _CurrCampainID },
                { "language", cbForeignLanguage.SelectedItem.ToString() },
                { "first_high_edu", firstHightEdu},
                { "mcado", cbMCADO.Checked},
                { "chernobyl", cbChernobyl.Checked},
                { "passing_examinations",cbExams.Checked },
                { "priority_right", cbPriority.Checked},
                { "special_conditions", cbSpecialConditions.Checked},
                { "master_appl", false},
                { "compatriot", cbCompatriot.Checked },
                { "courses", cbCourses.Checked }
            }, transaction);

            uint idDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
            {
                { "type", "identity" },
                { "series", tbIDDocSeries.Text.Trim()},
                { "number", tbIDDocNumber.Text.Trim()},
                { "date", dtpIDDocDate.Value} ,
                { "organization", tbIssuedBy.Text.Trim()}
            }, transaction);

            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
            {
                { "applications_id", applID},
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
                    { "applications_id", applID },
                    { "documents_id", otherDocID }
                }, transaction);
            }

            return applID;
        }

        private void SaveDiploma(uint applID, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            string eduDocType = "";
            if (!rbSpravka.Checked)
                eduDocType = _InstitutionTypes.First(s => s[0] == cbInstitutionType.SelectedItem.ToString())[1];
            else
                eduDocType = "academic_diploma";

            uint eduDocID = 0;
            if (cbOriginal.Checked)
            {
                eduDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", eduDocType },
                    { "series",  tbEduDocSeries.Text.Trim()},
                    { "number", tbEduDocNumber.Text.Trim()},
                    { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text.Trim() + "|" + tbInstitutionLocation.Text.Trim()},
                    { "original_recieved_date", DateTime.Now}
                }, transaction);
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "document_id", eduDocID },
                    { "year", cbGraduationYear.SelectedItem }
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", applID },
                    { "documents_id", eduDocID }
                }, transaction);
            }
            else
            {
                eduDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", eduDocType },
                    { "series",  tbEduDocSeries.Text.Trim()},
                    { "number", tbEduDocNumber.Text.Trim()},
                    { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text.Trim() + "|" + tbInstitutionLocation.Text.Trim()}
                }, transaction);
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "document_id", eduDocID },
                    { "year", cbGraduationYear.SelectedItem }
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", applID },
                    { "documents_id", eduDocID }
                }, transaction);
            }
            if (cbMedal.Checked)
            {
                uint insAchievementCategory = 0;
                switch (eduDocType)
                {
                    case "school_certificate":
                        insAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.MedalAchievement);
                        break;
                    case "middle_edu_diploma":
                        insAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.RedMiddleDiploma);
                        break;
                    case "high_edu_diploma":
                    case "academic_diploma":
                        insAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.RedDiplomaAchievement);
                        break;
                }
                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                {
                    { "application_id", applID },
                    { "institution_achievement_id", _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                                new Tuple<string, Relation, object>("category_id", Relation.EQUAL, insAchievementCategory)
                            })[0][0]},
                    { "document_id", eduDocID }
                }, transaction);
            }
        }

        private void SaveQuote(uint applID, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            if (_QuoteDoc.cause == "Сиротство")
            {
                uint orphDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", "orphan" },
                    { "date", _QuoteDoc.orphanhoodDocDate} ,
                    { "organization", _QuoteDoc.orphanhoodDocOrg.Trim()}
                }, transaction);
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "document_id", orphDocUid},
                    { "name", _QuoteDoc.orphanhoodDocName.Trim()},
                    { "dictionaries_dictionary_id", (uint)FIS_Dictionary.ORPHAN_DOC_TYPE},
                    { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.ORPHAN_DOC_TYPE, _QuoteDoc.orphanhoodDocType)}
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", applID },
                    { "documents_id", orphDocUid }
                }, transaction);
                _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                {
                    { "application_id", applID},
                    { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                    { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                    { "reason_document_id", orphDocUid},
                    { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                    { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") }
                }, transaction);
            }
            else if (_QuoteDoc.cause == "Медицинские показатели")
            {
                uint allowEducationDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "number", _QuoteDoc.conclusionNumber.Trim()},
                    { "date", _QuoteDoc.conclusionDate},
                    { "type", "allow_education"}
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", applID },
                    { "documents_id", allowEducationDocUid }
                }, transaction);

                if (_QuoteDoc.medCause == "Справка об установлении инвалидности")
                {
                    uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                    {
                        { "type", "disability" },
                        { "series", _QuoteDoc.medDocSerie.Trim()},
                        { "number", _QuoteDoc.medDocNumber.Trim()}
                    }, transaction);
                    _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                    {
                        { "document_id", medDocUid},
                        { "dictionaries_dictionary_id",(uint)FIS_Dictionary.DISABILITY_GROUP},
                        { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DISABILITY_GROUP, _QuoteDoc.disabilityGroup)}
                    }, transaction);
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                    {
                        { "applications_id", applID },
                        { "documents_id", medDocUid }
                    }, transaction);
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        {
                        { "application_id", applID},
                        { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                        { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности")},
                        { "reason_document_id", medDocUid},
                        { "allow_education_document_id", allowEducationDocUid},
                        { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                        { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") }
                    }, transaction);
                }
                else if (_QuoteDoc.medCause == "Заключение психолого-медико-педагогической комиссии")
                {
                    uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                    {
                        { "type", "medical" },
                        { "series", _QuoteDoc.medDocSerie.Trim()},
                        { "number", _QuoteDoc.medDocNumber.Trim()}
                    }, transaction);
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                    {
                        { "applications_id", applID },
                        { "documents_id", medDocUid }
                    }, transaction);
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        {
                        { "application_id", applID},
                        { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                        { "document_type_id",  _DB_Helper.GetDictionaryItemID( FIS_Dictionary.DOCUMENT_TYPE, "Заключение психолого-медико-педагогической комиссии")},
                        { "reason_document_id", medDocUid},
                        { "allow_education_document_id", allowEducationDocUid},
                        { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                        { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") }
                    }, transaction);
                }
            }
        }

        private void SaveSport(uint applID, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            uint sportDocID;
            if (SportDoc.diplomaType != DB_Helper.SportAchievementGTO && SportDoc.diplomaType != DB_Helper.SportAchievementEuropeChampionship
                && SportDoc.diplomaType != DB_Helper.SportAchievementWorldChampionship)
            {
                sportDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", "sport" },
                    { "date", SportDoc.docDate},
                    { "organization", SportDoc.orgName.Trim() }
                }, transaction);
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "document_id", sportDocID},
                    { "name", SportDoc.docName.Trim()},
                    { "dictionaries_dictionary_id", FIS_Dictionary.SPORT_DIPLOMA_TYPE },
                    { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SPORT_DIPLOMA_TYPE, SportDoc.diplomaType) }
                }, transaction);
            }
            else
            {
                sportDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", "custom" },
                    { "date", SportDoc.docDate},
                    { "organization", SportDoc.orgName.Trim() }
                }, transaction);
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "document_id", sportDocID },
                    { "name", SportDoc.docName.Trim() }
                }, transaction);
            }
            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
            {
                { "applications_id", applID },
                { "documents_id", sportDocID }
            }, transaction);

            uint achevmentCategoryId = 0;
            switch (SportDoc.diplomaType)
            {
                case DB_Helper.SportDocTypeOlympic:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementOlympic);
                    break;
                case DB_Helper.SportDocTypeParalympic:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementParalympic);
                    break;
                case DB_Helper.SportDocTypeDeaflympic:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementDeaflympic);
                    break;
                case DB_Helper.SportDocTypeWorldChampion:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementWorldChampion);
                    break;
                case DB_Helper.SportDocTypeEuropeChampion:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementEuropeChampion);
                    break;
                case DB_Helper.SportAchievementGTO:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementGTO);
                    break;
                case DB_Helper.SportAchievementWorldChampionship:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementWorldChampionship);
                    break;
                case DB_Helper.SportAchievementEuropeChampionship:
                    achevmentCategoryId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementEuropeChampionship);
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
                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                    {
                        { "application_id", applID },
                        { "institution_achievement_id", uint.Parse(achievments[0][0].ToString())},
                        { "document_id", sportDocID}
                    }, transaction);
        }

        private void SaveOlympic(uint applID, MySql.Data.MySqlClient.MySqlTransaction transaction)
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
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                        {
                            { "type", docType }
                        }, transaction);

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                        {
                            { "document_id", olympicDocId },
                            { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                            { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) },
                            { "olympic_id", (uint)OlympicDoc.olympID},
                            { "class_number", OlympicDoc.olympClass },
                            { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES },
                            { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) },
                            { "olympic_subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                            { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, OlympicDoc.olympDist) }
                        }, transaction);
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                        {
                            { "applications_id", applID },
                            { "documents_id", olympicDocId }
                        }, transaction);
                        break;

                    case "Диплом победителя/призера всероссийской олимпиады школьников":
                        docType = "olympic_total";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера всероссийской олимпиады школьников");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                        {
                            { "type", docType },
                            { "number", OlympicDoc.olympDocNumber }
                        }, transaction);

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                        {
                            { "document_id", olympicDocId },
                            { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                            { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) },
                            { "olympic_id", (uint)OlympicDoc.olympID},
                            { "class_number", OlympicDoc.olympClass },
                            { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES },
                            { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) },
                            { "olympic_subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                            { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, OlympicDoc.olympDist) }
                        }, transaction);
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                        {
                            { "applications_id", applID },
                            { "documents_id", olympicDocId }
                        }, transaction);
                        break;

                    case "Диплом 4 этапа всеукраинской олимпиады":
                        docType = "ukraine_olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера IV этапа всеукраинской ученической олимпиады");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                        {
                            { "type", docType },
                            { "number", OlympicDoc.olympDocNumber }
                        }, transaction);

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                        {
                            { "document_id", olympicDocId },
                            { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                            { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) },
                            { "olympic_name", OlympicDoc.olympName },
                            { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES },
                            { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) }
                        }, transaction);
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                        {
                            { "applications_id", applID },
                            { "documents_id", olympicDocId }
                        }, transaction);
                        break;

                    case "Диплом международной олимпиады":
                        docType = "international_olympic";
                        benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Документ об участии в международной олимпиаде");
                        olympicDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                        {
                            { "type", docType },
                            { "number", OlympicDoc.olympDocNumber }
                        }, transaction);

                        _DB_Connection.Insert(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                        {
                            { "document_id", olympicDocId },
                            { "olympic_name", OlympicDoc.olympName },
                            { "country_dict_id", (uint)FIS_Dictionary.COUNTRY },
                            { "country_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY, OlympicDoc.country) },
                            { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES },
                            { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) }
                        }, transaction);
                        _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                        {
                            { "applications_id", applID },
                            { "documents_id", olympicDocId }
                        }, transaction);
                        break;
                }
                _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                {
                    { "application_id", applID },
                    { "document_type_dict_id", FIS_Dictionary.DOCUMENT_TYPE },
                    { "document_type_id", benefitDocType},
                    { "reason_document_id", olympicDocId},
                    { "benefit_kind_dict_id", FIS_Dictionary.BENEFIT_KIND },
                    { "benefit_kind_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.BENEFIT_KIND, DB_Helper.BenefitOlympic) }
                }, transaction);
            }
        }

        private void SaveMADIOlympic(uint applID, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            if (cbMADIOlympiad.Checked)
            {
                uint olympDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", "custom" },
                    { "date", MADIOlympDoc.olympDate },
                    { "organization", MADIOlympDoc.olypmOrg.Trim() }
                }, transaction);
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "name", DB_Helper.MADIOlympDocName },
                    { "text_data", MADIOlympDoc.olympName.Trim() },
                    { "document_id", olympDocID }
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", applID },
                    { "documents_id", olympDocID }
                }, transaction);
                List<object[]> achievments = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                    new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("campaign_id", Relation.EQUAL, _CurrCampainID),
                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.OlympAchievementName))
                });

                uint achievementId = 0;
                if (achievments.Count != 0)
                    achievementId = (uint)(achievments[0][0]);

                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                {
                    { "application_id", applID },
                    { "institution_achievement_id", achievementId},
                    { "document_id", olympDocID}
                }, transaction);
            }
        }

        private void SaveExams(uint applID, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            foreach (DataGridViewRow row in dgvExams.Rows)
                if ((ushort)row.Cells[dgvExams_EGE.Index].Value > 0)
                    _DB_Connection.Insert(DB_Table.APPLICATION_EGE_RESULTS, new Dictionary<string, object>
                {
                    { "application_id", applID },
                    { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                    { "subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, row.Cells[dgvExams_Subject.Index].Value.ToString()) },
                    { "series", row.Cells[dgvExams_Series.Index].Value },
                    { "number", row.Cells[dgvExams_Number.Index].Value },
                    { "year", ushort.Parse(row.Cells[dgvExams_Year.Index].Value.ToString()) },
                    { "value", row.Cells[dgvExams_EGE.Index].Value },
                    { "checked", false },
                }, transaction);

            foreach (DataGridViewRow row in dgvExams.Rows)
                if ((ushort)row.Cells[dgvExams_EGE.Index].Value > 0)
                {
                    uint egeDocID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                    {
                        { "type", "ege" },
                        { "series", row.Cells[dgvExams_Series.Index].Value },
                        { "number", row.Cells[dgvExams_Number.Index].Value }
                    }, transaction);
                    _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                    {
                        { "applications_id", applID },
                        { "documents_id", egeDocID }
                    }, transaction);
                    _DB_Connection.Insert(DB_Table.DOCUMENTS_SUBJECTS_DATA, new Dictionary<string, object>
                    {
                        { "document_id", egeDocID },
                        { "value", row.Cells[dgvExams_EGE.Index].Value },
                        { "year", ushort.Parse(row.Cells[dgvExams_Year.Index].Value.ToString()) },
                        { "checked", false },
                        { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                        { "subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, row.Cells[dgvExams_Subject.Index].Value.ToString()) }
                    }, transaction);
                }
        }

        private void SaveCertificate(uint applID, MySql.Data.MySqlClient.MySqlTransaction transaction)
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
                { "applications_id", applID },
                { "documents_id", spravkaID }
            }, transaction);
        }

        private void SaveDirections(uint applID, MySql.Data.MySqlClient.MySqlTransaction transaction)
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
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                                {
                                    { "application_id", applID },
                                    { "faculty_short_name", ((DirTuple)cb.SelectedValue).Item2 },
                                    { "direction_id", ((DirTuple)cb.SelectedValue).Item1},
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM},
                                    { "edu_form_id", ((DirTuple)cb.SelectedValue).Item5},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE},
                                    { "edu_source_id", ((DirTuple)cb.SelectedValue).Item4}
                                }, transaction);
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
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                                {
                                    { "application_id", applID },
                                    { "faculty_short_name",  ((DirTuple)cb.SelectedValue).Item2 },
                                    { "direction_id",((DirTuple)cb.SelectedValue).Item1 },
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM},
                                    { "edu_form_id", ((DirTuple)cb.SelectedValue).Item5},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE},
                                    { "edu_source_id", ((DirTuple)cb.SelectedValue).Item4},
                                    { "profile_short_name", ((DirTuple)cb.SelectedValue).Item6}
                                }, transaction);
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
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                                {
                                    { "application_id", applID },
                                    { "faculty_short_name",  ((DirTuple)cb.SelectedValue).Item2 },
                                    { "direction_id", ((DirTuple)cb.SelectedValue).Item1},
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM},
                                    { "edu_form_id", ((DirTuple)cb.SelectedValue).Item5},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE},
                                    { "edu_source_id", ((DirTuple)cb.SelectedValue).Item4},
                                    { "target_organization_id", _TargetOrganizationID}
                                }, transaction);
                            }
                    }
                }
            }
        }


        private void LoadBasic(bool fullLoad)
        {
            Text += " № " + _ApplicationID;
            object[] application = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "needs_hostel", "language", "first_high_edu",
                "mcado", "chernobyl", "passing_examinations", "priority_right", "special_conditions", "entrant_id", "status", "compatriot", "courses" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, _ApplicationID)
                })[0];
            if (application[9].ToString() == "withdrawn" && fullLoad)
            {
                foreach (Control control in panel1.Controls)
                    control.Enabled = false;
                btClose.Enabled = true;
            }
            else if (application[9].ToString() == "adm_budget" || application[9].ToString() == "adm_paid" || application[9].ToString() == "adm_both")
            {
                btWithdraw.Enabled = false;
                btPrint.Enabled = true;
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
            cbCompatriot.Checked = (bool)application[10];
            cbCourses.Checked = (bool)application[11];
            if (fullLoad)
            {
                cbPassportCopy.Checked = true;
                cbAppAdmission.Checked = true;
            }

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
            tbMobilePhone.Text = entrant[2].ToString();
            tbHomePhone.Text = entrant[1].ToString();
        }

        private void LoadMarks()
        {
            bool passportMatch = true;
            List<object[]> marks = _DB_Connection.Select(DB_Table.APPLICATION_EGE_RESULTS, new string[] { "subject_id", "series", "number", "year", "value", "checked" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID)
                });
            foreach (object[] mark in marks)
                foreach (DataGridViewRow row in dgvExams.Rows)
                    if (_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, row.Cells[dgvExams_Subject.Index].Value.ToString()) == (uint)mark[0])
                    {
                        row.Cells[dgvExams_Year.Index].Value = mark[3].ToString();
                        row.Cells[dgvExams_EGE.Index].Value = (ushort)mark[4];
                        row.Cells[dgvExams_Series.Index].Value = mark[1];
                        row.Cells[dgvExams_Number.Index].Value = mark[2];
                        row.Cells[dgvExams_Checked.Index].Value = (bool)mark[5];

                        if (mark[1].ToString() != tbIDDocSeries.Text || mark[2].ToString() != tbIDDocNumber.Text)
                            passportMatch = false;
                    }
            if (passportMatch && marks.Count > 0)
                cbPassportMatch.Checked = true;

            var exMarks = _DB_Connection.Select(DB_Table.ENTRANTS_EXAMINATIONS_MARKS, new string[] { "examination_id", "mark" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("entrant_id", Relation.EQUAL, _EntrantID)
            });

            var examinations = _DB_Connection.Select(DB_Table.EXAMINATIONS, new string[] { "id", "subject_id", "date" }).Join(
                exMarks,
                ex => ex[0],
                exM => exM[0],
                (s1, s2) => new Tuple<uint, int, DateTime>((uint)s1[1], int.Parse(s2[1].ToString()), (DateTime)s1[2])
                );
            foreach (var examData in examinations)
                foreach (DataGridViewRow row in dgvExams.Rows)
                    if (row.Cells[dgvExams_Subject.Index].Value.ToString() == _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, examData.Item1))
                    {
                        row.Cells[dgvExams_Exam.Index].Value = examData.Item2;
                        row.Cells[dgvExams_Exam.Index].ToolTipText = examData.Item3.ToShortDateString();
                    }
        }

        private void LoadDocuments(bool fullLoad)
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
                else if (document[1].ToString() == "school_certificate" || document[1].ToString() == "middle_edu_diploma"
                    || document[1].ToString() == "high_edu_diploma" || document[1].ToString() == "academic_diploma")
                {
                    if (document[1].ToString() == "school_certificate")
                    {
                        rbCertificate.Checked = true;
                        if (fullLoad)
                            cbEduDoc.Checked = true;
                    }
                    else if (document[1].ToString() == "middle_edu_diploma" || document[1].ToString() == "high_edu_diploma")
                    {
                        rbDiploma.Checked = true;
                        if (fullLoad)
                            cbEduDoc.Checked = true;
                    }
                    else if (document[1].ToString() == "academic_diploma")
                    {
                        rbSpravka.Checked = true;
                        if (fullLoad)
                            cbCertificateHRD.Checked = true;
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
                    cbGraduationYear.SelectedItem = int.Parse(_DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "year" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, document[0])
                    })[0][0].ToString());

                    uint insAchievementCategory = 0;
                    switch (document[1].ToString())
                    {
                        case "school_certificate":
                            insAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.MedalAchievement);
                            break;
                        case "middle_edu_diploma":
                            insAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.RedMiddleDiploma);
                            break;
                        case "high_edu_diploma":
                        case "academic_diploma":
                            insAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.RedDiplomaAchievement);
                            break;
                    }
                    if (GetAppAchievementsByCategory(insAchievementCategory).Count > 0)
                        cbMedal.Checked = true;
                }
                else if (document[1].ToString() == "photos" && fullLoad)
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
                        case DB_Helper.SportAchievementOlympic:
                            SportDoc.diplomaType = DB_Helper.SportDocTypeOlympic;
                            break;
                        case DB_Helper.SportAchievementParalympic:
                            SportDoc.diplomaType = DB_Helper.SportDocTypeParalympic;
                            break;
                        case DB_Helper.SportAchievementDeaflympic:
                            SportDoc.diplomaType = DB_Helper.SportDocTypeDeaflympic;
                            break;
                        case DB_Helper.SportAchievementWorldChampion:
                            SportDoc.diplomaType = DB_Helper.SportDocTypeWorldChampion;
                            break;
                        case DB_Helper.SportDocTypeEuropeChampion:
                            SportDoc.diplomaType = DB_Helper.SportDocTypeEuropeChampion;
                            break;
                        case DB_Helper.SportAchievementGTO:
                            SportDoc.diplomaType = DB_Helper.SportAchievementGTO;
                            break;
                        case DB_Helper.SportAchievementWorldChampionship:
                            SportDoc.diplomaType = DB_Helper.SportAchievementWorldChampionship;
                            break;
                        case DB_Helper.SportAchievementEuropeChampionship:
                            SportDoc.diplomaType = DB_Helper.SportAchievementEuropeChampionship;
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
                    List<object[]> spravkaData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    });
                    if (spravkaData.Count > 0 && spravkaData[0][0].ToString() == DB_Helper.MedCertificate)
                    {
                        if (fullLoad)
                            cbMedCertificate.Checked = true;
                    }
                    else
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
                }
                else if (document[1].ToString() == "olympic")
                {
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
                        OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
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
                        OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
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
                        OlympicDoc.olympDocNumber = int.Parse(document[3].ToString());
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
                        if (documentData[0] != null && documentData[0].ToString() == DB_Helper.MADIOlympDocName)
                        {
                            MADIOlympDoc.olympName = documentData[1].ToString();
                            MADIOlympDoc.olympDate = (DateTime)document[4];
                            MADIOlympDoc.olypmOrg = document[5].ToString();
                            cbMADIOlympiad.Checked = true;
                        }
                        else
                        {
                            SportDoc.docDate = (DateTime)document[4];
                            SportDoc.docName = documentData[0].ToString();
                            SportDoc.orgName = document[5].ToString();
                            cbSport.Checked = true;

                            foreach (object[] achievement in _DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "institution_achievement_id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID)
                                }))
                            {
                                string achievementName = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.IND_ACH_CATEGORIES,
                                    (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" },
                                    new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)achievement[0])
                                })[0][0]);

                                if (achievementName == DB_Helper.SportAchievementGTO || achievementName == DB_Helper.SportAchievementWorldChampionship
                                    || achievementName == DB_Helper.SportAchievementEuropeChampionship)
                                    SportDoc.diplomaType = achievementName;
                            }
                        }
                }
            }
        }

        private void LoadDirections()
        {
            string[][] eduLevelsCodes = new string[][] { new string[] { "03", DB_Helper.EduLevelB }, new string[] { "04", DB_Helper.EduLevelM }, new string[] { "05", DB_Helper.EduLevelS } };
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
                    Display = s.ProfileShortName + " (" + s.Faculty + ", " + s.Level + ") " + s.Name
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
                    DirShortName = _DB_Helper.GetDirectionShortName(s1[1].ToString(), (uint)s1[0])
                }).Select(s => new
                {
                    Value = new DirTuple(s.Id, s.Faculty, s.Name, s.EduSource, s.EduForm, "", ""),
                    Display = s.DirShortName + " (" + s.Faculty + ", " + s.Level + ") " + s.Name
                }).ToList();

            var records = dirsRecords.Concat(profsRecords);

            foreach (var entrancesData in records)
            {
                if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceB))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormO)))
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

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormO)))
                {
                    cbDirection21.SelectedValue = entrancesData.Value;
                    cbDirection21.Visible = true;
                    cbDirection21.Enabled = true;
                    btRemoveDir21.Visible = true;
                    btRemoveDir21.Enabled = true;
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceB))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormOZ)))
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

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormOZ)))
                {
                    cbDirection41.SelectedValue = entrancesData.Value;
                    cbDirection41.Visible = true;
                    cbDirection41.Enabled = true;
                    btRemoveDir41.Visible = true;
                    btRemoveDir41.Enabled = true;
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormZ)))
                {
                    cbDirection51.SelectedValue = entrancesData.Value;
                    cbDirection51.Visible = true;
                    cbDirection51.Enabled = true;
                    btRemoveDir51.Visible = true;
                    btRemoveDir51.Enabled = true;
                }

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceQ))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormO)))
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

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceT))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormO)))
                {
                    _TargetOrganizationID = (uint)_DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "target_organization_id" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, entrancesData.Value.Item2),
                        new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, entrancesData.Value.Item1),
                        new Tuple<string, Relation, object>("edu_form_id", Relation.EQUAL, entrancesData.Value.Item5),
                        new Tuple<string, Relation, object>("edu_source_id", Relation.EQUAL, entrancesData.Value.Item4),
                    })[0][0];
                    cbTarget.Checked = true;
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

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceQ))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormOZ)))
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

                else if ((entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceT))
                    && (entrancesData.Value.Item5 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormOZ)))
                {
                    _TargetOrganizationID = (uint)_DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "target_organization_id" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, entrancesData.Value.Item2),
                        new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, entrancesData.Value.Item1),
                        new Tuple<string, Relation, object>("edu_form_id", Relation.EQUAL, entrancesData.Value.Item5),
                        new Tuple<string, Relation, object>("edu_source_id", Relation.EQUAL, entrancesData.Value.Item4),
                    })[0][0];
                    cbTarget.Checked = true;
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


        private void UpdateBasic(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            _DB_Connection.Update(DB_Table.ENTRANTS, new Dictionary<string, object>
            {
                { "email", mtbEMail.Text.Trim() },
                { "home_phone", tbHomePhone.Text!=tbHomePhone.Tag.ToString()? tbHomePhone.Text.Trim() : null },
                { "mobile_phone", tbMobilePhone.Text!=tbMobilePhone.Tag.ToString()? tbMobilePhone.Text.Trim() : null }
            }, new Dictionary<string, object>
            {
                { "id", _EntrantID }
            }, transaction);

            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;
            Random randNumber = new Random();

            _DB_Connection.Update(DB_Table.APPLICATIONS, new Dictionary<string, object>
            {
                { "needs_hostel", cbHostelNeeded.Checked},
                { "edit_time", _EditingDateTime},
                { "language", cbForeignLanguage.SelectedItem.ToString()},
                { "first_high_edu", firstHightEdu},
                { "mcado", cbMCADO.Checked},
                { "chernobyl", cbChernobyl.Checked},
                { "passing_examinations",cbExams.Checked },
                { "priority_right", cbPriority.Checked},
                { "special_conditions", cbSpecialConditions.Checked},
                { "compatriot", cbCompatriot.Checked },
                { "courses", cbCourses.Checked }
            }, new Dictionary<string, object>
            {
                { "id", _ApplicationID }
            }, transaction);
        }

        private void UpdateEGEMarks(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            string[] fieldNames = new string[] { "application_id", "subject_dict_id", "subject_id", "value", "year", "checked", "series", "number" };
            string[] keysNames = new string[] { "application_id", "subject_dict_id", "subject_id" };
            List<object[]> oldData = _DB_Connection.Select(DB_Table.APPLICATION_EGE_RESULTS, fieldNames,
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID )
                });
            List<object[]> newData = new List<object[]>();
            foreach (DataGridViewRow row in dgvExams.Rows)
                if ((ushort)row.Cells[dgvExams_EGE.Index].Value > 0)
                {
                        newData.Add(new object[] { _ApplicationID, (uint)FIS_Dictionary.SUBJECTS,
                        _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, row.Cells[dgvExams_Subject.Index].Value.ToString()),
                        row.Cells[dgvExams_EGE.Index].Value, row.Cells[dgvExams_Year.Index].Value, (bool)row.Cells[dgvExams_Checked.Index].Value,
                    row.Cells[dgvExams_Series.Index].Value, row.Cells[dgvExams_Number.Index].Value });
                }
            _DB_Helper.UpdateData(DB_Table.APPLICATION_EGE_RESULTS, oldData, newData, fieldNames, keysNames, transaction);
        }

        private void UpdateDocuments(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            List<object[]> appDocumentsLinks = _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, _ApplicationID)
            });

            bool qouteFound = false;
            bool sportFound = false;
            bool MADIOlympFound = false;
            bool olympFound = false;
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
                        _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                        {
                            { "series", tbIDDocSeries.Text.Trim()},
                            { "number", tbIDDocNumber.Text.Trim()},
                            { "date", dtpIDDocDate.Value} ,
                            { "organization", tbIssuedBy.Text.Trim()}
                        }, new Dictionary<string, object>
                        {
                            { "id", (uint)document[0] }
                        }, transaction);

                        _DB_Connection.Update(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                        {
                            { "last_name", tbLastName.Text.Trim()},
                            { "first_name", tbFirstName.Text.Trim()},
                            { "middle_name", tbMiddleName.Text.Trim()},
                            { "gender_dict_id", (uint)FIS_Dictionary.GENDER},
                            { "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER,cbSex.SelectedItem.ToString())},
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
                            { "reg_flat", tbAppartment.Text},
                            { "reg_index", tbPostcode.Text}
                        }, new Dictionary<string, object>
                        {
                            { "document_id", (uint)document[0] }
                        }, transaction);
                    }

                    else if (document[1].ToString() == "school_certificate" || document[1].ToString() == "middle_edu_diploma"
                        || document[1].ToString() == "high_edu_diploma" || document[1].ToString() == "academic_diploma")
                    {
                        string eduDocType = "";
                        if (!rbSpravka.Checked)
                            eduDocType = _InstitutionTypes.First(s => s[0] == cbInstitutionType.SelectedItem.ToString())[1];
                        else
                            eduDocType = "academic_diploma";

                        if ((document[6] as DateTime?) != null && (cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "series",  tbEduDocSeries.Text.Trim()},
                                { "type", eduDocType },
                                { "number", tbEduDocNumber.Text.Trim()},
                                { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text.Trim() + "|" + tbInstitutionLocation.Text.Trim()}
                            }, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                        else if ((document[6] as DateTime?) != null && (!cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "series",  tbEduDocSeries.Text.Trim()},
                                { "original_recieved_date", null },
                                { "type", eduDocType },
                                { "number", tbEduDocNumber.Text.Trim()},
                                { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text.Trim() + "|" + tbInstitutionLocation.Text.Trim()}
                            }, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                        else if ((document[6] as DateTime?) == null && (cbOriginal.Checked))
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "series",  tbEduDocSeries.Text.Trim()},
                                { "original_recieved_date", DateTime.Now },
                                { "type", eduDocType },
                                { "number", tbEduDocNumber.Text.Trim()},
                                { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text.Trim() + "|" + tbInstitutionLocation.Text.Trim()}
                            }, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                        else
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "series",  tbEduDocSeries.Text.Trim()},
                                { "type", eduDocType },
                                { "number", tbEduDocNumber.Text.Trim()},
                                { "organization", cbInstitutionType.SelectedItem.ToString() + "|" + tbInstitutionNumber.Text.Trim() + "|" + tbInstitutionLocation.Text.Trim()}
                            }, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);

                        _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                        {
                            { "year", cbGraduationYear.SelectedItem }
                        }, new Dictionary<string, object>
                        {
                            { "document_id", (uint)document[0] }
                        }, transaction);

                        uint newAchievementCategory = 0;
                        switch (eduDocType)
                        {
                            case "school_certificate":
                                newAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.MedalAchievement);
                                break;
                            case "middle_edu_diploma":
                                newAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.RedMiddleDiploma);
                                break;
                            case "high_edu_diploma":
                            case "academic_diploma":
                                newAchievementCategory = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.RedDiplomaAchievement);
                                break;
                        }

                        List<object[]> appCurrAchievements = new List<object[]>();
                        appCurrAchievements.AddRange(GetAppAchievementsByCategory(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.MedalAchievement)));
                        appCurrAchievements.AddRange(GetAppAchievementsByCategory(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.RedMiddleDiploma)));
                        appCurrAchievements.AddRange(GetAppAchievementsByCategory(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.RedDiplomaAchievement)));

                        if (cbMedal.Checked)
                        {
                            bool achFound = false;
                            foreach (object[] oldAchievement in appCurrAchievements)
                            {
                                uint oldAchievementCategory = (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "institution_achievement_id" },
                                    new List<Tuple<string, Relation, object>>
                                    {
                                        new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)oldAchievement[0])
                                    })[0][0])
                                })[0][0];

                                if (oldAchievementCategory == newAchievementCategory)
                                    achFound = true;
                                else
                                    _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                                    {
                                        { "id", (uint)oldAchievement[0] }
                                    }, transaction);
                            }
                            if (!achFound)
                                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                            {
                                { "application_id", _ApplicationID },
                                { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, newAchievementCategory)
                                })[0][0]},
                                { "document_id", (uint)document[0] }
                            }, transaction);
                        }
                        else
                            foreach (object[] oldAchievement in appCurrAchievements)
                                _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                                    {
                                        { "id", (uint)oldAchievement[0] }
                                    }, transaction);
                    }
                    else if (document[1].ToString() == "sport")
                    {
                        if (cbSport.Checked)
                        {
                            uint achevmentCategoryIdNew = 0;
                            switch (SportDoc.diplomaType)
                            {
                                case DB_Helper.SportDocTypeOlympic:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementOlympic);
                                    break;
                                case DB_Helper.SportDocTypeParalympic:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementParalympic);
                                    break;
                                case DB_Helper.SportDocTypeDeaflympic:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementDeaflympic);
                                    break;
                                case DB_Helper.SportDocTypeWorldChampion:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementWorldChampion);
                                    break;
                                case DB_Helper.SportDocTypeEuropeChampion:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementEuropeChampion);
                                    break;
                                case DB_Helper.SportAchievementGTO:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementGTO);
                                    break;
                                case DB_Helper.SportAchievementWorldChampionship:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementWorldChampionship);
                                    break;
                                case DB_Helper.SportAchievementEuropeChampionship:
                                    achevmentCategoryIdNew = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.SportAchievementEuropeChampionship);
                                    break;
                            }
                            uint achevmentCategoryIdOld = (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" },
                                new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("id", Relation.EQUAL,

                            (uint)_DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "institution_achievement_id" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            })[0][0])})[0][0];

                            if (achevmentCategoryIdNew == achevmentCategoryIdOld)
                            {
                                sportFound = true;
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "date", SportDoc.docDate },
                                    { "organization", SportDoc.orgName.Trim() }
                                }, new Dictionary<string, object>
                                {
                                    { "id", (uint)document[0] }
                                }, transaction);
                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                                {
                                    { "name", SportDoc.docName.Trim() }
                                }, new Dictionary<string, object>
                                {
                                    { "document_id", (uint)document[0] }
                                }, transaction);
                            }
                            else
                            {
                                _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                                {
                                    { "application_id", _ApplicationID },
                                    { "document_id", (uint)document[0] }
                                }, transaction);
                                _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "id", (uint)document[0] }
                                }, transaction);
                                //    _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", _ApplicationID},
                                //        { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                                //        new List<Tuple<string, Relation, object>>
                                //{
                                //    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                                //    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, achevmentCategoryIdNew)
                                //})[0][0]}, { "document_id", (uint)document[0]} }, transaction);
                                //    _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "date", SportDoc.docDate }, { "organization", SportDoc.orgName } },
                                //        new Dictionary<string, object> { { "id", (uint)document[0] } }, transaction);
                                //    _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "name", SportDoc.docName } },
                                //        new Dictionary<string, object> { { "document_id", (uint)document[0] } }, transaction);
                            }
                        }
                        else
                        {
                            _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                            {
                                { "application_id", _ApplicationID },
                                { "document_id", (uint)document[0] }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                        }
                    }

                    else if ((document[1].ToString() == "orphan") || (document[1].ToString() == "disability") || (document[1].ToString() == "medical"))
                    {
                        List<object[]> spravkaData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            });
                        if (spravkaData.Count > 0 && spravkaData[0][0].ToString() == DB_Helper.MedCertificate)
                        {
                            certificateFound = true;
                            if (!cbMedCertificate.Checked)
                                _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "id", (uint)document[0] }
                                }, transaction);
                        }
                        else if ((cbQuote.Checked) && (document[1].ToString() == "orphan") && (_QuoteDoc.cause == "Сиротство"))
                        {
                            qouteFound = true;
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "date", _QuoteDoc.orphanhoodDocDate },
                                { "organization", _QuoteDoc.orphanhoodDocOrg.Trim() }
                            }, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);

                            _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                            {
                                { "name", _QuoteDoc.orphanhoodDocName.Trim()},
                                { "dictionaries_dictionary_id", (uint)FIS_Dictionary.ORPHAN_DOC_TYPE},
                                { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.ORPHAN_DOC_TYPE, _QuoteDoc.orphanhoodDocType)}
                            }, new Dictionary<string, object>
                            {
                                { "document_id", (uint)document[0] }
                            }, transaction);

                            _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                            {
                                { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                                { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей")},
                                { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                                { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") }
                            }, new Dictionary<string, object>
                            {
                                { "application_id", _ApplicationID },
                                { "reason_document_id", (uint)document[0] }
                            }, transaction);
                        }
                        else if ((cbQuote.Checked) && ((document[1].ToString() == "disability") || (document[1].ToString() == "medical")) && (_QuoteDoc.cause == "Медецинские показатели"))
                        {
                            qouteFound = true;
                            uint allowEducationDocUid = (uint)(appDocuments.Find(x => x[1].ToString() == "allow_education")[0]);
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "number", _QuoteDoc.conclusionNumber.Trim() },
                                { "date", _QuoteDoc.conclusionDate }
                            }, new Dictionary<string, object>
                            {
                                { "id", allowEducationDocUid }
                            }, transaction);

                            if (document[1].ToString() == "disability")
                            {
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "series", _QuoteDoc.medDocSerie.Trim() },
                                    { "number", _QuoteDoc.medDocNumber.Trim() }
                                }, new Dictionary<string, object>
                                {
                                    { "document_id", (uint)document[0] }
                                }, transaction);

                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                                {
                                    { "dictionaries_dictionary_id",(uint)FIS_Dictionary.DISABILITY_GROUP},
                                    { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DISABILITY_GROUP, _QuoteDoc.disabilityGroup)}
                                }, new Dictionary<string, object>
                                {
                                    { "document_id", (uint)document[0] }
                                }, transaction);

                                _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                                {
                                    { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                                    { "document_type_id",  _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности")},
                                    { "allow_education_document_id", allowEducationDocUid},
                                    { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                                    { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") }
                                }, new Dictionary<string, object>
                                {
                                    { "application_id", _ApplicationID },
                                    { "reason_document_id", (uint)document[0] }
                                }, transaction);
                            }
                            else if (document[1].ToString() == "medical")
                            {
                                qouteFound = true;
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "series", _QuoteDoc.medDocSerie.Trim() },
                                    { "number", _QuoteDoc.medDocNumber.Trim() }
                                }, new Dictionary<string, object>
                                {
                                    { "document_id", (uint)document[0] }
                                }, transaction);

                                _DB_Connection.Update(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                                {
                                    { "document_type_dict_id", (uint)FIS_Dictionary.DOCUMENT_TYPE},
                                    { "document_type_id",  _DB_Helper.GetDictionaryItemID( FIS_Dictionary.DOCUMENT_TYPE, "Заключение психолого-медико-педагогической комиссии")},
                                    { "allow_education_document_id", allowEducationDocUid},
                                    { "benefit_kind_dict_id", (uint)FIS_Dictionary.BENEFIT_KIND},
                                    { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.BENEFIT_KIND, "По квоте приёма лиц, имеющих особое право") }
                                }, new Dictionary<string, object>
                                {
                                    { "application_id", _ApplicationID },
                                    { "reason_document_id", (uint)document[0] }
                                }, transaction);
                            }
                        }
                        else if (document[1].ToString() == "orphan")
                        {
                            _DB_Connection.Delete(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                            {
                                { "application_id", _ApplicationID },
                                { "reason_document_id", (uint)document[0] }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                            {
                                { "document_id", (uint)document[0] }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                            {
                                { "applications_id", _ApplicationID },
                                { "documents_id", (uint)document[0] }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                        }
                        else
                        {
                            uint allowEducationDocUid = (uint)(appDocuments.Find(x => x[1].ToString() == "allow_education")[0]);
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "number", _QuoteDoc.conclusionNumber.Trim() },
                                { "date", _QuoteDoc.conclusionDate }
                            }, new Dictionary<string, object>
                            {
                                { "id", allowEducationDocUid }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                            {
                                { "application_id", _ApplicationID },
                                { "reason_document_id", (uint)document[0] }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                            {
                                { "document_id", (uint)document[0] }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                            {
                                { "applications_id", _ApplicationID },
                                { "documents_id", (uint)document[0] }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                            {
                                { "applications_id", _ApplicationID },
                                { "documents_id", allowEducationDocUid }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "id", allowEducationDocUid }
                            }, transaction);
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                        }
                    }
                    else if (document[1].ToString() == "custom")
                    {
                        List<object[]> otherData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name", "text_data" },
                                new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            });
                        if (otherData.Count > 0 && otherData[0][0].ToString() == DB_Helper.MADIOlympDocName)
                            if (cbMADIOlympiad.Checked)
                            {
                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                                {
                                    { "text_data", MADIOlympDoc.olympName.Trim() }
                                }, new Dictionary<string, object>
                                {
                                    { "document_id", (uint)document[0] }
                                }, transaction);
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "date", MADIOlympDoc.olympDate },
                                    { "organization", MADIOlympDoc.olypmOrg.Trim() }
                                }, new Dictionary<string, object>
                                {
                                    { "id", (uint)document[0] }
                                }, transaction);
                                MADIOlympFound = true;
                            }
                            else
                            {
                                List<object[]> achievments = _DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "id" },
                                    new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                                new Tuple<string, Relation, object>("institution_achievement_id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                                new List<Tuple<string, Relation, object>>
                            {
                                    new Tuple<string, Relation, object> ("campaign_id", Relation.EQUAL, _CurrCampainID),
                                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, DB_Helper.OlympAchievementName))
                            })[0][0])});

                                uint achievementId = 0;
                                if (achievments.Count != 0)
                                    achievementId = (uint)(achievments[0][0]);

                                _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                                {
                                    { "id", achievementId }
                                }, transaction);
                                _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "id", (uint)document[0] }
                                }, transaction);
                            }
                        else if (cbSport.Checked && (SportDoc.diplomaType == DB_Helper.SportAchievementGTO || SportDoc.diplomaType == DB_Helper.SportAchievementWorldChampionship
                                || SportDoc.diplomaType == DB_Helper.SportAchievementEuropeChampionship))
                        {
                            string oldAchType = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.IND_ACH_CATEGORIES, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" },
                                new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "institution_achievement_id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                                })[0][0])})[0][0]);

                            if (oldAchType == SportDoc.diplomaType)
                            {
                                _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                                {
                                    { "name", SportDoc.docName.Trim() }
                                }, new Dictionary<string, object>
                                {
                                    { "document_id", (uint)document[0] }
                                }, transaction);
                                _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "date", SportDoc.docDate },
                                    { "organization", SportDoc.orgName.Trim() }
                                }, new Dictionary<string, object>
                                {
                                    { "id", (uint)document[0] }
                                }, transaction);
                                sportFound = true;
                            }
                            else
                            {
                                _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                                {
                                    { "document_id", (uint)document[0] }
                                }, transaction);
                                _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                {
                                    { "id", (uint)document[0] }
                                }, transaction);
                            }
                        }
                        else
                        {
                            _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                            {
                                { "document_id", (uint)document[0] }
                                }, transaction);
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                        }
                    }
                    else if (document[1].ToString() == "photos")
                    {
                        photosFound = true;
                        if (!cbPhotos.Checked)
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                    }
                    else if (document[1].ToString() == "olympic" || document[1].ToString() == "olympic_total"
                        || document[1].ToString() == "ukraine_olympic" || document[1].ToString() == "international_olympic")
                    {
                        string docType = "";
                        uint benefitDocType = 0;
                        switch (OlympicDoc.olympType)
                        {
                            case "Диплом победителя/призера олимпиады школьников":
                                docType = "olympic";
                                benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера олимпиады школьников");
                                break;
                            case "Диплом победителя/призера всероссийской олимпиады школьников":
                                docType = "olympic_total";
                                benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера всероссийской олимпиады школьников");
                                break;
                            case "Диплом 4 этапа всеукраинской олимпиады":
                                docType = "ukraine_olympic";
                                benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Диплом победителя/призера IV этапа всеукраинской ученической олимпиады");
                                break;
                            case "Диплом международной олимпиады":
                                docType = "international_olympic";
                                benefitDocType = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE, "Документ об участии в международной олимпиаде");
                                break;
                        }
                        if (document[1].ToString() == docType)
                        {
                            olympFound = true;
                            switch (docType)
                            {
                                case "olympic":
                                    _DB_Connection.Update(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                                    {
                                        { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                                        { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) },
                                        { "olympic_id", (uint)OlympicDoc.olympID}, { "class_number", OlympicDoc.olympClass },
                                        { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES },
                                        { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) },
                                        { "olympic_subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                                        { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, OlympicDoc.olympDist) }
                                    }, new Dictionary<string, object> { { "document_id", (uint)document[0] } }, transaction);
                                    break;
                                case "olympic_total":
                                    _DB_Connection.Update(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                                    {
                                        { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                                        { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) },
                                        { "olympic_id", (uint)OlympicDoc.olympID}, { "class_number", OlympicDoc.olympClass },
                                        { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES },
                                        { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) },
                                        { "olympic_subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                                        { "olympic_subject_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, OlympicDoc.olympDist) }
                                    }, new Dictionary<string, object>
                                    {
                                        { "document_id", (uint)document[0] }
                                    }, transaction);
                                    _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                    {
                                        { "number", OlympicDoc.olympDocNumber }
                                    }, new Dictionary<string, object>
                                    {
                                        { "id", (uint)document[0] }
                                    }, transaction);
                                    break;
                                case "ukraine_olympic":
                                    _DB_Connection.Update(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                                    {
                                        { "diploma_type_dict_id", (uint)FIS_Dictionary.DIPLOMA_TYPE },
                                        { "diploma_type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DIPLOMA_TYPE, OlympicDoc.diplomaType) },
                                        { "olympic_name", OlympicDoc.olympName },
                                        { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES },
                                        { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) }
                                    }, new Dictionary<string, object>
                                    {
                                        { "document_id", (uint)document[0] }
                                    }, transaction);
                                    _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                    {
                                        { "number", OlympicDoc.olympDocNumber }
                                    }, new Dictionary<string, object>
                                    {
                                        { "id", (uint)document[0] }
                                    }, transaction);
                                    break;
                                case "international_olympic":
                                    _DB_Connection.Update(DB_Table.OLYMPIC_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                                    {
                                        { "olympic_name", OlympicDoc.olympName },
                                        { "country_dict_id", (uint)FIS_Dictionary.COUNTRY },
                                        { "country_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY, OlympicDoc.country) },
                                        { "profile_dict_id", (uint)FIS_Dictionary.OLYMPICS_PROFILES },
                                        { "profile_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, OlympicDoc.olympProfile) }
                                    }, new Dictionary<string, object>
                                    {
                                        { "document_id", (uint)document[0] }
                                    }, transaction);
                                    _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                                    {
                                        { "number", OlympicDoc.olympDocNumber }
                                    }, new Dictionary<string, object>
                                    {
                                        { "id", (uint)document[0] }
                                    }, transaction);
                                    break;
                            }
                        }
                        else
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "document_id", (uint)document[0] }
                            }, transaction);
                    }
                }
                if (cbQuote.Checked && !qouteFound)
                {
                    SaveQuote(_ApplicationID.Value, transaction);
                }
                if (cbMADIOlympiad.Checked && !MADIOlympFound)
                {
                    SaveMADIOlympic(_ApplicationID.Value, transaction);
                }
                if (cbOlympiad.Checked && !olympFound)
                {
                    SaveOlympic(_ApplicationID.Value, transaction);
                }
                if (cbSport.Checked && !sportFound)
                {
                    SaveSport(_ApplicationID.Value, transaction);
                }
                if (cbMedCertificate.Checked && !certificateFound)
                {
                    SaveCertificate(_ApplicationID.Value, transaction);
                }

                if (cbPhotos.Checked && !photosFound)
                    _DB_Connection.Insert(
                        DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                        {
                            { "applications_id", _ApplicationID.Value },
                            { "documents_id", _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "type", "photos" }
                            },transaction)}
                        }, transaction);
            }
        }

        private void UpdateDirections(MySql.Data.MySqlClient.MySqlTransaction transaction)
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
                "edu_source_dict_id", "edu_source_id" }, transaction);
            foreach (TabPage page in tcDirections.Controls)
            {
                uint eduForm = 0;
                uint eduSource = 0;
                if (page.Name.Split('_')[1] == "budget")
                    eduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceB);
                if (page.Name.Split('_')[1] == "paid")
                    eduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP);
                if (page.Name.Split('_')[1] == "target")
                    eduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceT);
                if (page.Name.Split('_')[1] == "quote")
                    eduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceQ);

                if (page.Name.Split('_')[2] == "o")
                    eduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormO);
                if (page.Name.Split('_')[2] == "oz")
                    eduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormOZ);
                if (page.Name.Split('_')[2] == "z")
                    eduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormZ);

                foreach (Control control in page.Controls)
                {
                    CheckBox cb = control as CheckBox;
                    if ((cb != null) && (cb.Checked))
                    {
                        foreach (Control c in page.Controls)
                        {
                            ComboBox comboBox = c as ComboBox;
                            if ((comboBox != null) && (comboBox.Name == "cbDirection" + cb.Name.Substring(8)) && (comboBox.SelectedIndex != -1))
                                _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                                {
                                    { "is_agreed_date", DateTime.Now }
                                }, new Dictionary<string, object>
                                {
                                    { "faculty_short_name", ((DirTuple)comboBox.SelectedValue).Item2 },
                                    { "direction_id", ((DirTuple)comboBox.SelectedValue).Item1 },
                                    { "edu_form_id", eduForm },
                                    { "edu_source_id", eduSource },
                                    { "application_id", _ApplicationID }
                                }, transaction);
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
                    if (cb != null && !cb.Checked)
                    {
                        foreach (Control c in page.Controls)
                        {
                            ComboBox combo = c as ComboBox;
                            if (combo != null && combo.Name == "cbDirection" + cb.Name.Substring(8) && combo.SelectedValue != null)
                                foreach (object[] record in applDirs)
                                    if ((uint)record[2] == eduForm && (uint)record[3] == eduSource && record[0].ToString() == ((DirTuple)combo.SelectedValue).Item2
                                        && (uint)record[1] == ((DirTuple)combo.SelectedValue).Item1 && record[4] as DateTime? != null && record[5] as DateTime? == null)
                                        _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                                        {
                                            { "is_disagreed_date", DateTime.Now }
                                        }, new Dictionary<string, object>
                                        {
                                            { "faculty_short_name", ((DirTuple)combo.SelectedValue).Item2 },
                                            { "application_id", _ApplicationID },
                                            { "direction_id", ((DirTuple)combo.SelectedValue).Item1 },
                                            { "edu_form_id", eduForm },
                                            { "edu_source_id", eduSource }
                                        }, transaction);
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
            string[][] eduLevelsCodes = new string[][] { new string[] { "03", DB_Helper.EduLevelB }, new string[] { "04", DB_Helper.EduLevelM }, new string[] { "05", DB_Helper.EduLevelS } };

            var directionsData = _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "name", "code");

            if (isProfileList && eduSource != DB_Helper.EduSourceT)
            {
                string placesCountColumnName = "";
                if (eduForm == DB_Helper.EduFormO)
                    placesCountColumnName = "places_paid_o";
                else if (eduForm == DB_Helper.EduFormOZ)
                    placesCountColumnName = "places_paid_oz";
                else if (eduForm == DB_Helper.EduFormZ)
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
                        EduSource = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP),
                        EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm),
                        ProfileShortName = s1[2].ToString(),
                        ProfileName = s2[1].ToString()
                    }).Select(s => new
                    {
                        Value = new DirTuple(s.Id, s.Faculty, s.Name, s.EduSource, s.EduForm, s.ProfileShortName, s.ProfileName),
                        Display = s.ProfileShortName + " (" + s.Faculty + ", " + s.Level + ") " + s.ProfileName
                    }).ToList();
                combobox.ValueMember = "Value";
                combobox.DisplayMember = "Display";
                combobox.DataSource = selectedDirs;
                combobox.SelectedIndex = -1;
            }
            else if (eduSource != DB_Helper.EduSourceT)
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
                        EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm),
                        DirShortName = _DB_Helper.GetDirectionShortName(s1[1].ToString(), (uint)s1[0])
                    }).Select(s => new
                    {
                        Value = new DirTuple(s.Id, s.Faculty, s.Name, s.EduSource, s.EduForm, "", ""),
                        Display = s.DirShortName + " (" + s.Faculty + ", " + s.Level + ") " + s.Name
                    }).ToList();
                combobox.ValueMember = "Value";
                combobox.DisplayMember = "Display";
                combobox.DataSource = selectedDirs;
                combobox.SelectedIndex = -1;
            }
            else if (eduSource == DB_Helper.EduSourceT)
            {
                if (cbTarget.Checked && _TargetOrganizationID.HasValue)
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
                            new Tuple<string, Relation, object>(placesCountColumnName, Relation.GREATER, 0),
                            new Tuple<string, Relation, object>("target_organization_id", Relation.EQUAL, _TargetOrganizationID)
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
                            EduForm = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, eduForm),
                            DirShortName = _DB_Helper.GetDirectionShortName(s1[1].ToString(), (uint)s1[0])
                        }).Select(s => new
                        {
                            Value = new DirTuple(s.Id, s.Faculty, s.Name, s.EduSource, s.EduForm, "", ""),
                            Display = s.DirShortName + " (" + s.Faculty + ", " + s.Level + ") " + s.Name
                        }).Distinct().ToList();
                    combobox.ValueMember = "Value";
                    combobox.DisplayMember = "Display";
                    combobox.DataSource = selectedDirs;
                    combobox.SelectedIndex = -1;
                }
                else
                    combobox.DataSource = null;
            }
        }

        private void UpdateData(DB_Table table, List<object[]> oldDataList, List<object[]> newDataList, string[] fieldNames, bool autoGeneratedKey, string[] keyFieldsNames, MySql.Data.MySqlClient.MySqlTransaction transaction)
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

                        _DB_Connection.Update(table, columnsAndValues, keyAndValues, transaction);
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

                    _DB_Connection.Delete(table, keyAndValues, transaction);
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
                    _DB_Connection.Insert(table, columnsAndValues, transaction);
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
                if (page.Name.Split('_')[1] != "paid")
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

        private void DirectionDocEnableDisable()
        {
            if ((cbChernobyl.Checked || cbQuote.Checked || cbOlympiad.Checked || cbPriority.Checked || cbTarget.Checked || cbCompatriot.Checked) && (!_Loading))
                cbDirectionDoc.Enabled = true;
            else if ((cbChernobyl.Checked || cbQuote.Checked || cbOlympiad.Checked || cbPriority.Checked || cbTarget.Checked || cbCompatriot.Checked) && (_Loading))
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

        private List<object[]> GetAppAchievementsByCategory(uint categoryID)
        {
            List<object[]> institutionAchievements = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID),
                                new Tuple<string, Relation, object>("category_id", Relation.EQUAL, categoryID)
                            });
            if (institutionAchievements.Count == 0)
                throw new System.ArgumentException("В данной кампании не существует достижение данной категории.");
            else
                return _DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                            new Tuple<string, Relation, object>("institution_achievement_id", Relation.EQUAL, (uint)institutionAchievements[0][0])
                        });
        }

        private void BlockExam()
        {
            IEnumerable<DB_Queries.Exam> exams = DB_Queries.GetCampaignExams(_DB_Connection, _CurrCampainID);
            DateTime examRegEnd = DateTime.MinValue;
            foreach (DB_Queries.Exam exam in exams)
                if (exam.RegEndDate > examRegEnd)
                    examRegEnd = exam.RegEndDate;

            if (examRegEnd < DateTime.Now)
                cbExams.Enabled = false;
        }
    }
}
