namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Subjects;
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class GetActivationCodeFromUser : ActivationStepBase, IHandle<TokenRequestResultMessage>
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private readonly Subject<IExecuteResult> _subject;

        #endregion

        #region Constructors and Destructors

        public GetActivationCodeFromUser(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _subject = new Subject<IExecuteResult>();

            _eventAggregator.Subscribe(this);
        }

        #endregion

        #region Public Methods and Operators

        protected override IObservable<IExecuteResult> InternalExecute()
        {
            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage { Message = (string)Parameter.Value });
            return _subject;
        }

        public void Handle(TokenRequestResultMessage tokenRequestResultMessage)
        {
            var result = new ExecuteResult(SimpleStepStateEnum.Failed);
            if (tokenRequestResultMessage.Status == TokenRequestResultMessageStatusEnum.Successful)
            {
                result.State = SimpleStepStateEnum.Successful;
                result.Data = tokenRequestResultMessage.ActivationCode;
            }
            else
            {
                result.State = SimpleStepStateEnum.Failed;
            }

            _subject.OnNext(result);
            _subject.OnCompleted();
        }

        #endregion
    }
}