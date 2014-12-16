﻿namespace Omnipaste.ActivityDetails.Conversation
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

        protected override void OnActivate()
        {
            base.OnActivate();

            MessageStore.GetRelatedMessages(ContactInfo)
                .Select(CreateMessageViewModel)
                .Concat(CallStore.GetRelatedCalls(ContactInfo).Select(CreateCallViewModel).Cast<IDetailsViewModel>())
                .OrderBy(screen => ((IConversationItem)screen.Model).Time)
                .ForEach(ActivateItem);
            _messageSubscription =
                MessageStore.MessageObservable.SubscribeAndHandleErrors(
                    message => ActivateItem(CreateMessageViewModel(message)));
            _callSubscription =
                CallStore.CallObservable.SubscribeAndHandleErrors(message => ActivateItem(CreateCallViewModel(message)));
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

            base.OnDeactivate(true);
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