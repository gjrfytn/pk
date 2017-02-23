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

        private void menuStrip_Dictionaries_Click(object sender, EventArgs e)
        {
            DictionariesForm form = new DictionariesForm(_DB_Connection);
            form.ShowDialog();
        }
    }
}
