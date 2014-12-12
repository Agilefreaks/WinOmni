﻿namespace OmnipasteTests.ActivityList
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Events.Handlers;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Activity;
    using Omnipaste.ActivityList;
    using Omnipaste.Models;

    [TestFixture]
    public class ActivityListViewModelTests
    {
        private ActivityListViewModel _subject;

        private Mock<IEventsHandler> _mockEventsHandler;

        private Mock<IClipboardHandler> _mockClipboardHandler;

        private Mock<IActivityViewModelFactory> _mockActivityViewModelFactory;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _mockEventsHandler = new Mock<IEventsHandler> { DefaultValue = DefaultValue.Mock};
            _mockClipboardHandler = new Mock<IClipboardHandler> { DefaultValue = DefaultValue.Mock };
            _mockActivityViewModelFactory = new Mock<IActivityViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _subject = new ActivityListViewModel(_mockClipboardHandler.Object, _mockEventsHandler.Object, _mockActivityViewModelFactory.Object);
            _testScheduler = new TestScheduler();
        }

        [Test]
        public void ReceivingAClipping_Always_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var clippingObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Clipping>>(100, Notification.CreateOnNext(new Clipping())),
                    new Recorded<Notification<Clipping>>(200, Notification.CreateOnCompleted<Clipping>()));
            _mockClipboardHandler.Setup(x => x.Subscribe(It.IsAny<IObserver<Clipping>>()))
                .Callback<IObserver<Clipping>>(observer => clippingObservable.Subscribe(observer));
            var activityViewModel = new ActivityViewModel { Model = new Activity() };
            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<Activity>())).Returns(activityViewModel);
            var viewModel = new ActivityListViewModel(
                _mockClipboardHandler.Object,
                _mockEventsHandler.Object,
                _mockActivityViewModelFactory.Object);

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            viewModel.Items.Count.Should().Be(1);
            viewModel.Items[0].Should().Be(activityViewModel);
            
        }

        [Test]
        public void ReceivingAnEvent_Always_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var eventObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Event>>(100, Notification.CreateOnNext(new Event())),
                    new Recorded<Notification<Event>>(200, Notification.CreateOnCompleted<Event>()));
            _mockEventsHandler.Setup(x => x.Subscribe(It.IsAny<IObserver<Event>>()))
                .Callback<IObserver<Event>>(observer => eventObservable.Subscribe(observer));
            var activityViewModel = new ActivityViewModel { Model = new Activity() };
            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<Activity>())).Returns(activityViewModel);
            var viewModel = new ActivityListViewModel(
                _mockClipboardHandler.Object,
                _mockEventsHandler.Object,
                _mockActivityViewModelFactory.Object);

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            viewModel.Items.Count.Should().Be(1);
            viewModel.Items[0].Should().Be(activityViewModel);
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
            filteredItems.Count.Should().Be(3);
            filteredItems[0].Model.Type.Should().Be(ActivityTypeEnum.Clipping);
            filteredItems[1].Model.Type.Should().Be(ActivityTypeEnum.Call);
            filteredItems[2].Model.Type.Should().Be(ActivityTypeEnum.Message);	
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

        private void AddItemsForAllActivityTypes()
        {
            Enum.GetValues(typeof(ActivityTypeEnum))
                .Cast<ActivityTypeEnum>()
                .ToList()
                .ForEach(
                    activityType => _subject.Items.Add(new ActivityViewModel { Model = new Activity(activityType) }));
        }
    }
}