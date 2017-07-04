using System.Collections.Generic;

namespace SharedClasses.FIS
{
    public class FIS_Olympic_TEMP
    {
        public class FIS_Olympic_Profile
        {
            public System.Tuple<uint, uint>[] Subjects;
            public uint LevelDictID;
            public uint LevelID;
        }

        public ushort Year;
        public uint? Number;
        public string Name;
        public Dictionary<System.Tuple<uint, uint>, FIS_Olympic_Profile> Profiles;
    }
}
