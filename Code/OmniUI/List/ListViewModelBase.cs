namespace OmniUI.List
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows.Data;
    using Caliburn.Micro;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniUI.Details;

    public abstract class ListViewModelBase<TModel, TViewModel> : Conductor<TViewModel>.Collection.AllActive, IListViewModel<TViewModel>
        where TViewModel : class, IDetailsViewModel
    {
        #region Fields

        protected readonly CompositeDisposable Subscriptions;

        private readonly ListCollectionView _filteredItems;

        private ListViewModelStatusEnum _status;

        #endregion

        #region Constructors and Destructors

        protected ListViewModelBase()
        {
            Subscriptions = new CompositeDisposable();
            Items.CollectionChanged += OnViewModelsCollectionChanged;
            _filteredItems = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            _filteredItems.Filter = vm => CanShow((TViewModel)vm);
        }

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

        #region Properties

        protected virtual bool InsertItemsAtBottom { get; set; }

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
        }

        #endregion

        #region Methods

        protected virtual void AddItem(TModel model)
        {
            var viewModel = CreateViewModel(model);
            ActivateItem(viewModel);
        }

        protected void RemoveItem(TModel entity)
        {
            var viewModel = GetViewModel(entity);
            if (viewModel == null)
            {
                return;
            }
            DeactivateItem(viewModel, true);
        }

        //This is used as opposed to CollectionViewSource.Refresh to update the position of an item in a collection view when an item is changed.
        //This is done because calling Refresh would result in re-creating the associated views for all the other visible items in the collection.
        protected void RefreshViewForItem(TModel entity)
        {
            RemoveItem(entity);
            AddItem(entity);
        }

        protected virtual bool CanShow(TViewModel viewModel)
        {
            return true;
        }

        protected abstract TViewModel CreateViewModel(TModel model);

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

        protected virtual IObservable<IEnumerable<TModel>> GetFetchItemsObservable()
        {
            return Observable.Empty<IEnumerable<TModel>>(SchedulerProvider.Default);
        }

        protected virtual IObservable<TModel> GetItemAddedObservable()
        {
            return Observable.Empty<TModel>(SchedulerProvider.Default);
        }

        protected virtual IObservable<TModel> GetItemRemovedObservable()
        {
            return Observable.Empty<TModel>(SchedulerProvider.Default);
        }

        protected TViewModel GetViewModel(TModel entity)
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
                    .SubscribeAndHandleErrors(AddItems));
            Subscriptions.Add(
                GetItemAddedObservable()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(AddItem));
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

        private void AddItems(IEnumerable<TModel> models)
        {
            models.ToList().ForEach(AddItem);
        }

        private void OnViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = Items.Any() ? ListViewModelStatusEnum.NotEmpty : ListViewModelStatusEnum.Empty;
        }

        #endregion
    }
}