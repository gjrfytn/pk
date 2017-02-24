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
        public NewCampaignForm()
        {
            InitializeComponent();
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

        private void NewCampaignForm_Load(object sender, EventArgs e)
        {
            cbState.SelectedIndex = 0;
            cbType.SelectedIndex = 0;

            int currYear = DateTime.Now.Year;
            for (int i=-2; i<=1;i++)
            {
                cbStartYear.Items.Add((currYear + i).ToString());
                cbEndYear.Items.Add((currYear + i).ToString());
            }
            cbStartYear.SelectedIndex = 2;
            cbEndYear.SelectedIndex = 2;
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

        }
    }
}
