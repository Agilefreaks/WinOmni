namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;

    public class GetUserInfo : ActivationStepBase
    {
        #region Fields

        private readonly IUserInfo _userInfo;

        #endregion

        #region Constructors and Destructors

        public GetUserInfo(IUserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return
                _userInfo.Get()
                    .Select(userInfo => new ExecuteResult { State = SimpleStepStateEnum.Successful, Data = userInfo })
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