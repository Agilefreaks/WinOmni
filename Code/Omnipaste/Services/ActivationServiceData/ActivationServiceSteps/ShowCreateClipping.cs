namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public class ShowCreateClipping : PublishMessageStepBase<ShowCreateClippingMessage>
    {
        #region Constructors and Destructors

        public ShowCreateClipping(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        #endregion
    }
}