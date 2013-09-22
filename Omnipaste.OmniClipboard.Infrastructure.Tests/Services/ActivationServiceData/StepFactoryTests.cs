namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Services.ActivationServiceData
{
    using FluentAssertions;
    using NUnit.Framework;
    using Ninject.MockingKernel.Moq;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

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
        public void Create_TypeIsImplementationOfIActivationStep_ReturnsTheResultOfGetType()
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