namespace Omnipaste.ActivityDetails.Clipping
{
    public interface IClippingDetailsHeaderViewModel : IActivityDetailsHeaderViewModel
    {
        ClippingDetailsHeaderStateEnum State { get; set; }
    }
}