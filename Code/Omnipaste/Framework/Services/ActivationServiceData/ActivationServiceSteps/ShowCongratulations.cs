namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;
    using Omnipaste.Framework.EventAggregatorMessages;

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