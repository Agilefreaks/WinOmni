namespace Omnipaste.GroupMessage.ContactSelection
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ContactList;
    using Omnipaste.GroupMessage.ContactSelection.ContactInfo;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;

    public class ContactSelectionViewModel : ContactListViewModelBase<IContactInfoSelectionViewModel>,
                                             IContactSelectionViewModel
    {
        private IContactInfoViewModel _contactInfoViewModel;

        private IDisposable _itemsSelectedSubscription;

        private ContactInfoPresenter _pendingContact;

        private IObservable<EventPattern<PropertyChangedEventArgs>> _selectionChangedObservable;

        public ContactSelectionViewModel(   
            IContactRepository contactRepository,
            IContactInfoViewModelFactory contactInfoViewModelFactory)
            : base(contactRepository, contactInfoViewModelFactory)
        {
            _selectionChangedObservable = Observable.Empty<EventPattern<PropertyChangedEventArgs>>();
        }

        public ContactInfoPresenter PendingContact
        {
            get
            {
                return _pendingContact
                       ?? (_pendingContact = new ContactInfoPresenter(new Models.ContactInfo()));
            }
            set
            {
                if (_pendingContact != value)
                {
                    _pendingContact = value;
                    NotifyOfPropertyChange(() => _pendingContact);
                }
            }
        }


        public bool IsInitialized
        {
            get
            {
                return _itemsSelectedSubscription != null;
            }
        }

        public override void ActivateItem(IContactInfoSelectionViewModel item)
        {
            base.ActivateItem(item);

            TrackSelectionForNewItem(item);

            if (IsInitialized)
            {
                ItemsAdded();
            }

            if (item.Model.Identifier == PendingContact.Identifier)
            {
                AddedPendingContact(item);
            }
        }

        private void TrackSelectionForNewItem(IContactInfoSelectionViewModel item)
        {
            _selectionChangedObservable =
                _selectionChangedObservable.Merge(
                    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => item.PropertyChanged += h,
                        h => item.PropertyChanged -= h).Where(ep => ep.EventArgs.PropertyName == "IsSelected"));
        }

        public override void NotifyOfPropertyChange(string propertyName)
        {
            base.NotifyOfPropertyChange(propertyName);

            if (propertyName == "FilterText")
            {
                var phoneNumber = Regex.Replace(FilterText, "[^+0-9]", "");
                PendingContact.PhoneNumber = phoneNumber;
            }
        }

        public void AddPendingContact()
        {
            ContactRepository.Save(PendingContact.ContactInfo).Subscribe();
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

        private void AddedPendingContact(IContactInfoSelectionViewModel item)
        {
            item.IsSelected = true;
            _pendingContact = new ContactInfoPresenter(new Models.ContactInfo());
            NotifyOfPropertyChange(() => PendingContact);
            FilterText = "";
        }

        private void ItemSelectionChanged(EventPattern<PropertyChangedEventArgs> selectionChangedEvent)
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