using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK
{
    class DataManager
    {
        DB_Connector _DB_Connection;
        FIS_Connector _FIS_Connection;

        public DataManager(DB_Connector dbConnection, FIS_Connector fisConnection)
        {
            _DB_Connection = dbConnection;
            _FIS_Connection = fisConnection;
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
                 ).ToDictionary(i1 => (uint)i1[0], i2 => i2[1].ToString()
             );
        }

        public Dictionary<uint, string[]> GetDirectionsDictionaryItems()
        {
            return _DB_Connection.Select(DB_Table.DICTIONARY_10_ITEMS).ToDictionary(
                i1 => (uint)i1[0],
                i2 => new string[] { i2[1].ToString(), i2[2].ToString(), i2[3].ToString(), i2[4].ToString(), i2[5].ToString(), i2[6].ToString() }
             );
        }

        public void UpdateDictionaries()
        {
            var fisDictionaries = _FIS_Connection.GetDictionaries();
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
            Dictionary<uint, string> dbDictionaryItems = GetDictionaryItems(dictionaryID);

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
            Dictionary<uint, string[]> dbDictionaryItems = GetDirectionsDictionaryItems();

            string addedReport = "В справочник №" + 10 + " \"" + "Направления подготовки" + "\" добавлены элементы:";
            ushort addedCount = 0;

            foreach (var item in fisDictionaryItems)
                if (dbDictionaryItems.ContainsKey(item.Key))
                {
                    for (byte i = 0; i < item.Value.Length; ++i)
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
                addedReport = "Новых элементов нет.";
            else
                addedReport += "\nВсего: " + addedCount;

            MessageBox.Show(addedReport, "Справочник обновлён", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
