namespace Omnipaste.ActivityDetails.Clipping
{
    using OmniUI.Attributes;

    [UseView(typeof(OmniUI.Details.DetailsViewWithHeader))]
    public class ClippingDetailsViewModel : ActivityDetailsViewModel, IClippingDetailsViewModel
    {
        public ClippingDetailsViewModel(IClippingDetailsHeaderViewModel headerViewModel, IClippingDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }
    }
}