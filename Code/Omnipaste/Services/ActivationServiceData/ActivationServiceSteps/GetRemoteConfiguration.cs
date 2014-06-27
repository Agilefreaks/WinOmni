﻿namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using Omnipaste.Properties;
    using Retrofit.Net;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        #region Constants

        public const int MaxRetryCount = 5;

        #endregion

        #region Fields

        private RetryInfo _payload;

        #endregion

        #region Constructors and Destructors

        public GetRemoteConfiguration(IOAuth2 oAuth2)
        {
            OAuth2 = oAuth2;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        public override DependencyParameter Parameter { get; set; }

        public IOAuth2 OAuth2 { get; set; }

        #endregion

        #region Properties

        private RetryInfo PayLoad
        {
            get
            {
                _payload = (Parameter.Value as RetryInfo) ?? new RetryInfo((string)Parameter.Value);
                return _payload;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override IExecuteResult Execute()
        {
            var executeResult = new ExecuteResult();

            if (string.IsNullOrEmpty(PayLoad.AuthorizationCode))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
            }
            else
            {
                // TODO: move to a reactive (functional) approach
                Token token = new Token();
                try
                {
                    token = OAuth2.Create(PayLoad.AuthorizationCode).Wait();
                }
                catch (AggregateException)
                {
                    // empty
                }

                SetResultPropertiesBasedOnActivationData(executeResult, token);
            }

            return executeResult;
        }

        #endregion

        #region Methods

        private void SetResultPropertiesBasedOnActivationData(
            IExecuteResult executeResult,
            Token token)
        {
            if (token == null)
            {
                executeResult.Data = new RetryInfo(_payload.AuthorizationCode, _payload.FailCount + 1);
                executeResult.State = _payload.FailCount < MaxRetryCount
                                          ? GetRemoteConfigurationStepStateEnum.CommunicationFailure
                                          : GetRemoteConfigurationStepStateEnum.Failed;
            }
            else if (string.IsNullOrEmpty(token.access_token))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
                executeResult.Data = Resources.AuthorizationCodeError;
            }
            else
            {
                var authenticator = new Authenticator
                {
                    AccessToken = token.access_token,
                    RefreshToken = token.refresh_token,
                };
                executeResult.State = GetRemoteConfigurationStepStateEnum.Successful;
                executeResult.Data = authenticator;
                Kernel.Unbind<Authenticator>();
                Kernel.Bind<Authenticator>().ToConstant(authenticator);
            }
        }

        #endregion
    }
}