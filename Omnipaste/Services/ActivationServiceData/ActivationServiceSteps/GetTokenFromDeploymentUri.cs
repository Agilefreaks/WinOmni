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
            this._applicationDeploymentInfoProvider = provider;
            _configurationProvider = configurationProvider;
        }

        public override IExecuteResult Execute()
        {
            var result = new ExecuteResult { State = SimpleStepStateEnum.Failed };
            if (this._applicationDeploymentInfoProvider.HasValidActivationUri)
            {
                var token = this.GetActivationTokenFromDeploymentParameters();
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
            if (this._applicationDeploymentInfoProvider.HasValidActivationUri)
            {
                deploymentParameters = this._applicationDeploymentInfoProvider.ActivationUri.GetQueryStringParameters();
            }

            return deploymentParameters["token"];
        }
    }
}