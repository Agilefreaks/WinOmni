namespace Omnipaste.Activity
{
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using OmniUI.Attributes;

    [UseView(typeof(ActivityView))]
    public class VersionActivityViewModel : ActivityViewModel, IVersionActivityViewModel
    {
        private readonly IUpdaterService _updateService;

        public VersionActivityViewModel(IUiRefreshService uiRefreshService, ISessionManager sessionManager, IUpdaterService updateService)
            : base(uiRefreshService, sessionManager)
        {
            _updateService = updateService;
        }

        public bool WasInstalled
        {
            get
            {
                return ((UpdateInfoPresenter)Model.BackingModel).WasInstalled;
            }
        }

        public void UpdateApp()
        {
            _updateService.InstallNewVersion();
        }
    }
}