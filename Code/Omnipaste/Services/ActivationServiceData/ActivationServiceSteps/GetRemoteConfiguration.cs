namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using BugFreak;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
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

        protected override IObservable<IExecuteResult> InternalExecute()
        {
            var givenToken = Convert.ToString(Parameter.Value);
            return !string.IsNullOrEmpty(givenToken)
                       ? _oauth2.Create(givenToken)
                             .Select(GetExecuteResult)
                             .Catch((Func<Exception, IObservable<IExecuteResult>>)CreateErrorHandler)
                       : new[] { new ExecuteResult(SimpleStepStateEnum.Failed, Resources.MissingUserTokenError) }
                             .ToObservable();
        }

        protected IObservable<IExecuteResult> CreateErrorHandler(Exception exception)
        {
            ReportingService.Instance.BeginReport(new Exception(Resources.ExceptionDuringAuthentication, exception));
            var executeResult = new ExecuteResult(SimpleStepStateEnum.Failed, Resources.BrokenCommunicationError);

            return new[] { executeResult }.ToObservable();
        }

        #endregion

        #region Methods

        private static IExecuteResult GetExecuteResult(Token token)
        {
            if (string.IsNullOrEmpty(token.AccessToken))
            {
                ReportingService.Instance.BeginReport(new Exception(Resources.EmptyTokenFromServer));
            }

            return new ExecuteResult(SimpleStepStateEnum.Successful, token);
        }

        #endregion
    }
}