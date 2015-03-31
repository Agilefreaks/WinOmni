namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Dto;
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
            var user = Parameter.Value as UserDto ?? new UserDto();
            var userInfo = _configurationService.UserInfo;
            if (userInfo == null || userInfo.UpdatedAt != user.UpdatedAt)
            {
                userInfo = UserInfo.BeginBuild()
                    .WithEmail(user.Email)
                    .WithFirstName(user.FirstName)
                    .WithLastName(user.LastName)
                    .WithImageUrl(user.ImageUrl)
                    .WithUpdatedAt(user.UpdatedAt)
                    .Build();
            }

            if (!_configurationService.HasSavedValueFor(ConfigurationProperties.SMSSuffixEnabled))
            {
                _configurationService.IsSMSSuffixEnabled = user.ViaOmnipaste;
            }

            _configurationService.UserInfo = userInfo;

            return new ExecuteResult { State = SimpleStepStateEnum.Successful };
        }

        #endregion
    }
}
