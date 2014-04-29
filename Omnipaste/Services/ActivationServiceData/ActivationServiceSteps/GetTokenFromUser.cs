namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
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
            _eventAggregator = eventAggregator;
            _configurationProvider = configurationProvider;
            _eventAggregator.Subscribe(this);
        }

        public override IExecuteResult Execute()
        {
            _autoResetEvent = new AutoResetEvent(false);
            _eventAggregator.Publish(new GetTokenFromUserMessage());
            _autoResetEvent.WaitOne();

            var executeResult = new ExecuteResult();
            if (_lastRequestResult.Status == TokenRequestResultMessageStatusEnum.Successful)
            {
                executeResult.State = SimpleStepStateEnum.Successful;
                executeResult.Data = _lastRequestResult.ActivationCode;
            }
            else
            {
                executeResult.State = SimpleStepStateEnum.Failed;
            }

            return executeResult;
        }

        public void Handle(TokenRequestResultMessage tokenRequestResultMessage)
        {
            _lastRequestResult = tokenRequestResultMessage;
            _autoResetEvent.Set();
        }
    }
}