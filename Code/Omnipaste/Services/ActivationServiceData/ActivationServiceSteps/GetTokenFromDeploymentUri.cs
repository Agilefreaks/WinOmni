namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Specialized;
    using OmniCommon.DataProviders;
    using OmniCommon.ExtensionMethods;

    public class GetTokenFromDeploymentUri : ActivationStepBase
    {
        private readonly IApplicationDeploymentInfoProvider _applicationDeploymentInfoProvider;

        private readonly IConfigurationProvider _configurationProvider;

        public GetTokenFromDeploymentUri(IApplicationDeploymentInfoProvider provider, IConfigurationProvider configurationProvider)
        {
            _applicationDeploymentInfoProvider = provider;
            _configurationProvider = configurationProvider;
        }

        public override IExecuteResult Execute()
        {
            var result = new ExecuteResult { State = SimpleStepStateEnum.Failed };
            if (_applicationDeploymentInfoProvider.HasValidActivationUri)
            {
                var token = GetActivationTokenFromDeploymentParameters();
                if (!string.IsNullOrEmpty(token))
                {
                    result.State = SimpleStepStateEnum.Successful;
                    result.Data = token;
                    _configurationProvider["deviceIdentifier"] = token;
                }
            }

            return result;
        }

        private string GetActivationTokenFromDeploymentParameters()
        {
            var deploymentParameters = new NameValueCollection();
            if (_applicationDeploymentInfoProvider.HasValidActivationUri)
            {
                deploymentParameters = _applicationDeploymentInfoProvider.ActivationUri.GetQueryStringParameters();
            }

            return deploymentParameters["token"];
        }
    }
}