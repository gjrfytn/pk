
namespace PK.Classes
{
    class Settings
    {
        public const string TempPath = ".\\temp\\";

        public static readonly string DocumentsTemplatesPath = Properties.Settings.Default.DocumentsTemplatesPath;

        public static uint CurrentCampaignID
        {
            get { return Properties.Settings.Default.CampaignID; }
        }
    }
}
