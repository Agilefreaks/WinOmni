namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class SaveConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        private readonly string _channel;

        public SaveConfiguration(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }

        public override IExecuteResult Execute()
        {
            this._configurationService.UpdateCommunicationChannel((string)this.Parameter.Value);

            return new ExecuteResult { State = SingleStateEnum.Successful };
        }
    }
}