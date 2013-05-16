using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using OmniCommon.ExtensionMethods;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using RestSharp;

namespace Omnipaste.Services
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