namespace Omnipaste.Services
{
    using System.Configuration;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using RestSharp;

    public class ClickOnceActivationDataProvider : IActivationDataProvider
    {
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
            var restClient = new RestClient(ConfigurationManager.AppSettings["baseUrl"]);
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