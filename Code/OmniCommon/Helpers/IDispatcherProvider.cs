namespace OmniCommon.Helpers
{
    public interface IDispatcherProvider
    {
        IDispatcher Current { get; }

        IDispatcher Application { get; }
    }
}