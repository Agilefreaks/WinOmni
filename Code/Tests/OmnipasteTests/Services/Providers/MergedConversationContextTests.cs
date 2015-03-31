namespace OmnipasteTests.Services.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using FluentAssertions.Events;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using OmnipasteTests.Helpers;

    [TestFixture]
    public class MergedConversationContextTests
    {
        private TestScheduler _testScheduler;

        private MergedConversationContext _subject;

        private Mock<ISmsMessageRepository> _mockMessageRepository;

        private Mock<IPhoneCallRepository> _mockCallRepository;

        private Mock<IPhoneCallPresenterFactory> _mockPhoneCalllPresenterFactory;

        private Mock<ISmsMessagePresenterFactory> _mockSmsMessagePresenterFactory;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockMessageRepository = new Mock<ISmsMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<IPhoneCallRepository> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCalllPresenterFactory = new Mock<IPhoneCallPresenterFactory> { DefaultValue = DefaultValue.Mock };
            _mockSmsMessagePresenterFactory = new Mock<ISmsMessagePresenterFactory> { DefaultValue = DefaultValue.Mock };
            _subject = new MergedConversationContext(_mockMessageRepository.Object, _mockCallRepository.Object, _mockPhoneCalllPresenterFactory.Object, _mockSmsMessagePresenterFactory.Object);
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
