namespace Omnipaste.SmsComposer
{
    using System.ComponentModel;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.ExtensionMethods;

    public class SmsComposerViewModel : Screen, ISmsComposerViewModel
    {
        private readonly ISMSFactory _smsFactory;

        #region Fields

        private SmsMessage _model;

        private SmsComposerStatusEnum _state = SmsComposerStatusEnum.Composing;

        #endregion

        #region Constructors and Destructors

        public SmsComposerViewModel(IDevices devices, IEventAggregator eventAggregator, ISMSFactory smsFactory)
        {
            _smsFactory = smsFactory;
            Devices = devices;
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }

        #endregion

        #region Public Properties

        public bool CanSend
        {
            get
            {
                return State == SmsComposerStatusEnum.Composing && !string.IsNullOrEmpty(Model.Recipient)
                       && !string.IsNullOrEmpty(Model.Message);
            }
        }

        public IDevices Devices { get; set; }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public SmsMessage Model
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
                NotifyOfPropertyChange(() => Model);
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

        public void Handle(SendSmsMessage message)
        {
            State = SmsComposerStatusEnum.Composing;
            Model = _smsFactory.Create(message);

            DialogViewModel.ActivateItem(this);

            EventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }

        public void Send()
        {
            State = SmsComposerStatusEnum.Sending;
            Devices.SendSms(Model.Recipient, Model.Message)
                .SubscribeAndHandleErrors(m => { State = SmsComposerStatusEnum.Sent; });
        }

        #endregion

        #region Methods

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => CanSend);
        }

        #endregion
    }
}