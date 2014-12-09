namespace Omnipaste.Workspaces
{
    using Caliburn.Micro;

    public interface IMasterDetailsWorkspace : IWorkspace
    {
        IScreen MasterScreen { get; }

        IDetailsConductorViewModel DetailsConductor { get; }
    }
}