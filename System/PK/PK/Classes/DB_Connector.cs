using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace PK.Classes
{
    class DB_Connector : System.IDisposable
    {
        public readonly string User;
        public readonly string Password;

        private MySqlConnection _Connection;

        public DB_Connector(string connectionString, string user, string password)
        {
            #region Contracts
            if (password == null)
                throw new System.ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new System.ArgumentException("Некорректная строка подключения.", nameof(connectionString));
            if (string.IsNullOrWhiteSpace(user))
                throw new System.ArgumentException("Некорректное имя пользователя.", nameof(user));
            #endregion

            User = user;
            Password = password;

            _Connection = new MySqlConnection(connectionString + " user = " + user + "; password = " + password);
            _Connection.Open();
        }

        #region IDisposable Support
        private bool _Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    _Connection.Close();
                }

                _Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public List<object[]> Select(DB_Table table, params string[] fields)
        {
            #region Contracts
            if (fields == null)
                throw new System.ArgumentNullException(nameof(fields));
            #endregion

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
            #region Contracts
            if (fields == null)
                throw new System.ArgumentNullException(nameof(fields));
            if (whereExpressions == null)
                throw new System.ArgumentNullException(nameof(whereExpressions));
            if (fields.Length == 0)
                throw new System.ArgumentException("Массив с именами столбцов должен содержать хотя бы один элемент.", nameof(fields));
            if (whereExpressions.Count == 0)
                throw new System.ArgumentException("Список с параметрами фильтрации должен содержать хотя бы один элемент.", nameof(whereExpressions));
            #endregion

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
            #region Contracts
            if (columnsValues == null)
                throw new System.ArgumentNullException(nameof(columnsValues));
            if (columnsValues.Count == 0)
                throw new System.ArgumentException("Словарь с именами и значениями столбцов должен содержать хотя бы один элемент.", nameof(columnsValues));
            #endregion

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
            #region Contracts
            if (columnsValues == null)
                throw new System.ArgumentNullException(nameof(columnsValues));
            if (whereColumnValues == null)
                throw new System.ArgumentNullException(nameof(whereColumnValues));
            if (columnsValues.Count == 0)
                throw new System.ArgumentException("Словарь с именами и значениями столбцов должен содержать хотя бы один элемент.", nameof(columnsValues));
            if (whereColumnValues.Count == 0)
                throw new System.ArgumentException("Список с параметрами фильтрации должен содержать хотя бы один элемент.", nameof(whereColumnValues));
            #endregion

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
            #region Contracts
            if (whereColumnValues == null)
                throw new System.ArgumentNullException(nameof(whereColumnValues));
            if (whereColumnValues.Count == 0)
                throw new System.ArgumentException("Словарь с параметрами фильтрации должен содержать хотя бы один элемент.", nameof(whereColumnValues));
            #endregion

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
            #region Contracts
            if (columnsValues == null)
                throw new System.ArgumentNullException(nameof(columnsValues));
            if (columnsValues.Count == 0)
                throw new System.ArgumentException("Словарь с именами и значениями столбцов должен содержать хотя бы один элемент.", nameof(columnsValues));
            #endregion

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
            #region Contracts
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Некорректное имя процедуры.", nameof(name));
            #endregion

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
