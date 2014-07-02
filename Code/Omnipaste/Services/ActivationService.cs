namespace Omnipaste.Services
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.Transitions;

    public class ActivationService : IActivationService
    {
        #region Fields

        private readonly List<object> _finalStepIdIds;

        private readonly IStepFactory _stepFactory;

        private readonly TransitionCollection _transitions;

        #endregion

        #region Constructors and Destructors

        public ActivationService(IStepFactory stepFactory)
        {
            _stepFactory = stepFactory;

            _finalStepIdIds = new List<object> { typeof(Finished), typeof(Failed) };

            _transitions = TransitionCollection.Builder()
                .RegisterTransition<LoadLocalConfiguration, Finished, GetActivationCodeFromDeploymentUri>()
                .RegisterTransition<GetActivationCodeFromDeploymentUri, GetRemoteConfiguration, GetActivationCodeFromUser>()
                .RegisterTransition<GetActivationCodeFromUser, GetRemoteConfiguration, GetActivationCodeFromUser>()
                .RegisterTransition<GetRemoteConfiguration, Finished, Failed>()
                .Build();
        }

        #endregion

        #region Public Properties

        public IActivationStep CurrentStep { get; private set; }

        public TransitionCollection Transitions
        {
            get
            {
                return _transitions;
            }
        }

        public bool Success
        {
            get
            {
                return CurrentStep != null && CurrentStep.GetId().GetType() != typeof(Failed);
            }
        }

        #endregion

        #region Public Methods and Operators

        public async Task Run()
        {
            CurrentStep = _stepFactory.Create(typeof(LoadLocalConfiguration));
            while (CurrentStepIsIntermediateStep())
            {
                var activationStep = await CurrentStep.ExecuteAsync();

                MoveToNextStep(activationStep);
            }
        }

        #endregion

        #region Methods

        private bool CurrentStepIsIntermediateStep()
        {
            return CurrentStep == null || !_finalStepIdIds.Contains(CurrentStep.GetId());
        }

        private void MoveToNextStep(IExecuteResult previousResult)
        {
            var transitionId = new TransitionId(CurrentStep.GetId(), previousResult.State);
            var nextStepType = _transitions.GetTargetTypeForTransition(transitionId);

            CurrentStep = _stepFactory.Create(nextStepType, previousResult.Data);
        }

        #endregion
    }
}