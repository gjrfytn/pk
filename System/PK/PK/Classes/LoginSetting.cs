
namespace PK.Classes
{
    class LoginSetting : SharedClasses.FIS.FIS_Authorization.ILoginSetting
    {
        public string Value
        {
            get { return Properties.Settings.Default.FIS_Login; }
            set { Properties.Settings.Default.FIS_Login = value; }
        }

        public void Save() => Properties.Settings.Default.Save();
    }
}
