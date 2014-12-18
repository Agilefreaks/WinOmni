namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Ninject;
    using OmniUI.Framework;
    using OmniUI.List;
    using OmniUI.Presenters;

    public class ContactListContentViewModel : ListViewModelBase<IContactInfoPresenter, IContactViewModel>,
                                               IContactListContentViewModel
    {
        #region Fields

        private readonly IKernel _kernel;

        private readonly IDeepObservableCollectionView _items;

        private string _filterText;

        private bool _selectAll;

        #endregion

        #region Constructors and Destructors

        public ContactListContentViewModel(IObservable<IContactInfoPresenter> contactObservable, IKernel kernel)
            : base(contactObservable)
        {
            _kernel = kernel;
            FilterText = string.Empty;
            ViewModelFilter = IdentifierContainsFilterText;
            FilteredItems.CustomSort = new ContactViewModelComparer();
            _items = new DeepObservableCollectionView<IContactViewModel>(Items) { Filter = ModelIsSelected };
        }

        #endregion

        #region Public Properties

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

        public IDeepObservableCollectionView Contacts
        {
            get
            {
                return _items;
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

        protected override void OnFilterUpdated()
        {
            base.OnFilterUpdated();
            _selectAll = FilteredItems.Count > 0
                         && FilteredItems.Cast<IContactViewModel>().All(viewModel => viewModel.IsSelected);
            NotifyOfPropertyChange(() => SelectAll);
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