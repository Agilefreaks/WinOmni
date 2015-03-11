namespace OmnipasteTests.Helpers
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Helpers;

    [TestFixture]
    public class PhoneNumberMatcherTests
    {
        [Test]
        public void IsMatch_WhenPhoneNumber1IsSameAsPhoneNumber2_ReturnsTrue()
        {
            PhoneNumberMatcher.IsMatch("0766123123", "0766123123").Should().BeTrue();
        }

        [Test]
        public void IsMatch_WhenPhoneNumber1IsDifferentThanPhoneNumber2_ReturnsFalse()
        {
            PhoneNumberMatcher.IsMatch("0766123123", "7777777777").Should().BeFalse();
        }

        [Test]
        public void IsMatch_WhenPhoneNumber1IsPhoneNumber2AndAPrefix_ReturnsTrue()
        {
            PhoneNumberMatcher.IsMatch("0766123123", "+40766123123").Should().BeTrue();
        }

        [Test]
        public void IsMatch_WhenPhoneNumber2IsPhoneNumber1AndAPrefix_ReturnsTrue()
        {
            PhoneNumberMatcher.IsMatch("+40766123123", "0766123123").Should().BeTrue();
        }

        [Test]
        public void IsMatch_WhenPhoneNumber1ContainsExtraSpaces_ReturnsTrue()   
        {
            PhoneNumberMatcher.IsMatch("+4 0766 12 31 23", "0766123123").Should().BeTrue();
        }

        [Test]
        public void IsMatch_WhenPhoneNumber1ContainsParantheses_ReturnsTrue()
        {
            PhoneNumberMatcher.IsMatch("(+4 0766) 12 31 23", "0766123123").Should().BeTrue();
        }
    }
}
