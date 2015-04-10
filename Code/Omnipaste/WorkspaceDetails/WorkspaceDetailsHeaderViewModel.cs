namespace Omnipaste.WorkspaceDetails
{
    using OmniUI.Details;
    using OmniUI.Framework.Models;

    public class WorkspaceDetailsHeaderViewModel<TModel> : DetailsViewModelBase<TModel>, IWorkspaceDetailsHeaderViewModel
        where TModel : class, IModel
    {
    }
}