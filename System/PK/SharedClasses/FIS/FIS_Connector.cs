using System.Net;
using System.Xml.Linq;

namespace SharedClasses.FIS
{
    public static class FIS_Connector
    {
        public class FIS_Exception : System.Exception
        {
            public FIS_Exception(string message) : base(message) { }
        }

        public static XDocument GetDictionariesList(string address, string login, string password)
        {
            #region Contracts
            CheckLoginAndPassword(login, password);
            #endregion

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(MakeRoot(login, password, null).ToString());
            return GetResponse(address + "/import/importservice.svc/dictionary", byteArray);
        }

        public static XDocument GetDictionary(string address, string login, string password, uint dictionaryID)
        {
            #region Contracts
            CheckLoginAndPassword(login, password);
            #endregion

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(MakeRoot(login, password, new XElement("GetDictionaryContent",
                new XElement("DictionaryCode", dictionaryID)
                )).ToString());

            return GetResponse(address + "/import/importservice.svc/dictionarydetails", byteArray);
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
