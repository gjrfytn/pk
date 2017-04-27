using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK.Forms
{
    partial class Orders : Form
    {
        readonly Classes.DB_Connector _DB_Connection;

        public Orders(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;

            UpdateTable();
        }

        private void toolStrip_New_Click(object sender, EventArgs e)
        {
            OrderEdit form = new OrderEdit(_DB_Connection, null);
            form.ShowDialog();

            UpdateTable();
        }

        private void toolStrip_Edit_Click(object sender, EventArgs e)
        {
            //OrderEdit form = new OrderEdit(_DB_Connection, dataGridView.SelectedRows[0].Cells["dataGridView_Number"].Value.ToString());
            //form.ShowDialog();

            //UpdateTable();
        }

        private void toolStrip_Register_Click(object sender, EventArgs e)
        {
            ushort newProtocolNumber = (ushort)(_DB_Connection.Select(DB_Table.ORDERS, "protocol_number").Max(s => s[0] as ushort? != null ? (ushort)s[0] : 0) + 1);

            _DB_Connection.Update(
                DB_Table.ORDERS,
                new Dictionary<string, object> { { "protocol_number", newProtocolNumber } },
                new Dictionary<string, object> { { "number", dataGridView.SelectedRows[0].Cells["dataGridView_Number"].Value } }
                );

            dataGridView.SelectedRows[0].Cells["dataGridView_ProtNumber"].Value = newProtocolNumber;

            toolStrip_Edit.Enabled = false;
            toolStrip_Register.Enabled = false;
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells["dataGridView_ProtNumber"].Value != null)
            {
                MessageBox.Show("Невозможно удалить зарегестрированный приказ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (Classes.Utility.ShowUnrevertableActionMessageBox())
                _DB_Connection.Delete(DB_Table.ORDERS, new Dictionary<string, object> { { "number", e.Row.Cells["dataGridView_Number"].Value } });
            else
                e.Cancel = true;
        }

        private void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool registered = dataGridView["dataGridView_ProtNumber", e.RowIndex].Value != null;
            toolStrip_Edit.Enabled = !registered;
            toolStrip_Register.Enabled = !registered;
        }

        private void UpdateTable()
        {
            Dictionary<string, string> types = new Dictionary<string, string>
            {
               { "admission" ,"Зачисление" },
               { "exception" ,"Отчисление" },
               { "hostel" ,"Выделение мест в общежитии" }
           };

            dataGridView.Rows.Clear();

            dataGridView.Rows.Clear();
            foreach (object[] row in _DB_Connection.Select(
                DB_Table.ORDERS,
                new string[] { "number", "type", "date", "protocol_number" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,new Classes.DB_Helper(_DB_Connection).CurrentCampaignID)
                }))
                dataGridView.Rows.Add(
                    row[0],
                    types[row[1].ToString()],
                    ((DateTime)row[2]).ToShortDateString(),
                    row[3] as ushort?
                    );
        }
    }
}
