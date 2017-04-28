using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using System.Linq;

namespace PK.Classes
{
    class FIS_Connector
    {
        public class FIS_Exception : System.Exception
        {
            public FIS_Exception(string message) : base(message) { }
        }

        private readonly string _Login;
        private readonly string _Password;

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

            if (doc.Root.Name == "Error")
                throw new FIS_Exception(doc.Root.Element("ErrorText").Value);*/
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

        public Dictionary<uint, FIS_Olympic_TEMP> GetOlympicsDictionaryItems(byte years)
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

            return doc.Root.Element("DictionaryItems").Elements()
                .Where(//TODO нормальный фильтр
                o => o.Element("Year") != null &&
                ushort.Parse(o.Element("Year").Value) >= System.DateTime.Now.Year - (years - 1) &&
                o.Element("Profiles").Elements().Any(p => p.Element("ProfileID").Value == "4" || p.Element("ProfileID").Value == "7")
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

        public void Export(FIS_ExportClasses.PackageData data)
        {
            /*byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(new ImportClasses.Root(
                new ImportClasses.AuthData(_Login, _Password), data).ConvertToXElement().ToString());

            XDocument doc = GetResponse("http://priem.edu.ru:8000/import/importservice.svc/import", byteArray);
            System.Windows.Forms.MessageBox.Show(doc.ToString());*/

            new FIS_ExportClasses.Root(new FIS_ExportClasses.AuthData(_Login, _Password), data).ConvertToXElement().Save("testfile2.xml");
        }

        private XDocument GetResponse(string uri, byte[] requestData)
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
