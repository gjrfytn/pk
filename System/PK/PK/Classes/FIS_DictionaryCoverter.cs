using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PK.Classes
{
    static class FIS_DictionaryCoverter
    {
        private const uint _DirectionsDictionaryID = 10;
        private const uint _OlympicsDictionaryID = 19;

        public static Dictionary<uint, string> ConvertDictionariesList(XDocument doc)
        {
            #region Contracts
            if (doc == null)
                throw new System.ArgumentNullException(nameof(doc));
            #endregion

            return doc.Root.Elements().ToDictionary(k => uint.Parse(k.Element("Code").Value), v => v.Element("Name").Value);
        }

        public static Dictionary<uint, string> ConvertDictionary(XDocument doc)
        {
            #region Contracts
            if (doc == null)
                throw new System.ArgumentNullException(nameof(doc));
            // if (dictionaryID == _DirectionsDictionaryID || dictionaryID == _OlympicsDictionaryID)
            //      throw new System.ArgumentException("Нельзя получить данные справочника с ID " + dictionaryID + " при помощи этого метода.", nameof(dictionaryID));
            #endregion

            if (doc.Root.Element("DictionaryItems") != null)//TODO Из-за 24 справочника, в котором вопреки спецификации вообще нету элементов. Возможно из-за тестового клиента.
                return doc.Root.Element("DictionaryItems").Elements()
                    .ToDictionary(k => uint.Parse(k.Element("ID").Value), v => v.Element("Name")?.Value ?? "");//TODO [?.Value ?? ""] из-за 9 справочника, в котором вопреки спецификации нету элемента Name. Возможно из-за тестового клиента.

            return new Dictionary<uint, string>();
        }

        public static Dictionary<uint, string[]> ConvertDirectionsDictionary(XDocument doc)
        {
            #region Contracts
            if (doc == null)
                throw new System.ArgumentNullException(nameof(doc));
            // if (dictionaryID == _DirectionsDictionaryID || dictionaryID == _OlympicsDictionaryID)
            //      throw new System.ArgumentException("Нельзя получить данные справочника с ID " + dictionaryID + " при помощи этого метода.", nameof(dictionaryID));
            #endregion

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
                });
        }

        public static Dictionary<uint, SharedClasses.FIS.FIS_Olympic_TEMP> ConvertOlympicsDictionary(XDocument doc, params uint[] profiles)
        {
            #region Contracts
            if (doc == null)
                throw new System.ArgumentNullException(nameof(doc));
            if (profiles == null)
                throw new System.ArgumentNullException(nameof(profiles));
            if (profiles.Length == 0)
                throw new System.ArgumentException("Массив с профилями должен содержать хотя бы один элемент.", nameof(profiles));
            // if (dictionaryID == _DirectionsDictionaryID || dictionaryID == _OlympicsDictionaryID)
            //      throw new System.ArgumentException("Нельзя получить данные справочника с ID " + dictionaryID + " при помощи этого метода.", nameof(dictionaryID));
            #endregion

            string[] strProfiles = System.Array.ConvertAll(profiles, s => s.ToString());

            return doc.Root.Element("DictionaryItems").Elements()
                .Where(
                o => o.Element("Profiles").Elements().Any(p => strProfiles.Contains(p.Element("ProfileID").Value))
                ).ToDictionary(
                k1 => uint.Parse(k1.Element("OlympicID").Value),
                v1 => new SharedClasses.FIS.FIS_Olympic_TEMP
                {
                    Year = ushort.Parse(v1.Element("Year").Value),
                    Number = v1.Element("OlympicNumber") != null ? (uint?)uint.Parse(v1.Element("OlympicNumber").Value) : null,
                    Name = v1.Element("OlympicName").Value,
                    Profiles = v1.Element("Profiles").Elements().ToDictionary(
                        k2 => new System.Tuple<uint, uint>(
                            39,
                            uint.Parse(k2.Element("ProfileID").Value)
                            ),
                        v2 => new SharedClasses.FIS.FIS_Olympic_TEMP.FIS_Olympic_Profile
                        {
                            Subjects = v2.Element("Subjects").Elements().Where(/*TODO в ФИС сидят идиоты 2*/e => e.Value != "0")
                            .Select(s => new System.Tuple<uint, uint>(1, uint.Parse(s.Value))).ToArray(),
                            LevelDictID = 3,
                            LevelID = uint.Parse(v2.Element("LevelID").Value) != 0 ? uint.Parse(v2.Element("LevelID").Value) : 255 //TODO Временно, т.к. в ФИС сидят идиоты
                        })
                });
        }
    }
}
