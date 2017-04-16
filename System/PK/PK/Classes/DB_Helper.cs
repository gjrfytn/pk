using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    class DB_Helper
    {
        #region DictItemsNames
        public const string EduFormO = "Очная форма";
        public const string EduFormOZ = "Очно-заочная (вечерняя)";
        public const string EduFormZ = "Заочная форма";
        public const string EduLevelB = "Бакалавриат";
        public const string EduLevelM = "Магистратура";
        public const string EduLevelS = "Специалитет";
        public const string EduSourceB = "Бюджетные места";
        public const string EduSourceP = "С оплатой обучения";
        public const string EduSourceQ = "Квота приема лиц, имеющих особое право";
        public const string EduSourceT = "Целевой прием";
        public const string MedalAchievement = "Аттестат о среднем (полном) общем образовании, золотая медаль";
        #endregion

        public uint CurrentCampaignID
        {
            get { return (uint)_DB_Connection.Select(DB_Table.CONSTANTS, "current_campaign_id")[0][0]; }
        }

        private readonly DB_Connector _DB_Connection;

        public DB_Helper(DB_Connector connection)
        {
            _DB_Connection = connection;
        }

        public Dictionary<uint, string> GetDictionaryItems(FIS_Dictionary dictionary)
        {
            return _DB_Connection.Select(
                 DB_Table.DICTIONARIES_ITEMS,
                 new string[] { "item_id", "name" },
                 new List<System.Tuple<string, Relation, object>>
                 {
                    new System.Tuple<string, Relation, object>( "dictionary_id",Relation.EQUAL, (uint)dictionary)
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

        public Dictionary<uint, FIS_Olympic_TEMP> GetOlympicsDictionaryItems()
        {
            Dictionary<uint, FIS_Olympic_TEMP> dictionaryItems = new Dictionary<uint, FIS_Olympic_TEMP>();

            foreach (object[] olymp in _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS))
            {
                Dictionary<System.Tuple<uint, uint>, FIS_Olympic_TEMP.FIS_Olympic_Profile> profiles =
                    new Dictionary<System.Tuple<uint, uint>, FIS_Olympic_TEMP.FIS_Olympic_Profile>();
                foreach (object[] prof in _DB_Connection.Select(
                    DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                    new string[] { "*" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("olympic_id", Relation.EQUAL, olymp[0]) }))
                {
                    profiles.Add(
                       new System.Tuple<uint, uint>((uint)prof[1], (uint)prof[2]),
                        new FIS_Olympic_TEMP.FIS_Olympic_Profile
                        {
                            Subjects = _DB_Connection.Select(
                                DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                                new string[] { "dictionaries_items_dictionary_id", "dictionaries_items_item_id" },
                                new List<System.Tuple<string, Relation, object>>
                                {
                                    new System.Tuple<string, Relation, object>("dictionary_olympic_profiles_olympic_id", Relation.EQUAL, prof[0]),
                                    new System.Tuple<string, Relation, object>("dictionary_olympic_profiles_profile_dict_id", Relation.EQUAL, prof[1]),
                                    new System.Tuple<string, Relation, object>("dictionary_olympic_profiles_profile_id", Relation.EQUAL, prof[2])
                                }).Select(s => new System.Tuple<uint, uint>((uint)s[0], (uint)s[1])).ToArray(),
                            LevelID = (uint)prof[4]
                        });
                }
                dictionaryItems.Add(
                    (uint)olymp[0],
                    new FIS_Olympic_TEMP
                    {
                        Year = (ushort)olymp[1],
                        Number = olymp[2] as uint?,
                        Name = olymp[3].ToString(),
                        Profiles = profiles
                    });
            }

            return dictionaryItems;
        }

        public string GetDictionaryItemName(FIS_Dictionary dictionary, uint itemID)
        {
            List<object[]> list = _DB_Connection.Select(
                DB_Table.DICTIONARIES_ITEMS,
                new string[] { "name" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, (uint)dictionary),
                    new System.Tuple<string, Relation, object>("item_id", Relation.EQUAL, itemID)
                });

            if (list.Count == 0)
                throw new System.ArgumentException("Не найден справочник либо элемент справочника с заданным ID.");

            return list[0][0].ToString();
        }

        public uint GetDictionaryItemID(FIS_Dictionary dictionary, string itemName)
        {
            List<object[]> list = _DB_Connection.Select(
                DB_Table.DICTIONARIES_ITEMS,
                new string[] { "item_id" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("dictionary_id", Relation.EQUAL, (uint)dictionary),
                    new System.Tuple<string, Relation, object>("name", Relation.EQUAL, itemName)
                });

            if (list.Count == 0)
                throw new System.ArgumentException("Не найден справочник либо элемент справочника с заданным наименованием.");

            return (uint)list[0][0];
        }

        public int GetDirectionIDByName(string dirName)
        {
            List<object[]> dirList = _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS, new string[] { "id" }, new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("name", Relation.EQUAL, dirName)
                });
            if (dirList.Count == 0)
                return 0;
            else
                return int.Parse(dirList[0][0].ToString());
        }

        public System.Tuple<string, string> GetDirectionNameAndCode(uint id)//TODO Нужны другие поля?
        {
            List<object[]> list = _DB_Connection.Select(
                DB_Table.DICTIONARY_10_ITEMS,
                new string[] { "name", "code" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("id", Relation.EQUAL, id),
                });

            if (list.Count == 0)
                throw new System.ArgumentException("В справочнике не найдено направление с заданным ID.");

            return new System.Tuple<string, string>(list[0][0].ToString(), list[0][1].ToString());
        }
    }
}
