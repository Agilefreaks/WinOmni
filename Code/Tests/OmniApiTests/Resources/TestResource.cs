namespace OmniApiTests.Resources
{
    using System.Net.Http;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using Refit;

    public class TestResource : ResourceWithAuthorization<ITestApi>
    {
        #region Constructors and Destructors

        public TestResource(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        protected override ITestApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<ITestApi>(httpClient);
        }
    }
}