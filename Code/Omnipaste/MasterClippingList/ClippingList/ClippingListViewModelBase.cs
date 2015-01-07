namespace Omnipaste.MasterClippingList.ClippingList
{
    using System;
    using System.Linq;
    using Caliburn.Micro;
    using Clipboard.Models;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Clipping;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Helpers;
    using OmniUI.List;

    public abstract class ClippingListViewModelBase : ListViewModelBase<Clipping, IClippingViewModel>,
                                                      IClippingListViewModel
    {
        #region Fields

        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors and Destructors

        protected ClippingListViewModelBase(IObservable<Clipping> entityObservable)
            : base(entityObservable)
        {
        }

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IEventAggregator EventAggregator
        {
            get
            {
                return _eventAggregator;
            }
            set
            {
                _eventAggregator = value;
                _eventAggregator.Subscribe(this);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Handle(DeleteClippingMessage message)
        {
            var clippingViewModel = Items.SingleOrDefault(viewModel => viewModel.Model.UniqueId == message.ClippingId);
            if (clippingViewModel != null)
            {
                DeactivateItem(clippingViewModel, true);
            }
        }

        public void ShowVideoTutorial()
        {
            ExternalProcessHelper.ShowVideoTutorial();
        }

        #endregion

        #region Methods

        protected override IClippingViewModel CreateViewModel(Clipping clipping)
        {
            IClippingViewModel clippingViewModel = new ClippingViewModel();
            clippingViewModel.Model = clipping;

            return clippingViewModel;
        }

        #endregion
    }
}