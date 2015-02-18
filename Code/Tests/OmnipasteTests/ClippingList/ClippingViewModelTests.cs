﻿namespace OmnipasteTests.ClippingList
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ClippingList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Workspace;

    [TestFixture]
    public class ClippingViewModelTests
    {
        private ClippingViewModel _subject;

        private ClippingPresenter _clippingPresenter;

        private Mock<ISessionManager> _mockSessionManager;

        private Mock<IWorkspaceDetailsViewModelFactory> _mockDetailsViewModelFactory;

        private Mock<IClippingRepository> _mockClippingRepository;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockDetailsViewModelFactory = new Mock<IWorkspaceDetailsViewModelFactory>
                                               {
                                                   DefaultValue =
                                                       DefaultValue.Mock
                                               };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _clippingPresenter = new ClippingPresenter(new ClippingModel { Content = "Test" });
            _subject = new ClippingViewModel(_mockSessionManager.Object)
                           {
                               Model = _clippingPresenter,
                               DetailsViewModelFactory = _mockDetailsViewModelFactory.Object,
                               ClippingRepository = _mockClippingRepository.Object
                           };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void IsSelected_WhenSessionSelectedClippingIdIsSameAsModelId_ReturnsTrue()
        {
            _mockSessionManager.SetupGet(m => m[ClippingViewModel.SessionSelectionKey])
                .Returns(_clippingPresenter.UniqueId);

            _subject.IsSelected.Should().BeTrue();
        }

        [Test]
        public void IsSelected_WhenSessionSelectedClippingIdIsNotSameAsModelId_ReturnsFalse()
        {
            _mockSessionManager.SetupGet(m => m[ClippingViewModel.SessionSelectionKey])
                .Returns("other");

            _subject.IsSelected.Should().BeFalse();
        }

        [Test]
        public void OnIsStarredChanged_AfterActivate_UpdatesModel()
        {
            ((IActivate)_subject).Activate();

            _clippingPresenter.IsStarred = true;

            _clippingPresenter.BackingModel.IsStarred.Should().BeTrue();
        }

        [Test]
        public void OnIsStarredChanged_AfterActivate_SavesModel()
        {
            ((IActivate)_subject).Activate();

            _clippingPresenter.IsStarred = true;

            _mockClippingRepository.Verify(m => m.Save(_clippingPresenter.BackingModel));
        }

        [Test]
        public void OnIsStarredChanged_AfterDeactivate_DoesNotSaveModel()
        {
            ((IActivate)_subject).Activate();
            ((IDeactivate)_subject).Deactivate(false);

            _clippingPresenter.IsStarred = false;

            _mockClippingRepository.Verify(m => m.Save(It.IsAny<ClippingModel>()), Times.Never());
        }

        [Test]
        public void ShowDetails_Always_ActivatesAnActivityDetailsViewModelInItsParentActivityWorkspace()
        {
            var mockWorkspace = new Mock<IClippingWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            mockDetailsConductor.Verify(x => x.ActivateItem(It.IsAny<IWorkspaceDetailsViewModel>()), Times.Once());
        }

        [Test]
        public void ShowDetails_Always_StoresSelectedClippingIdInSession()
        {
            var mockWorkspace = new Mock<IClippingWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _mockSessionManager.VerifySet(m => m[ClippingViewModel.SessionSelectionKey] = _clippingPresenter.UniqueId);
        }
    }
}
