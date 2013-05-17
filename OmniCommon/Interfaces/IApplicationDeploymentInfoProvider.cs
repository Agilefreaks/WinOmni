using System;

namespace OmniCommon.Interfaces
{
    public interface IApplicationDeploymentInfoProvider
    {
        Uri ActivationUri { get; }

        bool HasValidActivationUri { get; }

        bool IsFirstNetworkRun { get; }
    }
}