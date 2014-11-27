namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;

    public class GetUser : ActivationStepBase
    {
        #region Fields

        private readonly IUsers _users;

        #endregion

        #region Constructors and Destructors

        public GetUser(IUsers users)
        {
            _users = users;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return
                _users.Get()
                    .Select(user => new ExecuteResult { State = SimpleStepStateEnum.Successful, Data = user })
                    .Catch<IExecuteResult, Exception>(
                        exception =>
                        Observable.Return(
                            new ExecuteResult(SimpleStepStateEnum.Failed, exception),
                            SchedulerProvider.Default))
                    .Take(1);
        }

        #endregion
    }
}