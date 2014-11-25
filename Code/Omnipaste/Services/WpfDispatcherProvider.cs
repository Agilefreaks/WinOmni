namespace Omnipaste.Services
{
    using OmniCommon.Helpers;

    public class WpfDispatcherProvider : IDispatcherProvider
    {
        public IDispatcher Current
        {
            get
            {
                return DispatcherWrapper.FromCurrent();
            }
        }

        public IDispatcher Application
        {
            get
            {
                return DispatcherWrapper.FromGiven(System.Windows.Application.Current.Dispatcher);
            }
        }
    }
}