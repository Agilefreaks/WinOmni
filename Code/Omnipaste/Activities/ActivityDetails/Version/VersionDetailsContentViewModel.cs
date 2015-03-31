namespace Omnipaste.Activities.ActivityDetails.Version
{
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public class VersionDetailsContentViewModel : DetailsViewModelBase<UpdateModel>, IVersionDetailsContentViewModel
    {
        public string ReleaseLog
        {
            get
            {
                return Model.ReleaseLog;
            }
        }

        public bool WasInstalled
        {
            get
            {
                return Model.WasInstalled;
            }
        }
    }
}
