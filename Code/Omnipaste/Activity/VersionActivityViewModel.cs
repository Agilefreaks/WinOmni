namespace Omnipaste.Activity
{
    using Omnipaste.Services;
    using OmniUI.Attributes;

    [UseView(typeof(ActivityView))]
    public class VersionActivityViewModel : ActivityViewModel, IVersionActivityViewModel
    {
        private readonly IUpdaterService _updateService;

        public VersionActivityViewModel(IUiRefreshService uiRefreshService, IUpdaterService updateService)
            : base(uiRefreshService)
        {
            _updateService = updateService;
        }

        public void UpdateApp()
        {
            _updateService.InstallNewVersion();
        }
    }
}