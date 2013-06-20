namespace OmniCommon.Services
{
    using OmniCommon.Interfaces;
    using OmniCommon.Services.ActivationServiceData;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;
    using OmniCommon.Services.ActivationServiceData.Transitions;

    public class ActivationService : IActivationService
    {
        public IActivationStep CurrentStep { get; private set; }

        public IStepFactory StepFactory { get; set; }

        private readonly TransitionCollection _transitions;

        public ActivationService()
        {
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
        }

        public void Initialize()
        {
            SetCurrentStep(StepFactory.Create(typeof(Start)));
        }

        public void MoveToNextStep()
        {
            var result = CurrentStep.Execute();
            var transitionKey = new TransitionId(CurrentStep.GetId(), result.State);
            var nextStepType = _transitions.GetTargetTypeForTransition(transitionKey);
            CurrentStep = StepFactory.Create(nextStepType);
        }

        protected void SetCurrentStep(IActivationStep step)
        {
            CurrentStep = step;
        }
    }
}