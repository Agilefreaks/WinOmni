using System;
using OmniCommon.Interfaces;

namespace Omnipaste.Services
{
    public class MockApplicationDeploymentInfoProvider : IApplicationDeploymentInfoProvider
    {
        public Uri ActivationUri { get { return new Uri("http://test.com?token=testToken"); } }

        public bool HasValidActivationUri { get { return true; } }

        public bool IsFirstNetworkRun { get { return true; } }
    }
}