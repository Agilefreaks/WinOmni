using System;
using WindowsClipboard.Interfaces;
using Caliburn.Micro;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;

namespace WindowsClipboardTests
{
    [TestFixture]
    public class WindowsClipboardTests
    {
        private MoqMockingKernel _kernel;

        private IWindowsClipboard _windowsClipboard;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IWindowsClipboardWrapper> _mockClipboardWrapper;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<IntPtr>().ToConstant(IntPtr.Zero);
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _mockClipboardWrapper = _kernel.GetMock<IWindowsClipboardWrapper>();

            _windowsClipboard = _kernel.Get<WindowsClipboard.WindowsClipboard>();
        }

        [Test]
        public void NewInstance_HasEventAggregatorInserted()
        {
            Assert.IsNotNull(_windowsClipboard.EventAggregator);
        }

        [Test]
        public void NewInstance_HasWindowsClipboardWrapperInserted()
        {
            Assert.IsNotNull(_windowsClipboard.WindowsClipboardWrapper);
        }

        [Test]
        public void Start_Always_SubscribesToTheEventAggregator()
        {
            _mockEventAggregator.Verify(ea => ea.Subscribe(_windowsClipboard), Times.Once());
        }

        [Test]
        public void Start_StartsWatchingTheClipboard()
        {
            _mockClipboardWrapper.Verify(cw => cw.StartWatchingClipboard());
        }

        [Test]
        public void Start_CreatesTheClippingsObservable()
        {
            Assert.IsNotNull(_windowsClipboard.Clippings);
        }
    }
}