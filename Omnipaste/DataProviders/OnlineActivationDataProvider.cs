namespace Omnipaste.DataProviders
{
    using System.Configuration;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using OmniCommon.DataProviders;
    using RestSharp;

    public class OnlineActivationDataProvider: IActivationDataProvider
    {
        private static string _tokenLink;

        public static string TokenLink
        {
            get
            {
                return _tokenLink ?? (_tokenLink = ConfigurationManager.AppSettings["baseUrl"] + "whatsmytoken");
            }
        }

        public ActivationData GetActivationData(string token)
        {
            ActivationData activationData = null;
            if (!string.IsNullOrEmpty(token))
            {
                var request = CreateRequest(token);
                var restResponse = PerformRequest(request);
                activationData = restResponse.Data;
            }

            return activationData ?? new ActivationData();
        }

        private static IRestResponse<ActivationData> PerformRequest(IRestRequest request)
        {
            var restClient = new RestClient(ConfigurationManager.AppSettings["apiUrl"]);
            var restResponse = restClient.Execute<ActivationData>(request);

            return restResponse;
        }

        private static RestRequest CreateRequest(string token)
        {
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
            var request = new RestRequest("users/activate/{token}", Method.GET);
            request.AddUrlSegment("token", token);

            return request;
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}