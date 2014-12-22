namespace OmniHolidaysTests.MessagesWorkspace.MessageDetails
{
    using System;
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniHolidays.Commands;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniUI.Framework;
    using OmniUI.Models;
    using OmniUI.Presenters;
    using OmniUI.Services;

    [TestFixture]
    public class MessageDetailsHeaderViewModelTests
    {
        #region Fields

        private Mock<ICommandService> _mockCommandService;

        private Mock<IContactSource> _mockContactsSource;

        private MessageDetailsHeaderViewModel _subject;

        private TestScheduler _testScheduler;

        #endregion

        #region Public Methods and Operators

        [Test]
        public void ClearContacts_Always_SetsIsSelectedFalseForAllCurrentContacts()
        {
            _subject.ContactsSource.Contacts.Cast<object>().Count().Should().Be(2);

            _subject.ClearContacts();

            _subject.ContactsSource.Contacts.Cast<IContactInfoPresenter>()
                .Where(contact => contact.IsSelected)
                .Should()
                .BeEmpty();
        }

        [Test]
        public void OnActivate_Always_SetsStateToNormal()
        {
            var executeObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Unit>>(200, Notification.CreateOnCompleted<Unit>()));
            _mockCommandService.Setup(x => x.Execute(It.IsAny<SendMassSMSMessageCommand>())).Returns(executeObservable);

            _subject.SendMessage(string.Empty);
            _testScheduler.Start();
            ((IMessageDetailsHeaderViewModel)_subject).Activate();

            _subject.State.Should().Be(MessageDetailsHeaderState.Normal);
        }

        [Test]
        public void SendMessage_Always_ExecutesASendMassSMSMessageCommand()
        {
            _subject.SendMessage("someTemplate");

            _mockCommandService.Verify(
                x =>
                x.Execute(
                    It.Is<SendMassSMSMessageCommand>(
                        command => command.Template.Equals("someTemplate") && command.Contacts.Count() == 2)),
                Times.Once());
        }

        [Test]
        public void SendMessage_CommandFails_ResetsTheMessageDetails()
        {
            var executeObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Unit>>(200, Notification.CreateOnError<Unit>(new Exception())));
            _mockCommandService.Setup(x => x.Execute(It.IsAny<SendMassSMSMessageCommand>())).Returns(executeObservable);
            var mockMessageDetails = new Mock<IMessageDetailsViewModel>();
            _subject.Parent = mockMessageDetails.Object;

            _subject.SendMessage(string.Empty);
            _testScheduler.Start();

            mockMessageDetails.Verify(x => x.Reset());
        }

        [Test]
        public void SendMessage_CommandIsExecutedSuccessfully_SetsStateToSent()
        {
            var executeObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Unit>>(100, Notification.CreateOnNext(new Unit())),
                    new Recorded<Notification<Unit>>(200, Notification.CreateOnCompleted<Unit>()));
            _mockCommandService.Setup(x => x.Execute(It.IsAny<SendMassSMSMessageCommand>())).Returns(executeObservable);

            _subject.SendMessage(string.Empty);
            _testScheduler.Start();

            _subject.State.Should().Be(MessageDetailsHeaderState.Sent);
        }

        [Test]
        public void SendMessage_CommandIsExecuting_SetsStateToSending()
        {
            var executeObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Unit>>(200, Notification.CreateOnCompleted<Unit>()));
            _mockCommandService.Setup(x => x.Execute(It.IsAny<SendMassSMSMessageCommand>())).Returns(executeObservable);

            _subject.SendMessage(string.Empty);
            _testScheduler.Start();

            _subject.State.Should().Be(MessageDetailsHeaderState.Sending);
        }

        [SetUp]
        public void Setup()
        {
            _mockContactsSource = CreateMockContactSource();
            _mockCommandService = new Mock<ICommandService> { DefaultValue = DefaultValue.Mock };
            _subject = new MessageDetailsHeaderViewModel
                           {
                               CommandService = _mockCommandService.Object,
                               ContactsSource = _mockContactsSource.Object
                           };
            _testScheduler = new TestScheduler();
        }

        #endregion

        #region Methods

        private static Mock<IContactSource> CreateMockContactSource()
        {
            var contactList = Enumerable.Range(0, 2).Select(_ => GetContactPresenter()).ToList();
            var sourceCollection = new BindableCollection<IContactInfoPresenter>(contactList);
            Predicate<object> filter = item => ((IContactInfoPresenter)item).IsSelected;
            var collectionView = new DeepObservableCollectionView<IContactInfoPresenter>(sourceCollection)
                                     {
                                         Filter = filter
                                     };
            var mockContactsSource = new Mock<IContactSource>();
            mockContactsSource.Setup(x => x.Contacts).Returns(collectionView);

            return mockContactsSource;
        }

        private static IContactInfoPresenter GetContactPresenter()
        {
            var mockContactInfoPresenter = new Mock<IContactInfoPresenter>();
            mockContactInfoPresenter.SetupProperty(x => x.IsSelected);
            mockContactInfoPresenter.SetupGet(x => x.ContactInfo).Returns(() => new ContactInfo());
            mockContactInfoPresenter.Object.IsSelected = true;

            return mockContactInfoPresenter.Object;
        }

        #endregion
    }
}