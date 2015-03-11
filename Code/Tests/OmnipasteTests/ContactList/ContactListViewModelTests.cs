namespace OmnipasteTests.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ContactList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ContactListViewModelTests
    {
        private ContactListViewModel _subject;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IContactInfoViewModelFactory> _mockContactInfoViewModelFactory;

        private Mock<IConversationProvider> _mockConversationProvider;

        private TestScheduler _testScheduler;

        private Mock<ISessionManager> _mockSessionManager;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;
            
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactInfoViewModelFactory = new Mock<IContactInfoViewModelFactory>();
            _mockConversationProvider = new Mock<IConversationProvider> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager.SetupAllProperties();
            _mockContactInfoViewModelFactory.Setup(x => x.Create(It.IsAny<ContactInfoPresenter>())).Returns<ContactInfoPresenter>(presenter => new ContactInfoViewModel(_mockSessionManager.Object) { Model = presenter });

            _subject = new ContactListViewModel(_mockContactRepository.Object, _mockConversationProvider.Object, _mockContactInfoViewModelFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void Activate_Always_PopulatesListWithStoredContacts()
        {
            var contacts = new List<ContactInfo> { new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } } }, new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } } } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactInfo>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void ContactIsSaved_AfterActivate_AddsContactToList()
        {
            var repositoryOperation = new RepositoryOperation<ContactInfo>(RepositoryMethodEnum.Change, new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "42" } } });
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ContactInfo>>>(100, Notification.CreateOnNext(repositoryOperation)),
                    new Recorded<Notification<RepositoryOperation<ContactInfo>>>(200, Notification.CreateOnCompleted<RepositoryOperation<ContactInfo>>()));
            _mockContactRepository.SetupGet(m => m.OperationObservable).Returns(contactObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void ShowStarred_ChangesToTrue_FiltersItemsThatAreStarred()
        {
            var contacts = new List<ContactInfo> { new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } }, IsStarred = true }, new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } }, IsStarred = false } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactInfo>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.ShowStarred = true;

            _subject.FilteredItems.Count.Should().Be(1);
        }

        [Test]
        public void ShowStarred_ChangesToFalse_ShowsAllItems()
        {
            var contacts = new List<ContactInfo> { new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } }, IsStarred = true }, new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } }, IsStarred = false } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactInfo>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.ShowStarred = false;

            _subject.FilteredItems.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void FilterText_ChangesToEmpty_ShowsAllItems()
        {
            var contacts = new List<ContactInfo> { new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } } }, new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } } } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactInfo>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = string.Empty;

            _subject.FilteredItems.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void FilterText_MatchesPhoneNumber_ShowsMatchingItems()
        {
            var contacts = new List<ContactInfo> { new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } } }, new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } } } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactInfo>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "1";

            _subject.FilteredItems.Count.Should().Be(1);
            ((IContactInfoViewModel)_subject.FilteredItems.GetItemAt(0)).Model.ContactInfo.Should().Be(contacts.First());
        }

        [Test]
        public void FilterText_MatchesName_ShowsMatchingItems()
        {
            var contacts = new List<ContactInfo> { new ContactInfo { FirstName = "Test" }, new ContactInfo { LastName = "T2" }, new ContactInfo { FirstName = "some" } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactInfo>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactInfo>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "t";

            _subject.FilteredItems.Count.Should().Be(2);
        }

        [Test]
        public void OnConversationItemAdded_WhenContactExists_RemovesItemFromList()
        {
            var contactInfo = new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "42" } } };
            var call = new PhoneCall { ContactInfo = contactInfo };
            _mockContactRepository.Setup(m => m.GetAll())
                .Returns(Observable.Return(new List<ContactInfo> { contactInfo }.AsEnumerable(), _testScheduler));
            var mockConversationContext = new Mock<IConversationContext> { DefaultValue = DefaultValue.Mock };
            mockConversationContext.Setup(m => m.ItemChanged).Returns(Observable.Return(call, _testScheduler));
            _mockConversationProvider.Setup(m => m.All()).Returns(mockConversationContext.Object);
            var eventArgs = new List<NotifyCollectionChangedEventArgs>();
            _subject.Items.CollectionChanged += (sender, args) => { eventArgs.Add(args); };

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            eventArgs[1].Action.Should().Be(NotifyCollectionChangedAction.Remove);
            eventArgs[1].OldItems.Cast<IContactInfoViewModel>().First().Model.ContactInfo.Should().Be(contactInfo);
        }

        [Test]
        public void OnConversationItemAdded_AfterActivate_AddsItemToList()
        {
            var contactInfo = new ContactInfo { PhoneNumbers = new[] { new PhoneNumber { Number = "42" } } };
            var call = new PhoneCall { ContactInfo = contactInfo };
            _mockContactRepository.Setup(m => m.GetAll())
                .Returns(Observable.Return(new List<ContactInfo> { contactInfo }.AsEnumerable(), _testScheduler));
            var mockConversationContext = new Mock<IConversationContext> { DefaultValue = DefaultValue.Mock };
            mockConversationContext.Setup(m => m.ItemChanged).Returns(Observable.Return(call, _testScheduler));
            _mockConversationProvider.Setup(m => m.All()).Returns(mockConversationContext.Object);
            var eventArgs = new List<NotifyCollectionChangedEventArgs>();
            _subject.Items.CollectionChanged += (sender, args) => { eventArgs.Add(args); };

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            eventArgs[2].Action.Should().Be(NotifyCollectionChangedAction.Add);
            eventArgs[2].NewItems.Cast<IContactInfoViewModel>().First().Model.ContactInfo.Should().Be(contactInfo);
        }
    }
}