namespace Omnipaste
{
    using System.Configuration;
    using CustomizedClickOnce.Common;

    public class ApplicationInfoFactory
    {
#if DEBUG
        public const string ApplicationName = "Omnipaste-Debug";
#elif STAGING
        public const string ApplicationName = "Omnipaste-Staging";
#else
        public const string ApplicationName = "Omnipaste";
#endif

        public const string PublisherName = "Omnipaste";

        private static string _baseUrl;

        public static string BaseUrl
        {
            get
            {
                return _baseUrl ?? (_baseUrl = ConfigurationManager.AppSettings["baseUrl"]);
            }
        }

        public static string AboutLink
        {
            get
            {
                return BaseUrl + "#about";
            }
        }

        public static string HelpLink
        {
            get
            {
                return BaseUrl + "#contact";
            }
        }

        protected ApplicationInfoFactory()
        {
        }

        public static ApplicationInfo Create()
        {
            return new ApplicationInfo
                       {
                           AboutLink = AboutLink,
                           HelpLink = HelpLink,
                           ProductName = ApplicationName,
                           PublisherName = PublisherName
                       };
        }
    }
}