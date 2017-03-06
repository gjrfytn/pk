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
        public string directionName;
        public string directionCode;
        public string directionID;
        List <string> codeFilters;

        public DirectionSelect(List<string> filters)
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
            codeFilters = new List<string>();
            codeFilters.AddRange(filters);
            foreach (var item in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS,"id", "code","name")) 
                foreach (var v in codeFilters)
                    if (item[1].ToString().Substring(3,2)==v)
                        dgvDirectionSelection.Rows.Add(item[0], item[1], item[2]);
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            if (dgvDirectionSelection.SelectedRows.Count < 1)
                MessageBox.Show("Выберите направление в таблице.");
            else
            {
                directionID = dgvDirectionSelection.SelectedRows[0].Cells[0].Value.ToString();
                directionCode = dgvDirectionSelection.SelectedRows[0].Cells[1].Value.ToString();
                directionName = dgvDirectionSelection.SelectedRows[0].Cells[2].Value.ToString();
                DialogResult = DialogResult.OK;
            }
        }
    }
}
