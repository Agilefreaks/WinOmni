namespace Events
{
    using Events.Api.Resources.v1;
    using Events.Handlers;
    using Ninject.Modules;
    using OmniCommon.Interfaces;

    public class EventsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IEvents>().To<Events>().InSingletonScope();

            Kernel.Bind<IHandler, IEventsHandler>().To<EventsHandler>().InSingletonScope();
        }
    }
}