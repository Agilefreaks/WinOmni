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
                return this._finalStepIdIds;
            }
        }

        public ActivationService(IStepFactory stepFactory)
        {
            this._stepFactory = stepFactory;

            this._finalStepIdIds = new List<object> { typeof(Finished), typeof(Failed) };

            this._transitions = new TransitionCollection();

            this._transitions.RegisterTransition(
                GenericTransitionId<Start>.Create(SingleStateEnum.Successful),
                typeof(GetTokenFromDeploymentUri));

            this._transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromDeploymentUri>.Create(SimpleStepStateEnum.Successful),
                typeof(GetRemoteConfiguration));
            this._transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromDeploymentUri>.Create(SimpleStepStateEnum.Failed),
                typeof(LoadLocalConfiguration));

            this._transitions.RegisterTransition(
                GenericTransitionId<LoadLocalConfiguration>.Create(SimpleStepStateEnum.Successful),
                typeof(Finished));
            this._transitions.RegisterTransition(
                GenericTransitionId<LoadLocalConfiguration>.Create(SimpleStepStateEnum.Failed),
                typeof(GetTokenFromUser));

            this._transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromUser>.Create(SimpleStepStateEnum.Successful),
                typeof(GetRemoteConfiguration));
            this._transitions.RegisterTransition(
                GenericTransitionId<GetTokenFromUser>.Create(SimpleStepStateEnum.Failed),
                typeof(Failed));

            this._transitions.RegisterTransition(
                GenericTransitionId<GetRemoteConfiguration>.Create(GetRemoteConfigurationStepStateEnum.CommunicationFailure),
                typeof(GetRemoteConfiguration));
            this._transitions.RegisterTransition(
                GenericTransitionId<GetRemoteConfiguration>.Create(GetRemoteConfigurationStepStateEnum.Failed),
                typeof(GetTokenFromUser));
            this._transitions.RegisterTransition(
                GenericTransitionId<GetRemoteConfiguration>.Create(GetRemoteConfigurationStepStateEnum.Successful),
                typeof(SaveConfiguration));

            this._transitions.RegisterTransition(
                GenericTransitionId<SaveConfiguration>.Create(SingleStateEnum.Successful),
                typeof(RegisterInOmniSync));

            this._transitions.RegisterTransition(
                GenericTransitionId<RegisterInOmniSync>.Create(SimpleStepStateEnum.Successful),
                typeof(Finished));

            this._transitions.RegisterTransition(
                GenericTransitionId<RegisterInOmniSync>.Create(SimpleStepStateEnum.Failed),
                typeof(Failed));

            this._transitions.RegisterTransition(
                GenericTransitionId<Finished>.Create(SingleStateEnum.Successful),
                typeof(Finished));

            this._transitions.RegisterTransition(
                GenericTransitionId<Failed>.Create(SingleStateEnum.Successful),
                typeof(Failed));
        }

        public void Run()
        {
            while (this.CurrentStepIsIntermediateStep())
            {
                this.MoveToNextStep();
            }
        }

        public void MoveToNextStep()
        {
            this.SetCurrentStep(this.GetNextStep());
        }

        public IActivationStep GetNextStep()
        {
            return this.CurrentStep != null ? this.GetNextStepBasedOnExecuteResult() : new Start();
        }

        protected void SetCurrentStep(IActivationStep step)
        {
            this.CurrentStep = step;
        }

        private IActivationStep GetNextStepBasedOnExecuteResult()
        {
            var result = this.CurrentStep.Execute();
            var transitionId = new TransitionId(this.CurrentStep.GetId(), result.State);
            var nextStepType = this._transitions.GetTargetTypeForTransition(transitionId);

            return this._stepFactory.Create(nextStepType, result.Data);
        }

        private bool CurrentStepIsIntermediateStep()
        {
            return this.CurrentStep == null || !this._finalStepIdIds.Contains(this.CurrentStep.GetId());
        }
    }
}