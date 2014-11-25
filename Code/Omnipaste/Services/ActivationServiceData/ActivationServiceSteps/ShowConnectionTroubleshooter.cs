namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public class ShowConnectionTroubleshooter : ActivationStepBase
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructors and Destructors

        public ShowConnectionTroubleshooter(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            _eventAggregator.PublishOnCurrentThread(new ShowConnectionTroubleshooterMessage());

            return Observable.Never<IExecuteResult>();
        }

        #endregion
    }
}