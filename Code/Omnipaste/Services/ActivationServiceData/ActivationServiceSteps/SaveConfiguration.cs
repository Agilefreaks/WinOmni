﻿namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using OmniApi.Models;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;

    public class SaveConfiguration : SynchronousStepBase
    {
        private readonly IConfigurationService _configurationService;

        public SaveConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            var token = (Token)Parameter.Value;
            _configurationService.SaveAuthSettings(token.AccessToken, token.RefreshToken);
            if (string.IsNullOrEmpty(_configurationService.AccessToken))
            {
                ExceptionReporter.Instance.Report(new Exception("Access token empty in SaveConfiguration - Problem with writing the file"));
            }

            return new ExecuteResult { State = SimpleStepStateEnum.Successful, Data = token.AccessToken };
        }
    }
}