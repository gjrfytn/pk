using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class CampaignEdit : Form
    {
        Classes.DB_Connector _DB_Connection;
        Classes.DB_Helper _DB_Helper;
        uint? _CampaignId;

        public CampaignEdit(uint? campUid)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector();
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

            foreach (var v in _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 14)
            }))
                lbEduFormsAll.Items.Add(v[0].ToString());

            if (_CampaignId.HasValue)
            {
                LoadCampaign();
                LoadTables();
            }
                            
        }

        void ButtonsAppearanceChange(int rowindex)
        {

            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.Rows.Count - 1].Cells[2].Style.Font = new Font("ESSTIXTwo", 11);
            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.Rows.Count - 1].Cells[2].Value = "=";

            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.Rows.Count - 1].Cells[6].Style.Font = new Font("ESSTIXTwo", 11);
            dgvTargetOrganizatons.Rows[dgvTargetOrganizatons.Rows.Count - 1].Cells[6].Value = "=";

        }

        void FillDirectionsTable()
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

        void FillProfiliesTable ()
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

        void FillFacultiesTable ()
        {
            dgvFacultities.Rows.Clear();
            foreach (var v in _DB_Connection.Select(DB_Table.FACULTIES, "short_name", "name"))
            {
                bool found = false;
                foreach (DataGridViewRow r in dgvDirections.Rows)
                    if (v[0].ToString() == r.Cells[4].Value.ToString())
                    {
                        found = true;
                    }
                if (found)
                dgvFacultities.Rows.Add(v[0].ToString(), v[1].ToString(), 0);
            }                
            dgvFacultities.Sort(dgvFacultities.Columns[1], ListSortDirection.Ascending);
        }

        void UpdateProfiliesTable()
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
                                v.Cells[i].Value = (Convert.ToInt32(v.Cells[i].Value.ToString()) + Convert.ToInt32(r.Cells[i].Value.ToString())).ToString();
                }
            }
        }

        void FillDistTable ()
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

        void SaveCampaign()
        {
            uint campaignId = _DB_Connection.Insert(DB_Table.CAMPAIGNS, new Dictionary<string, object>
                        {
                            { "name", tbName.Text }, { "start_year",  Convert.ToInt32(cbStartYear.SelectedItem)},
                            { "end_year", Convert.ToInt32(cbEndYear.SelectedItem) },
                            { "status_dict_id",  34}, { "status_id", _DB_Helper.GetDictionaryItemID(34, cbState.SelectedItem.ToString()) },                    
                            { "type_dict_id",  38}, { "type_id", _DB_Helper.GetDictionaryItemID(38, cbType.SelectedItem.ToString()) } });
            _CampaignId = campaignId;

            foreach (var v in lbEduFormsChoosen.Items)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_id", campaignId},
                    { "dictionaries_items_dictionary_id", 14},
                    { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(14, v.ToString()) } });

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
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object>
                {
                    { "campaign_id", campaignId},
                    { "direction_faculty", r.Cells[4].Value.ToString()},
                    { "direction_id", r.Cells[0].Value},
                    { "places_budget_o", r.Cells[5].Value},
                    { "places_budget_oz", r.Cells[6].Value},
                    { "places_budget_z", r.Cells[7].Value},
                    { "places_target_o", r.Cells[11].Value},
                    { "places_target_oz", r.Cells[12].Value},
                    { "places_target_z", 0},
                    { "places_quota_o", r.Cells[8].Value},
                    { "places_quota_oz", r.Cells[9].Value},
                    { "places_quota_z", r.Cells[10].Value}
                });

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

        void UpdateCampaing()
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

            List<object[]> eduFormsList = _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[]
                { "dictionaries_items_item_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId),
                        new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, 14)
                    });

            foreach (var v in lbEduFormsChoosen.Items)
            {
                bool found = false;
                foreach (var r in eduFormsList)
                    if ((uint)r[0] == _DB_Helper.GetDictionaryItemID(14, v.ToString()))
                        found = true;

                if (!found)
                {
                    _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    { { "campaigns_id", _CampaignId}, {"dictionaries_items_dictionary_id", 14},
                        { "dictionaries_items_item_id", _DB_Helper.GetDictionaryItemID(14, v.ToString()) } });
                }                    
            }
            foreach (var v in eduFormsList)
            {
                bool found = false;
                foreach (var r in lbEduFormsChoosen.Items)
                    if ((uint)v[0] == _DB_Helper.GetDictionaryItemID(14, r.ToString()))
                        found = true;
                if (!found)
                    _DB_Connection.Delete(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object> { { "campaigns_id", _CampaignId },
                        { "dictionaries_items_dictionary_id", 14 }, { "dictionaries_items_item_id", v[0] } });
            }

            List<uint> eduLevelsList = new List<uint>();

            if (cbEduLevelBacc.Checked)
                eduLevelsList.Add( _DB_Helper.GetDictionaryItemID(2, "Бакалавриат"));

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

                    if (!found)
                        _DB_Connection.Delete(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                        { { "campaigns_id", _CampaignId }, { "dictionaries_items_dictionary_id", 2 }, { "dictionaries_items_item_id", v } });
                }
            }


            oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_FACULTIES_DATA, new string[]
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
                        places = Convert.ToInt32(r[1]);
                    }

                if (found)
                {
                    _DB_Connection.Update(DB_Table.CAMPAIGNS_FACULTIES_DATA,
                        new Dictionary<string, object> { { "hostel_places", places } },
                        new Dictionary<string, object> { { "campaign_id", _CampaignId }, { "faculty_short_name", v[0].ToString() } });
                    newList.RemoveAt(newList.FindIndex(x => x[0] == v[0].ToString()));
                }
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_FACULTIES_DATA, new Dictionary<string, object>
                { { "faculty_short_name", v[0]}, { "hostel_places", int.Parse(v[1])}, { "campaign_id", _CampaignId} });


            oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[]
            { "direction_faculty", "direction_id", "places_budget_o", "places_budget_oz","places_budget_z","places_target_o",
                "places_target_oz","places_target_z","places_quota_o","places_quota_oz","places_quota_z"},
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                });

            newList.Clear();
            foreach (DataGridViewRow r in dgvDirections.Rows)
                if ((bool)r.Cells[1].Value)
                newList.Add(new string[] { r.Cells[4].Value.ToString(), r.Cells[0].Value.ToString(), r.Cells[5].Value.ToString(),
                    r.Cells[6].Value.ToString(), r.Cells[7].Value.ToString(), r.Cells[11].Value.ToString(), r.Cells[12].Value.ToString(),
                    "0", r.Cells[8].Value.ToString(), r.Cells[9].Value.ToString(), r.Cells[10].Value.ToString() });

            foreach (var v in oldList)
            {
                foreach (var r in newList)
                    if ((v[0].ToString() == r[0].ToString())&&(v[1].ToString() == r[1].ToString()))
                    {
                        _DB_Connection.Update(DB_Table.CAMPAIGNS_DIRECTIONS_DATA,
                        new Dictionary<string, object> { { "places_budget_o", int.Parse(r[2]) }, { "places_budget_oz", int.Parse(r[3])},
                            { "places_budget_z", int.Parse(r[4])}, { "places_target_o", int.Parse(r[5])},
                            { "places_target_oz", int.Parse(r[6])}, { "places_target_z", 0 }, {"places_quota_o", int.Parse(r[8])},
                            { "places_quota_oz", int.Parse(r[9])}, { "places_quota_z", int.Parse(r[10])} },
                        new Dictionary<string, object> { { "campaign_id", _CampaignId }, { "direction_faculty", v[0].ToString() },
                            { "direction_id", v[1]} });
                        newList.RemoveAt(newList.FindIndex(x => (x[0] == v[0].ToString()&&(x[1] == v[1].ToString()))));
                        break;
                    }
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object>
                { { "campaign_id", _CampaignId }, { "direction_faculty", v[0] }, { "direction_id", uint.Parse(v[1]) },
                    { "places_budget_o",int.Parse(v[2]) },{ "places_budget_oz", int.Parse(v[3])},
                    { "places_budget_z", int.Parse(v[4])}, { "places_target_o", int.Parse(v[5])},
                    { "places_target_oz", int.Parse(v[6])}, { "places_target_z", 0 }, {"places_quota_o", int.Parse(v[8])},
                    { "places_quota_oz", int.Parse(v[9])}, { "places_quota_z", int.Parse(v[10])}});


            oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[]
            { "profiles_direction_faculty", "profiles_direction_id", "profiles_name",
                "places_paid_o","places_paid_oz", "places_paid_z"},
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaigns_id", Relation.EQUAL, _CampaignId)
                });

            newList.Clear();
            foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                if (((bool)r.Cells[0].Value)&&(r.Cells[1].Value.ToString()=="П"))
                    newList.Add(new string[] { r.Cells[5].Value.ToString(), r.Cells[2].Value.ToString(), r.Cells[3].Value.ToString(),
                        r.Cells[6].Value.ToString(), r.Cells[7].Value.ToString(), r.Cells[8].Value.ToString()});

            foreach (var v in oldList)
            {
                foreach (var r in newList)
                    if ((v[0].ToString() == r[0]) && (v[1].ToString() == r[1])&&(v[2].ToString() == r[2]))
                    {
                        _DB_Connection.Update(DB_Table.CAMPAIGNS_PROFILES_DATA,
                        new Dictionary<string, object> { { "places_paid_o", int.Parse(r[3]) }, { "places_paid_oz", int.Parse(r[4])},
                            { "places_paid_z", int.Parse(r[5])}},
                        new Dictionary<string, object> { { "campaigns_id", _CampaignId }, { "profiles_direction_faculty", v[0].ToString() },
                            { "profiles_direction_id", v[1]}, { "profiles_name", v[2].ToString()} });
                        newList.RemoveAt(newList.FindIndex(x => (x[0] == v[0].ToString() && (x[1] == v[1].ToString())&&(x[2] == v[2].ToString()))));
                        break;
                    }
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object>
                { { "campaigns_id", _CampaignId }, { "profiles_direction_faculty", v[0] }, { "profiles_direction_id", int.Parse(v[1]) },
                    { "profiles_name", v[2]}, { "places_paid_o", int.Parse(v[3]) },{ "places_paid_oz", int.Parse(v[4])},
                    { "places_paid_z",int.Parse(v[5])}});


            oldList = _DB_Connection.Select(DB_Table.ENTRANCE_TESTS, new string[]
            { "direction_id", "subject_dict_id", "subject_id", "priority", "campaign_id", "direction_faculty"});

            newList.Clear();
            foreach (DataGridViewRow r in dgvEntranceTests.Rows)
            {
                newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Helper.GetDictionaryItemID((uint)oldList[0][1], r.Cells[4].Value.ToString()).ToString(), "1", _CampaignId.ToString(), r.Cells[3].Value.ToString()});

                newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Helper.GetDictionaryItemID((uint)oldList[0][1], r.Cells[5].Value.ToString()).ToString(), "2", _CampaignId.ToString(), r.Cells[3].Value.ToString()});

                newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Helper.GetDictionaryItemID((uint)oldList[0][1], r.Cells[6].Value.ToString()).ToString(), "3", _CampaignId.ToString(), r.Cells[3].Value.ToString()});
            }                

            foreach (var v in oldList)
            {
                bool found = false;
                foreach (var r in newList)
                    if ((v[0].ToString() == r[0]) && (v[1].ToString() == r[1]) && (v[2].ToString() == r[2])&&(v[4].ToString()==r[4])&&(v[5].ToString()==r[5]))
                    {
                        _DB_Connection.Update(DB_Table.ENTRANCE_TESTS,
                        new Dictionary<string, object> { { "priority", int.Parse(r[3]) } },
                        new Dictionary<string, object> { { "direction_id", v[0] }, { "subject_dict_id", v[1] },
                            { "subject_id", v[2]}, { "campaign_id", _CampaignId}, { "direction_faculty", v[5] } });
                        newList.RemoveAt(newList.FindIndex(x => (x[0] == v[0].ToString() && (x[1] == v[1].ToString()) && (x[2] == v[2].ToString()) && (v[4].ToString() == x[4]) && (v[5].ToString() == x[5]))));
                        found = true;
                        break;
                    }
                if (!found)
                    _DB_Connection.Delete(DB_Table.ENTRANCE_TESTS, new Dictionary<string, object> { { "direction_id", (uint)(v[0]) },
                        { "subject_dict_id", (uint)(v[1]) }, { "subject_id", (uint)(v[2]) },
                    { "priority", v[3]}, { "campaign_id", _CampaignId}, { "direction_faculty", v[5]} });
            }

            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.ENTRANCE_TESTS, new Dictionary<string, object>
                { { "direction_id", uint.Parse(v[0]) }, { "subject_dict_id", uint.Parse(v[1]) }, { "subject_id", uint.Parse(v[2]) },
                    { "priority", int.Parse(v[3])}, { "campaign_id", _CampaignId}, { "direction_faculty", v[5]} });
        }

        void LoadCampaign()
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

            foreach (var v in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, 14),
                    new Tuple<string, Relation, object> ("campaigns_id", Relation.EQUAL, _CampaignId)
                }))
            {
                string itemName = _DB_Helper.GetDictionaryItemName(14, (uint)v[0]);
                lbEduFormsChoosen.Items.Add(itemName);
                lbEduFormsAll.Items.Remove(itemName);
            }

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

        void LoadTables()
        {
            foreach (var v in _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[] { "direction_faculty",
                    "direction_id", "places_budget_o", "places_budget_oz", "places_budget_z", "places_target_o", "places_target_oz",
                    "places_target_z", "places_quota_o", "places_quota_oz", "places_quota_z"}, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                    }))
                foreach (DataGridViewRow r in dgvDirections.Rows)
                    if ((v[0].ToString() == r.Cells[4].Value.ToString())&&(v[1].ToString()==r.Cells[0].Value.ToString()))
                    {
                        r.Cells[1].Value = true;
                        for (int i = 5; i < r.Cells.Count; i++)
                            r.Cells[i].Value = v[i-3].ToString();
                    }

            foreach (var v in _DB_Connection.Select(DB_Table.CAMPAIGNS_FACULTIES_DATA, new string[] { "faculty_short_name", "hostel_places" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("campaign_id", Relation.EQUAL, _CampaignId)
                }))
                foreach (DataGridViewRow r in dgvFacultities.Rows)
                    if (r.Cells[0].Value.ToString() == v[0].ToString())
                        r.Cells[2].Value = v[1].ToString();

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

            foreach (var v in _DB_Connection.Select(DB_Table.ENTRANCE_TESTS, new string[] { "direction_id", "subject_dict_id", "subject_id", "priority", "direction_faculty" },
                new List<Tuple<string, Relation, object>>
                {
                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignId)
                }))                
                foreach (DataGridViewRow r in dgvEntranceTests.Rows)
                    if ((v[0].ToString() == r.Cells[0].Value.ToString())&&(v[4].ToString()== r.Cells[3].Value.ToString()))
                        r.Cells[3 + Convert.ToInt32(v[3])].Value = _DB_Helper.GetDictionaryItemName(1, (uint)v[2]);
        }

        private void btAddEduLevel_Click(object sender, EventArgs e)
        {
            if (lbEduFormsAll.SelectedIndex != -1)
            {
                lbEduFormsChoosen.Items.Add(lbEduFormsAll.SelectedItem.ToString());
                lbEduFormsAll.Items.Remove(lbEduFormsAll.SelectedItem.ToString());
            }
        }

        private void btRemoveEduLevel_Click(object sender, EventArgs e)
        {
            if (lbEduFormsChoosen.SelectedIndex != -1)
            {
                lbEduFormsAll.Items.Add(lbEduFormsChoosen.SelectedItem.ToString());
                lbEduFormsChoosen.Items.Remove(lbEduFormsChoosen.SelectedItem.ToString());
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
            ButtonsAppearanceChange(dgvTargetOrganizatons.Rows.Count);
        }

        private void dgvTargetOrganizatons_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
                switch (dgvTargetOrganizatons.CurrentCell.ColumnIndex)
            {
                case 2:
                    TargetOrganizationSelect form1 = new TargetOrganizationSelect();
                    form1.ShowDialog();
                    dgvTargetOrganizatons.CurrentRow.Cells[0].Value = form1.OrganizationID;
                    dgvTargetOrganizatons.CurrentRow.Cells[1].Value = form1.OrganizationName;
                    break;
                case 6:
                    List<string> filters = new List<string>();
                    if (cbEduLevelBacc.Checked)
                        filters.Add("03");
                    if (cbEduLevelSpec.Checked)
                        filters.Add("05");
                    if (cbEduLevelMag.Checked)
                        filters.Add("04");

                    DirectionSelect form2 = new DirectionSelect(filters);
                    form2.ShowDialog();
                    dgvTargetOrganizatons.CurrentRow.Cells[3].Value = form2.DirectionID;
                    dgvTargetOrganizatons.CurrentRow.Cells[4].Value = form2.DirectionName;
                    dgvTargetOrganizatons.CurrentRow.Cells[5].Value = form2.DirectionCode;
                    break;
            }           

        }

        private void dgvPaidPlaces_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateProfiliesTable();
        }

        private void cbEduLevelBacc_CheckedChanged(object sender, EventArgs e)
        {
            FillDirectionsTable();
            FillFacultiesTable();
            FillProfiliesTable();
            FillDistTable();
        }

        private void cbEduLevelSpec_CheckedChanged(object sender, EventArgs e)
        {
            FillDirectionsTable();
            FillFacultiesTable();
            FillProfiliesTable();
            FillDistTable();
        }

        private void cbEduLevelMag_CheckedChanged(object sender, EventArgs e)
        {
            FillDirectionsTable();
            FillFacultiesTable();
            FillProfiliesTable();
            FillDistTable();
        }

        private void dgvDirections_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (dgvDirections.CurrentCell.ColumnIndex==0)
            //    if ((dgvDirections.CurrentCell as DataGridViewCheckBoxCell).Value== (dgvDirections.CurrentCell as DataGridViewCheckBoxCell).TrueValue)
            //    {

            //    }

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
                    if (lbEduFormsChoosen.Items.Count == 0)
                        MessageBox.Show("Не выбраны формы обучения.");
                    else if (!(cbEduLevelBacc.Checked) && !(cbEduLevelMag.Checked) && !(cbEduLevelSpec.Checked))
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
    }
}
