namespace Omnipaste.Activity
{
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Presenters;

    public interface IActivityViewModel : IDetailsViewModelWithAutoRefresh<ActivityPresenter>
    {
        ContentTypeEnum ContentType { get; set; }

        ActivityContentInfo ContentInfo { get; set; }
    }
}