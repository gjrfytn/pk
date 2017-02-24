using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace PK
{
    class DB_Connector
    {
        MySqlConnection _Connection;

        public DB_Connector()
        {
            string connString = @"
                        server = localhost;
                        port=3306;
                        database = pk_db;
                        user = root;
                        password = 1234;
                ";
            _Connection = new MySqlConnection(connString);
            _Connection.Open();
        }

        ~DB_Connector()
        {
            _Connection.Close();
        }

        public Dictionary<uint, string> GetDictionaries()
        {
            MySqlCommand cmd = new MySqlCommand(GetTableName(DB_Table.DICTIONARIES), _Connection);
            cmd.CommandType = System.Data.CommandType.TableDirect;
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                Dictionary<uint, string> dictionaries = new Dictionary<uint, string>();
                while (reader.Read())
                    dictionaries.Add((uint)reader["id"], reader["name"].ToString());

                return dictionaries;
            }
        }

        public void InsertDictionary(uint id, string name)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO " + GetTableName(DB_Table.DICTIONARIES) + " (id, name) VALUES (" + id + ", @name);", _Connection);
            cmd.Parameters.AddWithValue("name", name);
            cmd.ExecuteNonQuery();
        }

        public void UpdateDictionary(uint id, string newname)
        {
            MySqlCommand cmd = new MySqlCommand("UPDATE " + GetTableName(DB_Table.DICTIONARIES) + " SET name= @newname WHERE id= " + id + ";", _Connection);
            cmd.Parameters.AddWithValue("newname", newname);
            cmd.ExecuteNonQuery();
        }

        public void DeleteDictionary(uint id)
        {
            MySqlCommand cmd = new MySqlCommand("DELETE FROM " + GetTableName(DB_Table.DICTIONARIES) + " WHERE id=" + id + ";", _Connection);
            cmd.ExecuteNonQuery();
        }

        public Dictionary<uint, string> GetDictionaryItems(uint dictionaryID)
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + GetTableName(DB_Table.DICTIONARIES_ITEMS) + " WHERE dictionary_id=" + dictionaryID + ";", _Connection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                Dictionary<uint, string> dictionaryItems = new Dictionary<uint, string>();
                while (reader.Read())
                    dictionaryItems.Add((uint)reader["item_id"], reader["name"].ToString());

                return dictionaryItems;
            }
        }

        public void InsertDictionaryItem(uint dictionaryID, uint itemID, string itemName)
        {
            //ТЕСТЫ (оба способа должны давать одинаковые результаты):
            /* MySqlCommand cmd = new MySqlCommand("INSERT INTO "+DB_Tables.DICTIONARIES_ITEMS+" (dictionary_id, item_id, name) VALUES (" + dictionaryID + ", " + itemID + ", @itemName);", _Connection);
             cmd.Parameters.AddWithValue("itemName", itemName);
             cmd.ExecuteNonQuery();*/

            Insert(DB_Table.DICTIONARIES_ITEMS, new Dictionary<string, object> {
                {"dictionary_id" ,dictionaryID},
                {"item_id",itemID },
                { "name",itemName}
            });
        }

        public void UpdateDictionaryItem(uint dictionaryID, uint itemID, string newname)
        {
            //ТЕСТЫ (оба способа должны давать одинаковые результаты):
            /*MySqlCommand cmd = new MySqlCommand("UPDATE "+DB_Tables.DICTIONARIES_ITEMS+" SET name= @newname WHERE dictionary_id= " + dictionaryID + " AND item_id= " + itemID + ";", _Connection);
            cmd.Parameters.AddWithValue("newname", newname);
            cmd.ExecuteNonQuery();*/

            Update(DB_Table.DICTIONARIES_ITEMS, new Dictionary<string, object> {
                { "name" ,newname} }, new Dictionary<string, object> {
                    { "dictionary_id",dictionaryID },
                    { "item_id" ,itemID}
                });
        }

        public void DeleteDictionaryItem(uint dictionaryID, uint itemID)
        {
            //ТЕСТЫ (оба способа должны давать одинаковые результаты):
            /*MySqlCommand cmd = new MySqlCommand("DELETE FROM "+DB_Tables.DICTIONARIES_ITEMS+" WHERE dictionary_id= " + dictionaryID + " AND item_id= " + itemID + ";", _Connection);
            cmd.ExecuteNonQuery();*/

            Delete(DB_Table.DICTIONARIES_ITEMS, new Dictionary<string, object> {
                { "dictionary_id", dictionaryID },
                { "item_id", itemID }
            });
        }

        public List<object[]> Select(DB_Table table, params string[] fields) //TODO Добавить WHERE?
        {
            MySqlCommand cmd = new MySqlCommand("", _Connection);
            if (fields.Length == 0)
            {
                cmd.CommandText = GetTableName(table);
                cmd.CommandType = System.Data.CommandType.TableDirect;
            }
            else
            {
                string queryFields = "";
                foreach (string field in fields)
                    queryFields += field + ", ";
                queryFields = queryFields.Remove(queryFields.Length - 2);

                cmd.CommandText = "SELECT " + queryFields + " FROM " + GetTableName(table) + ";";
            }

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                List<object[]> results = new List<object[]>();
                int colCount = reader.FieldCount/*VisibleFieldCount?*/;
                while (reader.Read())
                {
                    object[] row = new object[colCount];

                    for (byte i = 0; i < colCount; ++i)
                        row[i] = fields.Length == 0 ? reader[i] : reader[fields[i]]; //TODO Можно оптимизировать

                    results.Add(row);
                }
                return results;
            }
        }

        public uint Insert(DB_Table table, Dictionary<string, object> columnsValues)
        {
            MySqlCommand cmd = new MySqlCommand("", _Connection);
            string columns = "", values = "";
            byte count = 1;
            foreach (var cv in columnsValues)
            {
                columns += cv.Key + ", ";
                string paramName = "@p" + count;
                values += paramName + ", ";
                cmd.Parameters.AddWithValue(paramName, cv.Value);
                count++;
            }
            columns = columns.Remove(columns.Length - 2);
            values = values.Remove(values.Length - 2);

            cmd.CommandText = "INSERT INTO " + GetTableName(table) + " (" + columns + ") VALUES (" + values + ");";
            cmd.ExecuteNonQuery();
            return (uint)cmd.LastInsertedId;
        }

        public void Update(DB_Table table, Dictionary<string, object> columnsValues, Dictionary<string, object> whereColumnValues)
        {
            MySqlCommand cmd = new MySqlCommand("", _Connection);
            string set = "";
            byte count = 1;
            foreach (var cv in columnsValues)
            {
                string paramName = "@sp" + count;
                set += cv.Key + "= " + paramName + ", ";
                cmd.Parameters.AddWithValue(paramName, cv.Value);
                count++;
            }
            string where = "";
            count = 1;
            foreach (var cv in whereColumnValues)
            {
                string paramName = "@wp" + count;
                where += cv.Key + "= " + paramName + " AND ";
                cmd.Parameters.AddWithValue(paramName, cv.Value);
                count++;
            }
            set = set.Remove(set.Length - 2);
            where = where.Remove(where.Length - 5);

            cmd.CommandText = "UPDATE " + GetTableName(table) + " SET " + set + " WHERE " + where + ";";
            cmd.ExecuteNonQuery();
        }

        public void Delete(DB_Table table, Dictionary<string, object> whereColumnValues)
        {
            MySqlCommand cmd = new MySqlCommand("", _Connection);
            byte count = 1;
            string where = "";
            foreach (var cv in whereColumnValues)
            {
                string paramName = "@p" + count;
                where += cv.Key + "= " + paramName + " AND ";
                cmd.Parameters.AddWithValue(paramName, cv.Value);
                count++;
            }
            where = where.Remove(where.Length - 5);

            cmd.CommandText = "DELETE FROM " + GetTableName(table) + " WHERE " + where + ";";
            cmd.ExecuteNonQuery();
        }

        static string GetTableName(DB_Table table) => System.Enum.GetName(typeof(DB_Table), table).ToLower();
    }
}
