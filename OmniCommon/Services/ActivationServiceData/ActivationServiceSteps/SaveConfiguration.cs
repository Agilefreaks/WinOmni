namespace OmniCommon.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class SaveConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        private readonly string _channel;

        public SaveConfiguration(IConfigurationService configurationService, string payload)
        {
            _configurationService = configurationService;
            _channel = payload;
        }

        public override IExecuteResult Execute()
        {
            _configurationService.UpdateCommunicationChannel(_channel);

            return new ExecuteResult { State = SingleStateEnum.Successful };
        }
    }
}