namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Collections.Specialized;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using OmniCommon.DataProviders;
    using OmniCommon.ExtensionMethods;

    public class GetActivationCodeFromDeploymentUri : ActivationStepBase
    {
        #region Constants

        private const string TokenKey = "token";

        #endregion

        #region Fields

        private readonly IApplicationDeploymentInfoProvider _applicationDeploymentInfoProvider;

        #endregion

        #region Constructors and Destructors

        public GetActivationCodeFromDeploymentUri(IApplicationDeploymentInfoProvider provider)
        {
            _applicationDeploymentInfoProvider = provider;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return Observable.Create<IExecuteResult>(
                observer =>
                    {
                        var result = new ExecuteResult { State = SimpleStepStateEnum.Failed };
                        if (_applicationDeploymentInfoProvider.HasValidActivationUri)
                        {
                            var token =
                                GetActivationTokenFromDeploymentParameters(
                                    _applicationDeploymentInfoProvider.ActivationUri);
                            if (!string.IsNullOrEmpty(token))
                            {
                                result.State = SimpleStepStateEnum.Successful;
                                result.Data = token;
                            }
                        }
                        observer.OnNext(result);
                        observer.OnCompleted();

                        return Disposable.Empty;
                    });
        }

        #endregion

        #region Methods

        private static string GetActivationTokenFromDeploymentParameters(Uri activationUri)
        {
            var deploymentParameters = activationUri != null
                                           ? activationUri.GetQueryStringParameters()
                                           : new NameValueCollection();

            return deploymentParameters[TokenKey];
        }

        #endregion
    }
}