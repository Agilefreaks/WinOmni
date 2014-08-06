namespace OmnipasteTests.Notification.IncomingCallNotification
{
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using Omnipaste.Notification.IncomingCallNotification;

    [TestFixture]
    public class IncomingCallNotificationViewModelTests
    {
        private IncomingCallNotificationViewModel _subject;

        private MoqMockingKernel _kernel;

        private Mock<IPhones> _mockPhones;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockPhones = _kernel.GetMock<IPhones>();
            _mockPhones.DefaultValue = DefaultValue.Mock;
            _subject = _kernel.Get<IncomingCallNotificationViewModel>();
        }

        [Test]
        public void EndCall_CallsPhonesEndCallMethod()
        {
            _subject.EndCall();

            _mockPhones.Verify(p => p.EndCall(), Times.Once);
        }
    }
}