namespace OmniDebug.Services
{
    using Events.Api.Resources.v1;
    using Events.Models;

    public class EventsWrapper : ResourceWrapperBase<Event>, IEventsWrapper
    {
        public EventsWrapper(IEvents originalResource)
            : base(originalResource)
        {
        }
    }
}