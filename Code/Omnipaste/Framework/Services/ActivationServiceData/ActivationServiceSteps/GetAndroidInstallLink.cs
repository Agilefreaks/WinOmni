﻿namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Web;
    using OmniCommon.Interfaces;

    public class GetAndroidInstallLink : ActivationStepBase
    {
        private readonly IUrlShortenerService _urlShortenerService;

        private readonly IConfigurationService _configurationService;

        private const string UrlFormat = "{0}/downloads/android_client?email={1}";

        public GetAndroidInstallLink(IUrlShortenerService urlShortenerService, IConfigurationService configurationService)
        {
            _urlShortenerService = urlShortenerService;
            _configurationService = configurationService;
        }

        public override IObservable<IExecuteResult> Execute()
        {
            var safeEmailString = HttpUtility.UrlEncode(_configurationService.UserInfo.Email);
            var uri = new Uri(string.Format(UrlFormat, _configurationService.WebBaseUrl, safeEmailString));

            return
                _urlShortenerService.Shorten(uri)
                    .ToObservable()
                    .Select(shortUrl => new ExecuteResult { Data = shortUrl, State = SimpleStepStateEnum.Successful })
                    .Catch<IExecuteResult, Exception>(
                        _ => Observable.Return(new ExecuteResult(SimpleStepStateEnum.Successful, uri)));
        }
    }
}