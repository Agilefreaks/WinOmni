namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation.Call;
    using Omnipaste.WorkspaceDetails.Conversation.Message;
    using OmniUI.List;

    public class ConversationContentViewModel : ListViewModelBase<IConversationItem, IConversationItemViewModel>,
                                                IConversationContentViewModel
    {
        #region Fields
        
        private readonly IConversationProvider _conversationProvider;

        private ContactInfoPresenter _model;

        private IConversationContext _conversationContext;

        #endregion

        public ConversationContentViewModel(IConversationProvider conversationProvider)
        {
            _conversationProvider = conversationProvider;

            FilteredItems.SortDescriptions.Add(new SortDescription(default(IConversationItemViewModel).GetPropertyName(vm => vm.Time), ListSortDirection.Ascending));
        }

        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        #endregion

        #region Properties

        protected override bool InsertItemsAtBottom
        {
            get
            {
                return true;
            }
        }

        public ContactInfoPresenter Model
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

        #endregion

        #region Methods

        protected override void OnInitialize()
        {
            _conversationContext = _conversationProvider.ForContact(Model.ContactInfo);
            base.OnInitialize();
        }

        protected override IConversationItemViewModel CreateViewModel(IConversationItem model)
        {
            IConversationItemViewModel result;
            if (model is Models.Call)
            {
                result = Kernel.Get<ICallViewModel>();
            }
            else
            {
                result = Kernel.Get<IMessageViewModel>();
            }
            result.Model = model;

            return result;
        }

        protected override IObservable<IConversationItem> GetItemAddedObservable()
        {
            return _conversationContext.ItemAdded;
        }

        protected override IObservable<IConversationItem> GetItemRemovedObservable()
        {
            return _conversationContext.ItemRemoved;
        }

        protected override IObservable<IEnumerable<IConversationItem>> GetFetchItemsObservable()
        {
            return _conversationContext.GetItems();
        }

        public override void AddItem(IConversationItem item)
        {
            if (item == null)
            {
                return;
            }

            base.AddItem(item);

            if (!item.WasViewed)
            {
                MarkConversationItemAsViewed(item);
                DismissConversationItemNotification(item);
            }
        }

        private void MarkConversationItemAsViewed(IConversationItem item)
        {
            item.WasViewed = true;
            _conversationContext.SaveItem(item).SubscribeAndHandleErrors();
        }

        private void DismissConversationItemNotification(IConversationItem item)
        {
            EventAggregator.PublishOnUIThread(new DismissNotification(item.UniqueId));
        }

        #endregion
    }
}