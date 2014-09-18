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
            IObservable<IExecuteResult> failResult =
                new IExecuteResult[] { new ExecuteResult(SimpleStepStateEnum.Failed, Resources.UserTokeCodeError) }.ToObservable();
            IObservable<IExecuteResult> result = failResult;
            if (Parameter.Value != null && !string.IsNullOrEmpty(Parameter.Value.ToString()))
            {
                result =
                    _oauth2.Create(Parameter.Value.ToString()).Select(GetExecuteResult).Catch(failResult);
            }

            return result;
        }

        #endregion

        #region Methods

        private IExecuteResult GetExecuteResult(Token token)
        {
            if (string.IsNullOrEmpty(token.AccessToken))
            {
                ReportingService.Instance.BeginReport(new Exception("Access token empty in GetRemoteConfiguration.GetExecuteResult - Problem in the API"));
            }

            return new ExecuteResult(SimpleStepStateEnum.Successful, token);
        }

        #endregion
    }
}