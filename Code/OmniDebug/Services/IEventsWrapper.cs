namespace OmniDebug.Services
{
    using Events.Api.Resources.v1;
    using Events.Models;

    public interface IEventsWrapper : IEvents
    {
        void MockLast(Event @event);
    }
}