namespace Omnipaste.Shell.SessionInfo
{
    using System;
    using Omnipaste.Profile;
    using OmniUI.Workspaces;

    public interface ISessionInfoViewModel : IDisposable
    {
        IWorkspaceConductor WorkspaceConductor { get; set; }

        IProfileWorkspace ProfileWorkspace { get; set; }

        void ShowUserProfile();
    }
}