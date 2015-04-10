namespace OmnipasteTests.Services.Providers
{
    using System.Linq;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class MergedConversationContextTests
    {
        private TestScheduler _testScheduler;

        private MergedConversationContext _subject;

        private Mock<ISmsMessageRepository> _mockMessageRepository;

        private Mock<IPhoneCallRepository> _mockCallRepository;

        private Mock<IPhoneCallModelFactory> _mockPhoneCallModelFactory;

        private Mock<ISmsMessageModelFactory> _mockSmsMessageModelFactory;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockMessageRepository = new Mock<ISmsMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<IPhoneCallRepository> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCallModelFactory = new Mock<IPhoneCallModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockSmsMessageModelFactory = new Mock<ISmsMessageModelFactory> { DefaultValue = DefaultValue.Mock };
            _subject = new MergedConversationContext(_mockMessageRepository.Object, _mockCallRepository.Object, _mockPhoneCallModelFactory.Object, _mockSmsMessageModelFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void GetItems_Always_ReturnsAnEmptyObserver()
        {
            var testObserver = _testScheduler.Start(() => _subject.GetItems());

            testObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}
