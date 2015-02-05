namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System;
    using System.Linq;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation.Call;
    using Omnipaste.WorkspaceDetails.Conversation.Message;
    using OmniUI.Details;
    using OmniUI.List;

    public class ConversationContentViewModel : ListViewModelBase<IConversationItem, IDetailsViewModel>,
                                                IConversationContentViewModel
    {
        #region Fields

        private IDisposable _itemAddedObservable;

        private IDisposable _itemRemovedObservable;

        private ContactInfoPresenter _model;

        #endregion

        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        [Inject]
        public IConversationProvider ConversationProvider { get; set; }

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

        protected override IDetailsViewModel CreateViewModel(IConversationItem model)
        {
            IDetailsViewModel result;
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
            return ConversationProvider.ForContact(Model.ContactInfo).ItemAdded;
        }

        protected override IObservable<IConversationItem> GetItemRemovedObservable()
        {
            return ConversationProvider.ForContact(Model.ContactInfo).ItemRemoved;
        }

        protected override void OnActivate()
        {
            ConversationProvider.ForContact(Model.ContactInfo)
                .GetItems()
                .SubscribeAndHandleErrors(
                    items => items.OrderBy(conversationItem => conversationItem.Time).ForEach(DisplayItem));
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            DisposeItemAddedObservable();
            DisposeItemRemovedObservable();
            Items.ToList().Select(vm => vm.Model as IConversationItem).Where(model => model != null).ForEach(RemoveItem);

            base.OnDeactivate(true);
        }

        private void DisplayItem(IConversationItem item)
        {
            if (item == null)
            {
                return;
            }

            AddItem(item);

            if (!item.WasViewed)
            {
                MarkConversationItemAsViewed(item);
                DismissConversationItemNotification(item);
            }
        }

        private void MarkConversationItemAsViewed(IConversationItem item)
        {
            item.WasViewed = true;
            ConversationProvider.ForContact(Model.ContactInfo).Save(item);
        }

        private void DismissConversationItemNotification(IConversationItem item)
        {
            EventAggregator.PublishOnUIThread(new DismissNotification(item.UniqueId));
        }

        private void DisposeItemAddedObservable()
        {
            if (_itemAddedObservable != null)
            {
                _itemAddedObservable.Dispose();
                _itemAddedObservable = null;
            }
        }

        private void DisposeItemRemovedObservable()
        {
            if (_itemRemovedObservable != null)
            {
                _itemRemovedObservable.Dispose();
                _itemRemovedObservable = null;
            }
        }

        #endregion
    }
}