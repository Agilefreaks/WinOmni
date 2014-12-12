namespace OmnipasteTests.Activity
{
    using System;
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Activity;
    using Omnipaste.ActivityDetails;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using Omnipaste.Workspaces;

    [TestFixture]
    public class ActivityViewModelTests
    {
        private ActivityViewModel _subject;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private TestScheduler _testScheduler;

        private Mock<IActivityDetailsViewModelFactory> _mockDetailsViewModelFactory;

        [SetUp]
        public void Setup()
        {
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockDetailsViewModelFactory = new Mock<IActivityDetailsViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _subject = new ActivityViewModel(_mockUiRefreshService.Object)
                           {
                               DetailsViewModelFactory = _mockDetailsViewModelFactory.Object
                           };
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void OnRefreshSignal_ViewModelWasActivated_RaisesNotifyPropertyChangedOnTheModel()
        {
            var refreshUiObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(100, Notification.CreateOnNext(new Unit())));
            _mockUiRefreshService.Setup(x => x.RefreshObservable).Returns(refreshUiObservable);
            var callCount = 0;
            _subject.PropertyChanged += (sender, eventArgs) =>
                { if (eventArgs.PropertyName == "Model") callCount++; };

            ((IActivate)_subject).Activate();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            callCount.Should().Be(1);
        }

        [Test]
        public void OnRefreshSignal_ViewModelWasActivatedButThenDeactivatedWithCloseTrue_DoesNotRaiseNotifyPropertyChangedOnTheModel()
        {
            var refreshUiObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(100, Notification.CreateOnNext(new Unit())));
            _mockUiRefreshService.Setup(x => x.RefreshObservable).Returns(refreshUiObservable);
            var callCount = 0;
            _subject.PropertyChanged += (sender, eventArgs) =>
                { if (eventArgs.PropertyName == "Model") callCount++; };

            ((IActivate)_subject).Activate();
            ((IDeactivate)_subject).Deactivate(true);
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            callCount.Should().Be(0);
        }

        [Test]
        public void ShowDetails_Always_ActivatesAnActivityDetailsViewModelInItsParentActivityWorkspace()
        {
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            mockDetailsConductor.Verify(x => x.ActivateItem(It.IsAny<IActivityDetailsViewModel>()), Times.Once());
        }

        [Test]
        public void ShowDetails_Always_SetsModelWasViewedToTrue()
        {
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.Model.WasViewed.Should().BeTrue();
        }

        [Test]
        public void ShowDetails_WhenDetailsIsActive_SetsContentInfoStateToViewing()
        {
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            var mockActivityDetailsViewModel = new Mock<IActivityDetailsViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<Activity>()))
                .Returns(mockActivityDetailsViewModel.Object);
            mockActivityDetailsViewModel.SetupGet(x => x.IsActive).Returns(true);

            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.ContentInfo.ContentState.Should().Be(ContentStateEnum.Viewing);
        }

        [Test]
        public void ShowDetails_WhenDetailsIsNotActiveAndModelWasViewed_SetsContentInfoStateToViewing()
        {
            var activity = new Activity { WasViewed = true };
            _subject.Model = activity;
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            var mockActivityDetailsViewModel = new Mock<IActivityDetailsViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<Activity>()))
                .Returns(mockActivityDetailsViewModel.Object);
            mockActivityDetailsViewModel.SetupGet(x => x.IsActive).Returns(false);

            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.ContentInfo.ContentState.Should().Be(ContentStateEnum.Viewed);
        }

        [Test]
        public void ShowDetails_WhenDetailsIsNotActiveAndModelWasNotViewed_SetsContentInfoStateToViewing()
        {
            var activity = new Activity { WasViewed = false };
            _subject.Model = activity;
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            var mockActivityDetailsViewModel = new Mock<IActivityDetailsViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<Activity>()))
                .Returns(mockActivityDetailsViewModel.Object);
            mockActivityDetailsViewModel.SetupGet(x => x.IsActive).Returns(false);

            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.ContentInfo.ContentState.Should().Be(ContentStateEnum.NotViewed);
        }

        [Test]
        public void ShowDetails_WhenModelIsClipping_SetsContentInfoTypeToClipping()
        {
            var activity = new Activity { WasViewed = false };
            _subject.Model = activity;
            var mockWorkspace = new Mock<IActivityWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            var mockActivityDetailsViewModel = new Mock<IActivityDetailsViewModel>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<Activity>()))
                .Returns(mockActivityDetailsViewModel.Object);
            mockActivityDetailsViewModel.SetupGet(x => x.IsActive).Returns(false);

            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            _subject.ContentInfo.ContentState.Should().Be(ContentStateEnum.NotViewed);
        }
    }
}