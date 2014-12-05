namespace OmnipasteTests.Activity
{
    using System;
    using Clipboard.Models;
    using Events.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Activity;
    using Omnipaste.Properties;

    [TestFixture]
    public class ActivityTests
    {
        private Activity _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new Activity();
        }

        [Test]
        public void Type_ByDefault_IsGeneric()
        {
            _subject.Type.Should().Be(ActivityTypeEnum.None);
        }

        [Test]
        public void Content_ByDefault_IsEmptyString()
        {
            _subject.Content.Should().Be(string.Empty);
        }

        [Test]
        public void Ctor_Always_SetsTimeToCurrentTime()
        {
            var currentTime = DateTime.Now;
            TimeHelper.UtcNow = currentTime;

            var activity = new Activity();

            activity.Time.Should().Be(currentTime);
        }

        [Test]
        public void CtorWithClipping_Always_SetsTypeToClipping()
        {
            new Activity(new Clipping()).Type.Should().Be(ActivityTypeEnum.Clipping);
        }

        [Test]
        public void CtorWithClipping_Always_SetsContentToClippingContent()
        {
            new Activity(new Clipping("someContent")).Content.Should().Be("someContent");
        }

        [Test]
        public void CtorWithClipping_ClippingSourceIsCloud_SetsDeviceToCloud()
        {
            new Activity(new Clipping { Source = Clipping.ClippingSourceEnum.Cloud }).Device.Should()
                .Be(Resources.FromCloud);
        }

        [Test]
        public void CtorWithClipping_ClippingSourceIsLocal_SetsDeviceToCloud()
        {
            new Activity(new Clipping { Source = Clipping.ClippingSourceEnum.Local }).Device.Should()
                .Be(Resources.FromLocal);
        }

        [Test]
        public void CtorWithEvent_EventHasTypeIncomingCall_SetsTypeToCall()
        {
            var @event = new Event { Type = EventTypeEnum.IncomingCallEvent};

            new Activity(@event).Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void CtorWithEvent_EventHasTypeIncomingCall_SetsContentToContactNamePluPhoneNumber()
        {
            var eventTime = new DateTime(2000, 1, 1);
            var @event = new Event
                             {
                                 Type = EventTypeEnum.IncomingCallEvent,
                                 ContactName = "test",
                                 PhoneNumber = "123",
                                 Time = eventTime
                             };

            new Activity(@event).Content.Should().Be("IncomingCallEvent at 01.01.2000 0:00:00 from test");
        }

        [Test]
        public void CtorWithEvent_Always_SetsDeviceToCloud()
        {
            var @event = new Event();

            new Activity(@event).Device.Should().Be(Resources.FromCloud);
        }

        [TearDown]
        public void Teardown()
        {
            TimeHelper.Reset();
        }
    }
}