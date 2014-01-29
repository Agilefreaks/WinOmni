namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
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

            var executeResult = new ExecuteResult
                                    {
                                        Data = _configurationService.CommunicationSettings.Channel
                                    };

            executeResult.State = string.IsNullOrEmpty((string)executeResult.Data) ? SimpleStepStateEnum.Failed : SimpleStepStateEnum.Successful;
            return executeResult;
        }
    }
}