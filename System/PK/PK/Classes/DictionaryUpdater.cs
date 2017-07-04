using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using SharedClasses.DB;
using SharedClasses.FIS;

namespace PK.Classes
{
    class DictionaryUpdater
    {
        private readonly DB_Connector _DB_Connection;
        private readonly string _FIS_Address;
        private readonly string _FIS_Login;
        private readonly string _FIS_Password;
        private readonly DB_Helper _DB_Helper;

        public DictionaryUpdater(DB_Connector dbConnection, string address, string fisLogin, string fisPassword)
        {
            #region Contracts
            if (dbConnection == null)
                throw new System.ArgumentNullException(nameof(dbConnection));
            if (string.IsNullOrWhiteSpace(fisLogin))
                throw new System.ArgumentException("Некорректный логин.", nameof(fisLogin));
            if (string.IsNullOrWhiteSpace(fisPassword))
                throw new System.ArgumentException("Некорректный пароль.", nameof(fisPassword));
            #endregion

            _DB_Connection = dbConnection;
            _FIS_Address = address;
            _FIS_Login = fisLogin;
            _FIS_Password = fisPassword;
            _DB_Helper = new DB_Helper(_DB_Connection);
        }

        public void UpdateDictionaries()
        {
            Dictionary<uint, string> fisDictionaries = FIS_Connector.GetDictionaries(_FIS_Address,_FIS_Login, _FIS_Password);
            Dictionary<uint, string> dbDictionaries = _DB_Connection.Select(DB_Table.DICTIONARIES).ToDictionary(d1 => (uint)d1[0], d2 => d2[1].ToString());

            string addedReport = "Добавлены справочники:";
            ushort addedCount = 0;
            string deletedReport = "";

            foreach (var d in fisDictionaries)
                if (d.Key != 10 && d.Key != 19)
                {
                    var fisDictionaryItems = FIS_Connector.GetDictionaryItems(_FIS_Address,_FIS_Login, _FIS_Password, d.Key);

                    if (dbDictionaries.ContainsKey(d.Key))
                    {
                        if (d.Value == dbDictionaries[d.Key])
                            UpdateDictionaryItems((FIS_Dictionary)d.Key, d.Value, fisDictionaryItems);
                        else if (SharedClasses.Utility.ShowChoiceMessageWithConfirmation(
                         "В ФИС изменилось наименование справочника с кодом " + d.Key +
                         ":\nC \"" + dbDictionaries[d.Key] + "\"\nна \"" + d.Value +
                         "\".\n\nОбновить наименование в БД?\nВ случае отказа изменения элементов этого справочника проверятся не будут.", //TODO Нужен мягкий знак "проверятся"?
                         "Действие"
                         ))
                        {
                            _DB_Connection.Update(DB_Table.DICTIONARIES,
                                new Dictionary<string, object> { { "name", d.Value } },
                                new Dictionary<string, object> { { "id", d.Key } }
                                );
                            UpdateDictionaryItems((FIS_Dictionary)d.Key, d.Value, fisDictionaryItems);
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
                if (!fisDictionaries.ContainsKey(d.Key))
                    deletedReport += d.Key + ", ";

            if (addedCount == 0)
                addedReport = "Новых справочников нет.";
            else
                addedReport += "\nВсего: " + addedCount;

            if (deletedReport != "")
                deletedReport = "\n\nВнимание: в ФИС отсутствуют справочники с ID: " + deletedReport.Remove(deletedReport.Length - 2);

            MessageBox.Show(addedReport + deletedReport, "Обновление завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void UpdateDirectionsDictionary()
        {
            Dictionary<uint, string[]> fisDictionaryItems = FIS_Connector.GetDirectionsDictionaryItems(_FIS_Address,_FIS_Login, _FIS_Password);
            Dictionary<uint, string[]> dbDictionaryItems = _DB_Helper.GetDirectionsDictionaryItems();

            string addedReport = "В справочник №" + 10 + " \"" + "Направления подготовки" + "\" добавлены элементы:";
            ushort addedCount = 0;
            string deletedReport = "";

            foreach (var item in fisDictionaryItems)
                if (dbDictionaryItems.ContainsKey(item.Key))
                {
                    for (byte i = 0; i < item.Value.Length; ++i) //TODO Если несколько изменений
                        if (item.Value[i] != dbDictionaryItems[item.Key][i] && SharedClasses.Utility.ShowChoiceMessageWithConfirmation(
                                     "В ФИС изменилось значение " + (i + 2) + " столбца элемента с кодом "
                                     + item.Key + ":\nC \"" + dbDictionaryItems[item.Key][i] + "\"\nна \"" + item.Value[i] +
                                     "\".\n\nОбновить значение в БД?",
                                     "Действие"
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
                if (!fisDictionaryItems.ContainsKey(item.Key))
                    deletedReport += item.Key + ", ";

            if (addedCount == 0)
                addedReport = "Новых направлений нет.";
            else
                addedReport += "\nВсего: " + addedCount;

            if (deletedReport != "")
                deletedReport = "\n\nВнимание: в ФИС отсутствуют направления с ID: " + deletedReport.Remove(deletedReport.Length - 2);

            ShowUpdateMessage(addedReport + deletedReport);
        }

        public void UpdateOlympicsDictionary()
        {
            Dictionary<uint, FIS_Olympic_TEMP> fisDictionaryItems = FIS_Connector.GetOlympicsDictionaryItems(
                _FIS_Address,
                _FIS_Login,
                _FIS_Password,
                _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, "физика"),
                _DB_Helper.GetDictionaryItemID(FIS_Dictionary.OLYMPICS_PROFILES, "математика")
                );
            Dictionary<uint, FIS_Olympic_TEMP> dbDictionaryItems = _DB_Helper.GetOlympicsDictionaryItems();

            string addedReport = "В справочник №" + 19 + " \"" + "Олимпиады" + "\" добавлены элементы:";
            ushort addedCount = 0;
            string deletedReport = "";

            foreach (var olymp in fisDictionaryItems)
                if (dbDictionaryItems.ContainsKey(olymp.Key))
                {
                    if (olymp.Value.Year != dbDictionaryItems[olymp.Key].Year && SharedClasses.Utility.ShowChoiceMessageWithConfirmation(
                                     "В ФИС изменился год олимпиады " + olymp.Key + " \"" + dbDictionaryItems[olymp.Key].Name + "\""
                                     + ":\nC \"" + dbDictionaryItems[olymp.Key].Year + "\"\nна \"" + olymp.Value.Year +
                                     "\".\n\nОбновить значение в БД?",
                                     "Действие"
                                     ))
                        _DB_Connection.Update(DB_Table.DICTIONARY_19_ITEMS,
                            new Dictionary<string, object> { { "year", olymp.Value.Year } },
                            new Dictionary<string, object> { { "olympic_id", olymp.Key } }
                            );

                    if (olymp.Value.Number != dbDictionaryItems[olymp.Key].Number && SharedClasses.Utility.ShowChoiceMessageWithConfirmation(
                                     "В ФИС изменился номер олимпиады " + olymp.Key + " \"" + dbDictionaryItems[olymp.Key].Name + "\""
                                     + ":\nC \"" + dbDictionaryItems[olymp.Key].Number + "\"\nна \"" + olymp.Value.Number +
                                     "\".\n\nОбновить значение в БД?",
                                     "Действие"
                                     ))
                        _DB_Connection.Update(DB_Table.DICTIONARY_19_ITEMS,
                            new Dictionary<string, object> { { "olympic_number", olymp.Value.Number } },
                            new Dictionary<string, object> { { "olympic_id", olymp.Key } }
                            );

                    if (olymp.Value.Name != dbDictionaryItems[olymp.Key].Name && SharedClasses.Utility.ShowChoiceMessageWithConfirmation(
                                     "В ФИС изменилось имя олимпиады с кодом "
                                     + olymp.Key + ":\nC \"" + dbDictionaryItems[olymp.Key].Name + "\"\nна \"" + olymp.Value.Name +
                                     "\".\n\nОбновить значение в БД?",
                                     "Действие"
                                     ))
                        _DB_Connection.Update(DB_Table.DICTIONARY_19_ITEMS,
                            new Dictionary<string, object> { { "olympic_name", olymp.Value.Name } },
                            new Dictionary<string, object> { { "olympic_id", olymp.Key } }
                            );

                    foreach (var prof in olymp.Value.Profiles)
                        if (dbDictionaryItems[olymp.Key].Profiles.ContainsKey(prof.Key))
                        {
                            if (prof.Value.LevelID != dbDictionaryItems[olymp.Key].Profiles[prof.Key].LevelID && SharedClasses.Utility.ShowChoiceMessageWithConfirmation(
                                             "В ФИС изменился код уровня для профиля с кодом " + prof.Key.Item2 + " олимпиады " + olymp.Key
                                             + " \"" + dbDictionaryItems[olymp.Key].Name + "\"" + ":\nC \"" + dbDictionaryItems[olymp.Key].Profiles[prof.Key].LevelID + "\"\nна \"" + prof.Value.LevelID +
                                             "\".\n\nОбновить значение в БД?",
                                             "Действие"
                                             ))
                                _DB_Connection.Update(DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                                    new Dictionary<string, object> { { "level_id", prof.Value.LevelID } },
                                    new Dictionary<string, object>
                                    {
                                        { "olympic_id",olymp.Key },
                                        { "profile_dict_id", prof.Key.Item1 },
                                        { "profile_id", prof.Key.Item2 }
                                    }
                                    );

                            foreach (System.Tuple<uint, uint> subj in prof.Value.Subjects)
                                if (!dbDictionaryItems[olymp.Key].Profiles[prof.Key].Subjects.Contains(subj))
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
                            InsertProfileWithSubjects(olymp, prof);
                }
                else
                {
                    _DB_Connection.Insert(DB_Table.DICTIONARY_19_ITEMS,
                        new Dictionary<string, object>
                        {
                            { "olympic_id",olymp.Key },
                            { "year",olymp.Value.Year },
                            { "olympic_number", olymp.Value.Number },
                            { "olympic_name", olymp.Value.Name }
                            }
                        );

                    foreach (var prof in olymp.Value.Profiles)
                        InsertProfileWithSubjects(olymp, prof);

                    addedReport += "\n" + olymp.Key;
                    addedCount++;
                }

            foreach (var olymp in dbDictionaryItems)
                if (!fisDictionaryItems.ContainsKey(olymp.Key))
                    deletedReport += "олимпиада - " + olymp.Key + ", ";
                else
                    foreach (var prof in olymp.Value.Profiles)
                        if (!fisDictionaryItems[olymp.Key].Profiles.ContainsKey(prof.Key))
                            deletedReport += "олимпиада - " + olymp.Key + " -> профиль - " + prof.Key + ", ";
                        else
                            foreach (System.Tuple<uint, uint> subj in prof.Value.Subjects)
                                if (!fisDictionaryItems[olymp.Key].Profiles[prof.Key].Subjects.Contains(subj))
                                    deletedReport += "олимпиада - " + olymp.Key + " -> профиль - " + prof.Key + " -> дисциплина - " + subj.Item2 + ", ";

            if (addedCount == 0)
                addedReport = "Новых олимпиад нет.";
            else
                addedReport += "\nВсего: " + addedCount;

            if (deletedReport != "")
                deletedReport = "\n\nВнимание: в ФИС отсутствуют объекты с ID: " + deletedReport.Remove(deletedReport.Length - 2);

            ShowUpdateMessage(addedReport);
        }

        private void UpdateDictionaryItems(FIS_Dictionary dictionary, string dictionaryName, Dictionary<uint, string> fisDictionaryItems)
        {
            Dictionary<uint, string> dbDictionaryItems = _DB_Helper.GetDictionaryItems(dictionary);

            string addedReport = "В справочник №" + dictionary + " \"" + dictionaryName + "\" добавлены элементы:";
            ushort addedCount = 0;
            string deletedReport = "";

            foreach (var item in fisDictionaryItems)
                if (dbDictionaryItems.ContainsKey(item.Key))
                {
                    if (item.Value != dbDictionaryItems[item.Key] && SharedClasses.Utility.ShowChoiceMessageWithConfirmation(
                                 "Справочник №" + dictionary + " \"" + dictionaryName + "\":\nв ФИС изменилось наименование элемента с кодом "
                                 + item.Key + ":\nC \"" + dbDictionaryItems[item.Key] + "\"\nна \"" + item.Value +
                                 "\".\n\nОбновить наименование в БД?",
                                 "Действие"
                                 ))
                        _DB_Connection.Update(DB_Table.DICTIONARIES_ITEMS,
                            new Dictionary<string, object> { { "name", item.Value } },
                            new Dictionary<string, object> { { "dictionary_id", dictionary }, { "item_id", item.Key } }
                            );
                }
                else
                {
                    _DB_Connection.Insert(DB_Table.DICTIONARIES_ITEMS,
                        new Dictionary<string, object> { { "dictionary_id", dictionary }, { "item_id", item.Key }, { "name", item.Value } }
                        );
                    addedReport += "\n" + item.Key + " \"" + item.Value + "\"";
                    addedCount++;
                }

            foreach (var item in dbDictionaryItems)
                if (!fisDictionaryItems.ContainsKey(item.Key))
                    deletedReport += item.Key + ", ";

            if (addedCount != 0)
            {
                addedReport += "\nВсего: " + addedCount;
                ShowUpdateMessage(addedReport);
            }

            if (deletedReport != "")
            {
                deletedReport = "Справочник №" + dictionary + ": в ФИС отсутствуют элементы с ID: " + deletedReport.Remove(deletedReport.Length - 2);
                MessageBox.Show(deletedReport, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void InsertProfileWithSubjects(KeyValuePair<uint, FIS_Olympic_TEMP> olymp, KeyValuePair<System.Tuple<uint, uint>, FIS_Olympic_TEMP.FIS_Olympic_Profile> prof)
        {
            _DB_Connection.Insert(DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                new Dictionary<string, object>
                {
                    { "olympic_id",olymp.Key },
                    { "profile_dict_id", prof.Key.Item1 },
                    { "profile_id", prof.Key.Item2 },
                    { "level_dict_id", prof.Value.LevelDictID },
                    { "level_id", prof.Value.LevelID }
                });

            foreach (System.Tuple<uint, uint> subj in prof.Value.Subjects)
                _DB_Connection.Insert(DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                    new Dictionary<string, object>
                    {
                        { "dictionary_olympic_profiles_olympic_id",olymp.Key },
                        { "dictionary_olympic_profiles_profile_dict_id", prof.Key.Item1 },
                        { "dictionary_olympic_profiles_profile_id", prof.Key.Item2 },
                        { "dictionaries_items_dictionary_id", subj.Item1 },
                        { "dictionaries_items_item_id",subj.Item2 }
                    });
        }

        private void ShowUpdateMessage(string text)
        {
            MessageBox.Show(text, "Справочник обновлён", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
