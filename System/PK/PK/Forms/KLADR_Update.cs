using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PK.Forms
{
    public partial class KLADR_Update : Form
    {
        private readonly string _User;
        private readonly string _Password;

        public KLADR_Update(string user, string password)
        {
            InitializeComponent();

            _User = user;
            _Password = password;
        }

        private void bSubjects_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Открыть файл с субъектами";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                tbSubjects.Text = openFileDialog.FileName;
        }

        private void bStreets_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Открыть файл с улицами";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                tbStreets.Text = openFileDialog.FileName;
        }

        private void bHouses_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Открыть файл с домами";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                tbHouses.Text = openFileDialog.FileName;
        }

        private void bUpdate_Click(object sender, EventArgs e)
        {
            if (tbSubjects.Text == "" || tbStreets.Text == "" || tbHouses.Text == "")
            {
                MessageBox.Show("Не выбран один из файлов.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Classes.Utility.ShowChoiceMessageWithConfirmation("Рекомендуется создать резервную копию БД КЛАДР. Продолжить?", "Внимание"))
                return;

            MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.kladr_CS + " user = " + _User + "; password = " + _Password + ";");
            connection.Open();

            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                MySqlCommand cmd = new MySqlCommand("", connection, transaction);
                try
                {
                    Cursor = Cursors.WaitCursor;
                    statusStrip_Label.Text = "Очистка БД...";

                    cmd.CommandText = "DELETE FROM subjects;";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM streets;";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM houses;";
                    cmd.ExecuteNonQuery();

                    uint total = 0;
                    statusStrip_ProgressBar.Visible = true;
                    statusStrip_Label.Text = "Загрузка субъектов...";
                    uint count = 0;
                    using (Classes.DBF_Reader reader = new Classes.DBF_Reader(tbSubjects.Text))
                    {
                        statusStrip_ProgressBar.Value = 0;
                        statusStrip_ProgressBar.Maximum = (int)reader.RowCount;

                        while (reader.ReadRow())
                        {
                            cmd.CommandText = "INSERT INTO subjects (name, socr, code, `index`) VALUES ('" +
                                reader.Value("NAME").ToString().Trim() + "', '" +
                                reader.Value("SOCR").ToString().Trim() + "', '" +
                                reader.Value("CODE").ToString() + "', " +
                                (reader.Value("INDEX").ToString() != "" ? ("'" + reader.Value("INDEX").ToString() + "'") : "NULL") + ");";
                            cmd.ExecuteNonQuery();

                            count++;
                            statusStrip_ProgressBar.Value = (int)count;
                            statusStrip_ProgressLabel.Text = count.ToString() + "/" + reader.RowCount;
                            statusStrip.Update();
                        }
                    }
                    total += count;

                    statusStrip_Label.Text = "Загрузка улиц...";
                    count = 0;
                    using (Classes.DBF_Reader reader = new Classes.DBF_Reader(tbStreets.Text))
                    {
                        statusStrip_ProgressBar.Value = 0;
                        statusStrip_ProgressBar.Maximum = (int)reader.RowCount;

                        while (reader.ReadRow())
                        {
                            cmd.CommandText = "INSERT INTO streets (name, socr, code, `index`) VALUES ('" +
                                reader.Value("NAME").ToString().Trim() + "', '" +
                                reader.Value("SOCR").ToString().Trim() + "', '" +
                                reader.Value("CODE").ToString() + "', " +
                                (reader.Value("INDEX").ToString() != "" ? ("'" + reader.Value("INDEX").ToString() + "'") : "NULL") + ");";
                            cmd.ExecuteNonQuery();

                            count++;
                            statusStrip_ProgressBar.Value = (int)count;
                            statusStrip_ProgressLabel.Text = count.ToString() + "/" + reader.RowCount;
                            statusStrip.Update();
                        }
                    }
                    total += count;

                    statusStrip_Label.Text = "Загрузка домов...";
                    count = 0;
                    using (Classes.DBF_Reader reader = new Classes.DBF_Reader(tbHouses.Text))
                    {
                        statusStrip_ProgressBar.Value = 0;
                        statusStrip_ProgressBar.Maximum = (int)reader.RowCount;

                        while (reader.ReadRow())
                        {
                            cmd.CommandText = "INSERT INTO houses (name, code, `index`) VALUES ('" +
                                reader.Value("NAME").ToString().Trim() + "', '" +
                                reader.Value("CODE").ToString() + "', " +
                                (reader.Value("INDEX").ToString() != "" ? ("'" + reader.Value("INDEX").ToString() + "'") : "NULL") + ");";
                            cmd.ExecuteNonQuery();

                            count++;
                            statusStrip_ProgressBar.Value = (int)count;
                            statusStrip_ProgressLabel.Text = count.ToString() + "/" + reader.RowCount;
                            statusStrip.Update();
                        }
                    }
                    total += count;

                    transaction.Commit();

                    MessageBox.Show("Всего записей: " + total, "Обновление завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                    statusStrip_Label.Text = "Ожидание выбора...";
                    statusStrip_ProgressBar.Visible = false;
                    statusStrip_ProgressLabel.Text = "";
                }
            }
        }
    }
}
