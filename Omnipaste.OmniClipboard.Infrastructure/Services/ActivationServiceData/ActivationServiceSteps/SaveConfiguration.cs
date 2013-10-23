﻿namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class SaveConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        private readonly string _channel;

        public SaveConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public override IExecuteResult Execute()
        {
            _configurationService.UpdateCommunicationChannel((string)Parameter.Value);

            return new ExecuteResult { State = SingleStateEnum.Successful };
        }
    }
}