namespace OmnipasteTests.MasterEventList.AllEventList
{
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Event;
    using Omnipaste.MasterEventList.AllEventList;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class AllEventListViewModelTests
    {
        private MoqMockingKernel _kernel;

        private AllEventListViewModel _subject;

        private Mock<ICallRepository> _mockCallRepository;

        private Mock<IMessageRepository> _mockMessageRepository;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _kernel = new MoqMockingKernel();
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _kernel.Bind<ICallRepository>().ToConstant(_mockCallRepository.Object);
            _kernel.Bind<IMessageRepository>().ToConstant(_mockMessageRepository.Object);
            _kernel.Bind<IEventViewModel>().To<EventViewModel>();

            _subject = new AllEventListViewModel(_mockCallRepository.Object, _mockMessageRepository.Object, _kernel);
        }

        [Test]
        public void OnCallSaved_Always_AddsItemToList()
        {
            var callOperationObservable = _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Create, new Call()))));
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(callOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void OnCallRemoved_AfterPreviouslyAdded_RemovesItemFromList()
        {
            var call = new Call();
            var callOperationObservable = _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Call>>>(100, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Create, call))),
                    new Recorded<Notification<RepositoryOperation<Call>>>(200, Notification.CreateOnNext(new RepositoryOperation<Call>(RepositoryMethodEnum.Delete, call))));
            _mockCallRepository.SetupGet(m => m.OperationObservable).Returns(callOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(0);
        }

        [Test]
        public void OnMessageSaved_Always_AddsItemToList()
        {
            var callOperationObservable = _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Message>>>(100, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Create, new Message()))));
            _mockMessageRepository.SetupGet(m => m.OperationObservable).Returns(callOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void OnMessageRemoved_AfterPreviouslyAdded_RemovesItemFromList()
        {
            var message = new Message();
            var callOperationObservable = _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Message>>>(100, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Create, message))),
                    new Recorded<Notification<RepositoryOperation<Message>>>(200, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Delete, message))));
            _mockMessageRepository.SetupGet(m => m.OperationObservable).Returns(callOperationObservable);
            ((IActivate)_subject).Activate();

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(0);
        }
    }
}