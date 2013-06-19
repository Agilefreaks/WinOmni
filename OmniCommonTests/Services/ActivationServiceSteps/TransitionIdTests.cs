namespace OmniCommonTests.Services.ActivationServiceSteps
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Services.ActivationServiceData.Transitions;

    [TestFixture]
    public class TransitionIdTests
    {
        private TransitionId _subject;

        private int stateId;

        private int _state;

        [SetUp]
        public void Setup()
        {
            stateId = 1;
            _state = 2;
            _subject = new TransitionId(stateId, _state);
        }

        [Test]
        public void Equals_SecondIdHasEqualSourceStepTypeAndState_ReturnsTrue()
        {
            _subject.Equals(new TransitionId(stateId, _state)).Should().BeTrue();
        }

        [Test]
        public void Equals_SecondIdHasEqualSourceStepTypeButDifferentState_ReturnsFalse()
        {
            _subject.Equals(new TransitionId(stateId, 5)).Should().BeFalse();
        }

        [Test]
        public void Equals_SecondIdHasDifferentStepTypeButSameState_ReturnsFalse()
        {
            _subject.Equals(new TransitionId(3, _state)).Should().BeFalse();
        }

        [Test]
        public void Equals_SecondIdHasDifferentStepTypeAndState_ReturnsFalse()
        {
            _subject.Equals(new TransitionId(4, 5)).Should().BeFalse();
        }
    }
}