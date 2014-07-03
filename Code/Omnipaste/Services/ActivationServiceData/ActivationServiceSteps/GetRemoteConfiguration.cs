namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using Omnipaste.Properties;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        #region Constants

        public const int MaxRetryCount = 5;

        #endregion

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
            IObservable<IExecuteResult> result;
            if (Parameter.Value == null || string.IsNullOrEmpty(Parameter.Value.ToString()))
            {
                result = new IExecuteResult[] { new ExecuteResult(SimpleStepStateEnum.Failed) }.ToObservable();
            }
            else
            {
                result = _oauth2.Create(Parameter.Value.ToString()).Select(GetExecuteResult);
            }

            return result;
        }

        #endregion

        #region Methods

        private IExecuteResult GetExecuteResult(Token token)
        {
            var result = new ExecuteResult();
            if (string.IsNullOrEmpty(token.AccessToken))
            {
                result.State = GetRemoteConfigurationStepStateEnum.Failed;
                result.Data = Resources.AuthorizationCodeError;
            }
            else
            {
                result.State = GetRemoteConfigurationStepStateEnum.Successful;
                result.Data = token;
            }

            return result;
        }

        #endregion
    }
}