﻿namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Specialized;
    using OmniCommon.DataProviders;
    using OmniCommon.ExtensionMethods;

    public class GetTokenFromDeploymentUri : ActivationStepBase
    {
        #region Fields

        private readonly IApplicationDeploymentInfoProvider _applicationDeploymentInfoProvider;

        private readonly IConfigurationProvider _configurationProvider;

        #endregion

        #region Constructors and Destructors

        public GetTokenFromDeploymentUri(
            IApplicationDeploymentInfoProvider provider,
            IConfigurationProvider configurationProvider)
        {
            _applicationDeploymentInfoProvider = provider;
            _configurationProvider = configurationProvider;
        }

        #endregion

        #region Public Methods and Operators

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

        #endregion

        #region Methods

        private string GetActivationTokenFromDeploymentParameters()
        {
            var deploymentParameters = new NameValueCollection();
            if (_applicationDeploymentInfoProvider.HasValidActivationUri)
            {
                deploymentParameters = _applicationDeploymentInfoProvider.ActivationUri.GetQueryStringParameters();
            }

            return deploymentParameters["token"];
        }

        #endregion
    }
}