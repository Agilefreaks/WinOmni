namespace Omnipaste.Conversations.Conversation
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Conversations.Conversation.Message;
    using Omnipaste.Conversations.Conversation.PhoneCall;
    using Omnipaste.Framework.EventAggregatorMessages;
    using Omnipaste.Framework.ExtensionMethods;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services.Providers;
    using Omnipaste.Framework.Services.Repositories;
    using OmniUI.Framework.Models;
    using OmniUI.List;

    public class ConversationContentViewModel : ListViewModelBase<IConversationModel, IConversationItemViewModel>,
                                                IConversationContentViewModel
    {
        public ConversationContentViewModel(IConversationProvider conversationProvider)
        {
            _conversationProvider = conversationProvider;

            FilteredItems.SortDescriptions.Add(
                new SortDescription(
                    PropertyExtensions.GetPropertyName<IConversationItemViewModel, DateTime>(vm => vm.Time),
                    ListSortDirection.Ascending));
        }

        private readonly IConversationProvider _conversationProvider;

        private ContactModel _model;

        private IConversationContext _conversationContext;

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        protected override bool InsertItemsAtBottom
        {
            get
            {
                return true;
            }
        }

        public ContactModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (Equals(value, _model))
                {
                    return;
                }
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        public void RefreshConversation()
        {
            Items.Clear();
            Subscriptions.ClearAll();
            OnInitialize();
        }

        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            if (propertyName == "Model")
            {
                RefreshConversation();
            }
        }

        protected override void OnInitialize()
        {
            _conversationContext = Model != null
                                       ? _conversationProvider.ForContact(Model.BackingEntity)
                                       : _conversationProvider.All();
            base.OnInitialize();
        }

        protected override IConversationItemViewModel ChangeViewModel(IConversationModel model)
        {
            return UpdateViewModel(model) ?? CreateViewModel(model);
        }

        private IConversationItemViewModel CreateViewModel(IConversationModel model)
        {
            IConversationItemViewModel result;
            if (model is PhoneCallModel)
            {
                result = Kernel.Get<IPhoneCallViewModel>();
            }
            else
            {
                result = Kernel.Get<IMessageViewModel>();
            }
            result.Model = model;
            HandleNotViewedItem(model);

            return result;
        }

        private IConversationItemViewModel UpdateViewModel(IConversationModel model)
        {
            var result =
                (IConversationItemViewModel)
                Items.OfType<IPhoneCallViewModel>()
                    .FirstOrDefault(vm => vm.Model.Id == model.Id && vm.Model.Source == model.Source)
                ?? Items.OfType<IMessageViewModel>()
                       .FirstOrDefault(vm => vm.Model.Id == model.Id && vm.Model.Source == model.Source);

            return result;
        }

        protected override IObservable<IConversationModel> GetItemChangedObservable()
        {
            return _conversationContext.ItemChanged;
        }

        protected override IObservable<IConversationModel> GetItemRemovedObservable()
        {
            return _conversationContext.ItemRemoved;
        }

        protected override IObservable<IConversationModel> GetFetchItemsObservable()
        {
            return _conversationContext.GetItems();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            foreach (var item in Items)
            {
                HandleNotViewedItem((IConversationModel)item.Model);
            }
        }

<<<<<<< HEAD:Code/Omnipaste/Conversations/Conversation/ConversationContentViewModel.cs
        private void MarkConversationItemAsViewed(IConversationModel item)
=======
        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            if (propertyName == "Model")
            {
                RefreshConversation();
            }
        }

        private void MarkConversationItemAsViewed(IConversationPresenter item)
>>>>>>> Fixes selection problems:Code/Omnipaste/WorkspaceDetails/Conversation/ConversationContentViewModel.cs
        {
            item.WasViewed = true;
            _conversationContext.SaveItem(item).SubscribeAndHandleErrors();
        }

        private void DismissConversationItemNotification(IModel item)
        {
            EventAggregator.PublishOnUIThread(new DismissNotification(item.UniqueId));
        }

        private void HandleNotViewedItem(IConversationModel item)
        {
            if (item.WasViewed || !IsActive)
            {
                return;
            }
            MarkConversationItemAsViewed(item);
            DismissConversationItemNotification(item);
        }
    }
}