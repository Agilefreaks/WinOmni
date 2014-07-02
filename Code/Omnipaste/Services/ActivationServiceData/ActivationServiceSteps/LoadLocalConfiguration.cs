namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using OmniApi.Models;
    using OmniCommon.Interfaces;

    public class LoadLocalConfiguration : ActivationStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public LoadLocalConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return Observable.Create<IExecuteResult>(
                observer =>
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

                        observer.OnNext(result);
                        observer.OnCompleted();

                        return Disposable.Empty;
                    });
        }

        #endregion
    }
}