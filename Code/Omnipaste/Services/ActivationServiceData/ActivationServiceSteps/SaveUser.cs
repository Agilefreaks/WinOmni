namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Models;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class SaveUser : SynchronousStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public SaveUser(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Protected Methods

        protected override IExecuteResult ExecuteSynchronously()
        {
            var user = Parameter.Value as User ?? new User();
            _configurationService.UserInfo = new UserInfo
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImageUrl = user.ImageUrl
            };
            if (!_configurationService.HasSavedValueFor(ConfigurationProperties.SMSSuffixEnabled))
            {
                _configurationService.IsSMSSuffixEnabled = user.ViaOmnipaste;
            }

            return new ExecuteResult { State = SimpleStepStateEnum.Successful };
        }

        #endregion
    }
}
