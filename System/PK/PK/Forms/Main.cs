using System;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class Main : Form
    {
        Classes.DB_Connector _DB_Connection;

        public Main(byte userRole)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
        }

        private void menuStrip_Campaign_Campaigns_Click(object sender, EventArgs e)
        {
            Campaigns form = new Campaigns();
            form.ShowDialog();
        }

        private void menuStrip_CreateApplication_Click(object sender, EventArgs e)
        {
            ApplicationEdit form = new ApplicationEdit();
            form.ShowDialog();
        }

        private void toolStrip_CreateApplication_Click(object sender, EventArgs e)
        {
            ApplicationEdit form = new ApplicationEdit();
            form.ShowDialog();
        }
        private void menuStrip_TargetOrganizations_Click(object sender, EventArgs e)
        {
            TargetOrganizations form = new TargetOrganizations();
            form.ShowDialog();
        }
        private void menuStrip_Dictionaries_Click(object sender, EventArgs e)
        {
            Dictionaries form = new Dictionaries(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_DirDictionary_Click(object sender, EventArgs e)
        {
            DirectionsDictionary form = new DirectionsDictionary(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_OlympDictionary_Click(object sender, EventArgs e)
        {
            OlympicsDictionaryForm form = new OlympicsDictionaryForm(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_Faculties_Click(object sender, EventArgs e)
        {
            Faculties form = new Faculties();
            form.ShowDialog();
        }

        private void menuStrip_Directions_Click(object sender, EventArgs e)
        {
            DirectionsProfiles form = new DirectionsProfiles();
            form.ShowDialog();
        }

        private void menuStrip_Campaign_Exams_Click(object sender, EventArgs e)
        {
            Examinations form = new Examinations(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_Users_Click(object sender, EventArgs e)
        {
            Users form = new Users(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_FisImport_Click(object sender, EventArgs e)
        {
            Classes.FIS_Connector fisConnector = new Classes.FIS_Connector("XXX", "***");
            fisConnector.Import(Classes.FIS_Packager.MakePackage(_DB_Connection));
        }
    }
}
