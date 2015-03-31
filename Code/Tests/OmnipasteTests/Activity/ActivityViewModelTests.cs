namespace OmnipasteTests.Activity
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Activity;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces.Activity;
    using OmniUI.Workspace;

    [TestFixture]
    public class ActivityViewModelTests
    {
        private ActivityViewModel _subject;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private TestScheduler _testScheduler;

        private Mock<IWorkspaceDetailsViewModelFactory> _mockDetailsViewModelFactory;

        private Mock<ISessionManager> _mockSessionManager;

        private Mock<IContactRepository> _mockContactRepository;

        private ActivityModelFactory _activityModelFactory;

        [SetUp]
        public void Setup()
        {
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockContactRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(Observable.Return(new ContactEntity()));
            _mockDetailsViewModelFactory = new Mock<IWorkspaceDetailsViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _activityModelFactory = new ActivityModelFactory(_mockContactRepository.Object);

            _subject = new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object)
                           {
                               DetailsViewModelFactory = _mockDetailsViewModelFactory.Object
                           };
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
        public void ActivityType_Always_ReturnTheModelType()
        {
            _subject.Model = _activityModelFactory.Create(new ClippingEntity()).Wait();

            _subject.ActivityType.Should().Be(_subject.Model.Type);
        }

        [Test]
        public void IsSelected_WhenValueIsCurrentModelUniqueId_ReturnsTrue()
        {
            _subject.Model = _activityModelFactory.Create(new LocalPhoneCallEntity()).Wait();
            _mockSessionManager.SetupGet(m => m[ActivityViewModel.SessionSelectionKey]).Returns(_subject.Model.BackingEntity.UniqueId);

            _subject.IsSelected.Should().BeTrue();
        }

        [Test]
        public void IsSelected_WhenValueIsOtherThanCurrentModelUniqueId_ReturnsFalse()
        {
            _subject.Model = _activityModelFactory.Create(new LocalPhoneCallEntity()).Wait();
            _mockSessionManager.SetupGet(m => m[ActivityViewModel.SessionSelectionKey]).Returns("other");

            _subject.IsSelected.Should().BeFalse();
        }

        [Test]
        public void OnRefreshSignal_ViewModelWasActivated_RaisesNotifyPropertyChangedOnTheModel()
        {
            var refreshUiObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(100, Notification.CreateOnNext(new Unit())));
            _mockUiRefreshService.Setup(x => x.RefreshObservable).Returns(refreshUiObservable);
            ((IActivate)_subject).Activate();
            var callCount = 0;
            _subject.PropertyChanged += (sender, eventArgs) =>
                { if (eventArgs.PropertyName == "Model") callCount++; };

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            callCount.Should().Be(1);
        }

        [Test]
        public void OnRefreshSignal_ViewModelWasActivatedButThenDeactivatedWithCloseTrue_DoesNotRaiseNotifyPropertyChangedOnTheModel()
        {
            var refreshUiObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(100, Notification.CreateOnNext(new Unit())));
            _mockUiRefreshService.Setup(x => x.RefreshObservable).Returns(refreshUiObservable);
            ((IActivate)_subject).Activate();
            ((IDeactivate)_subject).Deactivate(true);
            var callCount = 0;
            _subject.PropertyChanged += (sender, eventArgs) =>
                { if (eventArgs.PropertyName == "Model") callCount++; };

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            callCount.Should().Be(0);
        }

        [Test]
        public void ShowDetails_Always_ActivatesAnActivityDetailsViewModelInItsParentActivityWorkspace()
        {
            _subject.Model = _activityModelFactory.Create(new LocalPhoneCallEntity()).Wait();
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            mockDetailsConductor.Verify(x => x.ActivateItem(It.IsAny<IWorkspaceDetailsViewModel>()), Times.Once());
        }

        [Test]
        public void ShowDetails_Always_SavesCurrentItemAsSelection()
        {
            _subject.Model = _activityModelFactory.Create(new LocalPhoneCallEntity()).Wait();
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _mockSessionManager.VerifySet(m => m[ActivityViewModel.SessionSelectionKey] = _subject.Model.BackingEntity.UniqueId);
        }

        [Test]
        public void ShowDetails_WhenDetailsIsActive_SetsContentInfoStateToViewing()
        {
            var activity = _activityModelFactory.Create(new ClippingEntity { WasViewed = true }).Wait();
            _subject.Model = activity;
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            var mockActivityDetailsViewModel = new Mock<IWorkspaceDetailsViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityModel>()))
                .Returns(mockActivityDetailsViewModel.Object);
            _mockSessionManager.SetupGet(m => m[ActivityViewModel.SessionSelectionKey])
                .Returns(activity.BackingEntity.UniqueId);

            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.ContentInfo.ContentState.Should().Be(ContentStateEnum.Viewing);
        }

        [Test]
        public void ShowDetails_WhenDetailsIsNotActiveAndModelWasViewed_SetsContentInfoStateToViewing()
        {
            var activity = _activityModelFactory.Create(new ClippingEntity { WasViewed = true }).Wait();
            _subject.Model = activity;
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            var mockActivityDetailsViewModel = new Mock<IWorkspaceDetailsViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityModel>()))
                .Returns(mockActivityDetailsViewModel.Object);
            _mockSessionManager.SetupGet(m => m[ActivityViewModel.SessionSelectionKey])
                .Returns("other");

            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.ContentInfo.ContentState.Should().Be(ContentStateEnum.Viewed);
        }

        [Test]
        public void ShowDetails_WhenDetailsIsNotActiveAndModelWasNotViewed_SetsContentInfoStateToViewing()
        {
            var activity = _activityModelFactory.Create(new ClippingEntity { DeviceId = "42", WasViewed = false }).Wait();
            _subject.Model = activity;
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            var mockActivityDetailsViewModel = new Mock<IWorkspaceDetailsViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityModel>()))
                .Returns(mockActivityDetailsViewModel.Object);
            mockActivityDetailsViewModel.SetupGet(x => x.IsActive).Returns(false);

            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.ContentInfo.ContentState.Should().Be(ContentStateEnum.NotViewed);
        }

        [Test]
        public void ShowDetails_WhenModelIsClipping_SetsContentInfoTypeToClipping()
        {
            var activity = _activityModelFactory.Create(new ClippingEntity { DeviceId = "42", WasViewed = false }).Wait();
            _subject.Model = activity;
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            var mockActivityDetailsViewModel = new Mock<IWorkspaceDetailsViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityModel>()))
                .Returns(mockActivityDetailsViewModel.Object);
            mockActivityDetailsViewModel.SetupGet(x => x.IsActive).Returns(false);

            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.ContentInfo.ContentState.Should().Be(ContentStateEnum.NotViewed);
        }
    }
}