namespace OmnipasteTests.Services.ActivationServiceData.Transitions
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.Transitions;

    [TestFixture]
    public class TransitionCollectionTests
    {
        private abstract class SourceTransition : IActivationStep
        {
            public DependencyParameter Parameter { get; set; }

            public IObservable<IExecuteResult> InternalExecute()
            {
                throw new System.NotImplementedException();
            }

            public Task<IExecuteResult> ExecuteAsync()
            {
                throw new System.NotImplementedException();
            }

            public object GetId()
            {
                throw new System.NotImplementedException();
            }
        }

        private abstract class SuccessTransition : IActivationStep
        {
            public DependencyParameter Parameter { get; set; }

            public IObservable<IExecuteResult> InternalExecute()
            {
                throw new System.NotImplementedException();
            }

            public Task<IExecuteResult> ExecuteAsync()
            {
                throw new System.NotImplementedException();
            }

            public object GetId()
            {
                throw new System.NotImplementedException();
            }
        }

        private abstract class FailedTransition : IActivationStep
        {
            public DependencyParameter Parameter { get; set; }

            public IObservable<IExecuteResult> InternalExecute()
            {
                throw new System.NotImplementedException();
            }

            public Task<IExecuteResult> ExecuteAsync()
            {
                throw new System.NotImplementedException();
            }

            public object GetId()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void RegisterTransition_Always_AddsAnObjectWithASpecificKey()
        {
            var transitionCollection = TransitionCollection.Builder()
                .RegisterTransition<SourceTransition, SuccessTransition>()
                .Build();

            transitionCollection.GetTargetTypeForTransition(
                GenericTransitionId<SourceTransition>.Create(SimpleStepStateEnum.Successful))
                .Name.Should()
                .Be("SuccessTransition");
        }

        [Test]
        public void RegisterTransition_ForFailSuccess_AddsTwoTargets()
        {
            var transitionCollection = TransitionCollection.Builder()
                .RegisterTransition<SourceTransition, SuccessTransition, FailedTransition>()
                .Build();

            transitionCollection.GetTargetTypeForTransition(
                GenericTransitionId<SourceTransition>.Create(SimpleStepStateEnum.Failed))
                .Should()
                .Be<FailedTransition>();
        }
    }
}