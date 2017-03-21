using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK.Forms
{
    partial class ExaminationEdit : Form
    {
        readonly Classes.DB_Connector _DB_Connection;
        readonly Classes.DB_Helper _DB_Helper;
        readonly uint? _ID;

        public ExaminationEdit(Classes.DB_Connector connection, uint? id)
        {
            #region Components
            InitializeComponent();

            dataGridView_Capacity.ValueType = typeof(ushort);
            #endregion

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _ID = id;

            Dictionary<uint, string> subjects = _DB_Helper.GetDictionaryItems(1);

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
            if (cbSubject.SelectedIndex != -1)
                if (dataGridView.Rows.Count > 1)
                {
                    foreach (DataGridViewRow row in dataGridView.Rows)
                        if (!row.IsNewRow && (row.Cells[0].Value == null || row.Cells[1].Value == null))
                        {
                            MessageBox.Show("Не заполнен номер или вместимость одной из аудиторий.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                    Dictionary<string, object> data = new Dictionary<string, object>
                       {
                        {"subject_dict_id",1},
                        {"subject_id",_DB_Helper.GetDictionaryItemID(1,cbSubject.SelectedItem.ToString())},
                        {"date",dtpDate.Value},
                        {"reg_start_date",dtpRegStartDate.Value},
                        {"reg_end_date",dtpRegEndDate.Value}
                       };

                    if (_ID.HasValue)
                    {
                        _DB_Connection.Update(DB_Table.EXAMINATIONS, data, new Dictionary<string, object> { { "id", _ID } });
                        foreach (DataGridViewRow row in dataGridView.Rows)
                            if (!row.IsNewRow)
                                _DB_Connection.InsertOnDuplicateUpdate(
                                    DB_Table.EXAMINATIONS_AUDIENCES,
                                    new Dictionary<string, object> {
                                { "examination_id", _ID },
                                { "number", row.Cells[0].Value },
                                { "capacity", row.Cells[1].Value }
                                    });
                    }
                    else
                    {
                        uint id = _DB_Connection.Insert(DB_Table.EXAMINATIONS, data);
                        foreach (DataGridViewRow row in dataGridView.Rows)
                            if (!row.IsNewRow)
                                _DB_Connection.Insert(
                                DB_Table.EXAMINATIONS_AUDIENCES,
                                new Dictionary<string, object> {
                                { "examination_id", id },
                                { "number", row.Cells[0].Value },
                                { "capacity", row.Cells[1].Value }
                                });
                    }

                    Close();
                }
                else
                    MessageBox.Show("Не добавлено ни одной аудитории.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Не выбрана дисциплина.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
