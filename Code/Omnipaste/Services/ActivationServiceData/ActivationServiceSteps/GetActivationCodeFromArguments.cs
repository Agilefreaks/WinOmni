namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Omnipaste.DataProviders;

    public class GetActivationCodeFromArguments : ActivationStepBase
    {
        private readonly IArgumentsDataProvider _dataProvider;

        public GetActivationCodeFromArguments(IArgumentsDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        protected override IObservable<IExecuteResult> InternalExecute()
        {
            return Observable.Create<IExecuteResult>(
                observer =>
                {
                    var result = new ExecuteResult { State = SimpleStepStateEnum.Failed };
                    var token = _dataProvider.AuthorizationKey;
                    if (!string.IsNullOrEmpty(token))
                    {
                        result.State = SimpleStepStateEnum.Successful;
                        result.Data = token;
                    }
                    observer.OnNext(result);
                    observer.OnCompleted();

                    return Disposable.Empty;
                });
        }
    }
}