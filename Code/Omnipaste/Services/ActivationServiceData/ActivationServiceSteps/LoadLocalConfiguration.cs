namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using Ninject;
    using OmniCommon.Interfaces;
    using Retrofit.Net;

    public class LoadLocalConfiguration : ActivationStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public LoadLocalConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        #endregion

        #region Public Methods and Operators

        public override IExecuteResult Execute()
        {
            var result = new ExecuteResult();

            var accessToken = _configurationService.AccessToken;
            var refreshToken = _configurationService.RefreshToken;
            var tokenType = _configurationService.TokenType;
            var clientId = _configurationService.ClientId;
            if (!string.IsNullOrEmpty(accessToken))
            {
                result.Data = accessToken;
                result.State = SimpleStepStateEnum.Successful;
                Kernel.Bind<Authenticator>()
                    .ToConstant(
                        new Authenticator
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

        #endregion
    }
}