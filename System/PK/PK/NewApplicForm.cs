using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PK
{
    public partial class NewApplicForm : Form
    {
        DB_Connector _DB_Connection;

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

        public NewApplicForm()
        {
            InitializeComponent();

            _DB_Connection = new DB_Connector();

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

            dgvExams.Rows.Add("Математика", null, "", "", 32);
            dgvExams.Rows.Add("Русский язык", null, "", "", 32);
            dgvExams.Rows.Add("Физика", null, "", "", 32);
            dgvExams.Rows.Add("Обществознание", null, "", "", 32);
            dgvExams.Rows.Add("Иностранный язык", null, "", "", 32);

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
    }
}
