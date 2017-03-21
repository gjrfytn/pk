using System.Windows.Forms;

namespace PK.Forms
{
    partial class DirectionsDictionaryForm : Form
    {
        Classes.DB_Connector _DB_Connection;
        Classes.FIS_Connector _FIS_Connection;
        Classes.DictionaryUpdater _Updater;

        public DirectionsDictionaryForm(Classes.DB_Connector dbConnection)
        {
            InitializeComponent();

            _DB_Connection = dbConnection;

            try
            {
                _FIS_Connection = new Classes.FIS_Connector("XXXX", "****");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения к ФИС", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            _Updater = new Classes.DictionaryUpdater(_DB_Connection, _FIS_Connection);

            UpdateTable();
        }

        private void toolStrip_Update_Click(object sender, System.EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            _Updater.UpdateDirectionsDictionary();
            UpdateTable();
            Cursor.Current = Cursors.Default;
        }

        void UpdateTable()
        {
            dataGridView.Rows.Clear();
            foreach (object[] item in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS))
                dataGridView.Rows.Add(item[0], item[1], item[2], item[3], item[4], item[5], item[6]);
        }
    }
}
