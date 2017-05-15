using System.Windows.Forms;

namespace PK.Forms
{
    partial class Dictionaries : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        private Classes.DictionaryUpdater _Updater;

        public Dictionaries(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            UpdateDictionariesTable();
        }

        private void dgvDictionaries_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dgvDictionaryItems.Rows.Clear();
            foreach (var d in _DB_Helper.GetDictionaryItems((FIS_Dictionary)dgvDictionaries.Rows[e.RowIndex].Cells[dgvDictionaries_ID.Index].Value))
                dgvDictionaryItems.Rows.Add(d.Key, d.Value);
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
            _Updater.UpdateDictionaries();
            UpdateDictionariesTable();
            Cursor.Current = Cursors.Default;
        }

        private void UpdateDictionariesTable()
        {
            dgvDictionaries.Rows.Clear();
            foreach (object[] d in _DB_Connection.Select(DB_Table.DICTIONARIES))
                dgvDictionaries.Rows.Add(d[0], d[1]);
        }
    }
}
