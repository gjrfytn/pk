using System.Windows.Forms;

namespace PK.Forms
{
    public partial class ApplicationDocsPrint : Form
    {
        public ApplicationDocsPrint()
        {
            InitializeComponent();
        }

        private void bPrint_Click(object sender, System.EventArgs e) => DialogResult = DialogResult.Yes;

        private void bOpen_Click(object sender, System.EventArgs e) => DialogResult = DialogResult.No;
    }
}
