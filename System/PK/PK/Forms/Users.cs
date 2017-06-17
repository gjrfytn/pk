using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class Users : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;

        public Users(Classes.DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            dataGridView_Phone.ValueType = typeof(ulong);
            #endregion

            _DB_Connection = connection;

            foreach (object[] row in _DB_Connection.Select(DB_Table.USERS))
                dataGridView.Rows.Add(row);
        }
        
        private void bAdd_Click(object sender, EventArgs e)
        {

        }

        private void bDelete_Click(object sender, EventArgs e)
        {

        }

        private void cbShowPasswords_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView_Password.Visible = cbShowPasswords.Checked;
        }

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == dataGridView_Phone.Index)
            {
                ulong phone;
                if (!ulong.TryParse(e.FormattedValue.ToString(), out phone))
                {
                    MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex != -1)
            //    _DB_Connection.Update(DB_Table.USERS,
            //        new Dictionary<string, object>
            //        {
            //            {dataGridView.Columns[e.ColumnIndex].DataPropertyName,dataGridView[e.ColumnIndex,e.RowIndex].Value }
            //        },
            //        new Dictionary<string, object>
            //        {
            //            {dataGridView_Login.DataPropertyName,dataGridView[0,e.RowIndex].Value }
            //        }                    );
        }

        private void Users_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
                try
                {
                    dataGridView.CurrentCell = null;
                }
                catch (InvalidOperationException)
                {
                    e.Cancel = true;
                }
        }
    }
}
