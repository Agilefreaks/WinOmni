namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;
    using Omnipaste.Framework.EventAggregatorMessages;

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