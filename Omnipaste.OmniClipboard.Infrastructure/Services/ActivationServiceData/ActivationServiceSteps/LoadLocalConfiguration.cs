﻿namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class LoadLocalConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        public LoadLocalConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public override IExecuteResult Execute()
        {
            _configurationService.Initialize();

            return new ExecuteResult
                       {
                           State =
                               string.IsNullOrEmpty(_configurationService.CommunicationSettings.Channel)
                                   ? SimpleStepStateEnum.Failed
                                   : SimpleStepStateEnum.Successful
                       };
        }
    }
}