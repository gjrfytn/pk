using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK.Forms
{
    partial class ExaminationEdit : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly uint? _ID;

        public ExaminationEdit(Classes.DB_Connector connection, uint? id)
        {
            _DB_Connection = connection;
            _ID = id;

            #region Components
            InitializeComponent();

            dataGridView_Capacity.ValueType = typeof(ushort);

            Tuple<uint, uint> curCampStartEnd = Classes.DB_Queries.GetCampaignStartEnd(_DB_Connection, Classes.Settings.CurrentCampaignID);

            dtpDate.MinDate = new DateTime((int)curCampStartEnd.Item1, 1, 1);
            dtpDate.MaxDate = new DateTime((int)curCampStartEnd.Item2, 12, 31);
            dtpRegStartDate.MinDate = dtpDate.MinDate;
            dtpRegStartDate.MaxDate = dtpDate.MaxDate;
            dtpRegEndDate.MinDate = dtpDate.MinDate;
            dtpRegEndDate.MaxDate = dtpDate.MaxDate;

            foreach (DateTimePicker dtp in Controls.OfType<DateTimePicker>())
                dtp.Tag = _ID.HasValue;
            #endregion

            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            Dictionary<uint, string> subjects = _DB_Helper.GetDictionaryItems(FIS_Dictionary.SUBJECTS);

            cbSubject.Items.AddRange(subjects.Values.ToArray());

            if (_ID.HasValue)
            {
                object[] exam = _DB_Connection.Select(DB_Table.EXAMINATIONS,
                    new string[] { "subject_id", "date", "reg_start_date", "reg_end_date" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id",Relation.EQUAL,_ID)
                    })[0];

                cbSubject.SelectedItem = subjects[(uint)exam[0]];
                dtpDate.Value = (DateTime)exam[1];
                dtpRegStartDate.Value = (DateTime)exam[2];
                dtpRegEndDate.Value = (DateTime)exam[3];

                foreach (object[] row in _DB_Connection.Select(
                    DB_Table.EXAMINATIONS_AUDIENCES,
                    new string[] { "number", "capacity", "priority" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,_ID)
                    }).OrderBy(s => s[2]))
                    dataGridView.Rows.Add(row[0], row[1]);
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            if (!AssureSave())
                return;

            Cursor.Current = Cursors.WaitCursor;

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "subject_dict_id",(uint)FIS_Dictionary.SUBJECTS},
                { "subject_id",_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS,cbSubject.Text)},
                { "date",dtpDate.Value},
                { "reg_start_date",dtpRegStartDate.Value},
                { "reg_end_date",dtpRegEndDate.Value}
            };

            using (MySql.Data.MySqlClient.MySqlTransaction transaction = _DB_Connection.BeginTransaction())
            {
                uint id;
                if (_ID.HasValue)
                {
                    _DB_Connection.Update(DB_Table.EXAMINATIONS, data, new Dictionary<string, object> { { "id", _ID } }, transaction);
                    id = _ID.Value;
                }
                else
                    id = _DB_Connection.Insert(DB_Table.EXAMINATIONS, data, transaction);

                string[] fields = { "examination_id", "number", "capacity", "priority" };
                List<object[]> oldList = _DB_Connection.Select(
                    DB_Table.EXAMINATIONS_AUDIENCES,
                    fields,
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("examination_id", Relation.EQUAL, id) }
                    );

                List<object[]> newList = new List<object[]>();
                foreach (DataGridViewRow row in dataGridView.Rows)
                    if (!row.IsNewRow)
                        newList.Add(new object[] { id, row.Cells[0].Value, row.Cells[1].Value, row.Index });

                _DB_Helper.UpdateData(DB_Table.EXAMINATIONS_AUDIENCES, oldList, newList, fields, new string[] { "examination_id", "number" }, transaction);

                transaction.Commit();
            }

            Cursor.Current = Cursors.Default;

            Classes.Utility.ShowChangesSavedMessage();
            DialogResult = DialogResult.OK;
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            ((DateTimePicker)sender).Tag = true;
        }

        private bool AssureSave()
        {
            if (cbSubject.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбрана дисциплина.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dataGridView.Rows.Count < 2)
            {
                MessageBox.Show("Не добавлено ни одной аудитории.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            foreach (DataGridViewRow row in dataGridView.Rows)
                if (!row.IsNewRow)
                {
                    bool error = false;
                    if (row.Cells[0].Value == null || row.Cells[1].Value == null)
                    {
                        MessageBox.Show("Не заполнен номер или вместимость одной из аудиторий.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        error = true;
                    }
                    else if (row.Cells[0].Value.ToString().Length > 5)
                    {
                        MessageBox.Show("Номер аудитории превышает максимальную длину.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        error = true;
                    }
                    else if ((ushort)row.Cells[1].Value == 0)
                    {
                        MessageBox.Show("Вместимость аудитории не может быть равна 0.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        error = true;
                    }
                    else
                        for (byte i = 0; i < row.Index; ++i)
                            if (dataGridView[0, i].Value.Equals(row.Cells[0].Value))
                            {
                                MessageBox.Show("Дублируются номера аудиторий.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                error = true;
                                break;
                            }

                    if (error)
                    {
                        dataGridView.ClearSelection();
                        row.Selected = true;
                        return false;
                    }
                }

            if (Controls.OfType<DateTimePicker>().Any(s => !(bool)s.Tag) && !Classes.Utility.ShowChoiceMessageBox("Одно из полей даты не менялось. Продолжить сохранение?", "Предупреждение"))
                return false;

            return true;
        }
    }
}
