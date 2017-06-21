using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using System.Linq;

namespace PK.Classes
{
    static class FIS_Connector
    {
        public class FIS_Exception : System.Exception
        {
            public FIS_Exception(string message) : base(message) { }
        }

        public static Dictionary<uint, string> GetDictionaries(string login, string password)
        {
            #region Contracts
            CheckLoginAndPassword(login, password);
            #endregion

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(MakeRoot(login, password, null).ToString());
            XDocument doc = GetResponse("http://priem.edu.ru:8000/import/importservice.svc/dictionary", byteArray);

            return doc.Root.Elements().ToDictionary(k => uint.Parse(k.Element("Code").Value), v => v.Element("Name").Value);
        }

        public static Dictionary<uint, string> GetDictionaryItems(string login, string password, uint dictionaryID)
        {
            #region Contracts
            CheckLoginAndPassword(login, password);
            #endregion

            if (dictionaryID == 10 || dictionaryID == 19)
                throw new System.ArgumentException("Нельзя получить данные справочника с ID " + dictionaryID + " при помощи этого метода.", nameof(dictionaryID));

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(MakeRoot(login, password, new XElement("GetDictionaryContent",
                new XElement("DictionaryCode", dictionaryID)
                )).ToString());

            XDocument doc = GetResponse("http://priem.edu.ru:8000/import/importservice.svc/dictionarydetails", byteArray);

            if (doc.Root.Element("DictionaryItems") != null)//TODO Из-за 24 справочника, в котором вопреки спецификации вообще нету элементов. Возможно из-за тестового клиента.
                return doc.Root.Element("DictionaryItems").Elements()
                    .ToDictionary(k => uint.Parse(k.Element("ID").Value), v => v.Element("Name")?.Value ?? "");//TODO [?.Value ?? ""] из-за 9 справочника, в котором вопреки спецификации нету элемента Name. Возможно из-за тестового клиента.

            return new Dictionary<uint, string>();
        }

        public static Dictionary<uint, string[]> GetDirectionsDictionaryItems(string login, string password)
        {
            #region Contracts
            CheckLoginAndPassword(login, password);
            #endregion

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(MakeRoot(login, password, new XElement("GetDictionaryContent",
                new XElement("DictionaryCode", 10)
                )).ToString());

            XDocument doc = GetResponse("http://priem.edu.ru:8000/import/importservice.svc/dictionarydetails", byteArray);

            return doc.Root.Element("DictionaryItems").Elements().ToDictionary(
                k => uint.Parse(k.Element("ID").Value),
                v => new string[]
                {
                    v.Element("Name").Value,
                    v.Element("NewCode").Value,//v.Element("Code").Value, TODO Не знаю, почему так.
                    v.Element("QualificationCode").Value,
                    "",//v.Element("Period").Value, TODO Почему-то его нет.
                    v.Element("UGSCode").Value,
                    v.Element("UGSName").Value
                }
                );
        }

        public static Dictionary<uint, FIS_Olympic_TEMP> GetOlympicsDictionaryItems(string login, string password, params uint[] profiles)
        {
            #region Contracts
            CheckLoginAndPassword(login, password);
            if (profiles == null)
                throw new System.ArgumentNullException(nameof(profiles));
            if (profiles.Length == 0)
                throw new System.ArgumentException("Массив с профилями должен содержать хотя бы один элемент.", nameof(profiles));
            #endregion

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(MakeRoot(login, password, new XElement("GetDictionaryContent",
                new XElement("DictionaryCode", 19)
                )).ToString());

            XDocument doc = GetResponse("http://priem.edu.ru:8000/import/importservice.svc/dictionarydetails", byteArray);

            string[] strProfiles = System.Array.ConvertAll(profiles, s => s.ToString());

            return doc.Root.Element("DictionaryItems").Elements()
                .Where(
                o => o.Element("Profiles").Elements().Any(p => strProfiles.Contains(p.Element("ProfileID").Value))
                ).ToDictionary(
                k1 => uint.Parse(k1.Element("OlympicID").Value),
                v1 => new FIS_Olympic_TEMP
                {
                    Year = ushort.Parse(v1.Element("Year").Value),
                    Number = v1.Element("OlympicNumber") != null ? (uint?)uint.Parse(v1.Element("OlympicNumber").Value) : null,
                    Name = v1.Element("OlympicName").Value,
                    Profiles = v1.Element("Profiles").Elements().ToDictionary(
                        k2 => new System.Tuple<uint, uint>(
                            39,
                            uint.Parse(k2.Element("ProfileID").Value)
                            ),
                        v2 => new FIS_Olympic_TEMP.FIS_Olympic_Profile
                        {
                            Subjects = v2.Element("Subjects").Elements().Where(/*TODO в ФИС сидят идиоты 2*/e => e.Value != "0")
                            .Select(s => new System.Tuple<uint, uint>(1, uint.Parse(s.Value))).ToArray(),
                            LevelDictID = 3,
                            LevelID = uint.Parse(v2.Element("LevelID").Value) != 0 ? uint.Parse(v2.Element("LevelID").Value) : 255 //TODO Временно, т.к. в ФИС сидят идиоты
                        })
                });
        }

        public static string Export(string address, string login, string password, XElement packageData)
        {
            #region Contracts
            CheckLoginAndPassword(login, password);
            if (packageData == null)
                throw new System.ArgumentNullException(nameof(packageData));
            if (string.IsNullOrWhiteSpace(address))
                throw new System.ArgumentException("Некорректный адрес.", nameof(address));
            #endregion

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(MakeRoot(login, password, packageData).ToString());

            XDocument doc = GetResponse(address + "/import/importservice.svc/import", byteArray);

            if (doc.Root.Name == "Error")
                throw new FIS_Exception(doc.Root.Element("ErrorText").Value);

            return doc.Root.Element("PackageID").Value;
        }

        private static XDocument GetResponse(string uri, byte[] requestData)
        {
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";

            request.ContentType = "text/xml";
            request.ContentLength = requestData.Length;

            System.IO.Stream dataStream = request.GetRequestStream();
            dataStream.Write(requestData, 0, requestData.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();

            //MessageBox.Show(((HttpWebResponse)response).StatusDescription);

            dataStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);

            XDocument doc = XDocument.Parse(reader.ReadToEnd());

            reader.Close();
            dataStream.Close();
            response.Close();

            return doc;
        }

        private static void CheckLoginAndPassword(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new System.ArgumentException("Некорректный логин.", nameof(login));
            if (string.IsNullOrWhiteSpace(password))
                throw new System.ArgumentException("Некорректный пароль.", nameof(password));
        }

        private static XDocument MakeRoot(string login, string password, XElement data)
        {
            return new XDocument(new XElement("Root",
                new XElement("AuthData",
                new XElement("Login", login),
                new XElement("Pass", password)
                ),
                data
                ));
        }
    }
}
