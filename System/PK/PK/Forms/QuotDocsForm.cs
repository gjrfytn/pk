using System;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class QuotDocsForm : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly ApplicationEdit _Parent;

        public QuotDocsForm(Classes.DB_Connector connection, ApplicationEdit parent)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _Parent = parent;
            Classes.DB_Helper dbHelper = new Classes.DB_Helper(_DB_Connection);

            cbCause.SelectedIndex = 0;
            cbMedCause.SelectedIndex = 0;

            cbOrphanhoodDocType.DataSource = new BindingSource(dbHelper.GetDictionaryItems(FIS_Dictionary.ORPHAN_DOC_TYPE), null);
            cbOrphanhoodDocType.DisplayMember = "Value";
            cbOrphanhoodDocType.ValueMember = "Value";
            cbOrphanhoodDocType.SelectedIndex = 0;            

            cbDisabilityGroup.DataSource = new BindingSource(dbHelper.GetDictionaryItems(FIS_Dictionary.DISABILITY_GROUP), null);
            cbDisabilityGroup.DisplayMember = "Value";
            cbDisabilityGroup.ValueMember = "Value";
            cbDisabilityGroup.SelectedIndex = 0;

            Forms.ApplicationEdit.QDoc loadedDocument = _Parent.QuoteDoc;
            if (loadedDocument.cause == "Медицинские показатели")
            {
                cbCause.SelectedItem = loadedDocument.cause;
                cbMedCause.SelectedItem = loadedDocument.medCause;
                tbMedDocSeries.Text = loadedDocument.medDocSerie.ToString();
                tbMedDocNumber.Text = loadedDocument.medDocNumber.ToString();
                cbDisabilityGroup.SelectedItem = loadedDocument.disabilityGroup;
                tbConclusionNumber.Text = loadedDocument.conclusionNumber.ToString();
                dtpConclusionDate.Value = loadedDocument.conclusionDate;
            }
            else if (loadedDocument.cause == "Сиротство")
            {
                cbOrphanhoodDocType.SelectedItem = loadedDocument.orphanhoodDocType;
                tbOrphanhoodDocName.Text = loadedDocument.orphanhoodDocName;
                dtpOrphanhoodDocDate.Value = loadedDocument.orphanhoodDocDate;
                tbOrphanhoodDocOrg.Text = loadedDocument.orphanhoodDocOrg;
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
            Forms.ApplicationEdit.QDoc newDocument;
            newDocument.cause = "";
            newDocument.conclusionNumber = 0;
            newDocument.disabilityGroup = "";
            newDocument.medCause = "";
            newDocument.medDocNumber = 0;
            newDocument.medDocSerie = 0;
            newDocument.orphanhoodDocName = "";
            newDocument.orphanhoodDocOrg = "";
            newDocument.orphanhoodDocType = "";
            newDocument.conclusionDate = DateTime.MinValue;
            newDocument.orphanhoodDocDate = DateTime.MinValue;

            bool saved = false;
            if (cbCause.SelectedItem.ToString() == "Сиротство")
            {
                if ((cbOrphanhoodDocType.SelectedIndex == -1) || (tbOrphanhoodDocOrg.Text == "") || (tbOrphanhoodDocName.Text == ""))
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                else
                {
                    newDocument.cause = cbCause.SelectedItem.ToString();
                    newDocument.orphanhoodDocType = cbOrphanhoodDocType.SelectedValue.ToString();
                    newDocument.orphanhoodDocOrg = tbOrphanhoodDocOrg.Text;
                    newDocument.orphanhoodDocName = tbOrphanhoodDocName.Text;
                    newDocument.orphanhoodDocDate = dtpOrphanhoodDocDate.Value;
                    saved = true;
                }
            }
            else if (cbCause.SelectedItem.ToString() == "Медицинские показатели")
            {
                newDocument.cause = cbCause.SelectedItem.ToString();
                if (cbMedCause.SelectedItem.ToString() == "Справква об установлении инвалидности")
                {
                    if ((tbMedDocSeries.Text == "") || (tbMedDocNumber.Text == "") || (cbDisabilityGroup.SelectedIndex == -1))
                        MessageBox.Show("Все доступные поля должны быть заполнены");
                    else
                    {
                        newDocument.medCause = cbMedCause.SelectedItem.ToString();
                        newDocument.medDocSerie = int.Parse(tbMedDocSeries.Text);
                        newDocument.medDocNumber = int.Parse(tbMedDocNumber.Text);
                        newDocument.disabilityGroup = cbDisabilityGroup.SelectedValue.ToString();
                        saved = true;
                    }
                }
                else if (cbMedCause.SelectedItem.ToString() == "Заключение психолого-медико-педагогической комиссии")
                {
                    if (tbMedDocNumber.Text == "")
                        MessageBox.Show("Все доступные поля должны быть заполнены");
                    else
                    {
                        newDocument.medCause = cbMedCause.SelectedItem.ToString();
                        newDocument.medDocNumber = int.Parse(tbMedDocNumber.Text);
                        saved = true;
                    }
                }
                else if (cbMedCause.SelectedIndex == -1)
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                if (tbConclusionNumber.Text == "")
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                else
                {
                    newDocument.conclusionNumber = int.Parse(tbConclusionNumber.Text);
                    newDocument.conclusionDate = dtpConclusionDate.Value;
                }
            }
            if (saved)
            {
                _Parent.QuoteDoc = newDocument;
                DialogResult = DialogResult.OK;
            }
        }
    }
}
