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
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ContactList;
    using Omnipaste.ContactList.ContactInfo;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    [TestFixture]
    public class ContactListViewModelTests
    {
        private ContactListViewModel _subject;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IContactInfoViewModelFactory> _mockContactInfoViewModelFactory;

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
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager.SetupAllProperties();
            _mockContactInfoViewModelFactory.Setup(
                x => x.Create<IContactInfoViewModel>(It.IsAny<ContactInfoPresenter>()))
                .Returns<ContactInfoPresenter>(
                    presenter => new ContactInfoViewModel(_mockSessionManager.Object) { Model = presenter });
            
            MoqMockingKernel kernel = new MoqMockingKernel();
            kernel.Bind<IContactListViewModel>().To<ContactListViewModel>();
            kernel.Bind<IContactRepository>().ToConstant(_mockContactRepository.Object);
            kernel.Bind<IContactInfoViewModelFactory>().ToConstant(_mockContactInfoViewModelFactory.Object);
            _subject = (ContactListViewModel)kernel.Get<IContactListViewModel>();
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
            var contacts = new List<ContactEntity> { new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } } }, new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } } } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.Items.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void ContactIsSaved_AfterActivate_AddsContactToList()
        {
            var repositoryOperation = new RepositoryOperation<ContactEntity>(RepositoryMethodEnum.Changed, new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "42" } } });
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ContactEntity>>>(100, Notification.CreateOnNext(repositoryOperation)),
                    new Recorded<Notification<RepositoryOperation<ContactEntity>>>(200, Notification.CreateOnCompleted<RepositoryOperation<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetOperationObservable()).Returns(contactObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void ShowStarred_ChangesToTrue_FiltersItemsThatAreStarred()
        {
            var contacts = new List<ContactEntity> { new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } }, IsStarred = true }, new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } }, IsStarred = false } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.ShowStarred = true;

            _subject.FilteredItems.Count.Should().Be(1);
        }

        [Test]
        public void ShowStarred_ChangesToFalse_ShowsAllItems()
        {
            var contacts = new List<ContactEntity> { new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } }, IsStarred = true }, new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } }, IsStarred = false } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.ShowStarred = false;

            _subject.FilteredItems.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void FilterText_ChangesToEmpty_ShowsAllItems()
        {
            var contacts = new List<ContactEntity> { new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } } }, new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } } } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = string.Empty;

            _subject.FilteredItems.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void FilterText_MatchesPhoneNumber_ShowsMatchingItems()
        {
            var contacts = new List<ContactEntity> { new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "1" } } }, new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "2" } } } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "1";

            _subject.FilteredItems.Count.Should().Be(1);
            ((IContactInfoViewModel)_subject.FilteredItems.GetItemAt(0)).Model.BackingModel.Should().Be(contacts.First());
        }

        [Test]
        public void FilterText_MatchesName_ShowsMatchingItems()
        {
            var contacts = new List<ContactEntity> { new ContactEntity { FirstName = "Test" }, new ContactEntity { LastName = "T2" }, new ContactEntity { FirstName = "some" } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "t";

            _subject.FilteredItems.Count.Should().Be(2);
        }


        [Test]
        public void FilterText_MatchesItems_SetsStateToNotEmpty()
        {
            var contacts = new List<ContactEntity> { new ContactEntity { FirstName = "Test" } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "est";

            _subject.Status.Should().Be(ListViewModelStatusEnum.NotEmpty);
        }

        [Test]
        public void FilterText_DoesntMatchAnyItem_SetsStateToEmptyFilter()
        {
            var contacts = new List<ContactEntity> { new ContactEntity { FirstName = "Test" } };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "bla";

            _subject.Status.Should().Be(ListViewModelStatusEnum.EmptyFilter);
        }

        [Test]
        public void FilteredItemsSorting_SortsItemsDescendingBasedOnLastActivity()
        {
            var oldestContact = new ContactEntity { FirstName = "Test1", LastActivityTime = DateTime.Now.AddDays(-1) };
            var newestContact = new ContactEntity { FirstName = "Test2", LastActivityTime = DateTime.Now };
            var contacts = new List<ContactEntity>
                               {
                                   oldestContact, 
                                   newestContact
                               };
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        100,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        200,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "Test";

            _subject.FilteredItems.Cast<IContactInfoViewModel>().First().Model.BackingModel.Should().Be(contacts[1]);
            _subject.FilteredItems.Cast<IContactInfoViewModel>().Last().Model.BackingModel.Should().Be(contacts[0]);
        }
    }
}