namespace Omnipaste.SendSms
{
    using System;
    using Caliburn.Micro;
    using OmniApi.Resources.v1;

    public class SendSmsViewModel : Screen, ISendSmsViewModel
    {
        public IPhones Phones { get; set; }

        #region Fields

        private string _message;

        private string _recipient;

        #endregion

        #region Public Properties

        public SmsMessage Model { get; set; }

        public SendSmsViewModel(IPhones phones)
        {
            Phones = phones;
        }

        public void Send()
        {
            Phones.SendSms(Model.Recipient, Model.Message).Subscribe(
                m => TryClose(true), 
                exception => { });
        }

        #endregion
    }
}