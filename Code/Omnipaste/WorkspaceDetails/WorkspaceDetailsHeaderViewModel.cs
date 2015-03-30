namespace Omnipaste.WorkspaceDetails
{
    using OmniUI.Details;
    using OmniUI.Presenters;

    public class WorkspaceDetailsHeaderViewModel<TModel> : DetailsViewModelBase<TModel>, IWorkspaceDetailsHeaderViewModel
        where TModel : class, IPresenter
    {
    }
}