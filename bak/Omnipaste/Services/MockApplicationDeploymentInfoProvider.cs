namespace Omnipaste.Services
{
    using System;
    using OmniCommon.DataProviders;

    public class MockApplicationDeploymentInfoProvider : IApplicationDeploymentInfoProvider
    {
        public Uri ActivationUri
        {
            get
            {
                return new Uri("http://test.com?token=testToken");
            }
        }

        public bool HasValidActivationUri
        {
            get
            {
                return true;
            }
        }

        public bool IsFirstNetworkRun
        {
            get
            {
                return true;
            }
        }
    }
}