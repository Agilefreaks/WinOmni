using System;

namespace PubNubClipboard
{
    public interface IApplicationDeploymentInfo
    {
        Uri ActivationUri { get; }

        bool HasValidActivationUri { get; }
    }
}