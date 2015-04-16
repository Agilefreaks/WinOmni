namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;

    public abstract class PublishMessageStepBase<TMessage> : SynchronousStepBase
        where TMessage : class, new()
    {
        private readonly IEventAggregator _eventAggregator;

        protected PublishMessageStepBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            _eventAggregator.PublishOnUIThread(GetMessageToSend());
            return new ExecuteResult(SimpleStepStateEnum.Successful);
        }

        protected virtual TMessage GetMessageToSend()
        {
            return new TMessage();
        }
    }
}