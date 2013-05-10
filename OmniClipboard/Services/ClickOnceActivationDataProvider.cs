using System.Collections.Specialized;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Omniclipboard.ExtensionMethods;
using RestSharp;

namespace Omniclipboard.Services
{
    public class ClickOnceActivationDataProvider : IActivationDataProvider
    {
        private readonly IApplicationDeploymentInfo _applicationDeploymentInfo;

        public ClickOnceActivationDataProvider(IApplicationDeploymentInfo applicationDeploymentInfo)
        {
            _applicationDeploymentInfo = applicationDeploymentInfo;
        }

        public ActivationData GetActivationData()
        {
            ActivationData activationData = null;
            var deploymentParameters = GetDeploymentParameters();
            var token = deploymentParameters["token"];
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
            var restClient = new RestClient("https://omnipasteapp.com/api");
            var restResponse = restClient.Execute<ActivationData>(request);

            return restResponse;
        }

        private static RestRequest CreateRequest(string token)
        {
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
            var request = new RestRequest("activate/{token}.json", Method.GET);
            request.AddUrlSegment("token", token);

            return request;
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private NameValueCollection GetDeploymentParameters()
        {
            var deploymentParameters = new NameValueCollection();
            if (_applicationDeploymentInfo.HasValidActivationUri)
            {
                deploymentParameters = _applicationDeploymentInfo.ActivationUri.GetQueryStringParameters();
            }

            return deploymentParameters;
        }
    }
}