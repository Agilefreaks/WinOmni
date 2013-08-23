namespace Omnipaste.Helpers
{
    using OmniCommon;
    using CustomizedClickOnce.Common;

    public class ApplicationInfoFactory
    {
        public static ApplicationInfo Create()
        {
            return new ApplicationInfo
                {
                    AboutLink = AppInfo.AboutLink,
                    HelpLink = AppInfo.HelpLink,
                    ProductName = AppInfo.ApplicationName,
                    PublisherName = AppInfo.PublisherName
                };
        }
    }
}
