namespace OmnipasteTests.MasterEventList.IncomingSmsEventList
{
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Event;
    using Omnipaste.MasterEventList.IncomingSmsEventList;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class IncomingSmsEventListViewModelTests
    {
        private MoqMockingKernel _kernel;

        private IncomingSmsEventListViewModel _subject;

        private TestScheduler _testScheduler;

        private Mock<IMessageRepository>  _mockMessageRepository;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;
            
            _kernel = new MoqMockingKernel();
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _kernel.Bind<IEventViewModel>().To<EventViewModel>();
            _kernel.Bind<IMessageRepository>().ToConstant(_mockMessageRepository.Object);

            _subject = new IncomingSmsEventListViewModel(_mockMessageRepository.Object, _kernel);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void OnMessageAdded_Always_AddedItemToList()
        {
            var messageOperationObservable = _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Message>>>(100, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Save, new Message()))));
            _mockMessageRepository.SetupGet(m => m.OperationObservable).Returns(messageOperationObservable);
            _subject = new IncomingSmsEventListViewModel(_mockMessageRepository.Object, _kernel);

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(1);
        }

        [Test]
        public void OnMessageRemoved_AfterPreviouslyAdded_RemovesItemFromList()
        {
            var message = new Message();
            var messageOperationObservable = _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Message>>>(100, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Save, message))),
                    new Recorded<Notification<RepositoryOperation<Message>>>(100, Notification.CreateOnNext(new RepositoryOperation<Message>(RepositoryMethodEnum.Delete, message)))
                    );
            _mockMessageRepository.SetupGet(m => m.OperationObservable).Returns(messageOperationObservable);
            _subject = new IncomingSmsEventListViewModel(_mockMessageRepository.Object, _kernel);

            _testScheduler.Start();
            _testScheduler.AdvanceBy(1000);

            _subject.Items.Count.Should().Be(0);
        }
    }
}