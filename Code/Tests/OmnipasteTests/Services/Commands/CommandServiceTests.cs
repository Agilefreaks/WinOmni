namespace OmnipasteTests.Services.Commands
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Framework.Services.Commands;
    using OmniUI.Framework.Commands;

    [TestFixture]
    public class CommandServiceTests
    {
        private CommandService _subject;

        private MoqMockingKernel _mockKernel;

        private TestScheduler _testScheduler;

        private Mock<ICommandDependency> _mockCommandDependency;

        public class TestResult
        {
        }

        public interface ICommandDependency
        {
        }

        public class TestCommand : IObservableCommand<TestResult>
        {
            private readonly IObservable<TestResult> _result;

            [Inject]
            public ICommandDependency CommandDependency { get; set; }

            public TestCommand()
            {
            }

            public TestCommand(IObservable<TestResult> result)
            {
                _result = result;
            }

            public IObservable<TestResult> Execute()
            {
                return _result;
            }
        }

        [SetUp]
        public void SetUp()
        {
            _mockKernel = new MoqMockingKernel();
            _mockCommandDependency = new Mock<ICommandDependency>();
            _mockKernel.Bind<ICommandDependency>().ToConstant(_mockCommandDependency.Object);
            _testScheduler = new TestScheduler();

            _subject = new CommandService(_mockKernel);
        }

        [Test]
        public void Execute_WhenCommandExecutesSuccessful_CallsOnNext()
        {
            var testCommandResult = new TestResult();
            var testObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<TestResult>>(100,
                        Notification.CreateOnNext(testCommandResult)),
                    new Recorded<Notification<TestResult>>(200,
                        Notification.CreateOnCompleted<TestResult>()));
            var testCommand = new TestCommand(testObservable);
            
            var observer = _testScheduler.Start(() => _subject.Execute(testCommand));
            
            observer.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
        }

        [Test]
        public void Execute_WhenCommandThrowsException_CallsOnError()
        {
            var testCommand = new TestCommand(Observable.Throw<TestResult>(new Exception("42")));

            var observer = _testScheduler.Start(() => _subject.Execute(testCommand));

            observer.Messages[0].Value.Kind.Should().Be(NotificationKind.OnError);
        }

        [Test]
        public void Execute_Always_InjectsDependencies()
        {
            var testCommand = new TestCommand(Observable.Throw<TestResult>(new Exception("42")));

            _testScheduler.Start(() => _subject.Execute(testCommand));

            testCommand.CommandDependency.Should().Be(_mockCommandDependency.Object);
        }
    }
}
