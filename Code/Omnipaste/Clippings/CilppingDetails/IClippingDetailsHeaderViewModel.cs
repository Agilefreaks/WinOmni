namespace Omnipaste.Clippings.CilppingDetails
{
    using OmniUI.Details;

    public interface IClippingDetailsHeaderViewModel : IDetailsViewModel
    {
        ClippingDetailsHeaderStateEnum State { get; set; }
    }
}