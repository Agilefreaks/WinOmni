using FluentAssertions;
using NUnit.Framework;
using OmniCommon;

namespace OmniCommonTests
{
    [TestFixture]
    public class ClipboardEventArgsTests
    {
        ClipboardEventArgs _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new ClipboardEventArgs("data");
        }

        [Test]
        public void Equals_OtherObjectHasSameData_ReturnsTrue()
        {
            _subject.Equals(new ClipboardEventArgs("data")).Should().BeTrue();
        }

        [Test]
        public void StaticEquals_BothItemsHaveSameData_ReturnsTrue()
        {
            ClipboardEventArgs.Equals(_subject, new ClipboardEventArgs("data")).Should().BeTrue();
        }

        [Test]
        public void StaticEquals_BothItemsAreNull_ReturnsTrue()
        {
            ClipboardEventArgs.Equals(null, null).Should().BeTrue();
        }

        [Test]
        public void StaticEquals_OneItemIsNullAndTheOtherNot_ReturnsFalse()
        {
            ClipboardEventArgs.Equals(null, _subject).Should().BeFalse();
        }

        [Test]
        public void StaticEquals_ItemsHaveDifferentData_ReturnsFalse()
        {
            ClipboardEventArgs.Equals(new ClipboardEventArgs("test"), _subject).Should().BeFalse();
        }
    }
}