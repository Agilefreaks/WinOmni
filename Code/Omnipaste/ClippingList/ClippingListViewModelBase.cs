namespace Omnipaste.ClippingList
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Clipboard.Models;
    using Ninject;
    using Omnipaste.Clipping;

    public abstract class ClippingListViewModelBase : Screen, IClippingListViewModel
    {
        private ClippingListViewModelStatusEnum _status;

        #region Constructors and Destructors

        protected ClippingListViewModelBase(IObservable<Clipping> clippingsObservable)
        {
            Clippings = new LimitableBindableCollection<IClippingViewModel>(42);
            Clippings.CollectionChanged += ClippingsCollectionChanged;

            ClippingsObservable = clippingsObservable;
            ClippingsObservable
                .Select(CreateViewModel)
                .Subscribe(
                    clippingViewModel => Clippings.Insert(0, clippingViewModel), 
                    exception => Debugger.Break());
        }

        #endregion

        #region Public Properties

        public IObservableCollection<IClippingViewModel> Clippings { get; set; }

        public IObservable<Clipping> ClippingsObservable { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        public ClippingListViewModelStatusEnum Status
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

        #region Methods

        protected IClippingViewModel CreateViewModel(Clipping clipping)
        {
            var clippingViewModel = Kernel.Get<IClippingViewModel>();
            clippingViewModel.Model = clipping;

            return clippingViewModel;
        }

        private void ClippingsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = Clippings.Any() 
                ? ClippingListViewModelStatusEnum.NotEmpty 
                : ClippingListViewModelStatusEnum.Empty;
        }

        #endregion
    }
}