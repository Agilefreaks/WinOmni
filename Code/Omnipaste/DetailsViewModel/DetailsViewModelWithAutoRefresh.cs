namespace Omnipaste.DetailsViewModel
{
    using System;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Services;

    public abstract class DetailsViewModelWithAutoRefresh<TModel> : DetailsViewModelBase<TModel>, IDetailsViewModelWithAutoRefresh<TModel>
        where TModel : class
    {
        #region Fields

        private readonly IUiRefreshService _uiRefreshService;

        private IDisposable _refreshSubscription;

        #endregion

        #region Constructors and Destructors

        protected DetailsViewModelWithAutoRefresh(IUiRefreshService uiRefreshService)
        {
            _uiRefreshService = uiRefreshService;
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            DisposeUiRefreshSubscription();
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            AddUiRefreshSubscription();
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            DisposeUiRefreshSubscription();
            base.OnDeactivate(close);
        }

        private void AddUiRefreshSubscription()
        {
            DisposeUiRefreshSubscription();
            _refreshSubscription = _uiRefreshService.RefreshObservable.SubscribeAndHandleErrors(_ => RefreshUi());
        }

        private void DisposeUiRefreshSubscription()
        {
            if (_refreshSubscription == null)
            {
                return;
            }
            _refreshSubscription.Dispose();
            _refreshSubscription = null;
        }

        private void RefreshUi()
        {
            NotifyOfPropertyChange(() => Model);
        }

        #endregion
    }
}