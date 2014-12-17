namespace OmnipasteTests.Services.Commands
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Services.Commands;

    [TestFixture]
    public class CommandServiceTests
    {
        private CommandService _subject;
        private Mock<ICommandProcessor<TestCommand, TestCommandResult>> _mockCommandProcessor;
        private MoqMockingKernel _mockKernel;

        public class TestCommand
        {
        }

        public class TestCommandResult
        {
        }

        [SetUp]
        public void SetUp()
        {
            _mockKernel = new MoqMockingKernel();
            _mockCommandProcessor = new Mock<ICommandProcessor<TestCommand, TestCommandResult>>();
            _mockKernel.Bind<ICommandProcessor<TestCommand, TestCommandResult>>().ToConstant(_mockCommandProcessor.Object);

            _subject = new CommandService(_mockKernel);
        }

        [Test]
        public void Execute_WhenCommandExecutesSuccessful_CallsOnNext()
        {
            var testScheduler = new TestScheduler();
            var testCommand = new TestCommand();
            var testCommandResult = new TestCommandResult();
            var testObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<TestCommandResult>>(100,
                        Notification.CreateOnNext(testCommandResult)),
                    new Recorded<Notification<TestCommandResult>>(200,
                        Notification.CreateOnCompleted<TestCommandResult>()));
            _mockCommandProcessor.Setup(m => m.Process(It.IsAny<TestCommand>())).Returns(testObservable);
            
            var observer = testScheduler.Start(
                () => _subject.Execute<TestCommand, TestCommandResult>(testCommand));

            observer.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
        }

        [Test]
        public void Execute_WhenCommandThrowsException_CallsOnError()
        {
            var testScheduler = new TestScheduler();
            var testCommand = new TestCommand();
            _mockCommandProcessor.Setup(m => m.Process(It.IsAny<TestCommand>())).Throws(new Exception("42"));

            var observer = testScheduler.Start(
                () => _subject.Execute<TestCommand, TestCommandResult>(testCommand));

            observer.Messages[0].Value.Kind.Should().Be(NotificationKind.OnError);
        }
    }
}
