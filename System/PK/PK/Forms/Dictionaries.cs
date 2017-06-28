using System.Windows.Forms;

namespace PK.Forms
{
    partial class Dictionaries : DictionaryBase
    {
        public Dictionaries(Classes.DB_Connector connection) : base(connection)
        {
            InitializeComponent();
        }

        override protected void UpdateMainTable()
        {
            dgvDictionaries.Rows.Clear();
            foreach (object[] d in _DB_Connection.Select(DB_Table.DICTIONARIES))
                dgvDictionaries.Rows.Add(d[0], d[1]);
        }

        protected override void UpdateDictionary(Classes.DictionaryUpdater updater)
        {
            updater.UpdateDictionaries();
        }

        private void dgvDictionaries_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dgvDictionaryItems.Rows.Clear();
            foreach (var d in _DB_Helper.GetDictionaryItems((FIS_Dictionary)dgvDictionaries.Rows[e.RowIndex].Cells[dgvDictionaries_ID.Index].Value))
                dgvDictionaryItems.Rows.Add(d.Key, d.Value);
        }
    }
}
