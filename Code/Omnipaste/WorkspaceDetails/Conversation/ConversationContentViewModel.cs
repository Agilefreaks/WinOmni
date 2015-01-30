namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Ninject;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation.Call;
    using Omnipaste.WorkspaceDetails.Conversation.Message;
    using OmniUI.Details;
    using OmniUI.List;

    public class ConversationContentViewModel : ListViewModelBase<IConversationItem, IDetailsViewModel>,
                                                IConversationContentViewModel
    {
        #region Fields

        private ContactInfo _contactInfo;

        private IDisposable _itemAddedObservable;

        private IDisposable _itemRemovedObservable;

        #endregion

        #region Public Properties

        [Inject]
        public ICallRepository CallRepository { get; set; }

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

        #endregion

        #region Properties

        protected override bool InsertItemsAtBottom
        {
            get
            {
                return true;
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
            return
                MessageRepository.OperationObservable.Created()
                    .ForContact(ContactInfo)
                    .Select(o => o.Item)
                    .Merge(
                        CallRepository.OperationObservable.Created()
                            .ForContact(ContactInfo)
                            .Select(o => o.Item)
                            .Cast<IConversationItem>());
        }

        protected override IObservable<IConversationItem> GetItemRemovedObservable()
        {
            return
                MessageRepository.OperationObservable.Deleted()
                    .ForContact(ContactInfo)
                    .Select(o => o.Item)
                    .Merge(
                        CallRepository.OperationObservable.Deleted()
                            .ForContact(ContactInfo)
                            .Select(o => o.Item)
                            .Cast<IConversationItem>());
        }

        protected override void OnActivate()
        {
            MessageRepository.GetByContact(ContactInfo)
                .Merge(CallRepository.GetByContact(ContactInfo).Select(i => i.Cast<IConversationItem>()))
                .Buffer(2)
                .Subscribe(
                    itemLists =>
                    itemLists.SelectMany(i => i.ToList())
                        .OrderBy(conversationItem => conversationItem.Time)
                        .ForEach(AddItem));
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            DisposeItemAddedObservable();
            DisposeItemRemovedObservable();
            Items.ToList().Select(vm => vm.Model as IConversationItem).Where(model => model != null).ForEach(RemoveItem);

            base.OnDeactivate(true);
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