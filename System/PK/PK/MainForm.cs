using System;
using System.Windows.Forms;

namespace PK
{
    public partial class MainForm : Form
    {
        public MainForm(byte userRole)
        {
            InitializeComponent();
        }

        private void menuStrip_Campaign_Campaigns_Click(object sender, EventArgs e)
        {
            CampaignsForm form = new CampaignsForm();
            form.ShowDialog();
        }

        private void menuStrip_CreateApplication_Click(object sender, EventArgs e)
        {
            NewApplicForm form = new NewApplicForm();
            form.ShowDialog();
        }

        private void toolStrip_CreateApplication_Click(object sender, EventArgs e)
        {
            NewApplicForm form = new NewApplicForm();
            form.ShowDialog();
        }

        private void целевыеОрганизацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TargetOrganizationsForm form = new TargetOrganizationsForm();
            form.ShowDialog();
        }
    }
}
