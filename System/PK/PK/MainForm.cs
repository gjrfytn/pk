using System;
using System.Windows.Forms;

namespace PK
{
    public partial class MainForm : Form
    {
        DB_Connector _DB_Connection;

        public MainForm(byte userRole)
        {
            InitializeComponent();

            _DB_Connection = new DB_Connector();
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
        private void menuStrip_Dictionaries_Click(object sender, EventArgs e)
        {
            DictionariesForm form = new DictionariesForm(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_DirDictionary_Click(object sender, EventArgs e)
        {
            DirectionsDictionaryForm form = new DirectionsDictionaryForm(_DB_Connection);
            form.ShowDialog();
        }

        private void факультетыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FacultiesForm form = new FacultiesForm();
            form.ShowDialog();
        }

        private void направленияПодготовкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectionsProfilesForm form = new DirectionsProfilesForm();
            form.ShowDialog();
        }
    }
}
