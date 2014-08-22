namespace Omnipaste.SendSms
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;

    public class SendSmsViewModel : Screen, ISendSmsViewModel
    {
        private SendSmsStatusEnum _state = SendSmsStatusEnum.Composing;

        #region Constructors and Destructors

        public SendSmsViewModel(IDevices devices, IEventAggregator eventAggregator)
        {
            Devices = devices;
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }

        #endregion

        #region Public Properties

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public SmsMessage Model { get; set; }

        public IDevices Devices { get; set; }

        public bool CanSend
        {
            get
            {
                return State == SendSmsStatusEnum.Composing;
            }
        }

        public SendSmsStatusEnum State
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
                NotifyOfPropertyChange(() => State);
                NotifyOfPropertyChange(() => CanSend);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Send()
        {
            State = SendSmsStatusEnum.Sending;
            Devices.SendSms(Model.Recipient, Model.Message)
                .Subscribe(
                    m =>
                    {
                        State = SendSmsStatusEnum.Sent;
                    }, 
                    exception => { });
        }

        #endregion

        public void Handle(SendSmsMessage message)
        {
            State = SendSmsStatusEnum.Composing;
            Model = new SmsMessage { Recipient = message.Recipient, Message = message.Message };

            DialogViewModel.ActivateItem(this);

            EventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }
    }
}