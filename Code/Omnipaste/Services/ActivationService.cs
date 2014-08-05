namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
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
                .RegisterTransition<LoadLocalConfiguration, StartOmniService, GetActivationCodeFromDeploymentUri>()
                .RegisterTransition<GetActivationCodeFromDeploymentUri, GetRemoteConfiguration, GetActivationCodeFromUser>()
                .RegisterTransition<GetActivationCodeFromUser, GetRemoteConfiguration, GetActivationCodeFromUser>()
                .RegisterTransition<GetRemoteConfiguration, SaveConfiguration, GetActivationCodeFromUser>()
                .RegisterTransition<SaveConfiguration, StartOmniService, Failed>()
                .RegisterTransition<StartOmniService, VerifyNumberOfDevices, Failed>()
                .RegisterTransition<VerifyNumberOfDevices, Finished, Failed>()
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

        public IObservable<IActivationStep> Run()
        {
            return Observable.Create<IActivationStep>(
                observer =>
                    {
                        CurrentStep = _stepFactory.Create(typeof(LoadLocalConfiguration));
                        while (CurrentStepIsIntermediateStep())
                        {
                            var activationStep = CurrentStep.Execute().Wait();
                            MoveToNextStep(activationStep);
                        }

                        observer.OnNext(CurrentStep);
                        observer.OnCompleted();

                        return Disposable.Empty;
                    });
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