namespace Omnipaste.GroupMessage.ContactSelection.ContactInfo
{
    using System;
    using Omnipaste.ContactList;
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public class ContactInfoSelectionViewModel : DetailsViewModelBase<ContactInfoPresenter>, IContactInfoSelectionViewModel
    {
        private DateTime? _lastActivityTime;

        public DateTime? LastActivityTime
        {
            get
            {
                return _lastActivityTime;
            }
            set
            {
                if (value.Equals(_lastActivityTime))
                {
                    return;
                }
                _lastActivityTime = value;
                NotifyOfPropertyChange(() => LastActivityTime);
            }
        }
    }
}