namespace Omnipaste.GroupMessage
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.GroupMessage.ContactSelection;
    using Omnipaste.GroupMessage.GroupMessageDetails;
    using Omnipaste.Presenters;

    public class GroupMessageViewModel : Screen, IGroupMessageViewModel
    {
        private IContactSelectionViewModel _contactSelection;

        private IGroupMessageDetailsViewModel _groupMessageDetails;

        private ObservableCollection<ContactInfoPresenter> _selectedContacts = new ObservableCollection<ContactInfoPresenter>();

        public IContactSelectionViewModel ContactSelection
        {
            get
            {
                return _contactSelection;
            }

            set
            {
                if (_contactSelection == value)
                {
                    return;
                }

                _contactSelection = value;
                _contactSelection.SelectedContacts = SelectedContacts;
                NotifyOfPropertyChange(() => ContactSelection);
            }
        }

        public IGroupMessageDetailsViewModel GroupMessageDetails
        {
            get
            {
                return _groupMessageDetails;
            }
            set
            {
                if (_groupMessageDetails == value)
                {
                    return;
                }

                _groupMessageDetails = value;
                _groupMessageDetails.Recipients = SelectedContacts;
                NotifyOfPropertyChange(() => GroupMessageDetails);
            }
        }

        public ObservableCollection<ContactInfoPresenter> SelectedContacts
        {
            get
            {
                return _selectedContacts;
            }
            set
            {
                if (_selectedContacts == value)
                {
                    return;
                }

                _selectedContacts = value;
                NotifyOfPropertyChange(() => SelectedContacts);
            }
        }

        public GroupMessageViewModel(IContactSelectionViewModel contactSelectionViewModel, IGroupMessageDetailsViewModel groupMessageDetailsViewModel)
        {
            ContactSelection = contactSelectionViewModel;
            GroupMessageDetails = groupMessageDetailsViewModel;    
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ContactSelection.Activate();
        }
    }
}