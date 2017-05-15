﻿using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace PK.Classes
{
    class DB_Connector : System.IDisposable
    {
        public readonly string User;
        public readonly string Password;

        private MySqlConnection _Connection;

        public DB_Connector(string user, string password)
        {
            User = user;
            Password = password;

            _Connection = new MySqlConnection(Properties.Settings.Default.pk_db_CS + " user = " + user + "; password = " + password);
            _Connection.Open();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _Connection.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public List<object[]> Select(DB_Table table, params string[] fields)
        {
            MySqlCommand cmd = new MySqlCommand("", _Connection);
            if (fields.Length == 0)
            {
                cmd.CommandText = GetTableName(table);
                cmd.CommandType = System.Data.CommandType.TableDirect;
            }
            else
                cmd.CommandText = "SELECT " + string.Join(", ", fields) + " FROM " + GetTableName(table) + ";";

            return ExecuteSelect(cmd);
        }

        public List<object[]> Select(DB_Table table, string[] fields, List<System.Tuple<string, Relation, object>> whereExpressions)
        {
            if (fields.Length == 0)
                throw new System.ArgumentException("Массив с именами столбцов должен содержать хотя бы одно значение.", "fields");
            if (whereExpressions.Count == 0)
                throw new System.ArgumentException("Список с параметрами фильтрации должен содержать хотя бы одно значение.", "whereExpressions");

            MySqlCommand cmd = new MySqlCommand("", _Connection);
            string whereClause = "";
            byte count = 1;
            foreach (var expr in whereExpressions)
            {
                whereClause += expr.Item1;
                if (expr.Item2 == Relation.NOT_EQUAL && expr.Item3 == null)
                {
                    whereClause += " IS NOT NULL AND ";
                }
                else
                {
                    switch (expr.Item2)
                    {
                        case Relation.EQUAL:
                            whereClause += " <=> ";
                            break;
                        case Relation.NOT_EQUAL:
                            whereClause += " != ";
                            break;
                        case Relation.LESS:
                            whereClause += " < ";
                            break;
                        case Relation.GREATER:
                            whereClause += " > ";
                            break;
                        case Relation.LESS_EQUAL:
                            whereClause += " <= ";
                            break;
                        case Relation.GREATER_EQUAL:
                            whereClause += " >= ";
                            break;
                        default:
                            throw new System.Exception("Reached unreachable.");
                    }
                    string paramName = "@p" + count;
                    whereClause += paramName + " AND ";
                    cmd.Parameters.AddWithValue(paramName, expr.Item3);
                    count++;
                }
            }
            whereClause = whereClause.Remove(whereClause.Length - 5);

            cmd.CommandText = "SELECT " + string.Join(", ", fields) + " FROM " + GetTableName(table) + " WHERE " + whereClause + ";";

            return ExecuteSelect(cmd);
        }

        public uint Insert(DB_Table table, Dictionary<string, object> columnsValues, MySqlTransaction transaction = null)
        {
            MySqlCommand cmd = transaction != null ? new MySqlCommand("", _Connection, transaction) : new MySqlCommand("", _Connection);
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

        public void Update(DB_Table table, Dictionary<string, object> columnsValues, Dictionary<string, object> whereColumnValues, MySqlTransaction transaction = null)
        {
            MySqlCommand cmd = transaction != null ? new MySqlCommand("", _Connection, transaction) : new MySqlCommand("", _Connection);
            string set = "";
            byte count = 1;
            foreach (var cv in columnsValues)
            {
                string paramName = "@sp" + count;
                set += cv.Key + " = " + paramName + ", ";
                cmd.Parameters.AddWithValue(paramName, cv.Value);
                count++;
            }
            string where = "";
            count = 1;
            foreach (var cv in whereColumnValues)
            {
                string paramName = "@wp" + count;
                where += cv.Key + " = " + paramName + " AND ";
                cmd.Parameters.AddWithValue(paramName, cv.Value);
                count++;
            }
            set = set.Remove(set.Length - 2);
            where = where.Remove(where.Length - 5);

            cmd.CommandText = "UPDATE " + GetTableName(table) + " SET " + set + " WHERE " + where + ";";
            cmd.ExecuteNonQuery();
        }

        public void Delete(DB_Table table, Dictionary<string, object> whereColumnValues, MySqlTransaction transaction = null)
        {
            MySqlCommand cmd = transaction != null ? new MySqlCommand("", _Connection, transaction) : new MySqlCommand("", _Connection);
            byte count = 1;
            string where = "";
            foreach (var cv in whereColumnValues)
            {
                string paramName = "@p" + count;
                where += cv.Key + " = " + paramName + " AND ";
                cmd.Parameters.AddWithValue(paramName, cv.Value);
                count++;
            }
            where = where.Remove(where.Length - 5);

            cmd.CommandText = "DELETE FROM " + GetTableName(table) + " WHERE " + where + ";";
            cmd.ExecuteNonQuery();
        }

        public void InsertOnDuplicateUpdate(DB_Table table, Dictionary<string, object> columnsValues, MySqlTransaction transaction = null)
        {
            MySqlCommand cmd = transaction != null ? new MySqlCommand("", _Connection, transaction) : new MySqlCommand("", _Connection);
            string columns = "", values = "", update = "";
            byte count = 1;
            foreach (var cv in columnsValues)
            {
                columns += cv.Key + ", ";
                string paramName = "@p" + count;
                values += paramName + ", ";

                update += cv.Key + " = " + paramName + ", ";

                cmd.Parameters.AddWithValue(paramName, cv.Value);
                count++;
            }
            columns = columns.Remove(columns.Length - 2);
            values = values.Remove(values.Length - 2);
            update = update.Remove(update.Length - 2);

            cmd.CommandText = "INSERT INTO " + GetTableName(table) + " (" + columns + ") VALUES (" + values + ") ON DUPLICATE KEY UPDATE " + update + ";";
            cmd.ExecuteNonQuery();
        }

        public List<object[]> CallProcedure(string name, object parameter)
        {
            MySqlCommand cmd = new MySqlCommand(name, _Connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("id", parameter);

            return ExecuteSelect(cmd);
        }

        public MySqlTransaction BeginTransaction()
        {
            return _Connection.BeginTransaction();
        }

        private static string GetTableName(DB_Table table) => System.Enum.GetName(typeof(DB_Table), table).ToLower();

        private static List<object[]> ExecuteSelect(MySqlCommand cmd)
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                List<object[]> results = new List<object[]>();
                while (reader.Read())
                {
                    object[] row = new object[reader.FieldCount/*VisibleFieldCount?*/];

                    reader.GetValues(row);
                    results.Add(row);
                }
                return results;
            }
        }
    }
}
