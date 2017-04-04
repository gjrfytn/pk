using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class DirectionSelect : Form
    {
        public uint DirectionID;
        public string DirectionCode;
        public string DirectionName;
        public string DirectionFaculty;

        Classes.DB_Connector _DB_Connection;
        Classes.DB_Helper _DB_Helper;

        public DirectionSelect(List<string> filters)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            foreach (object[] item in _DB_Connection.Select(DB_Table.DIRECTIONS, "direction_id", "faculty_short_name"))
            {
                string[] dirData = _DB_Helper.GetDirectionsDictionaryNameAndCode((uint)item[0]);
                foreach (string v in filters)
                    if (dirData[1].Split('.')[1] == v)
                        dgvDirectionSelection.Rows.Add(item[0], dirData[1], dirData[0], item[1]);
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
