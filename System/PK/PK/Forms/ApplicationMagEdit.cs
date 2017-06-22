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
        private bool _Agreed;
        private ApplicationEdit.QDoc _QuoteDoc;
        private string[] _DirsMed = { "13.04.02", "23.04.01", "23.04.02", "23.04.03" };

        private bool _DistrictNeedsReload;
        private bool _TownNeedsReload;
        private bool _StreetNeedsReload;
        private bool _HouseNeedsReload;

        private const int _AgreedChangeMaxCount = 2;

        public ApplicationMagEdit(Classes.DB_Connector connection, uint campaignID, string registratorsLogin, uint? applicationId)
        {
            _DB_Connection = connection;
            _RegistratorLogin = registratorsLogin;

            #region Components
            InitializeComponent();

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

            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _KLADR = new Classes.KLADR(connection.User, connection.Password);
            _CurrCampainID = campaignID;
            _ApplicationID = applicationId;

            cbIDDocType.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.IDENTITY_DOC_TYPE).Values.ToArray());
            cbIDDocType.SelectedIndex = 0;
            cbSex.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.GENDER).Values.ToArray());
            cbSex.SelectedIndex = 0;
            cbNationality.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.COUNTRY).Values.ToArray());
            cbNationality.SelectedIndex = 0;
            cbFirstTime.SelectedIndex = 0;
            cbRegion.Items.AddRange(_KLADR.GetRegions().ToArray());
            dtpDateOfBirth.MaxDate = DateTime.Now;
            dtpIDDocDate.MaxDate = DateTime.Now;
            dtpDiplomaDate.MaxDate = DateTime.Now;
            tbMobilePhone.Text = tbMobilePhone.Tag.ToString();
            tbMobilePhone.ForeColor = System.Drawing.Color.Gray;
            tbHomePhone.Text = tbHomePhone.Tag.ToString();
            tbHomePhone.ForeColor = System.Drawing.Color.Gray;

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

        private void btSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbLastName.Text) || string.IsNullOrWhiteSpace(tbFirstName.Text) || string.IsNullOrWhiteSpace(tbIDDocSeries.Text)
                || string.IsNullOrWhiteSpace(tbIDDocNumber.Text) || string.IsNullOrWhiteSpace(tbPlaceOfBirth.Text)
                || string.IsNullOrWhiteSpace(cbRegion.Text) || string.IsNullOrWhiteSpace(tbPostcode.Text))
                MessageBox.Show("Обязательные поля в разделе \"Из паспорта\" не заполнены.");
            else if (string.IsNullOrWhiteSpace(tbInstitution.Text))
                MessageBox.Show("Обязательные поля в разделе \"Документ об образовании\" не заполнены.");
            else if (!cbAppAdmission.Checked || (cbSpecialRights.Checked || cbTarget.Checked) && !cbDirectionDoc.Checked
                    || !cbEduDoc.Checked)
                MessageBox.Show("В разделе \"Забираемые документы\" не отмечены обязательные поля.");
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
                else
                {
                    Cursor.Current = Cursors.WaitCursor;

                    using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
                    {
                        if (_ApplicationID == null)
                        {
                            SaveApplication(transaction);
                            btPrint.Enabled = true;
                            btWithdraw.Enabled = true;
                            ChangeAgreedChBs(true);
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

        private void btPrint_Click(object sender, EventArgs e)
        {
            ApplicationDocsPrint form = new ApplicationDocsPrint(_DB_Connection, _ApplicationID.Value);
            form.ShowDialog();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cbSpecialRights_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSpecialRights.Checked && !_Loading)
            {
                QuotDocs form = new QuotDocs(_DB_Connection, _QuoteDoc);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (_QuoteDoc.cause == null || _QuoteDoc.cause == ""))
                    cbSpecialRights.Checked = false;
                else
                {
                    ApplicationEdit.QDoc quoteDoc = form._Document;
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
                }
            }
            else if (!cbSpecialRights.Checked)
                cbProgram_quote_o.SelectedIndex = -1;
            cbProgram_quote_o.Enabled = cbSpecialRights.Checked;
            label35.Enabled = cbSpecialRights.Checked;
            DirectionDocEnableDisable();
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
            tbPostcode.Text = e.Result.ToString();
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

        private void cbTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTarget.Checked && !_Loading)
            {
                TargetOrganizationSelect form = new TargetOrganizationSelect(_DB_Connection, _TargetOrganizationID);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK && (form.OrganizationID == null || form.OrganizationID == 0))
                    cbTarget.Checked = false;
                else _TargetOrganizationID = form.OrganizationID;
            }
            else if (!cbTarget.Checked)
                cbProgram_target_o.SelectedIndex = -1;
            cbProgram_target_o.Enabled = cbTarget.Checked;
            label34.Enabled = cbTarget.Checked;
            DirectionDocEnableDisable();
        }

        private void cbDistrict_Enter(object sender, EventArgs e)
        {
            if (_DistrictNeedsReload)
            {
                _DistrictNeedsReload = false;
                cbDistrict.Items.Clear();
                cbDistrict.Items.AddRange(_KLADR.GetDistricts(cbRegion.Text).ToArray());

                if (cbDistrict.AutoCompleteCustomSource.Count == 0)
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

        private void btWithdraw_Click(object sender, EventArgs e)
        {
            if (Classes.Utility.ShowChoiceMessageWithConfirmation("Забрать документы?", "Забрать документы"))
            {
                Cursor.Current = Cursors.WaitCursor;

                using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
                {
                    UpdateApplication(transaction);
                    _DB_Connection.Update(
                        DB_Table.APPLICATIONS,
                        new Dictionary<string, object> { { "status", "withdrawn" } },
                        new Dictionary<string, object> { { "id", _ApplicationID } },
                        transaction
                        );

                    transaction.Commit();
                }

                foreach (Control control in Controls)
                    control.Enabled = false;
                btClose.Enabled = true;

                Cursor.Current = Cursors.Default;
            }
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
                            ChangeAgreedChBs(false);
                            BlockDirChange();
                            ((CheckBox)sender).Enabled = true;
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
                    else if (Classes.Utility.ShowChoiceMessageWithConfirmation("Отменить согласие на зачисление на данную специальность?", "Согласие на зачисление"))
                    {
                        if (agreedCount == _AgreedChangeMaxCount && disagreedCount == _AgreedChangeMaxCount - 1)
                            ((CheckBox)sender).Enabled = false;
                        else
                            ChangeAgreedChBs(true);
                        cbAgreed.Checked = false;
                        cbAgreed.Enabled = false;
                    }
                    else
                    {
                        _Agreed = true;
                        ((CheckBox)sender).Checked = true;
                    }
                }
            _Agreed = false;
        }

        private void btFillRand_Click(object sender, EventArgs e)
        {
            tbLastName.Text = "";
            tbFirstName.Text = "";
            tbMiddleName.Text = "";
            tbPlaceOfBirth.Text = "";
            tbIssuedBy.Text = "";
            tbInstitution.Text = "";
            tbIDDocSeries.Text = "";
            tbIDDocNumber.Text = "";
            mtbSubdivisionCode.Text = "";
            tbEduDocSeries.Text = "";
            tbEduDocNumber.Text = "";
            tbSpecialty.Text = "";
            tbPostcode.Text = "";
            mtbEMail.Text = "";
            tbHomePhone.Text = "";
            tbMobilePhone.Text = "";

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
                tbInstitution.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
                tbSpecialty.Text += cyrillicLetters[rand.Next(cyrillicLetters.Length)];
            }
            for (int i = 0; i < numberLength; i++)
            {
                tbIDDocSeries.Text += digits[rand.Next(digits.Length)];
                tbIDDocNumber.Text += digits[rand.Next(digits.Length)];
                
                tbEduDocSeries.Text += digits[rand.Next(digits.Length)];
                tbEduDocNumber.Text += digits[rand.Next(digits.Length)];
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
            tbHomePhone.Text = phone;
            phone = "";
            if (rand.Next(2) > 0)
                for (int i = 0; i < 10; i++)
                    phone += digits[rand.Next(digits.Length)];
            tbMobilePhone.Text = phone;

            cbSex.SelectedIndex = rand.Next(cbSex.Items.Count);
            cbRegion.SelectedIndex = rand.Next(cbRegion.Items.Count);
            cbFirstTime.SelectedIndex = rand.Next(cbFirstTime.Items.Count);

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

            if (DateTime.Now.Year - dtpDateOfBirth.Value.Year < 18)
                year = dtpDateOfBirth.Value.Year + rand.Next(3);
            else year = rand.Next(dtpDateOfBirth.Value.Year, DateTime.Now.Year);
            if (year == DateTime.Now.Year)
                month = rand.Next(1, DateTime.Now.Month);
            else month = rand.Next(1, 12);
            if (year == DateTime.Now.Year && month == DateTime.Now.Month)
                day = rand.Next(1, DateTime.Now.Day - 1);
            else day = rand.Next(1, DateTime.DaysInMonth(year, month));
        }

        private void cbProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbMedCertificate.Enabled = false;
            foreach (TabPage page in tcPrograms.TabPages)
                foreach (Control control in page.Controls)
                {
                    ComboBox combo = control as ComboBox;
                    if (combo != null && combo.SelectedIndex != -1 && _DirsMed.Contains(_DB_Helper.GetDirectionNameAndCode(((ProgramTuple)combo.SelectedValue).Item1).Item2))
                        cbMedCertificate.Enabled = true;
                }
            if (!cbMedCertificate.Enabled)
                cbMedCertificate.Checked = false;
        }

        private void btAddDir_Click(object sender, EventArgs e)
        {
            if (sender == btAddDir_budget_o)
            {
                cbProgram_budget_o.Visible = true;
                cbProgram_budget_o.Enabled = true;
                btRemoveDir_budget_o.Visible = true;
                btRemoveDir_budget_o.Enabled = true;
                cbAgreed_budget_o.Visible = true;
            }
            else if (sender == btAddDir_paid_o)
            {
                cbProgram_paid_o.Visible = true;
                cbProgram_paid_o.Enabled = true;
                btRemoveDir_paid_o.Visible = true;
                btRemoveDir_paid_o.Enabled = true;
            }
            else if (sender == btAddDir_paid_z)
            {
                cbProgram_paid_z.Visible = true;
                cbProgram_paid_z.Enabled = true;
                btRemoveDir_paid_z.Visible = true;
                btRemoveDir_paid_z.Enabled = true;
            }
            else if (sender == btAddDir_target_o)
            {
                cbProgram_target_o.Visible = true;
                cbProgram_target_o.Enabled = true;
                btRemoveDir_target_o.Visible = true;
                btRemoveDir_target_o.Enabled = true;
                cbAgreed_target_o.Visible = true;
            }
            else if (sender == btAddDir_quote_o)
            {
                cbProgram_quote_o.Visible = true;
                cbProgram_quote_o.Enabled = true;
                btRemoveDir_quote_o.Visible = true;
                btRemoveDir_quote_o.Enabled = true;
                cbAgreed_quote_o.Visible = true;
            }
        }

        private void btRemoveDir_Click(object sender, EventArgs e)
        {
            if (sender == btRemoveDir_budget_o)
            {
                cbProgram_budget_o.SelectedIndex = -1;
                cbProgram_budget_o.Visible = false;
                cbProgram_budget_o.Enabled = false;
                btRemoveDir_budget_o.Visible = false;
                btRemoveDir_budget_o.Enabled = false;
                cbAgreed_budget_o.Visible = false;
            }
            else if (sender == btRemoveDir_paid_o)
            {
                cbProgram_paid_o.SelectedIndex = -1;
                cbProgram_paid_o.Visible = false;
                cbProgram_paid_o.Enabled = false;
                btRemoveDir_paid_o.Visible = false;
                btRemoveDir_paid_o.Enabled = false;
            }
            else if (sender == btRemoveDir_paid_z)
            {
                cbProgram_paid_z.SelectedIndex = -1;
                cbProgram_paid_z.Visible = false;
                cbProgram_paid_z.Enabled = false;
                btRemoveDir_paid_z.Visible = false;
                btRemoveDir_paid_z.Enabled = false;
            }
            else if (sender == btRemoveDir_target_o)
            {
                cbProgram_target_o.SelectedIndex = -1;
                cbProgram_target_o.Visible = false;
                cbProgram_target_o.Enabled = false;
                btRemoveDir_target_o.Visible = false;
                btRemoveDir_target_o.Enabled = false;
                cbAgreed_target_o.Visible = false;
            }
            else if (sender == btRemoveDir_quote_o)
            {
                cbProgram_quote_o.SelectedIndex = -1;
                cbProgram_quote_o.Visible = false;
                cbProgram_quote_o.Enabled = false;
                btRemoveDir_quote_o.Visible = false;
                btRemoveDir_quote_o.Enabled = false;
                cbAgreed_quote_o.Visible = false;
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

        private void ApplicationMagEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !Classes.Utility.ShowFormCloseMessageBox();
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


        private void SaveApplication(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            SaveBasic(transaction);
            SaveDiploma(transaction);
            if (cbSpecialRights.Checked)
                SaveQuote(transaction);
            if (cbMedCertificate.Checked)
                SaveCertificate(transaction);
            SaveDirections(transaction);
        }

        private void LoadApplication()
        {
            Cursor.Current = Cursors.WaitCursor;
            LoadBasic();
            LoadDocuments();
            LoadExamsMarks();
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
            List<object[]> passportFound = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "id" },
                new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("type", Relation.EQUAL, "identity"),
                new Tuple<string, Relation, object>("series", Relation.EQUAL, tbIDDocSeries.Text),
                new Tuple<string, Relation, object>("number", Relation.EQUAL, tbIDDocNumber.Text)
            });
            if (passportFound.Count > 0)
                _EntrantID = (uint)_DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "entrant_id" },
                    new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)_DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS,
                    new string[] { "applications_id" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("documents_id", Relation.EQUAL, (uint)passportFound[0][0])
                })[0][0])})[0][0];
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
                    { "home_phone", tbHomePhone.Text.Trim()},
                    { "mobile_phone", tbMobilePhone.Text.Trim() }
                }, transaction);
            }
            bool firstHightEdu = true;
            if (cbFirstTime.SelectedItem.ToString() == "Повторно")
                firstHightEdu = false;

            _ApplicationID = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object>
            {
                { "entrant_id", _EntrantID.Value},
                { "registration_time", DateTime.Now},
                { "needs_hostel", cbHostelNeeded.Checked},
                { "registrator_login", _RegistratorLogin},
                { "campaign_id", _CurrCampainID },                
                { "first_high_edu", firstHightEdu},
                { "special_conditions", cbSpecialConditions.Checked},
                { "master_appl", true }
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
                { "gender_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.GENDER,cbSex.SelectedItem.ToString())},
                { "subdivision_code",mtbSubdivisionCode.MaskFull? mtbSubdivisionCode.Text: null },
                { "type_dict_id", (uint)FIS_Dictionary.IDENTITY_DOC_TYPE},
                { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IDENTITY_DOC_TYPE,cbIDDocType.SelectedItem.ToString())},
                { "nationality_dict_id", (uint)FIS_Dictionary.COUNTRY},
                { "nationality_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.COUNTRY,cbNationality.SelectedItem.ToString())},
                { "birth_date", dtpDateOfBirth.Value },
                { "birth_place", tbPlaceOfBirth.Text.Trim()},
                { "reg_region", cbRegion.Text},
                { "reg_district", cbDistrict.Text},
                { "reg_town", cbTown.Text},
                { "reg_street", cbStreet.Text},
                { "reg_house", cbHouse.Text},
                { "reg_index", tbPostcode.Text},
                { "reg_flat", tbAppartment.Text}
            }, transaction);
        }

        private void SaveDiploma(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            int eduDocID = 0;
            eduDocID = (int)(_DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
            {
                { "type", "high_edu_diploma" },
                { "series",  tbEduDocSeries.Text.Trim()},
                { "number", tbEduDocNumber.Text.Trim()},
                { "organization", tbInstitution.Text.Trim()},
                { "date", dtpDiplomaDate.Value }
            }, transaction));
            _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
            {
                { "document_id", eduDocID },
                { "text_data", tbSpecialty.Text.Trim() }
            }, transaction);
            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
            {
                { "applications_id", _ApplicationID },
                { "documents_id", eduDocID }
            }, transaction);

            if (cbRedDiploma.Checked)
            {
                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                {
                    { "application_id", _ApplicationID},
                    { "document_id", eduDocID },
                    { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                    new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.MagAchievementRedDiploma)),
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                })[0][0]}
                }, transaction);
            }
        }

        private void SaveQuote(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            if (_QuoteDoc.cause == "Сиротство")
            {
                uint orphDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", "orphan" },
                    { "date", _QuoteDoc.orphanhoodDocDate},
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
                    { "applications_id", _ApplicationID },
                    { "documents_id", orphDocUid }
                }, transaction);
                _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                {
                    { "application_id", _ApplicationID},
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
                    { "applications_id", _ApplicationID },
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
                        { "applications_id", _ApplicationID },
                        { "documents_id", medDocUid }
                    }, transaction);
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                    {
                        { "application_id", _ApplicationID},
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
                        { "applications_id", _ApplicationID },
                        { "documents_id", medDocUid }
                    }, transaction);
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                    {
                        { "application_id", _ApplicationID},
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

        private void SaveCertificate(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
                uint spravkaID = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                {
                    { "type", "medical" }
                }, transaction);
                _DB_Connection.Insert(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                {
                    { "document_id", spravkaID },
                    { "name", Classes.DB_Helper.MedCertificate }
                }, transaction);
                _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object>
                {
                    { "applications_id", _ApplicationID },
                    { "documents_id", spravkaID }
                }, transaction);
        }

        private void SaveDirections(MySql.Data.MySqlClient.MySqlTransaction transaction)
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
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                                {
                                    { "application_id", _ApplicationID },
                                    { "faculty_short_name", ((ProgramTuple)cb.SelectedValue).Item2 },
                                    { "direction_id", ((ProgramTuple)cb.SelectedValue).Item1},                                    
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM},
                                    { "edu_form_id", ((ProgramTuple)cb.SelectedValue).Item4},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE},
                                    { "edu_source_id", ((ProgramTuple)cb.SelectedValue).Item3},
                                    { "target_organization_id", _TargetOrganizationID },
                                    { "profile_short_name", ((ProgramTuple)cb.SelectedValue).Item5}
                                }, transaction);
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
                                _DB_Connection.Insert(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                                {
                                    { "application_id", _ApplicationID },
                                     { "faculty_short_name",  ((ProgramTuple)cb.SelectedValue).Item2 },
                                    { "direction_id", ((ProgramTuple)cb.SelectedValue).Item1},                                    
                                    { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM},
                                    { "edu_form_id", ((ProgramTuple)cb.SelectedValue).Item4},
                                    { "edu_source_dict_id", (uint)FIS_Dictionary.EDU_SOURCE},
                                    { "edu_source_id", ((ProgramTuple)cb.SelectedValue).Item3},
                                    { "profile_short_name", ((ProgramTuple)cb.SelectedValue).Item5}
                                }, transaction);
                            }
                    }
                }
            }
            foreach (TabPage tab in tcPrograms.TabPages)
                foreach (Control c in tab.Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null && cb.SelectedIndex != -1)
                        _DB_Connection.Insert(DB_Table.MASTERS_EXAMS_MARKS, new Dictionary<string, object>
                        {
                            { "campaign_id", _CurrCampainID },
                            { "entrant_id", _EntrantID },
                            { "faculty", ((ProgramTuple)cb.SelectedValue).Item2 },
                            { "direction_id", ((ProgramTuple)cb.SelectedValue).Item1 },
                            { "profile_short_name", ((ProgramTuple)cb.SelectedValue).Item5 }
                        }, transaction);
                }
        }


        private void LoadBasic()
        {
            Text += " № " + _ApplicationID;
            object[] application = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "needs_hostel", "language", "first_high_edu", "special_conditions", "entrant_id", "status" }, 
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, _ApplicationID)
                })[0];

            if (application[5].ToString() == "withdrawn")
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

            _EntrantID = (uint)application[4];
            cbHostelNeeded.Checked = (bool)application[0];
            if ((bool)application[2])
                cbFirstTime.SelectedItem = "Впервые";
            else cbFirstTime.SelectedItem = "Повторно";            
            cbSpecialConditions.Checked = (bool)application[3];

            object[] entrant = _DB_Connection.Select(DB_Table.ENTRANTS, new string[] { "email", "home_phone", "mobile_phone" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, application[4])
                })[0];
            mtbEMail.Text = entrant[0].ToString();
            tbHomePhone.Text = entrant[1].ToString();
            tbMobilePhone.Text = entrant[2].ToString();
        }

        private void LoadDocuments()
        {
            cbAppAdmission.Checked = true;
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
                    cbPassportCopy.Checked = true;

                    object[] passport = _DB_Connection.Select(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new string[]{ "subdivision_code", "type_id",
                        "nationality_id", "birth_date", "birth_place", "reg_region", "reg_district", "reg_town", "reg_street", "reg_house",
                        "reg_index", "reg_flat", "last_name", "first_name", "middle_name"},
                        new List<Tuple<string, Relation, object>>
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
                    tbLastName.Text = passport[12].ToString();
                    tbFirstName.Text = passport[13].ToString();
                    tbMiddleName.Text = passport[14].ToString();
                }
                else if (document[1].ToString() == "high_edu_diploma")
                {
                    tbEduDocSeries.Text = document[2].ToString();
                    tbEduDocNumber.Text = document[3].ToString();
                    dtpDiplomaDate.Value = (DateTime)document[4];
                    tbInstitution.Text = document[5].ToString();
                    cbEduDoc.Checked = true;

                    tbSpecialty.Text = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "text_data" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, document[0])
                    })[0][0].ToString();

                    foreach (object[] achievement in _DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "institution_achievement_id" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    }))
                        if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.IND_ACH_CATEGORIES, (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)achievement[0])
                        })[0][0]) == Classes.DB_Helper.MagAchievementRedDiploma)
                            cbRedDiploma.Checked = true;
                }
                else if (document[1].ToString() == "orphan")
                {
                    _QuoteDoc.cause = "Сиротство";
                    cbSpecialRights.Checked = true;
                    _QuoteDoc.orphanhoodDocDate = (DateTime)document[4];
                    _QuoteDoc.orphanhoodDocOrg = document[5].ToString();

                    object[] orphanDoc = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name", "dictionaries_item_id" },
                        new List<Tuple<string, Relation, object>>
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
                    _QuoteDoc.medCause = "Справква об установлении инвалидности";
                    cbSpecialRights.Checked = true;
                    _QuoteDoc.medDocSerie = document[2].ToString();
                    _QuoteDoc.medDocNumber = document[3].ToString();
                    _QuoteDoc.disabilityGroup = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.DISABILITY_GROUP, (uint)_DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA,
                        new string[] { "dictionaries_item_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL,(uint)document[0]),
                            new Tuple<string, Relation, object>("dictionaries_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.DISABILITY_GROUP)
                        })[0][0]);
                    object[] allowDocument = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "number", "date" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    (uint)(_DB_Connection.Select(DB_Table.APPLICATION_COMMON_BENEFITS, new string[] { "allow_education_document_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Справка об установлении инвалидности"))
                    }))[0][0])})[0];
                    _QuoteDoc.conclusionNumber = allowDocument[0].ToString();
                    _QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                }
                else if (document[1].ToString() == "medical")
                {
                    List<object[]> spravkaData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name" },
                        new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                    });
                    if (spravkaData.Count > 0 && spravkaData[0][0].ToString() == Classes.DB_Helper.MedCertificate)
                        cbMedCertificate.Checked = true;
                    else
                    {
                        _QuoteDoc.cause = "Медицинские показатели";
                        _QuoteDoc.medCause = "Заключение психолого-медико-педагогической комиссии";
                        cbSpecialRights.Checked = true;
                        _QuoteDoc.medDocSerie = document[2].ToString();
                        _QuoteDoc.medDocNumber = document[3].ToString();
                        object[] allowDocument = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "number", "date" },
                            new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL,
                    (uint)(_DB_Connection.Select(DB_Table.APPLICATION_COMMON_BENEFITS, new string[] { "allow_education_document_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID),
                        new Tuple<string, Relation, object>("document_type_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.DOCUMENT_TYPE,"Заключение психолого-медико-педагогической комиссии"))
                    }))[0][0])})[0];
                        _QuoteDoc.conclusionNumber = allowDocument[0].ToString();
                        _QuoteDoc.conclusionDate = (DateTime)allowDocument[1];
                    }
                }
            }
        }

        private void LoadExamsMarks()
        {
            foreach (object[] mark in _DB_Connection.Select(DB_Table.MASTERS_EXAMS_MARKS, new string[] { "profile_short_name", "mark", "bonus", "date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("entrant_id", Relation.EQUAL, _EntrantID)
                }))
                if (dgvExamsResults.Rows.Count <= 2)
                {
                    dgvExamsResults.Rows.Add(mark[0].ToString(), mark[1], mark[2]);
                    DateTime? date = mark[3] as DateTime?;
                    if (date != null)
                        dgvExamsResults.Rows[dgvExamsResults.Rows.Count - 1].Cells[dgvExamsResults_Exam.Index].ToolTipText = ((DateTime)mark[3]).ToShortDateString();
                }
            while (dgvExamsResults.Rows.Count < 2)
                dgvExamsResults.Rows.Add();
        }

        private void LoadDirections()
        {          
            var profilesData = _DB_Connection.Select(DB_Table.PROFILES, new string[] { "short_name", "name", "faculty_short_name", "direction_id" });
            var appEntrances = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "direction_id", "faculty_short_name", "is_agreed_date",
            "profile_short_name", "edu_form_id", "edu_source_id", "target_organization_id", "is_disagreed_date"},
            new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID)
            });
            var records = appEntrances.Join(
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
                    cbAgreed_budget_o.Visible = true;
                    btRemoveDir_budget_o.Visible = true;
                    btRemoveDir_budget_o.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbProgram_paid_o.SelectedValue = entrancesData.Value;
                    cbProgram_paid_o.Visible = true;
                    cbProgram_paid_o.Enabled = true;
                    btRemoveDir_paid_o.Visible = true;
                    btRemoveDir_paid_o.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormZ)))
                {
                    cbProgram_paid_z.SelectedValue = entrancesData.Value;
                    cbProgram_paid_z.Visible = true;
                    cbProgram_paid_z.Enabled = true;
                    btRemoveDir_paid_z.Visible = true;
                    btRemoveDir_paid_z.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbSpecialRights.Checked = true;
                    cbProgram_quote_o.SelectedValue = entrancesData.Value;
                    cbProgram_quote_o.Visible = true;
                    cbProgram_quote_o.Enabled = true;
                    cbAgreed_quote_o.Visible = true;
                    btRemoveDir_quote_o.Visible = true;
                    btRemoveDir_quote_o.Enabled = true;
                }

                else if ((entrancesData.Value.Item3 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT))
                    && (entrancesData.Value.Item4 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO)))
                {
                    cbTarget.Checked = true;
                    _TargetOrganizationID = (uint)_DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "target_organization_id" },
                        new List<Tuple<string, Relation, object>>
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
                    cbAgreed_target_o.Visible = true;
                    btRemoveDir_target_o.Visible = true;
                    btRemoveDir_target_o.Enabled = true;              
                }
            }
            int disagreedTimes = 0;
            int agreedTimes = 0;
            foreach (object[] appEntrData in appEntrances)
                if (appEntrData[2] as DateTime? != null && appEntrData[7] as DateTime? == null)
                {
                    if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM, (uint)appEntrData[4]) == Classes.DB_Helper.EduFormO)
                    {
                        if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE, (uint)appEntrData[5]) == Classes.DB_Helper.EduSourceB)
                            cbAgreed_budget_o.Checked = true;
                        else if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE, (uint)appEntrData[5]) == Classes.DB_Helper.EduSourceQ)
                            cbAgreed_quote_o.Checked = true;
                        else if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE, (uint)appEntrData[5]) == Classes.DB_Helper.EduSourceT)
                            cbAgreed_target_o.Checked = true;
                    }
                    cbAgreed.Checked = true;
                    agreedTimes++;
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
                { "home_phone", tbHomePhone.Text.Trim() },
                { "mobile_phone", tbMobilePhone.Text.Trim() }
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
                { "first_high_edu", firstHightEdu},
                { "special_conditions", cbSpecialConditions.Checked}
            }, new Dictionary<string, object>
            {
                { "id", _ApplicationID }
            }, transaction);
        }

        private void UpdateDocuments(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            List<object[]> appDocumentsLinks = _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" },
                new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, _ApplicationID)
            });

            bool qouteFound = false;
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

                    else if (document[1].ToString() == "high_edu_diploma")
                    {
                        if (document[6] as DateTime? != null)
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "series",  tbEduDocSeries.Text.Trim()},
                                { "type", "high_edu_diploma" },
                                { "number", tbEduDocNumber.Text.Trim()},
                                { "organization", tbInstitution.Text.Trim() },
                                { "date", dtpDiplomaDate.Value }
                            }, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);

                        else if (document[6] as DateTime? == null)
                            _DB_Connection.Update(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "series",  tbEduDocSeries.Text.Trim()},
                                { "type", "high_edu_diploma" },
                                { "number", tbEduDocNumber.Text.Trim()},
                                { "organization", tbInstitution.Text.Trim() },
                                { "date", dtpDiplomaDate.Value }
                            }, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);

                        _DB_Connection.Update(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new Dictionary<string, object>
                        {
                            { "text_data", tbSpecialty.Text.Trim() }
                        },
                            new Dictionary<string, object>
                            {
                                { "document_id", (uint)document[0] }
                            }, transaction);

                        bool IAFound = false;
                        foreach (object[] achievement in _DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new string[] { "institution_achievement_id", "id" },
                            new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                        }))
                            if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.IND_ACH_CATEGORIES,
                                (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "category_id" },
                                new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)achievement[0])
                        })[0][0]) == Classes.DB_Helper.MagAchievementRedDiploma)
                            {
                                IAFound = true;
                                if (!cbRedDiploma.Checked)
                                    _DB_Connection.Delete(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                                    {
                                        { "id", (uint)achievement[1] }
                                    }, transaction);                                
                            }
                        if (cbRedDiploma.Checked && !IAFound)
                            _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object>
                            {
                                { "application_id", _ApplicationID},
                                { "document_id", (uint)document[0] },
                                { "institution_achievement_id", (uint)_DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "id" },
                                new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("category_id", Relation.EQUAL, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.IND_ACH_CATEGORIES, Classes.DB_Helper.MagAchievementRedDiploma)),
                            new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
                        })[0][0]} }, transaction);
                    }
                    else if ((document[1].ToString() == "orphan") || (document[1].ToString() == "disability") || (document[1].ToString() == "medical"))
                    {
                        List<object[]> spravkaData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, (uint)document[0])
                            });
                        if (spravkaData.Count > 0 && spravkaData[0][0].ToString() == Classes.DB_Helper.MedCertificate)
                        {
                            certificateFound = true;
                            if (!cbMedCertificate.Checked)
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                        }
                        else if ((cbSpecialRights.Checked) && (document[1].ToString() == "orphan") && (_QuoteDoc.cause == "Сиротство"))
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
                                { "name", _QuoteDoc.orphanhoodDocName.Trim() },
                                { "dictionaries_dictionary_id", (uint)FIS_Dictionary.ORPHAN_DOC_TYPE},
                                { "dictionaries_item_id", _DB_Helper.GetDictionaryItemID( FIS_Dictionary.ORPHAN_DOC_TYPE, _QuoteDoc.orphanhoodDocType)}
                            },new Dictionary<string, object>
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
                        else if ((cbSpecialRights.Checked) && ((document[1].ToString() == "disability") || (document[1].ToString() == "medical")) && (_QuoteDoc.cause == "Медецинские показатели"))
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
                    else if (document[1].ToString() == "photos")
                    {
                        photosFound = true;
                        if (!cbPhotos.Checked)
                            _DB_Connection.Delete(DB_Table.DOCUMENTS, new Dictionary<string, object>
                            {
                                { "id", (uint)document[0] }
                            }, transaction);
                    }
                }
            }
            if (cbSpecialRights.Checked && !qouteFound)
            {
                SaveQuote(transaction);
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
                        { "documents_id", _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object>
                        {
                            { "type", "photos" }
                        }, transaction)}
                    },
                    transaction
                    );
        }

        private void UpdateDirections(MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            List<object[]> oldD = new List<object[]>();
            List<object[]> newD = new List<object[]>();
            string[] fieldsList = new string[] { "application_id", "faculty_short_name", "direction_id", "edu_form_dict_id", "edu_form_id",
                "edu_source_dict_id", "edu_source_id", "profile_short_name", "target_organization_id" };
            foreach (object[] record in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, fieldsList,
                new List<Tuple<string, Relation, object>>
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
                "edu_source_dict_id", "edu_source_id" }, transaction);

            if (cbAgreed_budget_o.Checked)
                _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                {
                    { "is_agreed_date", DateTime.Now }
                }, new Dictionary<string, object>
                {
                    { "faculty_short_name", ((ProgramTuple)cbProgram_budget_o.SelectedValue).Item2 },
                    { "direction_id", ((ProgramTuple)cbProgram_budget_o.SelectedValue).Item1 },
                    { "edu_form_id", ((ProgramTuple)cbProgram_budget_o.SelectedValue).Item4 },
                    { "edu_source_id", ((ProgramTuple)cbProgram_budget_o.SelectedValue).Item3 },
                    { "application_id", _ApplicationID }
                }, transaction);
            else if (cbAgreed_quote_o.Checked)
                _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                {
                    { "is_agreed_date", DateTime.Now }
                }, new Dictionary<string, object>
                {
                    { "faculty_short_name", ((ProgramTuple)cbProgram_quote_o.SelectedValue).Item2 },
                    { "direction_id", ((ProgramTuple)cbProgram_quote_o.SelectedValue).Item1 },
                    { "edu_form_id", ((ProgramTuple)cbProgram_quote_o.SelectedValue).Item4 },
                    { "edu_source_id", ((ProgramTuple)cbProgram_quote_o.SelectedValue).Item3 },
                    { "application_id", _ApplicationID }
                }, transaction);
            else if (cbAgreed_target_o.Checked)
                _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                {
                    { "is_agreed_date", DateTime.Now }
                }, new Dictionary<string, object>
                {
                    { "faculty_short_name", ((ProgramTuple)cbProgram_target_o.SelectedValue).Item2 },
                    { "direction_id", ((ProgramTuple)cbProgram_target_o.SelectedValue).Item1 },
                    { "edu_form_id", ((ProgramTuple)cbProgram_target_o.SelectedValue).Item4 },
                    { "edu_source_id", ((ProgramTuple)cbProgram_target_o.SelectedValue).Item3 },
                    { "application_id", _ApplicationID }
                }, transaction);

            foreach (object[] appEntrance in _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "faculty_short_name", "direction_id", "edu_form_id", "edu_source_id", "is_agreed_date", "is_disagreed_date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ApplicationID)
                }))
                if (appEntrance[4] as DateTime? != null && appEntrance[5] as DateTime? == null)
                {
                    if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM, (uint)appEntrance[2]) == Classes.DB_Helper.EduFormO)
                    {
                        if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE, (uint)appEntrance[3]) == Classes.DB_Helper.EduSourceB && !cbAgreed_budget_o.Checked)
                            _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                            {
                               { "is_disagreed_date", DateTime.Now }
                            }, new Dictionary<string, object>
                            {
                                { "faculty_short_name", ((ProgramTuple)cbProgram_budget_o.SelectedValue).Item2 },
                                { "direction_id", ((ProgramTuple)cbProgram_budget_o.SelectedValue).Item1 },
                                { "edu_form_id", ((ProgramTuple)cbProgram_budget_o.SelectedValue).Item4 },
                                { "edu_source_id", ((ProgramTuple)cbProgram_budget_o.SelectedValue).Item3 },
                                { "application_id", _ApplicationID }
                            }, transaction);

                        else if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE, (uint)appEntrance[3]) == Classes.DB_Helper.EduSourceQ && !cbAgreed_quote_o.Checked)
                            _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                            {
                                { "is_disagreed_date", DateTime.Now }
                            }, new Dictionary<string, object>
                            {
                                { "faculty_short_name", ((ProgramTuple)cbProgram_quote_o.SelectedValue).Item2 },
                                { "direction_id", ((ProgramTuple)cbProgram_quote_o.SelectedValue).Item1 },
                                { "edu_form_id", ((ProgramTuple)cbProgram_quote_o.SelectedValue).Item4 },
                                { "edu_source_id", ((ProgramTuple)cbProgram_quote_o.SelectedValue).Item3 },
                                { "application_id", _ApplicationID }
                            }, transaction);

                        else if (_DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE, (uint)appEntrance[3]) == Classes.DB_Helper.EduSourceT && !cbAgreed_target_o.Checked)
                            _DB_Connection.Update(DB_Table.APPLICATIONS_ENTRANCES, new Dictionary<string, object>
                                {
                                { "is_disagreed_date", DateTime.Now }
                            }, new Dictionary<string, object>
                            {
                                { "faculty_short_name", ((ProgramTuple)cbProgram_target_o.SelectedValue).Item2 },
                                { "direction_id", ((ProgramTuple)cbProgram_target_o.SelectedValue).Item1 },
                                { "edu_form_id", ((ProgramTuple)cbProgram_target_o.SelectedValue).Item4 },
                                { "edu_source_id", ((ProgramTuple)cbProgram_target_o.SelectedValue).Item3 },
                                { "application_id", _ApplicationID }
                            }, transaction);
                    }
                }
            List<object[]> examsData = _DB_Connection.Select(DB_Table.MASTERS_EXAMS_MARKS, new string[] { "faculty", "direction_id", "profile_short_name", "mark" },
                new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("entrant_id", Relation.EQUAL, _EntrantID),
                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CurrCampainID)
            });
            foreach (TabPage tab in tcPrograms.TabPages)
                foreach (Control c in tab.Controls)
                {                    
                    ComboBox cb = c as ComboBox;
                    if (cb != null && cb.SelectedIndex != -1)
                    {
                        bool found = false;
                        foreach (object[] exam in examsData)
                            if (exam[0].ToString() == ((ProgramTuple)cb.SelectedValue).Item2 && (uint)exam[1] == ((ProgramTuple)cb.SelectedValue).Item1
                                && exam[2].ToString() == ((ProgramTuple)cb.SelectedValue).Item5)
                            {
                                found = true;
                                break;
                            }
                        if (!found)
                            _DB_Connection.Insert(DB_Table.MASTERS_EXAMS_MARKS, new Dictionary<string, object>
                            {
                                { "campaign_id", _CurrCampainID },
                                { "entrant_id", _EntrantID },
                                { "faculty", ((ProgramTuple)cb.SelectedValue).Item2 },
                                { "direction_id", ((ProgramTuple)cb.SelectedValue).Item1 },
                                { "profile_short_name", ((ProgramTuple)cb.SelectedValue).Item5 }
                            }, transaction);
                    }                        
                }
            foreach (object[] magExam in examsData)
                if (int.Parse(magExam[3].ToString()) != -1)
                {
                    bool found = false;
                    foreach (TabPage tab in tcPrograms.TabPages)
                        foreach (Control c in tab.Controls)
                        {
                            ComboBox cb = c as ComboBox;
                            if (cb != null && cb.SelectedIndex != -1 && (magExam[0].ToString() == ((ProgramTuple)cb.SelectedValue).Item2
                                && (uint)magExam[1] == ((ProgramTuple)cb.SelectedValue).Item1 && magExam[2].ToString() == ((ProgramTuple)cb.SelectedValue).Item5))
                                {
                                    found = true;
                                    break;
                                }
                        }
                    if (!found)
                        _DB_Connection.Delete(DB_Table.MASTERS_EXAMS_MARKS, new Dictionary<string, object>
                        {
                            { "campaign_id", _CurrCampainID },
                            { "entrant_id", _EntrantID },
                            { "faculty", magExam[0].ToString() },
                            { "direction_id", (uint)magExam[1] },
                            { "profile_short_name", magExam[2].ToString() }
                        }, transaction);
                }
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

        private void UpdateData(DB_Table table, List<object[]> oldDataList, List<object[]> newDataList, string[] fieldNames, bool autoGeneratedKey, string[] keyFieldsNames,MySql.Data.MySqlClient.MySqlTransaction transaction)
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
                foreach (TabPage page in tcPrograms.TabPages)
                    foreach (Control control in page.Controls)
                    {
                        CheckBox cb = control as CheckBox;
                        if (cb != null)
                            cb.Enabled = isEnabled;
                    }
        }

        private void BlockDirChange()
        {
            foreach (TabPage page in tcPrograms.TabPages)
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
            if ((cbSpecialRights.Checked || cbTarget.Checked) && !_Loading)
                cbDirectionDoc.Enabled = true;
            else if ((cbSpecialRights.Checked || cbTarget.Checked) && _Loading)
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
    }
}
