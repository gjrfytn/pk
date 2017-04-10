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

            ButtonsAppearanceChange(0);

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
            ButtonsAppearanceChange(dgvTargetOrganizatons.Rows.Count-1);
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
                    dgvTargetOrganizatons.CurrentRow.Cells[8].ReadOnly = false;
                    dgvTargetOrganizatons.CurrentRow.Cells[9].ReadOnly = false;
                    break;
            }
        }

        private void dgvPaidPlaces_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateProfiliesTable();
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
            if (cbEduFormO.Checked)
            {
                dgvDirections_OOOF.Visible = true;
                dgvDirections_OKOF.Visible = true;
            }
            else
            {
                dgvDirections_OOOF.Visible = false;
                dgvDirections_OKOF.Visible = false;
            }
            if (cbEduFormOZ.Checked)
            {
                dgvDirections_OOOZF.Visible = true;
                dgvDirections_OKOZF.Visible = true;
            }
            else
            {
                dgvDirections_OOOZF.Visible = false;
                dgvDirections_OKOZF.Visible = false;
            }
            if (cbEduFormZ.Checked)
            {
                
            }
            else
            {
                
            }
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ((sender as DataGridView).CurrentCell.Value.ToString() == "")
                (sender as DataGridView).CurrentCell.Value = 0;
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
        }

        private void ButtonsAppearanceChange(int rowindex)
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
                            dgvDirections.Rows.Add(v[2].ToString(),true, v[0].ToString(), v[1].ToString(), r[0].ToString(), 0, 0, 0, 0, 0, 0, 0, 0);

                else if ((cbEduLevelSpec.Checked)&&(v[1].ToString().Substring(3, 2) == "05")&&(r[1].ToString() == v[2].ToString()))
                        dgvDirections.Rows.Add(v[2].ToString(), true, v[0].ToString(), v[1].ToString(), r[0].ToString(), 0, 0, 0, 0, 0, 0, 0, 0);

                else if ((cbEduLevelMag.Checked)&&(v[1].ToString().Substring(3, 2) == "04")&&(r[1].ToString() == v[2].ToString()))
                            dgvDirections.Rows.Add(v[2].ToString(), true, v[0].ToString(), v[1].ToString(), r[0].ToString(), 0, 0, 0, 0, 0, 0, 0, 0);

            dgvDirections.Sort(dgvDirections.Columns[1], ListSortDirection.Ascending);
        }

        private void FillProfiliesTable ()
        {
            dgvPaidPlaces.Rows.Clear();
            foreach (DataGridViewRow r in dgvDirections.Rows)
            {
                bool found = false;
                foreach (DataGridViewRow v in dgvPaidPlaces.Rows)
                    if (v.Cells[2].Value.ToString() == r.Cells[0].Value.ToString())
                        found = true;
                if (!found)
                    dgvPaidPlaces.Rows.Add(false, "Н", r.Cells[0].Value.ToString(), r.Cells[2].Value.ToString(), 
                        r.Cells[3].Value.ToString(), r.Cells[4].Value.ToString(), 0, 0, 0);
                for (int i = 0; i < dgvPaidPlaces.Rows[dgvPaidPlaces.Rows.Count - 1].Cells.Count; i++)
                {
                    dgvPaidPlaces.Rows[dgvPaidPlaces.Rows.Count - 1].Cells[i].Style.Font = new Font(
                        dgvPaidPlaces.Font.Name.ToString(),
                        dgvPaidPlaces.Font.Size + 1,
                        FontStyle.Bold);
                    dgvPaidPlaces.Rows[dgvPaidPlaces.Rows.Count - 1].Cells[i].Style.BackColor = Color.LightGray;
                }
            }
            foreach (var v in _DB_Connection.Select(DB_Table.PROFILES, "name", "direction_id", "faculty_short_name"))
                for (int i = 0; i < dgvPaidPlaces.Rows.Count; i++)
                {
                    if ((dgvPaidPlaces.Rows[i].Cells[1].Value.ToString() == "Н")
                        && (dgvPaidPlaces.Rows[i].Cells[2].Value.ToString() == v[1].ToString()))
                        dgvPaidPlaces.Rows.Insert(i + 1, true, "П", dgvPaidPlaces.Rows[i].Cells[2].Value.ToString(), v[0].ToString(), 
                            dgvPaidPlaces.Rows[i].Cells[4].Value.ToString(), v[2].ToString(), 0, 0, 0);
                }
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
        }

        private void UpdateProfiliesTable()
        {
            foreach (DataGridViewRow v in dgvPaidPlaces.Rows)
            {
                if (v.Cells[1].Value.ToString() == "Н")
                {
                    for (int i = 6; i <= 8; i++)
                        v.Cells[i].Value = 0;

                    foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                        if (v.Cells[2].Value.ToString() == r.Cells[2].Value.ToString())
                            for (int i = 6; i <= 8; i++)
                                if (r.Cells[i].Value.ToString() == "")
                                    r.Cells[i].Value = 0;
                                else
                                    v.Cells[i].Value = (int.Parse(v.Cells[i].Value.ToString()) + int.Parse(r.Cells[i].Value.ToString())).ToString();
                }
            }
        }

        private void FillDistTable ()
        {
            dgvEntranceTests.Rows.Clear();
            foreach (DataGridViewRow r in dgvDirections.Rows)               
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

        private void UpdateEduForms()
        {
            List<object[]> eduFormsList = _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[]
                { "dictionaries_items_item_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId),
                        new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, 14)
                    });

            foreach (CheckBox chekBox in gbEduForms.Controls)
                if (chekBox.Checked)
                {
                    bool found = false;
                    foreach (object[] listElement in eduFormsList)
                        if ((uint)listElement[0] == _DB_Helper.GetDictionaryItemID(14, chekBox.Text))
                            found = true;
                    if (!found)
                        _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                        { { "campaigns_id", _CampaignId}, {"dictionaries_items_dictionary_id", 14},
                            { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(14, chekBox.Text) } });
                }
            foreach (object[] eduForm in eduFormsList)
            {
                bool found = false;
                foreach (CheckBox chekBox in gbEduForms.Controls)
                    if (((uint)eduForm[0] == _DB_Helper.GetDictionaryItemID(14, chekBox.Text)) && chekBox.Checked)
                        found = true;
                if (!found)
                    _DB_Connection.Delete(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object> { { "campaigns_id", _CampaignId },
                        { "dictionaries_items_dictionary_id", 14 }, { "dictionaries_items_item_id",  (uint)eduForm[0]} });
            }
        }

        private void UpdateEduLevels()
        {
            List<uint> eduLevelsList = new List<uint>();

            if (cbEduLevelBacc.Checked)
                eduLevelsList.Add(_DB_Helper.GetDictionaryItemID(2, "Бакалавриат"));

            if (cbEduLevelMag.Checked)
                eduLevelsList.Add(_DB_Helper.GetDictionaryItemID(2, "Магистратура"));

            if (cbEduLevelSpec.Checked)
                eduLevelsList.Add(_DB_Helper.GetDictionaryItemID(2, "Специалитет"));

            List<object[]> oldList = _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[]
            { "dictionaries_items_item_id", }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId),
                new Tuple<string, Relation, object>("dictionaries_items_dictionary_id", Relation.EQUAL, 2)
            });

            foreach (uint v in eduLevelsList)
            {
                bool found = false;
                foreach (var r in oldList)
                    if ((uint)(r[0]) == v)
                        found = true;
                if (!found)
                    _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    { { "campaigns_id", _CampaignId }, { "dictionaries_items_dictionary_id", 2 }, { "dictionaries_items_item_id", v } });
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
                        { { "campaigns_id", _CampaignId }, { "dictionaries_items_dictionary_id", 2 }, { "dictionaries_items_item_id", (uint)v[0] } });
                
            }
        }

        private List<object[]> UpdateDirections()
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
                if ((bool)r.Cells[1].Value)
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
                    missingDirections.Add(new object[]{ v[0].ToString(), v[1] });
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object>
                { { "campaign_id", _CampaignId }, { "direction_faculty", v[0] }, { "direction_id", uint.Parse(v[1]) },
                    { "places_budget_o",int.Parse(v[2]) },{ "places_budget_oz", int.Parse(v[3])},
                    {"places_quota_o", int.Parse(v[4])}, { "places_quota_oz", int.Parse(v[5])}});
            return missingDirections;
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
                if (row.Index < dgvTargetOrganizatons.Rows.Count - 1)
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
                        { "places_oz", int.Parse(item[4]) } }, new Dictionary<string, object> { { "campaign_id", _CampaignId }, { "direction_faculty", (uint)oldList[j][0]},
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
            { "profiles_direction_faculty", "profiles_direction_id", "profiles_name",
                "places_paid_o","places_paid_oz", "places_paid_z"},
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId)
                });

            List<string[]> newList = new List<string[]>();
            foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                if (((bool)r.Cells[0].Value) && (r.Cells[1].Value.ToString() == "П"))
                    newList.Add(new string[] { r.Cells[5].Value.ToString(), r.Cells[2].Value.ToString(), r.Cells[3].Value.ToString(),
                        r.Cells[6].Value.ToString(), r.Cells[7].Value.ToString(), r.Cells[8].Value.ToString()});

            foreach (var v in oldList)
            {
                bool found = false;
                int index = 0;
                foreach (var r in newList)
                    if ((v[0].ToString() == r[0]) && (v[1].ToString() == r[1]) && (v[2].ToString() == r[2]))
                    {
                        found = true;
                        index = newList.FindIndex(x => (x[0] == v[0].ToString() && (x[1] == v[1].ToString())));
                        break;
                    }
                if (found)
                {
                    _DB_Connection.Update(DB_Table.CAMPAIGNS_PROFILES_DATA,
                    new Dictionary<string, object> { { "places_paid_o", int.Parse(newList[index][3]) }, { "places_paid_oz", int.Parse(newList[index][4])},
                        { "places_paid_z", int.Parse(newList[index][5])}},
                    new Dictionary<string, object> { { "campaigns_id", _CampaignId }, { "profiles_direction_faculty", v[0].ToString() },
                        { "profiles_direction_id", v[1]}, { "profiles_name", v[2].ToString()} });
                    newList.RemoveAt(index);
                }
                else
                    _DB_Connection.Delete(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object> { { "campaigns_id", _CampaignId }, { "profiles_direction_faculty", v[0].ToString() },
                        { "profiles_direction_id", v[1]}, { "profiles_name", v[2].ToString()} });
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object>
                { { "campaigns_id", _CampaignId }, { "profiles_direction_faculty", v[0] }, { "profiles_direction_id", int.Parse(v[1]) },
                    { "profiles_name", v[2]}, { "places_paid_o", int.Parse(v[3]) },{ "places_paid_oz", int.Parse(v[4])},
                    { "places_paid_z",int.Parse(v[5])}});
        }

        private void UpdateEntranceTests()
        {
            List<object[]> oldList = _DB_Connection.Select(DB_Table.ENTRANCE_TESTS, new string[]
            { "direction_id", "subject_dict_id", "subject_id", "priority", "direction_faculty"}, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
            });

            List<string[]> newList = new List<string[]>();
            foreach (DataGridViewRow r in dgvEntranceTests.Rows)
            {
                newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Helper.GetDictionaryItemID((uint)oldList[0][1], r.Cells[4].Value.ToString()).ToString(), "1", r.Cells[3].Value.ToString()});

                newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Helper.GetDictionaryItemID((uint)oldList[0][1], r.Cells[5].Value.ToString()).ToString(), "2", r.Cells[3].Value.ToString()});

                newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Helper.GetDictionaryItemID((uint)oldList[0][1], r.Cells[6].Value.ToString()).ToString(), "3", r.Cells[3].Value.ToString()});
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

        private void SaveEduForms()
        {

        }

        private void SaveEduLevels()
        {

        }

        private void SaveDirections()
        {

        }

        private void SaveTargetOrganizations()
        {

        }

        private void SaveFaculties()
        {

        }

        private void SaveProfiles()
        {

        }

        private void SaveEntranceTests()
        {

        }

        private void LoadEduForms()
        {
            foreach (var v in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, 14),
                    new Tuple<string, Relation, object> ("campaigns_id", Relation.EQUAL, _CampaignId)
                }))
            {
                foreach (CheckBox chekBox in gbEduForms.Controls)
                    if (chekBox.Text == _DB_Helper.GetDictionaryItemName(14, (uint)v[0]))
                        chekBox.Checked = true;
            }
        }

        private void LoadEduLevels()
        {
            foreach (var v in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
            new List<Tuple<string, Relation, object>>
            {
                                new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, 2),
                                new Tuple<string, Relation, object> ("campaigns_id", Relation.EQUAL, _CampaignId)
            }))
            {
                string itemName = _DB_Helper.GetDictionaryItemName(2, (uint)v[0]);

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
                    if ((v[0].ToString() == r.Cells[4].Value.ToString()) && (v[1].ToString() == r.Cells[0].Value.ToString()))
                    {
                        r.Cells[1].Value = true;
                        for (int i = 5; i < r.Cells.Count; i++)
                            r.Cells[i].Value = v[i - 3].ToString();
                    }
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
                "", record[1], _DB_Helper.GetDirectionsDictionaryNameAndCode((uint)record[1])[0],
                _DB_Helper.GetDirectionsDictionaryNameAndCode((uint)record[1])[1], record[0], "", record[3], record[4]);
                ButtonsAppearanceChange(dgvTargetOrganizatons.Rows.Count - 2);
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
                    if (r.Cells[0].Value.ToString() == v[0].ToString())
                        r.Cells[2].Value = v[1].ToString();
        }

        private void LoadProfiles()
        {
            foreach (var v in _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_faculty",
                    "profiles_direction_id", "profiles_name", "places_paid_o", "places_paid_oz", "places_paid_z" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId)
                    }
                    ))
                foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                    if ((v[0].ToString() == r.Cells[5].Value.ToString()) && (v[1].ToString() == r.Cells[2].Value.ToString()) && (v[2].ToString() == r.Cells[3].Value.ToString()))
                        for (int i = 6; i < r.Cells.Count; i++)
                            r.Cells[i].Value = v[i - 3].ToString();
            UpdateProfiliesTable();
        }

        private void LoadEntranceTests()
        {
            foreach (var v in _DB_Connection.Select(DB_Table.ENTRANCE_TESTS, new string[] { "direction_id", "subject_dict_id", "subject_id", "priority", "direction_faculty" },
                new List<Tuple<string, Relation, object>>
                {
                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                }))
                foreach (DataGridViewRow r in dgvEntranceTests.Rows)
                    if ((v[0].ToString() == r.Cells[0].Value.ToString()) && (v[4].ToString() == r.Cells[3].Value.ToString()))
                        r.Cells[3 + Convert.ToInt32(v[3])].Value = _DB_Helper.GetDictionaryItemName(1, (uint)v[2]);
        }

        private void SaveCampaign()
        {
            uint campaignId = _DB_Connection.Insert(DB_Table.CAMPAIGNS, new Dictionary<string, object>
                        {
                            { "name", tbName.Text }, { "start_year",  Convert.ToInt32(cbStartYear.SelectedItem)},
                            { "end_year", Convert.ToInt32(cbEndYear.SelectedItem) },
                            { "status_dict_id",  34}, { "status_id", _DB_Helper.GetDictionaryItemID(34, cbState.SelectedItem.ToString()) },                    
                            { "type_dict_id",  38}, { "type_id", _DB_Helper.GetDictionaryItemID(38, cbType.SelectedItem.ToString()) } });
            _CampaignId = campaignId;

            if (cbEduFormO.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    {
                        { "campaigns_id", campaignId},
                        { "dictionaries_items_dictionary_id", 14},
                        { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(14, "Очная форма") } });
            if (cbEduFormOZ.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    {
                        { "campaigns_id", campaignId},
                        { "dictionaries_items_dictionary_id", 14},
                        { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(14, "Очно-заочная (вечерняя)") } });
            if (cbEduFormZ.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    {
                        { "campaigns_id", campaignId},
                        { "dictionaries_items_dictionary_id", 14},
                        { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(14, "Заочная форма") } });                

            if (cbEduLevelBacc.Checked)
                    _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_id", campaignId},
                    { "dictionaries_items_dictionary_id", 2},
                    { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(2, "Бакалавриат") } });

            if (cbEduLevelMag.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_id", campaignId},
                    { "dictionaries_items_dictionary_id", 2},
                    { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(2, "Магистратура") } });

            if (cbEduLevelSpec.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_id", campaignId},
                    { "dictionaries_items_dictionary_id", 2},
                    { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(2, "Специалитет") } });

            foreach (DataGridViewRow r in dgvDirections.Rows)
            {
                int[] places = new int[5];
                for (int i = 5; i <= 8; i++)
                    if (dgvDirections.Columns[i].Visible == true)
                        places[i - 5] = int.Parse(r.Cells[i].Value.ToString());

                _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object>
                {
                    { "campaign_id", campaignId},
                    { "direction_faculty", r.Cells[4].Value.ToString()},
                    { "direction_id", r.Cells[0].Value},
                    { "places_budget_o", places[0]},
                    { "places_budget_oz", places[1]},
                    { "places_quota_o", places[3]},
                    { "places_quota_oz", places[4]}});
            }                

            foreach (DataGridViewRow row in dgvTargetOrganizatons.Rows)
            {
                if (row.Index < dgvTargetOrganizatons.Rows.Count-1)
                    _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA, new Dictionary<string, object> { { "campaign_id", _CampaignId},
                        { "direction_faculty", row.Cells[6].Value}, { "direction_id", row.Cells[3].Value}, { "target_organization_id", row.Cells[0].Value},
                        { "places_o", row.Cells[8].Value}, { "places_oz",row.Cells[9].Value }, { "places_z", 0} });
            }

            foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                if ((r.Cells[1].Value.ToString()=="П")&&((bool)r.Cells[0].Value == true))
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object>
                {
                    { "campaigns_id", campaignId},
                    { "profiles_direction_faculty", r.Cells[5].Value.ToString()},
                    { "profiles_direction_id", r.Cells[2].Value},
                    { "profiles_name", r.Cells[3].Value},
                    { "places_paid_o", r.Cells[6].Value},
                    { "places_paid_oz", r.Cells[7].Value},
                    { "places_paid_z", r.Cells[8].Value}
                });

            foreach (DataGridViewRow r in dgvFacultities.Rows)

                _DB_Connection.Insert(DB_Table.CAMPAIGNS_FACULTIES_DATA, new Dictionary<string, object>
                {
                    { "campaign_id", campaignId},
                    { "faculty_short_name", r.Cells[0].Value.ToString()},
                    { "hostel_places", r.Cells[2].Value}
                });
            
                foreach (DataGridViewRow r in dgvEntranceTests.Rows)
                    for (int i = 1; i < r.Cells.Count-3; i++)
                    {
                    bool found = false;
                    uint subjectId = _DB_Helper.GetDictionaryItemID(1, r.Cells[i + 3].Value.ToString());

                    foreach (var v in _DB_Connection.Select(DB_Table.ENTRANCE_TESTS,
                        new string[] { "priority", "direction_id", "subject_dict_id", "subject_id", "direction_faculty" },
                        new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL,_CampaignId),
                                new Tuple<string, Relation, object>("direction_faculty", Relation.EQUAL, r.Cells[3].Value),
                                new Tuple<string, Relation, object> ("direction_id", Relation.EQUAL, r.Cells[0].Value),
                                new Tuple<string, Relation, object> ("subject_dict_id", Relation.EQUAL, 1),
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
                            { "subject_dict_id", 1 },
                            { "subject_id", subjectId },
                            { "priority", i }
                        });               
                }
        }

        private void UpdateCampaing()
        {
            _DB_Connection.Update(DB_Table.CAMPAIGNS, new Dictionary<string, object>
                        {   { "name", tbName.Text }, { "start_year",  int.Parse(cbStartYear.SelectedItem.ToString())},
                            { "end_year", int.Parse(cbEndYear.SelectedItem.ToString()) },
                            { "status_dict_id",  34}, { "status_id", _DB_Helper.GetDictionaryItemID(34, cbState.SelectedItem.ToString()) },
                            { "type_dict_id",  38}, { "type_id", _DB_Helper.GetDictionaryItemID(38, cbType.SelectedItem.ToString()) } },
                        new Dictionary<string, object>
                        {
                            { "id", _CampaignId}
                        });
            
            UpdateEduForms();
            UpdateEduLevels();
            List<object[]> removingDirections = UpdateDirections();
            UpdateTargetOrganizations();
            UpdateFaculties();            
            UpdateProfiles();
            UpdateEntranceTests();
            foreach (object[] direction in removingDirections)
                _DB_Connection.Delete(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object> { { "campaign_id", _CampaignId },
                        { "direction_faculty", direction[0].ToString() }, { "direction_id", (uint)direction[1]} });
        }

        private void LoadCampaign()
        {
            List<object> loadedCampaign = new List<object>();
            loadedCampaign.AddRange( _DB_Connection.Select(DB_Table.CAMPAIGNS, 
                new string[] { "name", "start_year", "end_year", "status_dict_id", "status_id", "type_dict_id", "type_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, _CampaignId)
                })[0]);

            tbName.Text = loadedCampaign[0].ToString();
            cbStartYear.SelectedItem = loadedCampaign[1].ToString();
            cbEndYear.SelectedItem = loadedCampaign[2].ToString();

            cbState.SelectedItem = _DB_Helper.GetDictionaryItemName((uint)loadedCampaign[3], (uint)loadedCampaign[4]);

            cbType.SelectedItem = _DB_Helper.GetDictionaryItemName((uint)loadedCampaign[5], (uint)loadedCampaign[6]);

            LoadEduForms();
            LoadEduLevels();
       }

        private void LoadTables()
        {
            LoadDirections();
            LoadTargetOrganizations();
            LoadFaculties();
            LoadProfiles();
            LoadEntranceTests();
        }        
    }
}
