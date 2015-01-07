namespace Omnipaste.ActivityDetails.Version
{
    using OmniUI.Attributes;

    [UseView(typeof(OmniUI.Details.DetailsViewWithHeader))]
    public class VersionDetailsViewModel : ActivityDetailsViewModel, IVersionDetailsViewModel
    {
        public VersionDetailsViewModel(IVersionDetailsHeaderViewModel headerViewModel, IVersionDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }
    }
}
