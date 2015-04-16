namespace OmnipasteTests.Services
{
    using System;
    using System.Reactive;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using NUnit.Framework;
    using Omni;
    using OmniCommon.Helpers;
    using OmniCommon.Settings;
    using Omnipaste.Framework.Services;
    using Omnipaste.Framework.Services.Monitors.Credentials;
    using Omnipaste.Framework.Services.Monitors.Internet;
    using Omnipaste.Framework.Services.Monitors.Power;
    using Omnipaste.Framework.Services.Monitors.ProxyConfiguration;
    using Omnipaste.Framework.Services.Monitors.User;
    using OmniSync;

    [TestFixture]
    public class ConnectivitySupervisorTests
    {
        private Mock<IOmniService> _mockOmniService;

        private Mock<IWebSocketMonitor> _mockWebSocketMonitor;

        private MockingKernel _mockingKernel;

        private Mock<IUserMonitor> _mockUserMonitor;

        private Mock<IPowerMonitor> _mockPowerMonitor;

        private Mock<IInternetConnectivityMonitor> _mockInternetConnectivityMonitor;

        private TestScheduler _testScheduler;

        private Mock<IProxyConfigurationMonitor> _mockProxyConfigurationMonitor;

        private Mock<ICredentialsMonitor> _mockCredentialsMonitor;

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MockingKernel();

            _mockOmniService = new Mock<IOmniService> { DefaultValue = DefaultValue.Mock };
            _mockWebSocketMonitor = new Mock<IWebSocketMonitor> { DefaultValue = DefaultValue.Mock };
            _mockUserMonitor = new Mock<IUserMonitor> { DefaultValue = DefaultValue.Mock };
            _mockPowerMonitor = new Mock<IPowerMonitor> { DefaultValue = DefaultValue.Mock };
            _mockInternetConnectivityMonitor = new Mock<IInternetConnectivityMonitor> { DefaultValue = DefaultValue.Mock };
            _mockProxyConfigurationMonitor = new Mock<IProxyConfigurationMonitor> { DefaultValue = DefaultValue.Mock };
            _mockCredentialsMonitor = new Mock<ICredentialsMonitor> { DefaultValue = DefaultValue.Mock };

            _mockingKernel.Bind<IOmniService>().ToConstant(_mockOmniService.Object);
            _mockingKernel.Bind<IWebSocketMonitor>().ToConstant(_mockWebSocketMonitor.Object);
            _mockingKernel.Bind<IUserMonitor>().ToConstant(_mockUserMonitor.Object);
            _mockingKernel.Bind<IPowerMonitor>().ToConstant(_mockPowerMonitor.Object);
            _mockingKernel.Bind<IInternetConnectivityMonitor>().ToConstant(_mockInternetConnectivityMonitor.Object);
            _mockingKernel.Bind<IProxyConfigurationMonitor>().ToConstant(_mockProxyConfigurationMonitor.Object);
            _mockingKernel.Bind<ICredentialsMonitor>().ToConstant(_mockCredentialsMonitor.Object);

            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [Test]
        public void AfterStart_LoosingWebsocketConnectivityWhileOmniServiceIsStartedAndNotInTransition_WillTryToStopAndThenStartTheOmniService()
        {
            _mockOmniService.SetupGet(x => x.State).Returns(OmniServiceStatusEnum.Started);
            _mockOmniService.SetupGet(x => x.InTransition).Returns(false);

            var webSocketObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<WebSocketConnectionStatusEnum>>(
                    100,
                    Notification.CreateOnNext(WebSocketConnectionStatusEnum.Disconnected)));
            _mockWebSocketMonitor.Setup(x => x.ConnectionObservable).Returns(webSocketObservable);
            var stopObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(400, Notification.CreateOnNext(new Unit())));
            var startObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(1000, Notification.CreateOnNext(new Unit())));
            _mockOmniService.Setup(x => x.Stop()).Returns(stopObservable);
            _mockOmniService.Setup(x => x.Start()).Returns(startObservable);

            _mockingKernel.Get<ConnectivitySupervisor>();
            _testScheduler.Start();

            _mockOmniService.Verify(x => x.Start(), Times.Once);
            _mockOmniService.Verify(x => x.Stop(), Times.Once);
        }

        [Test]
        public void AfterStart_LoosingWebsocketConnectivityWhileOmniServiceIsStartedAndNotInTransition_WillTryToStartTheOmniServiceAfterStoppingItUntilItSucceeds()
        {
            _mockOmniService.SetupGet(x => x.State).Returns(OmniServiceStatusEnum.Started);
            _mockOmniService.SetupGet(x => x.InTransition).Returns(false);

            var webSocketObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<WebSocketConnectionStatusEnum>>(
                    100,
                    Notification.CreateOnNext(WebSocketConnectionStatusEnum.Disconnected)));

            _mockWebSocketMonitor.Setup(x => x.ConnectionObservable).Returns(webSocketObservable);
            var stopObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(400, Notification.CreateOnNext(new Unit())));
            var startObservables = new[]
                                       {
                                           _testScheduler.CreateColdObservable(
                                               new Recorded<Notification<Unit>>(
                                               1000,
                                               Notification.CreateOnError<Unit>(new Exception("some error")))),
                                           _testScheduler.CreateColdObservable(
                                               new Recorded<Notification<Unit>>(
                                               TimeSpan.FromSeconds(6).Ticks,
                                               Notification.CreateOnNext(new Unit())))
                                       };

            _mockOmniService.Setup(x => x.Stop()).Returns(stopObservable);
            var counter = 0;
            _mockOmniService.Setup(x => x.Start()).Returns(() => startObservables[counter++]);

            _mockingKernel.Get<ConnectivitySupervisor>();
            _testScheduler.Start();

            _mockOmniService.Verify(x => x.Stop(), Times.Exactly(2));
            _mockOmniService.Verify(x => x.Start(), Times.Exactly(2));
        }

        [Test]
        public void AfterStart_LoosingWebsocketConnectivityWhileOmniServiceIsStartedAndNotInTransition_WillTryToStopTheOmniServiceUntilItSucceedsAndThenStartIt()
        {
            _mockOmniService.SetupGet(x => x.State).Returns(OmniServiceStatusEnum.Started);
            _mockOmniService.SetupGet(x => x.InTransition).Returns(false);

            var webSocketObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<WebSocketConnectionStatusEnum>>(
                    100,
                    Notification.CreateOnNext(WebSocketConnectionStatusEnum.Disconnected)));

            _mockWebSocketMonitor.Setup(x => x.ConnectionObservable).Returns(webSocketObservable);
            var stopObservables = new[]
                                     {
                                         _testScheduler.CreateColdObservable(
                                             new Recorded<Notification<Unit>>(
                                             100,
                                             Notification.CreateOnError<Unit>(new Exception("some exception")))),
                                         _testScheduler.CreateColdObservable(
                                             new Recorded<Notification<Unit>>(
                                             100,
                                             Notification.CreateOnNext(new Unit())))
                                     };
            var startObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(
                    200,
                    Notification.CreateOnNext(new Unit())));

            var counter = 0;
            _mockOmniService.Setup(x => x.Stop()).Returns(() => stopObservables[counter++]);
            _mockOmniService.Setup(x => x.Start()).Returns(startObservable);

            _mockingKernel.Get<ConnectivitySupervisor>();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(40).Ticks);

            _mockOmniService.Verify(x => x.Stop(), Times.Exactly(2));
            _mockOmniService.Verify(x => x.Start(), Times.Exactly(1));
        }

        [Test]
        public void AfterStart_WhenCredentialsChange_StopsOmniService()
        {
            var credentialsObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<OmnipasteCredentials>>(
                    100,
                    Notification.CreateOnNext(new OmnipasteCredentials())));
            _mockCredentialsMonitor.Setup(x => x.SettingObservable).Returns(credentialsObservable);
            var stopObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(400, Notification.CreateOnNext(new Unit())));
            _mockOmniService.Setup(x => x.Stop()).Returns(stopObservable);

            _mockingKernel.Get<ConnectivitySupervisor>();
            _testScheduler.Start();

            _mockOmniService.Verify(x => x.Stop(), Times.Once());
        }

        [Test]
        public void AfterStart_WhenNetworkChanges_RestartsOmniService()
        {
            _mockOmniService.SetupGet(x => x.State).Returns(OmniServiceStatusEnum.Started);
            _mockOmniService.SetupGet(x => x.InTransition).Returns(false);

            var webSocketObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<WebSocketConnectionStatusEnum>>(
                    100,
                    Notification.CreateOnNext(WebSocketConnectionStatusEnum.Disconnected)));
            _mockWebSocketMonitor.Setup(x => x.ConnectionObservable).Returns(webSocketObservable);

            var connectivityObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<InternetConnectivityStatusEnum>>(
                        200,
                        Notification.CreateOnNext(InternetConnectivityStatusEnum.Disconnected)));
            _mockInternetConnectivityMonitor.Setup(m => m.ConnectivityChangedObservable).Returns(connectivityObservable);

            var stopObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Unit>>(200, Notification.CreateOnNext(new Unit())));
            var startObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Unit>>(
                    200,
                    Notification.CreateOnNext(new Unit())));

            _mockOmniService.Setup(x => x.Stop()).Returns(stopObservable);
            _mockOmniService.Setup(x => x.Start()).Returns(startObservable);

            _mockingKernel.Get<ConnectivitySupervisor>();
            _testScheduler.AdvanceTo(TimeSpan.FromSeconds(40).Ticks);

            _mockOmniService.Verify(x => x.Stop(), Times.Exactly(1));
            _mockOmniService.Verify(x => x.Start(), Times.Exactly(1));
        }
    }
}