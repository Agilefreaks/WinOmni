namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Castle.Core.Internal;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Providers;
    using Omnipaste.WorkspaceDetails.Conversation.Call;
    using Omnipaste.WorkspaceDetails.Conversation.Message;
    using OmniUI.List;

    public class ConversationContentViewModel : ListViewModelBase<IConversationItem, IConversationItemViewModel>,
                                                IConversationContentViewModel
    {
        #region Fields

        private ContactInfoPresenter _model;

        #endregion

        public ConversationContentViewModel()
        {
            FilteredItems.SortDescriptions.Add(new SortDescription(default(IConversationItemViewModel).GetPropertyName(vm => vm.Time), ListSortDirection.Ascending));
        }

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
            return ConversationProvider.ForContact(Model.ContactInfo).ItemAdded;
        }

        protected override IObservable<IConversationItem> GetItemRemovedObservable()
        {
            return ConversationProvider.ForContact(Model.ContactInfo).ItemRemoved;
        }

        protected override void OnActivate()
        {
            Subscriptions.Add(
                ConversationProvider.ForContact(Model.ContactInfo)
                    .GetItems()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(items => items.ForEach(DisplayItem)));

            base.OnActivate();
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
            ConversationProvider.SaveItem(item).SubscribeAndHandleErrors();
        }

        private void DismissConversationItemNotification(IConversationItem item)
        {
            EventAggregator.PublishOnUIThread(new DismissNotification(item.UniqueId));
        }

        #endregion
    }
}