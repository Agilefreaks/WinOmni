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
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation;
    using Omnipaste.WorkspaceDetails.Conversation.Call;
    using Omnipaste.WorkspaceDetails.Conversation.Message;

    [TestFixture]
    public class ConversationContentViewModelTests
    {
        private ConversationContentViewModel _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private TestScheduler _testScheduler;

        private Mock<IConversationProvider> _mockConversationProvider;

        private Mock<IConversationContext> _mockConversation;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;
            var mockingKernel = new MoqMockingKernel();
            mockingKernel.Bind<ICallViewModel>().ToMethod(context => CreateMock<ICallViewModel>());
            mockingKernel.Bind<IMessageViewModel>().ToMethod(context => CreateMock<IMessageViewModel>());
            _mockConversationProvider = new Mock<IConversationProvider>();
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

            _mockConversation.Verify(m => m.Save(call));
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

            _subject.Items.Count().Should().Be(4);
            var models = _subject.Items.Select(item => item.Model).ToList();
            models.IndexOf(call1).Should().Be(0);
            models.IndexOf(message1).Should().Be(1);
            models.IndexOf(call2).Should().Be(2);
            models.IndexOf(message2).Should().Be(3);
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

        private static TViewModel CreateMock<TViewModel>()
            where TViewModel : class
        {
            var mock = new Mock<TViewModel> { DefaultValue = DefaultValue.Mock };
            mock.SetupAllProperties();
            return mock.Object;
        }
    }
}