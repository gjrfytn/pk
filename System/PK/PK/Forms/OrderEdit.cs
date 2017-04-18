using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class OrderEdit : Form
    {
        readonly Classes.DB_Connector _DB_Connection;

        public OrderEdit(Classes.DB_Connector connection, uint? id)
        {
            InitializeComponent();

            _DB_Connection = connection;

            cbDirOrProfile.ValueMember = "Value";
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                cbDirOrProfile.DataSource = null;
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

            lDirOrProfile.Text = rbPaid.Checked ? "Профиль" : "Направление";
            cbShowAdmitted.Enabled = rbPaid.Checked;
        }

        private void cbDirOrProfile_DropDown(object sender, EventArgs e)
        {
            Classes.DB_Helper dbHelper = new Classes.DB_Helper(_DB_Connection);
            int selectedIndex = cbDirOrProfile.SelectedIndex;
            cbDirOrProfile.DisplayMember = "Display";

            if (rbPaid.Checked)
                cbDirOrProfile.DataSource = _DB_Connection.Select(
                    DB_Table.CAMPAIGNS_PROFILES_DATA,
                    new string[] { "profiles_direction_faculty", "profiles_direction_id", "profiles_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_id",Relation.EQUAL,dbHelper.CurrentCampaignID),
                        new Tuple<string, Relation, object>("places_paid_"+gbEduForm.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag.ToString(),
                        Relation.GREATER,0 )
                    }
                    ).Select(s => new { Value = new Tuple<string, uint, string>(s[0].ToString(), (uint)s[1], s[2].ToString()), Display = s[0].ToString() + " " + s[2].ToString() }).ToList();
            else if (rbTarget.Checked)
                cbDirOrProfile.DataSource = _DB_Connection.Select(
                    DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA,
                    new string[] { "direction_faculty", "direction_id", "places_" + gbEduForm.Controls.Cast<Control>().Single(c => ((RadioButton)c).Checked).Tag.ToString() },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,dbHelper.CurrentCampaignID)
                    }
                    ).GroupBy(k => new Tuple<object, object>(k[0], k[1]), (k, g) => new { Faculty = k.Item1.ToString(), DirID = (uint)k.Item2, Places = g.Sum(s => (ushort)s[2]) })
                    .Where(s => s.Places > 0)
                    .Select(s => new { Value = new Tuple<string, uint>(s.Faculty, s.DirID), Display = s.Faculty + " " + dbHelper.GetDirectionNameAndCode(s.DirID).Item1 }).ToList();
            else
                cbDirOrProfile.DataSource = _DB_Connection.Select(
                        DB_Table.CAMPAIGNS_DIRECTIONS_DATA,
                        new string[] { "direction_faculty", "direction_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                        new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,dbHelper.CurrentCampaignID),
                        new Tuple<string, Relation, object>("places_" +gbEduSource.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag.ToString()+
                        "_"+gbEduForm.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag.ToString(),
                        Relation.GREATER,0 )
                        }
                        ).Select(s => new { Value = new Tuple<string, uint>(s[0].ToString(), (uint)s[1]), Display = s[0].ToString() + " " + dbHelper.GetDirectionNameAndCode((uint)s[1]).Item1 }).ToList();

            cbDirOrProfile.SelectedIndex = selectedIndex;
        }

        private void cbDirOrProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TODO Проверка кампании

            dataGridView.Rows.Clear();

            if (cbDirOrProfile.SelectedValue == null)
                return;

            uint eduSource;
            if (rbBudget.Checked)
                eduSource = 14;
            else if (rbPaid.Checked)
                eduSource = 15;
            else if (rbTarget.Checked)
                eduSource = 16;
            else
                eduSource = 20;

            uint eduForm;
            if (rbO.Checked)
                eduForm = 11;
            else if (rbOZ.Checked)
                eduForm = 12;
            else
                eduForm = 10;

            List<object[]> applicationsIDs;
            if (eduSource != 15)
            {
                Tuple<string, uint> buf = (Tuple<string, uint>)cbDirOrProfile.SelectedValue;
                applicationsIDs = _DB_Connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "application_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,buf.Item1),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,buf.Item2),
                        new Tuple<string, Relation, object>("edu_form_id",Relation.EQUAL,eduForm),
                        new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,eduSource)
                    });
            }
            else
            {
                Tuple<string, uint, string> buf = (Tuple<string, uint, string>)cbDirOrProfile.SelectedValue;
                applicationsIDs = _DB_Connection.Select(
                    DB_Table.APPLICATIONS_ENTRANCES,
                    new string[] { "application_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,buf.Item1),
                        new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,buf.Item2),
                        new Tuple<string, Relation, object>("edu_form_id",Relation.EQUAL,eduForm),
                        new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,eduSource),
                        new Tuple<string, Relation, object>("profile_name",Relation.EQUAL,buf.Item3)
                    });
            }

            var egeMarks = applicationsIDs.Join(
                _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, "applications_id", "documents_id"),//TODO Не обязательно.
                k1 => k1[0], k2 => k2[0], (s1, s2) => s2).Join(
                _DB_Connection.Select(
                    DB_Table.DOCUMENTS,
                    new string[] { "id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("type",Relation.EQUAL,"ege")
                    }),
                k1 => k1[1], k2 => k2[0], (s1, s2) => s1).Join(
                _DB_Connection.Select(DB_Table.DOCUMENTS_SUBJECTS_DATA, "document_id", "subject_id", "value"),
                k1 => k1[1], k2 => k2[0], (s1, s2) => new { ApplID = s1[0], Subject = s2[1], Value = s2[2] }
                );

            var entrants = applicationsIDs.Join(
                _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id"),
                k1 => k1[0], k2 => k2[0], (s1, s2) => new { A_ID = s2[0], E_ID = s2[1] }
                ).Join(
                _DB_Connection.Select(DB_Table.ENTRANTS, "id", "last_name", "first_name", "middle_name"),
                k1 => k1.E_ID,
                k2 => k2[0],
                (s1, s2) => new { ApplID = s1.A_ID, EntrID = s2[0], Name = s2[1].ToString() + " " + s2[2].ToString() + " " + s2[3].ToString() }
                );

            var table = entrants.GroupJoin(
                egeMarks,
                k1 => k1.ApplID,
                k2 => k2.ApplID,
                (s1, s2) => new
                {
                    EntrID = s1.EntrID,
                    Name = s1.Name,
                    Math = s2.FirstOrDefault(s => (uint)s.Subject == 2)?.Value, //TODO
                    Phys = s2.FirstOrDefault(s => (uint)s.Subject == 10)?.Value, //TODO
                    Rus = s2.FirstOrDefault(s => (uint)s.Subject == 1)?.Value, //TODO
                    Soc = s2.FirstOrDefault(s => (uint)s.Subject == 9)?.Value, //TODO
                    For = s2.FirstOrDefault(s => (uint)s.Subject == 6)?.Value //TODO
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
                    ent.EntrID,
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
                    gbEduSource.Enabled = false;
                    gbEduForm.Enabled = false;
                    cbDirOrProfile.Enabled = false;
                }
                else
                {
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if ((bool)row.Cells[e.ColumnIndex]
                            .GetEditedFormattedValue(row.Index, DataGridViewDataErrorContexts.CurrentCellChange)
                            )
                            return;

                    gbEduSource.Enabled = true;
                    gbEduForm.Enabled = true;
                    cbDirOrProfile.Enabled = true;
                }
            }
        }
    }
}
