namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Models;
    using OmniCommon.Interfaces;

    public class GetLocalActivationCode : SynchronousStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public GetLocalActivationCode(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        protected override IExecuteResult ExecuteSynchronously()
        {
            var result = new ExecuteResult();

            var accessToken = _configurationService.AccessToken;
            var refreshToken = _configurationService.RefreshToken;

            if (string.IsNullOrEmpty(accessToken))
            {
                result.State = SimpleStepStateEnum.Failed;
            }
            else
            {
                result.Data = new Token(accessToken, refreshToken);
                result.State = SimpleStepStateEnum.Successful;
            }

            return result;
        }

        #endregion
    }
}