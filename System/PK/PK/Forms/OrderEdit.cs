using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using RB_Tag = System.Tuple<string, uint>;
using CB_Value = System.Tuple<string, uint, string>;

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
        private readonly bool _IsMaster;

        public OrderEdit(Classes.DB_Connector connection, string number, bool readOnly)
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

            if (readOnly)
            {
                foreach (Control c in Controls)
                    c.Enabled = false;

                dataGridView.Enabled = true;
                dataGridView.ReadOnly = true;
            }
            #endregion

            cbFDP.DisplayMember = "Display";
            cbFDP.ValueMember = "Value";

            _IsMaster = _DB_Helper.IsMasterCampaign(Classes.Utility.CurrentCampaignID);

            if (_IsMaster)
            {
                dataGridView_Status.Visible = false;
                dataGridView_MFR.Visible = false;
                dataGridView_MOR.Visible = false;
                dataGridView_ROI.Visible = false;
                dataGridView_Math.Visible = false;
                dataGridView_Physics.Visible = false;
                dataGridView_Russian.Visible = false;
                dataGridView_Social.Visible = false;
                dataGridView_Foreign.Visible = false;

                dataGridView_Sum.Visible = true;
                dataGridView_Exam.Visible = true;
                dataGridView_IndAch.Visible = true;
                dataGridView_Honors.Visible = true;
            }

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
                else
                    cbFDP.SelectedValue = new CB_Value(order[4].ToString(), (uint)order[5], order[6] as string);

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

                if (_IsMaster)
                    lFDP.Text = "Программа:";
                else
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

            if (!_IsMaster)
                lFDP.Text = rbPaid.Checked ? "Профиль" : "Направление";
            cbShowAdmitted.Enabled = rbPaid.Checked && cbType.SelectedValue.ToString() == "admission";
        }

        private void cbFDP_DropDown(object sender, EventArgs e) //TODO нужна проверка на кол-во мест?
        {
            Cursor.Current = Cursors.WaitCursor;

            int selectedIndex = cbFDP.SelectedIndex;

            if (cbType.SelectedValue.ToString() == "hostel")
            {
                cbFDP.DataSource = _DB_Connection.Select(
                    DB_Table.CAMPAIGNS_FACULTIES_DATA,
                    new string[] { "faculty_short_name" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Utility.CurrentCampaignID) }
                    ).Join(
                    _DB_Connection.Select(DB_Table.FACULTIES, "short_name", "name"),
                    k1 => k1[0],
                    k2 => k2[0],
                    (s1, s2) => new { Value = s2[0], Display = s2[1] }
                    ).ToList();
            }
            else if (rbPaid.Checked || _IsMaster)
                cbFDP.DataSource = _DB_Connection.Select(
                    DB_Table.CAMPAIGNS_PROFILES_DATA,
                    new string[] { "profiles_direction_faculty", "profiles_direction_id", "profiles_short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_id",Relation.EQUAL,Classes.Utility.CurrentCampaignID),
                        new Tuple<string, Relation, object>("places_paid_"+((RB_Tag)gbEduForm.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag).Item1, Relation.GREATER,0 )
                    }).Select(
                    s => new
                    {
                        Value = new CB_Value(s[0].ToString(), (uint)s[1], s[2].ToString()),
                        Display = s[0].ToString() + " " + Classes.DB_Queries.GetProfileName(_DB_Connection, s[0].ToString(), (uint)s[1], s[2].ToString())
                    }).ToList();
            else if (rbTarget.Checked)
                cbFDP.DataSource = _DB_Connection.Select(
                    DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA,
                    new string[] { "direction_faculty", "direction_id", "places_" + ((RB_Tag)gbEduForm.Controls.Cast<Control>().Single(c => ((RadioButton)c).Checked).Tag).Item1 },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Classes.Utility.CurrentCampaignID)
                    }).GroupBy(k => Tuple.Create(k[0], k[1]), (k, g) => new { Faculty = k.Item1.ToString(), DirID = (uint)k.Item2, Places = g.Sum(s => (ushort)s[2]) })
                    .Where(s => s.Places > 0)
                   .Select(s => new { Value = new CB_Value(s.Faculty, s.DirID, null), Display = s.Faculty + " " + _DB_Helper.GetDirectionNameAndCode(s.DirID).Item1 }).ToList();
            else
                cbFDP.DataSource = _DB_Connection.Select(
                    DB_Table.CAMPAIGNS_DIRECTIONS_DATA,
                        new string[] { "direction_faculty", "direction_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Classes.Utility.CurrentCampaignID),
                            new Tuple<string, Relation, object>("places_" +((RB_Tag)gbEduSource.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag).Item1+
                            "_"+((RB_Tag)gbEduForm.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag).Item1,                            Relation.GREATER,0 )
                        }).Select(s => new { Value = new CB_Value(s[0].ToString(), (uint)s[1], null), Display = s[0].ToString() + " " + _DB_Helper.GetDirectionNameAndCode((uint)s[1]).Item1 }).ToList();

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
            if (!AssureSave())
                return;

            Cursor.Current = Cursors.WaitCursor;

            string faculty;
            uint? direction = null;
            string profile = null;
            if (cbType.SelectedValue.ToString() != "hostel")
            {
                CB_Value value = (CB_Value)cbFDP.SelectedValue;
                faculty = value.Item1;
                direction = value.Item2;
                profile = value.Item3;
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
                { "campaign_id",Classes.Utility.CurrentCampaignID },
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
            if (cbFDP.SelectedIndex == -1)
                return;

            Cursor.Current = Cursors.WaitCursor;
            FillTable(GetAdmissionCandidates());
            Cursor.Current = Cursors.Default;
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            dtpDate.Tag = true;
        }

        private IEnumerable<uint> GetAdmissionCandidates()
        {
            CB_Value buf = (CB_Value)cbFDP.SelectedValue;
            List<Tuple<string, Relation, object>> filter = new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("edu_form_id",Relation.EQUAL,CheckedEduForm),
                new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,CheckedEduSource),
                new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,buf.Item1),
                new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,buf.Item2),
                new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,buf.Item3)
            };

            if (rbPaid.Checked)
            {
                var entranceApplications = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "application_id" }, filter).Select(s => (uint)s[0]);

                if (cbShowAdmitted.Checked)
                    return GetCampApplsWithStatuses("new", "adm_budget", "adm_paid", "adm_both").Join(entranceApplications, k1 => k1, k2 => k2, (s1, s2) => s1);

                return GetCampApplsWithStatuses("new").Join(entranceApplications, k1 => k1, k2 => k2, (s1, s2) => s1)
                    .Where(appl =>
                    {
                        var orders = GetAdmExcOrders(appl);
                        var admOrders = orders.Where(s => s.Item1 == "admission");
                        if (admOrders.Count() == 0)
                            return true;

                        DateTime lastAdmDate = admOrders.Max(s => s.Item2);
                        var excOrders = orders.Where(s => s.Item1 == "exception");
                        DateTime lastExcDate = excOrders.Any() ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

                        return lastAdmDate < lastExcDate;//TODO <=?
                    });
            }
            else
            {
                filter.Add(new Tuple<string, Relation, object>("is_agreed_date", Relation.NOT_EQUAL, null));
                filter.Add(new Tuple<string, Relation, object>("is_disagreed_date", Relation.EQUAL, null));

                var entranceApplications = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "application_id" }, filter).Select(s => (uint)s[0]);

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
                DateTime lastExcDate = excOrders.Any() ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

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
                DateTime lastExcDate = excOrders.Any() ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

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
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Utility.CurrentCampaignID) }
                ).Where(s => statuses.Contains(s[1].ToString())).Select(s => (uint)s[0]);
        }

        private IEnumerable<Tuple<string, DateTime>> GetAdmExcOrders(uint applicationID)
        {
            CB_Value buf = (CB_Value)cbFDP.SelectedValue;

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
                        new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, buf.Item1),
                        new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, buf.Item2),
                        new Tuple<string, Relation, object>("profile_short_name", Relation.EQUAL, buf.Item3)
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

            if (_IsMaster)
            {
                if (cbType.SelectedValue.ToString() == "hostel")
                    foreach (var appl in candidates)
                        dataGridView.Rows.Add(false, appl.ApplID, appl.Name);
                else
                {
                    CB_Value buf = (CB_Value)cbFDP.SelectedValue;

                    var marks = _DB_Connection.Select(
                        DB_Table.MASTERS_EXAMS_MARKS,
                        new string[] { "entrant_id", "mark", "bonus" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL, Classes.Utility.CurrentCampaignID),
                            new Tuple<string, Relation, object>("faculty",Relation.EQUAL,buf.Item1),
                            new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,buf.Item2),
                            new Tuple<string, Relation, object>("profile_short_name",Relation.EQUAL,buf.Item3),
                            new Tuple<string, Relation, object>("mark",Relation.NOT_EQUAL,-1)
                        });

                    var table = candidates.Join(
                        marks,
                        k1 => k1.EntrID,
                        k2 => k2[0],
                        (s1, s2) => new { s1.ApplID, s1.Name, Mark = (short)s2[1], Bonus = (ushort)s2[2] }
                        ).GroupJoin(
                        _DB_Connection.Select(DB_Table.INDIVIDUAL_ACHIEVEMENTS, "application_id", "institution_achievement_id").Join(
                            _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, "id", "value"),
                            k1 => k1[1],
                            k2 => k2[0],
                            (s1, s2) => new { ApplID = (uint)s1[0], Value = (ushort)s2[1] }
                            ),
                        k1 => k1.ApplID,
                        k2 => k2.ApplID,
                        (s1, s2) => new { s1.ApplID, s1.Name, s1.Mark, s1.Bonus, IndAch = s2.Any() ? s2.Max(s => s.Value) : 0 }
                        );

                    foreach (var appl in table)
                        dataGridView.Rows.Add(false, appl.ApplID, appl.Name, null, null, null, null, null, null, null, null, null, appl.Mark + appl.IndAch + appl.Bonus, appl.Mark, appl.IndAch, appl.Bonus);
                }
            }
            else
            {
                IEnumerable<Classes.DB_Queries.Mark> marks = Classes.DB_Queries.GetMarks(_DB_Connection, candidates.Select(s => s.ApplID), Classes.Utility.CurrentCampaignID);

                var table = candidates.Join(
                    marks,
                    k1 => k1.ApplID,
                    k2 => k2.ApplID,
                    (s1, s2) => new { s1.ApplID, s1.Name, s2.SubjID, s2.Value, s2.Checked }
                    ).GroupBy(
                    k1 => k1.ApplID,
                    (k1, g1) =>
                    new
                    {
                        ApplID = k1,
                        g1.First().Name,
                        Subjects = g1.GroupBy(
                            k2 => k2.SubjID,
                            (k2, g2) =>
                            new
                            {
                                Subj = k2,
                                Mark = g2.Any(s => s.Checked) ? g2.Where(s => s.Checked).Max(s => s.Value) : g2.Max(s => s.Value),
                                Checked = g2.Any(s => s.Checked)
                            })
                    });


                if (cbType.SelectedValue.ToString() == "hostel")
                    foreach (var appl in table)
                        AddBachelorRow(appl.ApplID, appl.Name, null, appl.Subjects.Select(s => Tuple.Create(s.Subj, s.Mark)));
                else
                {
                    byte[] buf = Array.ConvertAll(
                        _DB_Connection.Select(DB_Table.CONSTANTS, "min_math_mark", "min_russian_mark", "min_physics_mark", "min_social_mark", "min_foreign_mark")[0],
                        s => (byte)(ushort)s
                        );

                    Dictionary<uint, byte> minMarks = new Dictionary<uint, byte>
                    {
                        { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Математика"),buf[0] },
                        { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Русский язык"),buf[1] },
                        { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Физика"),buf[2] },
                        { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Обществознание"),buf[3] },
                        { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Иностранный язык"),buf[4] }
                    };

                    IEnumerable<uint> dir_subjects = _DB_Connection.Select(
                        DB_Table.ENTRANCE_TESTS,
                        new string[] { "subject_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,Classes.Utility.CurrentCampaignID),
                            new Tuple<string, Relation, object>("direction_faculty",Relation.EQUAL,((CB_Value)cbFDP.SelectedValue).Item1),
                            new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,((CB_Value)cbFDP.SelectedValue).Item2)
                        }).Select(s => (uint)s[0]);

                    foreach (var appl in table)
                    {
                        var appl_dir_subj = appl.Subjects.Join(dir_subjects, k1 => k1.Subj, k2 => k2, (s1, s2) => s1);
                        if (appl_dir_subj.Count() == dir_subjects.Count())
                        {
                            string status = appl_dir_subj.All(s => s.Checked) ? (appl_dir_subj.Any(s => s.Mark < minMarks[s.Subj]) ? "Ниже мин." : "OK") : "Непров. ЕГЭ";

                            AddBachelorRow(appl.ApplID, appl.Name, status, appl.Subjects.Select(s => Tuple.Create(s.Subj, s.Mark)));
                        }
                    }
                }
            }

            dataGridView.Sort(dataGridView_Name, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void AddBachelorRow(uint applID, string name, string status, IEnumerable<Tuple<uint, byte>> applMarks)
        {
            byte? math = applMarks.SingleOrDefault(s => s.Item1 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Математика"))?.Item2 as byte?;
            byte? rus = applMarks.SingleOrDefault(s => s.Item1 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Русский язык"))?.Item2 as byte?;
            byte? phys = applMarks.SingleOrDefault(s => s.Item1 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Физика"))?.Item2 as byte?;
            byte? soc = applMarks.SingleOrDefault(s => s.Item1 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Обществознание"))?.Item2 as byte?;
            byte? foreign = applMarks.SingleOrDefault(s => s.Item1 == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, "Иностранный язык"))?.Item2 as byte?;

            ushort? MFR = null;
            if (math != null && phys != null && rus != null)
                MFR = (ushort)(math + phys + rus);

            ushort? MOR = null;
            if (math != null && soc != null && rus != null)
                MOR = (ushort)(math + soc + rus);

            ushort? ROI = null;
            if (rus != null && soc != null && foreign != null)
                ROI = (ushort)(rus + soc + foreign);

            dataGridView.Rows.Add(false, applID, name, status, MFR, MOR, ROI, math, phys, rus, soc, foreign);
        }

        private bool AssureSave()
        {
            if (tbNumber.Text == "")
            {
                MessageBox.Show("Не заполнен номер приказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cbFDP.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран факультет/направление/профиль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_EditNumber != tbNumber.Text && _DB_Connection.Select(DB_Table.ORDERS, "number").Any(s => s[0].ToString() == tbNumber.Text))
            {
                MessageBox.Show("Приказ с таким номером уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!(bool)dtpDate.Tag && !Classes.Utility.ShowChoiceMessageBox("Поле даты не менялось. Продолжить сохранение?", "Предупреждение"))
                return false;

            return true;
        }
    }
}
