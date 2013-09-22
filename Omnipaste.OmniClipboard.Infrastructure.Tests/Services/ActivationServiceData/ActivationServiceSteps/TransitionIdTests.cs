namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.Transitions;

    [TestFixture]
    public class TransitionIdTests
    {
        private TransitionId _subject;

        private int _stateId;

        private int _state;

        [SetUp]
        public void Setup()
        {
            this._stateId = 1;
            this._state = 2;
            this._subject = new TransitionId(this._stateId, this._state);
        }

        [Test]
        public void Equals_SecondIdHasEqualSourceStepTypeAndState_ReturnsTrue()
        {
            this._subject.Equals(new TransitionId(this._stateId, this._state)).Should().BeTrue();
        }

        [Test]
        public void Equals_SecondIdHasEqualSourceStepTypeButDifferentState_ReturnsFalse()
        {
            this._subject.Equals(new TransitionId(this._stateId, 5)).Should().BeFalse();
        }

        [Test]
        public void Equals_SecondIdHasDifferentStepTypeButSameState_ReturnsFalse()
        {
            this._subject.Equals(new TransitionId(3, this._state)).Should().BeFalse();
        }

        [Test]
        public void Equals_SecondIdHasDifferentStepTypeAndState_ReturnsFalse()
        {
            this._subject.Equals(new TransitionId(4, 5)).Should().BeFalse();
        }
    }
}