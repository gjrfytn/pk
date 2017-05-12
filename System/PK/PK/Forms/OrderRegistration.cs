using System;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class OrderRegistration : Form
    {
        public OrderRegistration()
        {
            InitializeComponent();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void tbNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }
    }
}
