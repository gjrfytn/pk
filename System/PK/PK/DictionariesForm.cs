using System.Windows.Forms;

namespace PK
{
    partial class DictionariesForm : Form
    {
        DB_Connector _DB_Connection;
        FIS_Connector _FIS_Connection;
        DataManager _DataManager;

        public DictionariesForm(DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;

            try
            {
                _FIS_Connection = new FIS_Connector("XXXX", "****");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения к ФИС", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            _DataManager = new DataManager(_DB_Connection, _FIS_Connection);

            UpdateDictionariesTable();
        }

        private void dgvDictionaries_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            UpdateDictionaryItemsTable((uint)dgvDictionaries.Rows[e.RowIndex].Cells["dgvDictionaries_ID"].Value);
        }

        private void toolStrip_Update_Click(object sender, System.EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            _DataManager.UpdateDictionaries();
            UpdateDictionariesTable();
            Cursor.Current = Cursors.Default;
        }

        void UpdateDictionariesTable()
        {
            dgvDictionaries.Rows.Clear();

            foreach (object[] d in _DB_Connection.Select(DB_Table.DICTIONARIES))
                dgvDictionaries.Rows.Add(d[0], d[1]);
        }

        void UpdateDictionaryItemsTable(uint dictionaryID)
        {
            dgvDictionaryItems.Rows.Clear();
            foreach (object[] d in _DB_Connection.Select(
                DB_Table.DICTIONARIES_ITEMS,
                new string[] { "item_id", "name" },
                new System.Collections.Generic.List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, dictionaryID) }
                ))
                dgvDictionaryItems.Rows.Add(d[0], d[1]);
        }
    }
}
