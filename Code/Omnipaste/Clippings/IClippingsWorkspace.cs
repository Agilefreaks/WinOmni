namespace Omnipaste.Clippings
{
    using Omnipaste.Clippings.ClippingList;
    using OmniUI.Workspaces;

    public interface IClippingsWorkspace : IMasterDetailsWorkspace
    {
        new IClippingListViewModel MasterScreen { get; }
    }
}