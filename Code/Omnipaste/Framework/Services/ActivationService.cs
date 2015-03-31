namespace Omnipaste.Framework.Services
{
    using System;
    using System.Reactive.Linq;
    using OmniCommon;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Services.ActivationServiceData;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Framework.Services.ActivationServiceData.Transitions;

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
            CurrentStep = null;
            while (MoveToNextStep(result))
            {
                try
                {
                    SimpleLogger.Log("Starting step: " + CurrentStep.GetType());
                    result = CurrentStep.Execute().Wait();
                    SimpleLogger.Log("Finished step: " + CurrentStep.GetType());
                }
                catch (Exception exception)
                {
                    SimpleLogger.Log("Step finished with exception: " + exception);
                    ExceptionReporter.Instance.Report(exception);
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
                CurrentStep = GetStepById(_activationSequence.InitialStepId);
            }
            else
            {
                var transitionId = new TransitionId(CurrentStep.GetId(), previousResult.State);
                var nextStepType = _activationSequence.Transitions.GetTargetTypeForTransition(transitionId);

                CurrentStep = GetStepById(nextStepType, previousResult.Data);
            }

            return true;
        }

        private IActivationStep GetStepById(Type initialStepId, object payload = null)
        {
            return _stepFactory.Create(initialStepId, payload);
        }

        #endregion
    }
}