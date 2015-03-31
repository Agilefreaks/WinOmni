namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;
    using Refit;

    [TestFixture]
    public class VerifySettingsTests
    {
        private VerifySettings _subject;

        private Mock<IDevices> _mockDevices;

        private Mock<IConfigurationService> _mockConfigurationService;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockDevices = new Mock<IDevices> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _subject = new VerifySettings(_mockDevices.Object, _mockConfigurationService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Execute_WhenDeviceIdIsEmpty_ReturnsSuccess()
        {
            _mockConfigurationService.Setup(m => m.DeviceId).Returns(string.Empty);

            var testObserver = _testScheduler.Start(() => _subject.Execute());

            testObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            testObserver.Messages.First().Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_WhenDeviceIdIsNotEmptyAndDeviceExistOnServer_ReturnsSuccess()
        {
            const string DeviceId = "42";
            _mockConfigurationService.Setup(m => m.DeviceId).Returns(DeviceId);
            _mockDevices.Setup(m => m.Get(DeviceId)).Returns(Observable.Return(new DeviceDto { Id = DeviceId }, _testScheduler));

            var testObserver = _testScheduler.Start(() => _subject.Execute());
            
            testObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            testObserver.Messages.First().Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_WhenDeviceIdIsNotEmptyAndDeviceDoesNotExistOnServer_ReturnsFail()
        {
            const string DeviceId = "42";
            _mockConfigurationService.Setup(m => m.DeviceId).Returns(DeviceId);
            var createNotFoundException = ApiException.Create(new HttpResponseMessage(HttpStatusCode.NotFound));
            createNotFoundException.Wait();
            _mockDevices.Setup(m => m.Get(DeviceId)).Returns(Observable.Throw<DeviceDto>(createNotFoundException.Result, _testScheduler));

            var testObserver = _testScheduler.Start(() => _subject.Execute());

            testObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            testObserver.Messages.First().Value.Value.State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_WhenDeviceIdIsNotEmptyAndDeviceDoesNotExistOnServer_PropagatesException()
        {
            const string DeviceId = "42";
            _mockConfigurationService.Setup(m => m.DeviceId).Returns(DeviceId);
            var createNotFoundException = ApiException.Create(new HttpResponseMessage(HttpStatusCode.BadRequest));
            createNotFoundException.Wait();
            _mockDevices.Setup(m => m.Get(DeviceId)).Returns(Observable.Throw<DeviceDto>(createNotFoundException.Result, _testScheduler));

            var testObserver = _testScheduler.Start(() => _subject.Execute());

            testObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnError);
            testObserver.Messages.First().Value.Exception.Should().Be(createNotFoundException.Result);
        }
    }
}
