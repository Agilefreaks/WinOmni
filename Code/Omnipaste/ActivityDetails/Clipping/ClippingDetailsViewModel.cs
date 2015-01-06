namespace Omnipaste.ActivityDetails.Clipping
{
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class ClippingDetailsViewModel : ActivityDetailsViewModel, IClippingDetailsViewModel
    {
        public ClippingDetailsViewModel(IClippingDetailsHeaderViewModel headerViewModel, IClippingDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }
    }
}