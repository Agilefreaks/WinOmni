namespace Omnipaste.Activities.ActivityDetails.Version
{
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsWithHeaderView))]
    public class VersionDetailsViewModel : DetailsWithHeaderViewModelBase<IDetailsViewModel, IDetailsViewModel>, IVersionDetailsViewModel
    {
        public VersionDetailsViewModel(IVersionDetailsHeaderViewModel headerViewModel, IVersionDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }
    }
}
