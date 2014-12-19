namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using System;
    using Caliburn.Micro;

    public abstract class MessageStepViewModelBase : Screen, IMessageStepViewModel
    {
        private MessageContext _messageContext;

        public event EventHandler<EventArgs> OnNext;

        public event EventHandler<EventArgs> OnPrevious;

        public MessageContext MessageContext
        {
            get
            {
                return _messageContext;
            }
            set
            {
                if (Equals(value, _messageContext))
                {
                    return;
                }
                _messageContext = value;
                NotifyOfPropertyChange(() => MessageContext);
            }
        }

        protected virtual void NotifyOnNext()
        {
            var handler = OnNext;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void NotifyOnPrevious()
        {
            var handler = OnPrevious;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}