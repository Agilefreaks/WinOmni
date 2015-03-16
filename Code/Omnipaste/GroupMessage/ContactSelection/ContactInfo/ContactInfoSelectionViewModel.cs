namespace Omnipaste.GroupMessage.ContactSelection.ContactInfo
{
    using System;
    using Omnipaste.ContactList;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public class ContactInfoSelectionViewModel : DetailsViewModelBase<ContactInfoPresenter>,
                                                 IContactInfoSelectionViewModel
    {
        private bool _isSelected;

        private DateTime? _lastActivityTime;

        public ContactInfoSelectionViewModel()
        {
            ClickCommand = new Command(ToggleSelection);
        }

        public Command ClickCommand { get; set; }

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