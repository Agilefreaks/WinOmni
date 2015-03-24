﻿namespace Omnipaste.WorkspaceDetails.Version
{
    using Omnipaste.Presenters;

    public class VersionDetailsContentViewModel : WorkspaceDetailsContentViewModel<UpdateInfoPresenter>, IVersionDetailsContentViewModel
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
