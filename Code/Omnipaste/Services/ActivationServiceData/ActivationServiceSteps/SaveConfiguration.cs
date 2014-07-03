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

        public override IObservable<IExecuteResult> Execute()
        {
            return Observable.Create<IExecuteResult>(
                observer =>
                {
                    var token = (Token)Parameter.Value;
                    _configurationService.SaveAuthSettings(token.AccessToken, token.RrefreshToken);

                    observer.OnNext(
                        new ExecuteResult { State = SimpleStepStateEnum.Successful, Data = token.AccessToken });
                    observer.OnCompleted();

                    return Disposable.Empty;
                });
        }
    }
}