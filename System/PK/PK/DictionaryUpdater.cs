using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK
{
    using Olympic =
           Dictionary<uint, System.Tuple<uint?, string, Dictionary<System.Tuple<uint, uint>, System.Tuple<System.Tuple<uint, uint>[], uint, uint>>>>;

    class DictionaryUpdater
    {
        readonly DB_Connector _DB_Connection;
        readonly FIS_Connector _FIS_Connection;
        readonly DB_Helper _DB_Helper;

        public DictionaryUpdater(DB_Connector dbConnection, FIS_Connector fisConnection)
        {
            _DB_Connection = dbConnection;
            _FIS_Connection = fisConnection;
            _DB_Helper = new DB_Helper(_DB_Connection);
        }

        public void UpdateDictionaries()
        {
            Dictionary<uint, string> fisDictionaries = _FIS_Connection.GetDictionaries();
            Dictionary<uint, string> dbDictionaries = _DB_Connection.Select(DB_Table.DICTIONARIES).ToDictionary(d1 => (uint)d1[0], d2 => d2[1].ToString());

            string addedReport = "Добавлены справочники:";
            ushort addedCount = 0;

            foreach (var d in fisDictionaries)
                if (d.Key != 10 && d.Key != 19)
                {
                    var fisDictionaryItems = _FIS_Connection.GetDictionaryItems(d.Key);

                    if (dbDictionaries.ContainsKey(d.Key))
                    {
                        if (d.Value == dbDictionaries[d.Key])
                            UpdateDictionaryItems(d.Key, d.Value, fisDictionaryItems);
                        else if (Utility.ShowActionMessageWithConfirmation(
                         "В ФИС изменилось наименование справочника с кодом " + d.Key +
                         ":\nC \"" + dbDictionaries[d.Key] + "\"\nна \"" + d.Value +
                         "\".\n\nОбновить наименование в БД?\nВ случае отказа изменения элементов этого справочника проверятся не будут." //TODO Нужен мягкий знак "проверятся"?
                         ))
                        {
                            _DB_Connection.Update(DB_Table.DICTIONARIES,
                                new Dictionary<string, object> { { "name", d.Value } },
                                new Dictionary<string, object> { { "id", d.Key } }
                                );
                            UpdateDictionaryItems(d.Key, d.Value, fisDictionaryItems);
                        }
                    }
                    else
                    {
                        _DB_Connection.Insert(DB_Table.DICTIONARIES,
                            new Dictionary<string, object> { { "id", d.Key }, { "name", d.Value } }
                            );
                        foreach (var item in fisDictionaryItems)
                            _DB_Connection.Insert(DB_Table.DICTIONARIES_ITEMS,
                                new Dictionary<string, object> { { "dictionary_id", d.Key }, { "item_id", item.Key }, { "name", item.Value } }
                                );

                        addedReport += "\n" + d.Key + " \"" + d.Value + "\"";
                        addedCount++;
                    }
                }

            foreach (var d in dbDictionaries)
                if (!fisDictionaries.ContainsKey(d.Key) && Utility.ShowActionMessageWithConfirmation(
                             "В ФИС отсутствует справочник " + d.Key + " \"" + d.Value + "\".\n\nУдалить справочник из БД?"
                             ))
                    _DB_Connection.Delete(DB_Table.DICTIONARIES, new Dictionary<string, object> { { "id", d.Key } });

            if (addedCount == 0)
                addedReport = "Новых справочников нет.";
            else
                addedReport += "\nВсего: " + addedCount;
            MessageBox.Show(addedReport, "Обновление завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void UpdateDictionaryItems(uint dictionaryID, string dictionaryName, Dictionary<uint, string> fisDictionaryItems)
        {
            Dictionary<uint, string> dbDictionaryItems = _DB_Helper.GetDictionaryItems(dictionaryID);

            string addedReport = "В справочник №" + dictionaryID + " \"" + dictionaryName + "\" добавлены элементы:";
            ushort addedCount = 0;

            foreach (var item in fisDictionaryItems)
                if (dbDictionaryItems.ContainsKey(item.Key))
                {
                    if (item.Value != dbDictionaryItems[item.Key] && Utility.ShowActionMessageWithConfirmation(
                                 "Справочник №" + dictionaryID + " \"" + dictionaryName + "\":\nв ФИС изменилось наименование элемента с кодом "
                                 + item.Key + ":\nC \"" + dbDictionaryItems[item.Key] + "\"\nна \"" + item.Value +
                                 "\".\n\nОбновить наименование в БД?"
                                 ))
                        _DB_Connection.Update(DB_Table.DICTIONARIES_ITEMS,
                            new Dictionary<string, object> { { "name", item.Value } },
                            new Dictionary<string, object> { { "dictionary_id", dictionaryID }, { "item_id", item.Key } }
                            );
                }
                else
                {
                    _DB_Connection.Insert(DB_Table.DICTIONARIES_ITEMS,
                        new Dictionary<string, object> { { "dictionary_id", dictionaryID }, { "item_id", item.Key }, { "name", item.Value } }
                        );
                    addedReport += "\n" + item.Key + " \"" + item.Value + "\"";
                    addedCount++;
                }

            foreach (var item in dbDictionaryItems)
                if (!fisDictionaryItems.ContainsKey(item.Key) && Utility.ShowActionMessageWithConfirmation(
                             "Справочник №" + dictionaryID + " \"" + dictionaryName + "\":\nв ФИС отсутствует элемент " +
                             item.Key + " \"" + item.Value + "\".\n\nУдалить элемент из БД?"
                             ))
                    _DB_Connection.Delete(DB_Table.DICTIONARIES_ITEMS,
                        new Dictionary<string, object> { { "dictionary_id", dictionaryID }, { "item_id", item.Key } }
                        );

            if (addedCount != 0)
            {
                addedReport += "\nВсего: " + addedCount;
                MessageBox.Show(addedReport, "Справочник обновлён", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void UpdateDirectionsDictionary()
        {
            Dictionary<uint, string[]> fisDictionaryItems = _FIS_Connection.GetDirectionsDictionaryItems();
            Dictionary<uint, string[]> dbDictionaryItems = _DB_Helper.GetDirectionsDictionaryItems();

            string addedReport = "В справочник №" + 10 + " \"" + "Направления подготовки" + "\" добавлены элементы:";
            ushort addedCount = 0;

            foreach (var item in fisDictionaryItems)
                if (dbDictionaryItems.ContainsKey(item.Key))
                {
                    for (byte i = 0; i < item.Value.Length; ++i) //TODO Если несколько изменений
                        if (item.Value[i] != dbDictionaryItems[item.Key][i] && Utility.ShowActionMessageWithConfirmation(
                                     "В ФИС изменилось значение " + (i + 2) + " столбца элемента с кодом "
                                     + item.Key + ":\nC \"" + dbDictionaryItems[item.Key][i] + "\"\nна \"" + item.Value[i] +
                                     "\".\n\nОбновить значение в БД?"
                                     ))
                            _DB_Connection.Update(DB_Table.DICTIONARY_10_ITEMS,
                                new Dictionary<string, object>
                                {
                                { "name", item.Value[0] },
                                { "code", item.Value[1] },
                                { "qualification_code", item.Value[2] },
                                { "period", item.Value[3] },
                                { "ugs_code", item.Value[4] },
                                { "ugs_name", item.Value[5] }
                                },
                                new Dictionary<string, object> { { "id", item.Key } }
                                );
                }
                else
                {
                    _DB_Connection.Insert(DB_Table.DICTIONARY_10_ITEMS,
                        new Dictionary<string, object>
                            {
                            {"id",item.Key },
                                { "name", item.Value[0] },
                                { "code", item.Value[1] },
                                { "qualification_code", item.Value[2] },
                                { "period", item.Value[3] },
                                { "ugs_code", item.Value[4] },
                                { "ugs_name", item.Value[5] }
                            }
                        );
                    addedReport += "\n" + item.Key;
                    addedCount++;
                }

            foreach (var item in dbDictionaryItems)
                if (!fisDictionaryItems.ContainsKey(item.Key) && Utility.ShowActionMessageWithConfirmation(
                             "В ФИС отсутствует направление " +
                             item.Key + ".\n\nУдалить его из БД?"
                             ))
                    _DB_Connection.Delete(DB_Table.DICTIONARY_10_ITEMS,
                        new Dictionary<string, object> { { "id", item.Key } }
                        );

            if (addedCount == 0)
                addedReport = "Новых направлений нет.";
            else
                addedReport += "\nВсего: " + addedCount;

            MessageBox.Show(addedReport, "Справочник обновлён", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void UpdateOlympicsDictionary()
        {
            Olympic fisDictionaryItems = _FIS_Connection.GetOlympicsDictionaryItems();
            Olympic dbDictionaryItems = _DB_Helper.GetOlympicsDictionaryItems();

            string addedReport = "В справочник №" + 19 + " \"" + "Олимпиады" + "\" добавлены элементы:";
            ushort addedCount = 0;

            foreach (var olymp in fisDictionaryItems)
                if (dbDictionaryItems.ContainsKey(olymp.Key))
                {
                    if (olymp.Value.Item1 != dbDictionaryItems[olymp.Key].Item1 && Utility.ShowActionMessageWithConfirmation(
                                     "В ФИС изменился номер олимпиады " + olymp.Key + " \"" + dbDictionaryItems[olymp.Key].Item2 + "\""
                                     + ":\nC \"" + dbDictionaryItems[olymp.Key].Item1 + "\"\nна \"" + olymp.Value.Item1 +
                                     "\".\n\nОбновить значение в БД?"
                                     ))
                        _DB_Connection.Update(DB_Table.DICTIONARY_19_ITEMS,
                            new Dictionary<string, object> { { "olympic_number", olymp.Value.Item1 } },
                            new Dictionary<string, object> { { "olympic_id", olymp.Key } }
                            );

                    if (olymp.Value.Item2 != dbDictionaryItems[olymp.Key].Item2 && Utility.ShowActionMessageWithConfirmation(
                                     "В ФИС изменилось имя олимпиады с кодом "
                                     + olymp.Key + ":\nC \"" + dbDictionaryItems[olymp.Key].Item2 + "\"\nна \"" + olymp.Value.Item2 +
                                     "\".\n\nОбновить значение в БД?"
                                     ))
                        _DB_Connection.Update(DB_Table.DICTIONARY_19_ITEMS,
                            new Dictionary<string, object> { { "olympic_name", olymp.Value.Item2 } },
                            new Dictionary<string, object> { { "olympic_id", olymp.Key } }
                            );

                    foreach (var prof in olymp.Value.Item3)
                        if (dbDictionaryItems[olymp.Key].Item3.ContainsKey(prof.Key))
                        {
                            if (prof.Value.Item3 != dbDictionaryItems[olymp.Key].Item3[prof.Key].Item3 && Utility.ShowActionMessageWithConfirmation(
                                             "В ФИС изменился код уровня для профиля с кодом " + prof.Key.Item2 + " олимпиады " + olymp.Key
                                             + " \"" + dbDictionaryItems[olymp.Key].Item2 + "\"" + ":\nC \"" + dbDictionaryItems[olymp.Key].Item3[prof.Key].Item3 + "\"\nна \"" + prof.Value.Item3 +
                                             "\".\n\nОбновить значение в БД?"
                                             ))
                                _DB_Connection.Update(DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                                    new Dictionary<string, object> { { "level_id", prof.Value.Item3 } },
                                    new Dictionary<string, object>
                                    {
                                        { "olympic_id",olymp.Key },
                                        { "profile_dict_id", prof.Key.Item1 },
                                        { "profile_id", prof.Key.Item2 }
                                    }
                                    );

                            foreach (System.Tuple<uint, uint> subj in prof.Value.Item1)
                                if (!dbDictionaryItems[olymp.Key].Item3[prof.Key].Item1.Contains(subj))
                                {
                                    _DB_Connection.Insert(DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                                        new Dictionary<string, object>
                                        {
                                            { "dictionary_olympic_profiles_olympic_id",olymp.Key },
                                            { "dictionary_olympic_profiles_profile_dict_id", prof.Key.Item1 },
                                            { "dictionary_olympic_profiles_profile_id", prof.Key.Item2 },
                                            { "dictionaries_items_dictionary_id", subj.Item1 },
                                            { "dictionaries_items_item_id",subj.Item2 }
                                        }
                                        );
                                }
                        }
                        else
                        {
                            _DB_Connection.Insert(DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                        new Dictionary<string, object>
                        {
                            { "olympic_id",olymp.Key },
                            { "profile_dict_id", prof.Key.Item1 },
                            { "profile_id", prof.Key.Item2 },
                            { "level_dict_id", prof.Value.Item2 },
                            { "level_id", prof.Value.Item3 }
                        }
                        );

                            foreach (System.Tuple<uint, uint> subj in prof.Value.Item1)
                                _DB_Connection.Insert(DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                            new Dictionary<string, object>
                            {
                                { "dictionary_olympic_profiles_olympic_id",olymp.Key },
                                { "dictionary_olympic_profiles_profile_dict_id", prof.Key.Item1 },
                                { "dictionary_olympic_profiles_profile_id", prof.Key.Item2 },
                                { "dictionaries_items_dictionary_id", subj.Item1 },
                                { "dictionaries_items_item_id",subj.Item2 }
                            }
                            );
                        }
                }
                else
                {
                    _DB_Connection.Insert(DB_Table.DICTIONARY_19_ITEMS,
                        new Dictionary<string, object>
                        {
                            { "olympic_id",olymp.Key },
                            { "olympic_number", olymp.Value.Item1 },
                            { "olympic_name", olymp.Value.Item2 }
                            }
                        );

                    foreach (var prof in olymp.Value.Item3)
                    {
                        _DB_Connection.Insert(DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                        new Dictionary<string, object>
                        {
                            { "olympic_id",olymp.Key },
                            { "profile_dict_id", prof.Key.Item1 },
                            { "profile_id", prof.Key.Item2 },
                            { "level_dict_id", prof.Value.Item2 },
                            { "level_id", prof.Value.Item3 }
                        }
                        );

                        foreach (System.Tuple<uint, uint> subj in prof.Value.Item1)
                            _DB_Connection.Insert(DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                        new Dictionary<string, object>
                        {
                            { "dictionary_olympic_profiles_olympic_id",olymp.Key },
                            { "dictionary_olympic_profiles_profile_dict_id", prof.Key.Item1 },
                            { "dictionary_olympic_profiles_profile_id", prof.Key.Item2 },
                            { "dictionaries_items_dictionary_id", subj.Item1 },
                            { "dictionaries_items_item_id",subj.Item2 }
                        }
                        );
                    }

                    addedReport += "\n" + olymp.Key;
                    addedCount++;
                }

            foreach (var olymp in dbDictionaryItems)
                if (!fisDictionaryItems.ContainsKey(olymp.Key))
                {
                    if (Utility.ShowActionMessageWithConfirmation(
                             "В ФИС отсутствует олимпиада " + olymp.Key + " \"" + olymp.Value.Item2 + "\""
                             + ".\n\nУдалить её из БД?"
                             ))
                        _DB_Connection.Delete(DB_Table.DICTIONARY_19_ITEMS,
                            new Dictionary<string, object> { { "olympic_id", olymp.Key } }
                            );
                }
                else
                    foreach (var prof in olymp.Value.Item3)
                        if (!fisDictionaryItems[olymp.Key].Item3.ContainsKey(prof.Key))
                        {
                            if (Utility.ShowActionMessageWithConfirmation(
                             "В ФИС отсутствует профиль с кодом " + prof.Key.Item2 + " олимпиады " + olymp.Key
                             + " \"" + dbDictionaryItems[olymp.Key].Item2 + "\"" + ".\n\nУдалить его из БД?"
                             ))
                                _DB_Connection.Delete(DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                                new Dictionary<string, object>
                                {
                                    { "olympic_id",olymp.Key },
                                    { "profile_dict_id", prof.Key.Item1 },
                                    { "profile_id", prof.Key.Item2 }
                                }
                                );
                        }
                        else
                            foreach (System.Tuple<uint, uint> subj in prof.Value.Item1)
                                if (!fisDictionaryItems[olymp.Key].Item3[prof.Key].Item1.Contains(subj) && Utility.ShowActionMessageWithConfirmation(
                             "В ФИС отсутствует дисциплина с кодом " + subj.Item2 + " для профиля с кодом " + prof.Key.Item2 + " олимпиады " + olymp.Key
                             + " \"" + dbDictionaryItems[olymp.Key].Item2 + "\"" + ".\n\nУдалить его из БД?"
                             ))
                                    _DB_Connection.Delete(DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                            new Dictionary<string, object>
                            {
                                { "dictionary_olympic_profiles_olympic_id",olymp.Key },
                                { "dictionary_olympic_profiles_profile_dict_id", prof.Key.Item1 },
                                { "dictionary_olympic_profiles_profile_id", prof.Key.Item2 },
                                { "dictionaries_items_dictionary_id", subj.Item1 },
                                { "dictionaries_items_item_id",subj.Item2 }
                            }
                            );

            if (addedCount == 0)
                addedReport = "Новых олимпиад нет.";
            else
                addedReport += "\nВсего: " + addedCount;

            MessageBox.Show(addedReport, "Справочник обновлён", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
