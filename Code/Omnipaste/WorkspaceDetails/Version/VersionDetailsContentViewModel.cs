namespace Omnipaste.WorkspaceDetails.Version
{
    using Omnipaste.Presenters;

    public class VersionDetailsContentViewModel : WorkspaceDetailsContentViewModel<ActivityPresenter>, IVersionDetailsContentViewModel
    {
        public string ReleaseLog
        {
            get
            {
                return Model.ExtraData.UpdateInfo.ReleaseLog;
            }
        }

        public bool WasInstalled
        {
            get
            {
                return Model.ExtraData.UpdateInfo.WasInstalled;
            }
        }
    }
}
