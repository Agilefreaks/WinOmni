namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Services.ActivationServiceData.Transitions;

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
        public void EqualsSecondIdHasEqualSourceStepTypeAndStateReturnsTrue()
        {
            this._subject.Equals(new TransitionId(this._stateId, this._state)).Should().BeTrue();
        }

        [Test]
        public void EqualsSecondIdHasEqualSourceStepTypeButDifferentStateReturnsFalse()
        {
            this._subject.Equals(new TransitionId(this._stateId, 5)).Should().BeFalse();
        }

        [Test]
        public void EqualsSecondIdHasDifferentStepTypeButSameStateReturnsFalse()
        {
            this._subject.Equals(new TransitionId(3, this._state)).Should().BeFalse();
        }

        [Test]
        public void EqualsSecondIdHasDifferentStepTypeAndStateReturnsFalse()
        {
            this._subject.Equals(new TransitionId(4, 5)).Should().BeFalse();
        }
    }
}