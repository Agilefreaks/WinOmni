namespace Omnipaste.DataProviders
{
    using System;
    using System.Deployment.Application;
    using OmniCommon.DataProviders;

    public class ApplicationDeploymentInfoProvider : IApplicationDeploymentInfoProvider
    {
        public Uri ActivationUri
        {
            get { return ApplicationDeployment.CurrentDeployment.ActivationUri; }
        }

        public bool HasValidActivationUri
        {
            get { return ApplicationDeploymentHelper.IsClickOnceApplication && ActivationUri != null; }
        }

        public bool IsFirstNetworkRun
        {
            get { return ApplicationDeploymentHelper.IsClickOnceApplication && ApplicationDeployment.CurrentDeployment.IsFirstRun; }
        }
    }
}