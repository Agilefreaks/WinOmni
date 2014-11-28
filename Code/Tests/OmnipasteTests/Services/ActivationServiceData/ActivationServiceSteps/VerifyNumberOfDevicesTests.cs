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
    using Omnipaste.Services.ActivationServiceData;
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
        public void Execute_WhenASingleDeviceExists_CompletesWithStatusOne()
        {
            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.One));
        }

        [Test]
        public void Execute_WhenTwoDevicesExistAndParameterIsNotADevice_CompletesWithStatusTwoOrMore()
        {
            _devices.Add(new Device());
            _subject.Parameter = null;

            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.TwoOrMore));
        }

        [Test]
        public void Execute_WhenTwoDevicesExistAndParameterIsADevice_CompletesWithStatusTwoAndThisOneIsNew()
        {
            _devices.Add(new Device());
            _subject.Parameter = new DependencyParameter { Value = new Device() };

            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.TwoAndThisOneIsNew));
        }

        [Test]
        public void Execute_WhenMoreThanTwoDevicesExists_CompletesWithStatusTwoOrMore()
        {
            _devices.Add(new Device());
            _devices.Add(new Device());
            _subject.Parameter = new DependencyParameter { Value = new Device() };

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
            _subject.Parameter = new DependencyParameter { Value = new Device() };

            _subject.Execute().Subscribe(_testObserver);
            _testScheduler.Start();

            _testObserver.Messages
                .Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State.Equals(NumberOfDevicesEnum.Zero));
        }
    }
}