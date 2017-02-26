using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
//using web_connector_test.ImportClasses;

namespace PK
{
    class FIS_Connector
    {
        string _Login;
        string _Password;

        public FIS_Connector(/*string address,*/string login, string password) //TODO
        {
            _Login = login;
            _Password = password;
            //
            /*byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(
                    @"<Root>
                        <AuthData>
                            <Login>" + _Login + @"</Login>
                            <Pass>" + _Password + @"</Pass>
                        </AuthData>
                    </Root>"
            );
            XDocument doc = GetResponse("http://priem.edu.ru:8000/import/importservice.svc/import", byteArray);

            if (doc.Root.Name == "Error" && doc.Root.Element("ErrorText").Value.Contains("Ошибка авторизации")) //TODO
                throw new System.Exception(doc.Root.Element("ErrorText").Value);*/
                //
        }

        public Dictionary<uint, string> GetDictionaries()
        {
            //
            /*byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(
                    @"<Root>
                        <AuthData>
                            <Login>" + _Login + @"</Login>
                            <Pass>" + _Password + @"</Pass>
                        </AuthData>
                    </Root>"
            );
            XDocument doc = GetResponse("http://priem.edu.ru:8000/import/importservice.svc/dictionary", byteArray);*/
            //

            XDocument doc = XDocument.Load("tempDictList.xml");
            Dictionary<uint, string> dictionaries = new Dictionary<uint, string>();

            foreach (var dict in doc.Root.Elements())
                dictionaries.Add(uint.Parse(dict.Element("Code").Value), dict.Element("Name").Value);

            return dictionaries;
        }

        public Dictionary<uint, string> GetDictionaryItems(uint dictionaryID)
        {
            if (dictionaryID == 10 || dictionaryID == 19)
                throw new System.ArgumentException("Нельзя получить данные справочника с ID " + dictionaryID + " при помощи этого метода.", "dictionaryID");
            //
            /*byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(
                    @"<Root>
                    <GetDictionaryContent>
                        <DictionaryCode>" + dictionaryID + @"</DictionaryCode>
                    </GetDictionaryContent>
                    <AuthData>
                        <Login>" + _Login + @"</Login>
                        <Pass>" + _Password + @"</Pass>
                    </AuthData>
                </Root>"
            );
          XDocument doc=GetResponse("http://priem.edu.ru:8000/import/importservice.svc/dictionarydetails", byteArray);*/
            //

            XDocument doc = XDocument.Load(".\\tempDictionaries\\dicn" + dictionaryID+".xml");
            Dictionary<uint, string> dictionaryItems = new Dictionary<uint, string>();

            if (doc.Root.Element("DictionaryItems") != null)//TODO Из-за 24 справочника, в котором вопреки спецификации вообще нету элементов. Возможно из-за тестового клиента.
                foreach (var item in doc.Root.Element("DictionaryItems").Elements())
                    if (item.Element("Name") != null) //TODO Из-за 9 справочника, в котором вопреки спецификации нету элемента Name. Возможно из-за тестового клиента.
                        dictionaryItems.Add(uint.Parse(item.Element("ID").Value), item.Element("Name").Value);
                    else
                        dictionaryItems.Add(uint.Parse(item.Element("ID").Value), "");

            return dictionaryItems;
        }

        public Dictionary<uint, string[]> GetDirectionsDictionaryItems()
        {
            //
            /* byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(
                     @"<Root>
                     <GetDictionaryContent>
                         <DictionaryCode>" + 10 + @"</DictionaryCode>
                     </GetDictionaryContent>
                     <AuthData>
                         <Login>" + _Login + @"</Login>
                         <Pass>" + _Password + @"</Pass>
                     </AuthData>
                 </Root>"
             );
           XDocument doc=GetResponse("http://priem.edu.ru:8000/import/importservice.svc/dictionarydetails", byteArray);*/
            //

            XDocument doc = XDocument.Load(".\\tempDictionaries\\dicn" + 10 + ".xml");
            Dictionary<uint, string[]> dictionaryItems = new Dictionary<uint, string[]>();

            foreach (var item in doc.Root.Element("DictionaryItems").Elements())
                dictionaryItems.Add(
                    uint.Parse(item.Element("ID").Value),
                    new string[]
                    {
                            item.Element("Name").Value,
                            item.Element("NewCode").Value,//item.Element("Code").Value, TODO Не знаю, почему так.
                            item.Element("QualificationCode").Value,
                            "",//item.Element("Period").Value, TODO Почему-то его нет.
                            item.Element("UGSCode").Value,
                            item.Element("UGSName").Value
                    }
                    );

            return dictionaryItems;
        }

        /*public void Import(PackageData data)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(new Root(
                new AuthData(_Login, _Password), data).ConvertToXElement().ToString());

            XDocument doc = GetResponse("http://priem.edu.ru:8000/import/importservice.svc/import", byteArray);
            System.Windows.Forms.MessageBox.Show(doc.ToString());
        }*/

        XDocument GetResponse(string uri, byte[] requestData)
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
    }
}
