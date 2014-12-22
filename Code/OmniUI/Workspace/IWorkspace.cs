namespace OmniUI.Workspace
{
    using Caliburn.Micro;

    public interface IWorkspace : IScreen, IConductor
    {
        string Background { get; set; }
    }
}