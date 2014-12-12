namespace Omnipaste.Activity
{
    using Omnipaste.DetailsViewModel;

    public interface IActivityViewModel : IDetailsViewModelWithAutoRefresh<Models.Activity>
    {
        ContentTypeEnum ContentType { get; set; }
    }
}