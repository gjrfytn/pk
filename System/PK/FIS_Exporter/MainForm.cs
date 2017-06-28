using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace FIS_Exporter
{
    public partial class MainForm : Form
    {
        private class LoginSetting : PK.Forms.FIS_Authorization.ILoginSetting
        {
            public string Value
            {
                get { return Properties.Settings.Default.FIS_Login; }
                set { Properties.Settings.Default.FIS_Login = value; }
            }

            public void Save() => Properties.Settings.Default.Save();
        }

        public MainForm()
        {
            #region Components
            InitializeComponent();

            cbAddress.Items.AddRange(Properties.Settings.Default.FIS_Addresses.Cast<string>().ToArray());
            cbAddress.SelectedIndex = 0;
            #endregion
        }

        private void bOpenAddressPage_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(cbAddress.Text);
        }

        private void bExport_Click(object sender, EventArgs e)
        {
            if (textBox.Text == "")
            {
                MessageBox.Show("Пустой запрос.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            System.Xml.Linq.XElement package;
            try
            {
                package = System.Xml.Linq.XElement.Parse(textBox.Text);
            }
            catch (System.Xml.XmlException ex)
            {
                MessageBox.Show("Ошибка считывания XML:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (package.Name != "PackageData")
            {
                MessageBox.Show("Корневым элементом XML должен быть PackageData.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            PK.Classes.Utility.TryAccessFIS_Function((login, password) =>
            {
                if (PK.Classes.Utility.ShowUnrevertableActionMessageBox())
                    MessageBox.Show(
                        "Идентификатор пакета: " +
                        PK.Classes.FIS_Connector.Export(
                            cbAddress.Text,
                            login,
                            password,
                            package
                            ),
                        "Пакет отправлен",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                        );
            }, new LoginSetting());

            Cursor.Current = Cursors.Default;
        }

        private void bOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(openFileDialog.FileName))
                    textBox.Text = reader.ReadToEnd();
        }
        
        private void bLoadDictionary_Click(object sender, EventArgs e)
        {
            //if(saveFileDialog.ShowDialog()==DialogResult.OK)
            //{
            //    Cursor.Current = Cursors.WaitCursor;

            //    PK.Classes.Utility.TryAccessFIS_Function((login, password) =>
            //    {
            //        if (PK.Classes.Utility.ShowUnrevertableActionMessageBox())
            //            MessageBox.Show(
            //                "Идентификатор пакета: " +
            //                PK.Classes.FIS_Connector.load(
            //                    cbAddress.Text,
            //                    login,
            //                    password,
            //                    package
            //                    ),
            //                "Пакет отправлен",
            //                MessageBoxButtons.OK,
            //                MessageBoxIcon.Information
            //                );
            //    }, new LoginSetting());

            //    Cursor.Current = Cursors.Default;
            //}
        }
    }
}
