﻿namespace OmnipasteTests.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ContactList;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Shell;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Workspace;

    [TestFixture]
    public class ComposeSMSCommandTests
    {
        private ComposeSMSCommand _subject;

        private ContactInfo _contactInfo;

        private Mock<IWorkspaceConductor> _mockWorkspaceConductor;

        private Mock<IPeopleWorkspace> _mockPeopleWorkspace;

        private Mock<IWorkspaceDetailsViewModelFactory> _mockDetailsViewModelFactory;

        private Mock<IShellViewModel> _mockShellViewModel;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _contactInfo = new ContactInfo();
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
            SchedulerProvider.Default = _testScheduler;
        }

        [Test]
        public void Execute_Always_ShowsTheShellViewModel()
        {
            _testScheduler.Start(_subject.Execute);

            _mockShellViewModel.Verify(x => x.Show(), Times.Once());
        }

        [Test]
        public void Execute_Always_ActivatesThePeopleWorkspaceInTheWorkspaceConductor()
        {
            _testScheduler.Start(_subject.Execute);

            _mockWorkspaceConductor.Verify(x => x.ActivateItem(_mockPeopleWorkspace.Object));
        }

        [Test]
        public void Execute_ACorrespondingContactViewModelsExistsForTheGivenContact_ShowsDetailsForThatViewModel()
        {
            var mockContactInfoViewModel = new Mock<IContactInfoViewModel>();
            mockContactInfoViewModel.SetupGet(x => x.Model).Returns(new ContactInfoPresenter(_contactInfo));
            var mockContactListViewModel = new Mock<IContactListViewModel>();
            var contactInfoViewModels = new List<IContactInfoViewModel> { mockContactInfoViewModel.Object };
            mockContactListViewModel.Setup(x => x.GetChildren()).Returns(contactInfoViewModels);
            _mockPeopleWorkspace.SetupGet(x => x.MasterScreen).Returns(mockContactListViewModel.Object);

            _testScheduler.Start(_subject.Execute);

            mockContactInfoViewModel.Verify(x => x.ShowDetails(), Times.Once());
        }
        
        [Test]
        public void Execute_ACorrespondingContactViewModelsDoesNotExistForTheGivenContact_RetriesUntilOneExistsAndShowsDetailsForThatViewModel()
        {
            var mockContactInfoViewModel = new Mock<IContactInfoViewModel>();
            mockContactInfoViewModel.SetupGet(x => x.Model).Returns(new ContactInfoPresenter(_contactInfo));
            var contactInfoViewModels = new List<IContactInfoViewModel> { mockContactInfoViewModel.Object };
            var getContactsCallcount = 0;
            var mockContactListViewModel = new Mock<IContactListViewModel>();
            mockContactListViewModel.Setup(x => x.GetChildren())
                .Returns(() => getContactsCallcount++ == 0 ? new List<IContactInfoViewModel>() : contactInfoViewModels);
            _mockPeopleWorkspace.SetupGet(x => x.MasterScreen).Returns(mockContactListViewModel.Object);

            _testScheduler.Start(_subject.Execute, TimeSpan.FromSeconds(1).Ticks);

            getContactsCallcount.Should().Be(2);
            mockContactInfoViewModel.Verify(x => x.ShowDetails(), Times.Once());
        }
    }
}