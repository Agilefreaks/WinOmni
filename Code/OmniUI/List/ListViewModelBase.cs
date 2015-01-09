namespace OmniUI.List
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Data;
    using Caliburn.Micro;
    using Ninject;

    public abstract class ListViewModelBase<TEntity, TViewModel> : Conductor<TViewModel>.Collection.AllActive,
                                                                   IDisposable,
                                                                   IStartable
        where TViewModel : class
    {
        #region Constants

        public const int MaxItemCount = 42;

        #endregion

        #region Fields

        private readonly IDictionary<TEntity, TViewModel> _viewModelDictionary;

        private readonly ListCollectionView _filteredItems;

        private ListViewModelStatusEnum _status;

        #endregion

        #region Constructors and Destructors

        protected ListViewModelBase()
        {
            Items.CollectionChanged += OnViewModelsCollectionChanged;
            _viewModelDictionary = new Dictionary<TEntity, TViewModel>();
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

        public void AddItem(TEntity entity)
        {
            var viewModel = CreateViewModel(entity);
            _viewModelDictionary.Add(entity, viewModel);
            ActivateItem(viewModel);
        }

        public void RemoveItem(TEntity entity)
        {
            var viewModel = _viewModelDictionary.ContainsKey(entity) ? _viewModelDictionary[entity] : null;
            if (viewModel == null)
            {
                return;
            }
            DeactivateItem(viewModel, true);
            _viewModelDictionary.Remove(entity);
        }

        public virtual void RefreshItems()
        {
            _filteredItems.Refresh();
        }

        public virtual void Dispose()
        {
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

        private void OnViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = Items.Any() ? ListViewModelStatusEnum.NotEmpty : ListViewModelStatusEnum.Empty;
        }

        #endregion
    }
}