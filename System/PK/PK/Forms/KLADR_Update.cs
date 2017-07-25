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

            if (!SharedClasses.Utility.ShowChoiceMessageWithConfirmation("Рекомендуется создать резервную копию БД КЛАДР. Продолжить?", "Внимание"))
                return;

            foreach (Button b in System.Linq.Enumerable.OfType<Button>(Controls))
                b.Enabled = false;

            statusStrip_ProgressBar.Visible = true;
            statusStrip_Label.Text = "Очистка БД...";

            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.kladr_CS + " user = " + _User + "; password = " + _Password + ";");
            connection.Open();

            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                MySqlCommand cmd = new MySqlCommand("", connection, transaction);

                cmd.CommandText = "DELETE FROM subjects;";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM streets;";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM houses;";
                cmd.ExecuteNonQuery();

                uint total = 0;
                total += LoadFileToTable(cmd, tbSubjects.Text, "subjects", true, "Загрузка субъектов...");
                total += LoadFileToTable(cmd, tbStreets.Text, "streets", true, "Загрузка улиц...");
                total += LoadFileToTable(cmd, tbHouses.Text, "houses", false, "Загрузка домов...");

                transaction.Commit();

                e.Result = total;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                Tuple<int, string> buf = (Tuple<int, string>)e.UserState;
                statusStrip_ProgressBar.Maximum = buf.Item1;
                statusStrip_Label.Text = buf.Item2;
            }

            statusStrip_ProgressBar.Value = e.ProgressPercentage;
            statusStrip_ProgressLabel.Text = e.ProgressPercentage + "/" + statusStrip_ProgressBar.Maximum;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show("Произошла ошибка:\n" + e.Error.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Всего записей: " + e.Result.ToString(), "Обновление завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);

            foreach (Button b in System.Linq.Enumerable.OfType<Button>(Controls))
                b.Enabled = true;

            statusStrip_Label.Text = "Ожидание выбора...";
            statusStrip_ProgressBar.Visible = false;
            statusStrip_ProgressLabel.Text = "";
        }

        private void KLADR_Update_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                MessageBox.Show("Невозможно закрыть форму во время обновления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private uint LoadFileToTable(MySqlCommand cmd, string dbfFile, string table, bool hasSocr, string message)
        {
            uint count = 0;
            using (Classes.DBF_Reader reader = new Classes.DBF_Reader(dbfFile))
            {
                backgroundWorker.ReportProgress(0, new Tuple<int, string>((int)reader.RowCount, message));

                while (reader.ReadRow())
                {
                    cmd.CommandText = "INSERT INTO " + table + " (name, " + (hasSocr ? "socr, " : "") + "code, `index`) VALUES ('" +
                        reader.Value("NAME").ToString().Trim() + "', '" +
                        (hasSocr ? reader.Value("SOCR").ToString().Trim() + "', '" : "") +
                        reader.Value("CODE").ToString() + "', " +
                        (reader.Value("INDEX").ToString() != "" ? ("'" + reader.Value("INDEX").ToString() + "'") : "NULL") + ");";
                    cmd.ExecuteNonQuery();

                    count++;

                    backgroundWorker.ReportProgress((int)count);
                }
            }

            return count;
        }
    }
}
