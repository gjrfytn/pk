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
            get { return dataGridView.SelectedRows[0].Cells["dataGridView_Number"].Value.ToString(); }
        }

        private readonly Tuple<string, APPL_STATUS>[] _Statuses =
            {
            new Tuple<string, APPL_STATUS>( "new" ,APPL_STATUS.NEW ),
            new Tuple<string, APPL_STATUS>(  "adm_budget" ,APPL_STATUS.ADM_BUDGET ),
            new Tuple<string, APPL_STATUS>(  "adm_paid" ,APPL_STATUS.ADM_PAID ),
            new Tuple<string, APPL_STATUS>(  "adm_both" ,APPL_STATUS.ADM_BOTH )
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

                if (dataGridView.SelectedRows[0].Cells["dataGridView_Type"].Value.ToString() == _OrderTypes["admission"])
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
                else if (dataGridView.SelectedRows[0].Cells["dataGridView_Type"].Value.ToString() == _OrderTypes["exception"])
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
            dataGridView.SelectedRows[0].Cells["dataGridView_ProtNumber"].Value = newProtocolNumber;

            toolStrip_Edit.Enabled = false;
            toolStrip_Register.Enabled = false;
        }

        private void toolStrip_Print_Click(object sender, EventArgs e)
        {
            object[] order = _DB_Connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "type", "date", "protocol_number", "protocol_date", "education_form_id", "finance_source_id", "faculty_short_name", "direction_id", "profile_short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("number",Relation.EQUAL,SelectedOrderNumber)
                    })[0];

            Tuple<string, string> dirNameCode = _DB_Helper.GetDirectionNameAndCode((uint)order[7]);
            string profile = order[8] as string;

            var applications = _DB_Connection.Select(
                         DB_Table.ORDERS_HAS_APPLICATIONS,
                         new string[] { "applications_id" },
                         new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, SelectedOrderNumber) }
                         ).Select(s => (uint)s[0]);

            var dir_subjects = _DB_Connection.Select(
                 DB_Table.ENTRANCE_TESTS,
                 new string[] { "subject_id" },
                 new List<Tuple<string, Relation, object>>
                 {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID),
                    new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,order[6]),
                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,order[7])
                 }).Select(s => (uint)s[0]);

            var marks = applications.GroupJoin(
                dir_subjects.Join(
                    _DB_Connection.Select(DB_Table.APPLICATIONS_EGE_MARKS_VIEW),//TODO exams
                    k1 => k1,
                    k2 => k2[1],
                    (s1, s2) => s2
                    ),
                k1 => k1,
                k2 => k2[0],
                (s1, s2) => new { ApplID = s1, MarksSum = s2.Sum(s => (uint)s[2]) }
                );

            var table = marks.Join(
                _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id"),
                k1 => k1.ApplID,
                k2 => k2[0],
                (s1, s2) => new { EntrID = s2[1], s1.MarksSum }
                ).Join(
                _DB_Connection.Select(DB_Table.ENTRANTS_VIEW),
                k1 => k1.EntrID,
                k2 => k2[0],
                (s1, s2) => new string[] { s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString(), s1.MarksSum.ToString() }
                );

            string doc = Classes.Utility.TempPath + "AdmOrder" + new Random().Next();
            Classes.DocumentCreator.Create(
                Classes.Utility.DocumentsTemplatesPath + "AdmOrder.xml",
                doc,
                new string[]
                {
                    ((DateTime)order[1]).ToShortDateString(),
                    SelectedOrderNumber,
                    order[2].ToString(),
                    ((DateTime)order[3]).ToShortDateString(),
                    ((DateTime)order[1]).Year.ToString(),
                    _DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM,(uint)order[4]).ToLower()+" обучения"+
                    ((uint)order[5]==_DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE,Classes.DB_Helper.EduSourceP)?" по договорам с оплатой стоимости обучения":""),
                    order[6].ToString(),
                    dirNameCode.Item2,
                    dirNameCode.Item1,
                    profile!=null?"Профиль: ":"",
                    profile !=null?profile+" - "+_DB_Connection.Select(
                        DB_Table.PROFILES,
                        new string[] { "name" },
                        new List<Tuple<string, Relation, object>> {new Tuple<string, Relation, object>("short_name",Relation.EQUAL,profile) }
                        )[0][0].ToString():""
                },
                new IEnumerable<string[]>[] { table }
                );
            Classes.Utility.Print(doc + ".docx");
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
