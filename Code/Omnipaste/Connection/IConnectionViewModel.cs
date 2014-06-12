namespace Omnipaste.Connection
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public interface IConnectionViewModel : IScreen, IHandleWithTask<ConfigurationCompletedMessage>
    {
    }
}