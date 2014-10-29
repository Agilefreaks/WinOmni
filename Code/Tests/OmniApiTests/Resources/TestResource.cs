namespace OmniApiTests.Resources
{
    using System.Configuration;
    using OmniApi.Resources;
    using OmniCommon;
    using Refit;

    public class TestResource : Resource<ITestApi>
    {
        public TestResource()
            : base(CreateResourceApi())
        {
        }

        public static ITestApi CreateResourceApi()
        {
            return RestService.For<ITestApi>(ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl]);
        }
    }
}