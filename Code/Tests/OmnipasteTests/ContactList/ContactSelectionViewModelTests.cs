namespace OmnipasteTests.ContactList
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ContactList;
    using Omnipaste.GroupMessage.ContactSelection;
    using Omnipaste.GroupMessage.ContactSelection.ContactInfo;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    [TestFixture]
    public class ContactSelectionViewModelTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IConversationProvider> _mockConversationProvider;

        private Mock<IContactInfoViewModelFactory> _mockContactInfoViewModelFactory;

        private IContactSelectionViewModel _subject;

        private TestScheduler _testScheduler;

        private Mock<ISessionManager> _mockSessionManager;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockConversationProvider = new Mock<IConversationProvider> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager.SetupAllProperties();
            _mockContactInfoViewModelFactory = new Mock<IContactInfoViewModelFactory>();
            _mockContactInfoViewModelFactory.Setup(
                x => x.Create<IContactInfoSelectionViewModel>(It.IsAny<ContactInfoPresenter>()))
                .Returns<ContactInfoPresenter>(
                    presenter => new ContactInfoSelectionViewModel(_mockSessionManager.Object) { Model = presenter });

            MoqMockingKernel kernel = new MoqMockingKernel();
            kernel.Bind<IContactRepository>().ToConstant(_mockContactRepository.Object);
            kernel.Bind<IConversationProvider>().ToConstant(_mockConversationProvider.Object);
            kernel.Bind<IContactInfoViewModelFactory>().ToConstant(_mockContactInfoViewModelFactory.Object);

            kernel.Bind<IContactSelectionViewModel>().To<ContactSelectionViewModel>();
            _subject = kernel.Get<IContactSelectionViewModel>();

            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void FilterItems_WhenEmpty_SetsStatusToEmptyFilter()
        {
            _subject.FilterText = "1234";

            _subject.Status.Should().Be(ListViewModelStatusEnum.EmptyFilter);
        }

        [Test]
        public void FilterItems_WhenThereAreItemsButNoneOnTheFilterList_SetsStatusToEmptyFilter()
        {
            var contactInfo = new ContactInfo { FirstName = "asdf" };
            var contactInfos = new List<ContactInfo>() {contactInfo};
            var observable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<IEnumerable<ContactInfo>>>(
                    100,
                    Notification.CreateOnNext(contactInfos.AsEnumerable())));
            _mockContactRepository.Setup(cr => cr.GetAll()).Returns(observable);
            _subject.Activate();
            _testScheduler.Start();

            _subject.FilterText = "1234";

            _subject.Status.Should().Be(ListViewModelStatusEnum.EmptyFilter);
        }

        [Test]
        public void FilterItems_WhenThereAreItemsButThatMatchTheFilter_SetsStatusToNotEmpty()
        {
            var contactInfo = new ContactInfo { FirstName = "asdf" };
            var contactInfos = new List<ContactInfo>() {contactInfo};
            var observable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<IEnumerable<ContactInfo>>>(
                    100,
                    Notification.CreateOnNext(contactInfos.AsEnumerable())));
            _mockContactRepository.Setup(cr => cr.GetAll()).Returns(observable);
            _subject.Activate();
            _testScheduler.Start();

            _subject.FilterText = "sd";

            _subject.Status.Should().Be(ListViewModelStatusEnum.NotEmpty);
        }
        
        [Test]
        public void FilterText_IsPhoneNumber_IsSetOnPendingContactAsPhoneNumber()
        {
            _subject.FilterText = "1234";

            _subject.PendingContact.Identifier.Should().Be(_subject.FilterText);
        }
    }
}