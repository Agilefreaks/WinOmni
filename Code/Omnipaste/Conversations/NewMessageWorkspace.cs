namespace Omnipaste.Conversations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework.Models;
    using Omnipaste.Properties;
    using OmniUI.Attributes;
    using OmniUI.Workspaces;

    [UseView(typeof(WorkspaceView))]
    public class NewMessageWorkspace : MasterDetailsWorkspace, INewMessageWorkspace
    {
        public static readonly TimeSpan SelectedContactAnimationDuration = TimeSpan.FromMilliseconds(500);

        private readonly IConversationHeaderViewModel _conversationHeaderViewModel;

        public override string DisplayName
        {
            get
            {
                return Resources.GroupMessage;
            }
        }

        public new IContactListViewModel MasterScreen { get; private set; }

        public IConversationViewModel ConversationViewModel { get; private set; }

        public NewMessageWorkspace(
            IContactListViewModel masterScreen,
            IConversationViewModel conversationViewModel,
            IDetailsConductorViewModel detailsConductor)
            : base(masterScreen, detailsConductor)
        {
            MasterScreen = masterScreen;
            ConversationViewModel = conversationViewModel;
            MasterScreen.CanSelectMultipleItems = false;
            DetailsConductor.ActiveItem = ConversationViewModel;
            _conversationHeaderViewModel = (IConversationHeaderViewModel)ConversationViewModel.HeaderViewModel;
            _conversationHeaderViewModel.State = ConversationHeaderStateEnum.Edit;
            CreateClearContactsSubscription();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            MasterScreen.SelectedContacts.CollectionChanged += OnSelectedContactsChanged;
            if (DetailsConductor.ActiveItem == null)
            {
                DetailsConductor.ActivateItem(_conversationHeaderViewModel);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            MasterScreen.SelectedContacts.CollectionChanged -= OnSelectedContactsChanged;
            HandleDeletedConversation();
        }

        private void HandleDeletedConversation()
        {
            if (ConversationViewModel.State != ConversationViewModelStateEnum.Deleted)
            {
                return;
            }

            MasterScreen.SelectedContacts.Clear();
            _conversationHeaderViewModel.State = ConversationHeaderStateEnum.Edit;
            ConversationViewModel.Recipients = new ObservableCollection<ContactModel>();
            ConversationViewModel.State = ConversationViewModelStateEnum.Normal;
        }

        private void CreateClearContactsSubscription()
        {
            Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => ((NotifyCollectionChangedEventHandler)((sender, eventArgs) => handler(eventArgs))),
                handler => MasterScreen.SelectedContacts.CollectionChanged += handler,
                handler => MasterScreen.SelectedContacts.CollectionChanged -= handler)
                .Throttle(SelectedContactAnimationDuration)
                .Subscribe(entry => ClearContactSelection());
        }

        private void OnSelectedContactsChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.NewItems == null)
            {
                return;
            }

            var contactModels = eventArgs.NewItems.OfType<ContactModel>();
            AddRecipients(contactModels);
        }

        private void ClearContactSelection()
        {
            MasterScreen.SelectedContacts.Clear();
        }

        private void AddRecipients(IEnumerable<ContactModel> contactModels)
        {
            foreach (var item in contactModels.Where(contact => !RecipientExists(contact)))
            {
                _conversationHeaderViewModel.Recipients.Add(item);
            }
        }

        private bool RecipientExists(ContactModel contact)
        {
            return _conversationHeaderViewModel.Recipients.Any(
                recipient => recipient.PhoneNumber == contact.PhoneNumber);
        }
    }
}