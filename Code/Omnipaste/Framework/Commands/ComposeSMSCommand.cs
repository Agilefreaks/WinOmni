namespace Omnipaste.Framework.Commands
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Conversations;
    using Omnipaste.Conversations.ContactList.Contact;
    using Omnipaste.Framework.Models;
    using Omnipaste.Shell;
    using OmniUI.Framework.Commands;
    using OmniUI.Workspaces;

    public class ComposeSMSCommand : IObservableCommand<Unit>
    {
        #region Constants

        private const int MaxRetryCount = 100;

        #endregion

        #region Static Fields

        private static readonly TimeSpan RetryInterval = TimeSpan.FromMilliseconds(50);

        #endregion

        public ComposeSMSCommand(ContactModel contact)
        {
            Contact = contact;
        }

        #region Public Properties

        public ContactModel Contact { get; private set; }

        [Inject]
        public IDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        [Inject]
        public IConversationWorkspace ConversationWorkspace { get; set; }

        [Inject]
        public IShellViewModel ShellViewModel { get; set; }

        [Inject]
        public IWorkspaceConductor WorkspaceConductor { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<Unit> Execute()
        {
            var dispatcherScheduler = SchedulerProvider.Dispatcher;
            var defaultScheduler = SchedulerProvider.Default;
            return
                Observable.Start(ShellViewModel.Show, defaultScheduler)
                    .Select(_ => Observable.Start(ActivatePeopleWorkspace, dispatcherScheduler))
                    .Switch()
                    .Select(_ => Observable.Start<IContactViewModel>(GetCorrespondingViewModel, defaultScheduler))
                    .Switch()
                    .RetryAfter(RetryInterval, MaxRetryCount, defaultScheduler)
                    .Select(viewModel => Observable.Start(viewModel.ShowDetails, dispatcherScheduler))
                    .Switch();
        }

        #endregion

        #region Methods

        private void ActivatePeopleWorkspace()
        {
            WorkspaceConductor.ActivateItem(ConversationWorkspace);
        }

        private IContactViewModel GetCorrespondingViewModel()
        {
            return
                ConversationWorkspace.ContactListViewModel.GetChildren()
                    .First(item => item.Model.BackingEntity.UniqueId == Contact.UniqueId);
        }

        #endregion
    }
}