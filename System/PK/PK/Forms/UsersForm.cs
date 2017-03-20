using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class UsersForm : Form
    {
        readonly Classes.DB_Connector _DB_Connection;

        public UsersForm(Classes.DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            dataGridView_Phone.ValueType = typeof(ulong);
            #endregion

            _DB_Connection = connection;

            foreach (object[] row in _DB_Connection.Select(DB_Table.USERS))
                dataGridView.Rows.Add(row);
        }

        private void cbShowPasswords_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView_Password.Visible = cbShowPasswords.Checked;
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
                _DB_Connection.Update(DB_Table.USERS,
                    new Dictionary<string, object>
                    {
                        {dataGridView.Columns[e.ColumnIndex].DataPropertyName,dataGridView[e.ColumnIndex,e.RowIndex].Value }
                    },
                    new Dictionary<string, object>
                    {
                        {dataGridView_Login.DataPropertyName,dataGridView[0,e.RowIndex].Value }
                    }
                    );
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
