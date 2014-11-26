namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public class ShowCongratulations : SynchronousStepBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ShowCongratulations(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            _eventAggregator.PublishOnUIThread(new ShowCongratulationsMessage());
            return new ExecuteResult(SimpleStepStateEnum.Successful);
        }
    }
}