namespace OmniCommon.Services
{
    using System.Collections.Generic;
    using OmniCommon.Interfaces;
    using OmniCommon.Services.ActivationServiceData;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;
    using OmniCommon.Services.ActivationServiceData.Transitions;

    public class ActivationService : IActivationService
    {
        private readonly List<object> _finalStepIdIds;
        private readonly TransitionCollection _transitions;

        public IActivationStep CurrentStep { get; private set; }

        public IStepFactory StepFactory { get; set; }

        public IEnumerable<object> FinalStepIds
        {
            get
            {
                return _finalStepIdIds;
            }
        }

        public ActivationService()
        {
            _finalStepIdIds = new List<object> { typeof(Finished), typeof(Failed) };

            _transitions = new TransitionCollection();

            _transitions.RegisterTransition(
                GenericTransitionId<Start>.Create(SingleStateEnum.Successful),
                typeof(GetTokenFromActivationData));

            _transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromActivationData>.Create(SimpleStepStateEnum.Successful),
                typeof(GetConfiguration));
            _transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromActivationData>.Create(SimpleStepStateEnum.Failed),
                typeof(LoadLocalConfiguration));

            _transitions.RegisterTransition(
                GenericTransitionId<LoadLocalConfiguration>.Create(SimpleStepStateEnum.Successful),
                typeof(Finished));
            _transitions.RegisterTransition(
                GenericTransitionId<LoadLocalConfiguration>.Create(SimpleStepStateEnum.Failed),
                typeof(GetTokenFromUser));

            _transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromUser>.Create(SimpleStepStateEnum.Successful),
                typeof(GetConfiguration));
            _transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromUser>.Create(SimpleStepStateEnum.Failed),
                typeof(Failed));

            _transitions.RegisterTransition(
                GenericTransitionId<GetConfiguration>.Create(GetConfigurationStepStateEnum.TimedOut),
                typeof(GetConfiguration));
            _transitions.RegisterTransition(
                GenericTransitionId<GetConfiguration>.Create(GetConfigurationStepStateEnum.Failed),
                typeof(GetTokenFromUser));
            _transitions.RegisterTransition(
                GenericTransitionId<GetConfiguration>.Create(GetConfigurationStepStateEnum.Successful),
                typeof(Finished));

            _transitions.RegisterTransition(
                GenericTransitionId<Finished>.Create(SingleStateEnum.Successful),
                typeof(Finished));

            _transitions.RegisterTransition(
                GenericTransitionId<Failed>.Create(SingleStateEnum.Successful),
                typeof(Failed));
        }

        public void Run()
        {
            while (CurrentStepIsIntermediateStep())
            {
                MoveToNextStep();
            }
        }

        public void MoveToNextStep()
        {
            SetCurrentStep(GetNextStep());
        }

        public IActivationStep GetNextStep()
        {
            return CurrentStep != null ? GetNextStepBasedOnExecuteResult() : new Start();
        }

        protected void SetCurrentStep(IActivationStep step)
        {
            CurrentStep = step;
        }

        private IActivationStep GetNextStepBasedOnExecuteResult()
        {
            var result = CurrentStep.Execute();
            var transitionKey = new TransitionId(CurrentStep.GetId(), result.State);
            var nextStepType = _transitions.GetTargetTypeForTransition(transitionKey);

            return StepFactory.Create(nextStepType);
        }

        private bool CurrentStepIsIntermediateStep()
        {
            return CurrentStep == null || !_finalStepIdIds.Contains(CurrentStep.GetId());
        }
    }
}