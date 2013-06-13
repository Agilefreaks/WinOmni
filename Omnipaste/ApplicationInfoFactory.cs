namespace Omnipaste
{
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

        public const string AboutLink = "http://www.omnipasteapp.com/#about";

        public const string HelpLink = "http://www.omnipasteapp.com/#contact";

        public const string PublisherName = "Omnipaste";

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