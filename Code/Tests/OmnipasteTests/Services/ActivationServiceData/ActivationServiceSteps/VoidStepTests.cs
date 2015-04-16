namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class VoidStepTests
    {
        public class VoidStepTest : VoidStep
        {
            public override SimpleStepStateEnum State
            {
                get
                {
                    return SimpleStepStateEnum.Successful;
                }
            }
        }

        [Test]
        public void Execute_Returns_State()
        {
            var mockObserver = new TestScheduler().CreateObserver<IExecuteResult>();
            
            (new VoidStepTest()).Execute().Subscribe(mockObserver);

            mockObserver.Messages.Should().HaveCount(2);
            mockObserver.Messages.Should().Contain(r => r.Value.Kind == NotificationKind.OnNext);
            mockObserver.Messages.Should().Contain(r => r.Value.Kind == NotificationKind.OnCompleted);
        }
    }
}