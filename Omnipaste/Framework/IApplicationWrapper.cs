namespace Omnipaste.Framework
{
    using System.Windows.Threading;

    public interface IApplicationWrapper
    {
        Dispatcher Dispatcher { get; }

        void ShutDown();
    }
}