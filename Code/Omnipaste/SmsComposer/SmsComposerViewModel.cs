namespace Omnipaste.SMSComposer
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Models;
    using OmniCommon.Helpers;
    using Omnipaste.Dialog;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using SMS.Resources.v1;

    public abstract class SMSComposerViewModel : Screen, ISMSComposerViewModel
    {
        #region Fields

        private readonly ISMSMessages _smsMessages;

        protected readonly ISMSMessageFactory SMSMessageFactory;

        private bool _isSending;

        private SMSMessage _model;

        private Command _sendCommand;

        #endregion

        #region Constructors and Destructors

        protected SMSComposerViewModel(ISMSMessages smsMessages, ISMSMessageFactory smsMessageFactory)
        {
            _smsMessages = smsMessages;
            SMSMessageFactory = smsMessageFactory;
            SendCommand = new Command(Send);
        }

        #endregion

        #region Public Properties

        public virtual bool CanSend
        {
            get
            {
                return !IsSending && !string.IsNullOrEmpty(Model.Recipient) && !string.IsNullOrEmpty(Model.Message);
            }
        }

        public bool CanEditText
        {
            get
            {
                return !IsSending;
            }
        }

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
                NotifyOfPropertyChange(() => CanEditText);
            }
        }

        [Inject]
        public IMessageRepository MessageRepository { get; set; }

        public SMSMessage Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model != null)
                {
                    _model.PropertyChanged -= ModelPropertyChanged;
                }

                _model = value;
                _model.PropertyChanged += ModelPropertyChanged;
                NotifyOfPropertyChange();
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

        #endregion

        #region Public Methods and Operators

        public virtual void Send()
        {
            IsSending = true;
            _smsMessages.Send(Model.Recipient, Model.Message)
                .SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Default)
                .Subscribe(OnSentSMS, OnSendSMSError);
        }

        #endregion

        #region Methods

        protected virtual void OnSendSMSError(Exception exception)
        {
            IsSending = false;
        }

        protected virtual void OnSentSMS(EmptyModel model)
        {
            IsSending = false;
            MessageRepository.Save(Model.BaseModel);
            NotifyOfPropertyChange(() => CanSend);
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => CanSend);
        }

        #endregion
    }
}