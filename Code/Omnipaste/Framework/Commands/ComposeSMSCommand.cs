namespace Omnipaste.Framework.Commands
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.ContactList;
    using Omnipaste.ContactList.Contact;
    using Omnipaste.Models;
    using Omnipaste.Shell;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces.People;
    using OmniUI.Framework.Commands;
    using OmniUI.Workspace;

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
        public IWorkspaceDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        [Inject]
        public IPeopleWorkspace PeopleWorkspace { get; set; }

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
            WorkspaceConductor.ActivateItem(PeopleWorkspace);
        }

        private IContactViewModel GetCorrespondingViewModel()
        {
            return
                PeopleWorkspace.MasterScreen.GetChildren()
                    .First(item => item.Model.BackingEntity.UniqueId == Contact.UniqueId);
        }

        #endregion
    }
}