namespace Omnipaste.ActivityList.Activity
{
    using Omnipaste.Framework.Entities;
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
                return ((UpdateEntity)Model.BackingEntity).WasInstalled;
            }
        }

        public void UpdateApp()
        {
            _updateService.InstallNewVersion();
        }
    }
}