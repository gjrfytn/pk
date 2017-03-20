using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    using Olympic =
           Dictionary<uint, System.Tuple<uint?, string, Dictionary<System.Tuple<uint, uint>, System.Tuple<System.Tuple<uint, uint>[], uint, uint>>>>;

    class DB_Helper
    {
        readonly DB_Connector _DB_Connection;

        public DB_Helper(DB_Connector connection)
        {
            _DB_Connection = connection;
        }

        public Dictionary<uint, string> GetDictionaryItems(uint dictionaryID)
        {
            if (dictionaryID == 10 || dictionaryID == 19)
                throw new System.ArgumentException("Нельзя получить данные справочника с ID " + dictionaryID + " при помощи этого метода.", "dictionaryID");

            return _DB_Connection.Select(
                 DB_Table.DICTIONARIES_ITEMS,
                 new string[] { "item_id", "name" },
                 new List<System.Tuple<string, Relation, object>>
                 {
                    new System.Tuple<string, Relation, object>( "dictionary_id",Relation.EQUAL, dictionaryID)
                 }
                 ).ToDictionary(i1 => (uint)i1[0], i2 => i2[1].ToString());
        }

        public Dictionary<uint, string[]> GetDirectionsDictionaryItems()
        {
            return _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS).ToDictionary(
                i1 => (uint)i1[0],
                i2 => new string[] { i2[1].ToString(), i2[2].ToString(), i2[3].ToString(), i2[4].ToString(), i2[5].ToString(), i2[6].ToString() }
                );
        }

        public Olympic GetOlympicsDictionaryItems()
        {
            Olympic dictionaryItems = new Olympic();

            foreach (object[] olymp in _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS))
            {
                Dictionary<System.Tuple<uint, uint>, System.Tuple<System.Tuple<uint, uint>[], uint, uint>> profiles =
                    new Dictionary<System.Tuple<uint, uint>, System.Tuple<System.Tuple<uint, uint>[], uint, uint>>();
                foreach (object[] prof in _DB_Connection.Select(
                    DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                    new string[] { "*" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("olympic_id", Relation.EQUAL, olymp[0]) }))
                {
                    profiles.Add(
                        new System.Tuple<uint, uint>((uint)prof[1], (uint)prof[2]),
                        new System.Tuple<System.Tuple<uint, uint>[], uint, uint>(
                            _DB_Connection.Select(
                                DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                                new string[] { "dictionaries_items_dictionary_id", "dictionaries_items_item_id" },
                                new List<System.Tuple<string, Relation, object>>
                                {
                                    new System.Tuple<string, Relation, object>("dictionary_olympic_profiles_olympic_id", Relation.EQUAL, prof[0]),
                                    new System.Tuple<string, Relation, object>("dictionary_olympic_profiles_profile_dict_id", Relation.EQUAL, prof[1]),
                                    new System.Tuple<string, Relation, object>("dictionary_olympic_profiles_profile_id", Relation.EQUAL, prof[2])
                                }).Select(s => new System.Tuple<uint, uint>((uint)s[0], (uint)s[1])).ToArray(),
                            (uint)prof[3],
                            (uint)prof[4]
                            ));
                }
                dictionaryItems.Add(
                    (uint)olymp[0],
                    new System.Tuple<uint?, string, Dictionary<System.Tuple<uint, uint>, System.Tuple<System.Tuple<uint, uint>[], uint, uint>>>(
                        olymp[1] as uint?,
                        olymp[2].ToString(),
                        profiles
                        ));
            }

            return dictionaryItems;
        }

        public string GetDictionaryItemName(uint dictionaryID, uint itemID)
        {
            List<object[]> list = _DB_Connection.Select(
                DB_Table.DICTIONARIES_ITEMS,
                new string[] { "name" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, dictionaryID),
                    new System.Tuple<string, Relation, object>("item_id", Relation.EQUAL, itemID)
                });

            if (list.Count == 0)
                throw new System.ArgumentException("Не найден справочник либо элемент справочника с заданным ID.");

            return list[0][0].ToString();
        }

        public uint GetDictionaryItemID(uint dictionaryID, string itemName)
        {
            List<object[]> list = _DB_Connection.Select(
                DB_Table.DICTIONARIES_ITEMS,
                new string[] { "item_id" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, dictionaryID),
                    new System.Tuple<string, Relation, object>("name", Relation.EQUAL, itemName)
                });

            if (list.Count == 0)
                throw new System.ArgumentException("Не найден справочник с заданным ID либо элемент справочника с заданным наименованием.");

            return (uint)list[0][0];
        }
    }
}
