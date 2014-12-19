namespace OmniHolidaysTests.MessagesWorkspace.ContactList
{
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Windows.Media;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniUI.Helpers;
    using OmniUI.List;
    using OmniUI.Presenters;
    using OmniUI.Services;

    [TestFixture]
    public class ContactListContentViewModelTests
    {
        private ContactListContentViewModel _subject;

        private Subject<IContactInfoPresenter> _contactInfoPresenterSource;

        private IKernel _mockKernel;

        private Mock<IApplicationHelper> _mockApplicationHelper;

        private Mock<ICommandService> _mockCommandService;

        [SetUp]
        public void Setup()
        {
            _contactInfoPresenterSource = new Subject<IContactInfoPresenter>();
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
            _contactInfoPresenterSource.OnNext(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            _contactInfoPresenterSource.OnNext(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            _contactInfoPresenterSource.OnNext(model3);

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
            _contactInfoPresenterSource.OnNext(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            _contactInfoPresenterSource.OnNext(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            _contactInfoPresenterSource.OnNext(model3);
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
            _contactInfoPresenterSource.OnNext(model1);
            var model2 = new ContactInfoPresenter { Identifier = identifiers[counter++ % identifiers.Length] };
            _contactInfoPresenterSource.OnNext(model2);
            var model3 = new ContactInfoPresenter { Identifier = identifiers[counter % identifiers.Length] };
            _contactInfoPresenterSource.OnNext(model3);
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

        private IContactViewModel ViewModelWithModel(ContactInfoPresenter model1)
        {
            return _subject.Items.Single(item => item.Model == model1);
        }
    }
}