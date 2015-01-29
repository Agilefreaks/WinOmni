namespace Omnipaste.WorkspaceDetails.Version
{
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class VersionDetailsViewModel : DetailsViewModelWithHeaderBase<IWorkspaceDetailsHeaderViewModel, IWorkspaceDetailsContentViewModel>, IVersionDetailsViewModel
    {
        public VersionDetailsViewModel(IVersionDetailsHeaderViewModel headerViewModel, IVersionDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }
    }
}
