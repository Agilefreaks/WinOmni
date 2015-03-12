﻿namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
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
            var userInfo = _configurationService.UserInfo ?? 
                UserInfo.BeginBuild()
                .WithEmail(user.Email)
                .WithFirstName(user.FirstName)
                .WithLastName(user.LastName)
                .WithImageUrl(user.ImageUrl)
                .Build();
            
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
