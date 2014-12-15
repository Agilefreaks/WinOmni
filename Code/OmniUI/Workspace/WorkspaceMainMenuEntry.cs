namespace OmniUI.Workspace
{
    using System;
    using Caliburn.Micro;
    using Ninject;

    public abstract class WorkspaceMainMenuEntry<TWorkspace> : Screen, IDisposable
        where TWorkspace : class, IWorkspace
    {
        private TWorkspace _workspace;

        public virtual bool CanPerformAction
        {
            get
            {
                return true;
            }
        }

        public abstract string Icon { get; }

        public bool IsOpen
        {
            get
            {
                return Workspace.IsActive;
            }
        }

        public override string DisplayName
        {
            get
            {
                return Workspace.DisplayName;
            }
        }

        [Inject]
        public IWorkspaceConductor WorkspaceConductor { get; set; }

        [Inject]
        public TWorkspace Workspace
        {
            get
            {
                return _workspace;
            }
            set
            {
                UnregisterWorkspaceHandlers(_workspace);
                _workspace = value;
                RegisterWorkspaceHandlers(_workspace);
            }
        }

        public virtual void PerformAction()
        {
            WorkspaceConductor.ActivateItem(Workspace);
        }

        public void Dispose()
        {
            UnregisterWorkspaceHandlers(_workspace);
        }

        private void RegisterWorkspaceHandlers(TWorkspace workspace)
        {
            if (workspace != null)
            {
                workspace.Activated += UpdateViewValues;
                workspace.Deactivated += UpdateViewValues;
            }
            UpdateViewValues();
        }

        private void UnregisterWorkspaceHandlers(TWorkspace workspace)
        {
            if (workspace != null)
            {
                workspace.Activated -= UpdateViewValues;
                workspace.Deactivated -= UpdateViewValues;
            }
        }

        private void UpdateViewValues(object sender = null, EventArgs eventArgs = null)
        {
            NotifyOfPropertyChange(() => DisplayName);
            NotifyOfPropertyChange(() => IsOpen);
        }
    }
}
