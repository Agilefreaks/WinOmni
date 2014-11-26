namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Clipboard.Handlers.WindowsClipboard;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class AddSampleClippingTests
    {
        private AddSampleClippings _subject;

        private Mock<IWindowsClipboardWrapper> _mockWindowsClipboardWrapper;

        [SetUp]
        public void Setup()
        {
            _mockWindowsClipboardWrapper = new Mock<IWindowsClipboardWrapper>();
            _subject = new AddSampleClippings(_mockWindowsClipboardWrapper.Object);
        }

        [Test]
        public void Execute_WhenSubscribedTo_SetsASampleTextOnTheLocalClipboard()
        {
            var autoResetEvent = new AutoResetEvent(false);
            SchedulerProvider.Default = new NewThreadScheduler();

            Task.Factory.StartNew(
                () =>
                {
                    _subject.Execute().Wait();
                    autoResetEvent.Set();
                });

            autoResetEvent.WaitOne();
            _mockWindowsClipboardWrapper.Verify(x => x.SetData(Omnipaste.Properties.Resources.SampleLocalClipping));
        }
    }
}