namespace OmniHolidaysTests.MessagesWorkspace.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Contacts.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniHolidays.Commands;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniUI.Helpers;
    using OmniUI.Presenters;
    using OmniUI.Services;

    [TestFixture]
    public class ContactListContentViewModelTests
    {
        private ContactListContentViewModel _subject;

        private IKernel _mockKernel;

        private Mock<IApplicationHelper> _mockApplicationHelper;

        private Mock<ICommandService> _mockCommandService;

        [SetUp]
        public void Setup()
        {
            _mockKernel = new MoqMockingKernel();

            _mockKernel.Bind<IContactViewModel>().ToMethod(context => new ContactViewModel());
            _mockCommandService = new Mock<ICommandService>();
            _subject = new ContactListContentViewModel(_mockCommandService.Object, _mockKernel);
            _mockApplicationHelper = new Mock<IApplicationHelper>();
            _mockApplicationHelper.Setup(x => x.FindResource(ContactInfoPresenter.UserPlaceholderBrush))
                .Returns(new DrawingBrush { Drawing = new DrawingGroup() });
            ApplicationHelper.Instance = _mockApplicationHelper.Object;
        }

        [TearDown]
        public void TearDown()
        {
            ApplicationHelper.Instance = null;
        }

        [Test]
        public void FilteredItems_Always_ContainsOnlyItemsWhichHaveAModelIdentifierWhichContainsTheFilterText()
        {
            _subject.Start();
            var identifiers = new[] { "a1b", "a2c", "b3c" };
            var counter = 0;
            var model1 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddChildViewModel(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddChildViewModel(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            AddChildViewModel(model3);

            _subject.FilterText = "a";

            var viewModels = _subject.FilteredItems.Cast<IContactViewModel>().ToList();
            viewModels.Count.Should().Be(2);
            viewModels[0].Model.Should().Be(model1);
            viewModels[1].Model.Should().Be(model2);
        }

        [Test]
        public void SetSelectAllTrue_Always_SetsIsSelectedTrueForAllTheFilteredItems()
        {
            _subject.Start();
            var identifiers = new[] { "a1b", "a2c", "b3c" };
            var counter = 0;
            var model1 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddChildViewModel(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddChildViewModel(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            AddChildViewModel(model3);
            ViewModelWithModel(model1).IsSelected = false;

            _subject.FilterText = "c";
            _subject.SelectAll = true;

            ViewModelWithModel(model1).IsSelected.Should().BeFalse();
            ViewModelWithModel(model2).IsSelected.Should().BeTrue();
            ViewModelWithModel(model3).IsSelected.Should().BeTrue();
        }

        [Test]
        public void SetSelectAllFalse_Always_SetsIsSelectedFalseForAllModelsOnAllTheFilteredItems()
        {
            _subject.Start();
            var identifiers = new[] { "a1b", "a2c", "b3c" };
            var counter = 0;
            var model1 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddChildViewModel(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            AddChildViewModel(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            AddChildViewModel(model3);
            ViewModelWithModel(model1).IsSelected = true;
            ViewModelWithModel(model2).IsSelected = true;
            ViewModelWithModel(model3).IsSelected = true;

            _subject.FilterText = "a";
            _subject.SelectAll = false;

            ViewModelWithModel(model1).IsSelected.Should().BeFalse();
            ViewModelWithModel(model2).IsSelected.Should().BeFalse();
            ViewModelWithModel(model3).IsSelected.Should().BeTrue();
        }

        [Test]
        public void ActivateItem_AlreadyActivatedMoreThenTheMaximum_ActivatesItemWithoutRemovingOtherItems()
        {
            ((IActivate)_subject).Activate();
            Enumerable.Range(0, ContactListContentViewModel.MaxItemCount)
                .Select(number => new ContactViewModel { Model = new ContactInfoPresenter() })
                .ToList()
                .ForEach(viewModel => _subject.ActivateItem(viewModel));
            var latestViewModel = new ContactViewModel { Model = new ContactInfoPresenter() };

            _subject.ActivateItem(latestViewModel);

            _subject.Items.Count.Should().Be(ContactListContentViewModel.MaxItemCount + 1);
            _subject.Items.First().Should().Be(latestViewModel);
        }

        [Test]
        public void SelectingAllContacts_Always_SetsSelectAllTrue()
        {
            var contactViewModel1 = new ContactViewModel { Model = new ContactInfoPresenter() };
            var contactViewModel2 = new ContactViewModel { Model = new ContactInfoPresenter() };
            _subject.Items.Add(contactViewModel1);
            _subject.Items.Add(contactViewModel2);

            _subject.SelectAll.Should().BeFalse();
            contactViewModel1.IsSelected = true;
            _subject.SelectAll.Should().BeFalse();
            contactViewModel2.IsSelected = true;
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
            var testScheduler = new TestScheduler();
            var contactObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<ContactList>>(100, Notification.CreateOnNext(contactList)),
                new Recorded<Notification<ContactList>>(200, Notification.CreateOnCompleted<ContactList>()));

            _mockCommandService.Setup(m => m.Execute(It.IsAny<SyncContactsCommand>())).Returns(contactObservable);

            _subject.Start();
            ((IActivate)_subject).Activate();
            testScheduler.Start(WaitWhileBusy(testScheduler));

            _subject.Items.Count.Should().Be(2);
        }

        private Func<IObservable<EventPattern<object>>> WaitWhileBusy(TestScheduler testScheduler)
        {
            return () =>
                   Observable.FromEventPattern(_subject, "PropertyChanged", testScheduler)
                       .Where(e => ((PropertyChangedEventArgs)e.EventArgs).PropertyName == "IsBusy")
                       .Take(2);
        }

        private void AddChildViewModel(ContactInfoPresenter model1)
        {
            _subject.Items.Add(new ContactViewModel { Model = model1 });
        }

        private IContactViewModel ViewModelWithModel(ContactInfoPresenter model1)
        {
            return _subject.Items.Single(item => item.Model == model1);
        }
    }
}