using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class OrderRegistration : Form
    {
        [Flags]
        private enum ApplicationStatus : byte
        {
            NEW = 0x0,
            ADM_BUDGET = 0x1,
            ADM_PAID = 0x2,
            ADM_BOTH = ADM_BUDGET | ADM_PAID,
        }

        [Flags]
        private enum EducationLevel : byte
        {
            NONE = 0x0,
            BACHELOR = 0x1,
            MASTER = 0x2,
            BOTH = BACHELOR | MASTER
        }

        private readonly Tuple<string, ApplicationStatus>[] _Statuses =            {
            Tuple.Create( "new" ,ApplicationStatus.NEW ),
            Tuple.Create(  "adm_budget" ,ApplicationStatus.ADM_BUDGET ),
            Tuple.Create(  "adm_paid" ,ApplicationStatus.ADM_PAID ),
            Tuple.Create(  "adm_both" ,ApplicationStatus.ADM_BOTH )
        };

        private readonly Tuple<Tuple<uint, bool, EducationLevel>, Tuple<uint, uint>>[] _RecordBooksRanges =
            {
            Tuple.Create(Tuple.Create((uint)11,false,EducationLevel.BACHELOR), Tuple.Create((uint)180001, (uint)181999)),
            Tuple.Create(Tuple.Create((uint)11,false,EducationLevel.MASTER), Tuple.Create((uint)184001, (uint)184999)),
            Tuple.Create(Tuple.Create((uint)11,true,EducationLevel.BOTH), Tuple.Create((uint)183001, (uint)183899)),
            Tuple.Create(Tuple.Create((uint)12,false,EducationLevel.BACHELOR), Tuple.Create((uint)182001, (uint)182399)),
            Tuple.Create(Tuple.Create((uint)12,true,EducationLevel.BACHELOR), Tuple.Create((uint)183901, (uint)183999)),
            Tuple.Create(Tuple.Create((uint)10,true,EducationLevel.BOTH), Tuple.Create((uint)185001, (uint)185999))
        };

        private readonly DB_Connector _DB_Connection;
        private readonly string _Number;

        public OrderRegistration(DB_Connector connection, string number)
        {
            _DB_Connection = connection;

            #region Components
            InitializeComponent();

            tbNumber.Text = (_DB_Connection.Select(DB_Table.ORDERS, "protocol_number").Max(s => s[0] as ushort? != null ? (ushort)s[0] : 1)).ToString();
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

            EducationLevel eduLevel = new DB_Helper(_DB_Connection).GetCampaignType(Classes.Settings.CurrentCampaignID)==DB_Helper.CampaignType.MASTER ? EducationLevel.MASTER : EducationLevel.BACHELOR; //TODO

            object[] buf = _DB_Connection.Select(
                DB_Table.ORDERS,
                new string[] { "type", "edu_form_id", "edu_source_id", "faculty_short_name" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("number", Relation.EQUAL, _Number) }
                )[0];

            string type = buf[0].ToString();
            uint eduForm = (uint)buf[1];
            bool paid = (uint)buf[2] == 15; //TODO
            string faculty = buf[3].ToString();

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
                    {
                        _DB_Connection.Update(
                            DB_Table.ORDERS_HAS_APPLICATIONS,
                            new Dictionary<string, object> { { "record_book_number", GetFreeRecordBookNumber(eduForm, paid, eduLevel) } },
                            new Dictionary<string, object>
                            {
                                { "orders_number", _Number },
                                { "applications_id", appl.ID }
                            },
                            transaction
                            );

                        _DB_Connection.Update(
                            DB_Table.APPLICATIONS,
                            new Dictionary<string, object>
                            {
                                { "status",_Statuses.Single(s1=>s1.Item2== (_Statuses.Single(s2=>s2.Item1==appl.Status).Item2|(paid?ApplicationStatus.ADM_PAID:ApplicationStatus.ADM_BUDGET))).Item1 }//TODO !!!
                            },
                            new Dictionary<string, object> { { "id", appl.ID } },
                            transaction
                            );
                    }
                }
                else if (type == "exception")
                {
                    foreach (var appl in applications)
                        _DB_Connection.Update(
                            DB_Table.APPLICATIONS,
                            new Dictionary<string, object>
                            {
                                { "status", _Statuses.Single(s1 => s1.Item2 == (_Statuses.Single(s2 => s2.Item1 == appl.Status).Item2 & ~(paid ? ApplicationStatus.ADM_PAID : ApplicationStatus.ADM_BUDGET))).Item1 }//TODO !!!
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
                            new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Classes.Settings.CurrentCampaignID),
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

        private uint GetFreeRecordBookNumber(uint eduForm, bool paid, EducationLevel eduLevel)
        {
            var range = _RecordBooksRanges.Single(s => s.Item1.Item1 == eduForm && s.Item1.Item2 == paid && (s.Item1.Item3 & eduLevel) != EducationLevel.NONE).Item2;
            var numbers = _DB_Connection.Select(
                                 DB_Table.ORDERS_HAS_APPLICATIONS,
                                 new string[] { "record_book_number" },
                                 new List<Tuple<string, Relation, object>>
                                 {
                                    new Tuple<string, Relation, object>("record_book_number",Relation.GREATER_EQUAL,range.Item1),
                                    new Tuple<string, Relation, object>("record_book_number",Relation.LESS_EQUAL,range.Item2)
                                 }).Select(s => (uint)s[0]);

            if (numbers.Any())
            {
                uint lastNumber = numbers.Max();
                if (lastNumber == range.Item2)
                    throw new InvalidOperationException("Превышение границы диапазона номеров зачётных книжек.");

                return lastNumber + 1;
            }
            else
                return range.Item1;
        }
    }
}
