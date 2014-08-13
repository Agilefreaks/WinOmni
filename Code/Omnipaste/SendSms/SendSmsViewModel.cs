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
        #region Constructors and Destructors

        public SendSmsViewModel(IPhones phones, IEventAggregator eventAggregator)
        {
            Phones = phones;
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }

        #endregion

        #region Public Properties

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public SmsMessage Model { get; set; }

        public IPhones Phones { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Send()
        {
            Phones.SendSms(Model.Recipient, Model.Message).Subscribe(m => TryClose(true), exception => { });
        }

        #endregion

        public void Handle(SendSmsMessage message)
        {
            Model = new SmsMessage { Recipient = message.Recipient, Message = message.Message };

            DialogViewModel.ActivateItem(this);

            EventAggregator.PublishOnCurrentThread(new ShowShellMessage());
        }
    }
}