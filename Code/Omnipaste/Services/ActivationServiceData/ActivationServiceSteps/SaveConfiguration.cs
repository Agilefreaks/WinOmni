namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Models;
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
            var token = (Token)Parameter.Value;
            _configurationService.SaveAuthSettings(token.AccessToken, token.RrefreshToken);

            return new ExecuteResult { State = SingleStateEnum.Successful, Data = token.AccessToken };
        }
    }
}