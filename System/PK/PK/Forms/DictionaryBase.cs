using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class DictionaryBase : Form
    {
        protected readonly DB_Connector _DB_Connection;
        protected readonly DB_Helper _DB_Helper;

        private Classes.DictionaryUpdater _Updater;

        public DictionaryBase()
        {
            InitializeComponent();
        }

        public DictionaryBase(DB_Connector connection) : this()
        {
            _DB_Connection = connection;
            _DB_Helper = new DB_Helper(_DB_Connection);
        }

        protected virtual void UpdateMainTable() { }

        protected virtual void LoadFromXML(System.Xml.Linq.XDocument doc) { }

        protected virtual void UpdateDictionary(Classes.DictionaryUpdater updater) { }

        private void DictionaryBase_Load(object sender, System.EventArgs e)
        {
            UpdateMainTable();
        }

        private void toolStrip_Update_Click(object sender, System.EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (SharedClasses.Utility.TryAccessFIS_Function((login, password) =>
            {
                if (_Updater == null)
                    _Updater = new Classes.DictionaryUpdater(_DB_Connection, "http://10.0.3.1:8080", login, password); //TODO address

                UpdateDictionary(_Updater);
            }, new Classes.LoginSetting()))
                UpdateMainTable();

            Cursor.Current = Cursors.Default;
        }

        private void toolStrip_Load_Click(object sender, System.EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                LoadFromXML(System.Xml.Linq.XDocument.Load(openFileDialog.FileName));
        }
    }
}
