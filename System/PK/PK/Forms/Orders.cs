using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK.Forms
{
    partial class Orders : Form
    {
        [Flags]
        private enum APPL_STATUS : byte
        {
            NEW = 0x0,
            ADM_BUDGET = 0x1,
            ADM_PAID = 0x2,
            ADM_BOTH = ADM_BUDGET | ADM_PAID,
        }

        private string SelectedOrderNumber
        {
            get { return dataGridView.SelectedRows[0].Cells[dataGridView_Number.Index].Value.ToString(); }
        }

        private readonly Tuple<string, APPL_STATUS>[] _Statuses =
            {
            Tuple.Create( "new" ,APPL_STATUS.NEW ),
            Tuple.Create(  "adm_budget" ,APPL_STATUS.ADM_BUDGET ),
            Tuple.Create(  "adm_paid" ,APPL_STATUS.ADM_PAID ),
            Tuple.Create(  "adm_both" ,APPL_STATUS.ADM_BOTH )
        };

        private readonly Dictionary<string, string> _OrderTypes = new Dictionary<string, string>
        {
            { "admission" ,"Зачисление" },
            { "exception" ,"Отчисление" },
            { "hostel" ,"Выделение мест в общежитии" }
        };

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        public Orders(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

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
            OrderEdit form = new OrderEdit(_DB_Connection, SelectedOrderNumber);
            form.ShowDialog();

            UpdateTable();
        }

        private void toolStrip_Delete_Click(object sender, EventArgs e)
        {
            if (Classes.Utility.ShowUnrevertableActionMessageBox())
            {
                _DB_Connection.Delete(DB_Table.ORDERS, new Dictionary<string, object> { { "number", SelectedOrderNumber } });
                UpdateTable();
            }
        }

        private void toolStrip_Register_Click(object sender, EventArgs e)
        {
            ushort newProtocolNumber = (ushort)(_DB_Connection.Select(DB_Table.ORDERS, "protocol_number").Max(s => s[0] as ushort? != null ? (ushort)s[0] : 0) + 1);

            OrderRegistration form = new OrderRegistration();
            form.tbNumber.Text = newProtocolNumber.ToString();

            if (form.ShowDialog() != DialogResult.OK)
                return;

            newProtocolNumber = ushort.Parse(form.tbNumber.Text);
            DateTime protocolDate = form.dtpDate.Value;
            string number = SelectedOrderNumber;
            uint eduSource = (uint)_DB_Connection.Select(
                DB_Table.ORDERS,
                new string[] { "finance_source_id" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("number", Relation.EQUAL, number) }
                )[0][0];

            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
            {
                _DB_Connection.Update(
                    DB_Table.ORDERS,
                    new Dictionary<string, object> { { "protocol_number", newProtocolNumber }, { "protocol_date", protocolDate } },
                    new Dictionary<string, object> { { "number", number } },
                    transaction
                    );

                var applications = _DB_Connection.Select(
                    DB_Table.ORDERS_HAS_APPLICATIONS,
                    new string[] { "applications_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, number) }
                    ).Join(
                    _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "status"),
                    k1 => k1[0], k2 => k2[0], (s1, s2) => new { ID = (uint)s2[0], Status = s2[1].ToString() }
                    );

                if (dataGridView.SelectedRows[0].Cells[dataGridView_Type.Index].Value.ToString() == _OrderTypes["admission"])
                {
                    foreach (var appl in applications)
                        _DB_Connection.Update(
                            DB_Table.APPLICATIONS,
                            new Dictionary<string, object>
                            {
                                { "status",_Statuses.Single(s1=>s1.Item2== (_Statuses.Single(s2=>s2.Item1==appl.Status).Item2|(eduSource==15?APPL_STATUS.ADM_PAID:APPL_STATUS.ADM_BUDGET))).Item1 }//TODO !!!
                            },
                            new Dictionary<string, object> { { "id", appl.ID } },
                            transaction
                            );
                }
                else if (dataGridView.SelectedRows[0].Cells[dataGridView_Type.Index].Value.ToString() == _OrderTypes["exception"])
                {
                    foreach (var appl in applications)
                        _DB_Connection.Update(
                            DB_Table.APPLICATIONS,
                            new Dictionary<string, object>
                            {
                                { "status", _Statuses.Single(s1 => s1.Item2 == (_Statuses.Single(s2 => s2.Item1 == appl.Status).Item2 & ~(eduSource == 15 ? APPL_STATUS.ADM_PAID : APPL_STATUS.ADM_BUDGET))).Item1 }//TODO !!!
                            },
                            new Dictionary<string, object> { { "id", appl.ID } },
                            transaction
                            );
                }

                transaction.Commit();
            }
            dataGridView.SelectedRows[0].Cells[dataGridView_ProtNumber.Index].Value = newProtocolNumber;

            toolStrip_Edit.Enabled = false;
            toolStrip_Register.Enabled = false;
        }

        private void toolStrip_Print_Click(object sender, EventArgs e)
        {
            Classes.OutDocuments.Order(_DB_Connection, SelectedOrderNumber);
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells[dataGridView_ProtNumber.Index].Value != null)
            {
                MessageBox.Show("Невозможно удалить зарегестрированный приказ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (Classes.Utility.ShowUnrevertableActionMessageBox())
                _DB_Connection.Delete(DB_Table.ORDERS, new Dictionary<string, object> { { "number", e.Row.Cells[dataGridView_Number.Index].Value } });
            else
                e.Cancel = true;
        }

        private void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool registered = dataGridView[dataGridView_ProtNumber.Index, e.RowIndex].Value != null;
            toolStrip_Edit.Enabled = !registered;
            toolStrip_Delete.Enabled = !registered;
            toolStrip_Register.Enabled = !registered;
            toolStrip_Print.Enabled = registered;
        }

        private void UpdateTable()
        {
            dataGridView.Rows.Clear();

            dataGridView.Rows.Clear();
            foreach (object[] row in _DB_Connection.Select(
                DB_Table.ORDERS,
                new string[] { "number", "type", "date", "protocol_number" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID)
                }))
                dataGridView.Rows.Add(
                    row[0],
                    _OrderTypes[row[1].ToString()],
                    ((DateTime)row[2]).ToShortDateString(),
                    row[3] as ushort?
                    );
        }
    }
}
