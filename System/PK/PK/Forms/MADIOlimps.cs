using System;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class MADIOlimps : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly ApplicationEdit _Parent;

        public MADIOlimps(Classes.DB_Connector connection, ApplicationEdit parent)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _Parent = parent;

            cbOlympType.SelectedIndex = 0;
            cbDiplomaType.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(18), null);
            cbDiplomaType.DisplayMember = "Value";
            cbDiplomaType.ValueMember = "Value";
            cbDiplomaType.SelectedIndex = -1;
            
            cbClass.SelectedItem = "10";

            cbDiscipline.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(1), null);
            cbDiscipline.DisplayMember = "Value";
            cbDiscipline.ValueMember = "Value";
            cbDiscipline.SelectedIndex = -1;

            cbContry.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(7), null);
            cbContry.DisplayMember = "Value";
            cbContry.ValueMember = "Value";

            foreach (var record in _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS, new string[] { "olympic_number" }))
                cbOlympID.Items.Add(record[0]);

            if ((_Parent.OlympicDoc.olympType != null) && (_Parent.OlympicDoc.olympType != ""))
            {
                cbOlympType.SelectedItem = _Parent.OlympicDoc.olympType;
                tbOlympName.Text = _Parent.OlympicDoc.olympName;
                tbDocNumber.Text = _Parent.OlympicDoc.olympDocNumber.ToString();
                cbDiplomaType.SelectedValue = _Parent.OlympicDoc.diplomaType;
                cbOlympID.SelectedItem = _Parent.OlympicDoc.olympID.ToString();
                cbOlympProfile.SelectedValue = _Parent.OlympicDoc.olympProfile;
                cbClass.SelectedItem = _Parent.OlympicDoc.olympClass.ToString();
                cbDiscipline.SelectedValue = _Parent.OlympicDoc.olympDist;
                cbContry.SelectedValue = _Parent.OlympicDoc.country;
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
                cbContry.Enabled = false;
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
                cbContry.Enabled = false;
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
                cbContry.Enabled = false;
                label9.Enabled = false;

                cbOlympProfile.DataSource = null;
                cbOlympProfile.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(39), null);
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
                cbContry.Enabled = true;
                label9.Enabled = true;

                cbOlympProfile.DataSource = null;
                cbOlympProfile.DataSource = new BindingSource(_DB_Helper.GetDictionaryItems(39), null);
                cbOlympProfile.DisplayMember = "Value";
                cbOlympProfile.ValueMember = "Value";
                cbOlympProfile.SelectedIndex = -1;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            _Parent.OlympicDoc.olympType = "";
            _Parent.OlympicDoc.olympName = "";
            _Parent.OlympicDoc.diplomaType = "";
            _Parent.OlympicDoc.olympDocNumber = 0;
            _Parent.OlympicDoc.diplomaType = "";
            _Parent.OlympicDoc.olympID = 0;
            _Parent.OlympicDoc.olympProfile = "";
            _Parent.OlympicDoc.olympClass = 0;
            _Parent.OlympicDoc.olympDist = "";
            _Parent.OlympicDoc.country = "";

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
                            _Parent.OlympicDoc.olympType = cbOlympType.SelectedItem.ToString();
                            _Parent.OlympicDoc.diplomaType = cbDiplomaType.SelectedValue.ToString();
                            _Parent.OlympicDoc.olympID = int.Parse(cbOlympID.SelectedItem.ToString());
                            _Parent.OlympicDoc.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympClass = int.Parse(cbClass.SelectedItem.ToString());
                            _Parent.OlympicDoc.olympDist = cbDiscipline.SelectedValue.ToString();
                            saved = true;
                        }
                        break;
                    case "Диплом победителя/призера всероссийской олимпиады школьников":
                        if ((tbDocNumber.Text == "") || (cbDiplomaType.SelectedIndex == -1) || (cbOlympProfile.SelectedIndex == -1)
                            || (cbClass.SelectedIndex == -1) || (cbDiscipline.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            _Parent.OlympicDoc.olympType = cbOlympType.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympDocNumber = int.Parse(tbDocNumber.Text);
                            _Parent.OlympicDoc.diplomaType = cbDiplomaType.SelectedValue.ToString();
                            _Parent.OlympicDoc.olympID = int.Parse(cbOlympID.SelectedItem.ToString());
                            _Parent.OlympicDoc.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympClass = int.Parse(cbClass.SelectedItem.ToString());
                            _Parent.OlympicDoc.olympDist = cbDiscipline.SelectedValue.ToString();
                            saved = true;
                        }
                        break;
                    case "Диплом 4 этапа всеукраинской олимпиады":
                        if ((tbOlympName.Text == "") || (tbDocNumber.Text == "") || (cbDiplomaType.SelectedIndex == -1)
                            || (cbOlympProfile.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            _Parent.OlympicDoc.olympType = cbOlympType.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympName = tbOlympName.Text;
                            _Parent.OlympicDoc.olympDocNumber = int.Parse(tbDocNumber.Text);
                            _Parent.OlympicDoc.diplomaType = cbDiplomaType.SelectedValue.ToString();
                            _Parent.OlympicDoc.olympProfile = cbOlympProfile.SelectedValue.ToString();
                            saved = true;
                        }
                        break;
                    case "Диплом международной олимпиады":
                        if ((tbOlympName.Text == "") || (tbDocNumber.Text == "") || (cbOlympProfile.SelectedIndex == -1)
                            || (cbContry.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            _Parent.OlympicDoc.olympType = cbOlympType.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympName = tbOlympName.Text;
                            _Parent.OlympicDoc.olympDocNumber = int.Parse(tbDocNumber.Text);
                            _Parent.OlympicDoc.olympProfile = cbOlympProfile.SelectedValue.ToString();
                            _Parent.OlympicDoc.country = cbContry.SelectedValue.ToString();
                            saved = true;
                        }
                        break;
                }
            if (saved)
                DialogResult = DialogResult.OK;
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
                    cbOlympProfile.Items.Add(_DB_Helper.GetDictionaryItemName(39, (uint)(record[0])));
                    cbOlympProfile.SelectedIndex = 0;
            }
        }
    }
}
