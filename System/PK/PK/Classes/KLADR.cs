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

        public KLADR(string user, string password)
        {
            _Connection = new MySqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["kladr"].ConnectionString +
                " user = " + user + "; password = " + password + ";"
                );
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
            string regCode = GetRegionCode(region);
            if (regCode == null)
                return new List<string>();

            string distrCode;
            if (district != "")
            {
                distrCode = GetDistrictCode(regCode, district);
                if (distrCode == null)
                    return new List<string>();
            }
            else
                distrCode = "000";

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr FROM subjects WHERE " + _WhereTown + " AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "';", _Connection);

            return SelectNameSocr(cmd);
        }

        public List<string> GetSettlements(string region, string district, string town)
        {
            string regCode, distrCode, townCode;
            if (!GetCodes(region, district, town, out regCode, out distrCode, out townCode))
                return new List<string>();

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr, `index` FROM subjects WHERE " + _WhereSettlement + " AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "' AND " + _RawTownCode + "='" + townCode + "';", _Connection);

            /*т.к. в одном субъекте могут быть нас. п. с одинаковыми названиями и категориями,
             мы прибавляем к одноимённым элементам их индекс.*/
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                List<string> results = new List<string>();
                List<string> indexes = new List<string>();
                while (reader.Read())
                {
                    results.Add(reader.GetString(0) + " (" + reader.GetString(1) + ")");
                    indexes.Add(reader.GetValue(2)?.ToString() ?? "");
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
                                    results[j] += " [" + indexes[j] + "]";

                        results[i] += " [" + indexes[i] + "]";
                    }

                return results;
            }
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

        private bool GetCodes(string region, string district, string town, out string regCode, out string distrCode, out string townCode)
        {
            regCode = GetRegionCode(region);
            if (regCode == null)
            {
                distrCode = null;
                townCode = null;
                return false;
            }

            if (district != "")
            {
                distrCode = GetDistrictCode(regCode, district);
                if (distrCode == null)
                {
                    townCode = null;
                    return false;
                }
            }
            else
                distrCode = "000";

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
            MySqlCommand cmd = new MySqlCommand(
                 "SELECT " + _RawTownCode + " FROM subjects WHERE " + _WhereTown + " AND name = '" +
                 town.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "' AND " +
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
            string[] splitSettl = settlement.Split('(', ')', '[', ']');
            if (splitSettl.Length == 1)
                return new List<string>();

            string name = splitSettl[0].Remove(splitSettl[0].Length - 1);
            string socr = splitSettl[1];
            string index = "";
            if (splitSettl.Length > 3)
                index = splitSettl[3];

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
            string[] splitStreet = street.Split('(', ')');
            if (splitStreet.Length == 1)
                return null;

            string name = splitStreet[0].Remove(splitStreet[0].Length - 1);
            string socr = splitStreet[1];

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
