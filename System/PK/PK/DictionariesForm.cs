using System.Windows.Forms;

namespace PK
{
    partial class DictionariesForm : Form
    {
        DB_Connector _DB_Connection;

        public DictionariesForm(DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;

            dgvDictionaries.Rows.Clear();

            //ТЕСТЫ (все три способа должны давать одинаковые результаты):
            foreach (var d in _DB_Connection.GetDictionaries())
                dgvDictionaries.Rows.Add(d.Key, d.Value);
            /*foreach (object[] d in _DB_Connection.Select(DB_Tables.DICTIONARIES))
                dgvDictionaries.Rows.Add(d[0], d[1]);*/
            /*foreach (object[] d in _DB_Connection.Select(DB_Tables.DICTIONARIES, "id", "name"))
                dgvDictionaries.Rows.Add(d[0], d[1]);*/


        }

        private void dgvDictionaries_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dgvDictionaryItems.Rows.Clear();
            foreach (var d in _DB_Connection.GetDictionaryItems((uint)dgvDictionaries.Rows[e.RowIndex].Cells["dgvDictionaries_ID"].Value))
                dgvDictionaryItems.Rows.Add(d.Key, d.Value);
        }
    }
}
