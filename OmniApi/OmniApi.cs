namespace OmniApi
{
    using System;
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
        
        static OmniApi()
        {
            RestClient = new RestClient();
        }
    }
}