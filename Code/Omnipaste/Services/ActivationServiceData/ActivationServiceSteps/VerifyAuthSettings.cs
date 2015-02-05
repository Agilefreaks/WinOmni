namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class VerifyAuthSettings : SynchronousStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public VerifyAuthSettings(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        protected override IExecuteResult ExecuteSynchronously()
        {
            var accessToken = _configurationService.AccessToken;

            return
                new ExecuteResult(
                    string.IsNullOrEmpty(accessToken) ? SimpleStepStateEnum.Failed : SimpleStepStateEnum.Successful);
        }

        #endregion
    }
}