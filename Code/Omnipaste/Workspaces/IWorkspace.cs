namespace Omnipaste.Workspaces
{
    using Caliburn.Micro;

    public interface IWorkspace : IScreen, IConductor
    {
        string Icon { get; }
    }
}