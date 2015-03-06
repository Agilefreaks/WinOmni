namespace Omnipaste.SMSComposer
{
    using System;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Dialog;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using Omnipaste.Properties;
    using SMS.Models;
    using SMS.Resources.v1;

    public class SMSComposerViewModel : Screen, ISMSComposerViewModel
    {
        #region Fields

        private readonly ISMSMessages _smsMessages;

        private readonly IConfigurationService _configurationService;

        private bool _isSending;

        private Command _sendCommand;

        private ContactInfo _contactInfo;

        private string _message;

        #endregion

        #region Constructors and Destructors

        public SMSComposerViewModel(ISMSMessages smsMessages, IConfigurationService configurationService)
        {
            _smsMessages = smsMessages;
            _configurationService = configurationService;
            SendCommand = new Command(Send);
        }

        #endregion

        #region Public Properties

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public bool IsSending
        {
            get
            {
                return _isSending;
            }
            set
            {
                if (value.Equals(_isSending))
                {
                    return;
                }
                _isSending = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanSend);
            }
        }

        public bool CanSend
        {
            get
            {
                return !IsSending;
            }
        }

        [Inject]
        public IMessageRepository MessageRepository { get; set; }

        public Command SendCommand
        {
            get
            {
                return _sendCommand;
            }
            set
            {
                if (Equals(value, _sendCommand))
                {
                    return;
                }
                _sendCommand = value;
                NotifyOfPropertyChange();
            }
        }

        public ContactInfo ContactInfo
        {
            get
            {
                return _contactInfo;
            }
            set
            {
                if (Equals(value, _contactInfo))
                {
                    return;
                }
                _contactInfo = value;
                NotifyOfPropertyChange(() => ContactInfo);
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value == _message)
                {
                    return;
                }
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        #endregion

        #region Public Methods and Operators

        public virtual void Send()
        {
            IsSending = true;
            _smsMessages.Send(ContactInfo.Phone, Message)
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Default)
                .Subscribe(OnSentSMS, OnSendSMSError);
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            StartNewMessage();
            base.OnActivate();
        }

        protected virtual void OnSendSMSError(Exception exception)
        {
            IsSending = false;
        }

        protected virtual void OnSentSMS(SmsMessageDto model)
        {
            IsSending = false;
            MessageRepository.Save(new LocalSmsMessage(model));
            StartNewMessage();
        }

        private void StartNewMessage()
        {
            Message = _configurationService.IsSMSSuffixEnabled
                          ? string.Format("{0}{1}", Environment.NewLine, Resources.SentFromOmnipaste)
                          : string.Empty;
        }

        #endregion
    }
}