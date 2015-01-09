﻿namespace OmnipasteTests.ActivityList
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reactive;
    using System.Threading;
    using Caliburn.Micro;
    using Events.Handlers;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Activity;
    using Omnipaste.ActivityList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ActivityListViewModelTests
    {
        private ActivityListViewModel _subject;

        private Mock<IEventsHandler> _mockEventsHandler;

        private Mock<IUpdaterService> _mockUpdateService;

        private Mock<IActivityViewModelFactory> _mockActivityViewModelFactory;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IClippingRepository> _mockClippingRepository;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockEventsHandler = new Mock<IEventsHandler> { DefaultValue = DefaultValue.Mock};
            _mockUpdateService = new Mock<IUpdaterService> { DefaultValue = DefaultValue.Mock };
            _mockActivityViewModelFactory = new Mock<IActivityViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new ActivityListViewModel(
                _mockClippingRepository.Object,
                _mockEventsHandler.Object,
                _mockActivityViewModelFactory.Object,
                _mockUpdateService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void ReceivingAClipping_AfterStart_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Save, new ClippingModel()))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(200,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityPresenter>())).Returns<ActivityPresenter>(presenter => new ActivityViewModel(_mockUiRefreshService.Object) { Model = presenter });
            var viewModel = new ActivityListViewModel(
                _mockClippingRepository.Object,
                _mockEventsHandler.Object,
                _mockActivityViewModelFactory.Object,
                _mockUpdateService.Object);
            viewModel.Start();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            viewModel.Items.Count.Should().Be(1);
        }

        [Test]
        public void RemovingAClipping_WhenClippingWasPreviouslyReceived_RemovesViewModelForClipping()
        {
            const string SourceId = "42";
            var clippingModel = new ClippingModel { UniqueId = SourceId };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(100, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Save, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(200, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Delete, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(300,Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityPresenter>())).Returns<ActivityPresenter>(presenter => new ActivityViewModel(_mockUiRefreshService.Object) { Model = presenter });
            var viewModel = new ActivityListViewModel(
                _mockClippingRepository.Object,
                _mockEventsHandler.Object,
                _mockActivityViewModelFactory.Object,
                _mockUpdateService.Object);
            viewModel.Start();
            _testScheduler.Start();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

            viewModel.Items.Count.Should().Be(0);
        }

        [Test]
        public void ReceivingAnEvent_AfterStart_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var eventObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Event>>(100, Notification.CreateOnNext(new Event())),
                    new Recorded<Notification<Event>>(200, Notification.CreateOnCompleted<Event>()));
            _mockEventsHandler.Setup(x => x.Subscribe(It.IsAny<IObserver<Event>>()))
                .Callback<IObserver<Event>>(observer => eventObservable.Subscribe(observer));
            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityPresenter>())).Returns<ActivityPresenter>(presenter => new ActivityViewModel(_mockUiRefreshService.Object) { Model = presenter });
            var viewModel = new ActivityListViewModel(
                _mockClippingRepository.Object,
                _mockEventsHandler.Object,
                _mockActivityViewModelFactory.Object,
                _mockUpdateService.Object);
            viewModel.Start();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            viewModel.Items.Count.Should().Be(1);
        }

        [Test]
        public void ReceivingAnUpdate_AfterStart_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var eventObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UpdateInfo>>(100, Notification.CreateOnNext(new UpdateInfo())),
                    new Recorded<Notification<UpdateInfo>>(200, Notification.CreateOnCompleted<UpdateInfo>()));
            _mockUpdateService.SetupGet(x => x.UpdateObservable).Returns(eventObservable);
            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityPresenter>())).Returns<ActivityPresenter>(presenter => new ActivityViewModel(_mockUiRefreshService.Object) { Model = presenter });
            var viewModel = new ActivityListViewModel(
                _mockClippingRepository.Object,
                _mockEventsHandler.Object,
                _mockActivityViewModelFactory.Object,
                _mockUpdateService.Object);
            viewModel.Start();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            viewModel.Items.Count.Should().Be(1);
        }

        [Test]
        public void OnlyShowClippingsTrue_Always_FiltersItemsSoThatOnlyClippingsRemain()
        {
            AddItemsForAllActivityTypes();

            _subject.ShowClippings = true;
            _subject.ShowCalls = false;
            _subject.ShowMessages = false;

            var filteredItems = _subject.FilteredItems.Cast<IActivityViewModel>().ToList();
            filteredItems.Count.Should().Be(1);
            filteredItems[0].Model.Type.Should().Be(ActivityTypeEnum.Clipping);
        }

        [Test]
        public void OnlyShowCallsTrue_Always_FiltersItemsSoThatOnlyCallsRemain()
        {
            AddItemsForAllActivityTypes();

            _subject.ShowClippings = false;
            _subject.ShowCalls = true;
            _subject.ShowMessages = false;

            var filteredItems = _subject.FilteredItems.Cast<IActivityViewModel>().ToList();
            filteredItems.Count.Should().Be(1);
            filteredItems[0].Model.Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void OnlyShowMessagesTrue_Always_FiltersItemsSoThatOnlyMessagesRemain()
        {
            AddItemsForAllActivityTypes();

            _subject.ShowClippings = false;
            _subject.ShowCalls = false;
            _subject.ShowMessages = true;

            var filteredItems = _subject.FilteredItems.Cast<IActivityViewModel>().ToList();
            filteredItems.Count.Should().Be(1);
            filteredItems[0].Model.Type.Should().Be(ActivityTypeEnum.Message);
        }

        [Test]
        public void AllFilterOptionsAreFalse_Always_FiltersItemsSoThatOnyOnesWithAValidTypeRemain()
        {
            AddItemsForAllActivityTypes();

            _subject.ShowClippings = false;
            _subject.ShowCalls = false;
            _subject.ShowMessages = false;

            var filteredItems = _subject.FilteredItems.Cast<IActivityViewModel>().ToList();

            var values = Enum.GetValues(typeof(ActivityTypeEnum)).Cast<ActivityTypeEnum>().Where(type => type != ActivityTypeEnum.All && type != ActivityTypeEnum.None).ToList();
            filteredItems.Count.Should().Be(values.Count);
            filteredItems.ForEach(vm => { values.Should().Contain(vm.Model.Type); });
        }

        [Test]
        public void MoreThanOneFilterIsTrue_Always_FiltersItemsSoThatOnyOnesWithACorrespondingTypeRemain()
        {
            AddItemsForAllActivityTypes();

            _subject.ShowClippings = true;
            _subject.ShowCalls = false;
            _subject.ShowMessages = true;

            var filteredItems = _subject.FilteredItems.Cast<IActivityViewModel>().ToList();
            filteredItems.Count.Should().Be(2);
            filteredItems[0].Model.Type.Should().Be(ActivityTypeEnum.Clipping);
            filteredItems[1].Model.Type.Should().Be(ActivityTypeEnum.Message);
	
        }

        [Test]
        public void ChangingFilterText_Always_UpdatesFilteredItemsSoAsToShowOnlyItemsWhoseModelsHaveTheFilterTextInTheirStringRepresentation()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
            var activityPresenter1 = new ActivityPresenter(new Activity(new Event { Type = EventTypeEnum.IncomingCallEvent}));
            var activityPresenter2 = new ActivityPresenter(new Activity(new Event { Type = EventTypeEnum.IncomingSmsEvent}));
            ((IActivate)_subject).Activate();
            _subject.ActivateItem(new ActivityViewModel(_mockUiRefreshService.Object) { Model = activityPresenter1 });
            _subject.ActivateItem(new ActivityViewModel(_mockUiRefreshService.Object) { Model = activityPresenter2 });

            _subject.FilterText = "sms de la";

            _subject.FilteredItems.Count.Should().Be(1);
            _subject.FilteredItems.Cast<IActivityViewModel>().First().Model.Should().Be(activityPresenter2);
        }

        [Test]
        public void ChangingFilterText_TextIsMadeUpOfMultipleWords_UpdatesFilteredItemsSoAsToShowOnlyItemsWhoseModelsHaveAllTheWordsInTheFilterTextInTheirStringRepresentation()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
            var activityPresenter1 = new ActivityPresenter(new Activity(new Event { Type = EventTypeEnum.IncomingSmsEvent, ContactName = "John Doe" }));
            var activityPresenter2 = new ActivityPresenter(new Activity(new Event { Type = EventTypeEnum.IncomingSmsEvent, ContactName = "Jane Doe" }));
            ((IActivate)_subject).Activate();
            _subject.ActivateItem(new ActivityViewModel(_mockUiRefreshService.Object) { Model = activityPresenter1 });
            _subject.ActivateItem(new ActivityViewModel(_mockUiRefreshService.Object) { Model = activityPresenter2 });

            _subject.FilterText = "JAnE DoE sms";

            _subject.FilteredItems.Count.Should().Be(1);
            _subject.FilteredItems.Cast<IActivityViewModel>().First().Model.Should().Be(activityPresenter2);
        }

        private void AddItemsForAllActivityTypes()
        {
            Enum.GetValues(typeof(ActivityTypeEnum))
                .Cast<ActivityTypeEnum>()
                .ToList()
                .ForEach(
                    activityType =>
                    _subject.Items.Add(
                        new ActivityViewModel(_mockUiRefreshService.Object) { Model = new ActivityPresenter(new Activity(activityType)) }));
        }
    }
}