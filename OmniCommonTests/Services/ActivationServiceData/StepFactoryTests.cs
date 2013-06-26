namespace OmniCommonTests.Services.ActivationServiceData
{
    using FluentAssertions;
    using NUnit.Framework;
    using Ninject.MockingKernel.Moq;
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
        public void Create_TypeIsImplementationOfIActivationStep_ReturnsTheResultOfGetType()
        {
            using (var kernel = new MoqMockingKernel())
            {
                var start = new Start();
                kernel.Bind<Start>().ToConstant(start);

                _subject.Kernel = kernel;

                _subject.Create(typeof(Start)).Should().Be(start);
            }
        }
    }
}