﻿namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Subjects;
    using Contacts.Models;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniHolidays.Commands;
    using OmniUI.Framework;
    using OmniUI.List;
    using OmniUI.Models;
    using OmniUI.Presenters;
    using OmniUI.Services;

    public class ContactListContentViewModel : ListViewModelBase<IContactInfoPresenter, IContactViewModel>,
                                               IContactListContentViewModel
    {
        #region Fields

        private readonly ICommandService _commandService;

        private readonly Subject<IContactInfoPresenter> _contactInfoSubject;

        private readonly IDeepObservableCollectionView _items;

        private readonly IKernel _kernel;

        private string _filterText;

        private bool _isEnabled;

        private bool _selectAll;

        #endregion

        #region Constructors and Destructors

        public ContactListContentViewModel(ICommandService commandService, IKernel kernel)
            : base(new Subject<IContactInfoPresenter>())
        {
            _commandService = commandService;
            _kernel = kernel;
            _contactInfoSubject = EntityObservable as Subject<IContactInfoPresenter>;
            IsEnabled = true;
            FilterText = string.Empty;
            ViewModelFilter = IdentifierContainsFilterText;
            FilteredItems.CustomSort = new ContactViewModelComparer();
            _items = new DeepObservableCollectionView<IContactViewModel>(Items) { Filter = ModelIsSelected };
        }

        #endregion

        #region Public Properties

        public IDeepObservableCollectionView Contacts
        {
            get
            {
                return _items;
            }
        }

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (value == _filterText)
                {
                    return;
                }
                _filterText = value;
                NotifyOfPropertyChange();
                OnFilterUpdated();
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (value.Equals(_isEnabled))
                {
                    return;
                }
                _isEnabled = value;
                NotifyOfPropertyChange();
            }
        }

        public bool SelectAll
        {
            get
            {
                return _selectAll;
            }
            set
            {
                if (value.Equals(_selectAll))
                {
                    return;
                }
                _selectAll = value;
                NotifyOfPropertyChange();
                UpdateContactSelection();
            }
        }

        #endregion

        #region Methods

        protected override IContactViewModel CreateViewModel(IContactInfoPresenter entity)
        {
            var contactViewModel = _kernel.Get<IContactViewModel>();
            contactViewModel.Model = entity;

            return contactViewModel;
        }

        protected override bool MaxItemsLimitReached()
        {
            return false;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (Items.Count != 0)
            {
                return;
            }
            IsEnabled = false;
            _commandService.Execute(new SyncContactsCommand()).RunToCompletion(OnGotNewContacts, OnGetContactsError);
        }

        protected override void OnFilterUpdated()
        {
            base.OnFilterUpdated();
            _selectAll = FilteredItems.Count > 0
                         && FilteredItems.Cast<IContactViewModel>().All(viewModel => viewModel.IsSelected);
            NotifyOfPropertyChange(() => SelectAll);
        }

        private static ContactListContactInfoPresenter GetContactInfoPresenter(Contact contact)
        {
            return
                new ContactListContactInfoPresenter(
                    new ContactInfo
                        {
                            FirstName = contact.FirstName,
                            LastName = contact.LastName,
                            Phone = contact.PhoneNumber,
                        });
        }

        private static bool ModelIsSelected(object viewModel)
        {
            var contactViewModel = viewModel as IContactViewModel;
            return contactViewModel != null && contactViewModel.IsSelected;
        }

        private bool IdentifierContainsFilterText(IContactViewModel viewModel)
        {
            return (viewModel != null)
                   && (string.IsNullOrWhiteSpace(FilterText)
                       || (CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                           viewModel.Model.Identifier,
                           FilterText,
                           CompareOptions.IgnoreCase) > -1));
        }

        private void OnGetContactsError(Exception exception)
        {
            ExceptionReporter.Instance.Report(exception);
            IsEnabled = true;
        }

        private void OnGotNewContacts(IEnumerable<ContactList> result)
        {
            foreach (var contactInfoPresenter in result.SelectMany(list => list.Contacts).Select(GetContactInfoPresenter))
            {
                _contactInfoSubject.OnNext(contactInfoPresenter);
            }

            IsEnabled = true;
        }

        private void UpdateContactSelection()
        {
            var selectAllChecked = SelectAll;
            foreach (IContactViewModel contactViewModel in FilteredItems)
            {
                contactViewModel.IsSelected = selectAllChecked;
            }
        }

        #endregion
    }
}