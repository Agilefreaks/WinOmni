namespace OmniCommonTests.Services.ActivationServiceData
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Services.ActivationServiceData;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class StepFactoryTests
    {
        private StepFactory _subject;

        private Mock<IDependencyResolver> _mockDependencyResolver;

        [SetUp]
        public void Setup()
        {
            _mockDependencyResolver = new Mock<IDependencyResolver>();
            _subject = new StepFactory(_mockDependencyResolver.Object);
        }

        [Test]
        public void Create_TypeIsNull_ReturnsNull()
        {
            _subject.Create(null).Should().BeNull();
        }

        [Test]
        public void Create_TypeIsImplementationOfIActivationStep_ReturnsTheResultOfGetType()
        {
            var result = new Start();
            _mockDependencyResolver.Setup(x => x.Get(typeof(Start))).Returns(result);

            _subject.Create(typeof(Start)).Should().Be(result);
        }

        [Test]
        public void Create_TypeIsNotImplementationOfIActivationStep_ReturnsNull()
        {
            _mockDependencyResolver.Setup(x => x.Get(typeof(string))).Returns("test");

            _subject.Create(typeof(string)).Should().BeNull();
        }
    }
}