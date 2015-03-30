namespace Omnipaste.WorkspaceDetails
{
    using OmniUI.Details;
    using OmniUI.Presenters;

    public class WorkspaceDetailsContentViewModel<TModel> : DetailsViewModelBase<TModel>, IWorkspaceDetailsContentViewModel
        where TModel : class, IPresenter
    {
    }
}