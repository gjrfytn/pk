using System;
using System.Linq;
using System.Windows.Forms;

using OlympTuple = System.Tuple<uint, string>;

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
            
            cbDiplomaType.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.DIPLOMA_TYPE).Values.ToArray());
            cbClass.SelectedItem = "10";
            cbDiscipline.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.SUBJECTS).Values.ToArray());
            cbCountry.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.COUNTRY).Values.ToArray());

            Forms.ApplicationEdit.ODoc loadedDocument = _Parent.OlympicDoc;

            if ((loadedDocument.olympType != null) && (loadedDocument.olympType != ""))
            {
                cbOlympType.SelectedItem = loadedDocument.olympType;
                tbDocNumber.Text = loadedDocument.olympDocNumber.ToString();

                if ((loadedDocument.diplomaType != null) && (loadedDocument.diplomaType != ""))
                    cbDiplomaType.SelectedItem = loadedDocument.diplomaType;

                if (loadedDocument.olympID != 0)
                {
                    tbOlympID.Text = loadedDocument.olympID.ToString();
                    cbOlympName.SelectedIndex = cbOlympName.FindString(_DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_name" }, new System.Collections.Generic.List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("olympic_id", Relation.EQUAL, loadedDocument.olympID)
                        })[0][0].ToString());
                    cbOlympProfile.SelectedIndex = cbOlympProfile.FindString(loadedDocument.olympProfile);
                }
                else
                {
                    if (loadedDocument.olympName != null)
                        cbOlympName.Text = loadedDocument.olympName;
                    cbOlympProfile.SelectedItem = loadedDocument.olympProfile;
                }

                if (loadedDocument.olympClass != 0)
                    cbClass.SelectedItem = loadedDocument.olympClass.ToString();

                if ((loadedDocument.olympDist != null) && (loadedDocument.olympDist != ""))
                    cbDiscipline.SelectedItem = loadedDocument.olympDist;

                if ((loadedDocument.country != null) && (loadedDocument.country != ""))
                    cbCountry.SelectedItem = loadedDocument.country;
            }
        }

        private void cbOlympType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOlympType.SelectedItem.ToString() == "Диплом победителя/призера олимпиады школьников")
            {
                label2.Enabled = false;
                tbDocNumber.Enabled = false;
                label3.Enabled = false;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                label5.Enabled = true;
                cbClass.Enabled = true;
                label7.Enabled = true;
                cbDiscipline.Enabled = true;
                label8.Enabled = true;
                cbCountry.Enabled = false;
                label9.Enabled = false;

                cbOlympName.ValueMember = "Value";
                cbOlympName.DisplayMember = "Name";
                cbOlympName.DataSource = _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_id", "olympic_name" }, new System.Collections.Generic.List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("year", Relation.GREATER_EQUAL, DateTime.Now.Year -1)
            }).Select(s => new
            {
                Name = s[1].ToString(),
                Value = new OlympTuple((uint)s[0], Name)
            }).ToArray();
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом победителя/призера всероссийской олимпиады школьников")
            {
                label2.Enabled = false;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                label5.Enabled = true;
                cbClass.Enabled = true;
                label7.Enabled = true;
                cbDiscipline.Enabled = true;
                label8.Enabled = true;
                cbCountry.Enabled = false;
                label9.Enabled = false;

                cbOlympName.ValueMember = "Value";
                cbOlympName.DisplayMember = "Name";
                cbOlympName.DataSource = _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_id", "olympic_name" }, new System.Collections.Generic.List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("year", Relation.EQUAL, DateTime.Now.Year)
            }).Select(s => new
            {
                Name = s[1].ToString(),
                Value = new OlympTuple((uint)s[0], Name)
            }).ToArray();
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом 4 этапа всеукраинской олимпиады")
            {
                label2.Enabled = true;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                label5.Enabled = false;
                cbClass.Enabled = false;
                label7.Enabled = false;
                cbDiscipline.Enabled = false;
                label8.Enabled = false;
                cbCountry.Enabled = false;
                label9.Enabled = false;

                cbOlympName.DataSource = null;
                cbOlympName.Text = "";
                tbOlympID.Text = "";
                cbOlympProfile.DataSource = null;
                cbOlympProfile.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.OLYMPICS_PROFILES).Values.ToArray());
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом международной олимпиады")
            {
                label2.Enabled = true;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = false;
                label4.Enabled = false;
                label5.Enabled = false;
                cbClass.Enabled = false;
                label7.Enabled = false;
                cbDiscipline.Enabled = false;
                label8.Enabled = false;
                cbCountry.Enabled = true;
                label9.Enabled = true;

                cbOlympName.DataSource = null;
                cbOlympName.Text = "";
                tbOlympID.Text = "";
                cbOlympProfile.DataSource = null;
                cbOlympProfile.Items.AddRange(_DB_Helper.GetDictionaryItems(FIS_Dictionary.OLYMPICS_PROFILES).Values.ToArray());
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
                            newDocument.diplomaType = cbDiplomaType.SelectedItem.ToString();
                            newDocument.olympID = int.Parse(tbOlympID.Text);
                            newDocument.olympProfile = ((OlympTuple)cbOlympProfile.SelectedValue).Item2;
                            newDocument.olympClass = int.Parse(cbClass.SelectedItem.ToString());
                            newDocument.olympDist = cbDiscipline.SelectedItem.ToString();
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
                            newDocument.diplomaType = cbDiplomaType.SelectedItem.ToString();
                            newDocument.olympID = int.Parse(tbOlympID.Text);
                            newDocument.olympProfile = ((OlympTuple)cbOlympProfile.SelectedValue).Item2;
                            newDocument.olympClass = int.Parse(cbClass.SelectedItem.ToString());
                            newDocument.olympDist = cbDiscipline.SelectedItem.ToString();
                            saved = true;
                        }
                        break;
                    case "Диплом 4 этапа всеукраинской олимпиады":
                        if ((cbOlympName.Text == "") || (tbDocNumber.Text == "") || (cbDiplomaType.SelectedIndex == -1)
                            || (cbOlympProfile.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            newDocument.olympType = cbOlympType.SelectedItem.ToString();
                            newDocument.olympName = cbOlympName.Text;
                            newDocument.olympDocNumber = int.Parse(tbDocNumber.Text);
                            newDocument.diplomaType = cbDiplomaType.SelectedItem.ToString();
                            newDocument.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            saved = true;
                        }
                        break;
                    case "Диплом международной олимпиады":
                        if ((cbOlympName.Text == "") || (tbDocNumber.Text == "") || (cbOlympProfile.SelectedIndex == -1)
                            || (cbCountry.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            newDocument.olympType = cbOlympType.SelectedItem.ToString();
                            newDocument.olympName = cbOlympName.Text;
                            newDocument.olympDocNumber = int.Parse(tbDocNumber.Text);
                            newDocument.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            newDocument.country = cbCountry.SelectedItem.ToString();
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

        private void cbOlympName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((OlympTuple)((ComboBox)sender).SelectedValue != null)
            {
                tbOlympID.Text = ((OlympTuple)((ComboBox)sender).SelectedValue).Item1.ToString();
                cbOlympProfile.ValueMember = "Value";
                cbOlympProfile.DisplayMember = "Name";

                cbOlympProfile.DataSource = _DB_Connection.Select(DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS, new string[] { "dictionary_olympic_profiles_profile_id" },
                    new System.Collections.Generic.List<Tuple<string, Relation, object>>
                    {
                    new Tuple<string, Relation, object>("dictionary_olympic_profiles_olympic_id", Relation.EQUAL, ((OlympTuple)((ComboBox)sender).SelectedValue).Item1)
                    }).Select(s => new
                    {
                        Name = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)s[0]),
                        Value = new OlympTuple((uint)s[0], _DB_Helper.GetDictionaryItemName(FIS_Dictionary.OLYMPICS_PROFILES, (uint)s[0]))
                    }).ToArray();
            }
        }
    }
}
