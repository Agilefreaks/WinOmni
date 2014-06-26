namespace Omnipaste.ClippingList
{
    using System;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Clipboard.Enums;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Ninject;
    using Omnipaste.Clipping;

    public class ClippingListViewModel : Screen, IClippingListViewModel
    {
        #region Constructors and Destructors

        public ClippingListViewModel(IClipboardHandler clipboardHandler)
        {
            Clippings = new BindableCollection<IClippingViewModel>();
            CloudClippings = new BindableCollection<IClippingViewModel>();
            LocalClippings = new BindableCollection<IClippingViewModel>();

            ClipboardHandler = clipboardHandler;
            SetupCollections();
        }

        #endregion

        #region Public Properties

        public IClipboardHandler ClipboardHandler { get; set; }

        public IObservableCollection<IClippingViewModel> Clippings { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        public IObservableCollection<IClippingViewModel> LocalClippings { get; set; }

        public IObservableCollection<IClippingViewModel> CloudClippings { get; set; }

        #endregion

        #region Methods

        private void SetupCollections()
        {
            ClipboardHandler.Select(CreateViewModel).Subscribe(x => Clippings.Insert(0, x));

            ClipboardHandler.Where(c => c.Source == ClippingSourceEnum.Cloud)
                .Select(CreateViewModel)
                .Subscribe(x => CloudClippings.Insert(0, x));

            ClipboardHandler.Where(c => c.Source == ClippingSourceEnum.Local)
                .Select(CreateViewModel)
                .Subscribe(x => LocalClippings.Insert(0, x));
        }

        private IClippingViewModel CreateViewModel(Clipping clipping)
        {
            var clippingViewModel = Kernel.Get<IClippingViewModel>();
            clippingViewModel.Model = clipping;

            return clippingViewModel;
        }

        #endregion
    }
}