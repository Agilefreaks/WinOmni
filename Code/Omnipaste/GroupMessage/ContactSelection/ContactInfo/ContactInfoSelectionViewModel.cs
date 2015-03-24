namespace Omnipaste.GroupMessage.ContactSelection.ContactInfo
{
    using System;
    using Omnipaste.ContactList;
    using Omnipaste.ContactList.ContactInfo;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Services;

    public class ContactInfoSelectionViewModel : ContactInfoViewModel,
                                                 IContactInfoSelectionViewModel
    {
        private bool _isSelected;

        private DateTime? _lastActivityTime;

        public ContactInfoSelectionViewModel(ISessionManager sessionManager)
            : base(sessionManager)
        {
            ClickCommand = new Command(ToggleSelection);
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value.Equals(_isSelected))
                {
                    return;
                }
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        private void ToggleSelection()
        {
            IsSelected = !IsSelected;
        }
    }
}