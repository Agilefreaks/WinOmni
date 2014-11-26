namespace OmnipasteTests.Event
{
    using System.Reactive.Linq;
    using System.Threading;
    using Caliburn.Micro;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using Omnipaste.Dialog;
    using Omnipaste.Event;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.MasterEventList.Calling;

    [TestFixture]
    public class EventViewModelTests
    {
        private MoqMockingKernel _kernel;

        private IEventViewModel _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IDevices> _mockDevices;

        private TestScheduler _testScheduler;

        private Mock<IDialogViewModel> _mockDialogViewModel;

        [SetUp]
        public void SetUp()
        {
            SetupTestScheduler();

            _kernel = new MoqMockingKernel();
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _kernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object);

            _mockDevices = _kernel.GetMock<IDevices>();
            _mockDevices.DefaultValue = DefaultValue.Mock;
            _kernel.Bind<IDevices>().ToConstant(_mockDevices.Object);

            _mockDialogViewModel = _kernel.GetMock<IDialogViewModel>();
            _kernel.Bind<IDialogViewModel>().ToConstant(_mockDialogViewModel.Object);

            _subject = _kernel.Get<EventViewModel>();
            _subject.Model = new Event { PhoneNumber = "phone_number" };
        }

        [Test]
        public void SendSms_SendsShowSmsMessage()
        {
            _subject.SendSms();

            _mockEventAggregator.Verify(ea => ea.Publish(It.Is<SendSmsMessage>(m => m.Recipient == "phone_number"), It.IsAny<System.Action<System.Action>>()));
        }

        [Test]
        public void CallBack_MakesTheDeviceCallTheEventPhoneNumber()
        {
            _subject.CallBack();

            _mockDevices.Verify(d => d.Call(It.Is<string>(s => s == "phone_number")), Times.Once);
        }

        [Test]
        public void CallBack_ShowsTheCallingScreenInTheDialogViewModel()
        {
            _mockDevices.Setup(d => d.Call(It.IsAny<string>())).Returns(Observable.Return(new EmptyModel()));
            DispatcherProvider.Current = new ImmediateDispatcher();
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
            _subject.Model.ContactName = "testName";

            _subject.Title.Should().Be("testName");
        }

        [Test]
        public void Title_EventHasEmptyContactName_IsPhoneNumber()
        {
            _subject.Model.PhoneNumber = "1231321";
            _subject.Model.ContactName = string.Empty;

            _subject.Title.Should().Be("1231321");
        }

        public void SetupTestScheduler()
        {
            _testScheduler = new TestScheduler();

            _testScheduler.CreateHotObservable(
                new Recorded<System.Reactive.Notification<EmptyModel>>(
                    0,
                    System.Reactive.Notification.CreateOnNext(new EmptyModel())));

            SchedulerProvider.Dispatcher = _testScheduler;
        }
    }
}