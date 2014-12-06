namespace Omnipaste.Framework
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;

    public abstract class ListViewModelBase<TEntity, TViewModel> : Screen, IDisposable
    {
        #region Constants

        private const int ListLimit = 42;

        #endregion

        #region Fields

        private readonly IDisposable _entityObserver;

        private ListViewModelStatusEnum _status;

        #endregion

        #region Constructors and Destructors

        protected ListViewModelBase(IObservable<TEntity> entityObservable)
        {
            Items = new LimitableBindableCollection<TViewModel>(ListLimit);
            Items.CollectionChanged += OnViewModelsCollectionChanged;

            _entityObserver =
                entityObservable.Where(entity => Filter(entity))
                    .Select(CreateViewModel)
                    .SubscribeAndHandleErrors(clippingViewModel => Items.Insert(0, clippingViewModel));
        }

        #endregion

        #region Public Properties

        public virtual Func<TEntity, bool> Filter
        {
            get
            {
                return @event => true;
            }
        }

        public IObservableCollection<TViewModel> Items { get; set; }

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

        #endregion

        #region Public Methods and Operators

        public virtual void Dispose()
        {
            _entityObserver.Dispose();
        }

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