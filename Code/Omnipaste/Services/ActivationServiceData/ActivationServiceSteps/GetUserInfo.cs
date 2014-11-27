namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;

    public class GetUserInfo : ActivationStepBase
    {
        #region Constants

        private const int DefaultRetryCount = 3;

        #endregion

        #region Static Fields

        private static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromSeconds(1);

        #endregion

        #region Fields

        private readonly int _retryCount;

        private readonly TimeSpan _retryDelay;

        private readonly IUserInfo _userInfo;

        #endregion

        #region Constructors and Destructors

        public GetUserInfo(IUserInfo userInfo)
            : this(userInfo, DefaultRetryDelay, DefaultRetryCount)
        {
        }

        public GetUserInfo(IUserInfo userInfo, TimeSpan retryDelay, int retryCount)
        {
            _userInfo = userInfo;
            _retryDelay = retryDelay;
            _retryCount = retryCount;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return
                Observable.Defer(() => _userInfo.Get().ReportErrors())
                    .RetryAfter(_retryDelay, _retryCount)
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