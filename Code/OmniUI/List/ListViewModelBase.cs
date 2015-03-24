namespace OmniUI.List
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows.Data;
    using Caliburn.Micro;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniUI.Details;

    public abstract class ListViewModelBase<TPresenter, TViewModel> : Conductor<TViewModel>.Collection.AllActive,
                                                                  IListViewModel<TViewModel>
        where TViewModel : class, IDetailsViewModel
    {
        #region Constructors and Destructors

        protected ListViewModelBase()
        {
            Subscriptions = new CompositeDisposable();
            Items.CollectionChanged += OnViewModelsCollectionChanged;
            _filteredItems = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            _filteredItems.Filter = vm => CanShow((TViewModel)vm);
        }

        #endregion

        #region Properties

        protected virtual bool InsertItemsAtBottom { get; set; }

        #endregion

        #region Fields

        protected readonly CompositeDisposable Subscriptions;

        private readonly ListCollectionView _filteredItems;

        private ListViewModelStatusEnum _status;

        #endregion

        #region Public Properties

        public virtual int MaxItemCount
        {
            get
            {
                return 42;
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
                if (_status == value)
                {
                    return;
                }

                _status = value;
                NotifyOfPropertyChange();
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

        public virtual void RefreshItems()
        {
            _filteredItems.Refresh();

            Status = FilteredItems.Count == 0 ? ListViewModelStatusEnum.EmptyFilter : ListViewModelStatusEnum.NotEmpty;
        }

        #endregion

        #region Methods

        protected virtual void ChangeItem(TPresenter presenter)
        {
            var viewModel = ChangeViewModel(presenter);
            ActivateItem(viewModel);
        }

        protected void RemoveItem(TPresenter entity)
        {
            var viewModel = GetViewModel(entity);
            if (viewModel == null)
            {
                return;
            }
            DeactivateItem(viewModel, true);
        }

        protected virtual bool CanShow(TViewModel viewModel)
        {
            return true;
        }

        protected abstract TViewModel ChangeViewModel(TPresenter model);

        protected override TViewModel EnsureItem(TViewModel newItem)
        {
            var index = Items.IndexOf(newItem);

            if (index == -1)
            {
                if (InsertItemsAtBottom)
                {
                    Items.Add(newItem);
                }
                else
                {
                    Items.Insert(0, newItem);
                }
            }
            else
            {
                newItem = Items[index];
            }

            return base.EnsureItem(newItem);
        }

        protected virtual IObservable<TPresenter> GetFetchItemsObservable()
        {
            return Observable.Empty<TPresenter>(SchedulerProvider.Default);
        }

        protected virtual IObservable<TPresenter> GetItemChangedObservable()
        {
            return Observable.Empty<TPresenter>(SchedulerProvider.Default);
        }

        protected virtual IObservable<TPresenter> GetItemRemovedObservable()
        {
            return Observable.Empty<TPresenter>(SchedulerProvider.Default);
        }

        protected TViewModel GetViewModel(TPresenter entity)
        {
            return Items.FirstOrDefault(vm => Equals(vm.Model, entity));
        }

        protected virtual bool MaxItemsLimitReached()
        {
            return MaxItemCount != 0 && Items.Count == MaxItemCount;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Subscriptions.Add(
                GetFetchItemsObservable()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .Subscribe(
                        ChangeItem,
                        e =>
                        {
                            SimpleLogger.Log("Exception encountered: " + e);
                            ExceptionReporter.Instance.Report(e);
                        },
                        ItemsAdded));
            Subscriptions.Add(
                GetItemChangedObservable()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(ChangeItem));
            Subscriptions.Add(
                GetItemRemovedObservable()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(RemoveItem));
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                Subscriptions.Dispose();
            }

            base.OnDeactivate(close);
        }

        protected virtual void ItemsAdded()
        {
            
        }

        private void OnViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = Items.Any() ? ListViewModelStatusEnum.NotEmpty : ListViewModelStatusEnum.Empty;
        }

        #endregion
    }
}