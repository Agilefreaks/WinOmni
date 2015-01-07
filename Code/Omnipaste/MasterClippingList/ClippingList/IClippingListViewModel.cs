namespace Omnipaste.MasterClippingList.ClippingList
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public interface IClippingListViewModel : IHandle<DeleteClippingMessage>
    {
    }
}