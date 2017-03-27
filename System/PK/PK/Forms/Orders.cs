using System;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class Orders : Form
    {
        readonly Classes.DB_Connector _DB_Connection;

        public Orders(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
        }

        private void toolStrip_New_Click(object sender, EventArgs e)
        {
            OrderEdit form = new OrderEdit(_DB_Connection, null);

            form.ShowDialog();
        }
    }
}
