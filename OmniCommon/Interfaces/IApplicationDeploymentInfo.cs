using System;

namespace OmniCommon.Interfaces
{
    public interface IApplicationDeploymentInfo
    {
        Uri ActivationUri { get; }

        bool HasValidActivationUri { get; }
    }
}