﻿namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Threading;
    using Caliburn.Micro;
    using OmniCommon.DataProviders;
    using OmniCommon.EventAggregatorMessages;

    public class GetTokenFromUser : ActivationStepBase, IHandle<TokenRequestResultMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IConfigurationProvider _configurationProvider;

        private AutoResetEvent _autoResetEvent;

        private TokenRequestResultMessage _lastRequestResult;

        public Action<TokenRequestResultMessage> OnTokenRequestResultAction { get; set; }

        public GetTokenFromUser(IEventAggregator eventAggregator, IConfigurationProvider configurationProvider)
        {
            this._eventAggregator = eventAggregator;
            _configurationProvider = configurationProvider;
            this._eventAggregator.Subscribe(this);
        }

        public override IExecuteResult Execute()
        {
            this._autoResetEvent = new AutoResetEvent(false);
            this._eventAggregator.Publish(new GetTokenFromUserMessage());
            this._autoResetEvent.WaitOne();

            var executeResult = new ExecuteResult();
            if (this._lastRequestResult.Status == TokenRequestResultMessageStatusEnum.Successful)
            {
                executeResult.State = SimpleStepStateEnum.Successful;
                executeResult.Data = _configurationProvider["deviceIdentifier"] = _lastRequestResult.Token;
            }
            else
            {
                executeResult.State = SimpleStepStateEnum.Failed;
            }

            return executeResult;
        }

        public void Handle(TokenRequestResultMessage tokenRequestResultMessage)
        {
            this._lastRequestResult = tokenRequestResultMessage;
            this._autoResetEvent.Set();
        }
    }
}