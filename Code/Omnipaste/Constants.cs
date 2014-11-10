namespace Omnipaste
{
    using System.Configuration;
    using OmniCommon;

    public class Constants
    {
        public static readonly string AppName = ConfigurationManager.AppSettings[ConfigurationProperties.AppName];

        public const string PublisherName = "Omnipaste";
    }
}