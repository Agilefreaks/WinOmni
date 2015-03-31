﻿namespace OmnipasteTests.WorkspaceDetails.Conversation
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Entities;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation;
    using Omnipaste.WorkspaceDetails.Conversation.Message;
    using Omnipaste.WorkspaceDetails.Conversation.PhoneCall;

    [TestFixture]
    public class ConversationContentViewModelTests
    {
        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IConversationContext> _mockConversationContext;

        private Mock<IConversationProvider> _mockConversationProvider;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private ConversationContentViewModel _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            var mockingKernel = new MoqMockingKernel();
            mockingKernel.Bind<IPhoneCallViewModel>()
                .ToMethod(context => new PhoneCallViewModel(_mockUiRefreshService.Object, _mockConfigurationService.Object));
            mockingKernel.Bind<IMessageViewModel>()
                .ToMethod(
                    context => new MessageViewModel(_mockUiRefreshService.Object, _mockConfigurationService.Object));
            _mockConversationProvider = new Mock<IConversationProvider> { DefaultValue = DefaultValue.Mock };
            _mockConversationContext = new Mock<IConversationContext> { DefaultValue = DefaultValue.Mock };
            _mockConversationProvider.Setup(x => x.ForContact(It.IsAny<ContactEntity>()))
                .Returns(_mockConversationContext.Object);
            _mockEventAggregator = new Mock<IEventAggregator> { DefaultValue = DefaultValue.Mock };
            _subject = new ConversationContentViewModel(_mockConversationProvider.Object)
                           {
                               Kernel = mockingKernel,
                               EventAggregator =
                                   _mockEventAggregator
                                   .Object
                           };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void OnActivate_Always_AddsACallViewModelForEachCallInTheConversation()
        {
            var contactInfo = new ContactEntity();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call1 = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { Id = "42" });
            var call2 = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { Id = "43" });
            SetupGetConversationItems(call1, call2);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.Items.Count().Should().Be(2);
        }

        [Test]
        public void OnActivate_WhenCallWasNotViewed_MarksCallAsViewed()
        {
            var contactInfo = new ContactEntity();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { UniqueId = "42" });
            SetupGetConversationItems(call);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            call.WasViewed.Should().BeTrue();
        }


        [Test]
        public void OnActivate_WhenCallWasNotViewed_DismissesNotificationForCall()
        {
            var contactInfo = new ContactEntity();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { UniqueId = "42" });
            SetupGetConversationItems(call);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _mockEventAggregator.Verify(
                m =>
                m.Publish(
                    It.Is<DismissNotification>(n => (string)n.Identifier == call.UniqueId),
                    It.IsAny<Action<Action>>()));
        }

        [Test]
        public void OnActivate_WhenCallWasNotViewed_SavesCall()
        {
            var contactInfo = new ContactEntity();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { UniqueId = "42" });
            SetupGetConversationItems(call);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _mockConversationContext.Verify(m => m.SaveItem(call));
        }

        [Test]
        public void ACallAppearsInTheConversation_ViewModelWasActivated_AddsACallViewModel()
        {
            _subject.Model = new ContactInfoPresenter(new ContactEntity());
            var callFromContact = new LocalPhoneCallPresenter(new LocalPhoneCallEntity());

            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationPresenter>>(
                        200,
                        Notification.CreateOnNext((IConversationPresenter)callFromContact)));
            _mockConversationContext.SetupGet(x => x.ItemChanged).Returns(callObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            var children = _subject.Items.ToList();
            children.Count().Should().Be(1);
        }

        [Test]
        public void ACallAppearsInTheConversation_ViewModelIsActive_MarksCallAsViewed()
        {
            _subject.Model = new ContactInfoPresenter(new ContactEntity());
            var callFromContact = new RemotePhoneCallPresenter(new RemotePhoneCallEntity());
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationPresenter>>(
                        200,
                        Notification.CreateOnNext((IConversationPresenter)callFromContact)));
            _mockConversationContext.SetupGet(x => x.ItemChanged).Returns(callObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            callFromContact.WasViewed.Should().BeTrue();
            _mockConversationContext.Verify(m => m.SaveItem(callFromContact));
        }

        [Test]
        public void OnActivate_Always_AddsAMessageViewModelForEachMessageInTheConversation()
        {
            _subject.Model = new ContactInfoPresenter(new ContactEntity());
            var message1 = new RemoteSmsMessagePresenter(new RemoteSmsMessageEntity());
            var message2 = new LocalSmsMessagePresenter(new LocalSmsMessageEntity());
            SetupGetConversationItems(message1, message2);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.Items.Count().Should().Be(2);
        }

        [Test]
        public void AMessageAppearsInTheConversation_ViewModelIsActive_MarksMessageAsViewed()
        {
            _subject.Model = new ContactInfoPresenter(new ContactEntity());
            var messageFromContact = new RemoteSmsMessagePresenter(new RemoteSmsMessageEntity());
            var observable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationPresenter>>(
                        200,
                        Notification.CreateOnNext((IConversationPresenter)messageFromContact)));
            _mockConversationContext.SetupGet(x => x.ItemChanged).Returns(observable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            messageFromContact.WasViewed.Should().BeTrue();
            _mockConversationContext.Verify(m => m.SaveItem(messageFromContact));
        }

        [Test]
        public void AMessageAppearsInTheConversation_ViewModelWasActivated_AddsAMessageViewModel()
        {
            _subject.Model = new ContactInfoPresenter(new ContactEntity());
            var messageFromContact = new LocalSmsMessagePresenter(new LocalSmsMessageEntity());
            var observable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationPresenter>>(
                        200,
                        Notification.CreateOnNext((IConversationPresenter)messageFromContact)));
            _mockConversationContext.SetupGet(x => x.ItemChanged).Returns(observable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            var children = _subject.Items.ToList();
            children.Count().Should().Be(1);
        }


        [Test]
        public void OnActivate_Always_OrdersItemsForMessagesAndCallsAccordingToTheirTime()
        {
            _subject.Model = new ContactInfoPresenter(new ContactEntity());
            var baseTime = DateTime.Now;
            var call1 = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { Id = "42", Time = baseTime });
            var call2 = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { Id = "43", Time = baseTime.Add(TimeSpan.FromSeconds(10)) });
            var message1 = new LocalSmsMessagePresenter(new LocalSmsMessageEntity { Time = baseTime.Add(TimeSpan.FromSeconds(5)) });
            var message2 = new RemoteSmsMessagePresenter(new RemoteSmsMessageEntity { Time = baseTime.Add(TimeSpan.FromSeconds(15)) });
            SetupGetConversationItems(call1, call2, message1, message2);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.RefreshItems();
            var items = _subject.FilteredItems.Cast<IConversationItemViewModel>().ToList();
            items.Count.Should().Be(4);
            items[0].Model.Should().Be(call1);
            items[1].Model.Should().Be(message1);
            items[2].Model.Should().Be(call2);
            items[3].Model.Should().Be(message2);
        }

        [Test]
        public void OnActivate_PreviousActivationOccured_DoesNotAddViewModelsMultipleTimes()
        {
            var contactInfo = new ContactEntity();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call1 = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { Id = "42" });
            var call2 = new LocalPhoneCallPresenter(new LocalPhoneCallEntity { Id = "43" });
            SetupGetConversationItems(call1, call2);

            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceBy(150);
            ((IDeactivate)_subject).Deactivate(false);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceBy(150);

            _subject.Items.Count().Should().Be(2);
        }

        [Test]
        public void AConversationIsCreated_ViewModelWasActivatedButIsNotActive_AddsACorrespondingChildViewModel()
        {
            _subject.Model = new ContactInfoPresenter(new ContactEntity());
            var callFromContact = new LocalPhoneCallPresenter(new LocalPhoneCallEntity());
            var observable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationPresenter>>(
                        200,
                        Notification.CreateOnNext((IConversationPresenter)callFromContact)));
            _mockConversationContext.SetupGet(x => x.ItemChanged).Returns(observable);

            ((IActivate)_subject).Activate();
            ((IDeactivate)_subject).Deactivate(false);
            _testScheduler.Start();

            _subject.Items.Any(item => item.Model == callFromContact).Should().BeTrue();
        }


        [Test]
        public void AConversationIsCreated_ViewModelWasActivatedButIsNotActive_DoesNotMarkTheItemAsViewed()
        {
            _subject.Model = new ContactInfoPresenter(new ContactEntity());
            var callFromContact = new LocalPhoneCallPresenter(new LocalPhoneCallEntity());
            var observable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationPresenter>>(
                        200,
                        Notification.CreateOnNext((IConversationPresenter)callFromContact)));
            _mockConversationContext.SetupGet(x => x.ItemChanged).Returns(observable);

            ((IActivate)_subject).Activate();
            ((IDeactivate)_subject).Deactivate(false);
            _testScheduler.Start();

            callFromContact.WasViewed.Should().BeFalse();
        }

        private void SetupGetConversationItems(params IConversationPresenter[] items)
        {
            _mockConversationContext.Setup(x => x.GetItems()).Returns(items.ToObservable());
        }
    }
}