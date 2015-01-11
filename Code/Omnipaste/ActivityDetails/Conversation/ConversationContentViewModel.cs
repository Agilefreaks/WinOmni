namespace Omnipaste.ActivityDetails.Conversation
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ActivityDetails.Conversation.Call;
    using Omnipaste.ActivityDetails.Conversation.Message;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;
    using OmniUI.List;
    using OmniUI.Models;

    public class ConversationContentViewModel : ListViewModelBase<IConversationItem, IDetailsViewModel>, IConversationContentViewModel
    {
        #region Fields

        private IDisposable _itemRemovedObservable;

        private ContactInfo _contactInfo;

        private IDisposable _itemAddedObservable;

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
                .Merge(CallRepository.GetByContact(ContactInfo).Select(i => i.Cast<IConversationItem>()))
                .Buffer(2)
                .Subscribe(
                    itemLists =>
                        {
                            itemLists.SelectMany(i => i.ToList())
                                .OrderBy(conversationItem => conversationItem.Time)
                                .ForEach(AddItem);
                        });

            DisposeItemAddedObservable();
            _itemAddedObservable = GetItemAddedObservable().SubscribeAndHandleErrors(AddItem);
            
            DisposeItemRemovedObservable();
            _itemRemovedObservable = GetItemRemovedObservable().SubscribeAndHandleErrors(RemoveItem);
        }

        private IObservable<IConversationItem> GetItemAddedObservable()
        {
            return
                MessageRepository.OperationObservable.Saved()
                    .ForContact(ContactInfo)
                    .Select(o => o.Item)
                    .Merge(
                        CallRepository.OperationObservable.Saved()
                            .ForContact(ContactInfo)
                            .Select(o => o.Item)
                            .Cast<IConversationItem>());
        }

        private IObservable<IConversationItem> GetItemRemovedObservable()
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

        protected override void OnDeactivate(bool close)
        {
            DisposeItemAddedObservable();
            DisposeItemRemovedObservable();
            Items.ToList().Select(vm => vm.Model as IConversationItem).Where(model => model != null).ForEach(RemoveItem);

            base.OnDeactivate(true);
        }

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

        private void DisposeItemRemovedObservable()
        {
            if (_itemRemovedObservable != null)
            {
                _itemRemovedObservable.Dispose();
                _itemRemovedObservable = null;
            }
        }

        private void DisposeItemAddedObservable()
        {
            if (_itemAddedObservable != null)
            {
                _itemAddedObservable.Dispose();
                _itemAddedObservable = null;
            }
        }

        #endregion
    }
}