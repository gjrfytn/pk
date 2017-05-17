using System;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class FIS_Export : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;

        public FIS_Export(Classes.DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            cbAdress.Items.AddRange(Properties.Settings.Default.FIS_Adresses.Cast<string>().ToArray());
            cbAdress.SelectedIndex = 0;
            #endregion

            _DB_Connection = connection;
        }

        private void bOpenAdressPage_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(cbAdress.Text);
        }

        private void bExport_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            Classes.Utility.TryAccessFIS_Function((login, password) =>
            {
                if (Classes.Utility.ShowUnrevertableActionMessageBox())
                    MessageBox.Show(
                        "Идентификатор пакета: " +
                        Classes.FIS_Connector.Export(
                            cbAdress.Text,
                            login,
                            password,
                            Classes.FIS_Packager.MakePackage(_DB_Connection, cbCampaignData.Checked, cbApplications.Checked, cbOrders.Checked)
                            ),
                        "Пакет отправлен",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                        );
            });

            Cursor.Current = Cursors.Default;
        }
    }
}
