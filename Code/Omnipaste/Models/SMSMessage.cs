namespace Omnipaste.Models
{
    using System;
    using Caliburn.Micro;

    public class SMSMessage : PropertyChangedBase
    {
        #region Constants

        private const int MessageLimit = 160;

        #endregion

        #region Fields

        private readonly Message _message;

        #endregion

        #region Constructors and Destructors

        public SMSMessage()
        {
            _message = new Message();
        }

        public SMSMessage(Message message)
        {
            _message = message;
        }

        #endregion

        #region Public Properties

        public Message BaseModel
        {
            get
            {
                return _message;
            }
        }

        public int CharactersRemaining
        {
            get
            {
                var totalMessageLength = Message.Length;
                var currentMessageLength = totalMessageLength % MessageLimit;
                var charactersRemaining = 0;
                if (totalMessageLength == 0 || currentMessageLength != 0)
                {
                    charactersRemaining = MessageLimit - (currentMessageLength);
                }

                return charactersRemaining;
            }
        }

        public string LimitMessage
        {
            get
            {
                return string.Format("{0}/{1}", CharactersRemaining, MaxCharacters);
            }
        }

        public int MaxCharacters
        {
            get
            {
                var result = MessageLimit;
                if (Message.Length != 0)
                {
                    result = (int)Math.Ceiling(((double)Message.Length) / MessageLimit) * MessageLimit;
                }
                return result;
            }
        }

        public string Message
        {
            get
            {
                return _message.Content;
            }
            set
            {
                if (_message.Content != value)
                {
                    _message.Content = value;
                    NotifyOfPropertyChange();
                    NotifyOfPropertyChange(() => LimitMessage);
                }
            }
        }

        public string Recipient
        {
            get
            {
                return _message.ContactInfo.Phone;
            }
            set
            {
                if (_message.ContactInfo.Phone != value)
                {
                    _message.ContactInfo.Phone = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        #endregion
    }
}