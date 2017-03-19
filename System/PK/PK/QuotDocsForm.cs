using System;
using System.Windows.Forms;

namespace PK
{
    public partial class QuotDocsForm : Form
    {
        DB_Connector _DB_Connection;
        NewApplicForm _Parent;

        public QuotDocsForm(NewApplicForm parent)
        {
            InitializeComponent();

            _DB_Connection = new DB_Connector();
            _Parent = parent;
            cbCause.SelectedIndex = 0;

            cbOrphanhoodDocType.DataSource = Utility.GetDictionaryItems(_DB_Connection, 42);
            cbOrphanhoodDocType.SelectedIndex = 0;
            cbMedCause.SelectedIndex = 0;
            cbDisabilityGroup.DataSource = Utility.GetDictionaryItems(_DB_Connection, 23);
            cbDisabilityGroup.SelectedIndex = 0;

            if (_Parent.QouteDoc.cause == "Медицинские показатели")
            {
                cbCause.SelectedItem = _Parent.QouteDoc.cause;
                cbMedCause.SelectedItem = _Parent.QouteDoc.medCause;
                tbMedDocSeries.Text = _Parent.QouteDoc.medDocSerie.ToString();
                tbMedDocNumber.Text = _Parent.QouteDoc.medDocNumber.ToString();
                cbDisabilityGroup.SelectedItem = _Parent.QouteDoc.disabilityGroup;
                tbConclusionNumber.Text = _Parent.QouteDoc.conclusionNumber.ToString();
                dtpConclusionDate.Value = _Parent.QouteDoc.conclusionDate;
            }
            else if (_Parent.QouteDoc.cause == "Сиротство")
            {
                cbOrphanhoodDocType.SelectedItem = _Parent.QouteDoc.orphanhoodDocType;
                tbOrphanhoodDocName.Text = _Parent.QouteDoc.orphanhoodDocName;
                dtpOrphanhoodDocDate.Value = _Parent.QouteDoc.orphanhoodDocDate;
                tbOrphanhoodDocOrg.Text = _Parent.QouteDoc.orphanhoodDocOrg;
            }
        }

        private void cbCause_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCause.SelectedIndex == 0)
            {
                pMed.Enabled = false;
                pMed.Visible = false;
                pOrphanhood.Visible = true;
                pOrphanhood.Enabled = true;
            }
            else if (cbCause.SelectedIndex == 1)
            {
                pOrphanhood.Visible = false;
                pOrphanhood.Enabled = false;
                pMed.Enabled = true;
                pMed.Visible = true;
            }
        }

        private void cbMedCause_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMedCause.SelectedItem.ToString() == "Справква об установлении инвалидности")
            {
                tbMedDocSeries.Enabled = true;
                cbDisabilityGroup.Enabled = true;
                label7.Enabled = true;
                label11.Enabled = true;
            }
            else if (cbMedCause.SelectedItem.ToString() == "Заключение психолого-медико-педагогической комиссии")
            {
                tbMedDocSeries.Enabled = false;
                cbDisabilityGroup.Enabled = false;
                label7.Enabled = false;
                label11.Enabled = false;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            _Parent.QouteDoc.cause = "";
            _Parent.QouteDoc.conclusionNumber = 0;
            _Parent.QouteDoc.disabilityGroup = "";
            _Parent.QouteDoc.medCause = "";
            _Parent.QouteDoc.medDocNumber = 0;
            _Parent.QouteDoc.medDocSerie = 0;
            _Parent.QouteDoc.orphanhoodDocName = "";
            _Parent.QouteDoc.orphanhoodDocOrg = "";
            _Parent.QouteDoc.orphanhoodDocType = "";

            bool saved = false;
            if (cbCause.SelectedItem.ToString() == "Сиротство")
            {
                if ((cbOrphanhoodDocType.SelectedIndex == -1) || (tbOrphanhoodDocOrg.Text == "") || (tbOrphanhoodDocName.Text == ""))
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                else
                {
                    _Parent.QouteDoc.cause = cbCause.SelectedItem.ToString();
                    _Parent.QouteDoc.orphanhoodDocType = cbOrphanhoodDocType.SelectedItem.ToString();
                    _Parent.QouteDoc.orphanhoodDocOrg = tbOrphanhoodDocOrg.Text;
                    _Parent.QouteDoc.orphanhoodDocName =tbOrphanhoodDocName.Text;
                    _Parent.QouteDoc.orphanhoodDocDate = dtpOrphanhoodDocDate.Value;
                    saved = true;
                }
            }
            else if (cbCause.SelectedItem.ToString() == "Медицинские показатели")
            {
                _Parent.QouteDoc.cause = cbCause.SelectedItem.ToString();
                if (cbMedCause.SelectedItem.ToString() == "Справква об установлении инвалидности")
                {
                    if ((tbMedDocSeries.Text == "") || (tbMedDocNumber.Text == "") || (cbDisabilityGroup.SelectedIndex == -1))
                        MessageBox.Show("Все доступные поля должны быть заполнены");
                    else
                    {
                        _Parent.QouteDoc.medCause = cbMedCause.SelectedItem.ToString();
                        _Parent.QouteDoc.medDocSerie = int.Parse(tbMedDocSeries.Text);
                        _Parent.QouteDoc.medDocNumber = int.Parse(tbMedDocNumber.Text);
                        _Parent.QouteDoc.disabilityGroup = cbDisabilityGroup.SelectedItem.ToString();
                        saved = true;
                    }
                }
                else if (cbMedCause.SelectedItem.ToString() == "Заключение психолого-медико-педагогической комиссии")
                {
                    if (tbMedDocNumber.Text == "")
                        MessageBox.Show("Все доступные поля должны быть заполнены");
                    else
                    {
                        _Parent.QouteDoc.medCause = cbMedCause.SelectedItem.ToString();
                        _Parent.QouteDoc.medDocNumber = int.Parse(tbMedDocNumber.Text);
                        saved = true;
                    }
                }
                else if (cbMedCause.SelectedIndex == -1)
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                if (tbConclusionNumber.Text == "")
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                else
                {
                    _Parent.QouteDoc.conclusionNumber = int.Parse(tbConclusionNumber.Text);
                    _Parent.QouteDoc.conclusionDate = dtpConclusionDate.Value;
                }
            }
            if (saved)
                DialogResult = DialogResult.OK;
        }
    }
}
