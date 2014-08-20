namespace OmnipasteTests.ClippingList
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.MasterClippingList;

    [TestFixture]
    public class LimitableBindableCollectionTests
    {
        private LimitableBindableCollection<int> _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new LimitableBindableCollection<int>(2);
        }

        [Test]
        public void Add_WhenThereAreNoElements_AddsTheItem()
        {
            _subject.Add(1);

            _subject.Should().HaveCount(1);
        }

        [Test]
        public void Add_WhenThereAreLimitMinusOneElements_AddsTheItem()
        {
            _subject.Add(1);

            _subject.Add(2);

            _subject.Should().HaveCount(2);
        }

        [Test]
        public void Add_WhenTheLimitIsAlreadyReached_DoesNotAddTheItem()
        {
            _subject.Add(1);
            _subject.Add(2);
            
            _subject.Add(3);

            _subject.Should().NotContain(3);
            _subject.Should().HaveCount(2);
        }

        [Test]
        public void Insert_InFirstPositionWhenTheLimitIsReached_RemovesItemsFromTheTailOfTheCollection()
        {
            _subject.Add(1);
            _subject.Add(2);

            _subject.Insert(0, 3);

            _subject.Should().Contain(3);
            _subject.Should().HaveCount(2);
            _subject.Should().NotContain(2);
        }
    }
}