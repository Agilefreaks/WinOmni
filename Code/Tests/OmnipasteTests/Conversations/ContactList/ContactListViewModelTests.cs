namespace OmnipasteTests.Conversations.ContactList
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
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Conversations.ContactList.Contact;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services;
    using Omnipaste.Framework.Services.Repositories;
    using OmniUI.Details;
    using OmniUI.List;
    using OmniUI.Workspaces;

    [TestFixture]
    public class ContactListViewModelTests
    {
        private ContactListViewModel _subject;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IContactViewModelFactory> _mockContactViewModelFactory;

        private TestScheduler _testScheduler;

        private Mock<ISessionManager> _mockSessionManager;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactViewModelFactory = new Mock<IContactViewModelFactory>();
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager.SetupAllProperties();
            _mockContactViewModelFactory.Setup(x => x.Create<IContactViewModel>(It.IsAny<ContactModel>()))
                .Returns<ContactModel>(
                    contactModel => new ContactViewModel(_mockSessionManager.Object) { Model = contactModel });

            MoqMockingKernel kernel = new MoqMockingKernel();
            kernel.Bind<IContactListViewModel>().To<ContactListViewModel>();
            kernel.Bind<IContactRepository>().ToConstant(_mockContactRepository.Object);
            kernel.Bind<IContactViewModelFactory>().ToConstant(_mockContactViewModelFactory.Object);
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
            var contacts = SetupExistingContacts(2);
            
            ((IActivate)_subject).Activate();
            _testScheduler.Start();

            _subject.Items.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void Activate_WhenCanSelectMultipleItemsIsFalse_WillNotShowDetails()
        {
            _subject.CanSelectMultipleItems = false;

            var mockDetailsWorkSpace = new Mock<IMasterDetailsWorkspace>();
            var mockDetailsConductorViewModel = new Mock<IDetailsConductorViewModel>();
            mockDetailsWorkSpace.SetupGet(mock => mock.DetailsConductor).Returns(mockDetailsConductorViewModel.Object);
            _subject.DetailsWorkspace = mockDetailsWorkSpace.Object;

            ((IActivate)_subject).Activate();

            mockDetailsConductorViewModel.Verify(
                mock => mock.ActivateItem(It.IsAny<IDetailsViewModelWithHeader>()),
                Times.Never);
        }

        [Test]
        public void ContactIsSaved_AfterActivate_AddsContactToList()
        {
            SetupContactAddedObservable();
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void ContactIsSaved_AfterActivateAndCanSelectMultipleItemsIsFalse_ClearsExistingSelection()
        {
            SetupExistingContacts(2, 110, 210);
            var addedContact = SetupContactAddedObservable(300, 400);

            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromMilliseconds(250).Ticks);
            var contactViewModel = _subject.Items.First();
            contactViewModel.IsSelected = true;
            _subject.SelectedContacts.Add(contactViewModel.Model);

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.SelectedContacts.Count.Should().Be(1);
            _subject.SelectedContacts.First().PhoneNumber.Should().Be(addedContact.PhoneNumber);
        }

        [Test]
        public void ShowStarred_ChangesToTrue_FiltersItemsThatAreStarred()
        {
            var contacts = SetupExistingContacts(2);
            contacts.First().IsStarred = true;
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.ShowStarred = true;

            _subject.FilteredItems.Count.Should().Be(1);
        }

        [Test]
        public void ShowStarred_ChangesToFalse_ShowsAllItems()
        {
            var contacts = SetupExistingContacts(2);
            contacts.First().IsStarred = true;
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.ShowStarred = false;

            _subject.FilteredItems.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void FilterText_ChangesToEmpty_ShowsAllItems()
        {
            var contacts = SetupExistingContacts(2);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = string.Empty;

            _subject.FilteredItems.Count.Should().Be(contacts.Count);
        }

        [Test]
        public void FilterText_MatchesPhoneNumber_ShowsMatchingItems()
        {
            var contacts = SetupExistingContacts(2);
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "1";

            _subject.FilteredItems.Count.Should().Be(1);
            ((IContactViewModel)_subject.FilteredItems.GetItemAt(0)).Model.BackingEntity.Should().Be(contacts.First());
        }

        [Test]
        public void FilterText_MatchesName_ShowsMatchingItems()
        {
            var contacts = SetupExistingContacts(3);
            contacts[0].FirstName = "Test";
            contacts[1].LastName = "T2";
            contacts[2].FirstName = "swome";
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "t";

            _subject.FilteredItems.Count.Should().Be(2);
        }

        [Test]
        public void FilterText_MatchesItems_SetsStateToNotEmpty()
        {
            var contacts = SetupExistingContacts(1);
            contacts.First().FirstName = "Test";
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "est";

            _subject.Status.Should().Be(ListViewModelStatusEnum.NotEmpty);
        }

        [Test]
        public void FilterText_DoesntMatchAnyItem_SetsStateToEmptyFilter()
        {
            var contacts = SetupExistingContacts(1);
            contacts.First().FirstName = "Test";
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "bla";

            _subject.Status.Should().Be(ListViewModelStatusEnum.EmptyFilter);
        }

        [Test]
        public void FilteredItemsSorting_SortsItemsDescendingBasedOnLastActivity()
        {
            var contacts = SetupExistingContacts(2);
            var oldestContact = contacts.First();
            var newestContact = contacts.Last();
            oldestContact.LastActivityTime = DateTime.Now.Subtract(2.Days());
            newestContact.LastActivityTime = DateTime.Now;
            oldestContact.FirstName = "Test1";
            newestContact.FirstName = "Test2";
            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.FilterText = "Test";

            _subject.FilteredItems.Cast<IContactViewModel>().First().Model.BackingEntity.Should().Be(contacts[1]);
            _subject.FilteredItems.Cast<IContactViewModel>().Last().Model.BackingEntity.Should().Be(contacts[0]);
        }

        private IList<ContactEntity> SetupExistingContacts(
            int contactsCount,
            int observableTriggerTimeStamp = 100,
            int observableEndTimeStamp = 200)
        {
            var contacts = CreateContacts(contactsCount);
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        observableTriggerTimeStamp,
                        Notification.CreateOnNext(contacts.AsEnumerable())),
                    new Recorded<Notification<IEnumerable<ContactEntity>>>(
                        observableEndTimeStamp,
                        Notification.CreateOnCompleted<IEnumerable<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetAll()).Returns(contactObservable);

            return contacts;
        }

        private ContactEntity SetupContactAddedObservable(
            int observableTriggerTimeStamp = 100,
            int observableEndTimeStamp = 200)
        {
            var contactEntity = new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = "42" } } };
            var repositoryOperation = new RepositoryOperation<ContactEntity>(
                RepositoryMethodEnum.Changed,
                contactEntity);
            var contactObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ContactEntity>>>(
                        observableTriggerTimeStamp,
                        Notification.CreateOnNext(repositoryOperation)),
                    new Recorded<Notification<RepositoryOperation<ContactEntity>>>(
                        observableEndTimeStamp,
                        Notification.CreateOnCompleted<RepositoryOperation<ContactEntity>>()));
            _mockContactRepository.Setup(m => m.GetOperationObservable()).Returns(contactObservable);

            return contactEntity;
        }

        private static List<ContactEntity> CreateContacts(int count)
        {
            return
                Enumerable.Range(0, count)
                    .Select(
                        index =>
                        new ContactEntity { PhoneNumbers = new[] { new PhoneNumber { Number = (index + 1).ToString() } } })
                    .ToList();
        }
    }
}