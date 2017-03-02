using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace PK
{
    class KLADR : System.IDisposable
    {
        const string _RawRegionCode = "SUBSTRING(code,1,2)";
        const string _RawDistrictCode = "SUBSTRING(code,3,3)";
        const string _RawTownCode = "SUBSTRING(code,6,3)";
        const string _RawSettlementCode = "SUBSTRING(code,9,3)";

        const string _WhereRegion = "SUBSTRING(code, 3) = '00000000000'";
        const string _WhereDistrict = "SUBSTRING(code, 6) = '00000000'" + " AND " + _RawDistrictCode + " != '000'";
        const string _WhereTown = "SUBSTRING(code, 9) = '00000'" + " AND " + _RawTownCode + " != '000'";
        const string _WhereSettlement = "SUBSTRING(code, 12) = '00'" + " AND " + _RawSettlementCode + " != '000'";

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
            MySqlCommand cmd = new MySqlCommand("SELECT name, socr,code FROM subjects WHERE " + _WhereRegion + ";", _Connection);

            return Select(cmd);
        }

        public List<string> GetDistricts(string region)
        {
            string regCode = GetRegionCode(region);
            if (regCode == "")
                return new List<string>();

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr,code FROM subjects WHERE " + _WhereDistrict + " AND " + _RawRegionCode + "='" + regCode + "';", _Connection);

            return Select(cmd);
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

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr,code FROM subjects WHERE " + _WhereTown + " AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "';", _Connection);

            return Select(cmd);
        }

        public List<string> GetSettlements(string region, string district, string town)
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

            string townCode;
            if (town != "")
            {
                townCode = GetTownCode(regCode, distrCode, town);
                if (distrCode == "")
                    return new List<string>();
            }
            else
                townCode = "000";

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr,code FROM subjects WHERE " + _WhereSettlement + " AND " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "' AND " + _RawTownCode + "='" + townCode + "';", _Connection);

            return Select(cmd);
        }

        public List<string> GetStreets(string region, string district, string town, string settlement)
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

            string townCode;
            if (town != "")
            {
                townCode = GetTownCode(regCode, distrCode, town);
                if (distrCode == "")
                    return new List<string>();
            }
            else
                townCode = "000";

            string settlementsCodes = "";
            if (settlement != "")
            {
                List<string> codes = GetSettlementCodes(regCode, distrCode, townCode, settlement);
                if (codes.Count == 0)
                    return new List<string>();

                foreach (string code in codes)
                    settlementsCodes += "' AND " + _RawSettlementCode + "='" + code;
            }
            else
                settlementsCodes = "' AND " + _RawSettlementCode + "= '000";

            MySqlCommand cmd = new MySqlCommand("SELECT name, socr,code FROM streets WHERE " + _RawRegionCode + "='" + regCode + "' AND " + _RawDistrictCode + "='" + distrCode + "' AND " + _RawTownCode + "='" + townCode + settlementsCodes + "';", _Connection);

            return Select(cmd);
        }

        string GetRegionCode(string region)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT " + _RawRegionCode + " FROM subjects WHERE " + _WhereRegion + " AND name='" +
                region.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "';",
                _Connection
                );

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

        string GetDistrictCode(string regionCode, string district)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT " + _RawDistrictCode + " FROM subjects WHERE " + _WhereDistrict + " AND name='" +
                district.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "' AND " +
                _RawRegionCode + " = '" + regionCode + "';",
                _Connection
                );

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

        string GetTownCode(string regionCode, string distrCode, string town)
        {
            MySqlCommand cmd = new MySqlCommand(
                 "SELECT " + _RawTownCode + " FROM subjects WHERE " + _WhereTown + " AND name='" +
                 town.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "' AND " +
                 _RawRegionCode + " = '" + regionCode + "' AND " +
                 _RawDistrictCode + " = '" + distrCode + "';",
                 _Connection
                 );

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

        List<string> GetSettlementCodes(string regionCode, string distrCode, string townCode, string settlement)
        {
            MySqlCommand cmd = new MySqlCommand(
                 "SELECT " + _RawSettlementCode + " FROM subjects WHERE " + _WhereSettlement + " AND name='" +
                 settlement.Split(new string[] { " (" }, System.StringSplitOptions.None)[0] + "' AND " +
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

        List<string> Select(MySqlCommand cmd)
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                List<string> results = new List<string>();
                while (reader.Read())
                    results.Add(reader.GetString(0) + " (" + reader.GetString(1) + ")" + reader.GetString(2));

                return results;
            }
        }
    }
}
