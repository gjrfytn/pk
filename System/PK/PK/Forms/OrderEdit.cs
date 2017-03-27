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
        }

        private void cbDirOrProfile_DropDown(object sender, EventArgs e)
        {
            Classes.DB_Helper dbHelper = new Classes.DB_Helper(_DB_Connection);

            cbDirOrProfile.Items.Clear();
            if (!rbPaid.Checked)
                cbDirOrProfile.Items.AddRange(_DB_Connection.Select(
                    DB_Table.CAMPAIGNS_DIRECTIONS_DATA,
                    new string[] { "direction_faculty", "direction_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("places_" +gbEduSource.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag.ToString()+
                        "_"+gbEduForm.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag.ToString(),
                        Relation.GREATER,0 )
                    }
                    ).Select(s => s[0].ToString() + " " + s[1].ToString() + " " + dbHelper.GetDirectionsDictionaryNameAndCode((uint)s[1])[0]).ToArray());
            else
                cbDirOrProfile.Items.AddRange(_DB_Connection.Select(
                    DB_Table.CAMPAIGNS_PROFILES_DATA,
                    new string[] { "profiles_direction_faculty", "profiles_direction_id", "profiles_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("places_paid_"+gbEduForm.Controls.Cast<Control>().Single(c=>((RadioButton)c).Checked).Tag.ToString(),
                        Relation.GREATER,0 )
                    }
                    ).Select(s => s[0].ToString() + " " + s[1].ToString() + " " + s[2].ToString()).ToArray());
        }

        private void rbPaid_CheckedChanged(object sender, EventArgs e)
        {
            lDirOrProfile.Text = rbPaid.Checked ? "Профиль" : "Направление";
            cbShowAdmitted.Enabled = rbPaid.Checked;
        }

        private void cbDirOrProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();

            uint eduSource;
            if (rbBudget.Checked)
                eduSource = 14;
            else if (rbPaid.Checked)
                throw new NotImplementedException();
            else if (rbPaid.Checked)
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

            string[] buf = cbDirOrProfile.SelectedItem.ToString().Split(' ');

            var temp = _DB_Connection.Select(
                DB_Table.APPLICATIONS_ENTRANCES,
                new string[] { "application_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("faculty_short_name",Relation.EQUAL,buf[0]),
                    new Tuple<string, Relation, object>("direction_id",Relation.EQUAL,buf[1]),
                    new Tuple<string, Relation, object>("edu_form_id",Relation.EQUAL,eduForm),
                    new Tuple<string, Relation, object>("edu_source_id",Relation.EQUAL,eduSource)
                }
                );

            var temp2 = _DB_Connection.Select(DB_Table.APPLICATIONS, "id", "entrant_id");

            var temp3 = temp.Join(temp2, k1 => k1[0], k2 => k2[0], (s1, s2) => s2);

            foreach (var ent in temp3.Join(
                _DB_Connection.Select(DB_Table.ENTRANTS, "id", "last_name", "first_name", "middle_name"),
                k1 => k1[1], k2 => k2[0], (s1, s2) => s2
                ))
                dataGridView.Rows.Add(false, ent[0], ent[1].ToString() + " " + ent[2].ToString() + " " + ent[3].ToString());
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
