namespace Omnipaste.Clippings.CilppingDetails
{
    using Caliburn.Micro;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsWithHeaderView))]
    public class ClippingDetailsViewModel : DetailsWithHeaderViewModelBase<IDetailsViewModel, IDetailsViewModel>, IClippingDetailsViewModel
    {
        public ClippingDetailsViewModel(
            IClippingDetailsHeaderViewModel headerViewModel,
            IClippingDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        protected override void OnDeactivate(bool close)
        {
            if (((IClippingDetailsHeaderViewModel)HeaderViewModel).State == ClippingDetailsHeaderStateEnum.Deleted && !close)
            {
                ((IConductor)Parent).DeactivateItem(this, true);
            }
            else
            {
                base.OnDeactivate(close);
            }
        }
    }
}