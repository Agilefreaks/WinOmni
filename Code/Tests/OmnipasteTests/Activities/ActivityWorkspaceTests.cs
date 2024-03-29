﻿namespace OmnipasteTests.Activities
{
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Activities;
    using Omnipaste.Activities.ActivityList;
    using OmniUI.Workspaces;

    [TestFixture]
    public class ActivityWorkspaceTests
    {
        private ActivityWorkspace _subject;

        private Mock<IDetailsConductorViewModel> _mockDetailConductor;

        private Mock<IActivityListViewModel> _mockActivityList;

        [SetUp]
        public void Setup()
        {
            _mockDetailConductor = new Mock<IDetailsConductorViewModel>();
            _mockActivityList = new Mock<IActivityListViewModel>();
            _subject = new ActivityWorkspace(_mockActivityList.Object, _mockDetailConductor.Object);
        }

        [Test]
        public void Activate_Always_ActivatesBotheTheListViewModelAndTheDetailsConductor()
        {
            ((IActivate)_subject).Activate();

            _mockActivityList.Verify(x => x.Activate(), Times.Once());
            _mockDetailConductor.Verify(x => x.Activate(), Times.Once());
        }
    }
}