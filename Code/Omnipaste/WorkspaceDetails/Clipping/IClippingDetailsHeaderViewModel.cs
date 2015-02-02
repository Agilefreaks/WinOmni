namespace Omnipaste.WorkspaceDetails.Clipping
{
    public interface IClippingDetailsHeaderViewModel : IWorkspaceDetailsHeaderViewModel
    {
        ClippingDetailsHeaderStateEnum State { get; set; }
    }
}