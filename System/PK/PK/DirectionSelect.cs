using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PK
{
    public partial class DirectionSelect : Form
    {
        DB_Connector _DB_Connection;
        public string DirectionName;
        public string DirectionCode;

        public DirectionSelect()
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
        }

        private void DirectionSelect_Load(object sender, EventArgs e)
        {
            foreach (var item in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS,"code","name","ugs_name"))
                dgvDirectionSelection.Rows.Add(item[0], item[1], item[2]);
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            if (dgvDirectionSelection.SelectedRows.Count < 1)
                MessageBox.Show("Выберите направление в таблице.");
            else
            {
                DirectionCode = dgvDirectionSelection.SelectedRows[0].Cells[0].Value.ToString();
                DirectionName = dgvDirectionSelection.SelectedRows[0].Cells[1].Value.ToString();
                DialogResult = DialogResult.OK;
            }
        }
    }
}
