﻿using System;
using System.Deployment.Application;
using OmniCommon.Interfaces;

namespace Omnipaste.Services
{
    public class ApplicationDeploymentWrapper : IApplicationDeploymentInfoProvider
    {
        public Uri ActivationUri
        {
            get { return ApplicationDeployment.CurrentDeployment.ActivationUri; }
        }

        public bool HasValidActivationUri
        {
            get { return ApplicationDeployment.IsNetworkDeployed && ActivationUri != null; }
        }

        public bool IsFirstNetworkRun
        {
            get { return ApplicationDeployment.IsNetworkDeployed || ApplicationDeployment.CurrentDeployment.IsFirstRun; }
        }
    }
}