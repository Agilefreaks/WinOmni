namespace Omnipaste.SMSComposer
{
    using System;
    using Caliburn.Micro;
    using OmniApi.Models;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using SMS.Resources.v1;

    public class ModalSMSComposerViewModel : SMSComposerViewModel, IModalSMSComposerViewModel
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private SMSComposerStatusEnum _state = SMSComposerStatusEnum.Composing;

        #endregion

        #region Constructors and Destructors

        public ModalSMSComposerViewModel(
            ISMSMessages smsMessages,
            IEventAggregator eventAggregator,
            ISMSMessageFactory smsMessageFactory)
            : base(smsMessages, smsMessageFactory)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        #endregion

        #region Public Properties

        public override bool CanSend
        {
            get
            {
                return base.CanSend && State == SMSComposerStatusEnum.Composing;
            }
        }

        public SMSComposerStatusEnum State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == _state)
                {
                    return;
                }
                _state = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanSend);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Handle(SendSmsMessage message)
        {
            State = SMSComposerStatusEnum.Composing;
            Model = SMSMessageFactory.Create(message);

            DialogViewModel.ActivateItem(this);

            _eventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }

        public override void Send()
        {
            State = SMSComposerStatusEnum.Sending;
            base.Send();
        }

        #endregion

        #region Methods

        protected override void OnSendSMSError(Exception exception)
        {
            base.OnSendSMSError(exception);
            State = SMSComposerStatusEnum.Sent;
            NotifyOfPropertyChange(() => CanSend);
        }

        protected override void OnSentSMS(EmptyModel model)
        {
            State = SMSComposerStatusEnum.Sent;
            base.OnSentSMS(model);
        }

        #endregion
    }
}