namespace OmnipasteTests.Services
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.Transitions;

    [TestFixture]
    public class ActivationServiceTests
    {
        private MoqMockingKernel _mockingKernel;

        private IStepFactory _stepFactory;

        private IActivationService _subject;

        private Mock<IActivationSequenceProvider> _mockActivationSequenceProvider;

        private Mock<IStep1> _mockStep1;

        private Mock<IStep2> _mockStep2;

        private ActivationSequence _activationSequence;

        private Mock<IStep3> _mockStep3;

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockingKernel.Bind<IStepFactory>().To<StepFactory>();
            _stepFactory = _mockingKernel.Get<IStepFactory>();

            _mockActivationSequenceProvider = new Mock<IActivationSequenceProvider>();
            _mockingKernel.Bind<IActivationSequenceProvider>().ToConstant(_mockActivationSequenceProvider.Object);

            _mockingKernel.Bind<IActivationService>().To<ActivationService>();

            _activationSequence = SetupMockActivationSequence();
            _mockActivationSequenceProvider.Setup(x => x.Get()).Returns(_activationSequence);

            _subject = _mockingKernel.Get<IActivationService>();
        }

        [Test]
        public void Success_WithCurrentStepNull_WillReturnFalse()
        {
            _subject.Success.Should().BeFalse();
        }

        [Test]
        public void Run_CurrentActivationStepTerminatesWithAnError_MovesToTheOnFailedRegisteredStep()
        {
            var observable = Observable.Create<IExecuteResult>(
                observer =>
                {
                    observer.OnError(new Exception("test"));
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
            _mockStep1.Setup(x => x.Execute()).Returns(observable);

            _subject.Run().Wait();

            _subject.CurrentStep.Should().Be(_mockStep3.Object);
        }

        [Test]
        public void Run_CurrentActivationStepTerminatesWithASuccessExecuteResult_MovesToTheOnSuccessRegisteredStep()
        {
            var observable = Observable.Create<IExecuteResult>(
                observer =>
                {
                    observer.OnNext(new ExecuteResult(SimpleStepStateEnum.Successful));
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
            _mockStep1.Setup(x => x.Execute()).Returns(observable);

            _subject.Run().Wait();

            _subject.CurrentStep.Should().Be(_mockStep2.Object);
        }

        [Test]
        public void Run_CurrentActivationStepTerminatesWithAFailedExecuteResult_MovesToTheOnFailedRegisteredStep()
        {
            var observable = Observable.Create<IExecuteResult>(
                observer =>
                {
                    observer.OnNext(new ExecuteResult(SimpleStepStateEnum.Failed));
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
            _mockStep1.Setup(x => x.Execute()).Returns(observable);

            _subject.Run().Wait();

            _subject.CurrentStep.Should().Be(_mockStep3.Object);
        }

        private ActivationSequence SetupMockActivationSequence()
        {
            _mockStep1 = CreateMockStep<IStep1>();
            _mockStep2 = CreateMockStep<IStep2>();
            _mockStep3 = CreateMockStep<IStep3>();
            var activationSequence = new ActivationSequence { InitialStepId = typeof(IStep1) };
            activationSequence.FinalStepIdIds.Add(typeof(IStep2));
            activationSequence.FinalStepIdIds.Add(typeof(IStep3));
            activationSequence.Transitions =
                TransitionCollection.Builder().RegisterTransition<IStep1, IStep2, IStep3>().Build();

            return activationSequence;
        }

        private Mock<T> CreateMockStep<T>() where T : class, IActivationStep
        {
            var mockStep = new Mock<T>();
            _mockingKernel.Bind<T>().ToConstant(mockStep.Object);
            mockStep.Setup(x => x.GetId()).Returns(typeof(T));
            var executeResult = new ExecuteResult { State = SimpleStepStateEnum.Successful };
            var executeObservable = Observable.Create<IExecuteResult>(
                observer =>
                {
                    observer.OnNext(executeResult);
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
            mockStep.Setup(x => x.Execute()).Returns(executeObservable);
            return mockStep;
        }

        public interface IStep1 : IActivationStep
        {
        }

        public interface IStep2 : IActivationStep
        {
        }

        public interface IStep3 : IActivationStep
        {
        }
    }
}