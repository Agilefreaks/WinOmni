namespace OmniDebugTests
{
    using System;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniDebug.Services;
    using PhoneCalls.Dto;
    using PhoneCalls.Resources.v1;

    [TestFixture]
    public class PhoneCallsWrapperTests
    {
        private PhoneCallsWrapper _subject;

        private Mock<IPhoneCalls> _mockPhoneCalls;

        [SetUp]
        public void Setup()
        {
            _mockPhoneCalls = new Mock<IPhoneCalls>();
            _subject = new PhoneCallsWrapper(_mockPhoneCalls.Object);
        }

        [Test]
        public void GetWithId_AfterFirstCallingMockGet_ReturnsTheGivenPhoneCallObject()
        {
            var mockObserver = new Mock<IObserver<PhoneCallDto>>();
            var phoneCall = new PhoneCallDto { Id = "42" };

            _subject.MockGet(phoneCall.Id, phoneCall);
            _subject.Get(phoneCall.Id).Subscribe(mockObserver.Object);

            mockObserver.Verify(x => x.OnNext(phoneCall), Times.Once);
        }

        [Test]
        public void GetWithId_WithoutFirstCallingMockGet_ReturnsTheObservableFromTheGivenPhoneCallLObject()
        {
            const string Id = "42";
            var mockObservable = new Mock<IObservable<PhoneCallDto>>();
            _mockPhoneCalls.Setup(x => x.Get(Id)).Returns(mockObservable.Object);

            _subject.Get(Id).Should().Be(mockObservable.Object);
        }
    }
}
