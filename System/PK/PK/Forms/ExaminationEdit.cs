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
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _ID = id;

            #region Components
            InitializeComponent();

            dataGridView_Capacity.ValueType = typeof(ushort);

            object[] curCampStartEnd = _DB_Connection.Select(
                DB_Table.CAMPAIGNS,
                new string[] { "start_year", "end_year" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("id", Relation.EQUAL, Classes.Utility.CurrentCampaignID) }
                )[0];

            dtpDate.MinDate = new DateTime((int)(uint)curCampStartEnd[0], 1, 1);
            dtpDate.MaxDate = new DateTime((int)(uint)curCampStartEnd[0], 12, 31);
            dtpRegStartDate.MinDate = dtpDate.MinDate;
            dtpRegStartDate.MaxDate = dtpDate.MaxDate;
            dtpRegEndDate.MinDate = dtpDate.MinDate;
            dtpRegEndDate.MaxDate = dtpDate.MaxDate;

            if (_ID.HasValue)
            {
                foreach (DateTimePicker dtp in Controls.OfType<DateTimePicker>())
                    dtp.Tag = true;
            }
            else
            {
                foreach (DateTimePicker dtp in Controls.OfType<DateTimePicker>())
                    dtp.Tag = false;
            }
            #endregion

            Dictionary<uint, string> subjects = _DB_Helper.GetDictionaryItems(FIS_Dictionary.SUBJECTS);

            cbSubject.Items.AddRange(subjects.Values.ToArray());

            if (_ID.HasValue)
            {
                object[] exam = _DB_Connection.Select(DB_Table.EXAMINATIONS,
                    new string[] { "subject_id", "date", "reg_start_date", "reg_end_date" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id",Relation.EQUAL,_ID)
                    }
                    )[0];

                cbSubject.SelectedItem = subjects[(uint)exam[0]];
                dtpDate.Value = (DateTime)exam[1];
                dtpRegStartDate.Value = (DateTime)exam[2];
                dtpRegEndDate.Value = (DateTime)exam[3];

                foreach (object[] row in _DB_Connection.Select(
                    DB_Table.EXAMINATIONS_AUDIENCES,
                    new string[] { "number", "capacity" },
                    new List<Tuple<string, Relation, object>>
                    {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,_ID)
                    }))
                    dataGridView.Rows.Add(row[0], row[1]);
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            if (cbSubject.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбрана дисциплина.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataGridView.Rows.Count < 2)
            {
                MessageBox.Show("Не добавлено ни одной аудитории.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow row in dataGridView.Rows)
                if (!row.IsNewRow && (row.Cells[0].Value == null || row.Cells[1].Value == null))
                {
                    MessageBox.Show("Не заполнен номер или вместимость одной из аудиторий.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            if (Controls.OfType<DateTimePicker>().Any(s => !(bool)s.Tag) && !Classes.Utility.ShowChoiceMessageBox("Одно из полей даты не менялось. Продолжить сохранение?", "Предупреждение"))
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
                if (_ID.HasValue)
                {
                    _DB_Connection.Update(
                        DB_Table.EXAMINATIONS,
                        data,
                        new Dictionary<string, object> { { "id", _ID } },
                        transaction
                        );

                    string[] fields = { "examination_id", "number", "capacity" };
                    List<object[]> oldL = _DB_Connection.Select(
                        DB_Table.EXAMINATIONS_AUDIENCES,
                        fields,
                        new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("examination_id", Relation.EQUAL, _ID) }
                        );

                    List<object[]> newL = new List<object[]>();
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if (!row.IsNewRow)
                            newL.Add(new object[] { _ID, row.Cells[0].Value, row.Cells[1].Value });

                    _DB_Helper.UpdateData(DB_Table.EXAMINATIONS_AUDIENCES, oldL, newL, fields, new string[] { "examination_id", "number" }, transaction);
                }
                else
                {
                    uint id = _DB_Connection.Insert(DB_Table.EXAMINATIONS, data, transaction);
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if (!row.IsNewRow)
                            _DB_Connection.Insert(
                            DB_Table.EXAMINATIONS_AUDIENCES,
                            new Dictionary<string, object>
                            {
                                { "examination_id", id },
                                { "number", row.Cells[0].Value },
                                { "capacity", row.Cells[1].Value }
                            },
                            transaction
                            );
                }

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
    }
}
