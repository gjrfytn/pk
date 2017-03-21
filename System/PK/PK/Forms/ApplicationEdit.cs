using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class ApplicationEdit : Form
    {
        readonly Classes.DB_Connector _DB_Connection;
        readonly Classes.DB_Helper _DB_Helper;

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

        void FillComboBox(ComboBox cb, int dictionaryNumber)
        {
            foreach (object[] v in _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                new List<Tuple<string, Relation, object>>
                {
                new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, dictionaryNumber)
                }))
                cb.Items.Add(v[0]);
            cb.SelectedIndex = 0;
        }

        public ApplicationEdit()
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            FillComboBox(cbIDDocType, 22);
            FillComboBox(cbSex, 5);
            FillComboBox(cbNationality, 7);
            FillComboBox(cbExamsDocType, 22);
            cbFirstTime.SelectedIndex = 0;
            cbForeignLanguage.SelectedIndex = 0;

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

            object[] directions = _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "name", "code")
                .Where(d => (d[1].ToString().Substring(3, 2) == "03") || (d[1].ToString().Substring(3, 2) == "05"))
                .Select(d => d[0])
                .ToArray();

            foreach (Control tab in tbDirections.Controls)
                foreach (Control c in tab.Controls)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                        cb.Items.AddRange(directions);
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

        private void NewApplicForm_Load(object sender, EventArgs e)
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

        private void cbSpecial_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSpecial.Checked)
            {
                QuotDocsForm form = new QuotDocsForm(this);
                form.ShowDialog();
            }
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

        }

        private void btSave_Click(object sender, EventArgs e)
        {
            uint entrantId = _DB_Connection.Insert(DB_Table.ENTRANTS, new Dictionary<string, object> { { "last_name", tbLastName.Text},
                { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},{ "gender_dict_id", 5},
                { "gender_id", _DB_Helper.GetDictionaryItemID( 5, cbSex.SelectedItem.ToString())},
                { "email", mtbEMail.Text},{ "is_from_krym", null}, { "home_phone", mtbHomePhone.Text}, { "mobile_phone", mtbMobilePhone.Text}});

            Random randNumber = new Random();
            uint applicationUid = _DB_Connection.Insert(DB_Table.APPLICATIONS, new Dictionary<string, object> { { "number", randNumber.Next()},
                { "entrant_id", entrantId}, { "registration_time", DateTime.Now}, { "needs_hostel", cbHostleNeeded.Checked},
            { "status_dict_id", 4},{ "status_id", _DB_Helper.GetDictionaryItemID( 4, "Новое")}, { "language", cbForeignLanguage.SelectedItem.ToString()} });

            uint idDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "identity" },
                { "series", tbIDDocSeries.Text}, { "number", tbIDDocNumber.Text}, { "date", dtpIDDocDate.Value} , { "organization", tbIssuedBy.Text} });

            _DB_Connection.Insert(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new Dictionary<string, object> { { "applications_id", applicationUid},
                { "documents_id", idDocUid } });

            _DB_Connection.Insert(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new Dictionary<string, object> { { "document_id", idDocUid},
                { "last_name", tbLastName.Text}, { "first_name", tbFirstName.Text}, { "middle_name", tbMidleName.Text},
                { "gender_dict_id", 5},{ "gender_id", _DB_Helper.GetDictionaryItemID(5,cbSex.SelectedItem.ToString())},
                { "subdivision_code", tbSubdivisionCode.Text},{ "type_dict_id", 22},
                { "type_id", _DB_Helper.GetDictionaryItemID(22,cbIDDocType.SelectedItem.ToString())},
                { "nationality_dict_id", 7}, { "nationality_id", _DB_Helper.GetDictionaryItemID(7,cbNationality.SelectedItem.ToString())},
                { "birth_date", dtpDateOfBirth.Value},{ "birth_place", tbPlaceOfBirth.Text}});

            uint examsDocId = 0;
            if (cbPassportMatch.Checked)
                examsDocId = idDocUid;
            else
                examsDocId = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "identity" },
                { "series", tbEduDocSeries.Text}, { "number", tbExamsDocNumber.Text} });

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
                    _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", applicationUid}, { "document_type_dict_id", 31},
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
                        _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", applicationUid}, { "document_type_dict_id", 31},
                            { "document_type_id",  _DB_Helper.GetDictionaryItemID(31,"Справка об установлении инвалидности")},
                            { "reason_document_id", medDocUid},{ "allow_education_document_id", allowEducationDocUid}, { "benefit_kind_dict_id", 30},
                            { "benefit_kind_id", _DB_Helper.GetDictionaryItemID( 30, "По квоте приёма лиц, имеющих особое право") } });
                    }
                    else if (QouteDoc.medCause == "Заключение психолого-медико-педагогической комиссии")
                    {
                        uint medDocUid = _DB_Connection.Insert(DB_Table.DOCUMENTS, new Dictionary<string, object> { { "type", "medical" },
                        { "series", QouteDoc.medDocSerie},  { "number", QouteDoc.medDocNumber} });
                        _DB_Connection.Insert(DB_Table.APPLICATION_COMMON_BENEFITS, new Dictionary<string, object>
                        { { "application_id", applicationUid}, { "document_type_dict_id", 31},
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

                _DB_Connection.Insert(DB_Table.INDIVIDUAL_ACHIEVEMENTS, new Dictionary<string, object> { { "application_id", applicationUid },
                    { "institution_achievement_id", achievementUid}, { "mark", _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, new string[] { "max_value" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("id", Relation.EQUAL, achievementUid)
                    })[0][0]}, { "document_id", sportDocUid} });
            }

            if (cbMADIOlympiad.Checked)
            {

            }
        }
    }
}
