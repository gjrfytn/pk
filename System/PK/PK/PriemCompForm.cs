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
    public partial class PriemCompForm : Form
    {
        public PriemCompForm()
        {
            InitializeComponent();
        }

        private void btCreatePriemComp_Click(object sender, EventArgs e)
        {
            NewPriemCompForm NPCForm = new NewPriemCompForm();
            NPCForm.Show();
        }
    }
}
