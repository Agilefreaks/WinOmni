namespace OmniHolidaysTests.MessagesWorkspace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniHolidays.MessagesWorkspace.MessageDetails;

    [TestFixture]
    public class MessageDetailsContentViewModelTests
    {
        private MessageDetailsContentViewModel _subject;

        private List<IMessageStepViewModel> _messageStepViews;

        public class TestMessageStepView : Screen, IMessageStepViewModel
        {
            public event EventHandler<EventArgs> OnNext;

            public event EventHandler<EventArgs> OnPrevious;

            public MessageContext MessageContext { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _messageStepViews = new List<IMessageStepViewModel>
                                    {
                                        new TestMessageStepView(),
                                        new TestMessageStepView()
                                    };
            _subject = new MessageDetailsContentViewModel(_messageStepViews);

            ((IActivate)_subject).Activate();
        }

        [Test]
        public void Constructor_Always_InitializesMessageContext()
        {
            _subject.CurrentMessageContext.Should().NotBeNull();
        }

        [Test]
        public void GoToNextStep_WhenCurrentItemIsNotLast_ChangesActiveItemToLastStep()
        {
            _subject.ActivateItem(_messageStepViews.First());

            _subject.GoToNextStep();

            _subject.ActiveItem.Should().Be(_messageStepViews.Last());
        }

        [Test]
        public void GoToNextStep_WhenCurrentItemIsLast_DoesNotChangeActiveItem()
        {
            _subject.ActivateItem(_messageStepViews.Last());

            _subject.GoToNextStep();

            _subject.ActiveItem.Should().Be(_messageStepViews.Last());
        }

        [Test]
        public void GoToPreviousStep_WhenCurrentItemIsNotFirst_ChangesActiveItemToPreviousStep()
        {
            _subject.ActivateItem(_messageStepViews.Last());

            _subject.GoToPreviousStep();

            _subject.ActiveItem.Should().Be(_messageStepViews.First());
        }

        [Test]
        public void GoToPreviousStep_WhenCurrentItemIstFirst_DoestNotChangeActiveItem()
        {
            _subject.ActivateItem(_messageStepViews.First());

            _subject.GoToPreviousStep();

            _subject.ActiveItem.Should().Be(_messageStepViews.First());
        }

        [Test]
        public void GoToNextStep_Always_SetsSelfOnNewStep()
        {
            _subject.ActivateItem(_messageStepViews.First());

            _subject.GoToNextStep();

            _subject.ActiveItem.MessageContext.Should().Be(_subject.CurrentMessageContext);
        }
    }
}
