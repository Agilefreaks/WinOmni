namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetRemoteConfigurationTests
    {
        private GetRemoteConfiguration _subject;

        private Mock<IOAuth2> _mockOAuth2;

        [SetUp]
        public void Setup()
        {
            var mockKernel = new MoqMockingKernel();

            _mockOAuth2 = mockKernel.GetMock<IOAuth2>();

            _subject = new GetRemoteConfiguration(_mockOAuth2.Object)
                           {
                               Parameter = new DependencyParameter(string.Empty, "42")
                           };
        }
    }
}