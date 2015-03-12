namespace ContactsTests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Subjects;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    [TestFixture]
    public class ContactsUpdatedHandlerTests
    {
        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IContacts> _mockContactsResource;

        private MoqMockingKernel _mockingKernel;

        private Mock<IUsers> _mockUsers;

        private IContactsUpdatedHandler _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _mockContactsResource = new Mock<IContacts>();
            _mockUsers = new Mock<IUsers> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService>();

            _testScheduler = new TestScheduler();

            _mockingKernel = new MoqMockingKernel();
            _mockingKernel.Bind<IContacts>().ToConstant(_mockContactsResource.Object);
            _mockingKernel.Bind<IConfigurationService>().ToConstant(_mockConfigurationService.Object);
            _mockingKernel.Bind<IContactsUpdatedHandler>().To<ContactsUpdatedHandler>();
            _mockingKernel.Bind<IUsers>().ToConstant(_mockUsers.Object);

            _subject = _mockingKernel.Get<IContactsUpdatedHandler>();
        }

        [Test]
        public void ContactsUpdatedArrve_WeGetContactsBasedOnContactsUpdatedAtDate()
        {
            var userInfo = UserInfo.BeginBuild().WithContactsUpdatedAt(DateTime.Now).Build();
            _mockConfigurationService.Setup(cs => cs.UserInfo).Returns(userInfo);
            var userObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<List<ContactDto>>>(100, Notification.CreateOnNext(new List<ContactDto> { new ContactDto()})));
            _mockContactsResource.Setup(cr => cr.GetUpdates(It.IsAny<DateTime>())).Returns(userObservable);
            var omniMessageObservable = new Subject<OmniMessage>();
            _subject.Start(omniMessageObservable);

            omniMessageObservable.OnNext(new OmniMessage { Type = "contacts_updated" });

            _mockContactsResource.Verify(cr => cr.GetUpdates(userInfo.ContactsUpdatedAt), Times.Once);
        }

        [Test]
        public void OnCompleted_UpdatesTheContactsUpdatedAtPropertyOnUserInfo()
        {
            using (TimeHelper.Freez())
            {
                TimeHelper.UtcNow = DateTime.Now;
                var lastContactsUpdateDate = TimeHelper.UtcNow.AddYears(-1);
                var userInfo = UserInfo.BeginBuild().WithContactsUpdatedAt(lastContactsUpdateDate).Build();
                _mockConfigurationService.Setup(cs => cs.UserInfo).Returns(userInfo);

                var testScheduler = new TestScheduler();
                SchedulerProvider.Default = testScheduler;
                var remoteUser = new User { ContactsUpdatedAt = TimeHelper.UtcNow };
                var userObservable =
                    testScheduler.CreateColdObservable(
                        new Recorded<Notification<User>>(100, Notification.CreateOnNext(remoteUser)));
                _mockUsers.Setup(m => m.Get()).Returns(userObservable);

                _subject.OnCompleted();
                testScheduler.Start(() => userObservable);

                _mockConfigurationService.VerifySet(u => u.UserInfo = It.Is<UserInfo>(ui => ui.ContactsUpdatedAt == TimeHelper.UtcNow));                
            }
        }
    }
}