namespace OmniApi
{
    using System;
    using global::OmniApi.Resources;
    using RestSharp;

    public static class OmniApi
    {
        public const string Version = "v1";

        private static string _baseUrl;

        public static string BaseUrl
        {
            get
            {
                return _baseUrl;
            }

            set
            {
                _baseUrl = value;
                RestClient.BaseUrl = ApiUrl;
            }
        }

        public static string ApiUrl
        {
            get
            {
                Uri uri = new Uri(new Uri(BaseUrl), string.Format("api/{0}", Version));
                return uri.ToString();
            }
        }

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

        static OmniApi()
        {
            RestClient = new RestClient();
        }

        public static void RegisterDevice(string deviceId)
        {
            
        }
    }
}