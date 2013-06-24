namespace Omnipaste
{
    using System.Configuration;
    using CustomizedClickOnce.Common;

    public class ApplicationInfoFactory
    {
        public const string PublisherName = "Omnipaste";

        private static string _baseUrl;

        private static string _applicatioName;

        public static string ApplicationName
        {
            get
            {
                return _applicatioName ?? (_applicatioName = ConfigurationManager.AppSettings["appName"]);
            }
        }

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