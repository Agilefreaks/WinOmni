namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;

    public class MessageDetailsContentViewModel : Conductor<IMessageStepViewModel>, IMessageDetailsContentViewModel
    {
        public MessageDetailsContentViewModel(IEnumerable<IMessageStepViewModel> messageSteps)
        {
            CurrentMessageContext = new MessageContext();
            MessageSteps = messageSteps.ToList();
        }

        public IList<IMessageStepViewModel> MessageSteps { get; set; }

        public MessageContext CurrentMessageContext { get; set; }

        public void GoToNextStep()
        {
            var currentStepIndex = MessageSteps.IndexOf(ActiveItem);
            if (currentStepIndex < MessageSteps.Count - 1)
            {
                GoToStep(MessageSteps[currentStepIndex + 1]);
            }
        }

        public void GoToPreviousStep()
        {
            var currentStepIndex = MessageSteps.IndexOf(ActiveItem);
            if (currentStepIndex > 0)
            {
                GoToStep(MessageSteps[currentStepIndex - 1]);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (MessageSteps.Any())
            {
                GoToStep(MessageSteps.First());
            }
        }

        private void GoToStep(IMessageStepViewModel messageStepViewModel)
        {
            if (ActiveItem != null)
            {
                ActiveItem.OnNext -= OnNext;
                ActiveItem.OnPrevious -= OnPrevious;
            }

            messageStepViewModel.MessageContext = CurrentMessageContext;
            messageStepViewModel.OnNext += OnNext;
            messageStepViewModel.OnPrevious += OnPrevious;

            ActivateItem(messageStepViewModel);
        }

        private void OnNext(object sender, EventArgs eventArgs)
        {
            GoToNextStep();
        }

        private void OnPrevious(object sender, EventArgs e)
        {
            GoToPreviousStep();
        }
    }
}