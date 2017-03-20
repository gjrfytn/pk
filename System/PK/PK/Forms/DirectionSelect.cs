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

        Classes.DB_Connector _DB_Connection;

        public DirectionSelect(List<string> filters)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();

            foreach (object[] item in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "id", "code", "name"))
                foreach (string v in filters)
                    if (item[1].ToString().Substring(3, 2) == v)
                        dgvDirectionSelection.Rows.Add(item);
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

                DialogResult = DialogResult.OK;
            }
        }
    }
}
