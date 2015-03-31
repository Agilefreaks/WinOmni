namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using Omnipaste.Properties;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        #region Fields

        private readonly IOAuth2 _oauth2;

        #endregion

        #region Constructors and Destructors

        public GetRemoteConfiguration(IOAuth2 oAuth2)
        {
            _oauth2 = oAuth2;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        public override DependencyParameter Parameter { get; set; }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            var givenToken = Convert.ToString(Parameter.Value);
            return string.IsNullOrEmpty(givenToken)
                       ? Observable.Return(new ExecuteResult(SimpleStepStateEnum.Failed, Resources.MissingUserTokenError))
                       : _oauth2.Create(givenToken)
                             .Select(GetExecuteResult)
                             .Catch<IExecuteResult, Exception>(CreateErrorHandler);
        }

        protected IObservable<IExecuteResult> CreateErrorHandler(Exception exception)
        {
            ExceptionReporter.Instance.Report(new Exception(Resources.ExceptionDuringAuthentication, exception));
            var executeResult = new ExecuteResult(SimpleStepStateEnum.Failed, Resources.BrokenCommunicationError);

            return Observable.Return(executeResult, SchedulerProvider.Default);
        }

        #endregion

        #region Methods

        private static IExecuteResult GetExecuteResult(TokenDto tokenDto)
        {
            if (string.IsNullOrEmpty(tokenDto.AccessToken))
            {
                ExceptionReporter.Instance.Report(new Exception(Resources.EmptyTokenFromServer));
            }

            return new ExecuteResult(SimpleStepStateEnum.Successful, tokenDto);
        }

        #endregion
    }
}