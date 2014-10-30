namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Subjects;
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class AndroidInstallGuide : ActivationStepBase, IHandle<AndroidInstallationCompleteMessage>
    {
        #region Fields

        private readonly Subject<IExecuteResult> _synchronizationSubject;

        #endregion

        #region Constructors and Destructors

        public AndroidInstallGuide(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
            _synchronizationSubject = new Subject<IExecuteResult>();
        }

        #endregion

        #region Public Properties

        public IEventAggregator EventAggregator { get; set; }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            EventAggregator.PublishOnCurrentThread(new ShowAndroidInstallGuideMessage());

            return _synchronizationSubject;
        }

        #endregion

        public void Handle(AndroidInstallationCompleteMessage message)
        {
            _synchronizationSubject.OnNext(new ExecuteResult(SimpleStepStateEnum.Successful));
            _synchronizationSubject.OnCompleted();
        }
    }
}