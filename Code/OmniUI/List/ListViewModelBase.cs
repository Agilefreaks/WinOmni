namespace OmniUI.List
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Data;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniUI.Details;

    public abstract class ListViewModelBase<TModel, TViewModel> : Conductor<TViewModel>.Collection.AllActive,
                                                                   IDisposable,
                                                                   IStartable
        where TViewModel : class, IDetailsViewModel
    {
        #region Constants

        public const int MaxItemCount = 42;

        #endregion

        #region Fields

        private readonly ListCollectionView _filteredItems;

        private ListViewModelStatusEnum _status;

        protected readonly List<IDisposable> Subscriptions;

        #endregion

        #region Constructors and Destructors
        
        protected ListViewModelBase()
        {
            Subscriptions = new List<IDisposable>();
            Items.CollectionChanged += OnViewModelsCollectionChanged;
            _filteredItems = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            _filteredItems.Filter = vm => CanShow((TViewModel) vm);
        }

        #endregion

        #region Public Properties

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

        #region Public Methods and Operators

        public override void ActivateItem(TViewModel item)
        {
            if (!Items.Contains(item) && MaxItemsLimitReached())
            {
                DeactivateItem(Items.Last(), true);
            }

            base.ActivateItem(item);
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
            Dispose();
        }

        protected virtual bool CanShow(TViewModel viewModel)
        {
            return true;
        }

        public void AddItem(TModel model)
        {
            var viewModel = CreateViewModel(model);
            ActivateItem(viewModel);
        }

        public void AddItems(IEnumerable<TModel> models)
        {
            models.ToList().ForEach(AddItem);
        }

        public void RemoveItem(TModel entity)
        {
            var viewModel = GetViewModel(entity);
            if (viewModel == null)
            {
                return;
            }
            DeactivateItem(viewModel, true);
        }

        public virtual void RefreshItems()
        {
            _filteredItems.Refresh();
        }

        public virtual void Dispose()
        {
            DisposeSubscriptions();
        }
        
        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            Subscriptions.Add(GetFetchItemsObservable().SubscribeOn(SchedulerProvider.Default).ObserveOn(SchedulerProvider.Dispatcher).SubscribeAndHandleErrors(AddItems));
            Subscriptions.Add(GetItemAddedObservable().SubscribeOn(SchedulerProvider.Default).ObserveOn(SchedulerProvider.Dispatcher).SubscribeAndHandleErrors(AddItem));
            Subscriptions.Add(GetItemRemovedObservable().SubscribeOn(SchedulerProvider.Default).ObserveOn(SchedulerProvider.Dispatcher).SubscribeAndHandleErrors(RemoveItem));
        }

        protected override void OnDeactivate(bool close)
        {
            DisposeSubscriptions();
            
            base.OnDeactivate(true);
        }

        protected TViewModel GetViewModel(TModel entity)
        {
            return Items.FirstOrDefault(vm => Equals(vm.Model, entity));
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

        protected abstract TViewModel CreateViewModel(TModel model);

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

        private void OnViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = Items.Any() ? ListViewModelStatusEnum.NotEmpty : ListViewModelStatusEnum.Empty;
        }

        private void DisposeSubscriptions()
        {
            Subscriptions.ForEach(s => s.Dispose());
            Subscriptions.Clear();
        }

        #endregion
    }
}