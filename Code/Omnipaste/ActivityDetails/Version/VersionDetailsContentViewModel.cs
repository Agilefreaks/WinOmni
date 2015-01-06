namespace Omnipaste.ActivityDetails.Version
{
    public class VersionDetailsContentViewModel : ActivityDetailsContentViewModel, IVersionDetailsContentViewModel
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
