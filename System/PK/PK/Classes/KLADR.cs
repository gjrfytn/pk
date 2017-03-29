using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    class KLADR : System.IDisposable
    {
        const string _RawRegionCode = "SUBSTR(code,1,2)";
        const string _RawDistrictCode = "SUBSTR(code,3,3)";
        const string _RawTownCode = "SUBSTR(code,6,3)";
        const string _RawSettlementCode = "SUBSTR(code,9,3)";
        const string _RawStreetCode = "SUBSTR(code,12,4)";

        const string _WhereRegion = "SUBSTR(code, 3) = '00000000000'";
        const string _WhereDistrict = "SUBSTR(code, 6) = '00000000' AND " + _RawDistrictCode + " != '000'";
        const string _WhereTown = "SUBSTR(code, 9) = '00000' AND " + _RawTownCode + " != '000'";
        const string _WhereSettlement = "SUBSTR(code, 12) = '00' AND " + _RawSettlementCode + " != '000'";

        MySqlConnection _Connection;

        public KLADR()
        {
            string connString = @"
                        server = localhost;
                        port=3306;
                        database = kladr;
                        user = root;
                        password = 1234;
                ";
            _Connection = new MySqlConnection(connString);
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
            if (regCode == "")
                return new List<string>();

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr FROM subjects WHERE " + _WhereDistrict + " AND " + _RawRegionCode + "='" + regCode + "';", _Connection);

            return SelectNameSocr(cmd);
        }

        public List<string> GetTowns(string region, string district)
        {
            string regCode = GetRegionCode(region);
            if (regCode == "")
                return new List<string>();

            string distrCode;
            if (district != "")
            {
                distrCode = GetDistrictCode(regCode, district);
                if (distrCode == "")
                    return new List<string>();
            }
            else
                distrCode = "000";

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr FROM subjects WHERE " + _WhereTown + " AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "';", _Connection);
             
            return SelectNameSocr(cmd); ;
        }

        public List<string> GetSettlements(string region, string district, string town)
        {
            string regCode, distrCode, townCode;
            if (!GetCodes(region, district, town, out regCode, out distrCode, out townCode))
                return new List<string>();

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr, `index` FROM subjects WHERE " + _WhereSettlement + " AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "' AND " + _RawTownCode + "='" + townCode + "';", _Connection);

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
                    if (results.Count(s => s == results[i]) > 1) //TODO Нужно?
                    {
                        for (ushort j = (ushort)(i + 1); j < results.Count; ++j)
                            if (results[i] == results[j])
                                if (indexes[i] == indexes[j])
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
            if (!GetCodes(region, district, town, out regCode, out distrCode, out townCode))
                return new List<string>();

            string settlCodesExpr = "";
            List<string> settlementsCodes = new List<string>();
            if (settlement != "")
            {
                settlementsCodes = GetSettlementCodes(regCode, distrCode, townCode, settlement);
                if (settlementsCodes.Count == 0)
                    return new List<string>();

                settlCodesExpr += "(";
                foreach (string code in settlementsCodes)
                    settlCodesExpr += _RawSettlementCode + "='" + code + "' OR ";

                settlCodesExpr = settlCodesExpr.Remove(settlCodesExpr.Length - 4);
                settlCodesExpr += ")";
            }
            else
                settlCodesExpr = _RawSettlementCode + "= '000'";

            string expr = "";
            if (settlementsCodes.Count != 0)
            {
                foreach (string code in settlementsCodes)
                {
                    string commonCode = regCode + distrCode + townCode + code;
                    expr += "code BETWEEN '" + commonCode + "0000' AND '" + commonCode + "9999' OR ";
                }
                expr = expr.Remove(expr.Length - 4);
            }
            else
            {
                string commonCode = regCode + distrCode + townCode + "000";
                expr += "code BETWEEN '" + commonCode + "0000' AND '" + commonCode + "9999'";
            }

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr FROM streets WHERE " + expr + ";", _Connection);

            return SelectNameSocr(cmd);
        }

        public List<string> GetHouses(string region, string district, string town, string settlement, string street)
        {
            string regCode, distrCode, townCode;
            if (!GetCodes(region, district, town, out regCode, out distrCode, out townCode))
                return new List<string>();

            string settlCodesExpr = "";
            List<string> settlementsCodes = new List<string>();
            if (settlement != "")
            {
                settlementsCodes = GetSettlementCodes(regCode, distrCode, townCode, settlement);
                if (settlementsCodes.Count == 0)
                    return new List<string>();

                foreach (string code in settlementsCodes)
                    settlCodesExpr += "' AND " + _RawSettlementCode + "='" + code;
            }
            else
                settlCodesExpr = "' AND " + _RawSettlementCode + "= '000";

            string streetCode = "";
            if (street != "")
            {
                if (settlementsCodes.Count != 0)
                {
                    foreach (string code in settlementsCodes)
                    {
                        streetCode = GetStreetCode(regCode, distrCode, townCode, code, street);
                        if (streetCode != "") //TODO Считаем, что нет улиц с одниковым названием в поселениях (вряд ли такое вообще может произойти).
                            break;
                    }

                    if (streetCode == "")
                        return new List<string>();
                }
                else
                    streetCode = GetStreetCode(regCode, distrCode, townCode, "000", street);
            }
            else
                streetCode = "0000";

            string expr = "";
            if (settlementsCodes.Count != 0)
            {
                foreach (string code in settlementsCodes)
                {
                    string commonCode = regCode + distrCode + townCode + code + streetCode;
                    expr += "code BETWEEN '" + commonCode + "0000' AND '" + commonCode + "9999' OR ";
                }
                expr = expr.Remove(expr.Length - 4);
            }
            else
            {
                string commonCode = regCode + distrCode + townCode + "000" + streetCode;
                expr += "code BETWEEN '" + commonCode + "0000' AND '" + commonCode + "9999'";
            }

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

        bool GetCodes(string region, string district, string town, out string regCode, out string distrCode, out string townCode)
        {
            regCode = GetRegionCode(region);
            if (regCode == "")
            {
                distrCode = "";
                townCode = "";
                return false;
            }

            if (district != "")
            {
                distrCode = GetDistrictCode(regCode, district);
                if (distrCode == "")
                {
                    townCode = "";
                    return false;
                }
            }
            else
                distrCode = "000";

            if (town != "")
            {
                townCode = GetTownCode(regCode, distrCode, town);
                if (townCode == "")
                    return false;
            }
            else
                townCode = "000";

            return true;
        }

        string GetRegionCode(string region)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT " + _RawRegionCode + " FROM subjects WHERE " + _WhereRegion + " AND name = '" +
                region.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "';",
                _Connection
                );

            return SelectOneString(cmd);
        }

        string GetDistrictCode(string regionCode, string district)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT " + _RawDistrictCode + " FROM subjects WHERE " + _WhereDistrict + " AND name = '" +
                district.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "' AND " +
                _RawRegionCode + " = '" + regionCode + "';",
                _Connection
                );

            return SelectOneString(cmd);
        }

        string GetTownCode(string regionCode, string distrCode, string town)
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

        List<string> GetSettlementCodes(string regionCode, string distrCode, string townCode, string settlement)
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

        string GetStreetCode(string regionCode, string distrCode, string townCode, string settlementCode, string street)
        {
            string[] splitStreet = street.Split('(', ')');
            if (splitStreet.Length == 1)
                return "";

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

        List<string> SelectNameSocr(MySqlCommand cmd)
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                List<string> results = new List<string>();
                while (reader.Read())
                    results.Add(reader.GetString(0) + " (" + reader.GetString(1) + ")");

                return results;
            }
        }

        string SelectOneString(MySqlCommand cmd)
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return reader.GetString(0);
                }
                else
                    return "";
            }
        }
    }
}
