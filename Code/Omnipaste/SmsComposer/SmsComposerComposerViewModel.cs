namespace Omnipaste.SmsComposer
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;

    public class SmsComposerComposerViewModel : Screen, ISmsComposerViewModel
    {
        private SmsComposerStatusEnum _state = SmsComposerStatusEnum.Composing;

        #region Constructors and Destructors

        public SmsComposerComposerViewModel(IDevices devices, IEventAggregator eventAggregator)
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
                return State == SmsComposerStatusEnum.Composing 
                    && !string.IsNullOrEmpty(Model.Recipient)
                    && !string.IsNullOrEmpty(Model.Message);
            }
        }

        public SmsComposerStatusEnum State
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
            State = SmsComposerStatusEnum.Sending;
            Devices.SendSms(Model.Recipient, Model.Message)
                .Subscribe(
                    m =>
                    {
                        State = SmsComposerStatusEnum.Sent;
                    }, 
                    exception => { });
        }

        #endregion

        public void Handle(SendSmsMessage message)
        {
            State = SmsComposerStatusEnum.Composing;
            Model = new SmsMessage { Recipient = message.Recipient, Message = message.Message };

            DialogViewModel.ActivateItem(this);

            EventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }
    }
}