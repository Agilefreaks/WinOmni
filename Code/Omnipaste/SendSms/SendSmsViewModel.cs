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

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public string Recipient
        {
            get
            {
                return _recipient;
            }
            set
            {
                _recipient = value;
                NotifyOfPropertyChange(() => Recipient);
            }
        }

        public SendSmsViewModel(IPhones phones)
        {
            Phones = phones;
        }

        public void Send()
        {
            Phones.SendSms(Recipient, Message).Subscribe(
                m => TryClose(true), 
                exception => { });
        }

        #endregion
    }
}