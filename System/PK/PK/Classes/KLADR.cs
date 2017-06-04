using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    class KLADR : System.IDisposable
    {
        private const string _RawRegionCode = "SUBSTR(code,1,2)";
        private const string _RawDistrictCode = "SUBSTR(code,3,3)";
        private const string _RawTownCode = "SUBSTR(code,6,3)";
        private const string _RawSettlementCode = "SUBSTR(code,9,3)";
        private const string _RawStreetCode = "SUBSTR(code,12,4)";

        private const string _WhereRegion = "SUBSTR(code, 3) = '00000000000'";
        private const string _WhereDistrict = "SUBSTR(code, 6) = '00000000' AND " + _RawDistrictCode + " != '000'";
        private const string _WhereTown = "SUBSTR(code, 9) = '00000' AND " + _RawTownCode + " != '000'";
        private const string _WhereSettlement = "SUBSTR(code, 12) = '00' AND " + _RawSettlementCode + " != '000'";

        private readonly MySqlConnection _Connection;
        private readonly Dictionary<string, string> _TownsAndSettls = new Dictionary<string, string>();

        public KLADR(string user, string password)
        {
            #region Contracts
            if (password == null)
                throw new System.ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(user))
                throw new System.ArgumentException("Некорректное имя пользователя.", nameof(user));
            #endregion

            _Connection = new MySqlConnection(Properties.Settings.Default.kladr_CS + " user = " + user + "; password = " + password + ";");
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

        public List<string> GetRegions()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT name, socr FROM subjects WHERE " + _WhereRegion + ";", _Connection);

            return SelectNameSocr(cmd);
        }

        public List<string> GetDistricts(string region)
        {
            string regCode = GetRegionCode(region);
            if (regCode == null)
                return new List<string>();

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr FROM subjects WHERE " + _WhereDistrict + " AND " + _RawRegionCode + "='" + regCode + "';", _Connection);

            return SelectNameSocr(cmd);
        }

        public List<string> GetTowns(string region, string district)
        {
            string regCode;
            string distrCode;
            if (!GetCodes(region, district, out regCode, out distrCode))
                return new List<string>();

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr FROM subjects WHERE " + _WhereTown + " AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "';", _Connection);

            return SelectNameSocr(cmd);
        }

        public List<string> GetSettlements(string region, string district, string town)
        {
            string regCode, distrCode, townCode;
            if (!GetCodes(region, district, town, out regCode, out distrCode, out townCode))
                return new List<string>();

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr, `index` FROM subjects WHERE " + _WhereSettlement + " AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "' AND " + _RawTownCode + "='" + townCode + "';", _Connection);

            List<string> results = new List<string>();
            List<string> indexes = new List<string>();
            /*т.к. в одном субъекте могут быть нас. п. с одинаковыми названиями и категориями,
             мы прибавляем к одноимённым элементам их индекс.*/
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(reader.GetString(0) + " (" + reader.GetString(1) + ")");
                    indexes.Add(reader.GetValue(2).ToString());
                }
            }

            for (ushort i = 0; i < results.Count; ++i)
                if (results.Count(s => s == results[i]) > 1) //TODO Нужно? Если элемент повторяется.
                {
                    for (ushort j = (ushort)(i + 1); j < results.Count; ++j) //Ищем повторения (до него их быть не может).
                        if (results[i] == results[j])
                            if (indexes[i] == indexes[j]) //Если у них совпадают ещё и индексы, то остаётся только исключить один из них из списка.
                            {
                                results.RemoveAt(j);
                                j--;
                            }
                            else
                                results[j] += " [" + indexes[j] + "]"; //TODO Пустой индекс

                    results[i] += " [" + indexes[i] + "]";
                }

            return results;

        }

        public List<string> GetTownsAndSettlements(string region, string district)
        {
            //_Towns.Clear(); //TODO Оставлено для проверки, потом удалить

            //foreach (string settl in GetSettlements(region, district, ""))
            //    _Towns.Add(settl, "");

            //foreach (string town in GetTowns(region, district))
            //{
            //    _Towns.Add(town, null);
            //    foreach (string settl in GetSettlements(region, district, town))
            //        if (_Towns.ContainsKey(settl))
            //            _Towns.Add(settl + "(" + town + ")", town);
            //        else
            //            _Towns.Add(settl, town);
            //}

            string regCode, distrCode;
            if (!GetCodes(region, district, out regCode, out distrCode))
                return new List<string>();

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr, code, `index` FROM subjects WHERE (" + _WhereTown + " OR " + _WhereSettlement + ") AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "' ORDER BY code;", _Connection);

            _TownsAndSettls.Clear();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                Dictionary<string, string> towns = new Dictionary<string, string>();
                while (reader.Read())
                {
                    string code = reader.GetString(2);
                    string townCode = code.Substring(5, 3);
                    string name = reader.GetString(0) + " (" + reader.GetString(1) + ")";
                    if (code.Substring(8) == "00000")
                    {
                        towns.Add(townCode, name);
                        _TownsAndSettls.Add(name, null);
                    }
                    else if (_TownsAndSettls.ContainsKey(name))
                    {
                        if (townCode == "000")
                        {
                            string index = reader.GetValue(3).ToString();
                            if (index != "")
                            {
                                string extendedName = name + " [" + index + "]";
                                if (!_TownsAndSettls.ContainsKey(extendedName))
                                    _TownsAndSettls.Add(extendedName, "");
                            }
                        }
                        else
                        {
                            string extendedName = name + "(" + towns[townCode] + ")";
                            if (_TownsAndSettls.ContainsKey(extendedName))
                            {
                                string index = reader.GetValue(3).ToString();
                                if (index != "")
                                {
                                    extendedName = name + " [" + index + "]" + "(" + towns[townCode] + ")";
                                    if (!_TownsAndSettls.ContainsKey(extendedName))
                                        _TownsAndSettls.Add(extendedName, towns[townCode]);
                                }
                            }
                            else
                                _TownsAndSettls.Add(extendedName, towns[townCode]);
                        }
                    }
                    else if (townCode == "000")
                        _TownsAndSettls.Add(name, "");
                    else
                        _TownsAndSettls.Add(name, towns[townCode]);
                }
            }

            return _TownsAndSettls.Keys.ToList();
        }

        public List<string> GetStreets(string region, string district, string town, string settlement)
        {
            string regCode, distrCode, townCode;
            List<string> settlementsCodes;
            if (!GetCodes(region, district, town, settlement, out regCode, out distrCode, out townCode, out settlementsCodes))
                return new List<string>();

            string expr = "";
            foreach (string code in settlementsCodes)//Если нас. п. несколько, выбираем улицы из всех в один список. Предполагаем, что совпадения маловероятны.
            {
                string commonCode = regCode + distrCode + townCode + code;
                expr += "code BETWEEN '" + commonCode + "0000' AND '" + commonCode + "9999' OR ";
            }
            expr = expr.Remove(expr.Length - 4);

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr FROM streets WHERE " + expr + ";", _Connection);

            return SelectNameSocr(cmd);
        }

        public List<string> GetStreets(string region, string district, string townAndSettlement)
        {
            System.Tuple<string, string> buf = SplitTownAndSettlement(townAndSettlement);
            return GetStreets(region, district, buf.Item1, buf.Item2);
        }

        public List<string> GetHouses(string region, string district, string town, string settlement, string street)
        {
            string regCode, distrCode, townCode;
            List<string> settlementsCodes;
            if (!GetCodes(region, district, town, settlement, out regCode, out distrCode, out townCode, out settlementsCodes))
                return new List<string>();

            string streetCode = null;
            if (street != "")
            {
                foreach (string code in settlementsCodes)
                {
                    streetCode = GetStreetCode(regCode, distrCode, townCode, code, street);
                    if (streetCode != null)
                        break; //Считаем, что нет улиц с одинаковым названием в нас. п., у которых тоже одинаковые названия.
                }

                if (streetCode == null)
                    return new List<string>();
            }
            else
                streetCode = "0000";

            string expr = "";
            foreach (string code in settlementsCodes)
            {
                string commonCode = regCode + distrCode + townCode + code + streetCode;
                expr += "code BETWEEN '" + commonCode + "0000' AND '" + commonCode + "9999' OR ";
            }
            expr = expr.Remove(expr.Length - 4);

            MySqlCommand cmd = new MySqlCommand("SELECT name FROM houses WHERE " + expr + ";", _Connection);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                List<string> results = new List<string>();
                while (reader.Read())
                {
                    foreach (string house in reader.GetString(0).Split(','))
                        results.Add(house);
                }

                return results;
            };
        }

        public List<string> GetHouses(string region, string district, string townAndSettlement, string street)
        {
            System.Tuple<string, string> buf = SplitTownAndSettlement(townAndSettlement);
            return GetHouses(region, district, buf.Item1, buf.Item2, street);
        }

        public string GetIndex(string region, string district, string town, string settlement, string street, string house)
        {
            string index = null;
            string regCode, distrCode, townCode;
            List<string> settlementsCodes;
            MySqlCommand cmd;
            if (GetCodes(region, district, town, settlement, out regCode, out distrCode, out townCode, out settlementsCodes))
            {
                string streetCode = null;
                if (street != "")
                    foreach (string code in settlementsCodes)
                    {
                        streetCode = GetStreetCode(regCode, distrCode, townCode, code, street);
                        if (streetCode != null)
                            break; //Считаем, что нет улиц с одниковым названием в нас. п., у которых тоже одинаковые названия.
                    }
                else
                    streetCode = "0000";

                if (streetCode != null)
                {
                    if (house != "")
                        foreach (string code in settlementsCodes)
                        {
                            cmd = new MySqlCommand(
                             "SELECT `index` FROM houses WHERE `index` IS NOT NULL AND name REGEXP '.*[[:<:]]" + house + "[[:>:]].*' AND " +
                             _RawRegionCode + " = '" + regCode + "' AND " +
                             _RawDistrictCode + " = '" + distrCode + "' AND " +
                             _RawTownCode + " = '" + townCode + "' AND " +
                             _RawSettlementCode + " = '" + code + "' AND " +
                             _RawStreetCode + " = '" + streetCode + "';",
                             _Connection
                             );

                            index = SelectOneString(cmd); //TODO //Считаем, что запрос возвращает одну строку.
                            if (index != null)//TODO
                                return index;
                        }

                    foreach (string code in settlementsCodes)
                    {
                        cmd = new MySqlCommand(
                         "SELECT `index` FROM streets WHERE `index` IS NOT NULL AND " +
                         _RawRegionCode + " = '" + regCode + "' AND " +
                         _RawDistrictCode + " = '" + distrCode + "' AND " +
                         _RawTownCode + " = '" + townCode + "' AND " +
                         _RawSettlementCode + " = '" + code + "' AND " +
                         _RawStreetCode + " = '" + streetCode + "';",
                         _Connection
                         );

                        index = SelectOneString(cmd);
                        if (index != null)//TODO
                            return index;
                    }
                }
            }

            if (settlementsCodes.Count != 0)
                foreach (string code in settlementsCodes)
                {
                    cmd = new MySqlCommand(
                     "SELECT `index` FROM subjects WHERE `index` IS NOT NULL AND " +
                     _WhereSettlement + " AND " +
                     _RawRegionCode + " = '" + regCode + "' AND " +
                     _RawDistrictCode + " = '" + distrCode + "' AND " +
                     _RawTownCode + " = '" + townCode + "' AND " +
                     _RawSettlementCode + " = '" + code + "';",
                     _Connection
                     );

                    index = SelectOneString(cmd);
                    if (index != null)//TODO
                        return index;
                }

            if (townCode != null)
            {
                cmd = new MySqlCommand(
                    "SELECT `index` FROM subjects WHERE `index` IS NOT NULL AND " +
                    _WhereTown + " AND " +
                    _RawRegionCode + " = '" + regCode + "' AND " +
                    _RawDistrictCode + " = '" + distrCode + "' AND " +
                    _RawTownCode + " = '" + townCode + "';",
                    _Connection
                    );

                index = SelectOneString(cmd);
                if (index != null)//TODO
                    return index;
            }

            if (distrCode != null)
            {
                cmd = new MySqlCommand(
                    "SELECT `index` FROM subjects WHERE `index` IS NOT NULL AND " +
                    _WhereDistrict + " AND " +
                    _RawRegionCode + " = '" + regCode + "' AND " +
                    _RawDistrictCode + " = '" + distrCode + "';",
                    _Connection
                    );

                index = SelectOneString(cmd);
                if (index != null)//TODO
                    return index;
            }

            if (regCode != null)
            {
                cmd = new MySqlCommand(
                    "SELECT `index` FROM subjects WHERE `index` IS NOT NULL AND " +
                    _WhereRegion + " AND " +
                    _RawRegionCode + " = '" + regCode + "';",
                    _Connection
                    );

                index = SelectOneString(cmd);
            }

            return index;
        }

        public string GetIndex(string region, string district, string townAndSettlement, string street, string house)
        {
            System.Tuple<string, string> buf = SplitTownAndSettlement(townAndSettlement);
            return GetIndex(region, district, buf.Item1, buf.Item2, street, house);
        }

        private bool GetCodes(string region, string district, out string regCode, out string distrCode)
        {
            regCode = GetRegionCode(region);
            if (regCode == null)
            {
                distrCode = null;
                return false;
            }

            if (district != "")
            {
                distrCode = GetDistrictCode(regCode, district);
                if (distrCode == null)
                    return false;
            }
            else
                distrCode = "000";

            return true;
        }

        private bool GetCodes(string region, string district, string town, out string regCode, out string distrCode, out string townCode)
        {
            if (!GetCodes(region, district, out regCode, out distrCode))
            {
                townCode = null;
                return false;
            }

            if (town != "")
            {
                townCode = GetTownCode(regCode, distrCode, town);
                if (townCode == null)
                    return false;
            }
            else
                townCode = "000";

            return true;
        }

        private bool GetCodes(string region, string district, string town, string settlement, out string regCode, out string distrCode, out string townCode, out List<string> settlementsCodes)
        {
            settlementsCodes = new List<string>();
            if (!GetCodes(region, district, town, out regCode, out distrCode, out townCode))
                return false;

            if (settlement != "")
            {
                settlementsCodes = GetSettlementCodes(regCode, distrCode, townCode, settlement);
                if (settlementsCodes.Count == 0)
                    return false;
            }
            else
                settlementsCodes.Add("000");

            return true;
        }

        private string GetRegionCode(string region)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT " + _RawRegionCode + " FROM subjects WHERE " + _WhereRegion + " AND name = '" +
                region.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "';",
                _Connection
                );

            return SelectOneString(cmd);
        }

        private string GetDistrictCode(string regionCode, string district)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT " + _RawDistrictCode + " FROM subjects WHERE " + _WhereDistrict + " AND name = '" +
                district.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "' AND " +
                _RawRegionCode + " = '" + regionCode + "';",
                _Connection
                );

            return SelectOneString(cmd);
        }

        private string GetTownCode(string regionCode, string distrCode, string town)
        {
            string[] buf = town.Split(new string[] { " (" }, System.StringSplitOptions.None);
            if (buf.Length == 1)
                return null;

            MySqlCommand cmd = new MySqlCommand(
                 "SELECT " + _RawTownCode + " FROM subjects WHERE " + _WhereTown + " AND name = '" +
                 string.Join(" (", buf.Take(buf.Length - 1)) + "' AND " +
                 _RawRegionCode + " = '" + regionCode + "' AND " +
                 _RawDistrictCode + " = '" + distrCode + "';",
                 _Connection
                 );

            return SelectOneString(cmd);
        }

        /*Мы предполагаем, что у субъектов, высших по иерархии, не может быть одноимённых элементов.
         Однако, у нас. п. такие совпадения возможны, поэтому эта функция возвращает список кодов
         всех нас. п. с таким названием и индексом (если есть).*/
        private List<string> GetSettlementCodes(string regionCode, string distrCode, string townCode, string settlement)
        {
            string[] buf = settlement.Split(new string[] { " (" }, System.StringSplitOptions.None);
            if (buf.Length == 1)
                return new List<string>();

            string name = string.Join(" (", buf.Take(buf.Length - 1));
            string[] socrIndex = buf[buf.Length - 1].Split(')');
            string socr = socrIndex[0];
            string index = "";
            if (socrIndex[1].Length != 0)
                index = socrIndex[1].Substring(2, 6);

            MySqlCommand cmd = new MySqlCommand(
                 "SELECT " + _RawSettlementCode + " FROM subjects WHERE " + _WhereSettlement +
                 " AND name = '" + name +
                 "' AND socr" + " = '" + socr +
                  (index != "" ? ("' AND `index` = '" + index) : "") + "' AND " +
                 _RawRegionCode + " = '" + regionCode + "' AND " +
                 _RawDistrictCode + " = '" + distrCode + "' AND " +
                 _RawTownCode + " = '" + townCode + "';",
                 _Connection
                 );

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                List<string> results = new List<string>();

                if (reader.HasRows)
                    while (reader.Read())
                        results.Add(reader.GetString(0));

                return results;
            }
        }

        private string GetStreetCode(string regionCode, string distrCode, string townCode, string settlementCode, string street)
        {
            string[] buf = street.Split(new string[] { " (" }, System.StringSplitOptions.None);
            if (buf.Length == 1)
                return null;

            string name = string.Join(" (", buf.Take(buf.Length - 1));
            string socr = buf[buf.Length - 1].Remove(buf[buf.Length - 1].Length - 1);

            MySqlCommand cmd = new MySqlCommand(
                 "SELECT " + _RawStreetCode + " FROM streets WHERE name = '" + name +
                 "' AND socr" + " = '" + socr + "' AND " +
                 _RawRegionCode + " = '" + regionCode + "' AND " +
                 _RawDistrictCode + " = '" + distrCode + "' AND " +
                 _RawTownCode + " = '" + townCode + "' AND " +
                 _RawSettlementCode + " = '" + settlementCode + "';",
                 _Connection
                 );

            return SelectOneString(cmd);
        }

        private System.Tuple<string, string> SplitTownAndSettlement(string townAndSettlement)
        {
            if (_TownsAndSettls.ContainsKey(townAndSettlement))
            {
                if (_TownsAndSettls[townAndSettlement] == null)
                    return new System.Tuple<string, string>(townAndSettlement, "");
                else
                {
                    string[] buf = townAndSettlement.Split('(');
                    if (buf.Length == 4)
                        return new System.Tuple<string, string>(_TownsAndSettls[townAndSettlement], buf[0] + "(" + buf[1]);
                    else
                        return new System.Tuple<string, string>(_TownsAndSettls[townAndSettlement], townAndSettlement);
                }
            }
            else if (townAndSettlement == "")
                return new System.Tuple<string, string>("", "");

            return null;
        }

        private List<string> SelectNameSocr(MySqlCommand cmd)
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                List<string> results = new List<string>();
                while (reader.Read())
                    results.Add(reader.GetString(0) + " (" + reader.GetString(1) + ")");

                return results;
            }
        }

        private string SelectOneString(MySqlCommand cmd)
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return reader.GetString(0);
                }
                else
                    return null;
            }
        }
    }
}
