using System.Windows.Forms;

namespace PK.Forms
{
    partial class DirectionsDictionary : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;

        private Classes.DictionaryUpdater _Updater;

        public DirectionsDictionary(Classes.DB_Connector dbConnection)
        {
            InitializeComponent();

            _DB_Connection = dbConnection;

            UpdateTable();
        }

        private void toolStrip_Update_Click(object sender, System.EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (Classes.Utility.TryAccessFIS_Function((login, password) =>
            {
                if (_Updater == null)
                    _Updater = new Classes.DictionaryUpdater(_DB_Connection, login, password);

                _Updater.UpdateDirectionsDictionary();
            }, new Classes.LoginSetting()))
                UpdateTable();

            Cursor.Current = Cursors.Default;
        }

        private void UpdateTable()
        {
            dataGridView.Rows.Clear();
            foreach (object[] item in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS))
                dataGridView.Rows.Add(item[0], item[1], item[2], item[3], item[4], item[5], item[6]);
        }
    }
}
