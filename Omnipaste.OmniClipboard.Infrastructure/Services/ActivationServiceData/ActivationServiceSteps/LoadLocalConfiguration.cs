namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class LoadLocalConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        public LoadLocalConfiguration(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }

        public override IExecuteResult Execute()
        {
            this._configurationService.Initialize();

            return new ExecuteResult
                       {
                           State =
                               string.IsNullOrEmpty(this._configurationService.CommunicationSettings.Channel)
                                   ? SimpleStepStateEnum.Failed
                                   : SimpleStepStateEnum.Successful
                       };
        }
    }
}