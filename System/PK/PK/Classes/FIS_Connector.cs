using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using System.Linq;
//using web_connector_test.ImportClasses;

namespace PK.Classes
{
    using Olympic =
        Dictionary<uint, System.Tuple<uint?, string, Dictionary<System.Tuple<uint, uint>, System.Tuple<System.Tuple<uint, uint>[], uint, uint>>>>;

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

            return doc.Root.Elements().ToDictionary(k => uint.Parse(k.Element("Code").Value), v => v.Element("Name").Value);
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

            XDocument doc = XDocument.Load(".\\tempDictionaries\\dicn" + dictionaryID + ".xml");

            if (doc.Root.Element("DictionaryItems") != null)//TODO Из-за 24 справочника, в котором вопреки спецификации вообще нету элементов. Возможно из-за тестового клиента.
                return doc.Root.Element("DictionaryItems").Elements()
                    .ToDictionary(k => uint.Parse(k.Element("ID").Value), v => v.Element("Name")?.Value ?? "");//TODO [?.Value ?? ""] из-за 9 справочника, в котором вопреки спецификации нету элемента Name. Возможно из-за тестового клиента.

            return new Dictionary<uint, string>();
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

        public Olympic GetOlympicsDictionaryItems()
        {
            //
            /* byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(
                     @"<Root>
                     <GetDictionaryContent>
                         <DictionaryCode>" + 19 + @"</DictionaryCode>
                     </GetDictionaryContent>
                     <AuthData>
                         <Login>" + _Login + @"</Login>
                         <Pass>" + _Password + @"</Pass>
                     </AuthData>
                 </Root>"
             );
           XDocument doc=GetResponse("http://priem.edu.ru:8000/import/importservice.svc/dictionarydetails", byteArray);*/
            //

            XDocument doc = XDocument.Load(".\\tempDictionaries\\dicn" + 19 + ".xml");

            return doc.Root.Element("DictionaryItems").Elements().ToDictionary(
                k1 => uint.Parse(k1.Element("OlympicID").Value),
                v1 => new System.Tuple<uint?, string, Dictionary<System.Tuple<uint, uint>, System.Tuple<System.Tuple<uint, uint>[], uint, uint>>>(
                    v1.Element("OlympicNumber") != null ? (uint?)uint.Parse(v1.Element("OlympicNumber").Value) : null,
                    v1.Element("OlympicName").Value,
                    v1.Element("Profiles").Elements().ToDictionary(
                        k2 => new System.Tuple<uint, uint>(
                            39,
                            uint.Parse(k2.Element("ProfileID").Value)
                            ),
                        v2 => new System.Tuple<System.Tuple<uint, uint>[], uint, uint>(
                            v2.Element("Subjects").Elements().Where(/*TODO в ФИС сидят идиоты 2*/e => e.Value != "0")
                            .Select(s => new System.Tuple<uint, uint>(1, uint.Parse(s.Value))).ToArray(),
                            3,
                            uint.Parse(v2.Element("LevelID").Value) != 0 ? uint.Parse(v2.Element("LevelID").Value) : 255 //TODO Временно, т.к. в ФИС сидят идиоты
                            ))));
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
