using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using OmniCommon.Domain;
using OmniCommon.Services;

namespace OmniCommonTests.Services
{
    public class InMemoryClippingRepositoryTests
    {
        private InMemoryClippingRepository _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new InMemoryClippingRepository();
        }

        [Test]
        public void Save_Always_AddsClip()
        {
            var clipping = new Clipping("Test");

            _subject.Save(clipping);

            _subject.GetAll().Any(c => c.Content == clipping.Content).Should().BeTrue();
        }
    }
}
