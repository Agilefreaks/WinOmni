namespace Omnipaste.ActivityDetails.Clipping
{
    using OmniUI.Attributes;

    [UseView("OmniUI.Details.DetailsViewWithHeader", IsFullyQualifiedName = true)]
    public class ClippingDetailsViewModel : ActivityDetailsViewModel, IClippingDetailsViewModel
    {
        public ClippingDetailsViewModel(IClippingDetailsHeaderViewModel headerViewModel, IClippingDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }
    }
}