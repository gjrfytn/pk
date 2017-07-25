using System;
using System.Linq;
using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class QuotDocs : Form
    {
        private readonly DB_Connector _DB_Connection;

        public Forms.ApplicationEdit.QDoc _Document;

        public QuotDocs(DB_Connector connection, Forms.ApplicationEdit.QDoc loadedDocument)
        {
            InitializeComponent();

            _DB_Connection = connection;
            DB_Helper dbHelper = new DB_Helper(_DB_Connection);
            

            cbCause.SelectedIndex = 0;
            cbMedCause.SelectedIndex = 0;

            cbOrphanhoodDocType.DataSource = dbHelper.GetDictionaryItems(FIS_Dictionary.ORPHAN_DOC_TYPE).Values.ToArray();
            cbOrphanhoodDocType.SelectedIndex = 0;            

            cbDisabilityGroup.DataSource =dbHelper.GetDictionaryItems(FIS_Dictionary.DISABILITY_GROUP).Values.ToArray();
            cbDisabilityGroup.SelectedIndex = 0;

            _Document = loadedDocument;
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
            if (cbMedCause.SelectedItem.ToString() == "Справка об установлении инвалидности")
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
            _Document.cause = "";
            _Document.conclusionNumber = "";
            _Document.disabilityGroup = "";
            _Document.medCause = "";
            _Document.medDocNumber = "";
            _Document.medDocSerie = "";
            _Document.orphanhoodDocName = "";
            _Document.orphanhoodDocOrg = "";
            _Document.orphanhoodDocType = "";
            _Document.conclusionDate = DateTime.MinValue;
            _Document.orphanhoodDocDate = DateTime.MinValue;

            bool saved = false;
            if (cbCause.SelectedItem.ToString() == "Сиротство")
            {
                if ((cbOrphanhoodDocType.SelectedIndex == -1) || (tbOrphanhoodDocOrg.Text == "") || (tbOrphanhoodDocName.Text == ""))
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                else
                {
                    _Document.cause = cbCause.SelectedItem.ToString();
                    _Document.orphanhoodDocType = cbOrphanhoodDocType.SelectedValue.ToString();
                    _Document.orphanhoodDocOrg = tbOrphanhoodDocOrg.Text;
                    _Document.orphanhoodDocName = tbOrphanhoodDocName.Text;
                    _Document.orphanhoodDocDate = dtpOrphanhoodDocDate.Value;
                    saved = true;
                }
            }
            else if (cbCause.SelectedItem.ToString() == "Медицинские показатели")
            {
                _Document.cause = cbCause.SelectedItem.ToString();
                if (cbMedCause.SelectedItem.ToString() == "Справка об установлении инвалидности")
                {
                    if ((tbMedDocSeries.Text == "") || (tbMedDocNumber.Text == "") || (cbDisabilityGroup.SelectedIndex == -1))
                        MessageBox.Show("Все доступные поля должны быть заполнены");
                    else
                    {
                        _Document.medCause = cbMedCause.SelectedItem.ToString();
                        _Document.medDocSerie = tbMedDocSeries.Text;
                        _Document.medDocNumber = tbMedDocNumber.Text;
                        _Document.disabilityGroup = cbDisabilityGroup.SelectedValue.ToString();
                        saved = true;
                    }
                }
                else if (cbMedCause.SelectedItem.ToString() == "Заключение психолого-медико-педагогической комиссии")
                {
                    if (tbMedDocNumber.Text == "")
                        MessageBox.Show("Все доступные поля должны быть заполнены");
                    else
                    {
                        _Document.medCause = cbMedCause.SelectedItem.ToString();
                        _Document.medDocNumber = tbMedDocNumber.Text;

                        saved = true;
                    }
                }
                else if (cbMedCause.SelectedIndex == -1)
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                if (tbConclusionNumber.Text == "")
                    MessageBox.Show("Все доступные поля должны быть заполнены");
                else
                {
                    _Document.conclusionNumber = tbConclusionNumber.Text;
                    _Document.conclusionDate = dtpConclusionDate.Value;
                }
            }
            if (saved)
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}
