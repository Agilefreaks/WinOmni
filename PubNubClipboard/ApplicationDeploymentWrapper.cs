using System;
using System.Deployment.Application;

namespace PubNubClipboard
{
    public class ApplicationDeploymentWrapper : IApplicationDeploymentInfo
    {
        public Uri ActivationUri
        {
            get { return ApplicationDeployment.CurrentDeployment.ActivationUri; }
        }

        public bool HasValidActivationUri
        {
            get { return ApplicationDeployment.IsNetworkDeployed && ActivationUri != null; }
        }
    }
}