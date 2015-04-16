namespace OmniUI.Workspaces
{
    using Caliburn.Micro;

    public interface IMasterDetailsWorkspace : IScreen, IConductor
    {
        IScreen MasterScreen { get; }

        IDetailsConductorViewModel DetailsConductor { get; }

        string Background { get; set; }
    }
}