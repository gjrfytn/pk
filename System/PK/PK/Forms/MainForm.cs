using System;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class MainForm : Form
    {
        Classes.DB_Connector _DB_Connection;

        public MainForm(byte userRole)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
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
        private void menuStrip_TargetOrganizations_Click(object sender, EventArgs e)
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

        private void menuStrip_OlympDictionary_Click(object sender, EventArgs e)
        {
            OlympicsDictionaryForm form = new OlympicsDictionaryForm(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_Faculties_Click(object sender, EventArgs e)
        {
            FacultiesForm form = new FacultiesForm();
            form.ShowDialog();
        }

        private void menuStrip_Directions_Click(object sender, EventArgs e)
        {
            DirectionsProfilesForm form = new DirectionsProfilesForm();
            form.ShowDialog();
        }

        private void menuStrip_Campaign_Exams_Click(object sender, EventArgs e)
        {
            ExaminationsForm form = new ExaminationsForm(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_Users_Click(object sender, EventArgs e)
        {
            UsersForm form = new UsersForm(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_FisImport_Click(object sender, EventArgs e)
        {
            Classes.FIS_Connector fisConnector = new Classes.FIS_Connector("XXX", "***");
            fisConnector.Import(Classes.FIS_Packager.MakePackage(_DB_Connection));
        }
    }
}
