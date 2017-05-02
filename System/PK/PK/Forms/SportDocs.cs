using System;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class SportDocs : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly ApplicationEdit _Parent;

        public SportDocs(Classes.DB_Connector connection, ApplicationEdit parent)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _Parent = parent;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            cbDocType.Items.Add(Classes.DB_Helper.SportAchievementGTO);
            cbDocType.Items.Add(Classes.DB_Helper.SportAchievementWorldChampionship);
            cbDocType.Items.Add(Classes.DB_Helper.SportAchievementEuropeChampionship);
            foreach (string itemName in _DB_Helper.GetDictionaryItems(FIS_Dictionary.SPORT_DIPLOMA_TYPE).Values)
                cbDocType.Items.Add(itemName);
            cbDocType.SelectedIndex = 0;

            Forms.ApplicationEdit.SDoc loadedDocument = _Parent.SportDoc;
            if ((loadedDocument.diplomaType != null) && (loadedDocument.diplomaType != ""))
            {
                cbDocType.SelectedItem = loadedDocument.diplomaType;
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
                newDocument.diplomaType = cbDocType.SelectedItem.ToString();
                newDocument.docName = tbDocName.Text;
                newDocument.docDate = dtpDocDate.Value;
                newDocument.orgName = tbOrgName.Text;
                _Parent.SportDoc = newDocument;

                DialogResult = DialogResult.OK;
            }
        }
    }
}
