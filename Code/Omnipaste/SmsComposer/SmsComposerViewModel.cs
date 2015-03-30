namespace Omnipaste.SMSComposer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Factories;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using SMS.Resources.v1;

    public class SMSComposerViewModel : Screen, ISMSComposerViewModel
    {
        private readonly IConfigurationService _configurationService;

        private readonly ISMSMessages _smsMessages;

        private bool _isSending;

        private string _message;

        private Command _sendCommand;

        public SMSComposerViewModel(ISMSMessages smsMessages, IConfigurationService configurationService)
        {
            _smsMessages = smsMessages;
            _configurationService = configurationService;
            SendCommand = new Command(Send);
        }

        [Inject]
        public ISmsMessageFactory SmsMessageFactory { get; set; }

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

        #region ISMSComposerViewModel Members

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

        public ObservableCollection<ContactInfoPresenter> Recipients { get; set; }

        public virtual void Send()
        {
            IsSending = true;

            _smsMessages.Send(Recipients.Select(r => r.ContactInfo.PhoneNumber).ToList(), Message)
                .SelectMany(
                    smsMessage =>
                        {
                            return Recipients.Select(
                                r =>
                                    {
                                        smsMessage.ContactId = r.ContactInfo.ContactId;
                                        return SmsMessageFactory.Create<LocalSmsMessage>(smsMessage);
                                    });
                        })
                .Merge()
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Default)
                .Subscribe(OnSentSMS, OnSendSMSError);
        }

        #endregion

        protected override void OnActivate()
        {
            StartNewMessage();
            base.OnActivate();
        }

        protected void OnSendSMSError(Exception exception)
        {
            IsSending = false;
        }

        protected void OnSentSMS(LocalSmsMessage smsMessage)
        {
            IsSending = false;
            StartNewMessage();
        }

        private void StartNewMessage()
        {
            Message = _configurationService.IsSMSSuffixEnabled
                          ? string.Format("{0}{1}", Environment.NewLine, Resources.SentFromOmnipaste)
                          : string.Empty;
        }
    }
}