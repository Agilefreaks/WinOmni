namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class SaveConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        public SaveConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public override IExecuteResult Execute()
        {
            _configurationService.UpdateCommunicationChannel((string)Parameter.Value);

            return new ExecuteResult { State = SingleStateEnum.Successful, Data = this.Parameter.Value };
        }
    }
}