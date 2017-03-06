using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PK
{
    public partial class NewCampaignForm : Form
    {
        DB_Connector _DB_Connection;
        uint? _CampaignUid;

        public NewCampaignForm(uint? campUid)
        {
            InitializeComponent();
            _DB_Connection = new DB_Connector();
            _CampaignUid = campUid;
           
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

            if (_CampaignUid.HasValue)
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
                foreach (var r in _DB_Connection.Select(DB_Table._FACULTIES_HAS_DICTIONARY_10_ITEMS, "faculties_short_name", "dictionary_10_items_id"))

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
            foreach (var v in _DB_Connection.Select(DB_Table.PROFILES, "name", "direction_id"))
                for (int i = 0; i < dgvPaidPlaces.Rows.Count; i++)
                {
                    if ((dgvPaidPlaces.Rows[i].Cells[1].Value.ToString() == "Н")
                        && (dgvPaidPlaces.Rows[i].Cells[2].Value.ToString() == v[1].ToString()))
                        dgvPaidPlaces.Rows.Insert(i + 1, true, "П", dgvPaidPlaces.Rows[i].Cells[2].Value.ToString(), v[0].ToString(), 
                            dgvPaidPlaces.Rows[i].Cells[4].Value.ToString(), dgvPaidPlaces.Rows[i].Cells[5].Value.ToString(), 0, 0, 0);
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
            dgvDist.Rows.Clear();
            foreach (DataGridViewRow r in dgvDirections.Rows)               
            {
                bool found = false;
                foreach (DataGridViewRow v in dgvDist.Rows)
                    if (v.Cells[0].Value.ToString() == r.Cells[0].Value.ToString())
                        found = true;
                if (!found)
                {
                    dgvDist.Rows.Add(r.Cells[0].Value.ToString(), r.Cells[2].Value.ToString(), r.Cells[3].Value.ToString());
                    for (int i=3; i< dgvDist.Rows[dgvDist.Rows.Count - 1].Cells.Count;i++)
                    {
                        (dgvDist.Rows[dgvDist.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Математика");
                        (dgvDist.Rows[dgvDist.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Русский язык");
                        (dgvDist.Rows[dgvDist.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Физика");
                        (dgvDist.Rows[dgvDist.Rows.Count - 1].Cells[i] as DataGridViewComboBoxCell).Items.Add("Обществозание");

                        switch (i)
                        {
                            case 3:
                                dgvDist.Rows[dgvDist.Rows.Count - 1].Cells[i].Value = "Математика";
                                break;
                            case 4:
                                dgvDist.Rows[dgvDist.Rows.Count - 1].Cells[i].Value = "Физика";
                                break;
                            case 5:
                                dgvDist.Rows[dgvDist.Rows.Count - 1].Cells[i].Value = "Русский язык";
                                break;
                        }
                        found = false;
                    }
                }                
            }
            dgvDist.Sort(dgvDist.Columns[1], ListSortDirection.Ascending);
        }

        void SaveCampaign()
        {
            uint campaignUid = _DB_Connection.Insert(DB_Table.CAMPAIGNS, new Dictionary<string, object>
                        {
                            { "name", tbName.Text }, { "start_year",  Convert.ToInt32(cbStartYear.SelectedItem)},
                            { "end_year", Convert.ToInt32(cbEndYear.SelectedItem) },
                            { "status_dict_id",  34}, { "status_id",  _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, 34),
                                    new Tuple<string, Relation, object>("name", Relation.EQUAL, cbState.SelectedItem.ToString())
                                })[0][0]},
                            { "type_dict_id",  38}, { "type_id",  _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, 38),
                                    new Tuple<string, Relation, object>("name", Relation.EQUAL, cbType.SelectedItem.ToString())
                                })[0][0]}
                        });

            foreach (var v in lbEduFormsChoosen.Items)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_uid", campaignUid},
                    { "dictionaries_items_dictionary_id", 14},
                    { "dictionaries_items_item_id",
                    _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 14),
                        new Tuple<string, Relation, object>("name", Relation.EQUAL, v)
                    })[0][0]
                    }
                });

            if (cbEduLevelBacc.Checked)
                    _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_uid", campaignUid},
                    { "dictionaries_items_dictionary_id", 2},
                    { "dictionaries_items_item_id",
                    _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object>("name", Relation.EQUAL, "Бакалавриат")
                    })[0][0]
                    }
                });

            if (cbEduLevelMag.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_uid", campaignUid},
                    { "dictionaries_items_dictionary_id", 2},
                    { "dictionaries_items_item_id",
                    _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object>("name", Relation.EQUAL, "Магистратура")
                    })[0][0]
                    }
                });

            if (cbEduLevelSpec.Checked)
                _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                {
                    { "campaigns_uid", campaignUid},
                    { "dictionaries_items_dictionary_id", 2},
                    { "dictionaries_items_item_id",
                    _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object>("name", Relation.EQUAL, "Специалитет")
                    })[0][0]
                    }
                });

            foreach (DataGridViewRow r in dgvDirections.Rows)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object>
                {
                    { "campaign_uid", campaignUid},
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
                    { "campaigns_uid", campaignUid},
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
                    { "campaign_uid", campaignUid},
                    { "faculty_short_name", r.Cells[0].Value.ToString()},
                    { "hostel_places", r.Cells[2].Value}
                });
            
                foreach (DataGridViewRow r in dgvDist.Rows)
                    for (int i = 1; i < r.Cells.Count-2; i++)
                    {
                    bool found = false;
                    int subjectId = Convert.ToInt32(_DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                        new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 1),
                                new Tuple<string, Relation, object> ("name", Relation.EQUAL, r.Cells[i+2].Value.ToString())
                            })[0][0]);

                    foreach (var v in _DB_Connection.Select(DB_Table.DIR_ENTRANCE_TESTS,
                        new string[] { "priority", "direction_id", "subject_dict_id", "subject_id" },
                        new List<Tuple<string, Relation, object>>
                            {
                                    new Tuple<string, Relation, object> ("direction_id", Relation.EQUAL, r.Cells[0].Value),
                                    new Tuple<string, Relation, object> ("subject_dict_id", Relation.EQUAL, 1),
                                    new Tuple<string, Relation, object> ("subject_id", Relation.EQUAL, subjectId)
                            }))

                        if (Convert.ToInt32(v[0]) == i)
                            found = true;
                        else
                        {
                            _DB_Connection.Update(DB_Table.DIR_ENTRANCE_TESTS, new Dictionary<string, object>
                            {
                                { "priority", i}
                            }, new Dictionary<string, object>
                            {
                                { "direction_id", v[1] },
                                { "subject_dict_id", v[2] },
                                { "subject_id", v[3] }
                            });
                            found = true;
                        }                          

                    if (!found)
                        _DB_Connection.Insert(DB_Table.DIR_ENTRANCE_TESTS, new Dictionary<string, object>
                        {
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
                        {
                            { "name", tbName.Text }, { "start_year",  Convert.ToInt32(cbStartYear.SelectedItem)},
                            { "end_year", Convert.ToInt32(cbEndYear.SelectedItem) },
                            { "status_dict_id",  34}, { "status_id",  _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, 34),
                                    new Tuple<string, Relation, object>("name", Relation.EQUAL, cbState.SelectedItem.ToString())
                                })[0][0]},
                            { "type_dict_id",  38}, { "type_id",  _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, 38),
                                    new Tuple<string, Relation, object>("name", Relation.EQUAL, cbType.SelectedItem.ToString())
                                })[0][0]}
                        },
                        new Dictionary<string, object>
                        {
                            { "uid", _CampaignUid}
                        });

            List<object[]> eduFormsList = new List<object[]>();
            eduFormsList = _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[]
                { "dictionaries_items_item_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_uid", Relation.EQUAL, _CampaignUid),
                        new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, 14)
                    });

            foreach (var v in lbEduFormsChoosen.Items)
            {
                bool found = false;
                foreach (var r in eduFormsList)
                    if (Convert.ToInt32(r[0]) == Convert.ToInt32(_DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, 14),
                            new Tuple<string, Relation, object>("name", Relation.EQUAL, v.ToString())
                        })[0][0]))
                        found = true;

                if (!found)
                {
                    _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    { { "campaigns_uid", _CampaignUid}, {"dictionaries_items_dictionary_id", 14}, {"dictionaries_items_item_id", _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, 14),
                            new Tuple<string, Relation, object>("name", Relation.EQUAL, v.ToString())
                        })[0][0]} });                    
                }                    
            }
            foreach (var v in eduFormsList)
            {
                bool found = false;
                foreach (var r in lbEduFormsChoosen.Items)
                    if (Convert.ToInt32(v[0]) == Convert.ToInt32(_DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, 14),
                            new Tuple<string, Relation, object>("name", Relation.EQUAL, r.ToString())
                        })[0][0]))
                        found = true;
                if (!found)
                    _DB_Connection.Delete(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object> { { "campaigns_uid", _CampaignUid },
                        { "dictionaries_items_dictionary_id", 14 }, { "dictionaries_items_item_id", v[0] } });
            }


            List<object[]> eduLevelsList = new List<object[]>();

            if (cbEduLevelBacc.Checked)
                eduLevelsList.Add(new object[] { _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object>("name", Relation.EQUAL, "Бакалавриат")
                    })[0][0]
                    });

            if (cbEduLevelMag.Checked)
                eduLevelsList.Add(new object[] { _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object>("name", Relation.EQUAL, "Магистратура")
                    })[0][0]
                    });

            if (cbEduLevelSpec.Checked)
                eduLevelsList.Add(new object[] { _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object>("name", Relation.EQUAL, "Специалитет")
                    })[0][0]
                    });

            List<object[]> oldList = _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[]
            { "dictionaries_items_item_id", }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("campaigns_uid", Relation.EQUAL, _CampaignUid),
                new Tuple<string, Relation, object>("dictionaries_items_dictionary_id", Relation.EQUAL, 2)
            });

            foreach (var v in eduLevelsList)
            {
                bool found = false;
                foreach (var r in oldList)
                    if (Convert.ToInt32(r[0]) == Convert.ToInt32(v[0]))
                        found = true;
                if (!found)
                    _DB_Connection.Insert(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                    { { "campaigns_uid", _CampaignUid }, { "dictionaries_items_dictionary_id", 2 }, { "dictionaries_items_item_id", v[0] } });
            }

            foreach (var v in oldList)
            {
                bool found = false;
                foreach (var r in eduLevelsList)
                {
                    if (Convert.ToInt32(v[0]) == Convert.ToInt32(r[0]))
                        found = true;

                    if (!found)
                        _DB_Connection.Delete(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new Dictionary<string, object>
                        { { "campaigns_uid", _CampaignUid }, { "dictionaries_items_dictionary_id", 2 }, { "dictionaries_items_item_id", v[0] } });
                }
            }


            oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_FACULTIES_DATA, new string[]
            { "faculty_short_name", "hostel_places" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("campaign_uid", Relation.EQUAL, _CampaignUid)
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
                        new Dictionary<string, object> { { "campaign_uid", _CampaignUid }, { "faculty_short_name", v[0].ToString() } });
                    newList.RemoveAt(newList.FindIndex(x => x[0] == v[0].ToString()));
                }
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_FACULTIES_DATA, new Dictionary<string, object>
                { { "faculty_short_name", v[0]}, { "hostel_places", Convert.ToInt32(v[1])}, { "campaign_uid", _CampaignUid} });


            oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new string[]
            { "direction_faculty", "direction_id", "places_budget_o", "places_budget_oz","places_budget_z","places_target_o",
                "places_target_oz","places_target_z","places_quota_o","places_quota_oz","places_quota_z"},
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_uid", Relation.EQUAL, _CampaignUid)
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
                        new Dictionary<string, object> { { "places_budget_o", Convert.ToInt32(r[2]) }, { "places_budget_oz", Convert.ToInt32(r[3])},
                            { "places_budget_z", Convert.ToInt32(r[4])}, { "places_target_o", Convert.ToInt32(r[5])},
                            { "places_target_oz", Convert.ToInt32(r[6])}, { "places_target_z", 0 }, {"places_quota_o", Convert.ToInt32(r[8])},
                            { "places_quota_oz", Convert.ToInt32(r[9])}, { "places_quota_z", Convert.ToInt32(r[10])} },
                        new Dictionary<string, object> { { "campaign_uid", _CampaignUid }, { "direction_faculty", v[0].ToString() },
                            { "direction_id", v[1]} });
                        newList.RemoveAt(newList.FindIndex(x => (x[0] == v[0].ToString()&&(x[1] == v[1].ToString()))));
                        break;
                    }
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_DIRECTIONS_DATA, new Dictionary<string, object>
                { { "campaign_uid", _CampaignUid }, { "direction_faculty", v[0] }, { "direction_id", Convert.ToInt32(v[1]) },
                    { "places_budget_o", Convert.ToInt32(v[2]) },{ "places_budget_oz", Convert.ToInt32(v[3])},
                    { "places_budget_z", Convert.ToInt32(v[4])}, { "places_target_o", Convert.ToInt32(v[5])},
                    { "places_target_oz", Convert.ToInt32(v[6])}, { "places_target_z", 0 }, {"places_quota_o", Convert.ToInt32(v[8])},
                    { "places_quota_oz", Convert.ToInt32(v[9])}, { "places_quota_z", Convert.ToInt32(v[10])}});


            oldList = _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[]
            { "profiles_direction_faculty", "profiles_direction_id", "profiles_name",
                "places_paid_o","places_paid_oz", "places_paid_z"},
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaigns_uid", Relation.EQUAL, _CampaignUid)
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
                        new Dictionary<string, object> { { "places_paid_o", Convert.ToInt32(r[3]) }, { "places_paid_oz", Convert.ToInt32(r[4])},
                            { "places_paid_z", Convert.ToInt32(r[5])}},
                        new Dictionary<string, object> { { "campaigns_uid", _CampaignUid }, { "profiles_direction_faculty", v[0].ToString() },
                            { "profiles_direction_id", v[1]}, { "profiles_name", v[2].ToString()} });
                        newList.RemoveAt(newList.FindIndex(x => (x[0] == v[0].ToString() && (x[1] == v[1].ToString())&&(x[2] == v[2].ToString()))));
                        break;
                    }
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.CAMPAIGNS_PROFILES_DATA, new Dictionary<string, object>
                { { "campaigns_uid", _CampaignUid }, { "profiles_direction_faculty", v[0] }, { "profiles_direction_id", Convert.ToInt32(v[1]) },
                    { "profiles_name", v[2]}, { "places_paid_o", Convert.ToInt32(v[3]) },{ "places_paid_oz", Convert.ToInt32(v[4])},
                    { "places_paid_z", Convert.ToInt32(v[5])}});


            oldList = _DB_Connection.Select(DB_Table.DIR_ENTRANCE_TESTS, new string[]
            { "direction_id", "subject_dict_id", "subject_id","priority"});

            newList.Clear();
            foreach (DataGridViewRow r in dgvDist.Rows)
            {
                newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                        new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, oldList[0][1]),
                                new Tuple<string, Relation, object> ("name", Relation.EQUAL, r.Cells[3].Value.ToString())
                            })[0][0].ToString(), "1"});
                newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                        new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, oldList[0][1]),
                                new Tuple<string, Relation, object> ("name", Relation.EQUAL, r.Cells[4].Value.ToString())
                            })[0][0].ToString(), "2"}); newList.Add(new string[] { r.Cells[0].Value.ToString(), oldList[0][1].ToString(),
                    _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "item_id" },
                        new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, oldList[0][1]),
                                new Tuple<string, Relation, object> ("name", Relation.EQUAL, r.Cells[5].Value.ToString())
                            })[0][0].ToString(), "3"});
            }
                

            foreach (var v in oldList)
            {
                foreach (var r in newList)
                    if ((v[0].ToString() == r[0]) && (v[1].ToString() == r[1])&&(v[2].ToString() == r[2]))
                    {
                        _DB_Connection.Update(DB_Table.DIR_ENTRANCE_TESTS,
                        new Dictionary<string, object> { { "priority", Convert.ToInt32(r[3]) } },
                        new Dictionary<string, object> { { "direction_id", v[0] }, { "subject_dict_id", v[1] },
                            { "subject_id", v[2]} });
                        newList.RemoveAt(newList.FindIndex(x => (x[0] == v[0].ToString() && (x[1] == v[1].ToString()) && (x[2] == v[2].ToString()))));
                        break;
                    }
            }
            foreach (var v in newList)
                _DB_Connection.Insert(DB_Table.DIR_ENTRANCE_TESTS, new Dictionary<string, object>
                { { "direction_id", Convert.ToInt32(v[0]) }, { "subject_dict_id", Convert.ToInt32(v[1]) }, { "subject_id", Convert.ToInt32((v[2])) },
                    { "priority", Convert.ToInt32(v[3])}});
        }

        void LoadCampaign()
        {
            List<object> temp = new List<object>();
                temp.AddRange( _DB_Connection.Select(DB_Table.CAMPAIGNS, 
                new string[] { "name", "start_year", "end_year", "status_dict_id", "status_id", "type_dict_id", "type_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("uid", Relation.EQUAL, _CampaignUid)
                })[0]);

            tbName.Text = temp[0].ToString();
            cbStartYear.SelectedItem = temp[1].ToString();
            cbEndYear.SelectedItem = temp[2].ToString();

            cbState.SelectedItem = _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, temp[3]),
                                    new Tuple<string, Relation, object>("item_id", Relation.EQUAL, temp[4])
                                })[0][0];

            cbType.SelectedItem = _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                                new List<Tuple<string, Relation, object>>
                                {
                                    new Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, temp[5]),
                                    new Tuple<string, Relation, object>("item_id", Relation.EQUAL, temp[6])
                                })[0][0];

            foreach (var v in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, 14),
                    new Tuple<string, Relation, object> ("campaigns_uid", Relation.EQUAL, _CampaignUid)
                }))
            {
                string itemName = _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                new List<Tuple<string, Relation, object>>
                {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 14),
                        new Tuple<string, Relation, object> ("item_id", Relation.EQUAL, v[0])
                })[0][0].ToString();
                lbEduFormsChoosen.Items.Add(itemName);
                lbEduFormsAll.Items.Remove(itemName);
            }

            foreach (var v in _DB_Connection.Select(DB_Table._CAMPAIGNS_HAS_DICTIONARIES_ITEMS, new string[] { "dictionaries_items_item_id" },
            new List<Tuple<string, Relation, object>>
            {
                    new Tuple<string, Relation, object> ("dictionaries_items_dictionary_id", Relation.EQUAL, 2),
                    new Tuple<string, Relation, object> ("campaigns_uid", Relation.EQUAL, _CampaignUid)
            }))
            {
                string itemName = _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                new List<Tuple<string, Relation, object>>
                {
                        new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 2),
                        new Tuple<string, Relation, object> ("item_id", Relation.EQUAL, v[0])
                })[0][0].ToString();
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
                        new Tuple<string, Relation, object>("campaign_uid", Relation.EQUAL, _CampaignUid)
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
                    new Tuple<string, Relation, object> ("campaign_uid", Relation.EQUAL, _CampaignUid)
                }))
                foreach (DataGridViewRow r in dgvFacultities.Rows)
                    if (r.Cells[0].Value.ToString() == v[0].ToString())
                        r.Cells[2].Value = v[1].ToString();

            foreach (var v in _DB_Connection.Select(DB_Table.CAMPAIGNS_PROFILES_DATA, new string[] { "profiles_direction_faculty",
                    "profiles_direction_id", "profiles_name", "places_paid_o", "places_paid_oz", "places_paid_z" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaigns_uid", Relation.EQUAL, _CampaignUid)
                    }
                    ))
                foreach (DataGridViewRow r in dgvPaidPlaces.Rows)
                    if ((v[0].ToString() == r.Cells[5].Value.ToString()) && (v[1].ToString() == r.Cells[2].Value.ToString()) && (v[2].ToString() == r.Cells[3].Value.ToString()))
                        for (int i = 6; i < r.Cells.Count; i++)
                            r.Cells[i].Value = v[i - 3].ToString();
            UpdateProfiliesTable();

            foreach (var v in _DB_Connection.Select(DB_Table.DIR_ENTRANCE_TESTS, new string[] { "direction_id", "subject_dict_id", "subject_id", "priority" }))
                foreach (DataGridViewRow r in dgvDist.Rows)
                    if ((v[0].ToString() == r.Cells[0].Value.ToString()))
                        r.Cells[2 + Convert.ToInt32(v[3])].Value = _DB_Connection.Select(DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                        new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("dictionary_id", Relation.EQUAL, 1),
                                new Tuple<string, Relation, object> ("item_id", Relation.EQUAL, Convert.ToInt32(v[2]))
                            })[0][0].ToString();
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
                    GridItemSelection form1 = new GridItemSelection();
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

            if ((!_CampaignUid.HasValue)&&(!(_DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "uid" },
                        new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object> ("name", Relation.EQUAL, tbName.Text)
                            }).Count == 0)))
                MessageBox.Show("Компания с таким именем уже существует.");
            else
            {
                foreach (DataGridViewRow r in dgvDist.Rows)
                    for (int i = 3; i < r.Cells.Count; i++)
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
                    else if (!_CampaignUid.HasValue)
                            {
                                SaveCampaign();
                                Close();
                            }
                        else
                        {
                            UpdateCampaing();
                        }
                }
            }            
        }

        private void NewCampaignForm_Load(object sender, EventArgs e)
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
