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
    public partial class CampaignsForm : Form
    {
        public CampaignsForm()
        {
            InitializeComponent();
        }

        private void btCreatePriemComp_Click(object sender, EventArgs e)
        {
            NewCampaignForm NPCForm = new NewCampaignForm();
            NPCForm.Show();
        }
    }
}
