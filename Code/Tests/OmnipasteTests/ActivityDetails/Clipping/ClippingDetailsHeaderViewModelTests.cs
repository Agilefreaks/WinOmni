namespace OmnipasteTests.ActivityDetails.Clipping
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.ActivityDetails.Clipping;
    using Omnipaste.Presenters;

    [TestFixture]
    public class ClippingDetailsHeaderViewModelTests
    {
        private ClippingDetailsHeaderViewModel _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new ClippingDetailsHeaderViewModel
                           {
                               Model = new ActivityPresenter()
                           };
        }

        [Test]
        public void DeleteClipping_Always_MarksTheCurrentModelForDeletion()
        {
            _subject.DeleteClipping();

            _subject.Model.MarkedForDeletion.Should().BeTrue();
        }

        [Test]
        public void DeleteClipping_Always_SetsViewModelStateToDeleted()
        {
            _subject.DeleteClipping();

            _subject.State.Should().Be(ClippingDetailsHeaderStateEnum.Deleted);
        }

        [Test]
        public void UndoDelete_Always_ClearsTheDeletionMarkFromTheModel()
        {
            _subject.UndoDelete();

            _subject.Model.MarkedForDeletion.Should().BeFalse();
        }

        [Test]
        public void UndoDelete_Always_SetsTheViewModelStateToNormal()
        {
            _subject.UndoDelete();

            _subject.State.Should().Be(ClippingDetailsHeaderStateEnum.Normal);
        }
    }
}