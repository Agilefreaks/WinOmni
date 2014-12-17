namespace OmniUI.List
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Data;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;

    public abstract class ListViewModelBase<TEntity, TViewModel> : Conductor<TViewModel>.Collection.AllActive,
                                                                   IDisposable,
                                                                   IStartable
        where TViewModel : class
    {
        #region Constants

        public const int MaxItemCount = 42;

        #endregion

        #region Fields

        private readonly IObservable<TEntity> _entityObservable;

        private readonly ListCollectionView _filteredItems;

        private IDisposable _entityObserver;

        private ListViewModelStatusEnum _status;

        private Func<TViewModel, bool> _viewModelFilter;

        #endregion

        #region Constructors and Destructors

        protected ListViewModelBase(IObservable<TEntity> entityObservable)
        {
            _entityObservable = entityObservable;
            Items.CollectionChanged += OnViewModelsCollectionChanged;
            _filteredItems = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            _filteredItems.Filter = ShouldShowViewModel;
        }

        #endregion

        #region Public Properties

        public virtual Func<TEntity, bool> EntityFilter
        {
            get
            {
                return @event => true;
            }
        }

        public ListCollectionView FilteredItems
        {
            get
            {
                return _filteredItems;
            }
        }

        public ListViewModelStatusEnum Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        #endregion

        #region Properties

        protected Func<TViewModel, bool> ViewModelFilter
        {
            get
            {
                return _viewModelFilter;
            }
            set
            {
                _viewModelFilter = value;
                OnFilterUpdated();
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void ActivateItem(TViewModel item)
        {
            if (!Items.Contains(item) && MaxItemsLimitReached())
            {
                DeactivateItem(Items.Last(), true);
            }

            base.ActivateItem(item);
        }

        public virtual void Dispose()
        {
            if (_entityObserver == null)
            {
                return;
            }
            _entityObserver.Dispose();
            _entityObserver = null;
        }

        public virtual void Start()
        {
            Stop();
            _entityObserver =
                _entityObservable.Where(entity => EntityFilter(entity))
                    .Select(CreateViewModel)
                    .SubscribeAndHandleErrors(ActivateItem);
        }

        public virtual void Stop()
        {
            Dispose();
        }

        #endregion

        #region Methods

        protected abstract TViewModel CreateViewModel(TEntity entity);

        protected override TViewModel EnsureItem(TViewModel newItem)
        {
            var index = Items.IndexOf(newItem);

            if (index == -1)
            {
                Items.Insert(0, newItem);
            }
            else
            {
                newItem = Items[index];
            }

            return base.EnsureItem(newItem);
        }

        protected virtual bool MaxItemsLimitReached()
        {
            return Items.Count == MaxItemCount;
        }

        protected virtual void OnFilterUpdated()
        {
            _filteredItems.Refresh();
        }

        private void OnViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = Items.Any() ? ListViewModelStatusEnum.NotEmpty : ListViewModelStatusEnum.Empty;
        }

        private bool ShouldShowViewModel(object viewModel)
        {
            return ViewModelFilter == null || ViewModelFilter.Invoke(viewModel as TViewModel);
        }

        #endregion
    }
}