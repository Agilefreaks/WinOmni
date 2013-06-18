using System;
using Moq;
using NUnit.Framework;
using Omnipaste.Services;

namespace OmnipasteTests.Services
{
    [TestFixture]
    public class ClickOnceActivationDataProviderTests
    {
        private ClickOnceActivationDataProvider _subject;
        private Mock<IApplicationDeploymentInfoProvider> _mockActivationInfoProvider;

        [SetUp]
        public void Setup()
        {
            _mockActivationInfoProvider = new Mock<IApplicationDeploymentInfoProvider>();
            _subject = new ClickOnceActivationDataProvider(_mockActivationInfoProvider.Object);
        }

        [Test]
        public void GetCommunicationChannel_ApplicationDeploymentInfoHasValidActivationUri_CallsApplicationDeploymentInfoActivationUri()
        {
            _mockActivationInfoProvider.Setup(x => x.HasValidActivationUri).Returns(true);
            _mockActivationInfoProvider.Setup(x => x.ActivationUri).Returns(new Uri("http://test.com"));

            _subject.GetActivationData();

            _mockActivationInfoProvider.Verify(x => x.ActivationUri);
        }
    }
}
