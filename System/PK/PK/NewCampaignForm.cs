using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PK
{
    public partial class NewCampaignForm : Form
    {
        DB_Connector _DB_Connection;

        public NewCampaignForm()
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();

            cbState.SelectedIndex = 0;
            cbType.SelectedIndex = 0;

            int currYear = DateTime.Now.Year;
            for (int i = -2; i <= 1; i++)
            {
                cbStartYear.Items.Add((currYear + i).ToString());
                cbEndYear.Items.Add((currYear + i).ToString());
            }
            cbStartYear.SelectedIndex = 2;
            cbEndYear.SelectedIndex = 2;

            ButtonsAppearanceChange(0);
            FillDirectionsTable();
        }

        private void ButtonsAppearanceChange(int rowindex)
        {

            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.Rows.Count - 1].Cells[1].Style.Font = new Font("ESSTIXTwo", 11);
            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.Rows.Count - 1].Cells[1].Value = "=";

            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.Rows.Count - 1].Cells[4].Style.Font = new Font("ESSTIXTwo", 11);
            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.Rows.Count - 1].Cells[4].Value = "=";

        }

        private void FillDirectionsTable()
        {
        }

        void FillProfiliesTable ()
        {
            //List<string> directions = new List<string>();
            //foreach (var v in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "ugs_code", "ugs_name", "code", "name"))
            //{
            //    if (!(directions.Contains(v[0].ToString())))
            //    {
            //        directions.Add(v[0].ToString());
            //        dgvDirections.Rows.Add(true, "Н", v[1], v[0], 0, 0, 0, 0, 0, 0, 0, 0);
            //        dgvDirections.Rows[dgvDirections.Rows.Count - 1].ReadOnly = true;
            //        dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells[0].ReadOnly = false;

            //        for (int i = 0; i < dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells.Count; i++)
            //        {
            //            dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells[i].Style.Font = new Font(
            //                dgvDirections.Font.Name.ToString(),
            //                dgvDirections.Font.Size + 1,
            //                FontStyle.Bold);
            //            dgvDirections.Rows[dgvDirections.Rows.Count - 1].Cells[i].Style.BackColor = Color.LightGray;
            //        }
            //    }
            //    dgvDirections.Rows.Add(true, "П", v[3], v[2], 0, 0, 0, 0, 0, 0, 0, 0);
            //}
            //dgvDirections.Sort(dgvDirections.Columns[3], ListSortDirection.Ascending);

        }

        private void UpdateDirectionTable()
        {
            //foreach (DataGridViewRow v in dgvDirections.Rows)
            //{
            //    if (v.Cells[1].Value.ToString() == "Н")
            //    {
            //        for (int i = 4; i <= 14; i++)
            //            v.Cells[i].Value = 0;

            //        foreach (DataGridViewRow r in dgvDirections.Rows)
            //            if (v.Cells[3].Value.ToString().Substring(0, 2) == r.Cells[3].Value.ToString().Substring(0, 2))
            //                for (int i = 4; i <= 14; i++)
            //                    v.Cells[i].Value = (Convert.ToInt32(v.Cells[i].Value.ToString()) + Convert.ToInt32(r.Cells[i].Value.ToString())).ToString();
            //    }
            //}
        }

        private void btAddEduLevel_Click(object sender, EventArgs e)
        {
            if (lbEduFormsAll.SelectedIndex != -1)
            {
                lbEduFormsChoosen.Items.Add(lbEduFormsAll.SelectedItem.ToString());
                lbEduFormsAll.Items.Remove(lbEduFormsAll.SelectedItem.ToString());
            }
        }

        private void btRemoveEduLevel_Click(object sender, EventArgs e)
        {
            if (lbEduFormsChoosen.SelectedIndex != -1)
            {
                lbEduFormsAll.Items.Add(lbEduFormsChoosen.SelectedItem.ToString());
                lbEduFormsChoosen.Items.Remove(lbEduFormsChoosen.SelectedItem.ToString());
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (CheckBox v in gbEduLevel.Controls)
                v.Checked = false;

            if (cbType.SelectedItem.ToString() == "Прием иностранце по направлениям Минобрнауки")
                foreach (CheckBox v in gbEduLevel.Controls)
                    v.Enabled = true;
            else
                foreach (CheckBox v in gbEduLevel.Controls)
                    v.Enabled = false;

            switch (cbType.SelectedItem.ToString())
            {
                case "Прием на обучение на бакалавриат/специалитет":
                    cbEduLevelBacc.Checked = true;
                    cbEduLevelSpec.Checked = true;
                    break;
                case "Прием на обучение в магистратуру":
                    cbEduLevelMag.Checked = true;
                    break;
                case "Прием на обучение в СПО":
                    cbEduLevelSPO.Checked = true;
                    break;
                case "Прием на подготовку кадров высшей квалификации":
                    cbEduLevelQual.Checked = true;
                    break;
            }
        }

        private void dgvTargetOrganizatons_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ButtonsAppearanceChange(dgvTargetOrganizatons.Rows.Count);
        }

        private void dgvTargetOrganizatons_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
                switch (dgvTargetOrganizatons.CurrentCell.ColumnIndex)
            {
                case 1:
                    GridItemSelection form1 = new GridItemSelection();
                    form1.ShowDialog();
                    dgvTargetOrganizatons.CurrentRow.Cells[0].Value = form1.organizationName;
                    break;
                case 4:
                    DirectionSelect form2 = new DirectionSelect();
                    form2.ShowDialog();
                    dgvTargetOrganizatons.CurrentRow.Cells[2].Value = form2.DirectionName;
                    dgvTargetOrganizatons.CurrentRow.Cells[3].Value = form2.DirectionCode;
                    break;
            }
            

        }

        private void dgvDirections_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateDirectionTable();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
