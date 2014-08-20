namespace Omnipaste.Framework
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.MasterClippingList;

    public abstract class ListViewModelBase<TEntity, TViewModel> : Screen
    {
        #region Constants

        private const int ListLimit = 42;

        #endregion

        #region Fields

        private ListViewModelStatusEnum _status;

        #endregion

        #region Constructors and Destructors

        protected ListViewModelBase(IObservable<TEntity> entityObservable)
        {
            Items = new LimitableBindableCollection<TViewModel>(ListLimit);
            Items.CollectionChanged += OnViewModelsCollectionChanged;

            EntityObservable = entityObservable;
            EntityObservable.Where(entity => Filter(entity))
                .Select(CreateViewModel)
                .Subscribe(clippingViewModel => Items.Insert(0, clippingViewModel), exception => Debugger.Break());
        }

        #endregion

        #region Public Properties

        public IObservable<TEntity> EntityObservable { get; set; }

        public virtual Func<TEntity, bool> Filter
        {
            get
            {
                return @event => true;
            }
        }

        [Inject]
        public IKernel Kernel { get; set; }

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

        public IObservableCollection<TViewModel> Items { get; set; }

        #endregion

        #region Methods

        protected abstract TViewModel CreateViewModel(TEntity entity);

        private void OnViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = Items.Any() ? ListViewModelStatusEnum.NotEmpty : ListViewModelStatusEnum.Empty;
        }

        #endregion
    }
}