namespace OmnipasteTests.MasterEventList.IncomingCallEventList
{
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Event;
    using Omnipaste.MasterEventList.IncomingCallEventList;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class IncomingCallEventListViewModelTests
    {
        private MoqMockingKernel _kernel;

        private IncomingCallEventListViewModel _subject;

        private Mock<ICallRepository> _mockCallRepository;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _kernel = new MoqMockingKernel();
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _kernel.Bind<ICallRepository>().ToConstant(_mockCallRepository.Object);
            _kernel.Bind<IEventViewModel>().To<EventViewModel>();

            _subject = new IncomingCallEventListViewModel(_mockCallRepository.Object, _kernel);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void OnCallAdded_Always_AddsItemToList()
        {
            var messageOperationObservable = _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Save, new Call()))));
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(messageOperationObservable);
            ((IActivate)_subject).Activate();
            
            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void OnCallRemoved_AfterPreviouslyAdded_RemovesItemFromList()
        {
            var call = new Call();
            var messageOperationObservable = _testScheduler.CreateColdObservable(
                     new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Save, call))),
                     new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Delete, call))));
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(messageOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(0);
        }
    }
}