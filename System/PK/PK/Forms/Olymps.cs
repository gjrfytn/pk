using System;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class Olymps : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly ApplicationEdit _Parent;

        public Olymps(Classes.DB_Connector connection, ApplicationEdit parent)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _Parent = parent;

            cbOlympType.SelectedIndex = 0;
            cbDiplomaType.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(FIS_Dictionary.DIPLOMA_TYPE), null);
            cbDiplomaType.DisplayMember = "Value";
            cbDiplomaType.ValueMember = "Value";
            cbDiplomaType.SelectedIndex = -1;
            
            cbClass.SelectedItem = "10";

            cbDiscipline.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(FIS_Dictionary.SUBJECTS), null);
            cbDiscipline.DisplayMember = "Value";
            cbDiscipline.ValueMember = "Value";
            cbDiscipline.SelectedIndex = -1;

            cbCountry.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(FIS_Dictionary.COUNTRY), null);
            cbCountry.DisplayMember = "Value";
            cbCountry.ValueMember = "Value";

            foreach (var record in _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_number" }))
                cbOlympID.Items.Add(record[0]);

            Forms.ApplicationEdit.ODoc loadedDocument = _Parent.OlympicDoc;

            if ((loadedDocument.olympType != null) && (loadedDocument.olympType != ""))
            {
                cbOlympType.SelectedItem = loadedDocument.olympType;
                tbOlympName.Text = loadedDocument.olympName;
                tbDocNumber.Text = loadedDocument.olympDocNumber.ToString();
                cbDiplomaType.SelectedValue = loadedDocument.diplomaType;
                cbOlympID.SelectedItem = loadedDocument.olympID.ToString();
                cbOlympProfile.SelectedValue = loadedDocument.olympProfile;
                cbClass.SelectedItem = loadedDocument.olympClass.ToString();
                cbDiscipline.SelectedValue = loadedDocument.olympDist;
                cbCountry.SelectedValue = loadedDocument.country;
            }
        }

        private void cbOlympType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOlympType.SelectedItem.ToString() == "Диплом победителя/призера олимпиады школьников")
            {
                tbOlympName.Enabled = false;
                label2.Enabled = false;
                tbDocNumber.Enabled = false;
                label3.Enabled = false;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                cbOlympID.Enabled = true;
                label5.Enabled = true;
                cbClass.Enabled = true;
                label7.Enabled = true;
                cbDiscipline.Enabled = true;
                label8.Enabled = true;
                cbCountry.Enabled = false;
                label9.Enabled = false;
                cbOlympProfile.DataSource = null;
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом победителя/призера всероссийской олимпиады школьников")
            {
                tbOlympName.Enabled = false;
                label2.Enabled = false;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                cbOlympID.Enabled = true;
                label5.Enabled = true;
                cbClass.Enabled = true;
                label7.Enabled = true;
                cbDiscipline.Enabled = true;
                label8.Enabled = true;
                cbCountry.Enabled = false;
                label9.Enabled = false;
                cbOlympProfile.DataSource = null;
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом 4 этапа всеукраинской олимпиады")
            {
                tbOlympName.Enabled = true;
                label2.Enabled = true;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                cbOlympID.Enabled = false;
                label5.Enabled = false;
                cbClass.Enabled = false;
                label7.Enabled = false;
                cbDiscipline.Enabled = false;
                label8.Enabled = false;
                cbCountry.Enabled = false;
                label9.Enabled = false;

                cbOlympProfile.DataSource = null;
                cbOlympProfile.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(FIS_Dictionary.OLYMPICS_PROFILES), null);
                cbOlympProfile.DisplayMember = "Value";
                cbOlympProfile.ValueMember = "Value";
                cbOlympProfile.SelectedIndex = -1;
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом международной олимпиады")
            {
                tbOlympName.Enabled = true;
                label2.Enabled = true;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = false;
                label4.Enabled = false;
                cbOlympID.Enabled = false;
                label5.Enabled = false;
                cbClass.Enabled = false;
                label7.Enabled = false;
                cbDiscipline.Enabled = false;
                label8.Enabled = false;
                cbCountry.Enabled = true;
                label9.Enabled = true;

                cbOlympProfile.DataSource = null;
                cbOlympProfile.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(FIS_Dictionary.OLYMPICS_PROFILES), null);
                cbOlympProfile.DisplayMember = "Value";
                cbOlympProfile.ValueMember = "Value";
                cbOlympProfile.SelectedIndex = -1;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            Forms.ApplicationEdit.ODoc newDocument;
            newDocument.olympType = "";
            newDocument.olympName = "";
            newDocument.diplomaType = "";
            newDocument.olympDocNumber = 0;
            newDocument.diplomaType = "";
            newDocument.olympID = 0;
            newDocument.olympProfile = "";
            newDocument.olympClass = 0;
            newDocument.olympDist = "";
            newDocument.country = "";

            bool saved = false;
            if ((cbOlympType.SelectedIndex == -1))
                MessageBox.Show("Выберите тип олимпиады");
            else
                switch (cbOlympType.SelectedItem.ToString())
                {
                    case "Диплом победителя/призера олимпиады школьников":
                        if ((cbDiplomaType.SelectedIndex == -1) || (cbOlympProfile.SelectedIndex == -1)
                            || (cbClass.SelectedIndex == -1) || (cbDiscipline.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            newDocument.olympType = cbOlympType.SelectedItem.ToString();
                            newDocument.diplomaType = cbDiplomaType.SelectedValue.ToString();
                            newDocument.olympID = int.Parse(cbOlympID.SelectedItem.ToString());
                            newDocument.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            newDocument.olympClass = int.Parse(cbClass.SelectedItem.ToString());
                            newDocument.olympDist = cbDiscipline.SelectedValue.ToString();
                            saved = true;
                        }
                        break;
                    case "Диплом победителя/призера всероссийской олимпиады школьников":
                        if ((tbDocNumber.Text == "") || (cbDiplomaType.SelectedIndex == -1) || (cbOlympProfile.SelectedIndex == -1)
                            || (cbClass.SelectedIndex == -1) || (cbDiscipline.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            newDocument.olympType = cbOlympType.SelectedItem.ToString();
                            newDocument.olympDocNumber = int.Parse(tbDocNumber.Text);
                            newDocument.diplomaType = cbDiplomaType.SelectedValue.ToString();
                            newDocument.olympID = int.Parse(cbOlympID.SelectedItem.ToString());
                            newDocument.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            newDocument.olympClass = int.Parse(cbClass.SelectedItem.ToString());
                            newDocument.olympDist = cbDiscipline.SelectedValue.ToString();
                            saved = true;
                        }
                        break;
                    case "Диплом 4 этапа всеукраинской олимпиады":
                        if ((tbOlympName.Text == "") || (tbDocNumber.Text == "") || (cbDiplomaType.SelectedIndex == -1)
                            || (cbOlympProfile.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            newDocument.olympType = cbOlympType.SelectedItem.ToString();
                            newDocument.olympName = tbOlympName.Text;
                            newDocument.olympDocNumber = int.Parse(tbDocNumber.Text);
                            newDocument.diplomaType = cbDiplomaType.SelectedValue.ToString();
                            newDocument.olympProfile = cbOlympProfile.SelectedValue.ToString();
                            saved = true;
                        }
                        break;
                    case "Диплом международной олимпиады":
                        if ((tbOlympName.Text == "") || (tbDocNumber.Text == "") || (cbOlympProfile.SelectedIndex == -1)
                            || (cbCountry.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            newDocument.olympType = cbOlympType.SelectedItem.ToString();
                            newDocument.olympName = tbOlympName.Text;
                            newDocument.olympDocNumber = int.Parse(tbDocNumber.Text);
                            newDocument.olympProfile = cbOlympProfile.SelectedValue.ToString();
                            newDocument.country = cbCountry.SelectedValue.ToString();
                            saved = true;
                        }
                        break;
                }
            if (saved)
            {
                _Parent.OlympicDoc = newDocument;
                DialogResult = DialogResult.OK;
            }
        }

        private void cbOlympID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((cbOlympID.SelectedIndex!=-1)&&(cbOlympID.SelectedItem.ToString()!=""))
            if ((cbOlympType.SelectedItem.ToString() == "Диплом победителя/призера олимпиады школьников")
                || (cbOlympType.SelectedItem.ToString() == "Диплом победителя/призера всероссийской олимпиады школьников"))
            {
                cbOlympProfile.DataSource = null;
                    cbOlympProfile.Items.Clear();
                    foreach (var record in _DB_Connection.Select(DB_Table.DICTIONARY_OLYMPIC_PROFILES, new string[] { "profile_id" },
                    new System.Collections.Generic.List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("olympic_id", Relation.EQUAL, int.Parse(_DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_id" },
                        new System.Collections.Generic.List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("olympic_number", Relation.EQUAL, int.Parse(cbOlympID.SelectedItem.ToString()))
                        })[0][0].ToString()))
                    }))
                    cbOlympProfile.Items.Add(_DB_Helper.GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)(record[0])));
                    cbOlympProfile.SelectedIndex = 0;
            }
        }
    }
}
