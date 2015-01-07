namespace Omnipaste.ActivityList
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public interface IActivityListViewModel : IScreen, IHandle<DeleteClippingMessage>
    {
    }
}