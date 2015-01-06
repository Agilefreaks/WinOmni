namespace Omnipaste.ActivityDetails.Version
{
    public class VersionDetailsContentViewModel : ActivityDetailsContentViewModel, IVersionDetailsContentViewModel
    {
        public string ReleaseLog
        {
            get
            {
                return Model.ExtraData.ReleaseLog;
            }
        }
    }
}
