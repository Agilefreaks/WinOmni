using System;

namespace Omniclipboard
{
    public interface IApplicationDeploymentInfo
    {
        Uri ActivationUri { get; }

        bool HasValidActivationUri { get; }
    }
}