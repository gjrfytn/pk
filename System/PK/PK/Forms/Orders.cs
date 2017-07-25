using System;
using System.Windows.Forms;
using System.Collections.Generic;
using SharedClasses.DB;
using System.Linq;

namespace PK.Forms
{
    partial class Orders : Form
    {
        private string SelectedOrderNumber
        {
            get { return dataGridView.SelectedRows[0].Cells[dataGridView_Number.Index].Value.ToString(); }
        }

        private readonly Dictionary<string, string> _OrderTypes = new Dictionary<string, string>
        {
            { "admission" ,"Зачисление" },
            { "exception" ,"Отчисление" },
            { "hostel" ,"Выделение мест в общежитии" }
        };

        private readonly DB_Connector _DB_Connection;

        public Orders(DB_Connector connection)
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
            OrderEdit form = new OrderEdit(_DB_Connection, SelectedOrderNumber);
            form.ShowDialog();

            UpdateTable();
        }

        private void toolStrip_Delete_Click(object sender, EventArgs e)
        {
            if (SharedClasses.Utility.ShowUnrevertableActionMessageBox())
            {
                _DB_Connection.Delete(DB_Table.ORDERS, new Dictionary<string, object> { { "number", SelectedOrderNumber } });
                UpdateTable();
            }
        }

        private void toolStrip_Register_Click(object sender, EventArgs e)
        {
            OrderRegistration form = new OrderRegistration(_DB_Connection, SelectedOrderNumber);
            if (form.ShowDialog() == DialogResult.OK)
            {
                dataGridView.SelectedRows[0].Cells[dataGridView_ProtNumber.Index].Value = _DB_Connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "protocol_number" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("number", Relation.EQUAL, SelectedOrderNumber) }
                    )[0][0];

                ToggleButtons();
            }
        }

        private void toolStrip_Print_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            System.Diagnostics.Process.Start(Classes.OutDocuments.Order(_DB_Connection, SelectedOrderNumber));
            Cursor.Current = Cursors.Default;
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells[dataGridView_ProtNumber.Index].Value != null)
            {
                MessageBox.Show("Невозможно удалить зарегестрированный приказ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (SharedClasses.Utility.ShowUnrevertableActionMessageBox())
                _DB_Connection.Delete(DB_Table.ORDERS, new Dictionary<string, object> { { "number", e.Row.Cells[dataGridView_Number.Index].Value } });
            else
                e.Cancel = true;
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            ToggleButtons();
        }

        private void UpdateTable()
        {
            DataGridViewColumn sortedColumn = dataGridView.SortedColumn;

            System.ComponentModel.ListSortDirection sortMethod = System.ComponentModel.ListSortDirection.Ascending;
            if (dataGridView.SortOrder == SortOrder.Ascending)
                sortMethod = System.ComponentModel.ListSortDirection.Ascending;
            else if (dataGridView.SortOrder == SortOrder.Descending)
                sortMethod = System.ComponentModel.ListSortDirection.Descending;

            int firstDisplayedRow = dataGridView.FirstDisplayedScrollingRowIndex;

            dataGridView.Rows.Clear();

            DB_Helper dbHelper = new DB_Helper(_DB_Connection);
            foreach (var row in _DB_Connection.Select(
                DB_Table.ORDERS,
                new string[] { "number", "type", "date", "protocol_number", "edu_source_id", "edu_form_id", "faculty_short_name", "direction_id", "profile_short_name" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Classes.Settings.CurrentCampaignID)
                }).GroupJoin(
                _DB_Connection.Select(DB_Table.ORDERS_HAS_APPLICATIONS, "orders_number"),
                k1 => k1[0],
                k2 => k2[0],
                (e, g) => new { e, Count = g.Count() }
                ))
                dataGridView.Rows.Add(
                    row.e[0],
                    _OrderTypes[row.e[1].ToString()],
                    ((DateTime)row.e[2]).ToShortDateString(),
                    row.e[3] as ushort?,
                    dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_SOURCE, (uint)row.e[4]),
                    dbHelper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM, (uint)row.e[5]),
                    row.e[7] is uint ? row.e[6].ToString() + " " + dbHelper.GetDirectionNameAndCode((uint)row.e[7]).Item1 : null,
                    row.e[8] is string ? DB_Queries.GetProfileName(_DB_Connection, row.e[6].ToString(), (uint)row.e[7], row.e[8].ToString()) : null,
                    row.Count
                    );

            if (sortedColumn != null)
                dataGridView.Sort(sortedColumn, sortMethod);

            if (firstDisplayedRow != -1 && dataGridView.Rows.Count > firstDisplayedRow)
                dataGridView.FirstDisplayedScrollingRowIndex = firstDisplayedRow;
        }

        private void ToggleButtons()
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                toolStrip_Edit.Enabled = false;
                toolStrip_Delete.Enabled = false;
                toolStrip_Register.Enabled = false;
                toolStrip_Print.Enabled = false;
            }
            else
            {
                bool registered = dataGridView.SelectedRows[0].Cells[dataGridView_ProtNumber.Index].Value != null;

                toolStrip_Edit.Text = registered ? "Просмотреть..." : "Редактировать...";
                toolStrip_Edit.Image = registered ? Properties.Resources.glass : Properties.Resources.pen;

                toolStrip_Edit.Enabled = true;
                toolStrip_Delete.Enabled = !registered;
                toolStrip_Register.Enabled = !registered;
                toolStrip_Print.Enabled = true;
            }
        }
    }
}
