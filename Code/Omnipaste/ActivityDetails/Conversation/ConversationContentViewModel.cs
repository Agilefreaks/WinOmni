﻿namespace Omnipaste.ActivityDetails.Conversation
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ActivityDetails.Conversation.Call;
    using Omnipaste.ActivityDetails.Conversation.Message;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;
    using OmniUI.Models;

    public class ConversationContentViewModel : Conductor<IScreen>.Collection.AllActive, IConversationContentViewModel
    {
        #region Fields

        private IDisposable _callSubscription;

        private ContactInfo _contactInfo;

        private IDisposable _messageSubscription;

        #endregion

        #region Public Properties

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
        public IMessageRepository MessageRepository { get; set; }

        [Inject]
        public ICallRepository CallRepository { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();

            MessageRepository.GetByContact(ContactInfo)
                .Select(messages => messages.Select(CreateMessageViewModel))
                .Merge(
                    CallRepository.GetByContact(ContactInfo)
                        .Select(calls => calls.Select(CreateCallViewModel).Cast<IDetailsViewModel>()))
                .Buffer(2)
                .Subscribe(
                    itemLists =>
                        {
                            itemLists.SelectMany(i => i.ToList())
                                .OrderBy(vm => ((IConversationItem)vm.Model).Time)
                                .ForEach(ActivateItem);
                        });

            _messageSubscription =
                MessageRepository.OperationObservable.Saved()
                    .ForContact(ContactInfo)
                    .SubscribeAndHandleErrors(o => ActivateItem(CreateMessageViewModel(o.Item)));
            _callSubscription =
                CallRepository.OperationObservable.Saved()
                    .ForContact(ContactInfo)
                    .SubscribeAndHandleErrors(o => ActivateItem(CreateCallViewModel(o.Item)));
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