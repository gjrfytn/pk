using System;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class SportDocsForm : Form
    {
        Classes.DB_Connector _DB_Connection;
        ApplicationEdit _Parent;

        public SportDocsForm(ApplicationEdit parent)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
            _Parent = parent;
            Classes.DB_Helper dbHelper = new Classes.DB_Helper(_DB_Connection);

            cbDocType.DataSource = new BindingSource(dbHelper.GetDictionaryItems(43), null);
            cbDocType.DisplayMember = "Value";
            cbDocType.ValueMember = "Value";
            cbDocType.SelectedIndex = 0;

            if ((_Parent.SportDoc.diplomaType != null) && (_Parent.SportDoc.diplomaType != ""))
            {
                cbDocType.SelectedItem = _Parent.SportDoc.diplomaType;
                tbDocName.Text = _Parent.SportDoc.docName;
                dtpDocDate.Value = _Parent.SportDoc.docDate;
                tbOrgName.Text = _Parent.SportDoc.orgName;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if ((cbDocType.SelectedIndex == -1) || (tbDocName.Text == "") || (tbOrgName.Text == ""))
                MessageBox.Show("Все поля должны быть заполнены");
            else
            {
                _Parent.SportDoc.diplomaType = cbDocType.SelectedValue.ToString();
                _Parent.SportDoc.docName = tbDocName.Text;
                _Parent.SportDoc.docDate = dtpDocDate.Value;
                _Parent.SportDoc.orgName = tbOrgName.Text;

                DialogResult = DialogResult.OK;
            }
        }
    }
}
