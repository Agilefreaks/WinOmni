namespace Omnipaste.ClippingList
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Clipboard.Models;
    using Ninject;
    using Omnipaste.Clipping;

    public abstract class ClippingListViewModelBase : Screen, IClippingListViewModel
    {
        #region Constructors and Destructors

        protected ClippingListViewModelBase(IObservable<Clipping> clippingsObservable)
        {
            Clippings = new BindableCollection<IClippingViewModel>();
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

        #endregion

        #region Methods

        protected IClippingViewModel CreateViewModel(Clipping clipping)
        {
            var clippingViewModel = Kernel.Get<IClippingViewModel>();
            clippingViewModel.Model = clipping;

            return clippingViewModel;
        }

        #endregion
    }
}