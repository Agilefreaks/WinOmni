namespace Omnipaste.Workspaces
{
    using Omnipaste.ClippingList;
    using OmniUI.Workspace;

    public interface IClippingWorkspace : IMasterDetailsWorkspace
    {
        new IClippingListViewModel MasterScreen { get; }
    }
}