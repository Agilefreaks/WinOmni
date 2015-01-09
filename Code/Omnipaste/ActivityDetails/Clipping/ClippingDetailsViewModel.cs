namespace Omnipaste.ActivityDetails.Clipping
{
    using Caliburn.Micro;
    using Omnipaste.Services.Repositories;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class ClippingDetailsViewModel : ActivityDetailsViewModel, IClippingDetailsViewModel
    {
        private readonly IClippingRepository _clippingRepository;

        public ClippingDetailsViewModel(
            IClippingDetailsHeaderViewModel headerViewModel,
            IClippingDetailsContentViewModel contentViewModel,
            IClippingRepository clippingRepository)
            : base(headerViewModel, contentViewModel)
        {
            _clippingRepository = clippingRepository;
        }

        protected override void OnDeactivate(bool close)
        {
            if (Model.MarkedForDeletion)
            {
                if (!close)
                {
                    var parentConductor = Parent as IConductor;
                    if (parentConductor != null)
                    {
                        parentConductor.DeactivateItem(this, true);
                    }
                }
                else
                {
                    _clippingRepository.Delete(Model.SourceId);
                }
            }
            else
            {
                base.OnDeactivate(close);
            }
        }
    }
}