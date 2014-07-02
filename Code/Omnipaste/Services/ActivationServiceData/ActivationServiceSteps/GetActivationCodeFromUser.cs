namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Threading;
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class GetActivationCodeFromUser : ActivationStepBase, IHandle<TokenRequestResultMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        private AutoResetEvent _autoResetEvent;

        private TokenRequestResultMessage _lastRequestResult;

        public Action<TokenRequestResultMessage> OnTokenRequestResultAction { get; set; }

        public GetActivationCodeFromUser(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public override IObservable<IExecuteResult> Execute()
        {
            throw new NotImplementedException();
//            _autoResetEvent = new AutoResetEvent(false);
//            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage { Message = (string)Parameter.Value });
//            _autoResetEvent.WaitOne();
//
//            var executeResult = new ExecuteResult();
//            if (_lastRequestResult.Status == TokenRequestResultMessageStatusEnum.Successful)
//            {
//                executeResult.State = SimpleStepStateEnum.Successful;
//                executeResult.Data = _lastRequestResult.ActivationCode;
//            }
//            else
//            {
//                executeResult.State = SimpleStepStateEnum.Failed;
//            }
//
//            return executeResult;
        }

        public void Handle(TokenRequestResultMessage tokenRequestResultMessage)
        {
            _lastRequestResult = tokenRequestResultMessage;
            _autoResetEvent.Set();
        }
    }
}