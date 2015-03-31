namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class VerifyNumberOfDevicesTests
    {
        private VerifyNumberOfDevices _subject;

        private TestScheduler _testScheduler;

        private ITestableObserver<IExecuteResult> _testObserver;

        private Mock<IDevices> _mockDevices;

        private ITestableObservable<List<DeviceDto>> _testObservable;

        private List<DeviceDto> _devices;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            var kernel = new MoqMockingKernel();
            _mockDevices = kernel.GetMock<IDevices>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new VerifyNumberOfDevices (_mockDevices.Object, _mockConfigurationService.Object);
            _devices = new List<DeviceDto> { new DeviceDto() };

            _testScheduler = new TestScheduler();
            _testObserver = _testScheduler.CreateObserver<IExecuteResult>();
            _testObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<List<DeviceDto>>>(0, Notification.CreateOnNext(_devices)),
                new Recorded<Notification<List<DeviceDto>>>(0, Notification.CreateOnCompleted<List<DeviceDto>>()));
            _mockDevices.Setup(d => d.GetAll()).Returns(_testObservable);
        }

        [Test]
        public void Execute_WhenASingleDeviceExists_CompletesWithStatusOne()
        {
            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.One));
        }

        [Test]
        public void Execute_WhenTwoDevicesExistAndIsNewDeviceIsFalse_CompletesWithStatusTwoOrMore()
        {
            _devices.Add(new DeviceDto());
            _mockConfigurationService.SetupGet(x => x.IsNewDevice).Returns(false);

            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.TwoOrMore));
        }

        [Test]
        public void Execute_WhenTwoDevicesExistAndIsNewDeviceIsTrue_CompletesWithStatusTwoAndThisOneIsNew()
        {
            _devices.Add(new DeviceDto());
            _mockConfigurationService.SetupGet(x => x.IsNewDevice).Returns(true);

            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.TwoAndThisOneIsNew));
        }

        [Test]
        public void Execute_WhenMoreThanTwoDevicesExists_CompletesWithStatusTwoOrMore()
        {
            _devices.Add(new DeviceDto());
            _devices.Add(new DeviceDto());
            _subject.Parameter = new DependencyParameter { Value = new DeviceDto() };

            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.TwoOrMore));
        }

        [Test]
        public void Execute_WhenNoDevicesExists_CompletesWithStatusZero()
        {
            _devices.Clear();
            _subject.Parameter = new DependencyParameter { Value = new DeviceDto() };

            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.Zero));
        }
    }
}