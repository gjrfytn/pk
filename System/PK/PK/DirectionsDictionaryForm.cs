using System.Windows.Forms;

namespace PK
{
    partial class DirectionsDictionaryForm : Form
    {
        DB_Connector _DB_Connection;
        FIS_Connector _FIS_Connection;
        DataManager _DataManager;

        public DirectionsDictionaryForm(DB_Connector dbConnection)
        {
            InitializeComponent();

            _DB_Connection = dbConnection;

            try
            {
                _FIS_Connection = new FIS_Connector("XXXX", "****");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения к ФИС", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            _DataManager = new DataManager(_DB_Connection, _FIS_Connection);

            UpdateTable();
        }

        private void toolStrip_Update_Click(object sender,System.EventArgs e)
        {
            _DataManager.UpdateDirectionsDictionary();

            UpdateTable();
        }

        void UpdateTable()
        {
            dataGridView.Rows.Clear();
            foreach (object[] item in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS))
                dataGridView.Rows.Add(item[0], item[1], item[2], item[3], item[4], item[5], item[6]);
        }
    }
}
