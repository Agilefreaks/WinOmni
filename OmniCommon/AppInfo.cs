namespace OmniCommon
{
    using System;
    using System.Configuration;

    public class AppInfo
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

        protected AppInfo()
        {
        }

        public static Uri GetTokenLink
        {
            get
            {
                return new Uri(BaseUrl + "whatsmytoken");
            }
        }
    }
}