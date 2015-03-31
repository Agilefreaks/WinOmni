namespace OmnipasteTests.Notification.IncomingCallNotification
{
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Windows.Threading;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Entities;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Notification;
    using Omnipaste.Notification.IncomingCallNotification;
    using OmniUI.Services;
    using PhoneCalls.Resources.v1;

    [TestFixture]
    public class IncomingCallNotificationViewModelTests
    {
        private IncomingCallNotificationViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IPhoneCalls> _mockPhoneCalls;

        private Mock<IApplicationService> _mockApplicationService;

        private Mock<ICommandService> _mockCommandService;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockPhoneCalls = _kernel.GetMock<IPhoneCalls>();
            _mockPhoneCalls.DefaultValue = DefaultValue.Mock;
            
            _mockCommandService = _kernel.GetMock<ICommandService>();
            _kernel.Bind<ICommandService>().ToConstant(_mockCommandService.Object).InSingletonScope();

            _mockApplicationService = _kernel.GetMock<IApplicationService>();
            _mockApplicationService.Setup(s => s.Dispatcher).Returns(Dispatcher.CurrentDispatcher);
            _kernel.Bind<IApplicationService>().ToConstant(_mockApplicationService.Object);
            
            _subject = _kernel.Get<IncomingCallNotificationViewModel>();
            _subject.State = ViewModelStatusEnum.Open;
        }

        [Test]
        public void EndCall_EndsThePhoneCallCorespondingToTheAssociatedResource()
        {
            const string ResourceId = "someId";
            _subject.Resource = new RemotePhoneCallModel(new RemotePhoneCallEntity { Id = ResourceId });
            
            _subject.EndCall();

            _mockPhoneCalls.Verify(p => p.EndCall(ResourceId), Times.Once);
        }

        [Test]
        public void ReplyWithSms_Always_ExecutesComposeSMSCommand()
        {
            var contactInfoPresenter = new ContactModel(new ContactEntity());
            _subject.Resource = new RemotePhoneCallModel(new RemotePhoneCallEntity()) { ContactModel = contactInfoPresenter };
            _mockCommandService.Setup(x => x.Execute(It.IsAny<ComposeSMSCommand>())).Returns(Observable.Return(new Unit()));
            var testScheduler = new TestScheduler();
            SchedulerProvider.Dispatcher = testScheduler;

            testScheduler.Start(() => _subject.ReplyWithSMS().ToObservable());

            _mockCommandService.Verify(x => x.Execute(It.Is<ComposeSMSCommand>(m => m.Contact == contactInfoPresenter)));
        }
    }
}
