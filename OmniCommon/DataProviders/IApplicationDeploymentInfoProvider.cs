namespace OmniCommon.DataProviders
{
    using System;

    public interface IApplicationDeploymentInfoProvider
    {
        Uri ActivationUri { get; }

        bool HasValidActivationUri { get; }

        bool IsFirstNetworkRun { get; }
    }
}