using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using RB_Tag = System.Tuple<string, uint>;
using CB_B_Value = System.Tuple<string, uint>;
using CB_P_Value = System.Tuple<string, uint, string>;

namespace PK.Forms
{
    partial class OrderEdit : Form
    {
        private uint CheckedEduSource
        {
            get { return ((RB_Tag)gbEduSource.Controls.Cast<Control>().Single(c => ((RadioButton)c).Checked).Tag).Item2; }
            set { gbEduSource.Controls.Cast<RadioButton>().Single(rb => ((RB_Tag)rb.Tag).Item2 == value).Checked = true; }
        }

        private uint CheckedEduForm
        {
            get { return ((RB_Tag)gbEduForm.Controls.Cast<Control>().Single(c => ((RadioButton)c).Checked).Tag).Item2; }
            set { gbEduForm.Controls.Cast<RadioButton>().Single(rb => ((RB_Tag)rb.Tag).Item2 == value).Checked = true; }
        }

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly string _EditNumber;

        public OrderEdit(Classes.DB_Connector connection, string number)
        {
            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _EditNumber = number;

            #region Components
            InitializeComponent();

            cbType.DisplayMember = "Item2";
            cbType.ValueMember = "Item1";
            cbType.DataSource = new List<Tuple<string, string>>
            {
                Tuple.Create( "admission" ,"Зачисление" ),
                Tuple.Create( "exception" ,"Отчисление"),
                Tuple.Create("hostel" ,"Выделение мест в общежитии" )
            };

            rbBudget.Tag = new RB_Tag("budget", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceB));
            rbPaid.Tag = new RB_Tag(null, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP));
            rbTarget.Tag = new RB_Tag(null, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT));
            rbQuota.Tag = new RB_Tag("quota", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ));
            rbO.Tag = new RB_Tag("o", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO));
            rbOZ.Tag = new RB_Tag("oz", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ));
            rbZ.Tag = new RB_Tag("z", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormZ));

            if (_EditNumber != null)
                dtpDate.Tag = true;
            else
                dtpDate.Tag = false;
            #endregion

            cbFDP.DisplayMember = "Display";
            cbFDP.ValueMember = "Value";

            if (_EditNumber != null)
            {
                object[] order = _DB_Connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "type", "date", "edu_form_id", "edu_source_id", "faculty_short_name",/**/ "direction_id"/**/, "profile_short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("number",Relation.EQUAL,_EditNumber)
                    })[0];

                tbNumber.Text = _EditNumber;
                dtpDate.Value = (DateTime)order[1];

                string type = order[0].ToString();
                cbType.SelectedValue = type;

                CheckedEduSource = (uint)order[3];
                CheckedEduForm = (uint)order[2];

                cbFDP_DropDown(null, null);

                if (type == "hostel")
                    cbFDP.SelectedValue = order[4].ToString();
                else if (rbPaid.Checked)
                    cbFDP.SelectedValue = new CB_P_Value(order[4].ToString(), (uint)order[5], order[6].ToString());
                else
                    cbFDP.SelectedValue = new CB_B_Value(order[4].ToString(), (uint)order[5]);

                cbFDP_SelectionChangeCommitted(null, null);

                foreach (uint applID in _DB_Connection.Select(
                    DB_Table.ORDERS_HAS_APPLICATIONS,
                    new string[] { "applications_id" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, _EditNumber) }
                    ).Select(s => (uint)s[0]))
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if ((uint)row.Cells[dataGridView_ID.Index].Value == applID)
                            row.Cells[dataGridView_Added.Index].Value = true;

                cbType.Enabled = false;
                gbEduSource.Enabled = false;
                gbEduForm.Enabled = false;
                cbFDP.Enabled = false;
                bSave.Enabled = true;
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            rbBudget.Checked = true;
            rbO.Checked = true;
            cbFDP.SelectedIndex = -1;
            dataGridView.Rows.Clear();

            if (cbType.SelectedValue.ToString() == "hostel")
            {
                rbPaid.Enabled = false;
                gbEduForm.Enabled = false;

                lFDP.Text = "Факультет:";

                dataGridView_Status.Visible = false;
            }
            else
            {
                rbPaid.Enabled = true;
                gbEduForm.Enabled = true;

                lFDP.Text = "Направление:";

                dataGridView_Status.Visible = true;
            }
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                cbFDP.SelectedIndex = -1;
                dataGridView.Rows.Clear();

                if (rb == rbPaid)
                    rbZ.Enabled = true;
                else if (rb == rbBudget || rb == rbTarget || rb == rbQuota)
                {
                    rbZ.Enabled = false;
                    if (rbZ.Checked)
                        rbO.Checked = true;
                }
            }
        }

        private void rbPaid_CheckedChanged(object sender, EventArgs e)
        {
            rb_CheckedChanged(sender, e);

            lFDP.Text = rbPaid.Checked ? "Профиль" : "Направление";
            cbShowAdmitted.Enabled = rbPaid.Checked && cbType.SelectedValue.ToString() == "admission";
        }

        private void cbFDP_DropDown(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            int selectedIndex = cbFDP.SelectedIndex;

            if (cbType.SelectedValue.ToString() == "hostel")
            {
                cbFDP.DataSource = _DB_Connection.Select(
                    DB_Table.CAMPAIGNS_FACULTIES_DATA,
                    new string[] { "faculty_short_name" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _DB_Helper.CurrentCampaignID) }
                    ).Join(
                    _DB_Connection.Select(DB_Table.FACULTIES, "short_name", "name"),
                    k1 => k1[0],
                    k2 => k2[0],
                    (s1, s2) => new { Value = s2[0], Display = s2[1] }
                    ).ToList();
            }
            else
            {
                if (rbPaid.Checked)
                    cbFDP.DataSource = _DB_Connection.Select(
                        DB_Table.CAMPAIGNS_PROFILES_DATA,
                        new string[] { "profiles_direction_faculty", "profiles_direction_id", "profiles_short_name" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaigns_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID),
                            new Tuple<string, Relation, object>("places_paid_"+((RB_Tag)gbEduForm.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag).Item1, Relation.GREATER,0 )
                        }).Select(
                        s => new
                        {
                            Value = new CB_P_Value(s[0].ToString(), (uint)s[1], s[2].ToString()),
                            Display = s[0].ToString() + " " + _DB_Connection.Select(
                                DB_Table.PROFILES,
                                new string[] { "name" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    Tuple.Create("faculty_short_name",Relation.EQUAL,s[0]),
                                    Tuple.Create("direction_id",Relation.EQUAL,s[1]),
                                    Tuple.Create("short_name",Relation.EQUAL,s[2])
                                })[0][0].ToString()
                        }
                        ).ToList();
                else if (rbTarget.Checked)
                    cbFDP.DataSource = _DB_Connection.Select(
                        DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA,
                        new string[] { "direction_faculty", "direction_id", "places_" + ((RB_Tag)gbEduForm.Controls.Cast<Control>().Single(c => ((RadioButton)c).Checked).Tag).Item1 },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID)
                        }).GroupBy(k => Tuple.Create(k[0], k[1]), (k, g) => new { Faculty = k.Item1.ToString(), DirID = (uint)k.Item2, Places = g.Sum(s => (ushort)s[2]) })
                        .Where(s => s.Places > 0)
                        .Select(s => new { Value = new CB_B_Value(s.Faculty, s.DirID), Display = s.Faculty + " " + _DB_Helper.GetDirectionNameAndCode(s.DirID).Item1 }).ToList();
                else
                    cbFDP.DataSource = _DB_Connection.Select(
                            DB_Table.CAMPAIGNS_DIRECTIONS_DATA,
                            new string[] { "direction_faculty", "direction_id" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID),
                                new Tuple<string, Relation, object>("places_" +((RB_Tag)gbEduSource.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag).Item1+
                                "_"+((RB_Tag)gbEduForm.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag).Item1,                            Relation.GREATER,0 )
                            }).Select(s => new { Value = new CB_B_Value(s[0].ToString(), (uint)s[1]), Display = s[0].ToString() + " " + _DB_Helper.GetDirectionNameAndCode((uint)s[1]).Item1 }).ToList();
            }

            cbFDP.SelectedIndex = selectedIndex;

            Cursor.Current = Cursors.Default;
        }

        private void cbFDP_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (cbType.SelectedValue.ToString() == "admission")
                FillTable(GetAdmissionCandidates());
            else if (cbType.SelectedValue.ToString() == "exception")
                FillTable(GetExceptionCandidates());
            else
                FillTable(GetHostelCandidates());

            Cursor.Current = Cursors.Default;
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex == dataGridView_Added.Index)
            {
                bool enable = false;
                if (!(bool)dataGridView[e.ColumnIndex, e.RowIndex].Value)
                {
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if ((bool)row.Cells[e.ColumnIndex].Value)
                            return;

                    enable = true;
                }

                lType.Enabled = enable;
                cbType.Enabled = enable;
                gbEduSource.Enabled = enable;
                gbEduForm.Enabled = enable;
                lFDP.Enabled = enable;
                cbFDP.Enabled = enable;
                bSave.Enabled = !enable;
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            if (tbNumber.Text == "")
            {
                MessageBox.Show("Не заполнен номер приказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbFDP.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран факультет/направление/профиль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_EditNumber != tbNumber.Text && _DB_Connection.Select(DB_Table.ORDERS, "number").Any(s => s[0].ToString() == tbNumber.Text))
            {
                MessageBox.Show("Приказ с таким номером уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(bool)dtpDate.Tag && !Classes.Utility.ShowChoiceMessageBox("Поле даты не менялось. Продолжить сохранение?", "Предупреждение"))
                return;

            Cursor.Current = Cursors.WaitCursor;

            string faculty;
            uint? direction = null;
            string profile = null;
            if (cbType.SelectedValue.ToString() != "hostel")
                if (rbPaid.Checked)
                {
                    CB_P_Value value = (CB_P_Value)cbFDP.SelectedValue;
                    faculty = value.Item1;
                    direction = value.Item2;
                    profile = value.Item3;
                }
                else
                {
                    CB_B_Value value = (CB_B_Value)cbFDP.SelectedValue;
                    faculty = value.Item1;
                    direction = value.Item2;
                }
            else
                faculty = cbFDP.SelectedValue.ToString();

            Dictionary<string, object> values = new Dictionary<string, object>
            {
                { "type", cbType.SelectedValue},
                { "date", dtpDate.Value},
                { "edu_form_dict_id", (uint)FIS_Dictionary.EDU_FORM},
                { "edu_form_id",CheckedEduForm },
                { "edu_source_dict_id",(uint)FIS_Dictionary.EDU_SOURCE },
                { "edu_source_id", CheckedEduSource},
                { "campaign_id",_DB_Helper.CurrentCampaignID },
                { "faculty_short_name",faculty },
                { "direction_id", direction},
                { "profile_short_name",profile }
            };

            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
            {
                if (_EditNumber != null)
                {
                    if (tbNumber.Text != _EditNumber)
                        values.Add("number", tbNumber.Text);

                    _DB_Connection.Update(
                        DB_Table.ORDERS,
                        values,
                        new Dictionary<string, object> { { "number", _EditNumber } },
                        transaction
                        );

                    string[] fields = { "orders_number", "applications_id" };
                    List<object[]> oldL = _DB_Connection.Select(
                        DB_Table.ORDERS_HAS_APPLICATIONS,
                        fields,
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, tbNumber.Text) }
                        );

                    List<object[]> newL = new List<object[]>();
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if ((bool)row.Cells[dataGridView_Added.Index].Value)
                            newL.Add(new object[] { tbNumber.Text, row.Cells[dataGridView_ID.Index].Value });

                    _DB_Helper.UpdateData(DB_Table.ORDERS_HAS_APPLICATIONS, oldL, newL, fields, fields, transaction);
                }
                else
                {
                    values.Add("number", tbNumber.Text);

                    _DB_Connection.Insert(DB_Table.ORDERS, values, transaction);

                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if ((bool)row.Cells[dataGridView_Added.Index].Value)
                            _DB_Connection.Insert(
                                DB_Table.ORDERS_HAS_APPLICATIONS,
                                new Dictionary<string, object>
                                {
                                    { "orders_number", tbNumber.Text},
                                    { "applications_id", row.Cells[dataGridView_ID.Index].Value}
                                },
                                transaction
                                );
                }

                transaction.Commit();
            }

            Cursor.Current = Cursors.Default;

            Classes.Utility.ShowChangesSavedMessage();
            DialogResult = DialogResult.OK;
        }

        private void cbShowAdmitted_CheckedChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            FillTable(GetAdmissionCandidates());
            Cursor.Current = Cursors.Default;
        }

        private IEnumerable<uint> GetAdmissionCandidates()
        {
            if (rbPaid.Checked)
            {
                CB_P_Value buf = (CB_P_Value)cbFDP.SelectedValue;
                var entranceApplications = _DB_Connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "application_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("edu_form_id",Relation.EQUAL,CheckedEduForm),
                        new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,CheckedEduSource),
                        new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,buf.Item1),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,buf.Item2),
                        new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,buf.Item3)
                    }).Select(s => (uint)s[0]);

                string[] statuses;
                if (cbShowAdmitted.Checked)
                    statuses = new string[] { "new", "adm_budget", "adm_paid", "adm_both" };
                else
                    statuses = new string[] { "new" };

                return GetCampApplsWithStatuses(statuses).Join(entranceApplications, k1 => k1, k2 => k2, (s1, s2) => s1)
                    .Where(appl =>
                    {
                        var orders = GetAdmExcOrders(appl);
                        var admOrders = orders.Where(s => s.Item1 == "admission");
                        if (admOrders.Count() == 0)
                            return true;

                        DateTime lastAdmDate = admOrders.Max(s => s.Item2);
                        var excOrders = orders.Where(s => s.Item1 == "exception");
                        DateTime lastExcDate = excOrders.Count() != 0 ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

                        return lastAdmDate < lastExcDate;//TODO <=?
                    });
            }
            else
            {
                CB_B_Value buf = (CB_B_Value)cbFDP.SelectedValue;
                var entranceApplications = _DB_Connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "application_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("edu_form_id",Relation.EQUAL,CheckedEduForm),
                        new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,CheckedEduSource),
                        new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,buf.Item1),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,buf.Item2),
                        new Tuple<string, Relation, object>("is_agreed_date",Relation.NOT_EQUAL,null),
                        new Tuple<string, Relation, object>("is_disagreed_date",Relation.EQUAL,null)
                    }).Select(s => (uint)s[0]);

                return GetCampApplsWithStatuses("new", "adm_paid").Join(entranceApplications, k1 => k1, k2 => k2, (s1, s2) => s1);
            }
        }

        private IEnumerable<uint> GetExceptionCandidates()
        {
            string[] statuses;
            if (rbPaid.Checked)
                statuses = new string[] { "adm_paid", "adm_both" };
            else
                statuses = new string[] { "adm_budget", "adm_both" };

            return GetCampApplsWithStatuses(statuses).Where(appl =>
            {
                var orders = GetAdmExcOrders(appl);
                var admOrders = orders.Where(s => s.Item1 == "admission");
                if (admOrders.Count() == 0)
                    return false;

                DateTime lastAdmDate = admOrders.Max(s => s.Item2);
                var excOrders = orders.Where(s => s.Item1 == "exception");
                DateTime lastExcDate = excOrders.Count() != 0 ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

                return lastAdmDate > lastExcDate;//TODO >=?
            });
        }

        private IEnumerable<uint> GetHostelCandidates()
        {
            return GetCampApplsWithStatuses("adm_budget", "adm_both").Where(appl =>
            {
                var orders = GetAdmExcOrders(appl, cbFDP.SelectedValue.ToString());
                var admOrders = orders.Where(s => s.Item1 == "admission");
                if (admOrders.Count() == 0)
                    return false;

                DateTime lastAdmDate = admOrders.Max(s => s.Item2);
                var excOrders = orders.Where(s => s.Item1 == "exception");
                DateTime lastExcDate = excOrders.Count() != 0 ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

                if (lastAdmDate > lastExcDate)//TODO >=?
                    return _DB_Connection.Select(
                        DB_Table.ORDERS_HAS_APPLICATIONS,
                        new string[] { "orders_number" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, appl) }
                        ).Join(
                        _DB_Connection.Select(
                            DB_Table.ORDERS,
                            new string[] { "number" },
                            new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("protocol_number",Relation.NOT_EQUAL,null),
                                new Tuple<string, Relation, object>("type", Relation.EQUAL, "hostel")
                            }),
                        k1 => k1[0],
                        k2 => k2[0],
                        (s1, s2) => s1[0]
                        ).Count() == 0;
                else
                    return false;
            });
        }

        private IEnumerable<uint> GetCampApplsWithStatuses(params string[] statuses)
        {
            return _DB_Connection.Select(
                     DB_Table.APPLICATIONS,
                     new string[] { "id", "status" },
                     new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _DB_Helper.CurrentCampaignID) }
                     ).Where(s => statuses.Contains(s[1].ToString())).Select(s => (uint)s[0]);
        }

        private IEnumerable<Tuple<string, DateTime>> GetAdmExcOrders(uint applicationID)
        {
            string faculty;
            uint direction;
            string profile = null;
            if (rbPaid.Checked)
            {
                CB_P_Value buf = (CB_P_Value)cbFDP.SelectedValue;
                faculty = buf.Item1;
                direction = buf.Item2;
                profile = buf.Item3;
            }
            else
            {
                CB_B_Value buf = (CB_B_Value)cbFDP.SelectedValue;
                faculty = buf.Item1;
                direction = buf.Item2;
            }

            return _DB_Connection.Select(
                DB_Table.ORDERS_HAS_APPLICATIONS,
                new string[] { "orders_number" },
                            new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, applicationID) }
                            ).Join(
                            _DB_Connection.Select(
                                DB_Table.ORDERS,
                                new string[] { "number", "type", "date" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("protocol_number",Relation.NOT_EQUAL,null),
                                    new Tuple<string, Relation, object>("type", Relation.NOT_EQUAL, "hostel"),
                                    new Tuple<string, Relation, object>("edu_form_id", Relation.EQUAL, CheckedEduForm),
                                    new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,CheckedEduSource),
                                    new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, faculty),
                                    new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, direction),
                                    new Tuple<string, Relation, object>("profile_short_name", Relation.EQUAL, profile)
                                }),
                            k1 => k1[0],
                            k2 => k2[0],
                            (s1, s2) => Tuple.Create(s2[1].ToString(), (DateTime)s2[2])
                            );
        }

        private IEnumerable<Tuple<string, DateTime>> GetAdmExcOrders(uint applicationID, string faculty)
        {
            return _DB_Connection.Select(
                         DB_Table.ORDERS_HAS_APPLICATIONS,
                         new string[] { "orders_number" },
                         new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, applicationID) }
                         ).Join(
                         _DB_Connection.Select(
                             DB_Table.ORDERS,
                             new string[] { "number", "type", "date" },
                             new List<Tuple<string, Relation, object>>
                             {
                                 new Tuple<string, Relation, object>("protocol_number",Relation.NOT_EQUAL,null),
                                 new Tuple<string, Relation, object>("type", Relation.NOT_EQUAL, "hostel"),
                                 new Tuple<string, Relation, object>("edu_form_id", Relation.EQUAL, CheckedEduForm),
                                 new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,CheckedEduSource),
                                 new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, faculty)
                             }),
                         k1 => k1[0],
                         k2 => k2[0],
                         (s1, s2) => Tuple.Create(s2[1].ToString(), (DateTime)s2[2])
                         );
        }

        private void FillTable(IEnumerable<uint> applications)
        {
            dataGridView.Rows.Clear();

            var candidates = applications.Join(
                _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id"),
                k1 => k1, k2 => k2[0], (s1, s2) => s2
                ).Join(
                _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                k1 => k1[1],
                k2 => k2[0],
                (s1, s2) => new { ApplID = (uint)s1[0], EntrID = (uint)s1[1], Name = s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString() }
                );

            IEnumerable<Tuple<uint, uint, byte, bool>> marks = Classes.DB_Queries.GetMarks(_DB_Connection, candidates.Select(s => s.ApplID), _DB_Helper.CurrentCampaignID);

            var table = candidates.Join(
                marks,
                k1 => k1.ApplID,
                k2 => k2.Item1,
                (s1, s2) => new { s1.ApplID, s1.Name, Subj = s2.Item2, Mark = s2.Item3, Checked = s2.Item4 }
                ).GroupBy(
                k1 => k1.ApplID,
                (k1, g1) =>
                new
                {
                    ApplID = k1,
                    g1.First().Name,
                    Subjects = g1.GroupBy(
                        k2 => k2.Subj,
                        (k2, g2) =>
                        new
                        {
                            Subj = k2,
                            Mark = g2.Any(s => s.Checked) ? g2.Where(s => s.Checked).Max(s => s.Mark) : g2.Max(s => s.Mark),
                            Checked = g2.Any(s => s.Checked)
                        })
                });

            byte[] buf = Array.ConvertAll(
                _DB_Connection.Select(DB_Table.CONSTANTS, "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark", "min_foreign_mark")[0],
                s => (byte)(ushort)s
                );
            Dictionary<uint, byte> minMarks = new Dictionary<uint, byte>
            {
                { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Математика"),buf[0] },
                {_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Русский язык"),buf[1] },
                {_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Физика"),buf[2] },
                {_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Обществознание"),buf[3] },
                {_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Иностранный язык"),buf[4] }
            };

            if (cbType.SelectedValue.ToString() == "hostel")
            {
                foreach (var appl in table)
                {
                    byte? math = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Математика"))?.Mark as byte?;
                    byte? rus = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Русский язык"))?.Mark as byte?;
                    byte? phys = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Физика"))?.Mark as byte?;
                    byte? soc = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Обществознание"))?.Mark as byte?;
                    byte? foreign = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Иностранный язык"))?.Mark as byte?;

                    ushort? MFR = null;
                    if (math != null && phys != null && rus != null)
                        MFR = (ushort)(math + phys + rus);

                    ushort? MOR = null;
                    if (math != null && soc != null && rus != null)
                        MOR = (ushort)(math + soc + rus);

                    ushort? ROI = null;
                    if (rus != null && soc != null && foreign != null)
                        ROI = (ushort)(rus + soc + foreign);

                    dataGridView.Rows.Add(false, appl.ApplID, appl.Name, null, MFR, MOR, ROI, math, phys, rus, soc, foreign);
                }
            }
            else
            {
                IEnumerable<uint> dir_subjects;
                if (rbPaid.Checked)
                    dir_subjects = _DB_Connection.Select(
                        DB_Table.ENTRANCE_TESTS,
                        new string[] { "subject_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID),
                        new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,((CB_P_Value)cbFDP.SelectedValue).Item1),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,((CB_P_Value)cbFDP.SelectedValue).Item2)
                        }).Select(s => (uint)s[0]);
                else
                    dir_subjects = _DB_Connection.Select(
                        DB_Table.ENTRANCE_TESTS,
                        new string[] { "subject_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID),
                        new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,((CB_B_Value)cbFDP.SelectedValue).Item1),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,((CB_B_Value)cbFDP.SelectedValue).Item2)
                        }).Select(s => (uint)s[0]);

                foreach (var appl in table)
                {
                    var appl_dir_subj = appl.Subjects.Join(dir_subjects, k1 => k1.Subj, k2 => k2, (s1, s2) => s1);
                    if (appl_dir_subj.Count() == dir_subjects.Count())
                    {
                        string status = appl_dir_subj.All(s => s.Checked) ? (appl_dir_subj.Any(s => s.Mark < minMarks[s.Subj]) ? "Ниже мин." : "OK") : "Непров. ЕГЭ";

                        byte? math = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Математика"))?.Mark as byte?;
                        byte? rus = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Русский язык"))?.Mark as byte?;
                        byte? phys = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Физика"))?.Mark as byte?;
                        byte? soc = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Обществознание"))?.Mark as byte?;
                        byte? foreign = appl.Subjects.SingleOrDefault(s => s.Subj == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Иностранный язык"))?.Mark as byte?;

                        ushort? MFR = null;
                        if (math != null && phys != null && rus != null)
                            MFR = (ushort)(math + phys + rus);

                        ushort? MOR = null;
                        if (math != null && soc != null && rus != null)
                            MOR = (ushort)(math + soc + rus);

                        ushort? ROI = null;
                        if (rus != null && soc != null && foreign != null)
                            ROI = (ushort)(rus + soc + foreign);

                        dataGridView.Rows.Add(false, appl.ApplID, appl.Name, status, MFR, MOR, ROI, math, phys, rus, soc, foreign);
                    }
                }
            }

            dataGridView.Sort(dataGridView_Name, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            dtpDate.Tag = true;
        }
    }
}
