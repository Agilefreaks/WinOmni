namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public class ShowCongratulations : PublishMessageStepBase<ShowCongratulationsMessage>
    {
        #region Constructors and Destructors

        public ShowCongratulations(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        #endregion
    }
}