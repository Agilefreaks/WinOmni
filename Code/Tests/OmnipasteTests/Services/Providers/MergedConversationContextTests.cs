namespace OmnipasteTests.Services.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class MergedConversationContextTests
    {
        private TestScheduler _testScheduler;

        private MergedConversationContext _subject;

        private Mock<IMessageRepository> _mockMessageRepository;

        private Mock<ICallRepository> _mockCallRepository;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new MergedConversationContext(_mockMessageRepository.Object, _mockCallRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void GetItems_Always_ReturnsMessagesAndCalls()
        {
            var message = new SmsMessage();
            _mockMessageRepository.Setup(m => m.GetAll()).Returns(Observable.Return(new List<SmsMessage> { message }, _testScheduler));
            var call = new Call();
            _mockCallRepository.Setup(m => m.GetAll()).Returns(Observable.Return(new List<Call> { call }));

            var testObserver = _testScheduler.Start(() => _subject.GetItems());

            testObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            testObserver.Messages.First().Value.Value.Should().Contain(message);
            testObserver.Messages.First().Value.Value.Should().Contain(call);
        }
    }
}
