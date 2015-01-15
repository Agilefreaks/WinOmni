﻿namespace OmnipasteTests.Presenters
{
    using System.Globalization;
    using System.Threading;
    using Clipboard.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using OmniUI.Models;

    [TestFixture]
    public class ActivityPresenterTests
    {
        [Test]
        public void CtorWithClipping_Always_SetsTypeToClipping()
        {
            new ActivityPresenter(new ClippingModel()).Type.Should().Be(ActivityTypeEnum.Clipping);
        }

        [Test]
        public void CtorWithClipping_Always_SetsContentToClippingContent()
        {
            new ActivityPresenter(new ClippingModel { Content = "someContent" }).Content.Should().Be("someContent");
        }

        [Test]
        public void CtorWithClipping_ClippingSourceIsCloud_SetsDeviceToCloud()
        {
            new ActivityPresenter(new ClippingModel { Source = Clipping.ClippingSourceEnum.Cloud }).Device.Should()
                .Be(Resources.FromCloud);
        }

        [Test]
        public void CtorWithClipping_ClippingSourceIsLocal_SetsDeviceToCloud()
        {
            new ActivityPresenter(new ClippingModel { Source = Clipping.ClippingSourceEnum.Local }).Device.Should()
                .Be(Resources.FromLocal);
        }

        [Test]
        public void CtorWithClipping_Always_SetsTheClippingIdInTheExtraData()
        {
            const string Id = "SomeId";
            (new ActivityPresenter(new ClippingModel { UniqueId = Id }).SourceId).Should().Be(Id);
        }

        [Test]
        public void CtorWithCall_Always_SetsTypeToCall()
        {
            var call = new Call();

            new ActivityPresenter(call).Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void CtorWithCall_Always_SetsDeviceToCloud()
        {
            new ActivityPresenter(new Call()).Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void CtorWithCall_TypeIsCall_SetsContentAnEmptyString()
        {
            new ActivityPresenter(new Call { ContactInfo = new ContactInfo { FirstName = "Some", LastName = "Contact" } }).Content
                .Should().Be(string.Empty);
        }

        [Test]
        public void CtorWithCall_Always_SetsContactInfo()
        {
            var call = new Call { ContactInfo = new ContactInfo { FirstName = "Some", LastName = "Name", Phone = "07xxxxxx" } };

            ContactInfo contactInfo = new ActivityPresenter(call).ExtraData.ContactInfo;

            contactInfo.Name.Should().Be("Some Name");
            contactInfo.Phone.Should().Be("07xxxxxx");
        }

        [Test]
        public void CtorWithCall_Always_SetsTheEventUniqueIdInExtraData()
        {
            const string Id = "42";
            (new ActivityPresenter(new Call { UniqueId = Id }).SourceId).Should().Be(Id);
        }

        [Test]
        public void CtorWithMessage_Always_SetsContactInfo()
        {
            var message = new Message { ContactInfo = new ContactInfo { FirstName = "Some", LastName = "Name", Phone = "07xxxxxx" } };

            ContactInfo contactInfo = new ActivityPresenter(message).ExtraData.ContactInfo;

            contactInfo.Name.Should().Be("Some Name");
            contactInfo.Phone.Should().Be("07xxxxxx");
        }

        [Test]
        public void CtorWithMessage_Always_SetsTheEventUniqueIdInExtraData()
        {
            const string Id = "42";
            (new ActivityPresenter(new Message { UniqueId = Id }).SourceId).Should().Be(Id);
        }

        [Test]
        public void CtorWithMessage_Always_SetsDeviceToCloud()
        {
            new ActivityPresenter(new Message()).Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void CtorWithUpdateInfo_Always_SetsExtraDataUpdateInfo()
        {
            var updateInfo = new UpdateInfo { WasInstalled = true, ReleaseLog = "test" };

            var activity = new ActivityPresenter(updateInfo);

            (activity.ExtraData.UpdateInfo as UpdateInfo).Should().Be(updateInfo);
        }
        
        [Test]
        public void ToString_Always_ReturnsAStringContainingTheDeviceName()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
            var clippingModel = new ClippingModel { Source = Clipping.ClippingSourceEnum.Local };

            var subject = new ActivityPresenter(clippingModel);

            subject.ToString().Should().Contain("Din Local Clipping");
        }

        [Test]
        public void ToString_Always_ReturnsAStringContainingTheTypeOfTheActivity()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
            var subject = new ActivityPresenter(new Call());

            subject.ToString().Should().Contain("Apel de la:");
        }
        
        [Test]
        public void ToString_Always_ReturnsAStringContainingTheContentOfTheActivity()
        {
            var subject = new ActivityPresenter(new Call { Content = "someContent" });

            subject.ToString().Should().Contain("someContent");
        }

        [Test]
        public void ToString_Always_ReturnsAStringContainingTheExtraDataOfTheActivity()
        {
            var contactInfo = new ContactInfo
                                          {
                                              FirstName = "someFirstName",
                                              LastName = "someLastName",
                                              Phone = "0987"
                                          };
            var subject = new ActivityPresenter(new Message { ContactInfo = contactInfo });
            
            subject.ToString().Should().Contain("someFirstName someLastName 0987");
        }

        [TearDown]
        public void Teardown()
        {
            TimeHelper.Reset();
        }
    }
}