namespace OmnipasteTests.ActivityList
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reactive;
    using System.Threading;
    using Caliburn.Micro;
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

        private Mock<IUpdateInfoRepository> _mockUpdateInfoRepository;

        private Mock<IActivityViewModelFactory> _mockActivityViewModelFactory;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<ICallRepository> _mockCallRepository;

        private Mock<IMessageRepository> _mockMessageRepository;

        private TestScheduler _testScheduler;

        private Mock<ISessionManager> _mockSessionManager;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockUpdateInfoRepository = new Mock<IUpdateInfoRepository> { DefaultValue = DefaultValue.Mock };
            _mockActivityViewModelFactory = new Mock<IActivityViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };

            _mockActivityViewModelFactory.Setup(x => x.Create(It.IsAny<ActivityPresenter>())).Returns<ActivityPresenter>(presenter => new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = presenter });

            _subject = new ActivityListViewModel(
                _mockClippingRepository.Object,
                _mockMessageRepository.Object,
                _mockCallRepository.Object,
                _mockUpdateInfoRepository.Object,
                _mockActivityViewModelFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void ReceivingAClipping_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Create, new ClippingModel()))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(200,
                        Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void RemovingAClipping_AfterActivateWhenClippingWasPreviouslyReceived_RemovesViewModelForClipping()
        {
            const string SourceId = "42";
            var clippingModel = new ClippingModel { UniqueId = SourceId };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(100, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Create, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(200, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Delete, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(300,Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();
            
            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(0);
        }

        [Test]
        public void UpdatingAClipping_AfterActivateWhenClippingWasPreviouslyReceived_UpdatesViewModelWithNewClipping()
        {
            const string SourceId = "42";
            var clippingModel = new ClippingModel { UniqueId = SourceId };
            var modifiedClipping = new ClippingModel { UniqueId = SourceId, Content = "Test" };
            var clippingOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(100, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Create, clippingModel))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(200, Notification.CreateOnNext(new RepositoryOperation<ClippingModel>(RepositoryMethodEnum.Update, modifiedClipping))),
                    new Recorded<Notification<RepositoryOperation<ClippingModel>>>(300, Notification.CreateOnCompleted<RepositoryOperation<ClippingModel>>()));
            _mockClippingRepository.SetupGet(x => x.OperationObservable).Returns(clippingOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.Should().Be(modifiedClipping);
        }

        [Test]
        public void ReceivingACall_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var repositoryOperation = new RepositoryOperation<Call>(RepositoryMethodEnum.Create, new Call { Source = SourceType.Remote });
            var eventObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(repositoryOperation)),
                    new Recorded<Notification<RepositoryOperation<Call>>>(200, Notification.CreateOnCompleted<RepositoryOperation<Call>>()));
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(eventObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void UpdatingACall_AfterActivateWhenPreviouslyReceived_UpdatesViewModelWithNewCall()
        {
            const string UniqueId = "42";
            var call = new Call { UniqueId = UniqueId, Source = SourceType.Remote };
            var modifiedCall = new Call { UniqueId = UniqueId, Content = "Test" };
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Create, call))),
                    new Recorded<Notification<RepositoryOperation<Call>>>(200, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Update, modifiedCall))),
                    new Recorded<Notification<RepositoryOperation<Call>>>(300, Notification.CreateOnCompleted<RepositoryOperation<Call>>()));
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(callObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.Should().Be(modifiedCall);
        }

        [Test]
        public void ReceivingAMessage_AfterActivate_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var message = new SmsMessage{ Source = SourceType.Remote};
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<SmsMessage>>>(100, Notification.CreateOnNext(new RepositoryOperation<SmsMessage>(RepositoryMethodEnum.Create, message))),
                    new Recorded<Notification<RepositoryOperation<SmsMessage>>>(200, Notification.CreateOnCompleted<RepositoryOperation<SmsMessage>>()));
            _mockMessageRepository.SetupGet(m => m.OperationObservable).Returns(messageObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void UpdatingAMessage_AfterActivateWhenPreviouslyReceived_UpdatesViewModelWithNewMessage()
        {
            const string UniqueId = "42";
            var message = new SmsMessage { UniqueId = UniqueId, Source = SourceType.Remote };
            var modifiedMessage = new SmsMessage { UniqueId = UniqueId, Content = "Test", Source = SourceType.Remote };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<SmsMessage>>>(100, Notification.CreateOnNext(new RepositoryOperation<SmsMessage>(RepositoryMethodEnum.Create, message))),
                    new Recorded<Notification<RepositoryOperation<SmsMessage>>>(200, Notification.CreateOnNext(new RepositoryOperation<SmsMessage>(RepositoryMethodEnum.Update, modifiedMessage))),
                    new Recorded<Notification<RepositoryOperation<SmsMessage>>>(300, Notification.CreateOnCompleted<RepositoryOperation<SmsMessage>>()));
            _mockMessageRepository.SetupGet(m => m.OperationObservable).Returns(messageObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(1000);

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Model.BackingModel.Should().Be(modifiedMessage);
        }

        [Test]
        public void ReceivingAnUpdate_AfterStart_CreatesANewActivityViewModelAndAddsItToItems()
        {
            var operationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<UpdateInfo>>>(100, Notification.CreateOnNext(new RepositoryOperation<UpdateInfo>(RepositoryMethodEnum.Create, new UpdateInfo()))),
                    new Recorded<Notification<RepositoryOperation<UpdateInfo>>>(200, Notification.CreateOnCompleted<RepositoryOperation<UpdateInfo>>()));
            _mockUpdateInfoRepository.SetupGet(x => x.OperationObservable).Returns(operationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);

            _subject.Items.Count.Should().Be(1);
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
            filteredItems.Count(vm => vm.Model.Type == ActivityTypeEnum.Clipping).Should().Be(1);
            filteredItems.Count(vm => vm.Model.Type == ActivityTypeEnum.Message).Should().Be(1);
        }

        [Test]
        public void ChangingFilterText_Always_UpdatesFilteredItemsSoAsToShowOnlyItemsWhoseModelsHaveTheFilterTextInTheirStringRepresentation()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
            var activityPresenter1 = new ActivityPresenter(new Call());
            var activityPresenter2 = new ActivityPresenter(new SmsMessage());
            ((IActivate)_subject).Activate();
            _subject.ActivateItem(new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = activityPresenter1 });
            _subject.ActivateItem(new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = activityPresenter2 });

            _subject.FilterText = "sms de la";

            _subject.FilteredItems.Count.Should().Be(1);
            _subject.FilteredItems.Cast<IActivityViewModel>().First().Model.Should().Be(activityPresenter2);
        }

        [Test]
        public void ChangingFilterText_TextIsMadeUpOfMultipleWords_UpdatesFilteredItemsSoAsToShowOnlyItemsWhoseModelsHaveAllTheWordsInTheFilterTextInTheirStringRepresentation()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
            var activityPresenter1 = new ActivityPresenter(new SmsMessage { ContactInfo = new ContactInfo { FirstName = "John", LastName = "Doe" } });
            var activityPresenter2 = new ActivityPresenter(new SmsMessage { ContactInfo = new ContactInfo { FirstName = "Jane", LastName = "Doe" } });
            ((IActivate)_subject).Activate();
            _subject.ActivateItem(new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = activityPresenter1 });
            _subject.ActivateItem(new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = activityPresenter2 });

            _subject.FilterText = "JAnE DoE sms";

            _subject.FilteredItems.Count.Should().Be(1);
            _subject.FilteredItems.Cast<IActivityViewModel>().First().Model.Should().Be(activityPresenter2);
        }

        private void AddItemsForAllActivityTypes()
        {
            _subject.Items.Add(new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = new ActivityPresenter(new ClippingModel()) });
            _subject.Items.Add(new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = new ActivityPresenter(new Call()) });
            _subject.Items.Add(new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = new ActivityPresenter(new SmsMessage()) });
            _subject.Items.Add(new ActivityViewModel(_mockUiRefreshService.Object, _mockSessionManager.Object) { Model = new ActivityPresenter(new UpdateInfo()) });
        }
    }
}