namespace OmnipasteTests.Models
{
    using System;
    using System.Globalization;
    using Clipboard.Models;
    using Events.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using OmniUI.Models;

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
        public void CtorWithClipping_Always_SetsTheClippingIdInTheExtraData()
        {
            const string Id = "SomeId";
            (new Activity(new Clipping { UniqueId = Id }).SourceId).Should().Be(Id);
        }

        [Test]
        public void CtorWithEvent_EventHasTypeIncomingCall_SetsTypeToCall()
        {
            var @event = new Event { Type = EventTypeEnum.IncomingCallEvent };

            new Activity(@event).Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void CtorWithEvent_Always_SetsDeviceToCloud()
        {
            new Activity(new Event()).Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void CtorWithEvent_TypeIsCall_SetsContentAnEmptyString()
        {
            new Activity(new Event { Type = EventTypeEnum.IncomingCallEvent, ContactName = "Some Contact" }).Content
                .Should().Be(string.Empty);
        }

        [Test]
        public void CtorWithEvent_Always_SetsContactInfo()
        {
            var @event = new Event { ContactName = "Some Name", PhoneNumber = "07xxxxxx" };

            ContactInfo contactInfo = new Activity(@event).ExtraData.ContactInfo;

            contactInfo.Name.Should().Be("Some Name");
            contactInfo.Phone.Should().Be("07xxxxxx");
        }

        [Test]
        public void CtorWithEvent_Always_SetsTheEventUniqueIdInExtraData()
        {
            const string Id = "42";
            (new Activity(new Event { UniqueId = Id }).SourceId).Should().Be(Id);
        }

        [Test]
        public void CtorWithUpdateInfo_Always_SetsExtraDataUpdateInfo()
        {
            var updateInfo = new UpdateInfo { WasInstalled = true, ReleaseLog = "test" };

            var activity = new Activity(updateInfo);

            (activity.ExtraData.UpdateInfo as UpdateInfo).Should().Be(updateInfo);
        }
        
        [Test]
        public void ToString_Always_ReturnsAStringContainingTheDeviceName()
        {
            _subject.Device = "some device";

            _subject.ToString().Should().Contain("some device");
        }

        [Test]
        public void ToString_Always_ReturnsAStringContainingTheTypeOfTheActivity()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
            _subject.Type = ActivityTypeEnum.Call;

            _subject.ToString().Should().Contain("Apel de la:");
        }
        
        [Test]
        public void ToString_Always_ReturnsAStringContainingTheContentOfTheActivity()
        {
            _subject.Content = "someContent";

            _subject.ToString().Should().Contain("someContent");
        }

        [Test]
        public void ToString_Always_ReturnsAStringContainingTheExtraDataOfTheActivity()
        {
            _subject.ExtraData.SomeData = new ContactInfo
                                                 {
                                                     FirstName = "someFirstName",
                                                     LastName = "someLastName",
                                                     Phone = "0987"
                                                 };

            _subject.ToString().Should().Contain("someFirstName someLastName 0987");
        }

        [TearDown]
        public void Teardown()
        {
            TimeHelper.Reset();
        }
    }
}