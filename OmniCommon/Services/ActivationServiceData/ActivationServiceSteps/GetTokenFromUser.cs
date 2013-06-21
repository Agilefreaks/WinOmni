namespace OmniCommon.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Threading;
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class GetTokenFromUser : ActivationStepBase, IHandle<TokenRequestResutMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private AutoResetEvent _autoResetEvent;

        private TokenRequestResutMessage _lastRequestResult;

        public Action<TokenRequestResutMessage> HandleTokenRequestResultAction { get; set; }

        public GetTokenFromUser(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            HandleTokenRequestResultAction = HandleTokenRequestResult;
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
                executeResult.Data = _lastRequestResult.Token;
            }
            else
            {
                executeResult.State = SimpleStepStateEnum.Failed;
            }

            return executeResult;
        }

        public void Handle(TokenRequestResutMessage tokenRequestResutMessage)
        {
            HandleTokenRequestResultAction(tokenRequestResutMessage);
            _autoResetEvent.Set();
        }

        public void HandleTokenRequestResult(TokenRequestResutMessage message)
        {
            _lastRequestResult = message;
        }
    }
}