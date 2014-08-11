namespace Omnipaste.SendSms
{
    using System;
    using Caliburn.Micro;

    public class SmsMessage : PropertyChangedBase
    {
        #region Fields

        private string _message = string.Empty;

        private string _recipient = string.Empty;

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
                NotifyOfPropertyChange(() => LimitMessage);
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

        public int CharactersRemaining
        {
            get
            {
                var totalMessageLength = Message.Length;
                var currentMessageLength = totalMessageLength % 160;
                var charactersRemaining = 0;
                if (totalMessageLength == 0 || currentMessageLength != 0)
                {
                    charactersRemaining = 160 - (currentMessageLength);
                }

                return charactersRemaining;
            }
        }

        public int MaxCharacters
        {
            get
            {
                int result = 160;
                if (Message.Length != 0)
                {
                    result = (int)Math.Ceiling(((double)Message.Length) / 160) * 160;
                }
                return result;
            }
        }

        public string LimitMessage
        {
            get
            {
                return string.Format("{0}/{1}", CharactersRemaining, MaxCharacters);
            }
        }

        #endregion
    }
}