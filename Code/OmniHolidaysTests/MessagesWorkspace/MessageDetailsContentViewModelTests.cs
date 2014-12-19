namespace OmniHolidaysTests.MessagesWorkspace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniHolidays.Commands;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage;
    using OmniUI.Framework;
    using OmniUI.Models;
    using OmniUI.Presenters;
    using OmniUI.Services;

    [TestFixture]
    public class MessageDetailsContentViewModelTests
    {
        private MessageDetailsContentViewModel _subject;

        private List<IMessageStepViewModel> _messageStepViewModels;

        public class TestMessageStepView : Screen, IMessageStepViewModel
        {
            public event EventHandler<EventArgs> OnNext;

            public event EventHandler<EventArgs> OnPrevious;

            public MessageContext MessageContext { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _messageStepViewModels = new List<IMessageStepViewModel>
                                    {
                                        new TestMessageStepView(),
                                        new TestMessageStepView()
                                    };
            _subject = new MessageDetailsContentViewModel(_messageStepViewModels);

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
            _subject.ActivateItem(_messageStepViewModels.First());

            _subject.GoToNextStep();

            _subject.ActiveItem.Should().Be(_messageStepViewModels.Last());
        }

        [Test]
        public void GoToNextStep_WhenCurrentItemIsLast_DoesNotChangeActiveItem()
        {
            _subject.ActivateItem(_messageStepViewModels.Last());

            _subject.GoToNextStep();

            _subject.ActiveItem.Should().Be(_messageStepViewModels.Last());
        }

        [Test]
        public void GoToPreviousStep_WhenCurrentItemIsNotFirst_ChangesActiveItemToPreviousStep()
        {
            _subject.ActivateItem(_messageStepViewModels.Last());

            _subject.GoToPreviousStep();

            _subject.ActiveItem.Should().Be(_messageStepViewModels.First());
        }

        [Test]
        public void GoToPreviousStep_WhenCurrentItemIstFirst_DoestNotChangeActiveItem()
        {
            _subject.ActivateItem(_messageStepViewModels.First());

            _subject.GoToPreviousStep();

            _subject.ActiveItem.Should().Be(_messageStepViewModels.First());
        }

        [Test]
        public void GoToNextStep_Always_SetsSelfOnNewStep()
        {
            _subject.ActivateItem(_messageStepViewModels.First());

            _subject.GoToNextStep();

            _subject.ActiveItem.MessageContext.Should().Be(_subject.CurrentMessageContext);
        }

        [Test]
        public void GoToNextStep_NextStepIsSendingMessageViewModel_CallsSendMessageOnTheHeaderViewModel()
        {
            _subject.CurrentMessageContext.Template = "someTemplate";
            _subject.MessageSteps.Add(new Mock<ISendingMessageViewModel>().Object);
            var mockMessageDetails = new Mock<IMessageDetailsViewModel>();
            var mockMessageDetailsHeader = new Mock<IMessageDetailsHeaderViewModel>();
            mockMessageDetails.SetupGet(x => x.HeaderViewModel).Returns(mockMessageDetailsHeader.Object);
            _subject.Parent = mockMessageDetails.Object;

            _subject.GoToNextStep();
            _subject.GoToNextStep();
            _subject.GoToNextStep();

            mockMessageDetailsHeader.Verify(x => x.SendMessage("someTemplate"), Times.Once());
        }
    }
}
