namespace Omnipaste
{
    using System.Configuration;
    using OmniCommon;

    public class Constants
    {
        public static readonly string AppName = ConfigurationManager.AppSettings[ConfigurationProperties.AppName];

        public const string PublisherName = "Omnipaste";

        public const string FacebookUrl = "https://www.facebook.com/Omnipaste";

        public const string TwitterUrl = "https://twitter.com/omnipaste";

        public const string UserVoiceUrl = "https://omnipasteapp.uservoice.com";
    }
}