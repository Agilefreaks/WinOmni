namespace Omnipaste.ActivityDetails.Clipping
{
    using Caliburn.Micro;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class ClippingDetailsViewModel : ActivityDetailsViewModel, IClippingDetailsViewModel
    {
        public ClippingDetailsViewModel(
            IClippingDetailsHeaderViewModel headerViewModel,
            IClippingDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        protected override void OnDeactivate(bool close)
        {
            if (((IClippingDetailsHeaderViewModel)HeaderViewModel).State == ClippingDetailsHeaderStateEnum.Deleted)
            {
                if (!close)
                {
                    var parentConductor = Parent as IConductor;
                    if (parentConductor != null)
                    {
                        parentConductor.DeactivateItem(this, true);
                    }
                }
            }
            else
            {
                base.OnDeactivate(close);
            }
        }
    }
}