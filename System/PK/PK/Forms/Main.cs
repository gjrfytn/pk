using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class Main : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        string _UsersLogin;

        public Main(byte userRole, string usersLogin)
        {
            InitializeComponent();

            _UsersLogin = usersLogin;
            _DB_Connection = new Classes.DB_Connector();
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            foreach (var application in _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "id", "number", "entrant_id" }))
            {
                object[] names = _DB_Connection.Select(DB_Table.ENTRANTS, new string[] { "last_name", "first_name", "middle_name" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)application[2])
                    })[0];
                dgvApplications.Rows.Add(application[0], application[1], names[0], names[1], names[2]);
            }

        }

        private void menuStrip_Campaign_Campaigns_Click(object sender, EventArgs e)
        {
            Campaigns form = new Campaigns();
            form.ShowDialog();
        }

        private void menuStrip_CreateApplication_Click(object sender, EventArgs e)
        {
            ApplicationEdit form = new ApplicationEdit(6, _UsersLogin, null);
            form.ShowDialog();
        }

        private void toolStrip_CreateApplication_Click(object sender, EventArgs e)
        {
            ApplicationEdit form = new ApplicationEdit(6, _UsersLogin, null);
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

        private void menuStrip_InstitutionAchievements_Click(object sender, EventArgs e)
        {
            InstitutionAchievementsEdit form = new InstitutionAchievementsEdit();
            form.ShowDialog();
        }

        private void menuStrip_Orders_Click(object sender, EventArgs e)
        {
            Orders form = new Orders(_DB_Connection);
            form.ShowDialog();
        }

        private void dgvApplications_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ApplicationEdit form = new ApplicationEdit(6, _UsersLogin, (uint)dgvApplications.SelectedRows[0].Cells[0].Value);
            form.ShowDialog();
        }
    }
}
