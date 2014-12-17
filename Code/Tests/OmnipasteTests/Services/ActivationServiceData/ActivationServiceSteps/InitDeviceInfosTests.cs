namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class InitDeviceInfosTests
    {
        private InitDeviceInfos _subject;

        private Mock<IDevices> _mockDevices;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            _mockDevices = new Mock<IDevices>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new InitDeviceInfos(_mockDevices.Object, _mockConfigurationService.Object);
        }

        [Test]
        public void Execute_WhenDevicesGetAllSucceeds_SavesDeviceInfos()
        {
            var testScheduler = new TestScheduler();
            _mockDevices.Setup(m => m.GetAll()).Returns(Observable.Return(new List<Device> { new Device("42") }));

            testScheduler.Start(_subject.Execute);

            _mockConfigurationService.VerifySet(m => m.DeviceInfos);
        }

        [Test]
        public void Execute_WhenDevicesGetAllSucceeds_SavesDeviceInfosData()
        {
            IList<DeviceInfo> resultData = null;
            var testScheduler = new TestScheduler();
            _mockDevices.Setup(m => m.GetAll()).Returns(Observable.Return(new List<Device> { new Device("42") }));
            _mockConfigurationService.SetupSet(m => m.DeviceInfos).Callback(list => resultData = list);
            
            testScheduler.Start(_subject.Execute);

            resultData.Count.Should().Be(1);
            resultData.First().Identifier.Should().Be("42");
        }

        [Test]
        public void Execute_WhenDevicesGetAllSucceeds_DoesNotSaveDeviceInfoForCurrentDevice()
        {
            IList<DeviceInfo> resultData = null;
            var testScheduler = new TestScheduler();
            _mockDevices.Setup(m => m.GetAll()).Returns(Observable.Return(new List<Device> { new Device("42") }));
            _mockConfigurationService.SetupSet(m => m.DeviceInfos).Callback(list => resultData = list);
            _mockConfigurationService.SetupGet(m => m.DeviceIdentifier).Returns("42");

            testScheduler.Start(_subject.Execute);

            resultData.Count.Should().Be(0);
        }
    }
}
