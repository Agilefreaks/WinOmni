namespace OmnipasteTests.ActivityDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation;
    using Omnipaste.WorkspaceDetails.Conversation.Call;
    using Omnipaste.WorkspaceDetails.Conversation.Message;
    using OmniUI.Details;

    [TestFixture]
    public class ConversationContentViewModelTests
    {
        private ConversationContentViewModel _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private TestScheduler _testScheduler;

        private Mock<IConversationProvider> _mockConversationProvider;

        private Mock<IConversationContext> _mockConversation;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            var mockingKernel = new MoqMockingKernel();
            mockingKernel.Bind<ICallViewModel>()
                .ToMethod(context => new CallViewModel(_mockUiRefreshService.Object, _mockConfigurationService.Object));
            mockingKernel.Bind<IMessageViewModel>()
                .ToMethod(
                    context => new MessageViewModel(_mockUiRefreshService.Object, _mockConfigurationService.Object));
            _mockConversationProvider = new Mock<IConversationProvider> { DefaultValue = DefaultValue.Mock };
            _mockConversation = new Mock<IConversationContext> { DefaultValue = DefaultValue.Mock };
            _mockConversationProvider.Setup(x => x.ForContact(It.IsAny<ContactInfo>()))
                .Returns(_mockConversation.Object);
            _mockEventAggregator = new Mock<IEventAggregator> { DefaultValue = DefaultValue.Mock };
            _subject = new ConversationContentViewModel
                           {
                               Kernel = mockingKernel,
                               EventAggregator = _mockEventAggregator.Object,
                               ConversationProvider = _mockConversationProvider.Object
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
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call1 = new Call();
            var call2 = new Call();
            SetupGetConversationItems(call1, call2);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.Items.Count().Should().Be(2);
        }

        [Test]
        public void Activate_WhenCallWasNotViewed_MarksCallAsViewed()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new Call { UniqueId = "42" };
            SetupGetConversationItems(call);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            call.WasViewed.Should().BeTrue();
        }

        [Test]
        public void Activate_WhenCallWasNotViewed_DismissesNotificationForCall()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new Call { UniqueId = "42" };
            SetupGetConversationItems(call);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _mockEventAggregator.Verify(m => m.Publish(It.Is<DismissNotification>(n => n.Identifier == call.UniqueId), It.IsAny<Action<Action>>()));
        }

        [Test]
        public void Activate_WhenCallWasNotViewed_SavesCall()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call = new Call { UniqueId = "42" };
            SetupGetConversationItems(call);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _mockConversationProvider.Verify(m => m.SaveItem(call));
        }

        [Test]
        public void ACallAppearsInTheConversation_Always_AddsACallViewModel()
        {
            var callFromContact = new Call { ContactInfo = new ContactInfo { Phone = "123" } };
            _subject.Model = new ContactInfoPresenter(new ContactInfo { Phone = "123" });
            var callObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<IConversationItem>>(200, Notification.CreateOnNext(callFromContact as IConversationItem)));
            _mockConversation.SetupGet(x => x.ItemAdded).Returns(callObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            var children = _subject.Items.ToList();
            children.Count().Should().Be(1);
        }

        [Test]
        public void OnActivate_Always_AddsAMessageViewModelForEachMessageInTheConversation()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var message1 = new Message();
            var message2 = new Message();
            SetupGetConversationItems(message1, message2);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.Items.Count().Should().Be(2);
        }

        [Test]
        public void AMessageAppearsInTheConversation_Always_AddsAMessageViewModel()
        {
            var messageFromContact = new Message { ContactInfo = new ContactInfo { Phone = "123" } };
            _subject.Model = new ContactInfoPresenter(new ContactInfo { Phone = "123" });
            var observable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<IConversationItem>>(200, Notification.CreateOnNext(messageFromContact as IConversationItem)));
            _mockConversation.SetupGet(x => x.ItemAdded).Returns(observable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            var children = _subject.Items.ToList();
            children.Count().Should().Be(1);
        }

        [Test]
        public void OnActivate_Always_OrdersItemsForMessagesAndCallsAccordingToTheirTime()
        {
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var baseTime = DateTime.Now;
            var call1 = new Call { Time = baseTime };
            var call2 = new Call { Time = baseTime.Add(TimeSpan.FromSeconds(10)) };
            var message1 = new Message { Time = baseTime.Add(TimeSpan.FromSeconds(5)) };
            var message2 = new Message { Time = baseTime.Add(TimeSpan.FromSeconds(15)) };
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
            var contactInfo = new ContactInfo();
            _subject.Model = new ContactInfoPresenter(contactInfo);
            var call1 = new Call();
            var call2 = new Call();
            SetupGetConversationItems(call1, call2);

            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceBy(150);
            ((IDeactivate)_subject).Deactivate(false);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceBy(150);

            _subject.Items.Count().Should().Be(2);
        }

        private void SetupGetConversationItems(params IConversationItem[] items)
        {
            var observable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<IConversationItem>>>(
                        100,
                        Notification.CreateOnNext<IEnumerable<IConversationItem>>(items)));
            _mockConversation.Setup(x => x.GetItems()).Returns(observable);
        }
    }
}