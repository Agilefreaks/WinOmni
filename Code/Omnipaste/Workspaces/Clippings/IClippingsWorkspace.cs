namespace Omnipaste.Workspaces.Clippings
{
    using Omnipaste.ClippingList;
    using OmniUI.Workspace;

    public interface IClippingsWorkspace : IMasterDetailsWorkspace
    {
        new IClippingListViewModel MasterScreen { get; }
    }
}