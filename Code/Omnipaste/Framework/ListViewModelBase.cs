namespace Omnipaste.Framework
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;

    public abstract class ListViewModelBase<TEntity, TViewModel> : Conductor<TViewModel>.Collection.AllActive, IDisposable
        where TViewModel : class
    {
        #region Constants

        public const int MaxItemCount = 42;

        #endregion

        #region Fields

        private readonly IDisposable _entityObserver;

        private ListViewModelStatusEnum _status;

        #endregion

        #region Constructors and Destructors

        protected ListViewModelBase(IObservable<TEntity> entityObservable)
        {
            Items.CollectionChanged += OnViewModelsCollectionChanged;
            _entityObserver =
                entityObservable.Where(entity => Filter(entity))
                    .Select(CreateViewModel)
                    .SubscribeAndHandleErrors(ActivateItem);
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

        public override void ActivateItem(TViewModel item)
        {
            if (!Items.Contains(item) && Items.Count == MaxItemCount)
            {
                DeactivateItem(Items.Last(), true);
            }

            base.ActivateItem(item);
        }

        public virtual void Dispose()
        {
            _entityObserver.Dispose();
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

        private void OnViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = Items.Any() ? ListViewModelStatusEnum.NotEmpty : ListViewModelStatusEnum.Empty;
        }

        #endregion
    }
}