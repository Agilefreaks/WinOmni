namespace OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage
{
    using Caliburn.Micro;

    public class SentMessage : PropertyChangedBase
    {
        private string _text;

        private string _category;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value == _text)
                {
                    return;
                }
                _text = value;
                NotifyOfPropertyChange();
            }
        }

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (value == _category)
                {
                    return;
                }
                _category = value;
                NotifyOfPropertyChange();
            }
        }
    }
}