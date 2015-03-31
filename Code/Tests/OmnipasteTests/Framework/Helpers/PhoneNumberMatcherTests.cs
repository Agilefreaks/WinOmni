namespace OmnipasteTests.Framework.Helpers
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Helpers;

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
    }
}
