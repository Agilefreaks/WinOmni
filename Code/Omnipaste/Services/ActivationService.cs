namespace Omnipaste.Services
{
    using System;
    using System.Reactive.Linq;
    using BugFreak;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.Transitions;

    public class ActivationService : IActivationService
    {
        #region Fields

        private readonly IStepFactory _stepFactory;

        private readonly ActivationSequence _activationSequence;

        #endregion

        #region Constructors and Destructors

        public ActivationService(IStepFactory stepFactory, IActivationSequenceProvider activationSequenceProvider)
        {
            _stepFactory = stepFactory;
            _activationSequence = activationSequenceProvider.Get();
        }

        #endregion

        #region Public Properties

        public IActivationStep CurrentStep { get; private set; }

        public bool Success
        {
            get
            {
                return CurrentStep != null && CurrentStep.GetId().GetType() != typeof(Failed);
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<IActivationStep> Run()
        {
            return Observable.Start<IActivationStep>(RunSynchronously);
        }

        #endregion

        #region Methods

        private IActivationStep RunSynchronously()
        {
            IExecuteResult result = null;
            while (MoveToNextStep(result))
            {
                try
                {
                    result = CurrentStep.Execute().Wait();
                }
                catch (Exception exception)
                {
                    ReportingService.Instance.BeginReport(exception);
                    result = new ExecuteResult(SimpleStepStateEnum.Failed, exception);
                }
            }

            return CurrentStep;
        }

        private bool CurrentStepIsIntermediateStep()
        {
            return CurrentStep == null || !_activationSequence.FinalStepIdIds.Contains(CurrentStep.GetId());
        }

        private bool MoveToNextStep(IExecuteResult previousResult)
        {
            if (!CurrentStepIsIntermediateStep()) return false;

            if (CurrentStep == null)
            {
                CurrentStep = _stepFactory.Create(_activationSequence.InitialStepId);                
            }
            else
            {
                var transitionId = new TransitionId(CurrentStep.GetId(), previousResult.State);
                var nextStepType = _activationSequence.Transitions.GetTargetTypeForTransition(transitionId);

                CurrentStep = _stepFactory.Create(nextStepType, previousResult.Data);
            }

            return true;
        }

        #endregion
    }
}