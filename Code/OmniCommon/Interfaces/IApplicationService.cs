namespace OmniCommon.Interfaces
{
    using System.Windows.Threading;

    public interface IApplicationService
    {
        Dispatcher Dispatcher { get; }

        bool AutoStart { get; set; }

        void ShutDown();
    }
}