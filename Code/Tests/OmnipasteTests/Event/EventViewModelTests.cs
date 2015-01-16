namespace OmnipasteTests.Event
{
    using System;
    using System.Reactive;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Dialog;
    using Omnipaste.Event;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.MasterEventList.Calling;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Models;
    using PhoneCalls.Models;
    using PhoneCalls.Resources.v1;

    [TestFixture]
    public class EventViewModelTests
    {
        private MoqMockingKernel _kernel;

        private IEventViewModel _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IPhoneCalls> _mockPhoneCalls;

        private TestScheduler _testScheduler;

        private Mock<IDialogViewModel> _mockDialogViewModel;

        private Mock<ICallRepository> _mockCallRepository;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _kernel = new MoqMockingKernel();
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _kernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object);

            _mockPhoneCalls = _kernel.GetMock<IPhoneCalls>();
            _mockPhoneCalls.DefaultValue = DefaultValue.Mock;
            _kernel.Bind<IPhoneCalls>().ToConstant(_mockPhoneCalls.Object);

            _mockCallRepository = _kernel.GetMock<ICallRepository>();
            _kernel.Bind<ICallRepository>().ToConstant(_mockCallRepository.Object);

            _mockDialogViewModel = _kernel.GetMock<IDialogViewModel>();
            _kernel.Bind<IDialogViewModel>().ToConstant(_mockDialogViewModel.Object);

            _subject = _kernel.Get<EventViewModel>();
            _subject.Model = new Call { ContactInfo = new ContactInfo { Phone = "phone_number" } };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void SendSms_SendsShowSmsMessage()
        {
            _subject.SendSms();

            _mockEventAggregator.Verify(ea => ea.Publish(It.Is<SendSmsMessage>(m => m.Recipient == "phone_number"), It.IsAny<Action<Action>>()));
        }

        [Test]
        public async Task CallBack_MakesTheDeviceCallTheEventPhoneNumber()
        {
            SetupCreateCall();
            SetupSaveCall();

            var task = _subject.CallBack();
            _testScheduler.Start();
            await task;

            _mockPhoneCalls.Verify(d => d.Call(It.Is<string>(s => s == "phone_number")), Times.Once);
        }

        [Test]
        public async Task CallBack_ShowsTheCallingScreenInTheDialogViewModel()
        {
            SetupCreateCall();
            SetupSaveCall();

            var task = _subject.CallBack();
            _testScheduler.Start();
            await task;

            _mockDialogViewModel.Verify(dvm => dvm.ActivateItem(It.IsAny<ICallingViewModel>()));
        }

        [Test]
        public async Task CallBack_SavesTheCreateCall()
        {
            SetupCreateCall();
            SetupSaveCall();

            var task = _subject.CallBack();
            _testScheduler.Start();
            await task;

            _mockCallRepository.Verify(x => x.Save(It.IsAny<Call>()), Times.Once());
        }

        [Test]
        public void Title_EventHasNonEmptyContactName_IsContactName()
        {
            _subject.Model.ContactInfo.FirstName = "test";
            _subject.Model.ContactInfo.LastName = "name";

            _subject.Title.Should().Be("test name");
        }

        [Test]
        public void Title_EventHasEmptyContactName_IsPhoneNumber()
        {
            _subject.Model.ContactInfo.Phone = "1231321";
            _subject.Model.ContactInfo.FirstName = string.Empty;

            _subject.Title.Should().Be("1231321");
        }

        private void SetupCreateCall()
        {
            var createCallObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<PhoneCall>>(100, Notification.CreateOnNext(new PhoneCall())),
                    new Recorded<Notification<PhoneCall>>(200, Notification.CreateOnCompleted<PhoneCall>()));
            _mockPhoneCalls.Setup(x => x.Call(It.IsAny<string>())).Returns(createCallObservable);
        }

        private void SetupSaveCall()
        {
            var saveCallObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Call>>>(
                        100,
                        Notification.CreateOnNext(
                            new RepositoryOperation<Call>(RepositoryMethodEnum.Create, new Call()))),
                    new Recorded<Notification<RepositoryOperation<Call>>>(
                        200,
                        Notification.CreateOnCompleted<RepositoryOperation<Call>>()));
            _mockCallRepository.Setup(x => x.Save(It.IsAny<Call>())).Returns(saveCallObservable);
        }
    }
}