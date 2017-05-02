﻿using System;
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

        private string _EditNumber;

        public OrderEdit(Classes.DB_Connector connection, string number)
        {
            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            #region Components
            InitializeComponent();

            cbType.DisplayMember = "Item2";
            cbType.ValueMember = "Item1";
            cbType.DataSource = new List<Tuple<string, string>>
            {
                new Tuple<string, string>( "admission" ,"Зачисление" ),
                new Tuple<string, string>( "exception" ,"Отчисление"),
                new Tuple<string, string>("hostel" ,"Выделение мест в общежитии" )
            };

            rbBudget.Tag = new RB_Tag("budget", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceB));
            rbPaid.Tag = new RB_Tag(null, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceP));
            rbTarget.Tag = new RB_Tag(null, _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceT));
            rbQuota.Tag = new RB_Tag("quota", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, Classes.DB_Helper.EduSourceQ));
            rbO.Tag = new RB_Tag("o", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormO));
            rbOZ.Tag = new RB_Tag("oz", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormOZ));
            rbZ.Tag = new RB_Tag("z", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, Classes.DB_Helper.EduFormZ));
            #endregion

            cbFDP.DisplayMember = "Display";
            cbFDP.ValueMember = "Value";

            _EditNumber = number;

            if (number != null)
            {
                object[] order = _DB_Connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "type", "date", "education_form_id", "finance_source_id", "faculty_short_name", "direction_id", "profile_short_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("number",Relation.EQUAL,number)
                    })[0];

                tbNumber.Text = number;
                dtpDate.Value = (DateTime)order[1];

                string type = order[0].ToString();
                cbType.SelectedValue = type;
                if (type == "hostel")
                {

                }
                else
                {
                    CheckedEduSource = (uint)order[3];
                    CheckedEduForm = (uint)order[2];

                    cbFDP_DropDown(null, null);
                    if (rbPaid.Checked)
                        cbFDP.SelectedValue = new CB_P_Value(order[4].ToString(), (uint)order[5], order[6].ToString());
                    else
                        cbFDP.SelectedValue = new CB_B_Value(order[4].ToString(), (uint)order[5]);

                    cbFDP_SelectionChangeCommitted(null, null);

                    foreach (uint applID in _DB_Connection.Select(
                        DB_Table._ORDERS_HAS_APPLICATIONS,
                        new string[] { "applications_id" },
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, number) }
                        ).Select(s => (uint)s[0]))
                        foreach (DataGridViewRow row in dataGridView.Rows)
                            if ((uint)row.Cells["dataGridView_ID"].Value == applID)
                                row.Cells["dataGridView_Added"].Value = true;
                }

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
                gbEduSource.Enabled = false;
                gbEduForm.Enabled = false;

                lFDP.Text = "Факультет:";
            }
            else
            {
                gbEduSource.Enabled = true;
                gbEduForm.Enabled = true;

                lFDP.Text = "Направление:";
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
                                    new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,s[0]),
                                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,s[1]),
                                    new Tuple<string, Relation, object>("short_name",Relation.EQUAL,s[2])
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
                        }).GroupBy(k => new Tuple<object, object>(k[0], k[1]), (k, g) => new { Faculty = k.Item1.ToString(), DirID = (uint)k.Item2, Places = g.Sum(s => (ushort)s[2]) })
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
        }

        private void cbFDP_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();

            uint[] applications;
            if (cbType.SelectedValue.ToString() == "admission")
            {
                applications = GetAdmissionCandidates();
            }
            else if (cbType.SelectedValue.ToString() == "exception")
            {
                applications = GetExceptionCandidates();
            }
            else
            {
                applications = null;
            }

            var names = applications.Join(
                _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id"),
                k1 => k1, k2 => k2[0], (s1, s2) => s2
                ).Join(
                _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name"),
                k1 => k1[1],
                k2 => k2[0],
                (s1, s2) => new { ApplID = (uint)s1[0], Name = s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString() }
                );

            var table = names.GroupJoin(
            _DB_Connection.Select(DB_Table.APPLICATIONS_EGE_MARKS_VIEW),
            k1 => k1.ApplID,
            k2 => k2[0],
            (s1, s2) => new
            {
                ApplID = s1.ApplID,
                Name = s1.Name,
                Math = s2.FirstOrDefault(s => (uint)s[1] == 2)?[2], //TODO
                Phys = s2.FirstOrDefault(s => (uint)s[1] == 10)?[2], //TODO
                Rus = s2.FirstOrDefault(s => (uint)s[1] == 1)?[2], //TODO
                Soc = s2.FirstOrDefault(s => (uint)s[1] == 9)?[2], //TODO
                For = s2.FirstOrDefault(s => (uint)s[1] == 6)?[2] //TODO
            });

            foreach (var ent in table)
            {
                uint? MFR = null;
                if (ent.Math != null && ent.Phys != null && ent.Rus != null)
                    MFR = (uint)ent.Math + (uint)ent.Phys + (uint)ent.Rus;

                uint? MOR = null;
                if (ent.Math != null && ent.Soc != null && ent.Rus != null)
                    MOR = (uint)ent.Math + (uint)ent.Soc + (uint)ent.Rus;

                uint? ROI = null;
                if (ent.Rus != null && ent.Soc != null && ent.For != null)
                    ROI = (uint)ent.Rus + (uint)ent.Soc + (uint)ent.For;

                dataGridView.Rows.Add(
                    false,
                    ent.ApplID,
                    ent.Name,
                    null,
                    MFR,
                    MOR,
                    ROI,
                    ent.Math,
                    ent.Phys,
                    ent.Rus,
                    ent.Soc,
                    ent.For
                    );
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if ((bool)dataGridView[e.ColumnIndex, e.RowIndex]
                    .GetEditedFormattedValue(e.RowIndex, DataGridViewDataErrorContexts.CurrentCellChange)
                    )
                {
                    cbType.Enabled = false;
                    gbEduSource.Enabled = false;
                    gbEduForm.Enabled = false;
                    cbFDP.Enabled = false;
                    bSave.Enabled = true;
                }
                else
                {
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if ((bool)row.Cells[e.ColumnIndex]
                            .GetEditedFormattedValue(row.Index, DataGridViewDataErrorContexts.CurrentCellChange)
                            )
                            return;

                    cbType.Enabled = true;
                    gbEduSource.Enabled = true;
                    gbEduForm.Enabled = true;
                    cbFDP.Enabled = true;
                    bSave.Enabled = false;
                }
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

            string faculty;
            uint direction;
            string profile;
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
                profile = null;
            }

            Dictionary<string, object> values = new Dictionary<string, object>
                            {
                                { "type", cbType.SelectedValue},
                                { "date", dtpDate.Value},
                                { "education_form_dict_id", (uint)FIS_Dictionary.EDU_FORM},
                                { "education_form_id",CheckedEduForm },
                                { "finance_source_dict_id",(uint)FIS_Dictionary.EDU_SOURCE },
                                { "finance_source_id", CheckedEduSource},
                                { "campaign_id",_DB_Helper.CurrentCampaignID },
                                { "faculty_short_name",faculty },
                                { "direction_id", direction},
                                { "profile_short_name",profile }
                            };

            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
            {
                if (cbType.SelectedValue.ToString() == "hostel")
                    throw new NotImplementedException();

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
                        DB_Table._ORDERS_HAS_APPLICATIONS,
                        fields,
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("orders_number", Relation.EQUAL, tbNumber.Text) }
                        );

                    List<object[]> newL = new List<object[]>();
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if ((bool)row.Cells["dataGridView_Added"].Value)
                            newL.Add(new object[] { tbNumber.Text, row.Cells["dataGridView_ID"].Value });

                    _DB_Helper.UpdateData(DB_Table._ORDERS_HAS_APPLICATIONS, oldL, newL, fields, fields, transaction);
                }
                else
                {
                    values.Add("number", tbNumber.Text);

                    _DB_Connection.Insert(DB_Table.ORDERS, values, transaction);

                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if ((bool)row.Cells["dataGridView_Added"].Value)
                            _DB_Connection.Insert(
                                DB_Table._ORDERS_HAS_APPLICATIONS,
                                new Dictionary<string, object>
                                {
                                    { "orders_number", tbNumber.Text},
                                    { "applications_id", row.Cells["dataGridView_ID"].Value}
                                },
                                transaction
                                );
                }

                transaction.Commit();
            }

            _EditNumber = tbNumber.Text;
        }

        private uint[] GetAdmissionCandidates()
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

                return GetCampApplsWithStatuses("new", "adm_budget", "adm_paid", "adm_both").Join(entranceApplications, k1 => k1, k2 => k2, (s1, s2) => s1)
                    .Where(appl =>
                    {
                        var orders = GetAdmExcOrders(appl, buf);
                        var admOrders = orders.Where(s => s.Item1 == "admission");
                        if (admOrders.Count() == 0)
                            return true;

                        DateTime lastAdmDate = admOrders.Max(s => s.Item2);
                        var excOrders = orders.Where(s => s.Item1 == "exception");
                        DateTime lastExcDate = excOrders.Count() != 0 ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

                        return lastAdmDate < lastExcDate;//TODO <=?
                    }).ToArray();
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

                return GetCampApplsWithStatuses("new", "adm_paid").Join(entranceApplications, k1 => k1, k2 => k2, (s1, s2) => s1).ToArray();
            }
        }

        private uint[] GetExceptionCandidates()
        {
            if (rbPaid.Checked)
                return GetCampApplsWithStatuses("adm_paid", "adm_both").Where(appl =>
                {
                    var orders = GetAdmExcOrders(appl, (CB_P_Value)cbFDP.SelectedValue);
                    var admOrders = orders.Where(s => s.Item1 == "admission");
                    if (admOrders.Count() == 0)
                        return false;

                    DateTime lastAdmDate = admOrders.Max(s => s.Item2);
                    var excOrders = orders.Where(s => s.Item1 == "exception");
                    DateTime lastExcDate = excOrders.Count() != 0 ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

                    return lastAdmDate > lastExcDate;//TODO >=?
                }).ToArray();
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
                        new Tuple<string, Relation, object>("is_disagreed_date",Relation.NOT_EQUAL,null)
                    }).Select(s => (uint)s[0]);

                return GetCampApplsWithStatuses("adm_budget", "adm_both").Join(entranceApplications, k1 => k1, k2 => k2, (s1, s2) => s1).Where(appl =>
                {
                    var orders = GetAdmExcOrders(appl, buf);
                    var admOrders = orders.Where(s => s.Item1 == "admission");
                    if (admOrders.Count() == 0)
                        return false;

                    DateTime lastAdmDate = admOrders.Max(s => s.Item2);
                    var excOrders = orders.Where(s => s.Item1 == "exception");
                    DateTime lastExcDate = excOrders.Count() != 0 ? excOrders.Max(s => s.Item2) : DateTime.MinValue;

                    return lastAdmDate > lastExcDate;//TODO >=?
                }).ToArray();
            }
        }

        private IEnumerable<uint> GetCampApplsWithStatuses(params string[] statuses)
        {
            return _DB_Connection.Select(
                     DB_Table.APPLICATIONS,
                     new string[] { "id", "status" },
                     new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _DB_Helper.CurrentCampaignID) }
                     ).Where(s => statuses.Contains(s[1].ToString())).Select(s => (uint)s[0]);
        }

        private IEnumerable<Tuple<string, DateTime>> GetAdmExcOrders(uint applicationID, CB_P_Value profile)
        {
            return _DB_Connection.Select(
                DB_Table._ORDERS_HAS_APPLICATIONS,
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
                                    new Tuple<string, Relation, object>("education_form_id", Relation.EQUAL, CheckedEduForm),
                                    new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, profile.Item1),
                                    new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, profile.Item2),
                                    new Tuple<string, Relation, object>("profile_short_name", Relation.EQUAL, profile.Item3)
                                }),
                            k1 => k1[0],
                            k2 => k2[0],
                            (s1, s2) => new Tuple<string, DateTime>(s2[1].ToString(), (DateTime)s2[2])
                            );
        }

        private IEnumerable<Tuple<string, DateTime>> GetAdmExcOrders(uint applicationID, CB_B_Value direction)
        {
            return _DB_Connection.Select(
                         DB_Table._ORDERS_HAS_APPLICATIONS,
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
                                 new Tuple<string, Relation, object>("education_form_id", Relation.EQUAL, CheckedEduForm),
                                 new Tuple<string, Relation, object>("education_source_id",Relation.EQUAL,CheckedEduSource),
                                 new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, direction.Item1),
                                 new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, direction.Item2)
                             }),
                         k1 => k1[0],
                         k2 => k2[0],
                         (s1, s2) => new Tuple<string, DateTime>(s2[1].ToString(), (DateTime)s2[2])
                         );
        }
    }
}
