namespace Omnipaste.ActivityDetails.Message
{
    using System;
    using System.Linq;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class ConversationViewModel : Conductor<IMessageViewModel>.Collection.AllActive, IConversationViewModel
    {
        private ContactInfo _contactInfo;

        private IDisposable _messageSubscription;

        public ContactInfo ContactInfo
        {
            get
            {
                return _contactInfo;
            }
            set
            {
                if (Equals(value, _contactInfo))
                {
                    return;
                }
                _contactInfo = value;
                NotifyOfPropertyChange();
            }
        }

        [Inject]
        public IMessageStore MessageStore { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            _messageSubscription =
                MessageStore.MessageObservable.SubscribeAndHandleErrors(
                    message => ActivateItem(CreateMessageViewModel(message)));
            MessageStore.Messages[ContactInfo.Phone].Select(CreateMessageViewModel).ForEach(ActivateItem);
        }

        protected override void OnDeactivate(bool close)
        {
            if (_messageSubscription != null)
            {
                _messageSubscription.Dispose();
                _messageSubscription = null;
            }

            base.OnDeactivate(close);
        }

        private IMessageViewModel CreateMessageViewModel(Models.Message message)
        {
            var viewModel = Kernel.Get<IMessageViewModel>();
            viewModel.Model = message;

            return viewModel;
        }
    }
}