namespace OmnipasteTests.WorkspaceDetails.Clipping
{
    using System;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Clipping;

    [TestFixture]
    public class ClippingDetailsContentViewModelTests
    {
        private ClippingDetailsContentViewModel _subject;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<IEventAggregator> _mockEventAggregator;

        [SetUp]
        public void SetUp()
        {
            _mockEventAggregator = new Mock<IEventAggregator> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new ClippingDetailsContentViewModel
                           {
                               EventAggregator = _mockEventAggregator.Object,
                               ClippingRepository = _mockClippingRepository.Object
                           };
        }
        
        [Test]
        public void Activate_WhenModelWasNotViewed_DismissesNotificationForActivity()
        {
            const string Identifier = "42";
            var activity = new ClippingPresenter(new ClippingModel { UniqueId = Identifier, WasViewed = false });
            _subject.Model = activity;

            ((IActivate)_subject).Activate();

            _mockEventAggregator.Verify(m => m.Publish(It.Is<DismissNotification>(o => Identifier.Equals(o.Identifier)), It.IsAny<Action<Action>>()));
        }

        [Test]
        public void ShowDetails_WhenModelWasNotViewed_SetsModelWasViewedToTrue()
        {
            var activity = new ClippingPresenter(new ClippingModel { WasViewed = false });
            _subject.Model = activity;

            ((IActivate)_subject).Activate();

            _subject.Model.WasViewed.Should().BeTrue();
        }

        [Test]
        public void ShowDetails_WhenModelWasNotViewed_SavesModel()
        {
            var activity = new ClippingPresenter(new ClippingModel { WasViewed = false });
            _subject.Model = activity;
            
            ((IActivate)_subject).Activate();

            _mockClippingRepository.Verify(m => m.Save(activity.BackingModel));
        }
    }
}
