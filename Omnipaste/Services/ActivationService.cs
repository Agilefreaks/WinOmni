namespace Omnipaste.Services
{
    using System.Collections.Generic;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.Transitions;

    public class ActivationService : IActivationService
    {
        private readonly List<object> _finalStepIdIds;
        private readonly TransitionCollection _transitions;
        private readonly IStepFactory _stepFactory;

        public IActivationStep CurrentStep { get; private set; }

        public IEnumerable<object> FinalStepIds
        {
            get
            {
                return _finalStepIdIds;
            }
        }

        public ActivationService(IStepFactory stepFactory)
        {
            _stepFactory = stepFactory;

            _finalStepIdIds = new List<object> { typeof(Finished), typeof(Failed) };

            _transitions = new TransitionCollection();

            _transitions.RegisterTransition(
                GenericTransitionId<Start>.Create(SingleStateEnum.Successful),
                typeof(GetTokenFromDeploymentUri));

            _transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromDeploymentUri>.Create(SimpleStepStateEnum.Successful),
                typeof(GetRemoteConfiguration));
            _transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromDeploymentUri>.Create(SimpleStepStateEnum.Failed),
                typeof(LoadLocalConfiguration));

            _transitions.RegisterTransition(
                GenericTransitionId<LoadLocalConfiguration>.Create(SimpleStepStateEnum.Successful),
                typeof(Finished));
            _transitions.RegisterTransition(
                GenericTransitionId<LoadLocalConfiguration>.Create(SimpleStepStateEnum.Failed),
                typeof(GetTokenFromUser));

            _transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromUser>.Create(SimpleStepStateEnum.Successful),
                typeof(GetRemoteConfiguration));
            _transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromUser>.Create(SimpleStepStateEnum.Failed),
                typeof(Failed));

            _transitions.RegisterTransition(
                GenericTransitionId<GetRemoteConfiguration>.Create(GetRemoteConfigurationStepStateEnum.CommunicationFailure),
                typeof(GetRemoteConfiguration));
            _transitions.RegisterTransition(
                GenericTransitionId<GetRemoteConfiguration>.Create(GetRemoteConfigurationStepStateEnum.Failed),
                typeof(GetTokenFromUser));
            _transitions.RegisterTransition(
                GenericTransitionId<GetRemoteConfiguration>.Create(GetRemoteConfigurationStepStateEnum.Successful),
                typeof(SaveConfiguration));

            _transitions.RegisterTransition(
                GenericTransitionId<SaveConfiguration>.Create(SingleStateEnum.Successful),
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
            var transitionId = new TransitionId(CurrentStep.GetId(), result.State);
            var nextStepType = _transitions.GetTargetTypeForTransition(transitionId);

            return _stepFactory.Create(nextStepType, result.Data);
        }

        private bool CurrentStepIsIntermediateStep()
        {
            return CurrentStep == null || !_finalStepIdIds.Contains(CurrentStep.GetId());
        }
    }
}