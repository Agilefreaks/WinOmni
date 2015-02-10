namespace Omnipaste.Framework.Commands
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.Helpers;
    using Omnipaste.Presenters;
    using Omnipaste.Shell;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Framework.Commands;
    using OmniUI.Workspace;

    public class ComposeSMSCommand : IObservableCommand<Unit>
    {
        #region Constructors and Destructors

        public ComposeSMSCommand(IContactInfoPresenter contactInfo)
        {
            ContactInfo = contactInfo;
        }

        #endregion

        #region Public Properties

        public IContactInfoPresenter ContactInfo { get; private set; }

        [Inject]
        public IWorkspaceDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        [Inject]
        public IPeopleWorkspace PeopleWorkspace { get; set; }

        [Inject]
        public IWorkspaceConductor WorkspaceConductor { get; set; }

        [Inject]
        public IShellViewModel ShellViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<Unit> Execute()
        {
            return Observable.Start(
                () =>
                    {
                        ShellViewModel.Show();
                        WorkspaceConductor.ActivateItem(PeopleWorkspace);
                        var detailsViewModel = DetailsViewModelFactory.Create(ContactInfo);
                        PeopleWorkspace.DetailsConductor.ActivateItem(detailsViewModel);
                    },
                SchedulerProvider.Dispatcher);
        }

        #endregion
    }
}