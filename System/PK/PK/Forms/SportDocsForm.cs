using System;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class SportDocsForm : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly ApplicationEdit _Parent;

        public SportDocsForm(Classes.DB_Connector connection, ApplicationEdit parent)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _Parent = parent;
            Classes.DB_Helper dbHelper = new Classes.DB_Helper(_DB_Connection);

            cbDocType.DataSource = new BindingSource(dbHelper.GetDictionaryItems(FIS_Dictionary.SPORT_DIPLOMA_TYPE), null);
            cbDocType.DisplayMember = "Value";
            cbDocType.ValueMember = "Value";
            cbDocType.SelectedIndex = 0;

            Forms.ApplicationEdit.SDoc loadedDocument = _Parent.SportDoc;
            if ((loadedDocument.diplomaType != null) && (loadedDocument.diplomaType != ""))
            {
                cbDocType.SelectedValue = loadedDocument.diplomaType;
                tbDocName.Text = loadedDocument.docName;
                dtpDocDate.Value = loadedDocument.docDate;
                tbOrgName.Text = loadedDocument.orgName;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if ((cbDocType.SelectedIndex == -1) || (tbDocName.Text == "") || (tbOrgName.Text == ""))
                MessageBox.Show("Все поля должны быть заполнены");
            else
            {
                Forms.ApplicationEdit.SDoc newDocument;
                newDocument.diplomaType = cbDocType.SelectedValue.ToString();
                newDocument.docName = tbDocName.Text;
                newDocument.docDate = dtpDocDate.Value;
                newDocument.orgName = tbOrgName.Text;
                _Parent.SportDoc = newDocument;

                DialogResult = DialogResult.OK;
            }
        }
    }
}
