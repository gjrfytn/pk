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

            cbAddress.Items.AddRange(Properties.Settings.Default.FIS_Addresses.Cast<string>().ToArray());
            cbAddress.SelectedIndex = 0;
            #endregion

            _DB_Connection = connection;
        }

        private void bOpenAddressPage_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(cbAddress.Text);
        }

        private void bExport_Click(object sender, EventArgs e)
        {
            if (!cbCampaignData.Checked && !cbApplications.Checked && !cbOrders.Checked)
            {
                MessageBox.Show("Не отмечена информация к выгрузке.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            Classes.Utility.TryAccessFIS_Function((login, password) =>
            {
                if (Classes.Utility.ShowUnrevertableActionMessageBox())
                    MessageBox.Show(
                        "Идентификатор пакета: " +
                        Classes.FIS_Connector.Export(
                            cbAddress.Text,
                            login,
                            password,
                            Classes.FIS_Packager.MakePackage(_DB_Connection, Classes.Utility.CurrentCampaignID, cbCampaignData.Checked, cbApplications.Checked, cbOrders.Checked)
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
