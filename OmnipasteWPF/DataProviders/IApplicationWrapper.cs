namespace OmnipasteWPF.DataProviders
{
    using System.Windows.Threading;

    public interface IApplicationWrapper
    {
        Dispatcher Dispatcher { get; }

        void ShutDown();
    }
}