namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class VerifyNumberOfDevicesTests
    {
        private VerifyNumberOfDevices _subject;

        private TestScheduler _testScheduler;

        private ITestableObserver<IExecuteResult> _testObserver;

        private Mock<IDevices> _mockDevices;

        private ITestableObservable<List<Device>> _testObservable;

        private List<Device> _devices;

        [SetUp]
        public void SetUp()
        {
            var kernel = new MoqMockingKernel();
            _mockDevices = kernel.GetMock<IDevices>();
            _subject = new VerifyNumberOfDevices (_mockDevices.Object);
            _devices = new List<Device> { new Device() };

            _testScheduler = new TestScheduler();
            _testObserver = _testScheduler.CreateObserver<IExecuteResult>();
            _testObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<List<Device>>>(0, Notification.CreateOnNext(_devices)),
                new Recorded<Notification<List<Device>>>(0, Notification.CreateOnCompleted<List<Device>>()));
            _mockDevices.Setup(d => d.GetAll()).Returns(_testObservable);
        }

        [Test]
        public void Execute_WhenRegisteredDeviceIsNotTheOnlyDevice_ReturnsFailed()
        {
            _devices.Add(new Device());
            
            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State == SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_WhenRegisteredDeviceIsTheOnlyDeviceIsSuccessful()
        {
            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State == SimpleStepStateEnum.Successful);
        }
    }
}