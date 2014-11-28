namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class AndroidInstallGuideTests
    {
        private ShowAndroidInstallGuide _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        private MoqMockingKernel _kernel;

        private Uri _givenUri;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _givenUri = new Uri("http://someUri.com");
            _subject = new ShowAndroidInstallGuide(_mockEventAggregator.Object)
                           {
                               Parameter = new DependencyParameter("test", _givenUri)
                           };
        }

        [Test]
        public void Excute_Always_PublishesShowAndroidInstallGuideMessageWithTheGivenUriAsTheAndroidInstallLink()
        {
            _subject.Execute().Wait();
            
            _mockEventAggregator
                .Verify(ea => ea.Publish(
                    It.Is<ShowAndroidInstallGuideMessage>(message => message.AndroidInstallLink == _givenUri),
                    It.IsAny<Action<Action>>()), 
                Times.Once);
        }
    }
}