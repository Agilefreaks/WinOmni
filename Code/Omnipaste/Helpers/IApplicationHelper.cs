namespace Omnipaste.Helpers
{
    using System.Windows.Threading;
    using Ninject;

    public interface IApplicationHelper
    {
        Dispatcher Dispatcher { get; }

        void Shutdown();

        void StartBackgroundService<TType>() where TType : IStartable;

        void StopBackgroundProcesses();
    }
}