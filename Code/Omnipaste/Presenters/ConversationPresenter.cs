namespace Omnipaste.Presenters
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;

    public class ConversationPresenter : PropertyChangedBase
    {
        private ObservableCollection<ConversationItemPresenter> _items;

        private ContactInfoPresenter _contactInfo;

        public ContactInfoPresenter ContactInfo
        {
            get
            {
                return _contactInfo;
            }
            set
            {
                if (Equals(value, _contactInfo))
                {
                    return;
                }
                _contactInfo = value;
                NotifyOfPropertyChange(() => ContactInfo);
            }
        }

        public ObservableCollection<ConversationItemPresenter> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (Equals(value, _items))
                {
                    return;
                }
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }
    }
}
