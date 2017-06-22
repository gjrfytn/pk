using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class OrderRegistration : Form
    {
        [Flags]
        private enum APPL_STATUS : byte
        {
            NEW = 0x0,
            ADM_BUDGET = 0x1,
            ADM_PAID = 0x2,
            ADM_BOTH = ADM_BUDGET | ADM_PAID,
        }

        private readonly Tuple<string, APPL_STATUS>[] _Statuses =            {
            Tuple.Create( "new" ,APPL_STATUS.NEW ),
            Tuple.Create(  "adm_budget" ,APPL_STATUS.ADM_BUDGET ),
            Tuple.Create(  "adm_paid" ,APPL_STATUS.ADM_PAID ),
            Tuple.Create(  "adm_both" ,APPL_STATUS.ADM_BOTH )
        };

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly string _Number;

        public OrderRegistration(Classes.DB_Connector connection, string number)
        {
            _DB_Connection = connection;

            #region Components
            InitializeComponent();

            tbNumber.Text = (_DB_Connection.Select(DB_Table.ORDERS, "protocol_number").Max(s => s[0] as ushort? != null ? (ushort)s[0] : 0) + 1).ToString();
            #endregion

            _Number = number;
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (tbNumber.Text == "")
            {
                MessageBox.Show("Не заполнен номер протокола.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            object[] buf = _DB_Connection.Select(
                DB_Table.ORDERS,
                new string[] { "type", "edu_source_id", "faculty_short_name" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("number", Relation.EQUAL, _Number) }
                )[0];

            string type = buf[0].ToString();
            uint eduSource = (uint)buf[1];
            string faculty = buf[2].ToString();

            var applications = _DB_Connection.Select(
                    DB_Table.ORDERS_HAS_APPLICATIONS,
                    new string[] { "applications_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, _Number) }
                    ).Join(
                    _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "status"),
                    k1 => k1[0], k2 => k2[0], (s1, s2) => new { ID = (uint)s2[0], Status = s2[1].ToString() }
                    );

            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
            {
                if (type == "admission")
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
                else if (type == "exception")
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
                else
                {
                    ushort places = (ushort)_DB_Connection.Select(
                        DB_Table.CAMPAIGNS_FACULTIES_DATA,
                        new string[] { "hostel_places" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Classes.Settings.CurrentCampaignID),
                            new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,faculty)
                        })[0][0];

                    ushort occupied = (ushort)(_DB_Connection.Select(
                          DB_Table.ORDERS,
                          new string[] { "number" },
                          new List<Tuple<string, Relation, object>>
                          {
                            new Tuple<string, Relation, object>("type",Relation.EQUAL,"hostel"),
                            new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,faculty),
                            new Tuple<string, Relation, object>("protocol_number",Relation.NOT_EQUAL,null)
                          }).Join(
                          _DB_Connection.Select(DB_Table.ORDERS_HAS_APPLICATIONS),
                          k1 => k1[0],
                          k2 => k2[0],
                          (s1, s2) => s2[1]
                          ).Count() + applications.Count());

                    if (places < occupied && MessageBox.Show(
                        "Число мест в общежитии, выделенных для факультета, меньше суммарного числа абитуриентов, назначенных на них по приказам. Продолжить регистрацию?",
                        "Внимание",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                        ) == DialogResult.No)
                        return;
                }

                _DB_Connection.Update(
                    DB_Table.ORDERS,
                    new Dictionary<string, object> { { "protocol_number", ushort.Parse(tbNumber.Text) }, { "protocol_date", dtpDate.Value } },
                    new Dictionary<string, object> { { "number", _Number } },
                    transaction
                    );

                transaction.Commit();
            }

            Cursor.Current = Cursors.Default;

            DialogResult = DialogResult.OK;
        }

        private void tbNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void tbNumber_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ushort r;
            if (!ushort.TryParse(tbNumber.Text, out r))
            {
                MessageBox.Show("Превышено максимальное значение номера.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }
    }
}
