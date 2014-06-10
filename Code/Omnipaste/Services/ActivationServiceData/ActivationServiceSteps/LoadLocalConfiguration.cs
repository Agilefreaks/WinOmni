using Ninject;
using Retrofit.Net;

namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class LoadLocalConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;
        
        [Inject]
        public IKernel Kernel { get; set; }

        public LoadLocalConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public override IExecuteResult Execute()
        {
            var result = new ExecuteResult();

            string accessToken = _configurationService.AccessToken;
            string refreshToken = _configurationService.RefreshToken;
            string tokenType = _configurationService.TokenType;
            string clientId = _configurationService.ClientId;
            if (!string.IsNullOrEmpty(accessToken))
            {
                result.Data = accessToken;
                result.State = SimpleStepStateEnum.Successful;
                Kernel.Bind<Authenticator>().ToConstant(new Authenticator
                                                        {
                                                            AccessToken = accessToken, 
                                                            RefreshToken = refreshToken, 
                                                            GrantType = tokenType, 
                                                            ClientId = clientId,
                                                            AuthenticationEndpoint = "oauth2/token"
                                                        });
            }
            else
            {
                result.State = SimpleStepStateEnum.Failed;
            }

            return result;
        }
    }
}