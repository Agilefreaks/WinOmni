﻿namespace OmnipasteTests.SMSComposer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Entities;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using Omnipaste.SMSComposer;
    using SMS.Dto;
    using SMS.Resources.v1;

    [TestFixture]
    public class SMSComposerViewModelTests
    {
        private ISMSComposerViewModel _subject;

        private Mock<ISMSMessages> _mockSMSMessages;

        private TestScheduler _testScheduler;

        private Mock<ISmsMessageFactory> _mockSmsMessageFactory;

        private Mock<IConfigurationService> _mockConfigurationService;

        private ContactEntity _contactEntity;

        [SetUp]
        public void Setup()
        {
            _mockSMSMessages = new Mock<ISMSMessages>();
            _mockSmsMessageFactory = new Mock<ISmsMessageFactory>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _contactEntity = new ContactEntity
                               {
                                   PhoneNumbers = new List<PhoneNumber> { new PhoneNumber { Number = "1234" } },
                                   FirstName = "F",
                                   LastName = "L"
                               };
            _subject = new SMSComposerViewModel(_mockSMSMessages.Object, _mockConfigurationService.Object)
                           {
                               Recipients = new ObservableCollection<ContactInfoPresenter> { new ContactInfoPresenter(_contactEntity) },
                               SmsMessageFactory = _mockSmsMessageFactory.Object
                           };
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Activate_WhenIsSMSSuffixEnabledIsFalse_PopulatesMessageWithEmptyString()
        {
            _mockConfigurationService.SetupGet(x => x.IsSMSSuffixEnabled).Returns(false);

            _subject.Activate();

            _subject.Message.Should().Be(string.Empty);
        }

        [Test]
        public void Activate_WhenIsSMSSuffixEnabledIsTrue_PopulatesMessageWithTheOmnipasteBranding()
        {
            _mockConfigurationService.SetupGet(x => x.IsSMSSuffixEnabled).Returns(true);

            _subject.Activate();

            _subject.Message.Should().Be(Environment.NewLine + Resources.SentFromOmnipaste);
        }

        [Test]
        public void Send_Always_SetsTheModelToANewSMSMessageAfterSending()
        {
            var sendObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<SmsMessageDto>>(100, Notification.CreateOnNext(new SmsMessageDto())),
                new Recorded<Notification<SmsMessageDto>>(200, Notification.CreateOnCompleted<SmsMessageDto>()));
            var createObservable = _testScheduler.CreateColdObservable(
                    new Recorded<Notification<LocalSmsMessageEntity>>(100, Notification.CreateOnNext(new LocalSmsMessageEntity())));
            _subject.Message = "test";
            _mockSMSMessages.Setup(x => x.Send(new List<string>{"1234"}, "test")).Returns(sendObservable);
            _mockSmsMessageFactory.Setup(x => x.Create<LocalSmsMessageEntity>(It.IsAny<SmsMessageDto>())).Returns(createObservable);

            _subject.Send();
            _testScheduler.Start();

            _mockSMSMessages.Verify(x => x.Send(new List<string>{"1234"}, "test"), Times.Once());
            _subject.Message.Should().Be("");
        }

        [Test]
        public void Send_Always_CallsSmsFactoryCreateWithLocalSmsForEachRecepient()
        {
            const string Content = "Test";
            var sendObservable = Observable.Return(new SmsMessageDto { Content = Content }, _testScheduler);
            _mockSMSMessages.Setup(x => x.Send(It.IsAny<List<string>>(), It.IsAny<string>())).Returns(sendObservable);
            _mockSmsMessageFactory.Setup(x => x.Create<LocalSmsMessageEntity>(It.IsAny<SmsMessageDto>())).Returns(Observable.Empty<LocalSmsMessageEntity>());
            _subject.Message = Content;
            _subject.Recipients.Add(new ContactInfoPresenter(new ContactEntity()));
            _subject.Recipients.Add(new ContactInfoPresenter(new ContactEntity()));

            _subject.Send();
            _testScheduler.Start();

            _mockSmsMessageFactory.Verify(x => x.Create<LocalSmsMessageEntity>(It.Is<SmsMessageDto>(m => m.Content == Content)), Times.Exactly(3));
        }
    }
}