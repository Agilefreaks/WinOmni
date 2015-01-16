namespace OmnipasteTests.Event
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.Helpers;
    using Omnipaste.Dialog;
    using Omnipaste.Event;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.MasterEventList.Calling;
    using Omnipaste.Models;
    using OmniUI.Models;
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

        [SetUp]
        public void SetUp()
        {
            SetupTestScheduler();

            _kernel = new MoqMockingKernel();
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _kernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object);

            _mockPhoneCalls = _kernel.GetMock<IPhoneCalls>();
            _mockPhoneCalls.DefaultValue = DefaultValue.Mock;
            _kernel.Bind<IPhoneCalls>().ToConstant(_mockPhoneCalls.Object);

            _mockDialogViewModel = _kernel.GetMock<IDialogViewModel>();
            _kernel.Bind<IDialogViewModel>().ToConstant(_mockDialogViewModel.Object);

            _subject = _kernel.Get<EventViewModel>();
            _subject.Model = new Call { ContactInfo = new ContactInfo { Phone = "phone_number" } };
        }

        [Test]
        public void SendSms_SendsShowSmsMessage()
        {
            _subject.SendSms();

            _mockEventAggregator.Verify(ea => ea.Publish(It.Is<SendSmsMessage>(m => m.Recipient == "phone_number"), It.IsAny<Action<Action>>()));
        }

        [Test]
        public void CallBack_MakesTheDeviceCallTheEventPhoneNumber()
        {
            _subject.CallBack();

            _mockPhoneCalls.Verify(d => d.Call(It.Is<string>(s => s == "phone_number")), Times.Once);
        }

        [Test]
        public void CallBack_ShowsTheCallingScreenInTheDialogViewModel()
        {
            _mockPhoneCalls.Setup(d => d.Call(It.IsAny<string>())).Returns(Observable.Return(new EmptyModel()));
            DispatcherProvider.Instance = new ImmediateDispatcherProvider();
            var autoResetEvent = new AutoResetEvent(false);
            _mockDialogViewModel.Setup(dvm => dvm.ActivateItem(It.IsAny<ICallingViewModel>()))
                .Callback(() => autoResetEvent.Set());

            _subject.CallBack();

            autoResetEvent.WaitOne();
            _mockDialogViewModel.Verify(dvm => dvm.ActivateItem(It.IsAny<ICallingViewModel>()));
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

        public void SetupTestScheduler()
        {
            _testScheduler = new TestScheduler();

            _testScheduler.CreateHotObservable(
                new Recorded<Notification<EmptyModel>>(
                    0,
                    Notification.CreateOnNext(new EmptyModel())));

            SchedulerProvider.Dispatcher = _testScheduler;
        }
    }
}