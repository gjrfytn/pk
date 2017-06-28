using System.Windows.Forms;

namespace PK.Forms
{
    partial class OlympicsDictionary : DictionaryBase
    {
        public OlympicsDictionary(Classes.DB_Connector connection) : base(connection)
        {
            InitializeComponent();
        }

        protected override void UpdateMainTable()
        {
            dgvOlympics.Rows.Clear();
            foreach (object[] olymp in _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS))
                dgvOlympics.Rows.Add(olymp[0], olymp[1], olymp[2] as uint?, olymp[3]);
        }

        protected override void UpdateDictionary(Classes.DictionaryUpdater updater)
        {
            updater.UpdateOlympicsDictionary();
        }

        private void dgvOlympics_SelectionChanged(object sender, System.EventArgs e)
        {
            if (dgvOlympics.SelectedRows.Count != 0)
            {
                dgvProfiles.Rows.Clear();
                foreach (object[] prof in _DB_Connection.Select(
                    DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                    new string[] { "profile_dict_id", "profile_id", "level_dict_id", "level_id" },
                    new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                    {
                    new System.Tuple<string, Relation, object> ("olympic_id", Relation.EQUAL, dgvOlympics.SelectedRows[0].Cells[dgvOlympics_ID.Index].Value)
                    }))
                    dgvProfiles.Rows.Add(
                        prof[0],
                        prof[1],
                        _DB_Connection.Select(
                            DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                            new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                            {
                            new System.Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,prof[0]),
                            new System.Tuple<string, Relation, object>("item_id",Relation.EQUAL,prof[1])
                            })[0][0],
                        _DB_Connection.Select(
                            DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                            new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                            {
                            new System.Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,prof[2]),
                            new System.Tuple<string, Relation, object>("item_id",Relation.EQUAL,prof[3])
                            })[0][0]);
            }
        }

        private void dgvProfiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            lbSubjects.Items.Clear();
            foreach (object[] subj in _DB_Connection.Select(
                DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                new string[] { "dictionaries_items_dictionary_id", "dictionaries_items_item_id" },
                new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object> ("dictionary_olympic_profiles_olympic_id", Relation.EQUAL, dgvOlympics.CurrentRow.Cells[dgvOlympics_ID.Index].Value),
                    new System.Tuple<string, Relation, object> ("dictionary_olympic_profiles_profile_dict_id", Relation.EQUAL, dgvProfiles[dgvProfiles_Dict_ID.Index,e.RowIndex].Value),
                    new System.Tuple<string, Relation, object> ("dictionary_olympic_profiles_profile_id", Relation.EQUAL, dgvProfiles[dgvProfiles_ID.Index,e.RowIndex].Value)
                }))
                lbSubjects.Items.Add(
                    _DB_Connection.Select(
                        DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                        new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                        {
                            new System.Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,subj[0]),
                            new System.Tuple<string, Relation, object>("item_id",Relation.EQUAL,subj[1])
                        })[0][0]);
        }
    }
}
