namespace OmnipasteTests.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ContactList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ContactListViewModelTests
    {
        private ContactListViewModel _subject;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IContactInfoViewModelFactory> _mockContactInfoViewModelFactory;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;
            
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactInfoViewModelFactory = new Mock<IContactInfoViewModelFactory>();

            _mockContactInfoViewModelFactory.Setup(x => x.Create(It.IsAny<ContactInfoPresenter>())).Returns<ContactInfoPresenter>(presenter => new ContactInfoViewModel() { Model = presenter });

            _subject = new ContactListViewModel(_mockContactRepository.Object, _mockContactInfoViewModelFactory.Object);
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
            var contacts = new List<ContactInfo> { new ContactInfo { Phone = "1" }, new ContactInfo { Phone = "2" } };
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
            var repositoryOperation = new RepositoryOperation<ContactInfo>(RepositoryMethodEnum.Create, new ContactInfo { Phone = "42" });
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
            var contacts = new List<ContactInfo> { new ContactInfo { Phone = "1", IsStarred = true }, new ContactInfo { Phone = "2", IsStarred = false} };
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
            var contacts = new List<ContactInfo> { new ContactInfo { Phone = "1", IsStarred = true }, new ContactInfo { Phone = "2", IsStarred = false } };
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
            var contacts = new List<ContactInfo> { new ContactInfo { Phone = "1" }, new ContactInfo { Phone = "2" } };
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
            var contacts = new List<ContactInfo> { new ContactInfo { Phone = "1" }, new ContactInfo { Phone = "2" } };
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
            ((IContactInfoViewModel)_subject.FilteredItems.GetItemAt(0)).Model.ContactInfo.Should().Be(contacts[0]);
            ((IContactInfoViewModel)_subject.FilteredItems.GetItemAt(1)).Model.ContactInfo.Should().Be(contacts[1]);
        }
    }
}