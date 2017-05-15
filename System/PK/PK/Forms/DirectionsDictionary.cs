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
            if (_Updater == null)
            {
                string login;
                string password;
                if (!Classes.Utility.GetFIS_AuthData(out login, out password))
                    return;

                _Updater = new Classes.DictionaryUpdater(_DB_Connection, login, password);
            }

            Cursor.Current = Cursors.WaitCursor;
            _Updater.UpdateDirectionsDictionary();
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
