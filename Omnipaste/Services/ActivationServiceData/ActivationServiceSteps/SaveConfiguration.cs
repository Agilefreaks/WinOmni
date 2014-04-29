using Retrofit.Net;

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
            var authenticator = (Authenticator)Parameter.Value;
            _configurationService.Save(authenticator.AccessToken, authenticator.GrantType, authenticator.RefreshToken);

            return new ExecuteResult { State = SingleStateEnum.Successful, Data = authenticator.AccessToken };
        }
    }
}