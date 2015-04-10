namespace Omnipaste.WorkspaceDetails.Version
{
    using Omnipaste.Framework.Models;

    public class VersionDetailsContentViewModel : WorkspaceDetailsContentViewModel<UpdateModel>, IVersionDetailsContentViewModel
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
