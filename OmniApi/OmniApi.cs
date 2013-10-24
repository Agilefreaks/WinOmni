namespace OmniApi
{
    using global::OmniApi.Resources;
    using RestSharp;

    public static class OmniApi
    {
        public static string BaseUrl { get; set; }

        public static IRestClient RestClient { get; set; }

        public static IActivationTokens ActivationTokens
        {
            get
            {
                return ApiResource.Build<ActivationTokens>(BaseUrl, RestClient);
            }
        }

        public static IClippings Clippings
        {
            get
            {
                return ApiResource.Build<Clippings>(BaseUrl, RestClient);
            }
        }
    }
}