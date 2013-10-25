namespace OmnipasteTests.Services.ActivationServiceData
{
    using FluentAssertions;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class StepFactoryTests
    {
        private StepFactory _subject;

        [SetUp]
        public void Setup()
        {
            this._subject = new StepFactory();
        }

        [Test]
        public void CreateTypeIsImplementationOfIActivationStepReturnsTheResultOfGetType()
        {
            using (var kernel = new MoqMockingKernel())
            {
                var start = new Start();
                kernel.Bind<Start>().ToConstant(start);

                this._subject.Kernel = kernel;

                this._subject.Create(typeof(Start)).Should().Be(start);
            }
        }
    }
}