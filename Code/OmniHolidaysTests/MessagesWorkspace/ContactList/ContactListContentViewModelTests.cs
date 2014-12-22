namespace OmniHolidaysTests.MessagesWorkspace.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Contacts.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniHolidays.Commands;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniUI.Helpers;
    using OmniUI.Presenters;
    using OmniUI.Services;

    [TestFixture]
    public class ContactListContentViewModelTests
    {
        private ContactListContentViewModel _subject;

        private Mock<IApplicationHelper> _mockApplicationHelper;

        private Mock<ICommandService> _mockCommandService;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _mockCommandService = new Mock<ICommandService>();
            _subject = new ContactListContentViewModel(_mockCommandService.Object);
            _mockApplicationHelper = new Mock<IApplicationHelper>();
            _mockApplicationHelper.Setup(x => x.FindResource(ContactInfoPresenter.UserPlaceholderBrush))
                .Returns(new DrawingBrush { Drawing = new DrawingGroup() });
            ApplicationHelper.Instance = _mockApplicationHelper.Object;
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            ApplicationHelper.Instance = null;
            SchedulerProvider.Default = null;
        }

        [Test]
        public void FilteredItems_Always_ContainsOnlyItemsWhichHaveAModelIdentifierWhichContainsTheFilterText()
        {
            var identifiers = new[] { "a1b", "a2c", "b3c" };
            var counter = 0;
            var model1 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddEntity(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddEntity(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            AddEntity(model3);

            _subject.FilterText = "a";

            var viewModels = _subject.FilteredItems.Cast<IContactInfoPresenter>().ToList();
            viewModels.Count.Should().Be(2);
            viewModels[0].Should().Be(model1);
            viewModels[1].Should().Be(model2);
        }

        [Test]
        public void SetSelectAllTrue_Always_SetsIsSelectedTrueForAllTheFilteredItems()
        {
            var identifiers = new[] { "a1b", "a2c", "b3c" };
            var counter = 0;
            var model1 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddEntity(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddEntity(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            AddEntity(model3);
            model1.IsSelected = false;

            _subject.FilterText = "c";
            _subject.SelectAll = true;

            model1.IsSelected.Should().BeFalse();
            model2.IsSelected.Should().BeTrue();
            model3.IsSelected.Should().BeTrue();
        }

        [Test]
        public void SetSelectAllFalse_Always_SetsIsSelectedFalseForAllModelsOnAllTheFilteredItems()
        {
            var identifiers = new[] { "a1b", "a2c", "b3c" };
            var counter = 0;
            var model1 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddEntity(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddEntity(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            AddEntity(model3);
            model1.IsSelected = true;
            model2.IsSelected = true;
            model3.IsSelected = true;

            _subject.FilterText = "a";
            _subject.SelectAll = false;

            model1.IsSelected.Should().BeFalse();
            model2.IsSelected.Should().BeFalse();
            model3.IsSelected.Should().BeTrue();
        }

        [Test]
        public void SelectingAllContacts_Always_SetsSelectAllTrue()
        {
            var contact1 = new ContactInfoPresenter();
            var contact2 = new ContactInfoPresenter();
            _subject.Items.Add(contact1);
            _subject.Items.Add(contact2);

            _subject.SelectAll.Should().BeFalse();
            contact1.IsSelected = true;
            _subject.SelectAll.Should().BeFalse();
            contact2.IsSelected = true;
            _subject.SelectAll.Should().BeTrue();
        }

        [Test]
        public void OnActivate_WhenCommandReturnsContactsWithMultiplePhoneNumbers_AddsEntriesForEachPhoneNumber()
        {
            var contact = new Contact
                              {
                                  FirstName = "first",
                                  LastName = "last",
                                  Numbers =
                                      new List<ContactPhoneNumber>
                                          {
                                              new ContactPhoneNumber { Number = "1" },
                                              new ContactPhoneNumber { Number = "2" }
                                          }
                              };
            var contactList = new ContactList { Contacts = new List<Contact> { contact } };
            var contactObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<ContactList>>(100, Notification.CreateOnNext(contactList)),
                new Recorded<Notification<ContactList>>(200, Notification.CreateOnCompleted<ContactList>()));

            _mockCommandService.Setup(m => m.Execute(It.IsAny<SyncContactsCommand>())).Returns(contactObservable);

            ((IActivate)_subject).Activate();
            _testScheduler.Start(WaitForViewModelToLoadContacts());

            _subject.Items.Count.Should().Be(2);
        }

        private Func<IObservable<bool>> WaitForViewModelToLoadContacts()
        {
            return
                () =>
                Observable.FromEventPattern(_subject, "PropertyChanged", _testScheduler)
                    .Select(@event => @event.EventArgs)
                    .Cast<PropertyChangedEventArgs>()
                    .Where(eventArgs => eventArgs.PropertyName == "IsBusy")
                    .Select(_ => _subject.IsBusy)
                    .Where(isBusy => !isBusy)
                    .Take(1);
        }

        private void AddEntity(IContactInfoPresenter model1)
        {
            _subject.Items.Add(model1);
        }
    }
}