namespace OmnipasteTests.Framework.Commands
{
    using System;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Shell;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.WorkspaceDetails.Conversation;
    using Omnipaste.Workspaces;
    using OmniUI.Workspace;

    [TestFixture]
    public class ComposeSMSCommandTests
    {
        private ComposeSMSCommand _subject;

        private IContactInfoPresenter _contactInfo;

        private Mock<IWorkspaceConductor> _mockWorkspaceConductor;

        private Mock<IPeopleWorkspace> _mockPeopleWorkspace;

        private Mock<IWorkspaceDetailsViewModelFactory> _mockDetailsViewModelFactory;

        private TestScheduler _testScheduler;

        private Mock<IShellViewModel> _mockShellViewModel;

        [SetUp]
        public void Setup()
        {
            _contactInfo = new ContactInfoPresenter(new ContactInfo());
            _mockWorkspaceConductor = new Mock<IWorkspaceConductor>();
            _mockPeopleWorkspace = new Mock<IPeopleWorkspace> { DefaultValue = DefaultValue.Mock };
            _mockDetailsViewModelFactory = new Mock<IWorkspaceDetailsViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockShellViewModel = new Mock<IShellViewModel>();
            _subject = new ComposeSMSCommand(_contactInfo)
                           {
                               WorkspaceConductor = _mockWorkspaceConductor.Object,
                               PeopleWorkspace = _mockPeopleWorkspace.Object,
                               DetailsViewModelFactory = _mockDetailsViewModelFactory.Object,
                               ShellViewModel = _mockShellViewModel.Object
                           };
            _testScheduler = new TestScheduler();
            SchedulerProvider.Dispatcher = _testScheduler;
        }

        [Test]
        public void Execute_Always_ShowsTheShellViewModel()
        {
            _subject.Execute().Subscribe(_ => { });
            _testScheduler.Start();

            _mockShellViewModel.Verify(x => x.Show(), Times.Once());
        }

        [Test]
        public void Execute_Always_ActivatesThePeopleWorkspaceInTheWorkspaceConductor()
        {
            _subject.Execute().Subscribe(_ => {});
            _testScheduler.Start();

            _mockWorkspaceConductor.Verify(x => x.ActivateItem(_mockPeopleWorkspace.Object));
        }

        [Test]
        public void Execute_Always_CreatesAWorkspaceDetailsViewModelForTheGivenContactInfo()
        {
            _subject.Execute().Subscribe(_ => {});
            _testScheduler.Start();

            _mockDetailsViewModelFactory.Verify(x => x.Create(_contactInfo), Times.Once());
        }

        [Test]
        public void Execute_Always_ActivatesTheObtainedWorkspaceDetailsViewModelInThePeopleWorkspace()
        {
            var mockConversationViewModel = new Mock<IConversationViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(_contactInfo)).Returns(mockConversationViewModel.Object);
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            _mockPeopleWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);

            _subject.Execute().Subscribe(_ => {});
            _testScheduler.Start();

            mockDetailsConductor.Verify(x => x.ActivateItem(mockConversationViewModel.Object), Times.Once());
        }
    }
}