namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using OmniApi.Models;
    using OmniCommon.Interfaces;

    public class SaveConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        public SaveConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        protected override IObservable<IExecuteResult> InternalExecute()
        {
            return Observable.Create<IExecuteResult>(
                observer =>
                {
                    var token = (Token)Parameter.Value;
                    _configurationService.SaveAuthSettings(token.AccessToken, token.RefreshToken);

                    observer.OnNext(
                        new ExecuteResult { State = SimpleStepStateEnum.Successful, Data = token.AccessToken });
                    observer.OnCompleted();

                    return Disposable.Empty;
                });
        }
    }
}