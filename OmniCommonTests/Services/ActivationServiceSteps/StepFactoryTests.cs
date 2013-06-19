namespace OmniCommonTests.Services.ActivationServiceSteps
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Services.ActivationServiceData;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class StepFactoryTests
    {
        private StepFactory _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new StepFactory();
        }

        [Test]
        public void Create_TypeIsNull_ReturnsNull()
        {
            _subject.Create(null).Should().BeNull();
        }

        [Test]
        public void Create_TypeIsImplementationOfIActivationStep_ReturnsInstanceOfType()
        {
            _subject.Create(typeof(Start)).Should().BeOfType<Start>();
        }

        [Test]
        public void Create_TypeIsNotImplementationOfIActivationStep_ReturnsNull()
        {
            _subject.Create(typeof(string)).Should().BeNull();
        }
    }
}