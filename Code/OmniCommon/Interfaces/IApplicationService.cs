namespace OmniCommon.Interfaces
{
    using System;
    using System.Windows.Threading;

    public interface IApplicationService
    {
        Dispatcher Dispatcher { get; }

        Version Version { get; }

        void ShutDown();
    }
}