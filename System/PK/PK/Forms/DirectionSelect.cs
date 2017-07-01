using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class DirectionSelect : Form
    {
        public uint DirectionID;
        public string DirectionCode;
        public string DirectionName;
        public string DirectionFaculty;

        private readonly DB_Connector _DB_Connection;
        private readonly DB_Helper _DB_Helper;

        public DirectionSelect(DB_Connector connection, List<string> filters)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new DB_Helper(_DB_Connection);

            foreach (object[] item in _DB_Connection.Select(DB_Table.DIRECTIONS, "direction_id", "faculty_short_name"))
            {
                Tuple<string,string> dirData = _DB_Helper.GetDirectionNameAndCode((uint)item[0]);
                foreach (string v in filters)
                    if (dirData.Item2.Split('.')[1] == v)
                        dgvDirectionSelection.Rows.Add(item[0], dirData.Item2, dirData.Item1, item[1]);
            }

        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            if (dgvDirectionSelection.SelectedRows.Count < 1)
                MessageBox.Show("Выберите направление в таблице.");
            else
            {
                DirectionID = (uint)dgvDirectionSelection.SelectedRows[0].Cells[0].Value;
                DirectionCode = dgvDirectionSelection.SelectedRows[0].Cells[1].Value.ToString();
                DirectionName = dgvDirectionSelection.SelectedRows[0].Cells[2].Value.ToString();
                DirectionFaculty = dgvDirectionSelection.SelectedRows[0].Cells[3].Value.ToString();

                DialogResult = DialogResult.OK;
            }
        }
    }
}
