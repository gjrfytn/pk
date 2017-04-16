using System;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class DateChoice : Form
    {
        public DateChoice()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
