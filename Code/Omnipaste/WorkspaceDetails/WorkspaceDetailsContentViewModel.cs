namespace Omnipaste.WorkspaceDetails
{
    using OmniUI.Details;
    using OmniUI.Framework.Models;

    public class WorkspaceDetailsContentViewModel<TModel> : DetailsViewModelBase<TModel>, IWorkspaceDetailsContentViewModel
        where TModel : class, IModel
    {
    }
}