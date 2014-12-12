namespace Omnipaste.ActivityDetails.Conversation
{
    using System;
    using System.Linq;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ActivityDetails.Conversation.Call;
    using Omnipaste.ActivityDetails.Conversation.Message;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class ConversationContentViewModel : Conductor<IScreen>.Collection.AllActive, IConversationContentViewModel
    {
        #region Fields

        private IDisposable _callSubscription;

        private ContactInfo _contactInfo;

        private IDisposable _messageSubscription;

        #endregion

        #region Public Properties

        [Inject]
        public ICallStore CallStore { get; set; }

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
        public IKernel Kernel { get; set; }

        [Inject]
        public IMessageStore MessageStore { get; set; }

        #endregion

        #region Methods

        protected override IScreen EnsureItem(IScreen newItem)
        {
            var index = Items.IndexOf(newItem);

            if (index == -1)
            {
                Items.Insert(0, newItem);
            }
            else
            {
                newItem = Items[index];
            }

            return base.EnsureItem(newItem);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _messageSubscription =
                MessageStore.MessageObservable.SubscribeAndHandleErrors(
                    message => ActivateItem(CreateMessageViewModel(message)));
            _callSubscription =
                CallStore.CallObservable.SubscribeAndHandleErrors(message => ActivateItem(CreateCallViewModel(message)));

            MessageStore.GetRelatedMessages(ContactInfo)
                .Select(CreateMessageViewModel)
                .Concat(CallStore.GetRelatedCalls(ContactInfo).Select(CreateCallViewModel).Cast<IDetailsViewModel>())
                .OrderBy(screen => ((IHaveTimestamp)screen.Model).Time)
                .ForEach(ActivateItem);
        }

        protected override void OnDeactivate(bool close)
        {
            if (_messageSubscription != null)
            {
                _messageSubscription.Dispose();
                _messageSubscription = null;
            }

            if (_callSubscription != null)
            {
                _callSubscription.Dispose();
                _callSubscription = null;
            }

            base.OnDeactivate(close);
        }

        private ICallViewModel CreateCallViewModel(Models.Call call)
        {
            var viewModel = Kernel.Get<ICallViewModel>();
            viewModel.Model = call;

            return viewModel;
        }

        private IMessageViewModel CreateMessageViewModel(Models.Message message)
        {
            var viewModel = Kernel.Get<IMessageViewModel>();
            viewModel.Model = message;

            return viewModel;
        }

        #endregion
    }
}