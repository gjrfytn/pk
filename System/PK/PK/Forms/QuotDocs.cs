using System;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class QuotDocs : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly ApplicationEdit _Parent;

        public QuotDocs(Classes.DB_Connector connection, ApplicationEdit parent)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _Parent = parent;
            Classes.DB_Helper dbHelper = new Classes.DB_Helper(_DB_Connection);

            cbCause.SelectedIndex = 0;
            cbMedCause.SelectedIndex = 0;

            cbOrphanhoodDocType.DataSource = dbHelper.GetDictionaryItems(FIS_Dictionary.ORPHAN_DOC_TYPE).Values.ToArray();
            cbOrphanhoodDocType.SelectedIndex = 0;            

            cbDisabilityGroup.DataSource =dbHelper.GetDictionaryItems(FIS_Dictionary.DISABILITY_GROUP).Values.ToArray();
            cbDisabilityGroup.SelectedIndex = 0;

            Forms.ApplicationEdit.QDoc loadedDocument = _Parent.QuoteDoc;
            if (loadedDocument.cause == "Медицинские показатели")
            {
                cbCause.SelectedItem = loadedDocument.cause;
                cbMedCause.SelectedItem = loadedDocument.medCause;
                tbMedDocSeries.Text = loadedDocument.medDocSerie;
                tbMedDocNumber.Text = loadedDocument.medDocNumber;
                cbDisabilityGroup.SelectedItem = loadedDocument.disabilityGroup;
                tbConclusionNumber.Text = loadedDocument.conclusionNumber;
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
            newDocument.conclusionNumber = "";
            newDocument.disabilityGroup = "";
            newDocument.medCause = "";
            newDocument.medDocNumber = "";
            newDocument.medDocSerie = "";
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
                        newDocument.medDocSerie = tbMedDocSeries.Text;
                        newDocument.medDocNumber = tbMedDocNumber.Text;
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
                        newDocument.medDocNumber = tbMedDocNumber.Text;
                        saved = true;
                    }
                }
                else if (cbMedCause.SelectedIndex == -1)
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                if (tbConclusionNumber.Text == "")
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                else
                {
                    newDocument.conclusionNumber = tbConclusionNumber.Text;
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
