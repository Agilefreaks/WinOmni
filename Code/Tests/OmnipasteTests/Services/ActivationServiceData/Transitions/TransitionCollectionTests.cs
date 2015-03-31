namespace OmnipasteTests.Services.ActivationServiceData.Transitions
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Services.ActivationServiceData;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Framework.Services.ActivationServiceData.Transitions;

    [TestFixture]
    public class TransitionCollectionTests
    {
        private abstract class SourceTransition : IActivationStep
        {
            public DependencyParameter Parameter { get; set; }
            
            public IObservable<IExecuteResult> Execute()
            {
                throw new NotImplementedException();
            }

            public object GetId()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class SuccessTransition : IActivationStep
        {
            public DependencyParameter Parameter { get; set; }

            public IObservable<IExecuteResult> Execute()
            {
                throw new NotImplementedException();
            }

            public object GetId()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class FailedTransition : IActivationStep
        {
            public DependencyParameter Parameter { get; set; }

            public IObservable<IExecuteResult> Execute()
            {
                throw new NotImplementedException();
            }

            public object GetId()
            {
                throw new NotImplementedException();
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