using System;

namespace ClipboardWatcher.Core
{
    public interface IApplicationDeploymentInfo
    {
        Uri ActivationUri { get; }

        bool HasValidActivationUri { get; }
    }
}