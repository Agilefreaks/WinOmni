namespace OmniApi.Resources
{
    using System.Configuration;
    using OmniCommon;
    using Refit;

    public abstract class Resource<T>
    {
        protected readonly T ResourceApi;

        protected Resource()
        {
            ResourceApi = RestService.For<T>(ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl]);
        }
    }
}