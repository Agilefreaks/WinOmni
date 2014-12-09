namespace Omnipaste.ActivityDetails.Clipping
{
    using OmniUI.Attributes;

    [UseView("Omnipaste.ActivityDetails.ActivityDetailsView", IsFullyQualifiedName = true)]
    public class ClippingDetailsViewModel : ActivityDetailsViewModel
    {
        public ClippingDetailsViewModel(IClippingDetailsHeaderViewModel headerViewModel, IClippingDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }
    }
}