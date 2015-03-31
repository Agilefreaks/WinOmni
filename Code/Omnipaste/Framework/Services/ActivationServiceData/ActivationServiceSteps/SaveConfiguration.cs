namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using OmniApi.Dto;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Settings;

    public class SaveConfiguration : SynchronousStepBase
    {
        private readonly IConfigurationService _configurationService;

        public SaveConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            var token = (TokenDto)Parameter.Value;
            _configurationService.SaveAuthSettings(new OmnipasteCredentials(token.AccessToken, token.RefreshToken));
            if (string.IsNullOrEmpty(_configurationService.AccessToken))
            {
                ExceptionReporter.Instance.Report(new Exception("Access token empty in SaveConfiguration - Problem with writing the file"));
            }

            return new ExecuteResult { State = SimpleStepStateEnum.Successful, Data = token.AccessToken };
        }
    }
}