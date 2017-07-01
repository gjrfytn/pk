using SharedClasses.DB;

namespace PK.Forms
{
    partial class DirectionsDictionary : DictionaryBase
    {
        public DirectionsDictionary(DB_Connector connection) : base(connection)
        {
            InitializeComponent();
        }

        override protected void UpdateMainTable()
        {
            dataGridView.Rows.Clear();
            foreach (object[] item in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS))
                dataGridView.Rows.Add(item[0], item[1], item[2], item[3], item[4], item[5], item[6]);
        }

        protected override void UpdateDictionary(Classes.DictionaryUpdater updater)
        {
            updater.UpdateDirectionsDictionary();
        }
    }
}
