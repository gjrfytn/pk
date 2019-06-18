﻿using System.Collections.Generic;
using System.Linq;

namespace SharedClasses.DB
{
    public class DB_Helper
    {
        public enum CampaignType : byte { BACHELOR_SPECIALIST, MASTER, SPO }

        #region DictItemsNames
        public const string EduFormO = "Очная форма";
        public const string EduFormOZ = "Очно-заочная (вечерняя)";
        public const string EduFormZ = "Заочная форма";
        public const string EduLevelB = "Бакалавриат";
        public const string EduLevelM = "Магистратура";
        public const string EduLevelS = "Специалитет";
        public const string EduLevelSPO = "СПО";
        public const string EduSourceB = "Бюджетные места";
        public const string EduSourceP = "С оплатой обучения";
        public const string EduSourceQ = "Квота приема лиц, имеющих особое право";
        public const string EduSourceT = "Целевой прием";

        public const string MADIOlympDocName = "Диплом участника олимпиад";
        public const string OlympAchievementName = "Участие в олимпиадах и иных конкурсах";
        public const string SportAchievementOlympic = "Статус чемпиона и призера Олимпийских игр";
        public const string SportAchievementParalympic = "Статус чемпиона и призера Паралимпийских игр";
        public const string SportAchievementDeaflympic = "Статус чемпиона и призера Сурдлимпийских игр";
        public const string SportAchievementWorldChampion = "Чемпион Мира";
        public const string SportAchievementEuropeChampion = "Чемпион Европы";
        public const string SportAchievementGTO = "Золотой знак отличия ГТО";
        public const string SportAchievementWorldChampionship = "Победитель первенства мира";
        public const string SportAchievementEuropeChampionship = "Победитель первенства Европы";

        public const string SportDocTypeWorldChampion = "Диплом чемпиона мира";
        public const string SportDocTypeEuropeChampion = "Диплом чемпиона Европы";
        public const string SportDocTypeOlympic = "Диплом чемпиона/призера Олимпийских игр";
        public const string SportDocTypeParalympic = "Диплом чемпиона/призера Паралимпийских игр";
        public const string SportDocTypeDeaflympic = "Диплом чемпиона/призера Сурдлимпийских игр";

        public const string MedCertificate = "Медицинская справка";
        public const string PassportName = "Паспорт гражданина РФ";
        public const string ResidenceName = "Вид на жительство";
        public const string IntPassportName = "Паспорт гражданина иностранного государства";

        public const string NationalityRus = "Российская Федерация";

        public const string SubjectMath = "Математика";
        public const string SubjectRus = "Русский язык";
        public const string SubjectPhis = "Физика";
        public const string SubjectObsh = "Обществознание";
        public const string SubjectForen = "Иностранный язык";

        public const string BenefitOlympic = "Зачисление без вступительных испытаний";

        public const string MedalAchievement = "Аттестат о среднем (полном) общем образовании, золотая медаль";
        public const string RedMiddleDiploma = "Диплом о среднем профессиональном образовании с отличием";
        public const string RedDiplomaAchievement = "Иное";

        public const string NoEGE = "Не сдавал ЕГЭ";
        public const string EGEAdditionalDoc = "Дополнительный документ ЕГЭ";
        #endregion

        private readonly DB_Connector _DB_Connection;

        public DB_Helper(DB_Connector connection)
        {
            #region Contracts
            if (connection == null)
                throw new System.ArgumentNullException(nameof(connection));
            #endregion

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

        public Dictionary<uint, FIS.FIS_Olympic_TEMP> GetOlympicsDictionaryItems()
        {
            Dictionary<uint, FIS.FIS_Olympic_TEMP> dictionaryItems = new Dictionary<uint, FIS.FIS_Olympic_TEMP>();

            foreach (object[] olymp in _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS))
            {
                Dictionary<System.Tuple<uint, uint>, FIS.FIS_Olympic_TEMP.FIS_Olympic_Profile> profiles =
                    new Dictionary<System.Tuple<uint, uint>, FIS.FIS_Olympic_TEMP.FIS_Olympic_Profile>();
                foreach (object[] prof in _DB_Connection.Select(
                    DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                    new string[] { "*" },
                    new List<System.Tuple<string, Relation, object>> { new System.Tuple<string, Relation, object>("olympic_id", Relation.EQUAL, olymp[0]) }))
                {
                    profiles.Add(
                       new System.Tuple<uint, uint>((uint)prof[1], (uint)prof[2]),
                        new FIS.FIS_Olympic_TEMP.FIS_Olympic_Profile
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
                    new FIS.FIS_Olympic_TEMP
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
            #region Contracts
            if (string.IsNullOrWhiteSpace(itemName))
                throw new System.ArgumentException("Некорректное имя элемента.", nameof(itemName));
            #endregion

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

        public System.Tuple<string, string> GetDirectionNameAndCode(uint id)
        {
            List<object[]> list = _DB_Connection.Select(
                DB_Table.DICTIONARY_10_ITEMS,
                new string[] { "name", "code" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("id", Relation.EQUAL, id),
                });

            if (list.Count == 0)
                throw new System.ArgumentException("В справочнике не найдено направление с заданным ID.", nameof(id));

            return new System.Tuple<string, string>(list[0][0].ToString(), list[0][1].ToString());
        }

        public string GetDirectionShortName(string facultyShortName, uint directionID)
        {
            List<object[]> result = _DB_Connection.Select(
                DB_Table.DIRECTIONS,
                new string[] { "short_name" },
                new List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object>("faculty_short_name", Relation.EQUAL, facultyShortName),
                    new System.Tuple<string, Relation, object>("direction_id", Relation.EQUAL, directionID)
                });

            if (!result.Any())
                throw new System.ArgumentException("Направление не найдено.");

            return result[0][0].ToString();
        }

        public void UpdateData(
            DB_Table table,
            IEnumerable<object[]> oldDataList,
            IEnumerable<object[]> newDataList,
            string[] fieldsNames,
            string[] keyFieldsNames,
            MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            #region Contracts
            if (oldDataList == null)
                throw new System.ArgumentNullException(nameof(oldDataList));
            if (newDataList == null)
                throw new System.ArgumentNullException(nameof(newDataList));
            if (fieldsNames == null)
                throw new System.ArgumentNullException(nameof(fieldsNames));
            if (keyFieldsNames == null)
                throw new System.ArgumentNullException(nameof(keyFieldsNames));
            if (transaction == null)
                throw new System.ArgumentNullException(nameof(transaction));
            if (fieldsNames.Length == 0)
                throw new System.ArgumentException("Массив с именами полей должен содержать хотя бы один элемент.", nameof(fieldsNames));
            if (keyFieldsNames.Length == 0)
                throw new System.ArgumentException("Массив с именами ключевых полей должен содержать хотя бы один элемент.", nameof(keyFieldsNames));
            #endregion

            Dictionary<string, object> whereColumns = keyFieldsNames.ToDictionary(k => k, v => (object)null);
            foreach (object[] oldItem in oldDataList)
            {
                bool contains = false;
                foreach (object[] newItem in newDataList)
                {
                    bool allEqual = true;
                    for (int i = 0; i < fieldsNames.Length; ++i)
                        if (keyFieldsNames.Contains(fieldsNames[i]) && oldItem[i].ToString() != newItem[i].ToString())
                        {
                            allEqual = false;
                            break;
                        }

                    if (allEqual)
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    for (byte k = 0; k < keyFieldsNames.Length; ++k)
                        whereColumns[keyFieldsNames[k]] = oldItem[k];

                    _DB_Connection.Delete(table, whereColumns, transaction);
                }
            }

            Dictionary<string, object> columnsValues = fieldsNames.ToDictionary(k => k, v => (object)null);
            foreach (object[] newItem in newDataList)
            {
                for (byte i = 0; i < fieldsNames.Length; ++i)
                    columnsValues[fieldsNames[i]] = newItem[i];

                _DB_Connection.InsertOnDuplicateUpdate(table, columnsValues, transaction);
            }
        }

        public CampaignType GetCampaignType(uint id)
        {
            switch ((uint)_DB_Connection.Select(
                  DB_Table.CAMPAIGNS,
                  new string[] { "type_id" },
                  new List<System.Tuple<string, Relation, object>>
                  {
                      new System.Tuple<string, Relation, object> ("id",Relation.EQUAL,id)
                  }).Single()[0])
            {
                case 1:
                    return CampaignType.BACHELOR_SPECIALIST;
                case 2:
                    return CampaignType.MASTER;
                case 3:
                    return CampaignType.SPO;
                default:
                    throw new System.Exception("Reached unreachable.");
            }
        }

        //TODO Contracts
        public ushort GetMinMark(uint subjectID)
        {
            Dictionary<uint, string> constMap = new Dictionary<uint, string>
            {
                { 1, "min_russian_mark" },
                { 2, "min_math_mark" },
                { 6, "min_foreign_mark" },
                { 9, "min_social_mark" },
                { 10, "min_physics_mark" }
            };

            return (ushort)_DB_Connection.Select(DB_Table.CONSTANTS, constMap[subjectID])[0][0];
        }
    }
}
