namespace OmnipasteTests.Models
{
    using System;
    using System.Globalization;
    using System.Threading;
    using Clipboard.Models;
    using FluentAssertions;
    using NUnit.Framework;
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
        public void CtorWithClipping_Always_SetsTypeToClipping()
        {
            new Activity(new ClippingModel()).Type.Should().Be(ActivityTypeEnum.Clipping);
        }

        [Test]
        public void CtorWithClipping_Always_SetsContentToClippingContent()
        {
            new Activity(new ClippingModel { Content = "someContent" }).Content.Should().Be("someContent");
        }

        [Test]
        public void CtorWithClipping_ClippingSourceIsCloud_SetsDeviceToCloud()
        {
            new Activity(new ClippingModel { Source = Clipping.ClippingSourceEnum.Cloud }).Device.Should()
                .Be(Resources.FromCloud);
        }

        [Test]
        public void CtorWithClipping_ClippingSourceIsLocal_SetsDeviceToCloud()
        {
            new Activity(new ClippingModel { Source = Clipping.ClippingSourceEnum.Local }).Device.Should()
                .Be(Resources.FromLocal);
        }

        [Test]
        public void CtorWithClipping_Always_SetsTheClippingIdInTheExtraData()
        {
            const string Id = "SomeId";
            (new Activity(new ClippingModel { UniqueId = Id }).SourceId).Should().Be(Id);
        }

        [Test]
        public void CtorWithCall_Always_SetsTypeToCall()
        {
            var call = new Call();

            new Activity(call).Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void CtorWithCall_Always_SetsDeviceToCloud()
        {
            new Activity(new Call()).Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void CtorWithCall_TypeIsCall_SetsContentAnEmptyString()
        {
            new Activity(new Call { ContactInfo = new ContactInfo { FirstName = "Some", LastName = "Contact" } }).Content
                .Should().Be(string.Empty);
        }

        [Test]
        public void CtorWithCall_Always_SetsContactInfo()
        {
            var call = new Call { ContactInfo = new ContactInfo { FirstName = "Some", LastName = "Name", Phone = "07xxxxxx" } };

            ContactInfo contactInfo = new Activity(call).ExtraData.ContactInfo;

            contactInfo.Name.Should().Be("Some Name");
            contactInfo.Phone.Should().Be("07xxxxxx");
        }

        [Test]
        public void CtorWithCall_Always_SetsTheEventUniqueIdInExtraData()
        {
            const string Id = "42";
            (new Activity(new Call { UniqueId = Id }).SourceId).Should().Be(Id);
        }

        [Test]
        public void CtorWithMessage_Always_SetsContactInfo()
        {
            var message = new Message { ContactInfo = new ContactInfo { FirstName = "Some", LastName = "Name", Phone = "07xxxxxx" } };

            ContactInfo contactInfo = new Activity(message).ExtraData.ContactInfo;

            contactInfo.Name.Should().Be("Some Name");
            contactInfo.Phone.Should().Be("07xxxxxx");
        }

        [Test]
        public void CtorWithMessage_Always_SetsTheEventUniqueIdInExtraData()
        {
            const string Id = "42";
            (new Activity(new Message { UniqueId = Id }).SourceId).Should().Be(Id);
        }

        [Test]
        public void CtorWithMessage_Always_SetsDeviceToCloud()
        {
            new Activity(new Message()).Device.Should().Be(Resources.FromCloud);
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
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
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