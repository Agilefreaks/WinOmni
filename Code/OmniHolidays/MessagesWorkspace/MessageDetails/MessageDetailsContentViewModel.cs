namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage;
    using OmniUI.ExtensionMethods;

    public class MessageDetailsContentViewModel : Conductor<IMessageStepViewModel>, IMessageDetailsContentViewModel
    {
        private IContactSource _contactSource;

        #region Constructors and Destructors

        public MessageDetailsContentViewModel(IEnumerable<IMessageStepViewModel> messageSteps)
        {
            CurrentMessageContext = new MessageContext();
            MessageSteps = messageSteps.ToList();
        }

        #endregion

        #region Public Properties

        public MessageContext CurrentMessageContext { get; private set; }

        public IList<IMessageStepViewModel> MessageSteps { get; private set; }

        #endregion

        #region Public Methods and Operators

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

        public void Reset()
        {
            GoToStep(MessageSteps[0]);
        }

        public IContactSource ContactSource
        {
            get
            {
                return _contactSource;
            }
            set
            {
                if (Equals(value, _contactSource))
                {
                    return;
                }
                _contactSource = value;
                CurrentMessageContext.Contacts = ContactSource.Contacts;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

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

            if (messageStepViewModel is ISendingMessageViewModel)
            {
                this.GetParentOfType<IMessageDetailsViewModel>()
                    .HeaderViewModel.SendMessage(CurrentMessageContext.Template);
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

        #endregion
    }
}