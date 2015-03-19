namespace Omnipaste.GroupMessage.ContactSelection
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ContactList;
    using Omnipaste.GroupMessage.ContactSelection.ContactInfo;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;

    public class ContactSelectionViewModel : ContactListViewModelBase<IContactInfoSelectionViewModel>, IContactSelectionViewModel
    {
        private IObservable<EventPattern<PropertyChangedEventArgs>> _selectionChangedObservable;

        private IDisposable _itemsSelectedSubscription;

        private ObservableCollection<ContactInfoPresenter> _selectedContacts = new ObservableCollection<ContactInfoPresenter>();

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

        public ContactSelectionViewModel(IContactRepository contactRepository, IConversationProvider conversationProvider, IContactInfoViewModelFactory contactInfoViewModelFactory)
            : base(contactRepository, conversationProvider, contactInfoViewModelFactory)
        {
            _selectionChangedObservable = Observable.Empty<EventPattern<PropertyChangedEventArgs>>();
        }

        public override void ActivateItem(IContactInfoSelectionViewModel item)
        {
            base.ActivateItem(item);

            _selectionChangedObservable = _selectionChangedObservable.Merge(
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => item.PropertyChanged += h,
                    h => item.PropertyChanged -= h)
                    .Where(ep => ep.EventArgs.PropertyName == "IsSelected"));
        }

        protected override void ItemsAdded()
        {
            base.ItemsAdded();

            if (_itemsSelectedSubscription != null)
            {
                _itemsSelectedSubscription.Dispose();
                _itemsSelectedSubscription = null;
            }

            _itemsSelectedSubscription = _selectionChangedObservable.SubscribeAndHandleErrors(ItemSelectionChanged);
        }

        void ItemSelectionChanged(EventPattern<PropertyChangedEventArgs> selectionChangedEvent)
        {
            var contactInfoSelectionViewModel = (ContactInfoSelectionViewModel)selectionChangedEvent.Sender;

            if (contactInfoSelectionViewModel.IsSelected)
            {
                SelectedContacts.Add(contactInfoSelectionViewModel.Model);
            }
            else
            {
                SelectedContacts.Remove(contactInfoSelectionViewModel.Model);
            }
        }
    }
}