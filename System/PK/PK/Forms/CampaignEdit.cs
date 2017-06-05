using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class CampaignEdit : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly  Classes.DB_Helper _DB_Helper;

        private uint? _CampaignId;

        public CampaignEdit(Classes.DB_Connector connection,uint? campUid)
        {
            #region Components
            InitializeComponent();

            dgvDirections_OOOF.ValueType = typeof(ushort);
            dgvDirections_OOOZF.ValueType = typeof(ushort);
            dgvDirections_OKOF.ValueType = typeof(ushort);
            dgvDirections_OKOZF.ValueType = typeof(ushort);

            dgvTargetOrganizatons_OF.ValueType = typeof(ushort);
            dgvTargetOrganizatons_OZF.ValueType = typeof(ushort);

            dgvFacultities_HostelPlaces.ValueType = typeof(ushort);

            dgvPaidPlaces_OFPM.ValueType = typeof(ushort);
            dgvPaidPlaces_OZFPM.ValueType = typeof(ushort);
            dgvPaidPlaces_ZFPM.ValueType = typeof(ushort);
            #endregion

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _CampaignId = campUid;

            cbState.SelectedIndex = 0;
            cbType.SelectedIndex = 0;

            int currYear = DateTime.Now.Year;
            for (int i = -2; i <= 1; i++)
            {
                cbStartYear.Items.Add((currYear + i).ToString());
                cbEndYear.Items.Add((currYear + i).ToString());
            }
            cbStartYear.SelectedIndex = 2;
            cbEndYear.SelectedIndex = 2;

            ChangeButtonsAppearance(0);

            if (_CampaignId.HasValue)
            {
                LoadCampaign();
                LoadTables();
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (CheckBox v in gbEduLevel.Controls)
                v.Checked = false;

            if (cbType.SelectedItem.ToString() == "Прием иностранце по направлениям Минобрнауки")
                foreach (CheckBox v in gbEduLevel.Controls)
                    v.Enabled = true;
            else
                foreach (CheckBox v in gbEduLevel.Controls)
                    v.Enabled = false;

            switch (cbType.SelectedItem.ToString())
            {
                case "Прием на обучение на бакалавриат/специалитет":
                    cbEduLevelBacc.Visible = true;
                    cbEduLevelBacc.Enabled = true;
                    cbEduLevelSpec.Visible = true;
                    cbEduLevelSpec.Enabled = true;
                    break;
                case "Прием на обучение в магистратуру":
                    cbEduLevelMag.Checked = true;
                    break;
            }
        }

        private void dgvTargetOrganizatons_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ChangeButtonsAppearance(dgvTargetOrganizatons.Rows.Count-1);
        }

        private void dgvTargetOrganizatons_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
                switch (dgvTargetOrganizatons.CurrentCell.ColumnIndex)
            {
                case 2:
                    TargetOrganizationSelect form1 = new TargetOrganizationSelect(_DB_Connection,null);
                    form1.ShowDialog();
                    dgvTargetOrganizatons.CurrentRow.Cells[0].Value = form1.OrganizationID;
                    dgvTargetOrganizatons.CurrentRow.Cells[1].Value = form1.OrganizationName;
                    break;
                case 7:
                    List<string> filters = new List<string>();
                    if (cbEduLevelBacc.Checked)
                        filters.Add("03");
                    if (cbEduLevelSpec.Checked)
                        filters.Add("05");
                    if (cbEduLevelMag.Checked)
                        filters.Add("04");

                    DirectionSelect form2 = new DirectionSelect(_DB_Connection,filters);
                    form2.ShowDialog();
                    dgvTargetOrganizatons.CurrentRow.Cells[3].Value = form2.DirectionID;
                    dgvTargetOrganizatons.CurrentRow.Cells[4].Value = form2.DirectionName;
                    dgvTargetOrganizatons.CurrentRow.Cells[5].Value = form2.DirectionCode;
                    dgvTargetOrganizatons.CurrentRow.Cells[6].Value = form2.DirectionFaculty;
                    dgvTargetOrganizatons.CurrentRow.Cells[dgvTargetOrganizatons_OF.Index].Value = 0;
                    dgvTargetOrganizatons.CurrentRow.Cells[dgvTargetOrganizatons_OZF.Index].Value = 0;
                    dgvTargetOrganizatons.CurrentRow.Cells[dgvTargetOrganizatons_Sum.Index].Value = 0;
                    if (dgvTargetOrganizatons.CurrentRow.Cells[3].Value != null)
                    {
                        dgvTargetOrganizatons.Rows.Add();
                        foreach (DataGridViewCell cell in dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.CurrentRow.Index].Cells)
                        {
                            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.CurrentRow.Index - 1].Cells[cell.ColumnIndex].Value = cell.Value;
                            if (cell.ColumnIndex != dgvTargetOrganizatons_Select.Index && cell.ColumnIndex != dgvTargetOrganizatons_DirSelect.Index)
                                cell.Value = "";
                        }
                    }
                    break;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            bool progressEnabled = true;

            if ((!_CampaignId.HasValue)&&(!(_DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "id" },
                        new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("name", Relation.EQUAL, tbName.Text)
                            }).Count == 0)))
                MessageBox.Show("Компания с таким именем уже существует.");
            else
            {
                foreach (DataGridViewRow r in dgvEntranceTests.Rows)
                    for (int i = 4; i < r.Cells.Count; i++)
                        for (int j = i + 1; j < r.Cells.Count; j++)
                        {
                            if ((r.Cells[i] as DataGridViewComboBoxCell).Value.ToString() ==
                                (r.Cells[j] as DataGridViewComboBoxCell).Value.ToString())
                            {
                                progressEnabled = false;
                                MessageBox.Show("Совпадают дисциплины вступительных испытаний для направления\n\n" + r.Cells[2].Value.ToString()
                                    + "\n" + r.Cells[1].Value.ToString() + "\n\nДисциплины для одного направления не должны повторяться.");
                            }
                        }
                if (progressEnabled)
                {
                    if (!cbEduFormO.Checked && !cbEduFormOZ.Checked && !cbEduFormZ.Checked)
                        MessageBox.Show("Не выбраны формы обучения.");                    
                    else if (!cbEduLevelBacc.Checked && !cbEduLevelMag.Checked && !cbEduLevelSpec.Checked)
                        MessageBox.Show("Не выбран уровень образования.");
                    else if (!_CampaignId.HasValue)
                            {
                                SaveCampaign();
                                DialogResult = DialogResult.OK;
                            }
                        else
                        {
                            UpdateCampaing();
                        }
                }
            }            
        }

        private void CampaignEdit_Load(object sender, EventArgs e)
        {
            if (_DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS).Count == 0)
            {
                MessageBox.Show("Справочник направлний ФИС пуст. Чтобы загрузить его, выберите:\nГлавное Меню -> Справка -> Справочник направлений ФИС -> Обновить");
                DialogResult = DialogResult.Abort;
            }
            else if (_DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS).Count == 0)
            {
                MessageBox.Show("Справочники пусты. Чтобы загрузить их, выберите:\nГлавное Меню -> Справка -> Справочники ФИС -> Обновить");
                DialogResult = DialogResult.Abort;
            }
        }

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void cbEduForm_CheckedChanged(object sender, EventArgs e)
        {
            dgvDirections_OOOF.Visible = cbEduFormO.Checked;
            dgvDirections_OKOF.Visible = cbEduFormO.Checked;
            dgvTargetOrganizatons_OF.Visible = cbEduFormO.Checked;
            dgvPaidPlaces_OFPM.Visible = cbEduFormO.Checked;
            if (!cbEduFormO.Checked)
            {
                foreach (DataGridViewRow row in dgvDirections.Rows)
                {
                    row.Cells[dgvDirections_OOOF.Index].Value = 0;
                    row.Cells[dgvDirections_OKOF.Index].Value = 0;
                }
                foreach (DataGridViewRow row in dgvTargetOrganizatons.Rows)
                    if (row.Index < dgvTargetOrganizatons.Rows.Count - 1)
                        row.Cells[dgvTargetOrganizatons_OF.Index].Value = 0;

                foreach (DataGridViewRow row in dgvPaidPlaces.Rows)
                    row.Cells[dgvPaidPlaces_OFPM.Index].Value = 0;
            }

            dgvDirections_OOOZF.Visible = cbEduFormOZ.Checked;
            dgvDirections_OKOZF.Visible = cbEduFormOZ.Checked;
            dgvTargetOrganizatons_OZF.Visible = cbEduFormOZ.Checked;
            dgvPaidPlaces_OZFPM.Visible = cbEduFormOZ.Checked;
            if (!cbEduFormOZ.Checked)
            {
                foreach (DataGridViewRow row in dgvDirections.Rows)
                {
                    row.Cells[dgvDirections_OOOZF.Index].Value = 0;
                    row.Cells[dgvDirections_OKOZF.Index].Value = 0;
                }
                foreach (DataGridViewRow row in dgvTargetOrganizatons.Rows)
                    if (row.Index < dgvTargetOrganizatons.Rows.Count - 1)
                        row.Cells[dgvTargetOrganizatons_OZF.Index].Value = 0;

                foreach (DataGridViewRow row in dgvPaidPlaces.Rows)
                    row.Cells[dgvPaidPlaces_OZFPM.Index].Value = 0;
            }

            dgvPaidPlaces_ZFPM.Visible = cbEduFormZ.Checked;
            if (!cbEduFormZ.Checked)
                foreach (DataGridViewRow row in dgvPaidPlaces.Rows)
                    row.Cells[dgvPaidPlaces_ZFPM.Index].Value = 0;
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).CurrentCell.Value.ToString() == "")
                ((DataGridView)sender).CurrentCell.Value = 0;
            if (((DataGridView)sender).CurrentCell != null)
                SumCells(((DataGridView)sender).CurrentCell);
        }

        private void cbEduLevel_CheckedChanged(object sender, EventArgs e)
        {
            FillDirectionsTable();
            FillFacultiesTable();
            FillProfiliesTable();
            FillDistTable();
            if (_CampaignId != null)
            {
                LoadTables();
            }
            dgvEntranceTests.Visible = !cbEduLevelMag.Checked;
            dgvEntranceTests.Enabled = !cbEduLevelMag.Checked;
            label10.Visible = !cbEduLevelMag.Checked;
        }


        private void ChangeButtonsAppearance(int rowindex)
        {

            //dgvTargetOrganizatons.Rows[rowindex].Cells[2].Style.Font = new Font("ESSTIXTwo", 11);
            dgvTargetOrganizatons.Rows[rowindex].Cells[2].Value = "<";

            //dgvTargetOrganizatons.Rows[rowindex].Cells[7].Style.Font = new Font("ESSTIXTwo", 11);
            dgvTargetOrganizatons.Rows[rowindex].Cells[7].Value = "<";

        }

        private void FillDirectionsTable()
        {
            dgvDirections.Rows.Clear();
            foreach (var v in _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, "name", "code", "id" ))
                foreach (var r in _DB_Connection.Select(DB_Table.DIRECTIONS, "faculty_short_name", "direction_id"))

                if ((cbEduLevelBacc.Checked)&&(v[1].ToString().Substring(3, 2) == "03")&&(r[1].ToString()==v[2].ToString()))
                            dgvDirections.Rows.Add(v[2].ToString(),true, v[0].ToString(), v[1].ToString(), r[0].ToString(), 0, 0, 0, 0, 0, 0, 0, 0, 0);

                else if ((cbEduLevelSpec.Checked)&&(v[1].ToString().Substring(3, 2) == "05")&&(r[1].ToString() == v[2].ToString()))
                        dgvDirections.Rows.Add(v[2].ToString(), true, v[0].ToString(), v[1].ToString(), r[0].ToString(), 0, 0, 0, 0, 0, 0, 0, 0, 0);

                else if ((cbEduLevelMag.Checked)&&(v[1].ToString().Substring(3, 2) == "04")&&(r[1].ToString() == v[2].ToString()))
                            dgvDirections.Rows.Add(v[2].ToString(), true, v[0].ToString(), v[1].ToString(), r[0].ToString(), 0, 0, 0, 0, 0, 0, 0, 0, 0);

            dgvDirections.Sort(dgvDirections.Columns[1], ListSortDirection.Ascending);
            dgvDirections.Rows.Add("", false, "ИТОГО", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dgvDirections.Rows[dgvDirections.Rows.Count - 1].ReadOnly = true;
        }

        private void FillProfiliesTable ()
        {
            dgvPaidPlaces.Rows.Clear();
            foreach (var v in _DB_Connection.Select(DB_Table.PROFILES, "name", "direction_id", "faculty_short_name", "short_name"))
                foreach (DataGridViewRow row in dgvDirections.Rows)
                    if (row.Index < dgvDirections.Rows.Count-1 && uint.Parse(row.Cells[dgvDirections_DirID.Index].Value.ToString()) == (uint)v[1]
                        && row.Cells[dgvDirections_Fac.Index].Value.ToString() == v[2].ToString())
                        dgvPaidPlaces.Rows.Add(v[1], v[0], _DB_Helper.GetDirectionNameAndCode((uint)v[1]).Item2, 
                            _DB_Connection.Select(DB_Table.DIRECTIONS, new string[] { "short_name" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, v[2].ToString()),
                                new Tuple<string, Relation, object>("direction_id", Relation.EQUAL, (uint)v[1])
                            })[0][0],                            
                            v[2], 0, 0, 0, v[3], 0);
            dgvPaidPlaces.Rows.Add("", "ИТОГО", "", "", "", 0, 0, 0, "", 0);
            dgvPaidPlaces.Rows[dgvPaidPlaces.Rows.Count - 1].ReadOnly = true;
        }

        private void FillFacultiesTable ()
        {
            dgvFacultities.Rows.Clear();
            foreach (var v in _DB_Connection.Select(DB_Table.FACULTIES, "short_name", "name"))
            {
                bool found = false;
                foreach (DataGridViewRow r in dgvDirections.Rows)
                    if ((v[0].ToString() == r.Cells[4].Value.ToString())&&((bool)(r.Cells[1].Value)))
                    {
                        found = true;
                    }
                if (found)
                dgvFacultities.Rows.Add(v[0].ToString(), v[1].ToString(), 0);
            }                
            dgvFacultities.Sort(dgvFacultities.Columns[1], ListSortDirection.Ascending);
            dgvFacultities.Rows.Add("", "ИТОГО", 0);
            dgvFacultities.Rows[dgvFacultities.Rows.Count - 1].ReadOnly = true;
        }

        private void FillDistTable ()
        {
            dgvEntranceTests.Rows.Clear();
            foreach (DataGridViewRow r in dgvDirections.Rows)
                if (r.Index < dgvDirections.Rows.Count - 1)        
                {
                    bool found = false;
                    foreach (DataGridViewRow v in dgvEntranceTests.Rows)
                        if ((v.Cells[0].Value.ToString() == r.Cells[0].Value.ToString())&&(v.Cells[3].Value==r.Cells[4].Value))
                            found = true;
                    if (!found)
                    {
                        dgvEntranceTests.Rows.Add(r.Cells[0].Value.ToString(), r.Cells[2].Value.ToString(), r.Cells[3].Value.ToString(), r.Cells[4].Value);
                        for (int i=4; i< dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells.Count; i++)
                        {
                            (dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Математика");
                            (dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Русский язык");
                            (dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Физика");
                            (dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Обществознание");
                            (dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Иностранный язык");

                            switch (i)
                            {
                                case 4:
                                    dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells[i].Value = "Математика";
                                    break;
                                case 5:
                                    dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells[i].Value = "Физика";
                                    break;
                                case 6:
                                    dgvEntranceTests.Rows[dgvEntranceTests.Rows.Count - 1].Cells[i].Value = "Русский язык";
                                    break;
                            }
                            found = false;
                        }
                    }                
                }
            dgvEntranceTests.Sort(dgvEntranceTests.Columns[1], ListSortDirection.Ascending);
        }

        private void SumCells(DataGridViewCell cell)
        {
            DataGridView table = cell.DataGridView;
            DataGridViewColumn sumColumn = null;
            DataGridViewColumn start = null;
            DataGridViewColumn finish = null;
            if (table == dgvDirections)
            {
                sumColumn = dgvDirections_Sum;
                start = dgvDirections_OOOF;
                finish = dgvDirections_OKOZF;
            }
            else if (table == dgvPaidPlaces)
            {
                sumColumn = dgvPaidPlaces_Sum;
                start = dgvPaidPlaces_OFPM;
                finish = dgvPaidPlaces_ZFPM;
            }

            if (table == dgvDirections && cell != null && cell.ColumnIndex >= start.Index && cell.ColumnIndex < sumColumn.Index && cell.RowIndex < table.Rows.Count - 1
                    || table == dgvPaidPlaces && cell != null && cell.ColumnIndex >= start.Index && cell.ColumnIndex < sumColumn.Index && cell.RowIndex < table.Rows.Count - 1)
            {
                table.Rows[cell.RowIndex].Cells[sumColumn.Index].Value = 0;
                table.Rows[table.Rows.Count - 1].Cells[cell.ColumnIndex].Value = 0;
                table.Rows[table.Rows.Count - 1].Cells[sumColumn.Index].Value = 0;

                for (int i = start.Index; i <= finish.Index; i++)
                    table.Rows[cell.RowIndex].Cells[sumColumn.Index].Value =
                int.Parse(table.Rows[cell.RowIndex].Cells[sumColumn.Index].Value.ToString())
                + int.Parse(table.Rows[cell.RowIndex].Cells[i].Value.ToString());

                foreach (DataGridViewRow row in table.Rows)
                    if (row.Index < table.Rows.Count - 1)
                    {
                        table.Rows[table.Rows.Count - 1].Cells[cell.ColumnIndex].Value =
                            int.Parse(table.Rows[table.Rows.Count - 1].Cells[cell.ColumnIndex].Value.ToString())
                            + int.Parse(row.Cells[cell.ColumnIndex].Value.ToString());
                        table.Rows[table.Rows.Count - 1].Cells[sumColumn.Index].Value =
                            int.Parse(table.Rows[table.Rows.Count - 1].Cells[sumColumn.Index].Value.ToString())
                            + int.Parse(row.Cells[sumColumn.Index].Value.ToString());
                    }
            }
            else if (table == dgvFacultities && cell.RowIndex < table.Rows.Count - 1)
            {
                table.Rows[table.Rows.Count - 1].Cells[dgvFacultities_HostelPlaces.Index].Value = 0;
                foreach (DataGridViewRow row in dgvFacultities.Rows)
                    if (row.Index < table.Rows.Count - 1)
                        table.Rows[table.Rows.Count - 1].Cells[dgvFacultities_HostelPlaces.Index].Value = int.Parse(table.Rows[table.Rows.Count - 1].Cells[dgvFacultities_HostelPlaces.Index].Value.ToString())
                            + int.Parse(row.Cells[dgvFacultities_HostelPlaces.Index].Value.ToString());
            }
            else if (table == dgvTargetOrganizatons)
            {
                dgvTargetOrganizatons.Rows[cell.RowIndex].Cells[dgvTargetOrganizatons_Sum.Index].Value =
                    int.Parse(dgvTargetOrganizatons.Rows[cell.RowIndex].Cells[dgvTargetOrganizatons_OF.Index].Value.ToString()) +
                    int.Parse(dgvTargetOrganizatons.Rows[cell.RowIndex].Cells[dgvTargetOrganizatons_OZF.Index].Value.ToString());               
            }
        }


        private void SaveCampaign()
        {
            Cursor.Current = Cursors.WaitCursor;
            _CampaignId = _DB_Connection.Insert(DB_Table.CAMPAIGNS, new Dictionary<string, object>
                        {
                            { "name", tbName.Text }, { "start_year",  Convert.ToInt32(cbStartYear.SelectedItem)},
                            { "end_year", Convert.ToInt32(cbEndYear.SelectedItem) },
                            { "status_dict_id",  (uint)FIS_Dictionary.CAMPAIGN_STATUS}, { "status_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.CAMPAIGN_STATUS, cbState.SelectedItem.ToString()) },
                            { "type_dict_id",  (uint)FIS_Dictionary.CAMPAIGN_TYPE}, { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.CAMPAIGN_TYPE, cbType.SelectedItem.ToString()) } });

            SaveEduForms();
            SaveEduLevels();
            SaveFaculties();
            SaveDirections();
            SaveTargetOrganizations();
            SaveProfiles();            
            SaveEntranceTests();
            Cursor.Current = Cursors.Default;
        }

        private void UpdateCampaing()
        {
            Cursor.Current = Cursors.WaitCursor;
            _DB_Connection.Update(DB_Table.CAMPAIGNS, new Dictionary<string, object>
                        {   { "name", tbName.Text }, { "start_year",  int.Parse(cbStartYear.SelectedItem.ToString())},
                            { "end_year", int.Parse(cbEndYear.SelectedItem.ToString()) },
                            { "status_dict_id",  (uint)FIS_Dictionary.CAMPAIGN_STATUS}, { "status_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.CAMPAIGN_STATUS, cbState.SelectedItem.ToString()) },
                            { "type_dict_id",  (uint)FIS_Dictionary.CAMPAIGN_TYPE}, { "type_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.CAMPAIGN_TYPE, cbType.SelectedItem.ToString()) } },
                        new Dictionary<string, object>
                        {
                            { "id", _CampaignId}
                        });

            UpdateEduForms();
            UpdateEduLevels();
            UpdateFaculties();
            UpdateDirections();
            UpdateTargetOrganizations();            
            UpdateProfiles();
            UpdateEntranceTests();
            Cursor.Current = Cursors.Default;
        }

        private void LoadCampaign()
        {
            Cursor.Current = Cursors.WaitCursor;
            List<object> loadedCampaign = new List<object>();
            loadedCampaign.AddRange(_DB_Connection.Select(DB_Table.CAMPAIGNS,
                new string[] { "name", "start_year", "end_year", "status_dict_id", "status_id", "type_dict_id", "type_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, _CampaignId)
                })[0]);

            tbName.Text = loadedCampaign[0].ToString();
            cbStartYear.SelectedItem = loadedCampaign[1].ToString();
            cbEndYear.SelectedItem = loadedCampaign[2].ToString();

            cbState.SelectedItem = _DB_Helper.GetDictionaryItemName((FIS_Dictionary)loadedCampaign[3], (uint)loadedCampaign[4]);

            cbType.SelectedItem = _DB_Helper.GetDictionaryItemName((FIS_Dictionary)loadedCampaign[5], (uint)loadedCampaign[6]);

            LoadEduForms();
            LoadEduLevels();
            Cursor.Current = Cursors.Default;
        }

        private void LoadTables()
        {
            LoadDirections();
            LoadTargetOrganizations();
            LoadFaculties();
            LoadProfiles();
            LoadEntranceTests();
        }


        private void SaveEduForms()
        {
            if (cbEduFormO.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    {
                        { "campaigns_id", _CampaignId},
                        { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_FORM},
                        { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, "Очная форма") } });
            if (cbEduFormOZ.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    {
                        { "campaigns_id", _CampaignId},
                        { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_FORM},
                        { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, "Очно-заочная (вечерняя)") } });
            if (cbEduFormZ.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    {
                        { "campaigns_id", _CampaignId},
                        { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_FORM},
                        { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, "Заочная форма") } });
        }

        private void SaveEduLevels()
        {
            if (cbEduLevelBacc.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_id", _CampaignId},
                    { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_LEVEL},
                    { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_LEVEL, "Бакалавриат") } });

            if (cbEduLevelMag.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_id", _CampaignId},
                    { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_LEVEL},
                    { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_LEVEL, "Магистратура") } });

            if (cbEduLevelSpec.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_id", _CampaignId},
                    { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_LEVEL},
                    { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_LEVEL, "Специалитет") } });
        }

        private void SaveDirections()
        {
            foreach (DataGridViewRow r in dgvDirections.Rows)
                if (r.Index < dgvDirections.Rows.Count - 1)
                {
                
                    int[] places = new int[5];
                    for (int i = 5; i <= 8; i++)
                        if (dgvDirections.Columns[i].Visible == true)
                            places[i - 5] = int.Parse(r.Cells[i].Value.ToString());

                    _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object>
                    {
                        { "campaign_id", _CampaignId},
                        { "direction_faculty", r.Cells[dgvDirections_Fac.Index].Value.ToString()},
                        { "direction_id", uint.Parse(r.Cells[dgvDirections_DirID.Index].Value.ToString())},
                        { "places_budget_o", places[0]},
                        { "places_budget_oz", places[1]},
                        { "places_quota_o", places[2]},
                        { "places_quota_oz", places[3]}});
                }
        }

        private void SaveTargetOrganizations()
        {
            foreach (DataGridViewRow row in dgvTargetOrganizatons.Rows)
                if (row.Index < dgvTargetOrganizatons.Rows.Count - 1)
                {
                    if (row.Index < dgvTargetOrganizatons.Rows.Count - 1)
                        _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new Dictionary<string, object> { { "campaign_id", _CampaignId},
                            { "direction_faculty", row.Cells[dgvTargetOrganizatons_faculty.Index].Value}, { "direction_id", row.Cells[dgvTargetOrganizatons_DirID.Index].Value},
                            { "target_organization_id", row.Cells[dgvTargetOrganizatons_ID.Index].Value},
                            { "places_o", row.Cells[dgvTargetOrganizatons_OF.Index].Value}, { "places_oz", row.Cells[dgvTargetOrganizatons_OZF.Index].Value } });
                }
        }

        private void SaveProfiles()
        {
            foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                if (r.Index < dgvPaidPlaces.Rows.Count - 1)
                    _DB_Connection.Insert(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object>
                    {
                        { "campaigns_id", _CampaignId},
                        { "profiles_direction_faculty", r.Cells[dgvPaidPlaces_Fac.Index].Value.ToString()},
                        { "profiles_direction_id", r.Cells[dgvPaidPlaces_ID.Index].Value},
                        { "profiles_short_name", r.Cells[dgvPaidPlaces_ProfileShortName.Index].Value},
                        { "places_paid_o", r.Cells[dgvPaidPlaces_OFPM.DisplayIndex].Value},
                        { "places_paid_oz", r.Cells[dgvPaidPlaces_OZFPM.Index].Value},
                        { "places_paid_z", r.Cells[dgvPaidPlaces_ZFPM.Index].Value}
                    });
        }

        private void SaveFaculties()
        {
            foreach (DataGridViewRow r in dgvFacultities.Rows)
                if (r.Index < dgvFacultities.Rows.Count - 1)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_FACULTIES_DATA, new Dictionary<string, object>
                {
                    { "campaign_id", _CampaignId},
                    { "faculty_short_name", r.Cells[dgvFacultities_ShortFacName.Index].Value.ToString()},
                    { "hostel_places", r.Cells[dgvFacultities_HostelPlaces.Index].Value}
                });
        }

        private void SaveEntranceTests()
        {
            if (!cbEduLevelMag.Checked)
                foreach (DataGridViewRow r in dgvEntranceTests.Rows)
                    for (int i = 1; i < r.Cells.Count - 3; i++)
                    {
                        bool found = false;
                        uint subjectId = _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, r.Cells[i + 3].Value.ToString());

                        foreach (var v in _DB_Connection.Select(DB_Table.ENTRANCE_TESTS,
                            new string[] { "priority", "direction_id", "subject_dict_id", "subject_id", "direction_faculty" },
                            new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL,_CampaignId),
                                    new Tuple<string, Relation, object>("direction_faculty", Relation.EQUAL, r.Cells[3].Value),
                                    new Tuple<string, Relation, object> ("direction_id", Relation.EQUAL, r.Cells[0].Value),
                                    new Tuple<string, Relation, object> ("subject_dict_id", Relation.EQUAL, (uint)FIS_Dictionary.SUBJECTS),
                                    new Tuple<string, Relation, object> ("subject_id", Relation.EQUAL, subjectId)
                                }))

                            if (Convert.ToInt32(v[0]) == i)
                                found = true;
                            else
                            {
                                _DB_Connection.Update(DB_Table.ENTRANCE_TESTS, new Dictionary<string, object>
                                {
                                    { "priority", i}
                                }, new Dictionary<string, object>
                                {
                                    { "campaign_id", _CampaignId},
                                    { "direction_id", v[1] },
                                    { "direction_faculty", v[4]},
                                    { "subject_dict_id", v[2] },
                                    { "subject_id", v[3] }
                                });
                                found = true;
                            }

                        if (!found)
                            _DB_Connection.Insert(DB_Table.ENTRANCE_TESTS, new Dictionary<string, object>
                            {
                                { "campaign_id", _CampaignId},
                                { "direction_faculty", r.Cells[3].Value },
                                { "direction_id", r.Cells[0].Value },
                                { "subject_dict_id", (uint)FIS_Dictionary.SUBJECTS },
                                { "subject_id", subjectId },
                                { "priority", i }
                            });
                    }
        }


        private void LoadEduForms()
        {
            foreach (var v in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.EDU_FORM),
                    new Tuple<string, Relation, object> ("campaigns_id", Relation.EQUAL, _CampaignId)
                }))
            {
                foreach (CheckBox chekBox in gbEduForms.Controls)
                    if (chekBox.Text == _DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_FORM, (uint)v[0]))
                        chekBox.Checked = true;
            }
        }

        private void LoadEduLevels()
        {
            foreach (var v in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
            new List<Tuple<string, Relation, object>>
            {
                                new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.EDU_LEVEL),
                                new Tuple<string, Relation, object> ("campaigns_id", Relation.EQUAL, _CampaignId)
            }))
            {
                string itemName = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.EDU_LEVEL, (uint)v[0]);

                switch (itemName)
                {
                    case "Бакалавриат":
                        cbEduLevelBacc.Checked = true;
                        break;
                    case "Магистратура":
                        cbEduLevelMag.Checked = true;
                        break;
                    case "Специалитет":
                        cbEduLevelSpec.Checked = true;
                        break;
                }
            }
        }

        private void LoadDirections()
        {
            foreach (var v in _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[] { "direction_faculty",
                    "direction_id", "places_budget_o", "places_budget_oz", "places_quota_o", "places_quota_oz"},
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                    }))
                foreach (DataGridViewRow r in dgvDirections.Rows)
                    if (r.Index < dgvDirections.Rows.Count - 1 && (v[0].ToString() == r.Cells[4].Value.ToString()) && (v[1].ToString() == r.Cells[0].Value.ToString()))
                    {
                        r.Cells[1].Value = true;
                        for (int i = 5; i < r.Cells.Count - 1; i++)
                            r.Cells[i].Value = v[i - 3].ToString();
                    }
            foreach (DataGridViewRow row in dgvDirections.Rows)
                if (row.Index < dgvDirections.Rows.Count)
                    SumCells(row.Cells[dgvDirections_OOOF.Index]);
            for (int i = dgvDirections_OOOF.Index; i <= dgvDirections_OKOZF.Index; i++)
                SumCells(dgvDirections.Rows[0].Cells[i]);
        }

        private void LoadTargetOrganizations()
        {
            dgvTargetOrganizatons.Rows.Clear();
            foreach (var record in _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new string[] { "direction_faculty", "direction_id", "target_organization_id",
                "places_o", "places_oz" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                }))
            {
                dgvTargetOrganizatons.Rows.Add(record[2], _DB_Connection.Select(DB_Table.TARGET_ORGANIZATIONS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)record[2])
                })[0][0].ToString(),
                "", record[1], _DB_Helper.GetDirectionNameAndCode((uint)record[1]).Item1,
                _DB_Helper.GetDirectionNameAndCode((uint)record[1]).Item2, record[0], "", record[3], record[4]);
                ChangeButtonsAppearance(dgvTargetOrganizatons.Rows.Count - 2);
                foreach (DataGridViewRow row in dgvTargetOrganizatons.Rows)
                    if (row.Index < dgvTargetOrganizatons.Rows.Count - 1)
                        SumCells(row.Cells[dgvTargetOrganizatons_OF.Index]);
            }
        }

        private void LoadFaculties()
        {
            foreach (var v in _DB_Connection.Select(DB_Table.CAMPAIGNS_FACULTIES_DATA, new string[] { "faculty_short_name", "hostel_places" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("campaign_id", Relation.EQUAL, _CampaignId)
                }))
                foreach (DataGridViewRow r in dgvFacultities.Rows)
                    if (r.Index < dgvFacultities.Rows.Count - 1 && r.Cells[0].Value.ToString() == v[0].ToString())
                        r.Cells[2].Value = v[1].ToString();
            foreach (DataGridViewRow row in dgvFacultities.Rows)
                if (row.Index < dgvFacultities.Rows.Count)
                    SumCells(row.Cells[dgvFacultities_HostelPlaces.Index]);
        }

        private void LoadProfiles()
        {
            foreach (var v in _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_faculty",
                    "profiles_direction_id", "profiles_short_name", "places_paid_o", "places_paid_oz", "places_paid_z" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId)
                    }
                    ))
                foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                    if (r.Index < dgvPaidPlaces.Rows.Count - 1 && (v[0].ToString() == r.Cells[dgvPaidPlaces_Fac.Index].Value.ToString()) && ((uint)v[1] == uint.Parse(r.Cells[dgvPaidPlaces_ID.Index].Value.ToString()))
                        && (v[2].ToString() == r.Cells[dgvPaidPlaces_ProfileShortName.Index].Value.ToString()))
                        for (int i = dgvPaidPlaces_OFPM.Index; i < r.Cells.Count-2; i++)
                            r.Cells[i].Value = v[i - v.Length + dgvPaidPlaces_OFPM.Index - 1].ToString();
            foreach (DataGridViewRow row in dgvPaidPlaces.Rows)
                if (row.Index < dgvPaidPlaces.Rows.Count)
                    SumCells(row.Cells[dgvPaidPlaces_OFPM.Index]);
            for (int i = dgvPaidPlaces_OFPM.Index; i <= dgvPaidPlaces_ZFPM.Index; i++)
                SumCells(dgvPaidPlaces.Rows[0].Cells[i]);
        }

        private void LoadEntranceTests()
        {
            if (!cbEduLevelMag.Checked)
                foreach (var v in _DB_Connection.Select(DB_Table.ENTRANCE_TESTS, new string[] { "direction_id", "subject_dict_id", "subject_id", "priority", "direction_faculty" },
                    new List<Tuple<string, Relation, object>>
                    {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                    }))
                    foreach (DataGridViewRow r in dgvEntranceTests.Rows)
                        if ((v[0].ToString() == r.Cells[0].Value.ToString()) && (v[4].ToString() == r.Cells[3].Value.ToString()))
                            r.Cells[3 + Convert.ToInt32(v[3])].Value = _DB_Helper.GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)v[2]);
        }


        private void UpdateEduForms()
        {
            List<object[]> eduFormsList = _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[]
                { "dictionaries_items_item_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId),
                        new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.EDU_FORM)
                    });

            foreach (CheckBox chekBox in gbEduForms.Controls)
                if (chekBox.Checked)
                {
                    bool found = false;
                    foreach (object[] listElement in eduFormsList)
                        if ((uint)listElement[0] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, chekBox.Text))
                            found = true;
                    if (!found)
                        _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                        { { "campaigns_id", _CampaignId}, {"dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_FORM},
                            { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, chekBox.Text) } });
                }
            foreach (object[] eduForm in eduFormsList)
            {
                bool found = false;
                foreach (CheckBox chekBox in gbEduForms.Controls)
                    if (((uint)eduForm[0] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, chekBox.Text)) && chekBox.Checked)
                        found = true;
                if (!found)
                    _DB_Connection.Delete(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object> { { "campaigns_id", _CampaignId },
                        { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_FORM }, { "dictionaries_items_item_id",  (uint)eduForm[0]} });
            }
        }

        private void UpdateEduLevels()
        {
            List<uint> eduLevelsList = new List<uint>();

            if (cbEduLevelBacc.Checked)
                eduLevelsList.Add(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_LEVEL, "Бакалавриат"));

            if (cbEduLevelMag.Checked)
                eduLevelsList.Add(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_LEVEL, "Магистратура"));

            if (cbEduLevelSpec.Checked)
                eduLevelsList.Add(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_LEVEL, "Специалитет"));

            List<object[]> oldList = _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[]
            { "dictionaries_items_item_id", }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId),
                new Tuple<string, Relation, object>("dictionaries_items_dictionary_id", Relation.EQUAL, (uint)FIS_Dictionary.EDU_LEVEL)
            });

            foreach (uint v in eduLevelsList)
            {
                bool found = false;
                foreach (var r in oldList)
                    if ((uint)(r[0]) == v)
                        found = true;
                if (!found)
                    _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    { { "campaigns_id", _CampaignId }, { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_LEVEL }, { "dictionaries_items_item_id", v } });
            }

            foreach (var v in oldList)
            {
                bool found = false;
                foreach (uint r in eduLevelsList)
                {
                    if ((uint)(v[0]) == r)
                        found = true;
                }
                    if (!found)
                        _DB_Connection.Delete(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                        { { "campaigns_id", _CampaignId }, { "dictionaries_items_dictionary_id", (uint)FIS_Dictionary.EDU_LEVEL }, { "dictionaries_items_item_id", (uint)v[0] } });
                
            }
        }

        private void UpdateDirections()
        {
            List<object[]> missingDirections = new List<object[]>();
            List<object[]> oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[]
            { "direction_faculty", "direction_id", "places_budget_o", "places_budget_oz", "places_quota_o","places_quota_oz"},
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                });

            List<string[]> newList = new List<string[]>();
            foreach (DataGridViewRow r in dgvDirections.Rows)
                if (r.Index < dgvDirections.Rows.Count && (bool)r.Cells[1].Value)
                {
                    int[] places = new int[4];
                    for (int i = 5; i <= 8; i++)
                        if (dgvDirections.Columns[i].Visible == true)
                            places[i - 5] = int.Parse(r.Cells[i].Value.ToString());

                    newList.Add(new string[] { r.Cells[4].Value.ToString(), r.Cells[0].Value.ToString(), places[0].ToString(),
                   places[1].ToString(), places[2].ToString(), places[3].ToString()});
                }

            foreach (var v in oldList)
            {
                bool found = false;
                int index = 0;
                foreach (var r in newList)
                    if ((v[0].ToString() == r[0].ToString()) && (v[1].ToString() == r[1].ToString()))
                    {
                        found = true;
                        index = newList.FindIndex(x => (x[0] == v[0].ToString() && (x[1] == v[1].ToString())));
                        break;
                    }
                if (found)
                {
                    _DB_Connection.Update(DB_Table.CAMPAIGNS_DIRECTIONS_DATA,
                    new Dictionary<string, object> { { "places_budget_o", int.Parse(newList[index][2]) }, { "places_budget_oz", int.Parse(newList[index][3])},
                        {"places_quota_o", int.Parse(newList[index][4])}, { "places_quota_oz", int.Parse(newList[index][5])} },
                    new Dictionary<string, object> { { "campaign_id", _CampaignId }, { "direction_faculty", v[0].ToString() },
                        { "direction_id", v[1]} });
                    newList.RemoveAt(index);
                }
                else
                    _DB_Connection.Delete(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object> { { "campaign_id", _CampaignId }, { "direction_faculty", v[0].ToString() },
                    { "direction_id", (uint)v[1] } });
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object>
                { { "campaign_id", _CampaignId }, { "direction_faculty", v[0] }, { "direction_id", uint.Parse(v[1]) },
                    { "places_budget_o",int.Parse(v[2]) },{ "places_budget_oz", int.Parse(v[3])},
                    {"places_quota_o", int.Parse(v[4])}, { "places_quota_oz", int.Parse(v[5])}});
        }

        private void UpdateTargetOrganizations()
        {
            List<object[]> oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new string[] { "direction_faculty", "direction_id", "target_organization_id",
                "places_o", "places_oz" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                });
            List<string[]> newList = new List<string[]>();
            foreach (DataGridViewRow row in dgvTargetOrganizatons.Rows)
                if (row.Index < dgvTargetOrganizatons.Rows.Count - 2)
                    newList.Add(new string[] { row.Cells[6].Value.ToString(), row.Cells[3].Value.ToString(), row.Cells[0].Value.ToString(), row.Cells[8].Value.ToString(), row.Cells[9].Value.ToString() });
            int j = 0;
            while (j < oldList.Count)
            {
                bool keysMatch = false;
                bool valuesMatch = false;
                string[] item = new string[1];
                foreach (string[] newItem in newList)
                {
                    item = newItem;
                    if ((oldList[j][0].ToString() == newItem[0]) && (oldList[j][1].ToString() == newItem[1]) && (oldList[j][2].ToString() == newItem[2]))
                        keysMatch = true;
                    if ((oldList[j][3].ToString() == newItem[3]) && (oldList[j][4].ToString() == newItem[4]))
                        valuesMatch = true;
                    if (keysMatch)
                        break;
                }
                if (keysMatch && valuesMatch)
                {
                    oldList.RemoveAt(j);
                    newList.Remove(item);
                }
                else if (keysMatch && !valuesMatch)
                {
                    _DB_Connection.Update(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new Dictionary<string, object> { { "places_o",  int.Parse(item[3])},
                        { "places_oz", int.Parse(item[4]) } }, new Dictionary<string, object> { { "campaign_id", _CampaignId }, { "direction_faculty", oldList[j][0]},
                            { "direction_id", (uint)oldList[j][1] }, { "target_organization_id", (uint)oldList[j][2] } });
                    oldList.RemoveAt(j);
                    newList.Remove(item);
                }
                else
                    j++;
            }
            foreach (string[] record in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new Dictionary<string, object> {{ "campaign_id", _CampaignId },
                        { "direction_faculty", record[0]}, { "direction_id", uint.Parse(record[1]) }, { "target_organization_id", uint.Parse(record[2]) },
                        { "places_o",  uint.Parse(record[3])}, { "places_oz", uint.Parse(record[4]) } });
            foreach (object[] record in oldList)
                _DB_Connection.Delete(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new Dictionary<string, object> {{ "campaign_id", _CampaignId },
                        { "direction_faculty", record[0].ToString()}, { "direction_id", (uint)record[1] }, { "target_organization_id", (uint)record[2] }});
        }

        private void UpdateFaculties()
        {
            List<object[]> oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_FACULTIES_DATA, new string[]
           { "faculty_short_name", "hostel_places" }, new List<Tuple<string, Relation, object>>
           {
                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
           });

            List<string[]> newList = new List<string[]>();
            foreach (DataGridViewRow r in dgvFacultities.Rows)
                if (r.Index < dgvFacultities.Rows.Count - 1)
                    newList.Add(new string[] { r.Cells[0].Value.ToString(), r.Cells[2].Value.ToString() });

            foreach (var v in oldList)
            {
                bool found = false;
                int places = 0;
                foreach (var r in newList)
                    if (v[0].ToString() == r[0].ToString())
                    {
                        found = true;
                        places = int.Parse(r[1]);
                    }

                if (found)
                {
                    _DB_Connection.Update(DB_Table.CAMPAIGNS_FACULTIES_DATA,
                        new Dictionary<string, object> { { "hostel_places", places } },
                        new Dictionary<string, object> { { "campaign_id", _CampaignId }, { "faculty_short_name", v[0].ToString() } });
                    newList.RemoveAt(newList.FindIndex(x => x[0] == v[0].ToString()));
                }
                else
                    _DB_Connection.Delete(DB_Table.CAMPAIGNS_FACULTIES_DATA, new Dictionary<string, object> { { "campaign_id", _CampaignId }, { "faculty_short_name", v[0].ToString() } });
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_FACULTIES_DATA, new Dictionary<string, object>
                { { "faculty_short_name", v[0]}, { "hostel_places", int.Parse(v[1])}, { "campaign_id", _CampaignId} });
        }

        private void UpdateProfiles()
        {
            List<object[]> oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[]
            { "profiles_direction_faculty", "profiles_direction_id", "profiles_short_name",
                "places_paid_o","places_paid_oz", "places_paid_z"},
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId)
                });

            List<string[]> newList = new List<string[]>();
            foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                if (r.Index < dgvPaidPlaces.Rows.Count - 1)
                    newList.Add(new string[] { r.Cells[dgvPaidPlaces_Fac.Index].Value.ToString(), r.Cells[dgvPaidPlaces_ID.Index].Value.ToString(),
                        r.Cells[dgvPaidPlaces_ProfileShortName.Index].Value.ToString(), r.Cells[dgvPaidPlaces_OFPM.Index].Value.ToString(),
                        r.Cells[dgvPaidPlaces_OZFPM.Index].Value.ToString(), r.Cells[dgvPaidPlaces_ZFPM.Index].Value.ToString()});

            foreach (var v in oldList)
            {
                bool found = false;
                int index = 0;
                foreach (var r in newList)
                    if ((v[0].ToString() == r[0]) && (v[1].ToString() == r[1]) && (v[2].ToString() == r[2]))
                    {
                        found = true;
                        index = newList.FindIndex(x => ((x[0] == v[0].ToString()) && (x[1] == v[1].ToString()) && (x[2] == v[2].ToString())));
                        break;
                    }
                if (found)
                {
                    _DB_Connection.Update(DB_Table.CAMPAIGNS_PROFILES_DATA,
                    new Dictionary<string, object> { { "places_paid_o", int.Parse(newList[index][3]) }, { "places_paid_oz", int.Parse(newList[index][4])},
                        { "places_paid_z", int.Parse(newList[index][5])}},
                    new Dictionary<string, object> { { "campaigns_id", _CampaignId }, { "profiles_direction_faculty", v[0].ToString() },
                        { "profiles_direction_id", v[1]}, { "profiles_short_name", v[2].ToString()} });
                    newList.RemoveAt(index);
                }
                else
                    _DB_Connection.Delete(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object> { { "campaigns_id", _CampaignId }, { "profiles_direction_faculty", v[0].ToString() },
                        { "profiles_direction_id", v[1]}, { "profiles_short_name", v[2].ToString()} });
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object>
                { { "campaigns_id", _CampaignId }, { "profiles_direction_faculty", v[0] }, { "profiles_direction_id", int.Parse(v[1]) },
                    { "profiles_short_name", v[2]}, { "places_paid_o", int.Parse(v[3]) },{ "places_paid_oz", int.Parse(v[4])},
                    { "places_paid_z",int.Parse(v[5])}});
        }

        private void UpdateEntranceTests()
        {
            if (!cbEduLevelMag.Checked)
            {
                List<object[]> oldList = _DB_Connection.Select(DB_Table.ENTRANCE_TESTS, new string[]
                { "direction_id", "subject_dict_id", "subject_id", "priority", "direction_faculty"}, new List<Tuple<string, Relation, object>>
                {
                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                });

                List<string[]> newList = new List<string[]>();
                foreach (DataGridViewRow r in dgvEntranceTests.Rows)
                {
                    newList.Add(new string[] { r.Cells[0].Value.ToString(), "1",
                    _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, r.Cells[4].Value.ToString()).ToString(), "1", r.Cells[3].Value.ToString()});

                    newList.Add(new string[] { r.Cells[0].Value.ToString(), "1",
                    _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, r.Cells[5].Value.ToString()).ToString(), "2", r.Cells[3].Value.ToString()});

                    newList.Add(new string[] { r.Cells[0].Value.ToString(), "1",
                    _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, r.Cells[6].Value.ToString()).ToString(), "3", r.Cells[3].Value.ToString()});
                }

                foreach (var v in oldList)
                {
                    bool found = false;
                    foreach (var r in newList)
                        if ((v[0].ToString() == r[0]) && (v[1].ToString() == r[1]) && (v[2].ToString() == r[2]) && (v[3].ToString() == r[3]) && (v[4].ToString() == r[4]))
                        {
                            _DB_Connection.Update(DB_Table.ENTRANCE_TESTS,
                            new Dictionary<string, object> { { "priority", int.Parse(r[3]) } },
                            new Dictionary<string, object> { { "direction_id", v[0] }, { "subject_dict_id", v[1] },
                            { "subject_id", v[2]}, { "campaign_id", _CampaignId}, { "direction_faculty", v[4] } });
                            newList.Remove(r);
                            found = true;
                            break;
                        }
                    if (!found)
                        _DB_Connection.Delete(DB_Table.ENTRANCE_TESTS, new Dictionary<string, object> { { "direction_id", (uint)(v[0]) },
                        { "subject_dict_id", (uint)(v[1]) }, { "subject_id", (uint)(v[2]) },
                    { "priority", v[3]}, { "campaign_id", _CampaignId}, { "direction_faculty", v[4]} });
                }

                foreach (var v in newList)
                    _DB_Connection.Insert(DB_Table.ENTRANCE_TESTS, new Dictionary<string, object>
                { { "direction_id", uint.Parse(v[0]) }, { "subject_dict_id", uint.Parse(v[1]) }, { "subject_id", uint.Parse(v[2]) },
                    { "priority", int.Parse(v[3])}, { "campaign_id", _CampaignId}, { "direction_faculty", v[4]} });
            }
        }
    }
}
