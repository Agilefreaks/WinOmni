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
            IObservable<IExecuteResult> failResult =
                new IExecuteResult[] { new ExecuteResult(SimpleStepStateEnum.Failed, Resources.AuthorizationCodeError) }.ToObservable();
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
            return new ExecuteResult(SimpleStepStateEnum.Successful, token);
        }

        #endregion
    }
}