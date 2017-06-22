using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PK.Forms
{
    partial class FIS_Export : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private static readonly System.Xml.Schema.XmlSchemaSet _SchemaSet = new System.Xml.Schema.XmlSchemaSet();

        public FIS_Export(Classes.DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            cbAddress.Items.AddRange(Properties.Settings.Default.FIS_Addresses.Cast<string>().ToArray());
            cbAddress.SelectedIndex = 0;

            tbXSD_Path.Text = Properties.Settings.Default.FIS_XSD_Path;
            #endregion

            _DB_Connection = connection;
        }

        private void bOpenAddressPage_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(cbAddress.Text);
        }

        private void bOpenXSD_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbXSD_Path.Text = openFileDialog.FileName;
                Properties.Settings.Default.FIS_XSD_Path = openFileDialog.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void bExport_Click(object sender, EventArgs e)
        {
            if (!CheckCheckBoxes())
                return;

            Cursor.Current = Cursors.WaitCursor;

            XElement package = MakePackage();

            if (ValidateXML(package))
            {
                Classes.Utility.TryAccessFIS_Function((login, password) =>
                {
                    if (Classes.Utility.ShowUnrevertableActionMessageBox())
                        MessageBox.Show(
                            "Идентификатор пакета: " +
                            Classes.FIS_Connector.Export(cbAddress.Text, login, password, package),
                            "Пакет отправлен",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                            );
                }, new Classes.LoginSetting());
            }

            Cursor.Current = Cursors.Default;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            if (!CheckCheckBoxes())
                return;

            Cursor.Current = Cursors.WaitCursor;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                XElement package = MakePackage();

                if (ValidateXML(package))
                    package.Save(saveFileDialog.FileName);
            }

            Cursor.Current = Cursors.Default;
        }

        private bool CheckCheckBoxes()
        {
            if (!cbCampaignData.Checked && !cbApplications.Checked && !cbOrders.Checked)
            {
                MessageBox.Show("Не отмечена информация к выгрузке.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private XElement MakePackage()
        {
            return Classes.FIS_Packager.MakePackage(
                _DB_Connection,
                Classes.Settings.CurrentCampaignID,
                cbCampaignData.Checked,
                cbApplications.Checked,
                cbOrders.Checked
                ).ConvertToXElement();
        }

        private bool ValidateXML(XElement xml)
        {
            if (System.IO.File.Exists(tbXSD_Path.Text))
            {
                _SchemaSet.Add(null, tbXSD_Path.Text);
                XDocument doc = new XDocument(new XElement("Root",
                    new XElement("AuthData", new XElement("Login"), new XElement("Pass")),
                    xml
                    ));

                bool foundError = false;
                System.Xml.Schema.Extensions.Validate(doc, _SchemaSet, (sender, e) =>
                {
                    foundError = true;
                    MessageBox.Show(e.Message, "Ошибка в XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                });

                if (foundError && !Classes.Utility.ShowChoiceMessageBox("Продолжить выгрузку данных?", "Выбор"))
                    return false;
            }
            else
                MessageBox.Show("Валидация пропущена: не найден файл схемы.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return true;
        }
    }
}
