namespace OmniUI.Helpers
{
    using System.Windows.Threading;

    public interface IApplicationHelper
    {
        object FindResource(string key);

        void Shutdown();

        Dispatcher Dispatcher { get; }
    }
}