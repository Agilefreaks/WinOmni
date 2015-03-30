namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails.Conversation.Message;
    using Omnipaste.WorkspaceDetails.Conversation.PhoneCall;
    using OmniUI.List;
    using OmniUI.Presenters;

    public class ConversationContentViewModel : ListViewModelBase<IConversationPresenter, IConversationItemViewModel>,
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

            FilteredItems.SortDescriptions.Add(new SortDescription(PropertyExtensions.GetPropertyName<IConversationItemViewModel, DateTime>(vm => vm.Time), ListSortDirection.Ascending));
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
            _conversationContext = _conversationProvider.ForContact(Model.BackingModel);
            base.OnInitialize();
        }

        protected override IConversationItemViewModel ChangeViewModel(IConversationPresenter model)
        {
            return UpdateViewModel(model) ?? CreateViewModel(model);
        }

        private IConversationItemViewModel CreateViewModel(IConversationPresenter model)
        {
            IConversationItemViewModel result;
            if (model is PhoneCallPresenter)
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

        private IConversationItemViewModel UpdateViewModel(IConversationPresenter model)
        {
            IConversationItemViewModel result =
                (IConversationItemViewModel)Items.OfType<IPhoneCallViewModel>().FirstOrDefault(vm => vm.Model.Id == model.Id && vm.Model.Source == model.Source) ??
                Items.OfType<IMessageViewModel>().FirstOrDefault(vm => vm.Model.Id == model.Id && vm.Model.Source == model.Source);

            return result;
        }

        protected override IObservable<IConversationPresenter> GetItemChangedObservable()
        {
            return _conversationContext.ItemChanged;
        }

        protected override IObservable<IConversationPresenter> GetItemRemovedObservable()
        {
            return _conversationContext.ItemRemoved;
        }

        protected override IObservable<IConversationPresenter> GetFetchItemsObservable()
        {
            return _conversationContext.GetItems();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            foreach (var item in Items)
            {
                HandleNotViewedItem((IConversationPresenter)item.Model);
            }
        }

        private void MarkConversationItemAsViewed(IConversationPresenter item)
        {
            item.WasViewed = true;
            _conversationContext.SaveItem(item).SubscribeAndHandleErrors();
        }

        private void DismissConversationItemNotification(IPresenter item)
        {
            EventAggregator.PublishOnUIThread(new DismissNotification(item.UniqueId));
        }

        private void HandleNotViewedItem(IConversationPresenter item)
        {
            if (item.WasViewed || !IsActive)
            {
                return;
            }
            MarkConversationItemAsViewed(item);
            DismissConversationItemNotification(item);
        }

        #endregion
    }
}