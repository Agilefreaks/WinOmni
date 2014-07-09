namespace Omnipaste.Framework
{
    using System.Windows.Threading;

    public interface IApplicationService
    {
        Dispatcher Dispatcher { get; }

        void ShutDown();
    }
}